using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Vehicle.Aplication.Interfaces;
using Vehicle.Aplication.Services.Database;
using Vehicle.Domain.Entities;
using Entities = Vehicle.Domain.Entities;

namespace Vehicle.Infrastructure.Database
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly VehicleContext _dbContext;
        private readonly ILogger<VehicleRepository> _logger;

        public VehicleRepository(VehicleContext dbContext,
            ILogger<VehicleRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Entities.Vehicle>> Get()
        {
            _logger.LogInformation($"RetrieveVehicleAsync");

            return await _dbContext.Vehicles.Include(x => x.Category).AsNoTracking().ToListAsync();
        }

        public async Task<Entities.Vehicle?> Get(int idVehicle)
        {
            _logger.LogInformation($"RetrieveVehicleById-{idVehicle}");

            return await _dbContext.Vehicles.FindAsync(idVehicle);
        }

        public async Task<int> Create(Entities.Vehicle vehicle)
        {
            _logger.LogInformation($"InsertVehicleAsync - {JsonSerializer.Serialize(vehicle)}");

            _dbContext.Vehicles.Add(vehicle);
            await _dbContext.SaveChangesAsync();
            return vehicle.Id;
        }

        public async Task<int> Update(Entities.Vehicle vehicle)
        {
            _logger.LogInformation($"UpdateVehicleAsync - {JsonSerializer.Serialize(vehicle)}");

            _dbContext.Vehicles.Update(vehicle);
            await _dbContext.SaveChangesAsync();
            return vehicle.Id;
        }


        public async Task<int> Delete(int idVehicle)
        {
            _logger.LogInformation($"DeleteVehicleAsync - {idVehicle}");

            var vehicle = await _dbContext.Vehicles.FindAsync(idVehicle);
            _dbContext.Vehicles.Remove(vehicle);
            await _dbContext.SaveChangesAsync();

            return idVehicle;
        }

        public async Task UpdateCategoryVehiclesToPreviousCategory(int idCategoryDeleted, int previousCategory)
        {
            _logger.LogInformation($"UpdateCategoryVehiclesToPreviousCategory - {new { idCategoryDeleted, previousCategory }}");

            await _dbContext.Vehicles
                    .Where(x => x.CategoryId == idCategoryDeleted)
                    .ExecuteUpdateAsync(x => x
                        .SetProperty(p => p.CategoryId, previousCategory));
        }

        public async Task UpdateCategoryVehiclesWithWeightInNewCategory(decimal minWeightPreviousCategory, int idNewCategory)
        {
            _logger.LogInformation($"UpdateCategoryVehiclesWithWeightInNewCategory - {new { minWeightPreviousCategory, idNewCategory }}");

            await _dbContext.Vehicles
                    .Where(x => x.WeightKg >= minWeightPreviousCategory)
                    .ExecuteUpdateAsync(x => x
                        .SetProperty(p => p.CategoryId, idNewCategory));
        }
    }
}
