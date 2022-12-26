using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Shared.Configurations;
using Shared.DTOs;
using Shared.Identity;

namespace Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public TokenResponse GetToken(TokenRequest request)
    {
        var token = GenerateJwt();

        var result = new TokenResponse(token);
        
        return result;
    }
    
    //generate jwt
    private string GenerateJwt() => GenerateEncryption(GetSigningCredentials());
        

    private SigningCredentials GetSigningCredentials()
    {
        byte[] key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

        return new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
    }
    
    private string GenerateEncryption(SigningCredentials signingCredentials)
    {
        var token = new JwtSecurityToken(
            signingCredentials: signingCredentials
        );
        
        var tokenHandler = new JwtSecurityTokenHandler();
        
        return tokenHandler.WriteToken(token);
    }
}