using Coursea.Models;
using System.Security.Claims;

namespace Coursea.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(List<Claim> claims);
    }
}
