using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Vehicle.Aplication.CustomExceptions;
using Vehicle.Aplication.Interfaces;
using Vehicle.Aplication.Validators;
using Vehicle.Domain.Entities;

namespace Vehicle.Aplication.Services.Database
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<CategoryService> _logger;

        private readonly IValidator<Category> _categoryValidator;
        private readonly decimal _MAX_WEIGHT = 9999999999999999;
        public CategoryService(ICategoryRepository categoryRepository,
                            IVehicleService vehicleService,
                            ILogger<CategoryService> logger,
                            IValidator<Category> categoryValidator)
        {
            _categoryRepository = categoryRepository;
            _vehicleService = vehicleService;
            _categoryValidator = categoryValidator;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> Get()
        {
            _logger.LogInformation($"RetrieveCategoryAsync");

            return await _categoryRepository.Get();

        }

        public async Task<Category> Get(int idCategory)
        {
            _logger.LogInformation($"RetrieveCategoryById - {idCategory}");

            var vehicle = await _categoryRepository.Get(idCategory);
            return vehicle;
        }

        public async Task<int> Create(Category category)
        {
            _logger.LogInformation($"InsertCategoryAsync - {JsonSerializer.Serialize(category)}");
            await categoryValidations(category);

            category.MaxWeightKg = _MAX_WEIGHT;
            await _categoryRepository.Create(category);
            await _vehicleService.UpdateCategoryVehiclesWithWeightInNewCategory(category.MinWeightKg, category.Id);
            await WithoutGapsWithPreviousCategory(category);

            return category.Id;
        }

        public async Task<int> Update(Category category)
        {
            _logger.LogInformation($"UpdateCategoryAsync - {JsonSerializer.Serialize(category)}");

            await categoryValidations(category);

            return await _categoryRepository.Update(category);
        }

        public async Task<int> Delete(int idCategory)
        {
            _logger.LogInformation($"DeleteVehicleAsync - {idCategory}");

            var categories = await HasEnoughtCategories();
            var categoryForOrphanVehicles = GetPreviousOrNextCategory(idCategory, categories);
            
            await _vehicleService.UpdateCategoryVehiclesToPreviousCategory(idCategory, categoryForOrphanVehicles.Id);
            await _categoryRepository.Update(categoryForOrphanVehicles);
            return await _categoryRepository.Delete(idCategory);
        }

        public async Task<int> GetCategoryByWeight(decimal weight)
        {
            return await _categoryRepository.GetCategoryByWeight(weight);
        }

        private async Task categoryValidations(Category category)
        {
            var validationResult = await _categoryValidator.ValidateAsync(category);
            if (!validationResult.IsValid)
            {
                throw new MyCustomException(validationResult);
            }
        }

        private async Task WithoutGapsWithPreviousCategory(Category category)
        {
            var categories = await Get();
            var previousCategory = categories.OrderByDescending(x => x.MinWeightKg).SkipWhile(x => x.Id != category.Id).Skip(1).FirstOrDefault();
            previousCategory.MaxWeightKg = category.MinWeightKg - 0.01m;
            await _categoryRepository.Update(previousCategory);
        }

        private async Task<IEnumerable<Category>> HasEnoughtCategories()
        {
            var categories = await Get();
            if (categories.Count() == 1)
            {
                throw new MyCustomException("It is the last category, you cannot delete it");
            }

            return categories;
        }
        private Category GetPreviousOrNextCategory(int idCategoryDeleted, IEnumerable<Category> categories) 
        {
            var categoryDeleted = categories.Where(x => idCategoryDeleted == x.Id).FirstOrDefault();
            var previousCategory = categories.OrderByDescending(x => x.MaxWeightKg).SkipWhile(x => x.Id != idCategoryDeleted).Skip(1).FirstOrDefault();
            var nextCategory = categories.OrderBy(x => x.MaxWeightKg).SkipWhile(x => x.Id != idCategoryDeleted).Skip(1).FirstOrDefault();
            setPreviousProperties(previousCategory, categoryDeleted);
            setNextProperties(nextCategory, categoryDeleted);

            return previousCategory ?? nextCategory;
        }

        private void setPreviousProperties(Category previousCategory, Category categoryDeleted)
        {
            if(previousCategory is not null)
            { 
                previousCategory.MaxWeightKg = categoryDeleted.MaxWeightKg;
            }
        }

        private void setNextProperties(Category nextCategory, Category categoryDeleted)
        {
            if (nextCategory is not null)
            {
                nextCategory.MinWeightKg = categoryDeleted.MinWeightKg;
            }
        }


    }
}
