namespace VehicleApi.DTOs
{
    public class VehicleDTO
    {
        public int Id { get; set; }
        public string OwnerName { get; set; } = null!;

        public string Manufacturer { get; set; } = null!;

        public int YearOfManufacture { get; set; }

        public decimal WeightKg { get; set; }

        public int CategoryId { get; set; }

        public CategoryDTO? Category { get; set; } 
    }
}
