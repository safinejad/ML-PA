using AuthorContracts;
using AuthorContracts.Dtos;

namespace AuthorBusinessLogic
{
    public interface IAuthorBusinessService
    {
        Author SaveAuthor(Author author);
        Author GetAuthorById(int id);
        void DeleteAuthorById(int id);
        IEnumerable<Author> SearchAuthorsByName(string partialKeyword);
        void DeleteBookByExternalId(long externalId);
        Book SaveBook(Book book);
        IEnumerable<Book> GetAuthorBooks(int authorId);
    }

    public interface IAuthorConsumerService
    {
        void DeleteBookByExternalId(BookConsumerDeleteDto bookDel);
        Book SaveBook(BookConsumerSaveDto bookSave);

    }
}