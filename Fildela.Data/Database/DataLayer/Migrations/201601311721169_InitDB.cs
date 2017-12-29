namespace Fildela.Data.Database.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                {
                    UserID = c.Int(nullable: false, identity: true),
                    AccountTypeID = c.Int(nullable: false),
                    FirstName = c.String(maxLength: 150, unicode: false),
                    LastName = c.String(maxLength: 150, unicode: false),
                    Email = c.String(nullable: false, maxLength: 150, unicode: false),
                    PasswordHash = c.String(nullable: false, maxLength: 150, unicode: false),
                    PasswordSalt = c.String(nullable: false, maxLength: 1000, unicode: false),
                    DefaultIpAddress = c.String(nullable: false, maxLength: 50, unicode: false),
                    DefaultEmailAddress = c.String(nullable: false, maxLength: 150, unicode: false),
                    IsDeleted = c.Boolean(nullable: false),
                    AgreeUserAgreement = c.Boolean(nullable: false),
                    DateRegistered = c.DateTime(nullable: false),
                    DateLastActive = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.UserID);

            CreateTable(
                "dbo.News",
                c => new
                {
                    NewsID = c.Int(nullable: false, identity: true),
                    CategoryID = c.Int(nullable: false),
                    PublishedByID = c.Int(nullable: false),
                    PublishedByFullName = c.String(maxLength: 300, unicode: false),
                    PublishedByEmail = c.String(nullable: false, maxLength: 150, unicode: false),
                    Title = c.String(nullable: false, maxLength: 50, unicode: false),
                    PreviewText = c.String(nullable: false),
                    TextBlobName = c.String(nullable: false),
                    ImageBlobURL = c.String(nullable: false),
                    DatePublished = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.NewsID)
                .ForeignKey("dbo.Accounts", t => t.PublishedByID, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: true)
                .Index(t => t.CategoryID)
                .Index(t => t.PublishedByID);

            CreateTable(
                "dbo.Categories",
                c => new
                {
                    CategoryID = c.Int(nullable: false, identity: true),
                    CategoryTypeID = c.Int(nullable: false),
                    Name = c.String(nullable: false, maxLength: 100, unicode: false),
                })
                .PrimaryKey(t => t.CategoryID)
                .ForeignKey("dbo.CategoryTypes", t => t.CategoryTypeID, cascadeDelete: true)
                .Index(t => t.CategoryTypeID);

            CreateTable(
                "dbo.CategoryTypes",
                c => new
                {
                    CategoryTypeID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 100, unicode: false),
                })
                .PrimaryKey(t => t.CategoryTypeID);

            CreateTable(
                "dbo.AccountLinks",
                c => new
                {
                    UserID = c.Int(nullable: false),
                    GuestID = c.Int(nullable: false),
                    DateCreated = c.DateTime(nullable: false),
                    DateStart = c.DateTime(nullable: false),
                    DateExpires = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => new { t.UserID, t.GuestID })
                .ForeignKey("dbo.Accounts", t => t.GuestID)
                .ForeignKey("dbo.Accounts", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.GuestID);

            CreateTable(
                "dbo.AccountLinkPermissions",
                c => new
                {
                    UserID = c.Int(nullable: false),
                    GuestID = c.Int(nullable: false),
                    PermissionID = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.UserID, t.GuestID, t.PermissionID })
                .ForeignKey("dbo.AccountLinks", t => new { t.UserID, t.GuestID }, cascadeDelete: true)
                .ForeignKey("dbo.Permissions", t => t.PermissionID, cascadeDelete: true)
                .Index(t => new { t.UserID, t.GuestID })
                .Index(t => t.PermissionID);

            CreateTable(
                "dbo.Permissions",
                c => new
                {
                    PermissionID = c.Int(nullable: false, identity: true),
                    PermissionTypeID = c.Int(nullable: false),
                    Name = c.String(nullable: false, maxLength: 100, unicode: false),
                })
                .PrimaryKey(t => t.PermissionID)
                .ForeignKey("dbo.PermissionTypes", t => t.PermissionTypeID, cascadeDelete: true)
                .Index(t => t.PermissionTypeID);

            CreateTable(
                "dbo.PermissionTypes",
                c => new
                {
                    PermissionTypeID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 100, unicode: false),
                })
                .PrimaryKey(t => t.PermissionTypeID);

            CreateTable(
                "dbo.AccountAuthenticationProviders",
                c => new
                {
                    AccountID = c.Int(nullable: false),
                    AuthenticationProviderID = c.Int(nullable: false),
                    KeyHash = c.String(nullable: false, maxLength: 1000, unicode: false),
                    KeySalt = c.String(nullable: false, maxLength: 1000, unicode: false),
                })
                .PrimaryKey(t => new { t.AccountID, t.AuthenticationProviderID })
                .ForeignKey("dbo.AuthenticationProviders", t => t.AuthenticationProviderID, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.AccountID, cascadeDelete: true)
                .Index(t => t.AccountID)
                .Index(t => t.AuthenticationProviderID);

            CreateTable(
                "dbo.AuthenticationProviders",
                c => new
                {
                    AuthenticationProviderID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 100, unicode: false),
                    IconClass = c.String(nullable: false, maxLength: 100, unicode: false),
                    IconColor = c.String(nullable: false, maxLength: 100, unicode: false),
                })
                .PrimaryKey(t => t.AuthenticationProviderID);

            CreateTable(
                "dbo.AccountUsagePremiums",
                c => new
                {
                    UserID = c.Int(nullable: false),
                    AllowedFileCount = c.Int(nullable: false),
                    AllowedGuestAccountCount = c.Int(nullable: false),
                    AllowedLinkCount = c.Int(nullable: false),
                    AllowedStoredBytes = c.Long(nullable: false),
                    DateExpires = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.UserID)
                .ForeignKey("dbo.Accounts", t => t.UserID)
                .Index(t => t.UserID);

            CreateTable(
                "dbo.UserRoles",
                c => new
                {
                    UserID = c.Int(nullable: false),
                    RoleID = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.UserID, t.RoleID })
                .ForeignKey("dbo.Roles", t => t.RoleID, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.RoleID);

            CreateTable(
                "dbo.Roles",
                c => new
                {
                    RoleID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 100, unicode: false),
                })
                .PrimaryKey(t => t.RoleID);

            CreateTable(
                "dbo.AccountTypes",
                c => new
                {
                    AccountTypeID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 150, unicode: false),
                })
                .PrimaryKey(t => t.AccountTypeID);

        }

        public override void Down()
        {
            DropForeignKey("dbo.AccountLinks", "UserID", "dbo.Accounts");
            DropForeignKey("dbo.UserRoles", "UserID", "dbo.Accounts");
            DropForeignKey("dbo.UserRoles", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.AccountUsagePremiums", "UserID", "dbo.Accounts");
            DropForeignKey("dbo.AccountAuthenticationProviders", "AccountID", "dbo.Accounts");
            DropForeignKey("dbo.AccountAuthenticationProviders", "AuthenticationProviderID", "dbo.AuthenticationProviders");
            DropForeignKey("dbo.AccountLinks", "GuestID", "dbo.Accounts");
            DropForeignKey("dbo.AccountLinkPermissions", "PermissionID", "dbo.Permissions");
            DropForeignKey("dbo.Permissions", "PermissionTypeID", "dbo.PermissionTypes");
            DropForeignKey("dbo.AccountLinkPermissions", new[] { "UserID", "GuestID" }, "dbo.AccountLinks");
            DropForeignKey("dbo.News", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.Categories", "CategoryTypeID", "dbo.CategoryTypes");
            DropForeignKey("dbo.News", "PublishedByID", "dbo.Accounts");
            DropIndex("dbo.UserRoles", new[] { "RoleID" });
            DropIndex("dbo.UserRoles", new[] { "UserID" });
            DropIndex("dbo.AccountUsagePremiums", new[] { "UserID" });
            DropIndex("dbo.AccountAuthenticationProviders", new[] { "AuthenticationProviderID" });
            DropIndex("dbo.AccountAuthenticationProviders", new[] { "AccountID" });
            DropIndex("dbo.Permissions", new[] { "PermissionTypeID" });
            DropIndex("dbo.AccountLinkPermissions", new[] { "PermissionID" });
            DropIndex("dbo.AccountLinkPermissions", new[] { "UserID", "GuestID" });
            DropIndex("dbo.AccountLinks", new[] { "GuestID" });
            DropIndex("dbo.AccountLinks", new[] { "UserID" });
            DropIndex("dbo.Categories", new[] { "CategoryTypeID" });
            DropIndex("dbo.News", new[] { "PublishedByID" });
            DropIndex("dbo.News", new[] { "CategoryID" });
            DropTable("dbo.AccountTypes");
            DropTable("dbo.Roles");
            DropTable("dbo.UserRoles");
            DropTable("dbo.AccountUsagePremiums");
            DropTable("dbo.AuthenticationProviders");
            DropTable("dbo.AccountAuthenticationProviders");
            DropTable("dbo.PermissionTypes");
            DropTable("dbo.Permissions");
            DropTable("dbo.AccountLinkPermissions");
            DropTable("dbo.AccountLinks");
            DropTable("dbo.CategoryTypes");
            DropTable("dbo.Categories");
            DropTable("dbo.News");
            DropTable("dbo.Accounts");
        }
    }
}
