namespace Vehicle.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string IconUrl { get; set; } = null!;

        public decimal MinWeightKg { get; set; }

        public decimal MaxWeightKg { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
