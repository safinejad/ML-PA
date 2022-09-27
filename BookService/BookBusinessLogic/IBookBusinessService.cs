using BookContracts;

namespace BookBusinessLogic;

public interface IBookBusinessService
{
    Book SaveBook(Book book);
    Book GetBookById(long id);
    IEnumerable<Book> SearchBooksByName(string partialKeyword);

    Author CreateAuthor(Author author);
    void DeleteAuthorByExternalId(long id);
    void DeleteBookById(long id);
    Book CreateBook(Book book);

    Author GetAuthorByExternalId(int bookExternalAuthorId);
    IEnumerable<Author> SearchAuthorsByName(string partialKeyword);
}