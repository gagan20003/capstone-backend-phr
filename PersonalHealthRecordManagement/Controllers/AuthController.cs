using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Services;

namespace PersonalHealthRecordManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtTokenService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JwtTokenService jwtService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("User registration failed for {Email}: {Errors}", dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");
            _logger.LogInformation("User registered successfully: {Email}", dto.Email);

            return Ok(new { message = "User created successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt with invalid email: {Email}", dto.Email);
                return Unauthorized(new { error = "Invalid credentials" });
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Login attempt with invalid password for: {Email}", dto.Email);
                return Unauthorized(new { error = "Invalid credentials" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.CreateToken(user, roles);

            var userDetails = new
            {
                Name = user?.FullName,
                Email = user?.Email
            };

            _logger.LogInformation("User logged in successfully: {Email}", dto.Email);

            return Ok(new { token, userDetails });
        }
    }
}
