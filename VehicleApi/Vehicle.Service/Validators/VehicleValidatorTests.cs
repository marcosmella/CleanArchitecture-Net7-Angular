using Vehicle.Aplication.Validators;
using Entities = Vehicle.Domain.Entities;

namespace Vehicle.Service.Test.Validators
{
    [TestClass]
    public class VehicleValidatorTests
    {
        [TestMethod]
        public void Validate_ValidVehicle_ShouldPass()
        {
            // Arrange
            var validator = new VehicleValidator();
            var validVehicle = new Entities.Vehicle
            {
                OwnerName = "John Doe",
                Manufacturer = "Toyota",
                YearOfManufacture = 2022,
                WeightKg = 1500
            };

            // Act
            var result = validator.Validate(validVehicle);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_InvalidOwnerName_ShouldFail()
        {
            // Arrange
            var validator = new VehicleValidator();
            var invalidVehicle = new Entities.Vehicle
            {
                OwnerName = "", // Invalid OwnerName (empty)
                Manufacturer = "Toyota",
                YearOfManufacture = 2022,
                WeightKg = 1500
            };

            // Act
            var result = validator.Validate(invalidVehicle);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Is required", result.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void Validate_InvalidYearOfManufacture_ShouldFail()
        {
            // Arrange
            var validator = new VehicleValidator();
            var invalidVehicle = new Entities.Vehicle
            {
                OwnerName = "John Doe",
                Manufacturer = "Toyota",
                YearOfManufacture = DateTime.Now.Year + 1, 
                WeightKg = 1500
            };

            // Act
            var result = validator.Validate(invalidVehicle);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Year of manufacture must before or equal " + DateTime.Now.Year, result.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void Validate_InvalidWeightKg_ShouldFail()
        {
            // Arrange
            var validator = new VehicleValidator();
            var invalidVehicle = new Entities.Vehicle
            {
                OwnerName = "John Doe",
                Manufacturer = "Toyota",
                YearOfManufacture = 2022,
                WeightKg = 0
            };

            // Act
            var result = validator.Validate(invalidVehicle);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Is required", result.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void Validate_InvalidManufacturer_ShouldFail()
        {
            // Arrange
            var validator = new VehicleValidator();
            var invalidVehicle = new Entities.Vehicle
            {
                OwnerName = "John Doe",
                Manufacturer = "",
                YearOfManufacture = 2022,
                WeightKg = 1500
            };

            // Act
            var result = validator.Validate(invalidVehicle);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Is required", result.Errors[0].ErrorMessage);
        }       

        [TestMethod]
        public void Validate_ValidVehicleWithDecimalWeight_ShouldPass()
        {
            // Arrange
            var validator = new VehicleValidator();
            var validVehicle = new Entities.Vehicle
            {
                OwnerName = "John Doe",
                Manufacturer = "Toyota",
                YearOfManufacture = 2022,
                WeightKg = 1500.50m 
            };

            // Act
            var result = validator.Validate(validVehicle);

            // Assert
            Assert.IsTrue(result.IsValid);
        }
    }
}
