using Vehicle.Domain.Entities;

namespace Vehicle.Aplication.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> Get();
        Task<Category> Get(int idCategory);
        Task<int> Create(Category category);
        Task<int> Update(Category category);
        Task<int> Delete(int idCategory);
        Task<int> GetCategoryByWeight(decimal weight);

    }
}
