using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RepositoryPOO;
using AP_FINAL;


namespace AP_FINAL.Controllers
{
    /*[ApiController]
    [Route("Book")]
    public class BookController : Controller
    {
        private readonly IConfiguration _configuration;
        public BookController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Renvoie tous les objets avec les paramètres 
        /// </summary>
        /// <returns></returns>
        [Route("Search")]
        [HttpGet]
        public IEnumerable<Book> GetAllBooks([FromQuery] Book b)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));

            List<Book> books = repo.GetByPredicate(b).Cast<Book>().ToList();
            return books;
        }

        [Route("Single")]
        [HttpGet]
        public Book GetBookById(int id)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));

            Book book = (Book)repo.GetObjectById(new Book() { Id = id });

            return book;
        }

        [Route("Add")]
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult<Book> Insert(Book book)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));

            int id = repo.SaveObject(book);
            if (id > 0)
            {
                return Ok(book);
            }
            else
            {
                return StatusCode(500);
            }
        }


        [Route("Update")]
        [HttpPost]
        public ActionResult<Book> Update(Book book)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));

            if (book.Id > 0)
            {

                int id = repo.SaveObject(book);

                if (id > 0)
                {
                    return Ok(book);
                }
                else
                {
                    return StatusCode(500);
                }
            }
            else
            {
                return StatusCode(400);
            }
        }

        [Route("Delete")]
        [HttpDelete]
        public ActionResult<Book> Delete(Book book)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));

            if (book.Id > 0)
            {

                bool res = repo.DeleteObject(book);

                if (res)
                {
                    return Ok(book);
                }
                else
                {
                    return StatusCode(500);
                }
            }
            else
            {
                return StatusCode(400);
            }
        }
    }*/
}
