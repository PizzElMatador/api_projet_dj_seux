using Bibliotheque_classe_projet_Seux;
using Microsoft.AspNetCore.Mvc;
using RepositoryPOO;

namespace AP_FINAL.Controllers
{
    [ApiController]
    [Route("Prestation")]
    public class PrestationController : Controller
    {
        private readonly IConfiguration _configuration;

        public PrestationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Renvoie toutes les prestations correspondant aux paramètres
        /// </summary>
        [Route("Search")]
        [HttpGet]
        public IEnumerable<Prestation> GetAllPrestations([FromQuery] Prestation? p)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));
            List<Prestation> prestations = repo.GetByPredicate(p ?? new Prestation()).Cast<Prestation>().ToList();
            return prestations;
        }

        /// <summary>
        /// Renvoie une prestation par son identifiant
        /// </summary>
        [Route("Single")]
        [HttpGet]
        public Prestation GetPrestationById(int id)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));
            Prestation prestation = (Prestation)repo.GetObjectById(new Prestation() { Id = id });
            return prestation;
        }

        /// <summary>
        /// Ajoute une nouvelle prestation
        /// </summary>
        [HttpPost]
        [Route("Add")]
        public ActionResult<Prestation> Insert([FromBody] Prestation prestation)
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));
            int id = repo.SaveObject(prestation);
            if (id > 0)
            {
                return Ok(prestation);
            }
            else
            {
                return StatusCode(500);
            }
        }

        
    }
}