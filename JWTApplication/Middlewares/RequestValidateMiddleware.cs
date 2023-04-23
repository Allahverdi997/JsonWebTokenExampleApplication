using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWTApplication.Middlewares
{
    public class RequestValidateMiddleware
    {
        public IConfiguration configuration { get; set; }
        public RequestDelegate request { get; set; }
        public RequestValidateMiddleware(RequestDelegate _request, IConfiguration _configuration)
        {
            configuration = _configuration;
            request= _request;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var requestStream = new MemoryStream();
            await context.Request.Body.CopyToAsync(requestStream);
            var name = new StreamReader(requestStream).ReadToEnd();
            if (name=="Ali")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(configuration.GetSection("JwtKey").ToString());

                var tokenDecription = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Ali"), new Claim(ClaimTypes.Role, "Admin") }),
                    Expires = DateTime.UtcNow.AddSeconds(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDecription);
                string tokenKey = tokenHandler.WriteToken(token);
                var jeton =Encoding.ASCII.GetBytes(tokenKey);
                await context.Response.Body.WriteAsync(jeton);
                context.Request.Body.Seek(0, SeekOrigin.Begin);
                
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }
            await request.Invoke(context);
        }
    }
}
