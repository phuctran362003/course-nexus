using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Test.UTils
{
    public class Token
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public Token(Mock<IHttpContextAccessor> mockHttpContextAccessor)
        {
            _mockHttpContextAccessor = mockHttpContextAccessor;
        }

        public void SetupMockHttpContext(string token)
        {
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(req => req.Headers).Returns(new HeaderDictionary
        {
            { "Authorization", $"Bearer {token}" }
        });

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Request).Returns(mockRequest.Object);

            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            var key = "your-256-bit-secret-key-1234567890123456";
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

            var jwtToken = new JwtSecurityTokenHandler().CreateJwtSecurityToken(
                issuer: "testIssuer",
                audience: "testAudience",
                subject: new ClaimsIdentity(new Claim[] { new Claim("id", "1") }),
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            mockRequest.Setup(req => req.Headers["Authorization"]).Returns(new[] { $"Bearer {tokenString}" });
        }
    }
}
