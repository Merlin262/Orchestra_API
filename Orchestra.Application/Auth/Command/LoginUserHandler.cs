using Orchestra.Dtos;
using Orchestra.Models;
using Orchestra.Serviecs;
using Orchestra.Serviecs.Intefaces;
using System.Threading.Tasks;

namespace Orchestra.Application.Auth.Command
{
    public class LoginUserHandler
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;

        public LoginUserHandler(IUserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        public async Task<(bool Success, string Token, string Message)> Handle(UserLoginDto dto)
        {
            var users = await _userService.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return (false, null, "Credenciais inválidas.");

            if (!user.IsActive)
                return (false, null, "Usuário inativo. Entre em contato com o administrador.");

            var token = _jwtService.GenerateToken(user);
            return (true, token, null);
        }
    }
}
