using Bibliotheque_classe_projet_Seux;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RepositoryPOO;
using System.Text.Json;

namespace AP_FINAL.Controllers
{
    [ApiController]
    [Route("User")]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        /// <summary>
        /// Renvoie tous les utilisateurs correspondant aux paramètres
        /// </summary>
        /// <returns></returns>
        [Route("Search")]
        [HttpGet]
        public IEnumerable<Utilisateur> GetAllUsers([FromQuery] Utilisateur? u)
        {
            MysqlRepository repo = new MysqlRepository(
                _configuration.GetConnectionString("DefaultConnection")
            );

            var users = repo.GetByPredicate(u ?? new Utilisateur())
                .Cast<Utilisateur>()
                .ToList();

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

        [Route("GetAll")]
        [HttpGet]
        public IEnumerable<Utilisateur> GetAllUsersUnfiltered()
        {
            MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection"));

            // Récupérer directement tous les utilisateurs sans validation
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                string query = @"
SELECT 
    u.id_utilisateur,
    u.nom,
    u.prenom,
    u.email,
    u.date_inscription,
    u.AspNetUserId,
    u.telephone,
    u.adresse,
    r.Name AS Role
FROM utilisateur u
LEFT JOIN aspnetuserroles ur ON ur.UserId = u.AspNetUserId
LEFT JOIN aspnetroles r ON r.Id = ur.RoleId";
                var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection);
                var reader = command.ExecuteReader();

                List<Utilisateur> users = new List<Utilisateur>();
                while (reader.Read())
                {
                    Utilisateur user = new Utilisateur();
                    user.FillWithDataReader(reader);
                    users.Add(user);
                }

                return users;
            }
        }

        /// <summary>
        /// Change le rôle d'un utilisateur
        /// </summary>
        /// <param name="userId">ID de l'utilisateur (id_utilisateur de la table Utilisateur)</param>
        /// <param name="newRole">Nouveau rôle : Admin, Manager, User</param>
        [HttpPost("UpdateRole")]
        public async Task<IActionResult> UpdateUserRole([FromQuery] int userId, [FromQuery] string newRole)
        {
            try
            {
                // Récupérer l'AspNetUserId de l'utilisateur
                string aspNetUserId = "";
                using (var connection = new MySql.Data.MySqlClient.MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                        "SELECT AspNetUserId FROM Utilisateur WHERE id_utilisateur = @id", connection);
                    cmd.Parameters.AddWithValue("@id", userId);

                    using var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        aspNetUserId = reader["AspNetUserId"].ToString() ?? "";
                    }
                }

                if (string.IsNullOrEmpty(aspNetUserId))
                    return NotFound(new { success = false, message = "Utilisateur non trouvé" });

                // Récupérer l'utilisateur ASP.NET Identity
                var user = await _userManager.FindByIdAsync(aspNetUserId);
                if (user == null)
                    return NotFound(new { success = false, message = "Utilisateur ASP.NET non trouvé" });

                // Récupérer les rôles actuels
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Supprimer les anciens rôles
                if (currentRoles.Count > 0)
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                        return BadRequest(new { success = false, message = "Erreur lors de la suppression des anciens rôles" });
                }

                // Ajouter le nouveau rôle
                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addResult.Succeeded)
                    return BadRequest(new { success = false, message = "Erreur lors de l'attribution du nouveau rôle" });

                return Ok(new { success = true, message = $"Rôle {newRole} assigné avec succès à l'utilisateur" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Erreur serveur : {ex.Message}" });
            }
        }

        /// <summary>
        /// Bannit un utilisateur (le rend inactif)
        /// </summary>
        /// <param name="userId">ID de l'utilisateur à bannir</param>
        [HttpPost("Ban")]
        public async Task<IActionResult> BanUser([FromQuery] int userId)
        {
            try
            {
                using (var connection = new MySql.Data.MySqlClient.MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                        "UPDATE Utilisateur SET is_banned = 1 WHERE id_utilisateur = @id", connection);
                    cmd.Parameters.AddWithValue("@id", userId);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                        return NotFound(new { success = false, message = "Utilisateur non trouvé" });

                    return Ok(new { success = true, message = "L'utilisateur a été banni avec succès" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Erreur serveur : {ex.Message}" });
            }
        }

        /// <summary>
        /// Débannit un utilisateur (le rend actif à nouveau)
        /// </summary>
        /// <param name="userId">ID de l'utilisateur à débannir</param>
        [HttpPost("Unban")]
        public async Task<IActionResult> UnbanUser([FromQuery] int userId)
        {
            try
            {
                using (var connection = new MySql.Data.MySqlClient.MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                        "UPDATE Utilisateur SET is_banned = 0 WHERE id_utilisateur = @id", connection);
                    cmd.Parameters.AddWithValue("@id", userId);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                        return NotFound(new { success = false, message = "Utilisateur non trouvé" });

                    return Ok(new { success = true, message = "L'utilisateur a été débanni avec succès" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Erreur serveur : {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("UpdateProfile")]
        public IActionResult UpdateProfile([FromBody] JsonElement data)
        {
            try
            {
                int id = data.GetProperty("id").GetInt32();
                string nom = data.GetProperty("nom").GetString() ?? "";
                string prenom = data.GetProperty("prenom").GetString() ?? "";
                string email = data.GetProperty("email").GetString() ?? "";
                string telephone = data.TryGetProperty("telephone", out var telProp) ? telProp.GetString() ?? "" : "";
                string adresse = data.TryGetProperty("adresse", out var adrProp) ? adrProp.GetString() ?? "" : "";

                using var connection = new MySql.Data.MySqlClient.MySqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                connection.Open();

                var cmd = new MySql.Data.MySqlClient.MySqlCommand(@"
            UPDATE utilisateur
            SET nom = @nom,
                prenom = @prenom,
                email = @email,
                telephone = @telephone,
                adresse = @adresse
            WHERE id_utilisateur = @id", connection);

                cmd.Parameters.AddWithValue("@nom", nom);
                cmd.Parameters.AddWithValue("@prenom", prenom);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@telephone", telephone);
                cmd.Parameters.AddWithValue("@adresse", adresse);
                cmd.Parameters.AddWithValue("@id", id);

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                    return NotFound("Utilisateur non trouvé");

                return Ok(new
                {
                    id,
                    nom,
                    prenom,
                    email,
                    telephone,
                    adresse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }

}
