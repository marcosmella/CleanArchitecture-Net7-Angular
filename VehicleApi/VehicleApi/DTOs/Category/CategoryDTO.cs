namespace VehicleApi.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string IconUrl { get; set; } = null!;

        public decimal MinWeightKg { get; set; }

        public decimal MaxWeightKg { get; set; }
    }
}
