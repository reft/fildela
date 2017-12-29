namespace Fildela.Data.Database.DataLayer.Migrations
{
    using Fildela.Data.Database.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DataLayer>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Fildela.Data.Database.DataLayer.DataLayer";
            AutomaticMigrationDataLossAllowed = true;
            MigrationsDirectory = "Database\\DataLayer\\Migrations";
        }

        protected override void Seed(DataLayer context)
        {
            context.AccountType.AddOrUpdate(
                r => r.AccountTypeID,
                new AccountType { Name = "User", AccountTypeID = 1 },
                new AccountType { Name = "Guest", AccountTypeID = 2 });

            context.Role.AddOrUpdate(
                r => r.RoleID,
                new Role { Name = "AccountOwner", RoleID = 1 },
                new Role { Name = "Publisher", RoleID = 2 },
                new Role { Name = "Support", RoleID = 3 },
                new Role { Name = "Admin", RoleID = 4 });

            context.PermissionType.AddOrUpdate(
                m => m.PermissionTypeID,
                new PermissionType { Name = "File", PermissionTypeID = 1 },
                new PermissionType { Name = "Link", PermissionTypeID = 2 });

            context.CategoryTypes.AddOrUpdate(
                c => c.CategoryTypeID,
                new CategoryType { Name = "Contact", CategoryTypeID = 1 },
                new CategoryType { Name = "News", CategoryTypeID = 2 });

            context.AuthenticationProvider.AddOrUpdate(
                s => s.AuthenticationProviderID,
                new AuthenticationProvider { AuthenticationProviderID = 1, Name = "Google", IconClass = "fa fa-google-plus", IconColor = "#e0492f" },
                new AuthenticationProvider { AuthenticationProviderID = 2, Name = "Facebook", IconClass = "fa fa-facebook", IconColor = "#3a5795" });

            context.SaveChanges();

            context.Category.AddOrUpdate(
                c => c.CategoryID,
                new Category { Name = "Suggestion", CategoryTypeID = 1, CategoryID = 1 },
                new Category { Name = "Support", CategoryTypeID = 1, CategoryID = 2 },
                new Category { Name = "Thoughts", CategoryTypeID = 1, CategoryID = 3 },
                new Category { Name = "Other", CategoryTypeID = 1, CategoryID = 4 },

                new Category { Name = "Update", CategoryTypeID = 2, CategoryID = 5 },
                new Category { Name = "Functionality", CategoryTypeID = 2, CategoryID = 6 },
                new Category { Name = "Coming", CategoryTypeID = 2, CategoryID = 7 },
                new Category { Name = "Other", CategoryTypeID = 2, CategoryID = 8 });

            context.Permission.AddOrUpdate(
                r => r.PermissionID,
                new Permission { Name = "Read", PermissionID = 1, PermissionTypeID = 1 },
                new Permission { Name = "Write", PermissionID = 2, PermissionTypeID = 1 },
                new Permission { Name = "Edit", PermissionID = 3, PermissionTypeID = 1 },
                new Permission { Name = "Read", PermissionID = 4, PermissionTypeID = 2 },
                new Permission { Name = "Write", PermissionID = 5, PermissionTypeID = 2 },
                new Permission { Name = "Edit", PermissionID = 6, PermissionTypeID = 2 });
        }
    }
}