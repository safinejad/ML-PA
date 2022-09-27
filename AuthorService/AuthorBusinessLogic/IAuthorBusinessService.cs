using AuthorContracts;

namespace AuthorBusinessLogic
{
    public interface IAuthorBusinessService
    {
        Author SaveAuthor(Author author);
        Author GetAuthorById(int id);
        void DeleteAuthorById(int id);
        IEnumerable<Author> SearchAuthorsByName(string partialKeyword);
    }
}