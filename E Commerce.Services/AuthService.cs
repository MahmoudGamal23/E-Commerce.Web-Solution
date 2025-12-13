using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class AuthService : IAutheService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public AuthService(UserManager<ApplicationUser> userManager,IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<bool> CheckEmailAsync(string Email)
        {
            var User = await userManager.FindByEmailAsync(Email);
            return User != null;
        }

        public async Task<Result<UserDTO>> GetUserByEmailAsync(string Email)
        {
            var User = await userManager.FindByEmailAsync(Email);
            if (User == null)
                return Error.NotFound("User.NotFound", $"No User With Email {Email} was Found");
            return new UserDTO(User.Email!, User.DisplayName, await CreateTokenAsync(User));
        }

        public async Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var User = await userManager.FindByEmailAsync(loginDTO.Email);
            if (User == null)
                 Error.InvalidCrendentials("User.InvalidCrendentials");
            var IsPasswardValid = await userManager.CheckPasswordAsync(User, loginDTO.Passward);
            if (!IsPasswardValid)
                 Error.InvalidCrendentials("User.InvalidCrendentials");

            var Token = await CreateTokenAsync(User);

            return new UserDTO(User.Email!, User.DisplayName, Token);
        }

        public async Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO)
        {
            var User = new ApplicationUser()
            {
                Email = registerDTO.Email,
                DisplayName = registerDTO.DisplayName,
                PhoneNumber = registerDTO.PhoneNumber,
                UserName = registerDTO.UserName,
            };
            var IdentityResult = await userManager.CreateAsync(User, registerDTO.Passward);
            if (IdentityResult.Succeeded)
            {
                var Token = await CreateTokenAsync(User);
                return new UserDTO(User.Email, User.DisplayName, Token);
            }
               
            return IdentityResult.Errors.Select(E => Error.Validation(E.Code, E.Description)).ToList();

        }

        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var Claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!)
            };
            var Roles = await userManager.GetRolesAsync(user);
            foreach(var role in Roles) 
                Claims.Add(new Claim(ClaimTypes.Role,role));


            var SecuretyKey = configuration["JWTOptions:SecuretyKey"];
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecuretyKey));
            var Cred = new SigningCredentials(Key,SecurityAlgorithms.HmacSha256);



            var Token = new JwtSecurityToken(
                issuer: configuration["JWTOptions:Issuer"],
                audience: configuration["JWTOptions:Audience"],
                expires: DateTime.UtcNow.AddHours(1),
                claims: Claims,
                signingCredentials: Cred);
                
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }

    }
}