using AuthorBusinessLogic;
using AuthorContracts;
using AuthorContracts.Dtos;
using AutoMapper;
using FakeRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RMQMessageBusClient;

namespace AuthorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IMapper _autoMapper;
        private readonly IAuthorBusinessService _authorService;


        public AuthorController(IMapper autoMapper, IAuthorBusinessService authorService)
        {
            _autoMapper = autoMapper;
            _authorService = authorService;
            
        }
        [Route("s/{partialKeyword}")]
        [HttpGet]
        public ActionResult<IEnumerable<AuthorGetDto>> SearchAuthorsByName(string partialKeyword)
        {
            var authors = _authorService.SearchAuthorsByName(partialKeyword);
            var converted = _autoMapper.Map<IEnumerable<AuthorGetDto>>(authors);
            return Ok(converted);
        }
        [Route("{id}")]
        [HttpGet]
        public ActionResult<AuthorGetDto> GetAuthorById(int id)
        {
            var author = _authorService.GetAuthorById(id);
            var converted = _autoMapper.Map<AuthorGetDto>(author);
            return Ok(converted);
        }
        [HttpPost]
        public ActionResult<AuthorGetDto> CreateAuthor(AuthorSaveDto author)
        {
            var converted = _autoMapper.Map<Author>(author);
            var authorCreated = _authorService.SaveAuthor(converted);
            var createdConverted = _autoMapper.Map<AuthorGetDto>(authorCreated);
            return Ok(createdConverted);
        }
        [HttpPut]
        public ActionResult<AuthorGetDto> EditAuthor(AuthorUpdateDto author)
        {
            var converted = _autoMapper.Map<Author>(author);
            var authorEdited = _authorService.SaveAuthor(converted);
            var editedConverted = _autoMapper.Map<AuthorGetDto>(authorEdited);
            return Ok(editedConverted);
        }
    }
}