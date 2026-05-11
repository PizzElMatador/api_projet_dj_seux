using Bibliotheque_classe_projet_Seux;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryPOO;

namespace AP_FINAL.Controllers
{
    [ApiController]
    [Route("User")]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Renvoie tous les utilisateurs correspondant aux paramètres
        /// </summary>
        /// <returns></returns>
        [Route("Search")]
        [HttpGet]
        public IEnumerable<Utilisateur> GetAllUsers([FromQuery] Utilisateur u)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));
            List<Utilisateur> users = repo.GetByPredicate(u).Cast<Utilisateur>().ToList();
            return users;
        }

        [Route("Single")]
        [HttpGet]
        public Utilisateur GetUserById(int id)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));
            Utilisateur user = (Utilisateur)repo.GetObjectById(new Utilisateur() { Id = id });
            return user;
        }

        [HttpPost]
        [Route("Add")]
        public ActionResult<Utilisateur> Insert([FromBody] Utilisateur user)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));
            int id = repo.SaveObject(user);
            if (id > 0)
            {
                return Ok(user);
            }
            else
            {
                return StatusCode(500);
            }
        }

      
       
    }

}
