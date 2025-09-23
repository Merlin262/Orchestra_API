using Orchestra.Serviecs.Intefaces;
using Orchestra.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Orchestra.Application.Auth.Query
{
    public class GetAuthenticatedUserResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public User? User { get; set; }
    }

    public class GetAuthenticatedUserHandler
    {
        private readonly IUserService _userService;
        public GetAuthenticatedUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetAuthenticatedUserResult> Handle(string token)
        {
            string userId;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "id")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return new GetAuthenticatedUserResult { Success = false, Message = "Token inválido." };
            }
            catch
            {
                return new GetAuthenticatedUserResult { Success = false, Message = "Token inválido." };
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return new GetAuthenticatedUserResult { Success = false, Message = "Usuário não encontrado." };

            return new GetAuthenticatedUserResult { Success = true, User = user };
        }
    }
}
