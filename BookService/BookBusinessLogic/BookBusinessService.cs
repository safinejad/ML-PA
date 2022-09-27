using System.ComponentModel.DataAnnotations;
using BookContracts;
using FakeRepository;

namespace BookBusinessLogic
{
    public class BookBusinessService : IBookBusinessService
    {
        private readonly FakeRepo<Author> _authRepo;
        private readonly FakeRepo<Book> _bookRepo;

        public BookBusinessService(FakeRepo<Book> bookRepo, FakeRepo<Author> authRepo)
        {
            _authRepo = authRepo;
            _bookRepo = bookRepo;
        }
        public Book SaveBook(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (book.Id < 1)
            {
                var id = _bookRepo.Create(book);
                book.Id = id;
            }
            else
            {
                var oldBook = _bookRepo.GetAll().FirstOrDefault(x => x.Id == book.Id);
                if (oldBook == null) throw new InvalidDataException(nameof(book.Id));
                oldBook.Author = book.Author;
                oldBook.Name = book.Name;
                oldBook.SerialNumber = book.SerialNumber;
                _bookRepo.Update(oldBook);
                book = oldBook; //for fake repo
            }
            return book;
        }
        public Book GetBookById(long id)
        {
            return _bookRepo.GetAll().FirstOrDefault(x => x.Id == id)!;
        }
        public IEnumerable<Book> SearchBooksByName(string partialKeyword)
        {
            return _bookRepo.GetAll().Where(x =>
                x.Name.Contains(partialKeyword, StringComparison.InvariantCultureIgnoreCase));
        }


        public Author CreateAuthor(Author author)
        {
            var id = _authRepo.Create(author);
            return _authRepo.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void DeleteAuthorByExternalId(long exId)
        {
            var author = _authRepo.GetAll().FirstOrDefault(x => x.ExternalId == exId);
            if (author == null)
            {
                throw new InvalidDataException("External Id Not found");
            }

            _authRepo.Delete(author);
        }

        public void DeleteBookById(long id)
        {
            var book = _bookRepo.GetAll().FirstOrDefault(x => x.Id == id);
            if (book == null) throw new InvalidDataException("Id not found");
            _bookRepo.Delete(book);
        }

        public Book CreateBook(Book book)
        {
            var id = _bookRepo.Create(book);
            return _bookRepo.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public Author GetAuthorByExternalId(int externalAuthorId)
        {
            return _authRepo.GetAll().FirstOrDefault(x => x.ExternalId == externalAuthorId);
        }

        public IEnumerable<Author> SearchAuthorsByName(string partialKeyword)
        {
            return _authRepo.GetAll().Where(x =>
                x.Name.Contains(partialKeyword, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}