using System.Text.Json;
using AuthorContracts;
using AuthorContracts.Dtos;
using AutoMapper;
using FakeRepository;
using Microsoft.Extensions.Options;
using RMQMessageBusClient;

namespace AuthorBusinessLogic
{
    public class AuthorBusinessService: IAuthorBusinessService
    {
        private readonly FakeRepo<Author> _authRepo;
        private readonly RMQPublisher _authorProducer;
        private readonly IMapper _autoMapper;
        private readonly FakeRepo<Book> _bookRepo;

        public AuthorBusinessService(FakeRepo<Author> authRepo, FakeRepo<Book> bookRepo, IOptions<AuthorPublisherConfig> publisherConfig, IMapper autoMapper)
        {
            _authRepo = authRepo; //These Repos are fake - Each Service Has its own Database (which is implemented by FakeRepo<T>)
            _bookRepo = bookRepo;
            _authorProducer = new RMQPublisher(publisherConfig.Value);
            _autoMapper = autoMapper;
        }
        public Author SaveAuthor(Author author)
        {
            if (author == null) throw new ArgumentNullException(nameof(author));
            if (author.Id < 1)
            {
                var id = _authRepo.Create(author);
                author.Id = id;
            }
            else
            {
                var oldAuthor = _authRepo.GetAll().FirstOrDefault(x => x.Id == author.Id);
                if (oldAuthor == null) throw new InvalidDataException(nameof(author.Id));
                oldAuthor.AuthorGuid = author.AuthorGuid;
                oldAuthor.Name = author.Name;
                _authRepo.Update(oldAuthor);
                author = oldAuthor; //for fake repo
            }
            var converted = _autoMapper.Map<AuthorPublishSaveDto>(author);
            _authorProducer.Publish(converted, type: MessageEventTypeEnum.Save);
            return author;
        }

        public void DeleteAuthorById(int id)
        {
            var author = GetAuthorById(id);
            if (author == null)
            {
                throw new InvalidDataException("Author not found");
            }

            if (_bookRepo.GetAll().Any(x => x.Author.Id == id))
            {
                throw new InvalidOperationException("Cannot delete Author when there books assigned to it.");
            }
            _authRepo.Delete(author);
            _authorProducer.Publish(id, type: MessageEventTypeEnum.Delete);
        }
        public Author GetAuthorById(int id)
        {
            return _authRepo.GetAll().FirstOrDefault(x => x.Id == id)!;
        }
        public IEnumerable<Author> SearchAuthorsByName(string partialKeyword)
        {
            var all=_authRepo.GetAll();
            if (!string.IsNullOrWhiteSpace(partialKeyword))
            {
                return all.Where(x =>
                    x.Name.Contains(partialKeyword, StringComparison.InvariantCultureIgnoreCase));
            }

            return all;
        }

        public void DeleteBookByExternalId(long externalId)
        {
            var book = _bookRepo.GetAll().FirstOrDefault(x => x.ExternalId == externalId);
            if (book == null)
            {
                throw new InvalidDataException("External Id  not found!");
            }
            _bookRepo.Delete(book);
        }

        public Book SaveBook(Book book)
        {
            book.Author = _authRepo.GetAll().FirstOrDefault(x => x.Id == book.Author.Id); //For Fake Repo
            var already = _bookRepo.GetAll().FirstOrDefault(x => x.ExternalId == book.ExternalId);
            if (already == null)
            {
                var id = _bookRepo.Create(book);
                book.Id = id; //For FakeRepo
                return _bookRepo.GetAll().FirstOrDefault(x => x.Id == id);
            }
            else
            {
                already.Name = book.Name;
                already.Author = book.Author;
                _bookRepo.Update(already);
                return already;
            }
        }

        public IEnumerable<Book> GetAuthorBooks(int authorId)
        {
            return _bookRepo.GetAll().Where(x => x.Author.Id == authorId);
        }
    }
}