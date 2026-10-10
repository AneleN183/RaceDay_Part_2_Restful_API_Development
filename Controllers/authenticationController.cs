using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Part_2_Restful_API_Development.Data;
using RaceDay_Part_2_Restful_API_Development.Models;

namespace RaceDay_Part_2_Restful_API_Development.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class authenticationController : ControllerBase
    {
        private readonly RaceDayDatabase _context;

        public authenticationController(RaceDayDatabase context)
        {
            _context = context;
        }

        
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            // 1. Validate input
            if (string.IsNullOrWhiteSpace(request.email) ||
                string.IsNullOrWhiteSpace(request.password) ||
                string.IsNullOrWhiteSpace(request.first_name) ||
                string.IsNullOrWhiteSpace(request.last_name) ||
                string.IsNullOrWhiteSpace(request.user_role))
            {
                return BadRequest(new { message = "All fields are required." });
            }

            // 2. Validate role
            if (request.user_role != "organiser" && request.user_role != "participant")
            {
                return BadRequest(new { message = "Role must be 'organiser' or 'participant'." });
            }

            // 3. Check for duplicate email
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.email == request.email);

            if (existingUser != null)
            {
                return Conflict(new { message = "A user with this email already exists." });
            }

            // 4. Hash the password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.password);

            // 5. Create the user
            var newUser = new Users
            {
                first_name = request.first_name,
                last_name = request.last_name,
                email = request.email,
                user_password = hashedPassword,
                user_role = request.user_role,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "User registered successfully.",
                user_id = newUser.user_id
            });
        }

        
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.email) ||
                string.IsNullOrWhiteSpace(request.password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            // 1. Find the user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.email == request.email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // 2. Verify password against stored hash
            bool passwordValid = BCrypt.Net.BCrypt.Verify(request.password, user.user_password);

            if (!passwordValid)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // 3. Create session
            HttpContext.Session.SetInt32("UserId", user.user_id);
            HttpContext.Session.SetString("UserRole", user.user_role);
            HttpContext.Session.SetString("UserName", $"{user.first_name} {user.last_name}");

            return Ok(new
            {
                message = "Login successful.",
                user = new
                {
                    user_id = user.user_id,
                    first_name = user.first_name,
                    last_name = user.last_name,
                    email = user.email,
                    user_role = user.user_role
                }
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully." });
        }

        
        [HttpGet("me")]
        public async Task<ActionResult> GetCurrentUser()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized(new { message = "Not logged in." });
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return Unauthorized(new { message = "Session user no longer exists." });
            }

            return Ok(new
            {
                user_id = user.user_id,
                first_name = user.first_name,
                last_name = user.last_name,
                email = user.email,
                user_role = user.user_role
            });
        }
    }

    
    public class RegisterRequest
    {
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string user_role { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }
}