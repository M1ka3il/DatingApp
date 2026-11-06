using AutoMapper;
using API.DTO;
using API.Entities;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
  public AutoMapperProfiles()
  {
    CreateMap<AppUser, MemberDTO>()
        .ForMember(d => d.ImageUrl,
            o => o.MapFrom(s => s.Photos.FirstOrDefault(p => p.IsMain)!.Url));
    CreateMap<Photo, PhotoDTO>();

    CreateMap<Message, MessageDTO>()
        .ForMember(d => d.SenderPhotoUrl,
            o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(p => p.IsMain)!.Url))
        .ForMember(d => d.RecipientPhotoUrl,
            o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(p => p.IsMain)!.Url));
  }
}
