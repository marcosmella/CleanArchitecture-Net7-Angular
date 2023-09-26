using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Vehicle.Aplication.Interfaces;
using Vehicle.Aplication.Services.Database;
using Vehicle.Aplication.Validators;
using Entities = Vehicle.Domain.Entities;
using Vehicle.Infrastructure.Database;

namespace VehicleApi.Utils
{
    public static class IoC
    {
        public static void ConfigureIoC(this IServiceCollection services)
        {
            #region services
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<ICategoryService, CategoryService>();
            #endregion

            #region repositories
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            #endregion

            #region validators
            services.AddValidatorsFromAssemblyContaining<CategoryValidator>();
            services.AddValidatorsFromAssemblyContaining<VehicleValidator>();
            #endregion
        }

    }
}
