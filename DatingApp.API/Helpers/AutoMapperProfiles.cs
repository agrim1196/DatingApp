using System.Linq;
using AutoMapper;
using DatingApp.API.DTOS;
using DatingApp.API.Model;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles: Profile
    {
       
        public AutoMapperProfiles()
        {
             CreateMap<User, UserForList>()
               .ForMember(dest => dest.PhotoURL, opt => 
                opt.MapFrom(src =>  src.Photos.FirstOrDefault(p => p.IsMain).Url))
               .ForMember(dest =>  dest.Age, (opt => opt.MapFrom(src => src.DateOfBirth.CaclulateAge())));
           
                
         
              CreateMap<User, UserForDetailsDTO>()
                .ForMember(dest => dest.PhotoURL, opt => 
                opt.MapFrom(src =>  src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest =>  dest.Age, (opt => opt.MapFrom(src => src.DateOfBirth.CaclulateAge())));
           
         
               CreateMap<Photo, PhotosForDetailsDTO>(); 
               CreateMap<UserForUpdateDTO, User>();
               CreateMap<Photo, PhotoForReturnDto>();
               CreateMap<PhotoForCreationDto, Photo>();
               CreateMap<UserForRegisterDTO, User>();  
            }
    }
}