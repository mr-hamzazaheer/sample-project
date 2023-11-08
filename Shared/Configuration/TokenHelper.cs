using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Shared
{
    public static class TokenHelper
    {
        public static string GetToken(this string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimKey.UserId.ToString(), userId?.ToString()),
            };
            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(Static.Settings.Jwt.Issuer,
              Static.Settings.Jwt.Issuer,
              claims, null,
              DateTime.Now.AddMinutes(Static.Settings.Jwt.TokenExpiryMinutes),
              signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Static.Settings.Jwt.Key)), SecurityAlgorithms.HmacSha256)));
        }
        public static string GetToken(this HttpContext httpContext)
        {
            string[] param = (Convert.ToString(httpContext.Request.Headers["Authorization"]) ?? "no-token").Split(" ");
            if (param[0].Equals("Bearer", StringComparison.InvariantCulture))
                return param[1];
            else
                return null;
        }
        public static TokenValidationParameters GetValidationParameters(double expiryMin, string issuer, string key)
        {
            return new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.FromMinutes(expiryMin),// We recommend 5 minutes or less
                RequireSignedTokens = true,
                RequireExpirationTime = true, // Ensure the token hasn't expired:
                ValidateLifetime = true,
                ValidateAudience = true, // Ensure the token audience matches our audience value (default true)                
                ValidateIssuer = true,// Ensure the token was issued by a trusted authorization server (default true)
                ValidIssuer = issuer,
                ValidAudience = issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }
        public static JwtSecurityToken Validate(this string token)
        {
            SecurityToken validatedToken;
            IPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(token, GetValidationParameters(Static.Settings.Jwt.TokenExpiryMinutes, Static.Settings.Jwt.Issuer, Static.Settings.Jwt.Key), out validatedToken);
            return (JwtSecurityToken)validatedToken;
        }
        public static UserProfile GetClaim(this string token)
        {
            try
            {
                JwtSecurityToken validToken = token.Validate();
                return token.ReadToken();
            }
            catch (SecurityTokenValidationException)
            {
                throw new UnauthorizedAccessException();
            }
            catch (ArgumentException)
            {
                throw new UnauthorizedAccessException();
            }
        }
        public static UserProfile ReadToken(this string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            return new UserProfile
            {
                Id = securityToken.Payload.Claims?.FirstOrDefault(x => x.Type.Equals(ClaimKey.UserId.ToString(), StringComparison.OrdinalIgnoreCase))?.Value
            };
        }

    }
}
