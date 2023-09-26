using AutoMapper;
using Vehicle.Domain.Entities;
using VehicleApi.DTOs;
using Entities = Vehicle.Domain.Entities;

namespace VehicleApi.Utils
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            #region request
            CreateMap<CategoryDTO, Category>();
            CreateMap<VehicleDTO, Entities.Vehicle>();
            #endregion

            #region response
            CreateMap<Category, CategoryDTO>();
            CreateMap<Entities.Vehicle, VehicleDTO>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => new CategoryDTO { Id = src.Category.Id, IconUrl = src.Category.IconUrl }));
            #endregion

        }
    }
}
