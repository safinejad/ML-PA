using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BookContracts;
using BookContracts.Dtos;
using FakeRepository;
using Microsoft.Extensions.Options;
using RMQMessageBusClient;

namespace BookBusinessLogic
{
    public class BookBusinessService : IBookBusinessService
    {
        private readonly FakeRepo<Author> _authRepo;
        private readonly FakeRepo<Book> _bookRepo;
        private readonly RMQPublisher _bookProducer;
        private readonly IMapper _autoMapper;

        public BookBusinessService(FakeRepo<Book> bookRepo, FakeRepo<Author> authRepo, IOptions<BookPublisherConfig> publisherConfig, IMapper autoMapper)
        {
            _authRepo = authRepo;
            _bookRepo = bookRepo;
            _autoMapper = autoMapper;
            _bookProducer = new RMQPublisher(publisherConfig.Value);
        }
        public Book SaveBook(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            var author = _authRepo.GetAll().FirstOrDefault(x => x.Id == book.Author.Id);
            if (author != null) book.Author = author; //For FakeRepo
            if (book.Id < 1)
            {
                var id = _bookRepo.Create(book) * 100;
                book.Id = id; // For Fake Repo
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
            var converted = _autoMapper.Map<BookConsumerSaveDto>(book);
            _bookProducer.Publish(converted, typeToTopic: true);
            return book;
        }
        public Book GetBookById(long id)
        {
            var book= _bookRepo.GetAll().FirstOrDefault(x => x.Id == id)!;
            return book;
        }
        public IEnumerable<Book> SearchBooksByName(string partialKeyword)
        {
            var all = _bookRepo.GetAll();
            if (!string.IsNullOrWhiteSpace(partialKeyword))
            {
                return all.Where(x =>
                    x.Name.Contains(partialKeyword, StringComparison.InvariantCultureIgnoreCase));
            }

            return all;
        }


        public Author SaveAuthor(Author author)
        {
            var already = _authRepo.GetAll().FirstOrDefault(x => x.ExternalId == author.ExternalId);
            if (already == null)
            {
                var id = _authRepo.Create(author) * 100;
                author.Id = id; // For Fake Repo
                return _authRepo.GetAll().FirstOrDefault(x => x.Id == id);
            }
            else
            {
                already.Name = author.Name;
                _authRepo.Update(already);
                return already;
            }
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
            var deleteDto = new BookConsumerDeleteDto()
            {
                ExternalId = id
            };
            _bookProducer.Publish(deleteDto, typeToTopic: true);
        }


        public Author GetAuthorByExternalId(int externalAuthorId)
        {
            return _authRepo.GetAll().FirstOrDefault(x => x.ExternalId == externalAuthorId);
        }

        public IEnumerable<Author> SearchAuthorsByName(string partialKeyword)
        {
            var all = _authRepo.GetAll();
            if (!string.IsNullOrWhiteSpace(partialKeyword))
                return all.Where(x =>
                    x.Name.Contains(partialKeyword, StringComparison.InvariantCultureIgnoreCase));
            return all;
        }
    }

}