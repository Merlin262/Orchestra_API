using Orchestra.Dtos;
using Orchestra.Models;
using Orchestra.Enums;
using Orchestra.Serviecs.Intefaces;
using System;
using System.Collections.Generic;
using BCrypt.Net;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestra.Application.Auth.Command
{
    public class RegisterUserHandler
    {
        private readonly IUserService _userService;

        public RegisterUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<(bool Success, string Message)> Handle(UserRegisterDto dto)
        {
            var allUsers = await _userService.GetAllAsync();
            bool existing = allUsers.Any(u => u.Email == dto.Email || u.UserName == dto.UserName);
            if (existing)
                return (false, "Usuário já existente.");

            bool isFirstUser = !allUsers.Any();

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

            await _userService.AddAsync(user);

            return (true, "Usuário registrado com sucesso.");
        }
    }
}
