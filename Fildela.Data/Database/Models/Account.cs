using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fildela.Data.Database.Models
{
    public abstract class Account
    {
        public Account()
        {
            News = new HashSet<News>();
        }

        public Account(string firstName, string lastName, string email,
            string passwordHash, string passwordSalt, bool agreeUserAgreement)
        {
            News = new HashSet<News>();
            this.FirstName = firstName ?? string.Empty;
            this.LastName = lastName ?? string.Empty;
            this.Email = email;
            this.DateRegistered = Helpers.DataTimeZoneExtensions.GetCurrentDate();
            this.DateLastActive = Helpers.DataTimeZoneExtensions.GetCurrentDate();
            this.PasswordHash = passwordHash;
            this.PasswordSalt = passwordSalt;
            this.IsDeleted = false;
            this.AgreeUserAgreement = agreeUserAgreement;
        }

        [Key]
        public int UserID { get; set; }

        [StringLength(150)]
        [Column(TypeName = "VARCHAR")]
        public string FirstName { get; set; }

        [StringLength(150)]
        [Column(TypeName = "VARCHAR")]
        public string LastName { get; set; }

        [Required]
        [StringLength(150)]
        [Column(TypeName = "VARCHAR")]
        public string Email { get; set; }

        [Required]
        [StringLength(150)]
        [Column(TypeName = "VARCHAR")]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(1000)]
        [Column(TypeName = "VARCHAR")]
        public string PasswordSalt { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string DefaultIpAddress { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 6)]
        [Column(TypeName = "VARCHAR")]
        public string DefaultEmailAddress { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public bool AgreeUserAgreement { get; set; }

        public DateTime DateRegistered { get; set; }
        public DateTime DateLastActive { get; set; }

        public ICollection<News> News { get; set; }
    }
}
