using AutoMoqCore;
using FluentValidation.Results;
using Moq;
using Entities = Vehicle.Domain.Entities;
using Vehicle.Aplication.CustomExceptions;
using Vehicle.Aplication.Interfaces;
using Vehicle.Aplication.Services.Database;
using FluentValidation;

namespace Category.Service.Test
{
    [TestClass]
    public class CategoryServiceTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly ICategoryService _categoryService;

        public CategoryServiceTests()
        {
            _autoMoqer = new AutoMoqer();
            _categoryService = _autoMoqer.Resolve<CategoryService>();
        }

        [TestMethod]
        public async Task GetAsync()
        {
            //ARRANGE
            IEnumerable<Entities.Category> Categories = new List<Entities.Category>
            {
                new Entities.Category
                {
                    Id = 1,
                    Name = "Toyota",
                    IconUrl = "icon-car",
                    MaxWeightKg = 500.00m,
                    MinWeightKg = 1000.00m
                },
                new Entities.Category
                {
                    Id = 2,
                    Name = "Nissan",
                    IconUrl = "icon-car",
                    MaxWeightKg = 700.00m,
                    MinWeightKg = 900.00m
                },
            };

            _autoMoqer.GetMock<ICategoryRepository>().Setup(x => x.Get())
                .ReturnsAsync(Categories);

            //ACT
            var result = await _categoryService.Get();

            //ASSERT
            for (int i = 0; i < Categories.Count(); i++)
            {
                Assert.AreEqual(result.ElementAt(i).Id, Categories.ElementAt(i).Id);
                Assert.AreEqual(result.ElementAt(i).Name, Categories.ElementAt(i).Name);
                Assert.AreEqual(result.ElementAt(i).IconUrl, Categories.ElementAt(i).IconUrl);
                Assert.AreEqual(result.ElementAt(i).MaxWeightKg, Categories.ElementAt(i).MaxWeightKg);
                Assert.AreEqual(result.ElementAt(i).MinWeightKg, Categories.ElementAt(i).MinWeightKg);
            }
        }

        [TestMethod]
        public async Task Get_WhenRepositoryReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var vehiclesEmptyList = new List<Entities.Category>();

            _autoMoqer.GetMock<ICategoryRepository>().Setup(x => x.Get())
                .ReturnsAsync(vehiclesEmptyList);

            // Act
            var result = await _categoryService.Get();

            // Assert
            Assert.AreEqual(result, vehiclesEmptyList);
        }

        [TestMethod]
        public async Task GetByIdAsync()
        {
            //ARRANGE
            int id = 5;
            var vehicle = new Entities.Category
            {
                Id = 1,
                Name = "Toyota",
                IconUrl = "icon-car",
                MaxWeightKg = 500.00m,
                MinWeightKg = 1000.00m
            };

            _autoMoqer.GetMock<ICategoryRepository>().Setup(x => x.Get(id))
                .ReturnsAsync(vehicle);

            //ACT
            var result = await _categoryService.Get(id);

            //ASSERT            
            Assert.AreEqual(result.Id, vehicle.Id);
            Assert.AreEqual(result.Name, vehicle.Name);
            Assert.AreEqual(result.IconUrl, vehicle.IconUrl);
            Assert.AreEqual(result.MaxWeightKg, vehicle.MaxWeightKg);
            Assert.AreEqual(result.MinWeightKg, vehicle.MinWeightKg);
        }

        [TestMethod]
        public async Task Get_WhenRepositoryReturnsEmptyObject_ReturnsEmptyObject()
        {
            // Arrange
            var id = 14;
            var vehiclesEmpty = new Entities.Category();

            _autoMoqer.GetMock<ICategoryRepository>().Setup(x => x.Get(id))
                .ReturnsAsync(vehiclesEmpty);

            // Act
            var result = await _categoryService.Get(id);

            // Assert
            Assert.AreEqual(result, vehiclesEmpty);
        }

        [TestMethod]
        public async Task Create_ValidCategory_HappyPath()
        {
            // Arrange
            var validCategory = new Entities.Category
            {
                Id = 1,
                Name = "Toyota",
                IconUrl = "icon-car",
                MaxWeightKg = 500.00m,
                MinWeightKg = 1000.00m
            };

            IEnumerable<Entities.Category> Categories = new List<Entities.Category>
            {
                new Entities.Category
                {
                    Id = 1,
                    Name = "Toyota",
                    IconUrl = "icon-car",
                    MaxWeightKg = 500.00m,
                    MinWeightKg = 1000.00m
                },
                new Entities.Category
                {
                    Id = 2,
                    Name = "Nissan",
                    IconUrl = "icon-car",
                    MaxWeightKg = 700.00m,
                    MinWeightKg = 900.00m
                },
            };

            _autoMoqer.GetMock<IValidator<Entities.Category>>()
               .Setup(v => v.ValidateAsync(It.IsAny<Entities.Category>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _autoMoqer.GetMock<ICategoryRepository>()
               .Setup(c => c.Get())
               .ReturnsAsync(Categories);

            _autoMoqer.GetMock<ICategoryRepository>()
                .Setup(c => c.Create(It.IsAny<Entities.Category>()))
                .ReturnsAsync(1);

            _autoMoqer.GetMock<IVehicleService>()
                .Setup(v => v.UpdateCategoryVehiclesWithWeightInNewCategory(It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _categoryService.Create(validCategory);

            // Assert
            Assert.AreEqual(validCategory.Id, result);
            _autoMoqer.GetMock<ICategoryRepository>().Verify(r => r.Create(validCategory), Times.Once);
            _autoMoqer.GetMock<IVehicleService>().Verify(v => v.UpdateCategoryVehiclesWithWeightInNewCategory(validCategory.MinWeightKg, validCategory.Id), Times.Once);
        }

        [TestMethod]
        public async Task Create_InvalidCategory_ThrowsCustomException()
        {
            // Arrange
            var invalidCategory = new Entities.Category(); 

            _autoMoqer.GetMock<IValidator<Entities.Category>>()
               .Setup(v => v.ValidateAsync(It.IsAny<Entities.Category>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("Property", "Error message")
                })); 

            // Act & Assert
            await Assert.ThrowsExceptionAsync<MyCustomException>(() => _categoryService.Create(invalidCategory));
        }

        [TestMethod]
        public async Task Create_WithoutGapsWithPreviousCategory_HappyPath()
        {
            // Arrange         
            var validCategory = new Entities.Category
            {
                Id = 1,
                Name = "Toyota",
                IconUrl = "icon-car",
                MaxWeightKg = 500.00m,
                MinWeightKg = 1000.00m
            };

            IEnumerable<Entities.Category> Categories = new List<Entities.Category>
            {
                new Entities.Category
                {
                    Id = 1,
                    Name = "Toyota",
                    IconUrl = "icon-car",
                    MaxWeightKg = 500.00m,
                    MinWeightKg = 1000.00m
                },
                new Entities.Category
                {
                    Id = 2,
                    Name = "Nissan",
                    IconUrl = "icon-car",
                    MaxWeightKg = 700.00m,
                    MinWeightKg = 900.00m
                },
            };

            _autoMoqer.GetMock<IValidator<Entities.Category>>()
               .Setup(v => v.ValidateAsync(It.IsAny<Entities.Category>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

            _autoMoqer.GetMock<ICategoryRepository>()
               .Setup(c => c.Get())
               .ReturnsAsync(Categories);

            _autoMoqer.GetMock<ICategoryRepository>()
                .Setup(c => c.Create(It.IsAny<Entities.Category>()))
                .ReturnsAsync(1); 

            _autoMoqer.GetMock<IVehicleService>()
                .Setup(v => v.UpdateCategoryVehiclesWithWeightInNewCategory(It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            await _categoryService.Create(validCategory);

            // Assert
            _autoMoqer.GetMock<ICategoryRepository>().Verify(r => r.Create(validCategory), Times.Once);
        }

        [TestMethod]
        public async Task Create_WithoutGapsWithPreviousCategory_ThrowsException()
        {
            // Arrange         
            var validCategory = new Entities.Category
            {
                Id = 1,
                Name = "",
                IconUrl = "icon-car",
                MaxWeightKg = 500.00m,
                MinWeightKg = 1000.00m
            };

            _autoMoqer.GetMock<IValidator<Entities.Category>>()
                .Setup(v => v.ValidateAsync(It.IsAny<Entities.Category>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("Name", "Is required")
                }));
            // Act & Assert
            await Assert.ThrowsExceptionAsync<MyCustomException>(() => _categoryService.Create(validCategory));
        }

        [TestMethod]
        public async Task UpdateCategory_HappyPath()
        {
            // Arrange
            var validCategory = new Entities.Category
            {
                Id = 1,
                Name = "Toyota",
                IconUrl = "icon-car",
                MaxWeightKg = 500.00m,
                MinWeightKg = 1000.00m
            };

            _autoMoqer.GetMock<IValidator<Entities.Category>>()
               .Setup(v => v.ValidateAsync(It.IsAny<Entities.Category>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

            _autoMoqer.GetMock<ICategoryRepository>()
               .Setup(c => c.Update(It.IsAny<Entities.Category>()))
               .ReturnsAsync(1);

            // Act
             var result = await _categoryService.Update(validCategory);

            // Assert
            Assert.AreEqual(1, result);
            _autoMoqer.GetMock<ICategoryRepository>().Verify(r => r.Update(validCategory), Times.Once);
        }

        [TestMethod]
        public async Task UpdateCategory_InvalidCategory_ThrowsCustomException()
        {
            // Arrange
            var invalidCategory = new Entities.Category
            {
                Id = 1,
                Name = "Toyota",
                IconUrl = "icon-car",
                MaxWeightKg = 500.00m,
                MinWeightKg = 1000.00m
            };

            _autoMoqer.GetMock<IValidator<Entities.Category>>()
                .Setup(v => v.ValidateAsync(It.IsAny<Entities.Category>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("Property", "Error message")
                }));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<MyCustomException>(() => _categoryService.Update(invalidCategory));
        }

        [TestMethod]
        public async Task Delete_HappyPath()
        {
            // Arrange
            IEnumerable<Entities.Category> Categories = new List<Entities.Category>
            {
                new Entities.Category
                {
                    Id = 1,
                    Name = "Toyota",
                    IconUrl = "icon-car",
                    MaxWeightKg = 500.00m,
                    MinWeightKg = 1000.00m
                },
                new Entities.Category
                {
                    Id = 2,
                    Name = "Nissan",
                    IconUrl = "icon-car",
                    MaxWeightKg = 700.00m,
                    MinWeightKg = 900.00m
                },
            };

            _autoMoqer.GetMock<ICategoryRepository>().Setup(x => x.Get())
                .ReturnsAsync(Categories);

            _autoMoqer.GetMock<ICategoryRepository>()
                .Setup(c => c.GetCategoryByWeight(It.IsAny<decimal>()))
                .ReturnsAsync(1); 

            _autoMoqer.GetMock<IVehicleService>()
                .Setup(v => v.UpdateCategoryVehiclesToPreviousCategory(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask); 

            _autoMoqer.GetMock<ICategoryRepository>()
                .Setup(c => c.Update(It.IsAny<Entities.Category>()))
                .ReturnsAsync(1); 

            _autoMoqer.GetMock<ICategoryRepository>()
                .Setup(c => c.Delete(It.IsAny<int>()))
                .ReturnsAsync(1); 

            // Act
            var result = await _categoryService.Delete(1);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Delete_UnhappyPath_LastCategory()
        {
            // Arrange

            _autoMoqer.GetMock<ICategoryRepository>()
                .Setup(c => c.Get())
                .ReturnsAsync(new List<Entities.Category> { new Entities.Category() });

            // Act & Assert
            await Assert.ThrowsExceptionAsync<MyCustomException>(() => _categoryService.Delete(1));
        }

        [TestMethod]
        public async Task Delete_ThrowLastCategory()
        {
            // Arrange

            _autoMoqer.GetMock<ICategoryRepository>()
                .Setup(c => c.Get())
                .ReturnsAsync(new List<Entities.Category> { new Entities.Category
                {
                    Id = 1,
                    Name = "Toyota",
                    IconUrl = "icon-car",
                    MaxWeightKg = 500.00m,
                    MinWeightKg = 1000.00m
                } });

            // Act & Assert
            await Assert.ThrowsExceptionAsync<MyCustomException>(() => _categoryService.Delete(1));
        }
    }
}
