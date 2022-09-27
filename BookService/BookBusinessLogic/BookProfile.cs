using AutoMapper;
using BookContracts;
using BookContracts.Dtos;

namespace BookBusinessLogic;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookGetDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
            .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.Author.Id));
        CreateMap<BookSaveDto, Book>();;
        CreateMap<BookUpdateDto, Book>();
        CreateMap<Author, AuthorGetDto>();

        CreateMap<AuthorPublishSaveDto, Author>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
    }
}