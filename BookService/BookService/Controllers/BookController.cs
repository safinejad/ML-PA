using AutoMapper;
using BookBusinessLogic;
using BookContracts;
using BookContracts.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IMapper _autoMapper;
        private readonly IBookBusinessService _bookService;


        public BookController(IMapper autoMapper, IBookBusinessService bookService)
        {
            _autoMapper = autoMapper;
            _bookService = bookService;

        }
        [Route("s/{partialKeyword?}")]
        [HttpGet]
        public ActionResult<IEnumerable<BookGetDto>> SearchBooksByName(string partialKeyword = "")
        {
            var books = _bookService.SearchBooksByName(partialKeyword);
            var converted = _autoMapper.Map<IEnumerable<BookGetDto>>(books);
            return Ok(converted);
        }
        [Route("{id}")]
        [HttpGet]
        public ActionResult<BookGetDto> GetBookById(int id)
        {
            var book = _bookService.GetBookById(id);
            var converted = _autoMapper.Map<BookGetDto>(book);
            return Ok(converted);
        }
        [Route("{id}")]
        [HttpDelete]
        public ActionResult<BookGetDto> DeleteBookById(int id)
        {
            try
            {
                _bookService.DeleteBookById(id);
            }
            catch (InvalidDataException ex)
            {
                //log
                return NotFound();
            }

            return Ok();
        }
        [HttpPost]
        public ActionResult<BookGetDto> CreateBook(BookSaveDto book)
        {
            var converted = _autoMapper.Map<Book>(book);
            var created = _bookService.SaveBook(converted);
            var createdConverted = _autoMapper.Map<BookGetDto>(created);
            return Ok(createdConverted);
        }
        [HttpPut]
        public ActionResult<BookGetDto> EditBook(BookUpdateDto book)
        {
            var converted = _autoMapper.Map<Book>(book);
            try
            {
                var edited = _bookService.SaveBook(converted);
                var editedConverted = _autoMapper.Map<BookGetDto>(edited);
                return Ok(editedConverted);
            }
            catch (InvalidDataException ex)
            {
                return NotFound();
            }
        }
        [Route("s/authors/{partialKeyword?}")]
        [HttpGet]
        public ActionResult<IEnumerable<AuthorGetDto>> SearchAuthorsByName(string partialKeyword)
        {
            var authors = _bookService.SearchAuthorsByName(partialKeyword);
            var converted = _autoMapper.Map<IEnumerable<AuthorGetDto>>(authors);
            return Ok(converted);
        }
    }
}