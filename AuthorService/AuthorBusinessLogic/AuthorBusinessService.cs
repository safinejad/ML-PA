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

        public AuthorBusinessService(FakeRepo<Author> authRepo, IOptions<AuthorPublisherConfig> publisherConfig, IMapper autoMapper)
        {
            _authRepo = authRepo;
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
            _authorProducer.Publish(id, type: MessageEventTypeEnum.Delete);
            _authRepo.Delete(author);
        }
        public Author GetAuthorById(int id)
        {
            return _authRepo.GetAll().FirstOrDefault(x => x.Id == id)!;
        }
        public IEnumerable<Author> SearchAuthorsByName(string partialKeyword)
        {
            return _authRepo.GetAll().Where(x =>
                x.Name.Contains(partialKeyword, StringComparison.InvariantCultureIgnoreCase));
        }
        public Author GetBooks(int id)
        {
            return _authRepo.GetAll().FirstOrDefault(x => x.Id == id)!;
        }
    }
}