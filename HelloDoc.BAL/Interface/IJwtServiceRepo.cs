
using System.IdentityModel.Tokens.Jwt;
using HelloDoc.DAL.Models;

namespace HelloDoc.BAL.Interface
{
    public interface IJwtServiceRepo
    {
        public string GenerateJwtToken(AspNetUser aspnetuser);

        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);
    }
}
