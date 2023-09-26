using Moq;
using Vehicle.Aplication.Validators;
using Entities = Vehicle.Domain.Entities;
using Vehicle.Aplication.Interfaces;
using FluentValidation.TestHelper;
using AutoMoqCore;
using FluentValidation;

namespace Vehicle.Service.Test.Validators
{
    [TestClass]
    public class CategoryValidatorTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IValidator<Entities.Category> _categoryValidator;
        private readonly IEnumerable<Entities.Category> _categories;
        public CategoryValidatorTests()
        {
            _autoMoqer = new AutoMoqer();
            _categoryValidator = _autoMoqer.Resolve<CategoryValidator>();
            _categories = new List<Entities.Category>
            {
                new Entities.Category
                {
                    Id = 1,
                    Name = "Toyota",
                    IconUrl = "icon-car",
                    MaxWeightKg = 100.00m,
                    MinWeightKg = 400.00m
                },
                new Entities.Category
                {
                    Id = 2,
                    Name = "Nissan",
                    IconUrl = "icon-car",
                    MaxWeightKg = 401.00m,
                    MinWeightKg = 499.00m
                },
            };
        }       

        [TestMethod]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            // Arrange           
            _autoMoqer.GetMock<ICategoryRepository>().Setup(x => x.Get())
                               .ReturnsAsync(_categories);
            var category = new Entities.Category 
                { 
                    Name = "", 
                    IconUrl = "icon.png",                 
                    MinWeightKg = 500, 
                    MaxWeightKg = 750 
                };

            // Act
            var result = _categoryValidator.TestValidate(category);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.Name)
                .WithErrorMessage("Is required");
        }

        [TestMethod]
        public void Should_Have_Error_When_IconUrl_Is_Empty()
        {
            // Arrange
            _autoMoqer.GetMock<ICategoryRepository>().Setup(x => x.Get())
                                .ReturnsAsync(_categories);
            var category = new Entities.Category 
                { 
                    Name = "Category", 
                    IconUrl = "",
                    MinWeightKg = 500,
                    MaxWeightKg = 750
                };

            // Act
            var result = _categoryValidator.TestValidate(category);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.IconUrl)
                .WithErrorMessage("Is required");
        }

        [TestMethod]
        public void Should_Have_Error_When_MinWeight_Is_Greater_Than_Max()
        {
            // Arrange
            _autoMoqer.GetMock<ICategoryRepository>().Setup(x => x.Get())
                               .ReturnsAsync(_categories);
            var category = new Entities.Category 
                    { 
                        Name = "Category", 
                        IconUrl = "icon.png",
                        MinWeightKg = 800,
                        MaxWeightKg = 750
                    };

            // Act
            var result = _categoryValidator.TestValidate(category);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.MinWeightKg)
                .WithErrorMessage("Must less than MaxWeightKg");
        }

        [TestMethod]
        public void Should_Have_Error_When_MaxWeight_Is_Less_Than_MinWeight()
        {
            // Arrange
            _autoMoqer.GetMock<ICategoryRepository>().Setup(x => x.Get())
                               .ReturnsAsync(_categories);
            var category = new Entities.Category 
                    { 
                        Name = "Category", 
                        IconUrl = "icon.png",
                        MinWeightKg = 700,
                        MaxWeightKg = 500
                    };

            // Act
            var result = _categoryValidator.TestValidate(category);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.MaxWeightKg)
                .WithErrorMessage("Must greater than MaxWeightKg");
        }


        [TestMethod]
        public void Should_Not_Have_Error_When_Validation_Passes()
        {
            // Arrange
            _autoMoqer.GetMock<ICategoryRepository>().Setup(x => x.Get())
                               .ReturnsAsync(_categories);             
            var category = new Entities.Category 
                    { 
                        Name = "Category", 
                        IconUrl = "icon.png",
                        MinWeightKg = 499.01m,
                        MaxWeightKg = 750.00m
                    };

            // Act
            var result = _categoryValidator.TestValidate(category);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Should_Have_Error_When_Creating_New_Category_And_There_Are_Overwrite_Gaps()
        {
            // Arrange
             _autoMoqer.GetMock<ICategoryRepository>().Setup(x => x.Get())
                .ReturnsAsync(_categories);

            var newCategory = new Entities.Category 
                    {
                        Name = "Category",
                        IconUrl = "icon.png",
                        MinWeightKg = 200, 
                        MaxWeightKg = 500, 
                        Id = 0 
                    };

            // Act
            var result = _categoryValidator.TestValidate(newCategory);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.MinWeightKg)
                .WithErrorMessage("You must set value greater than minimun weight seted: 499,00");
        }       
    }
}
