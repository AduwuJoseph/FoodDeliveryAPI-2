using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FoodDeliveryApp.API.Models;
using FoodDeliveryApp.API.Services.Interfaces;
using FoodDeliveryApp.API.Helpers;

namespace FoodDeliveryApp.API.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly AppSettings appSettings;
        public AuthenticateService(IOptions<AppSettings> _appSettings)
        {
            appSettings = _appSettings.Value;
        }
        List<AspNetUserVM> users = new List<AspNetUserVM>()
        {
            new AspNetUserVM{ Id= Helper.GenerateGuid(), Email="aduwujoseph4real@gmail.com", UserName="aduwujoseph4real@gmail.com", Password = "Pa$$w00rd"}
        };
        public AspNetUserVM Authenticate(AspNetUserVM vm)
        {
            var user = users.SingleOrDefault(x => x.UserName == vm.UserName && x.Password == vm.Password);
            if (user == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.Version, "V3.1")
                }),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            user.Password = null;
            return user;
        }
    }
}
