using E_Commerce.Domain.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Contracts;

namespace E_Commerce.Persistence.IdentityData.DataSeed
{
    
    public class IdentityDataIntializer : IDataInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentityDataIntializer> logger;

        public IdentityDataIntializer(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IdentityDataIntializer> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.logger = logger;
        }
        public async Task InitializeAsync()
        {
            try
            {
                if (!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                }
                if (!_userManager.Users.Any())
                {
                    var User01 = new ApplicationUser()
                    {
                        DisplayName = "Mohamed Tarek",
                        UserName = "MohamedTarek",
                        Email = "MohamedTarek@gmail.com",
                        PhoneNumber = "01202884455"
                    };
                    var User02 = new ApplicationUser()
                    {
                        DisplayName = "Salma Tarek",
                        UserName = "SalmaTarek",
                        Email = "Salmatarek@gmail.com",
                        PhoneNumber = "01202884455"
                    };

                    await _userManager.CreateAsync(User01, "P@ssw0rd");
                    await _userManager.CreateAsync(User02, "P@ssw0rd");

                    await _userManager.AddToRoleAsync(User01, "Admin");
                    await _userManager.AddToRoleAsync(User02, "SuperAdmin");

                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error While Seeding Identity Database : Message = {ex.Message}");
            }
        }
    }
}