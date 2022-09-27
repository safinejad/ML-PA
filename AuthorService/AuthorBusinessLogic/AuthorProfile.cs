using AuthorContracts;
using AuthorContracts.Dtos;
using AutoMapper;

namespace AuthorBusinessLogic;

public class AuthorProfile : Profile
{
    public AuthorProfile()
    {
        CreateMap<Author, AuthorGetDto>();
        CreateMap<Author, AuthorPublishSaveDto>();
        CreateMap<AuthorSaveDto, Author>();
        CreateMap<AuthorUpdateDto, Author>();
        CreateMap<BookConsumerSaveDto, Book>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => new Author()
            {
                Id = src.ExternalAuthorId
            }));
        CreateMap<Book, BookGetDto>()
            .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.Author.Id))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));

    }
}