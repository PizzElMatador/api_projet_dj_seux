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

            List<Prestation> prestations =
                repo.GetByPredicate(p ?? new Prestation())
                    .Cast<Prestation>()
                    .ToList();

            using var connection = new MySql.Data.MySqlClient.MySqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            connection.Open();

            var cmd = new MySql.Data.MySqlClient.MySqlCommand(
            "SELECT id_prestation FROM reservation WHERE statut IN ('En attente', 'Acceptée')",
            connection);

            var prestationsReservees = new List<int>();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                prestationsReservees.Add(Convert.ToInt32(reader["id_prestation"]));
            }

            return prestations.Where(prestation =>
                !prestationsReservees.Contains(prestation.Id_prestation)
            );
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