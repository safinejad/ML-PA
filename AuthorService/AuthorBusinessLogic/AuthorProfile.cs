using AuthorContracts;
using AuthorContracts.Dtos;
using AutoMapper;

namespace AuthorBusinessLogic;

public class AuthorProfile : Profile
{
    public AuthorProfile()
    {
        // Source -> Target
        CreateMap<Author, AuthorGetDto>();
        CreateMap<Author, AuthorPublishSaveDto>();
        CreateMap<AuthorSaveDto, Author>();
        CreateMap<AuthorUpdateDto, Author>();
    }
}