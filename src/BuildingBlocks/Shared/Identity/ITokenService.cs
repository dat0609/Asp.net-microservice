using Shared.DTOs;

namespace Shared.Identity;

public interface ITokenService
{
    TokenResponse GetToken(TokenRequest request);
}