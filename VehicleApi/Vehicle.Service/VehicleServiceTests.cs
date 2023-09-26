using AutoMoqCore;
using Vehicle.Aplication.Interfaces;
using Vehicle.Aplication.Services.Database;
using Vehicle.Aplication.Validators;
using Entities = Vehicle.Domain.Entities;
using Moq;
using FluentValidation.Results;
using Vehicle.Aplication.CustomExceptions;
using FluentValidation;

namespace Vehicle.Service.Test
{
    [TestClass]
    public class VehicleServiceTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IVehicleService _vehicleService;

        public VehicleServiceTests()
        {
            _autoMoqer = new AutoMoqer();
            _vehicleService = _autoMoqer.Resolve<VehicleService>();
        }

        [TestMethod]
        public async Task GetAsync()
        {
            //ARRANGE
            IEnumerable<Entities.Vehicle> Vehicles = new List<Entities.Vehicle>
            {
                new Entities.Vehicle
                {
                    Id = 1,
                    OwnerName = "Toyota",
                    Manufacturer = "A45F222A",
                    YearOfManufacture = 2015,
                    WeightKg = 1000,
                    CategoryId = 5
                },
                new Entities.Vehicle
                {
                   Id = 1,
                    OwnerName = "Nissan",
                    Manufacturer = "A45F222A",
                    YearOfManufacture = 2020,
                    WeightKg = 1000,
                    CategoryId = 5
                },
            };

            var mockVehicleRepository = _autoMoqer.GetMock<IVehicleRepository>();
            mockVehicleRepository.Setup(x => x.Get())
                .ReturnsAsync(Vehicles);

            //ACT
            var result = await _vehicleService.Get();

            //ASSERT
            for (int i = 0; i < Vehicles.Count(); i++)
            {
                Assert.AreEqual(result.ElementAt(i).Id, Vehicles.ElementAt(i).Id);
                Assert.AreEqual(result.ElementAt(i).OwnerName, Vehicles.ElementAt(i).OwnerName);
                Assert.AreEqual(result.ElementAt(i).Manufacturer, Vehicles.ElementAt(i).Manufacturer);
                Assert.AreEqual(result.ElementAt(i).YearOfManufacture, Vehicles.ElementAt(i).YearOfManufacture);
                Assert.AreEqual(result.ElementAt(i).WeightKg, Vehicles.ElementAt(i).WeightKg);
                Assert.AreEqual(result.ElementAt(i).CategoryId, Vehicles.ElementAt(i).CategoryId);

            }
        }

        [TestMethod]
        public async Task Get_WhenRepositoryReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var vehiclesEmptyList = new List<Entities.Vehicle>();

            var mockVehicleRepository = _autoMoqer.GetMock<IVehicleRepository>();
            mockVehicleRepository.Setup(x => x.Get())
                .ReturnsAsync(vehiclesEmptyList);

            // Act
            var result = await _vehicleService.Get();

            // Assert
            Assert.AreEqual(result, vehiclesEmptyList);
        }

        [TestMethod]
        public async Task GetByIdAsync()
        {
            //ARRANGE
            int id = 5;
            var vehicle = new Entities.Vehicle
            {
                Id = 5,
                OwnerName = "Toyota",
                Manufacturer = "A45F222A",
                YearOfManufacture = 2015,
                WeightKg = 1000,
                CategoryId = 5
            };

            var mockVehicleRepository = _autoMoqer.GetMock<IVehicleRepository>();
            mockVehicleRepository.Setup(x => x.Get(id))
                .ReturnsAsync(vehicle);

            //ACT
            var result = await _vehicleService.Get(id);

            //ASSERT            
            Assert.AreEqual(result.Id, vehicle.Id);
            Assert.AreEqual(result.OwnerName, vehicle.OwnerName);
            Assert.AreEqual(result.Manufacturer, vehicle.Manufacturer);
            Assert.AreEqual(result.YearOfManufacture, vehicle.YearOfManufacture);
            Assert.AreEqual(result.WeightKg, vehicle.WeightKg);
            Assert.AreEqual(result.CategoryId, vehicle.CategoryId);

            
        }

        [TestMethod]
        public async Task Get_WhenRepositoryReturnsEmptyObject_ReturnsEmptyObject()
        {
            // Arrange
            var id = 5987;
            var vehiclesEmpty = new Entities.Vehicle();

            var mockVehicleRepository = _autoMoqer.GetMock<IVehicleRepository>();
            mockVehicleRepository.Setup(x => x.Get(id))
                .ReturnsAsync(vehiclesEmpty);

            // Act
            var result = await _vehicleService.Get(id);

            // Assert
            Assert.AreEqual(result, vehiclesEmpty);
        }

        [TestMethod]
        public async Task Create_ValidVehicle_ReturnsId()
        {
            // Arrange
            var idCategory = 5;
            var weight = 1000;
            var validVehicle = new Entities.Vehicle
            {
                Id = 5,
                OwnerName = "Toyota",
                Manufacturer = "A45F222A",
                YearOfManufacture = 2015,
                WeightKg = weight,
                CategoryId = idCategory
            };

             _autoMoqer.GetMock<VehicleValidator>()
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<Entities.Vehicle>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _autoMoqer.GetMock<ICategoryRepository>()
                .Setup(c => c.GetCategoryByWeight(It.IsAny<decimal>()))
                .ReturnsAsync(idCategory); 

            _autoMoqer.GetMock<IVehicleRepository>()
                .Setup(v => v.Create(It.IsAny<Entities.Vehicle>()))
                .ReturnsAsync(validVehicle.Id);

            // Act
            var result = await _vehicleService.Create(validVehicle);

            // Assert
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public async Task Create_InvalidVehicle_ThrowsCustomException()
        {
            // Arrange
            var invalidVehicle = new Entities.Vehicle
            {
                Id = 0,
                OwnerName = "",
                Manufacturer = "test",
                YearOfManufacture = 2015,
                WeightKg = 1000,
                CategoryId = 5
            };

            _autoMoqer.GetMock<VehicleValidator>()
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<Entities.Vehicle>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("OwnerName", "Is required")
                })); 
            

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<MyCustomException>(() => _vehicleService.Create(invalidVehicle));
            StringAssert.Contains("OwnerName - Is required", exception.Errors.First());
        }

        [TestMethod]
        public async Task Update_ValidVehicle_ReturnsId()
        {
            // Arrange
            var idCategory = 5;
            var weight = 1000;
            var validVehicle = new Entities.Vehicle
            {
                Id = 5,
                OwnerName = "Toyota",
                Manufacturer = "TestData",
                YearOfManufacture = 2015,
                WeightKg = weight,
                CategoryId = idCategory
            };

            var asd = _autoMoqer.GetMock<VehicleValidator>()
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<Entities.Vehicle>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _autoMoqer.GetMock<ICategoryRepository>()
                .Setup(c => c.GetCategoryByWeight(It.IsAny<decimal>()))
                .ReturnsAsync(idCategory);

            _autoMoqer.GetMock<IVehicleRepository>()
                .Setup(v => v.Update(It.IsAny<Entities.Vehicle>()))
                .ReturnsAsync(validVehicle.Id);

            // Act
            var result = await _vehicleService.Update(validVehicle);

            // Assert
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public async Task Update_InvalidVehicle_ThrowsCustomException()
        {
            // Arrange
            var invalidVehicle = new Entities.Vehicle
            {
                Id = 5,
                OwnerName = "",
                Manufacturer = "test",
                YearOfManufacture = 2015,
                WeightKg = 1000,
                CategoryId = 5
            };

            _autoMoqer.GetMock<VehicleValidator>()
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<Entities.Vehicle>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("OwnerName", "Is required")
                }));


            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<MyCustomException>(() => _vehicleService.Update(invalidVehicle));
            StringAssert.Contains("OwnerName - Is required", exception.Errors.First());


        }

        [TestMethod]
        public async Task Delete_ValidId_CallsRepository()
        {
            // Arrange
            var validId = 1;

            _autoMoqer.GetMock<IVehicleRepository>()
                .Setup(v => v.Delete(It.IsAny<int>()))
                .ReturnsAsync(1);

            // Act
            var result = await _vehicleService.Delete(validId);

            // Assert
            Assert.AreEqual(1, result);
            _autoMoqer.GetMock<IVehicleRepository>().Verify(r => r.Delete(validId), Times.Once);
        }

        [TestMethod]
        public async Task Delete_InvalidId_ReturnsZero()
        {
            // Arrange
            var invalidId = -1; // Assume an invalid vehicle id

            _autoMoqer.GetMock<IVehicleRepository>()
                .Setup(v => v.Delete(It.IsAny<int>()))
                .ReturnsAsync(0);

            // Act
            var result = await _vehicleService.Delete(invalidId);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public async Task UpdateCategoryVehiclesToPreviousCategory_HappyPath_CallsRepository()
        {
            // Arrange

            var idCategoryDeleted = 1;
            var previousCategory = 2;

            _autoMoqer.GetMock<IVehicleRepository>()
                .Setup(v => v.UpdateCategoryVehiclesToPreviousCategory(It.IsAny<int>(), It.IsAny<int>())); 

            // Act
            await _vehicleService.UpdateCategoryVehiclesToPreviousCategory(idCategoryDeleted, previousCategory);

            // Assert
            _autoMoqer.GetMock<IVehicleRepository>()
                .Verify(v => v.UpdateCategoryVehiclesToPreviousCategory(idCategoryDeleted, previousCategory), Times.Once);
        }

        [TestMethod]
        public async Task UpdateCategoryVehiclesToPreviousCategory_ThrowsException()
        {
            // Arrange
            var mocker = new AutoMoqer();
            var vehicleService = mocker.Create<VehicleService>();

            var idCategoryDeleted = 1;
            var previousCategory = 2;

            mocker.GetMock<IVehicleRepository>()
                .Setup(v => v.UpdateCategoryVehiclesToPreviousCategory(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Some error occurred.")); // Simulate an error

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => vehicleService.UpdateCategoryVehiclesToPreviousCategory(idCategoryDeleted, previousCategory));
        }

        [TestMethod]
        public async Task UpdateCategoryVehiclesWithWeightInNewCategory_HappyPath()
        {
            // Arrange

            _autoMoqer.GetMock<IVehicleRepository>()
                .Setup(v => v.UpdateCategoryVehiclesWithWeightInNewCategory(It.IsAny<decimal>(), It.IsAny<int>()));

            // Act
            await _vehicleService.UpdateCategoryVehiclesWithWeightInNewCategory(100.0m, 2);

            // Assert
            _autoMoqer.GetMock<IVehicleRepository>().Verify(r => r.UpdateCategoryVehiclesWithWeightInNewCategory(100.0m, 2), Times.Once);
        }

        [TestMethod]
        public async Task UpdateCategoryVehiclesWithWeightInNewCategory_ThrowsException()
        {
            // Arrange
            var mocker = new AutoMoqer();
            var vehicleService = mocker.Create<VehicleService>();

            mocker.GetMock<IVehicleRepository>()
                .Setup(v => v.UpdateCategoryVehiclesWithWeightInNewCategory(It.IsAny<decimal>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Error"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => vehicleService.UpdateCategoryVehiclesWithWeightInNewCategory(100.0m, 2));
        }
    }
}