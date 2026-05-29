using AP_FINAL.DBContext;
using Bibliotheque_classe_projet_Seux;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RepositoryPOO;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AP_FINAL.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ITokenService tokenService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        private async Task<(string firstName, string lastName, int idUtilisateur)> GetNomPrenomAsync(string aspNetUserId)
        {
            string firstName = "";
            string lastName = "";
            int idUtilisateur = 0;
            try
            {
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                    "SELECT prenom, nom, id_utilisateur FROM Utilisateur WHERE AspNetUserId = @id", connection);
                cmd.Parameters.AddWithValue("@id", aspNetUserId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    firstName = reader["prenom"].ToString() ?? "";
                    lastName = reader["nom"].ToString() ?? "";
                    idUtilisateur = Convert.ToInt32(reader["id_utilisateur"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur récupération nom/prénom : " + ex.Message);
            }
            return (firstName, lastName, idUtilisateur);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            await _userManager.AddToRoleAsync(user, "User");

            try
            {
                MysqlRepository repo = new MysqlRepository(_configuration.GetConnectionString("DefaultConnection")!);
                var utilisateur = new Utilisateur()
                {
                    Nom = model.LastName,
                    Prenom = model.FirstName,
                    Email = model.Email,
                    AspNetUserId = user.Id,
                    Date_inscription = DateTime.Now
                };
                repo.SaveObject(utilisateur);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur SaveObject : " + ex.Message);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var (firstName, lastName, idUtilisateur) = await GetNomPrenomAsync(user.Id);

            return Ok(new
            {
                message = "Inscription réussie",
                accessToken = token,
                refreshToken = refreshToken,
                expiresIn = 3600,
                user = new
                {
                    id = user.Id,
                    idUtilisateur = idUtilisateur,
                    email = user.Email,
                    username = user.UserName,
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    roles = roles
                }
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Email ou mot de passe incorrect" });

            var isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isValidPassword)
                return Unauthorized(new { message = "Email ou mot de passe incorrect" });

            var (firstName, lastName, idUtilisateur) = await GetNomPrenomAsync(user.Id);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return Ok(new
            {
                message = "Connexion réussie",
                accessToken = token,
                refreshToken = refreshToken,
                expiresIn = 3600,
                user = new
                {
                    id = user.Id,
                    idUtilisateur = idUtilisateur,
                    email = user.Email,
                    username = user.UserName,
                    firstName = firstName,
                    lastName = lastName,
                    roles = roles
                }
            });
        }

        [HttpGet("Me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null)
                return NotFound(new { message = "Utilisateur non trouvé" });

            var (firstName, lastName, idUtilisateur) = await GetNomPrenomAsync(user.Id);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                id = user.Id,
                idUtilisateur = idUtilisateur,
                email = user.Email,
                username = user.UserName,
                firstName = firstName,
                lastName = lastName,
                roles = roles,
                emailConfirmed = user.EmailConfirmed
            });
        }

        [HttpPut("ChangePwd")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null)
                return NotFound(new { message = "Utilisateur non trouvé" });

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            return Ok(new { message = "Mot de passe modifié avec succès" });
        }
    }

    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}