using Microsoft.Extensions.Logging;
using System.Text.Json;
using Vehicle.Aplication.CustomExceptions;
using Vehicle.Aplication.Interfaces;
using Vehicle.Aplication.Validators;
using Vehicle.Domain.Entities;
using Entities = Vehicle.Domain.Entities;

namespace Vehicle.Aplication.Services.Database
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<VehicleService> _logger;
        private readonly VehicleValidator _vehicleValidator;

        public VehicleService(IVehicleRepository vehicleRepository,
            ICategoryRepository categoryRepository,
            VehicleValidator vehicleValidator,
            ILogger<VehicleService> logger) 
        {
            _vehicleRepository = vehicleRepository;
            _categoryRepository = categoryRepository;
            _vehicleValidator = vehicleValidator;
            _logger = logger;
        }

        public async Task<IEnumerable<Entities.Vehicle>> Get()
        {
            _logger.LogInformation($"RetrieveVehicleAsync");

            return await _vehicleRepository.Get();
        }

        public async Task<Entities.Vehicle> Get(int idVehicle)
        {
            _logger.LogInformation($"RetrieveVehicleById-{idVehicle}");

            var vehicle = await _vehicleRepository.Get(idVehicle);
            return vehicle;            
        }

        public async Task<int> Create(Entities.Vehicle vehicle)
        {
            _logger.LogInformation($"InsertVehicleAsync - {JsonSerializer.Serialize(vehicle)}");

            await VehicleValidations(vehicle);

            vehicle.CategoryId = await GetCategoryByWeight(vehicle.WeightKg);
            return await _vehicleRepository.Create(vehicle);
        }

        public async Task<int> Update(Entities.Vehicle vehicle)
        {
            _logger.LogInformation($"UpdateVehicleAsync - {JsonSerializer.Serialize(vehicle)}");

            await VehicleValidations(vehicle);

            vehicle.CategoryId = await GetCategoryByWeight(vehicle.WeightKg); 
            return await _vehicleRepository.Update(vehicle);
        }

        public async Task<int> Delete(int idVehicle)
        {
            _logger.LogInformation($"DeleteVehicleAsync - {idVehicle}");

            return await _vehicleRepository.Delete(idVehicle);
        }

        public async Task UpdateCategoryVehiclesToPreviousCategory(int idCategoryDeleted, int previousCategory)
        {
            _logger.LogInformation($"UpdateCategoryVehiclesToPreviousCategory - { new { idCategoryDeleted, previousCategory }}");

            await _vehicleRepository.UpdateCategoryVehiclesToPreviousCategory(idCategoryDeleted, previousCategory);
        }

        public async Task UpdateCategoryVehiclesWithWeightInNewCategory(decimal minWeightPreviousCategory, int idNewCategory)
        {
            _logger.LogInformation($"UpdateCategoryVehiclesWithWeightInNewCategory - {new { minWeightPreviousCategory, idNewCategory }}");

            await _vehicleRepository.UpdateCategoryVehiclesWithWeightInNewCategory(minWeightPreviousCategory, idNewCategory);
        }

        private async Task VehicleValidations(Entities.Vehicle vehicle)
        {
            var validationResult = await _vehicleValidator.ValidateAsync(vehicle);
            if (!validationResult.IsValid)
            {
                throw new MyCustomException(validationResult);
            }
        }

        private async Task<int> GetCategoryByWeight(decimal weightKg)
        {
            var categoryWeight = await _categoryRepository.GetCategoryByWeight(weightKg);
            if (categoryWeight == 0)
            {
                throw new MyCustomException("This Weight doesn't have category");
            }
            return categoryWeight;
        }
    }
}
