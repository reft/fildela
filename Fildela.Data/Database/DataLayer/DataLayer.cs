using Fildela.Data.Database.DataLayer.Migrations;
using Fildela.Data.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;
using System.Linq;

namespace Fildela.Data.Database.DataLayer
{
    public class DataLayer : DbContext
    {
        public DataLayer()
            : base("FildelaDatabase")
        {
            Configuration.LazyLoadingEnabled = false;
            System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataLayer, Configuration>());
        }
        public DbSet<User> User { get; set; }
        public DbSet<Guest> Guest { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<AccountType> AccountType { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<AccountLink> AccountLink { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<PermissionType> PermissionType { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<CategoryType> CategoryTypes { get; set; }
        public DbSet<AccountUsagePremium> AccountUsagePremium { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<AccountLinkPermission> AccountLinkPermission { get; set; }
        public DbSet<AuthenticationProvider> AuthenticationProvider { get; set; }
        public DbSet<AccountAuthenticationProvider> AccountAuthenticationProvider { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().ToTable("Users");
            //modelBuilder.Entity<Guest>().ToTable("Guests");

            modelBuilder.Entity<Account>()
                .Map<User>(m => m.Requires("AccountTypeID").HasValue(1))
                .Map<Guest>(m => m.Requires("AccountTypeID").HasValue(2));

            modelBuilder.Entity<AccountLink>().HasRequired(p => p.Guest).WithMany(m => m.AccountLinks).WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //    .Property(t => t.Email)
            //    .HasColumnAnnotation(
            //        IndexAnnotation.AnnotationName,
            //        new IndexAnnotation(
            //        new IndexAttribute("IX_EMAIL") { IsUnique = true }));

            //modelBuilder.Entity<Guest>()
            //    .Property(t => t.Email)
            //    .HasColumnAnnotation(
            //        IndexAnnotation.AnnotationName,
            //        new IndexAnnotation(
            //        new IndexAttribute("IX_EMAIL") { IsUnique = true }));

            base.OnModelCreating(modelBuilder);
        }

        //Get news SP
        public List<News> GetNews(Nullable<int> newsID, string newsTitle, Nullable<int> categoryID, Nullable<int> orderByTitle, Nullable<int> orderByPublisher, Nullable<int> orderByDate, Nullable<int> orderByCategory)
        {
            var newsIDParam = newsID.HasValue ?
                new SqlParameter("@NewsID", newsID) :
                new SqlParameter("@NewsID", DBNull.Value);

            var newsTitleParam = string.IsNullOrEmpty(newsTitle) ?
                new SqlParameter("@NewsTitle", DBNull.Value) :
                new SqlParameter("@NewsTitle", newsTitle);

            var categoryIDParam = categoryID.HasValue ?
                new SqlParameter("@CategoryID", categoryID) :
                new SqlParameter("@CategoryID", DBNull.Value);

            var orderByTitleParam = orderByTitle.HasValue ?
                new SqlParameter("@OrderByTitle", orderByTitle) :
                new SqlParameter("@OrderByTitle", DBNull.Value);

            var orderByPublisherParam = orderByPublisher.HasValue ?
                new SqlParameter("@OrderByPublisher", orderByPublisher) :
                new SqlParameter("@OrderByPublisher", DBNull.Value);

            var orderByDateParam = orderByDate.HasValue ?
                new SqlParameter("@OrderByDate", orderByDate) :
                new SqlParameter("@OrderByDate", DBNull.Value);

            var orderByCategoryParam = orderByCategory.HasValue ?
                new SqlParameter("@OrderByCategory", orderByCategory) :
                new SqlParameter("@OrderByCategory", DBNull.Value);

            object[] sqlParam = new object[] { newsIDParam, newsTitleParam, categoryIDParam, orderByTitleParam, orderByPublisherParam, orderByDateParam, orderByCategoryParam };

            List<News> news = this.Database.SqlQuery<News>("GetNews @NewsID, @NewsTitle, @CategoryID, @OrderByTitle, @OrderByPublisher, @OrderByDate, @OrderByCategory", sqlParam).ToList();

            return news;
        }
    }
}
