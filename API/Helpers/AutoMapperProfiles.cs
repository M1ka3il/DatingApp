using AutoMapper;
using API.DTO;
using API.Entities;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
  public AutoMapperProfiles()
  {
    CreateMap<AppUser, MemberDTO>();
  }
}
