using AuthorContracts;
using AuthorContracts.Dtos;
using AutoMapper;
using FakeRepository;

namespace AuthorBusinessLogic;

public class AuthorConsumerService : IAuthorConsumerService
{
    private readonly IAuthorBusinessService _authorBusinessService;
    private readonly IMapper _mapper;

    public AuthorConsumerService(IAuthorBusinessService authorBusinessService, FakeRepo<Book> bookRepo, IMapper mapper)
    {
        _authorBusinessService = authorBusinessService;
        _mapper = mapper;

    }
    public void DeleteBookByExternalId(BookConsumerDeleteDto bookDel)
    {
        _authorBusinessService.DeleteBookByExternalId(bookDel.ExternalId);
    }

    public Book SaveBook(BookConsumerSaveDto bookSave)
    {
        var book = _mapper.Map<Book>(bookSave);
        return _authorBusinessService.SaveBook(book);
    }
}