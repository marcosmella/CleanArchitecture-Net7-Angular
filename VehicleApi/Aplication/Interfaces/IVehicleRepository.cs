using Entities = Vehicle.Domain.Entities;

namespace Vehicle.Aplication.Interfaces
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Entities.Vehicle>> Get();
        Task<Entities.Vehicle?> Get(int idVehicle);
        Task<int> Create(Entities.Vehicle vehicle);
        Task<int> Update(Entities.Vehicle vehicle);
        Task<int> Delete(int idVehicle);
        Task UpdateCategoryVehiclesToPreviousCategory(int idCategoryDeleted, int previousCategory);
        Task UpdateCategoryVehiclesWithWeightInNewCategory(decimal minWeightPreviousCategory, int idNewCategory);
    }
}
