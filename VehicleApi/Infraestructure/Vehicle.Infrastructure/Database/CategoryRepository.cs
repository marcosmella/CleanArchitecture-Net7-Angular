using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Vehicle.Aplication.Interfaces;
using Vehicle.Aplication.Services.Database;
using Vehicle.Domain.Entities;

namespace Vehicle.Infrastructure.Database
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly VehicleContext _dbContext;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(VehicleContext dbContext,
            ILogger<CategoryRepository> logger) 
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> Get()
        {
            _logger.LogInformation($"RetrieveCategoryAsync");

            return await _dbContext.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category> Get(int idCategory)
        {
            _logger.LogInformation($"RetrieveCategoryById - {idCategory}");

            return await _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idCategory);
        }

        public async Task<int> Create(Category category)
        {
            _logger.LogInformation($"InsertCategoryAsync - {JsonSerializer.Serialize(category)}");

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            return category.Id;
        }

        public async Task<int> Update(Category category)
        {
            _logger.LogInformation($"UpdateCategoryAsync - {JsonSerializer.Serialize(category)}");

            _dbContext.Entry(category).State = EntityState.Detached;
            _dbContext.Set<Category>().Update(category);
            await _dbContext.SaveChangesAsync();
            return category.Id;
        }

        public async Task<int> Delete(int idCategory)
        {
            _logger.LogInformation($"DeleteVehicleAsync - {idCategory}");

            var category = await _dbContext.Categories.FindAsync(idCategory);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            return idCategory;
        }

        public async Task<int> GetCategoryByWeight(decimal weight)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.MaxWeightKg >= weight && x.MinWeightKg <= weight);
            return category!= null ? category.Id : 0;
        }
    }
}
