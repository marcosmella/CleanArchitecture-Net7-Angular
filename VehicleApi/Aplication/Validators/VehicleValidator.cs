using FluentValidation;
using Entities = Vehicle.Domain.Entities;

namespace Vehicle.Aplication.Validators
{
    public class VehicleValidator : AbstractValidator<Entities.Vehicle>
    {
        public VehicleValidator() 
        {
            RuleFor(x => x.OwnerName)
                .NotEmpty()
                .WithMessage("Is required");
            RuleFor(x => x.Manufacturer)
                .NotEmpty()
                .WithMessage("Is required");
            RuleFor(x => x.YearOfManufacture)
                .NotEmpty()
                .WithMessage("Is required")
                .LessThanOrEqualTo(x => DateTime.Now.Year)
                .WithMessage("Year of manufacture must before or equal " + DateTime.Now.Year);
            RuleFor(x => x.WeightKg)
                .NotEmpty()
                .WithMessage("Is required");
        }
    }
}
