using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace UptimeMonitoring.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    public class UptimeMonitorDb : DbContext
    {
        public UptimeMonitorDb()
            : base("DefaultConnection")
        {
        }

        public DbSet<SiteModel> Sites { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string emailAddress { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class SiteModel
    {
        [Key]
        public int Id { get; set; }

        public string user { get; set; }

        [Required]
        [Display(Name = "Site")]
        public string site_name { get; set; }

        [Required]
        [DataAnnotationsExtensions.Url]
        [Display(Name = "Site URL")]
        public string site_url { get; set; }

        [Display(Name = "Last Check")]
        public string site_last_check { get; set; }

        [Display(Name = "Online")]
        public string site_online { get; set; }

        [Display(Name = "Status Code")]
        public int result { get; set; }

        [Required]
        [Range(5, 30)]
        [Display(Name = "Frequency")]
        public int frequency { get; set; }
    }

    public class StatusModel
    {
        public SiteModel CreateSiteModel { get; set; }
        public List<SiteModel> ListSiteModel { get; set; } 
    }
}
