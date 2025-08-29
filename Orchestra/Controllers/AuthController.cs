using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data;
using Orchestra.Data.Context;
using Orchestra.Models;
using System;
using Orchestra.Dtos;
using Orchestra.Serviecs;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Orchestra.Enums;


namespace Orchestra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            var existing = await _context.Users.AnyAsync(u => u.Email == dto.Email || u.UserName == dto.UserName);
            if (existing)
                return BadRequest("Usuário já existente.");

            bool isFirstUser = !await _context.Users.AnyAsync();

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = dto.UserName,
                Email = dto.Email,
                FullName = dto.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Roles = null,
                ProfileType = isFirstUser 
                    ? new List<ProfileTypeEnum> { ProfileTypeEnum.ADM } 
                    : new List<ProfileTypeEnum> { ProfileTypeEnum.Employee }
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Usuário registrado com sucesso.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Credenciais inválidas.");

            var token = _jwtService.GenerateToken(user);
            return Ok(new { token });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            // Obtém o token do header Authorization
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Token não fornecido.");

            var token = authHeader.Substring("Bearer ".Length).Trim();

            // Valida o token e extrai o userId
            string userId;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "id")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Token inválido.");
            }
            catch
            {
                return Unauthorized("Token inválido.");
            }

            // Busca o usuário no banco de dados
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("Usuário não encontrado.");

            return Ok(new
            {
                success = true,
                data = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FullName,
                    user.Roles,
                    user.IsActive,
                    user.CreatedAt,
                    user.ProfileType
                }
            });
        }
    }
}
