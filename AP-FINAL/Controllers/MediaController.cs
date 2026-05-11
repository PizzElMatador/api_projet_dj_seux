using Bibliotheque_classe_projet_Seux;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryPOO;

namespace AP_FINAL.Controllers
{
    [ApiController]
    [Route("Media")]
    public class MediaController : Controller
    {
        private readonly IConfiguration _configuration;
        public MediaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Renvoie tous les objets avec les paramètres 
        /// </summary>qz
        /// <returns></returns>
        [Route("Search")]
        [HttpGet]
        public IEnumerable<Media> GetAllBooks([FromQuery] Media b)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));

            List<Media> books = repo.GetByPredicate(b).Cast<Media>().ToList();
            return books;
        }

        [Route("Single")]
        [HttpGet]
        public Media GetBookById(int id)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));

            Media book = (Media)repo.GetObjectById(new Media() { Id = id });

            return book;
        }

        [HttpPost]
        [Route("Add")]
        public ActionResult<Media> Insert([FromBody] Media media)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));

            int id = repo.SaveObject(media);
            if (id > 0)
            {
                return Ok(media);
            }
            else
            {
                return StatusCode(500);
            }
        }
    }
}
