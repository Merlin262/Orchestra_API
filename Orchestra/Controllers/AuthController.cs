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
using Orchestra.Application.Auth;
using Orchestra.Serviecs.Intefaces;
using Orchestra.Application.Auth.Command;
using Orchestra.Application.Auth.Query;


namespace Orchestra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;

        public AuthController(IUserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            var handler = new RegisterUserHandler(_userService);
            var result = await handler.Handle(dto);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var handler = new LoginUserHandler(_userService, _jwtService);
            var result = await handler.Handle(dto);
            if (!result.Success)
                return Unauthorized(result.Message);
            return Ok(new { token = result.Token });
        }

        [HttpGet("me")]
        //[Authorize]
        public async Task<IActionResult> Me()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Token não fornecido.");

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var handler = new GetAuthenticatedUserHandler(_userService);
            var result = await handler.Handle(token);
            if (!result.Success)
            {
                if (result.Message == "Usuário não encontrado.")
                    return NotFound(result.Message);
                return Unauthorized(result.Message);
            }

            var user = result.User;
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
