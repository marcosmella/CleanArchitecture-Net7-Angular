using FluentValidation;
using Vehicle.Aplication.Interfaces;
using Vehicle.Domain.Entities;


namespace Vehicle.Aplication.Validators
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        internal readonly ICategoryRepository _categoryRepository;
        public CategoryValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Is required");
            RuleFor(x => x.IconUrl)
                .NotEmpty()
                .WithMessage("Is required");

            RuleFor(x => x.MinWeightKg)
                .NotNull()
                .WithMessage("Is required")
                .LessThan(x => x.MaxWeightKg)
                .WithMessage("Must less than MaxWeightKg");
            RuleFor(x => x.MaxWeightKg)
                .NotEmpty()
                .WithMessage("Is required")
                .GreaterThan(x => x.MinWeightKg)
                .WithMessage("Must greater than MaxWeightKg");


            RuleFor(category => category.MinWeightKg)
            .Must(NewCategoryMinWeightKgHasGreaterThanLastCategoryDbMinWeightKg())
            .WithMessage("You must set value greater than minimun weight seted: {MinWeightKg}")
            .When(category => category.Id == 0);

            RuleFor(category => category.MaxWeightKg)
            .Must(ThereAreGapsWhenUpdateCategory()).WithMessage("Categories should cover all possible weights with no gaps.")
            .When(category => category.Id != 0);
        }

        private Func<Category, decimal, ValidationContext<Category>, bool> ThereAreGapsWhenUpdateCategory()
        {
            return (category, maxWeight, context) =>
            {
                var categoriesWithOverlappingWeights = _categoryRepository.Get()
                    .Result.Any(c => c.Id != category.Id
                                     && c.MinWeightKg < category.MaxWeightKg
                                     && c.MaxWeightKg > category.MinWeightKg);
                return !categoriesWithOverlappingWeights;
            };
        }

        private Func<Category, decimal, ValidationContext<Category>, bool> NewCategoryMinWeightKgHasGreaterThanLastCategoryDbMinWeightKg()
        {
            return (category, maxWeight, context) =>
            {

                var MaxMinimunWeightKgInDb = _categoryRepository.Get()
                    .Result.Select(c => c.MinWeightKg).Max();
                context.MessageFormatter.AppendArgument("MinWeightKg", MaxMinimunWeightKgInDb);

                return MaxMinimunWeightKgInDb < category.MinWeightKg;
            };
        }
    }
}
