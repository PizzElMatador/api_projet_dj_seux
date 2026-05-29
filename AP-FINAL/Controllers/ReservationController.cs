using Bibliotheque_classe_projet_Seux;
using Microsoft.AspNetCore.Mvc;
using RepositoryPOO;
using MySql.Data.MySqlClient;

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
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                connection.Open();

                var checkCmd = new MySql.Data.MySqlClient.MySqlCommand(@"
            SELECT COUNT(*)
FROM reservation
WHERE id_client = @id_client
AND id_prestation = @id_prestation
AND statut IN ('En attente', 'Acceptée')", connection);

                checkCmd.Parameters.AddWithValue("@id_client", reservation.Id_client);
                checkCmd.Parameters.AddWithValue("@id_prestation", reservation.Id_prestation);

                var count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    return BadRequest("Vous avez déjà réservé cette prestation.");
                }

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

        [HttpGet]
        [Route("GetAll")]
        public ActionResult<IEnumerable<object>> GetAll()
        {
            try
            {
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                connection.Open();

                var cmd = new MySql.Data.MySqlClient.MySqlCommand(@"
    SELECT r.id_reservation, r.date_reservation, r.date_prestation, 
           r.rue, r.code_postal, r.ville,
           r.id_prestation, r.id_client,
           p.nom_prestation,
           u.nom AS client_nom, u.prenom AS client_prenom
    FROM reservation r
    LEFT JOIN prestation p ON r.id_prestation = p.id_prestation
    LEFT JOIN utilisateur u ON r.id_client = u.id_utilisateur", connection);

                var reservations = new List<object>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reservations.Add(new
                    {
                        id_reservation = reader["id_reservation"],
                        date_reservation = reader["date_reservation"],
                        date_prestation = reader["date_prestation"],
                        rue = reader["rue"],
                        code_postal = reader["code_postal"],
                        ville = reader["ville"],
                        id_prestation = reader["id_prestation"],
                        id_client = reader["id_client"],
                        nom_prestation = reader["nom_prestation"],
                        client_nom = reader["client_nom"],
                        client_prenom = reader["client_prenom"]
                    });
                }

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR GetAll : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("Client")]
        public ActionResult<IEnumerable<object>> GetByClient([FromQuery] int id)
        {
            try
            {
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                connection.Open();

                var cmd = new MySql.Data.MySqlClient.MySqlCommand(@"
            SELECT r.id_reservation, r.date_reservation, r.date_prestation,
                r.rue, r.code_postal, r.ville,
                r.id_prestation, r.id_client,
                r.statut,
                p.nom_prestation,
                u.nom AS client_nom, u.prenom AS client_prenom
            FROM reservation r
            LEFT JOIN prestation p ON r.id_prestation = p.id_prestation
            LEFT JOIN utilisateur u ON r.id_client = u.id_utilisateur
            WHERE r.id_client = @id", connection);

                cmd.Parameters.AddWithValue("@id", id);

                var reservations = new List<object>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reservations.Add(new
                    {
                        id_reservation = reader["id_reservation"],
                        date_reservation = reader["date_reservation"],
                        date_prestation = reader["date_prestation"],
                        rue = reader["rue"],
                        code_postal = reader["code_postal"],
                        ville = reader["ville"],
                        id_prestation = reader["id_prestation"],
                        id_client = reader["id_client"],
                        nom_prestation = reader["nom_prestation"],
                        client_nom = reader["client_nom"],
                        client_prenom = reader["client_prenom"],
                        statut = reader["statut"]
                    });
                }

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR GetByClient : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("Prestataire")]
        public IActionResult GetReservationsByPrestataire(int id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT r.*,
                       p.nom_prestation,
                       u.nom AS client_nom,
                       u.prenom AS client_prenom
                FROM reservation r
                INNER JOIN prestation p ON r.id_prestation = p.id_prestation
                INNER JOIN utilisateur u ON r.id_client = u.id_utilisateur
                WHERE p.id_prestataire = @id
                ORDER BY r.date_reservation DESC";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);

                    List<object> reservations = new List<object>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservations.Add(new
                            {
                                id_reservation = reader["id_reservation"],
                                date_reservation = reader["date_reservation"],
                                date_prestation = reader["date_prestation"],
                                rue = reader["rue"],
                                code_postal = reader["code_postal"],
                                ville = reader["ville"],
                                id_prestation = reader["id_prestation"],
                                id_client = reader["id_client"],
                                nom_prestation = reader["nom_prestation"],
                                client_nom = reader["client_nom"],
                                client_prenom = reader["client_prenom"],
                                statut = reader["statut"]
                            });
                        }
                    }

                    return Ok(reservations);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("Accept/{id}")]
        public IActionResult AcceptReservation(int id)
        {
            try
            {
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                connection.Open();

                var cmd = new MySql.Data.MySqlClient.MySqlCommand(@"
            UPDATE reservation
            SET statut = 'Acceptée'
            WHERE id_reservation = @id", connection);

                cmd.Parameters.AddWithValue("@id", id);

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                    return NotFound("Réservation introuvable");

                return Ok(new { message = "Réservation acceptée" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("Refuse/{id}")]
        public IActionResult RefuseReservation(int id)
        {
            try
            {
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                connection.Open();

                var cmd = new MySql.Data.MySqlClient.MySqlCommand(@"
            UPDATE reservation
            SET statut = 'Refusée'
            WHERE id_reservation = @id", connection);

                cmd.Parameters.AddWithValue("@id", id);

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                    return NotFound("Réservation introuvable");

                return Ok(new { message = "Réservation refusée" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}