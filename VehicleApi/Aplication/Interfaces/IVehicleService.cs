using Vehicle.Domain.Entities;
using Entities = Vehicle.Domain.Entities;

namespace Vehicle.Aplication.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<Entities.Vehicle>> Get();
        Task<Entities.Vehicle> Get(int idVehicle);
        Task<int> Create(Entities.Vehicle vehicle);
        Task<int> Update(Entities.Vehicle vehicle);
        Task<int> Delete(int idVehicle);
        Task UpdateCategoryVehiclesToPreviousCategory(int idCategoryDeleted, int previuosCategory);
        Task UpdateCategoryVehiclesWithWeightInNewCategory(decimal minWeightPreviousCategory, int idNewCategory);

    }
}
