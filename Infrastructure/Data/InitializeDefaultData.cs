﻿using Core.Models.Identity;
using Core.Models.Settings;
using Core.Models.Uploads;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class InitializeDefaultData
    {
        /************************************************************************************
        in the API/program.cs execute this class to add Roles, SuperAdmin and Seed users data
        *************************************************************************************/

        // all default Roles and Users
        public static string[] ROLES = { "SuperAdmin", "Admin", "Editor", "Visitor" };

        // Languages
        public static string[] DefLanguageCodeId = { "en", "ar", "de", "fr", "it", "es", "nl", "sv", "tr" };
        public static string[] DefLanguages = { "English", "العربية", "Deutsche", "Français", "Italiana", "español", "Nederlands", "svenska", "Türk" };
        public static string[] DefLangDirection = { "ltr", "rtl", "ltr", "ltr", "ltr", "ltr", "ltr", "ltr", "ltr" };

        //
        public static string superAdmin_id;
        public static string DefPassword = "!QA1qa";
        public static string UploadImageDir = "/Uploads/Images/";

        //
        static CultureInfo MyCultureInfo = new CultureInfo("de-DE");

        //*********************************************************************************************
        public static async Task<bool> AddDefaultRolesInDatabase(IServiceProvider services)
        {
            // Get RoleManager Service to Application
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            if (!roleManager.Roles.Any())
            {
                for (int i = 0; i < ROLES.Length; i++)
                {
                    // create the Role
                    IdentityRole role = new IdentityRole();
                    if (!roleManager.RoleExistsAsync(ROLES[i]).Result)
                    {
                        role.Name = ROLES[i];
                        await roleManager.CreateAsync(role);
                    }
                }
                return true;
            }
            else
                return false;
        }
        // END Add Default Roles In Database -------------------------------

        //*********************************************************************************************
        public static async Task AddDefaultUplodTypes(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();

            // for now just the image types, maybe leater add fileCV, fileCoverletter, ...
            string[] types = { "imageProfile", "imageCover", "imageBlog" };

            for (int i = 0; i < types.Length; i++)
            {
                var type = new UploadType
                {
                    Name = types[i]
                };
                await context.UploadType.AddAsync(type);
            }
            await context.SaveChangesAsync();
        }
        // END Add Default Uplod Types -------------------------------

        //**********************************************************************************************
        public static async Task AddDefaultLanguages(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();

            for (int i = 0; i < DefLanguages.Length; i++)
            {
                var lan = new Language
                {
                    CodeId = DefLanguageCodeId[i],
                    Name = DefLanguages[i],
                    LanguageDirection = DefLangDirection[i],
                };
                await context.Language.AddAsync(lan);
            }
            await context.SaveChangesAsync();
        }
        // END 

        //**********************************************************************************************
        public static async Task AddSuperAdminUser(IServiceProvider services)
        {
            // Get UserManager Service to Application
            UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();

            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    Email = "ahmad.alloush@gmail.com",
                    EmailConfirmed = true,
                    UserName = "Al-Alloush",
                    FirstName = "Ahmad",
                    LastName = "Alloush",
                    SelectedLanguages = "ar,en,",
                    RegisterDate = DateTime.Now,
                    Birthday = DateTime.Parse("15/6/1978", MyCultureInfo),
                    PhoneNumber = "515885",
                    PhoneNumberConfirmed = true,
                    AutomatedPassword = false,
                    Address = new Address
                    {
                        Street = "Bahn",
                        BuildingNum = "12",
                        Flore = "L 3",
                        City = "xxxxxxx",
                        Zipcode = "8734",
                        Country = "German"
                    }
                };

                string Password = DefPassword;
                IdentityResult create = await userManager.CreateAsync(user, Password);
                if (create.Succeeded)
                {
                    superAdmin_id = user.Id;
                    userManager.AddToRoleAsync(user, ROLES[0]).Wait();
                }

                var context = services.GetRequiredService<AppDbContext>();

                // add default image for SuperAdmin in upload and UserImage tables
                string[] imgName = { "superAdmin", "superAdminCover", "user (10)", "user (15)" };
                for (int i = 0; i < imgName.Length; i++)
                {
                    var typ = i + 1;
                    var def = true;
                    if (i > 1)
                    {
                        // just the first two images are default
                        typ = 3;
                        def = false;
                    }

                    var upload = new Upload
                    {
                        Name = imgName[i],
                        Path = UploadImageDir + imgName[i] + ".jpg",
                        AddedDateTime = DateTime.Now,
                        UserId = superAdmin_id
                    };
                    await context.Upload.AddAsync(upload);
                    await context.SaveChangesAsync();

                    var userImage = new UploadUserImagesList
                    {
                        UploadId = upload.Id,
                        UserId = superAdmin_id,
                        Default = def,
                        UploadTypeId = typ
                    };
                    await context.UploadUserImagesList.AddAsync(userImage);
                    await context.SaveChangesAsync();
                }
            }

        }
        // END Add SuperAdmin User -----------------------------
    }
}
