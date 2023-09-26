using FluentValidation.Results;
using System.Globalization;

namespace Vehicle.Aplication.CustomExceptions
{
    public class MyCustomException : Exception
    {
        public List<string> Errors { get; } = null!;

        public MyCustomException() : base() { }

        public MyCustomException(string message) : base(message) { }

        public MyCustomException(ValidationResult validationResult)
           : base()
        {
            Errors = validationResult.Errors.Select(e => (e.PropertyName + " - " + e.ErrorMessage)).ToList(); 
        }
    }
}
