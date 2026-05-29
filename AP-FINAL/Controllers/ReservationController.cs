using Bibliotheque_classe_projet_Seux;
using Microsoft.AspNetCore.Mvc;
using RepositoryPOO;

namespace AP_FINAL.Controllers
{
    [ApiController]
    [Route("Reservation")]
    public class ReservationController : Controller
    {
        private readonly IConfiguration _configuration;

        public ReservationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Add")]
        public ActionResult<Reservation> Insert([FromBody] Reservation reservation)
        {
            try
            {
                MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));
                int id = repo.SaveObject(reservation);

                if (id > 0)
                    return Ok(reservation);
                else
                    return StatusCode(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR RESERVATION : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("Test")]
        public IActionResult Test()
        {
            return Ok("ReservationController fonctionne");
        }
    }
}