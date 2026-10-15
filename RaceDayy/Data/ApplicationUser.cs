using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.ComponentModel.DataAnnotations;
namespace RaceDayy.Data;

//Add profile data for application users by adding properties to the ApplicationUser class

public enum Gender
{
    Male,

    Female,

    Other,

    PreferNotToSay
}
public class ApplicationUser : IdentityUser   
{
    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    [MaxLength(260)]
    public string? ProfileImagePath { get; set; }

    public string FullName => $"{FirstName} {LastName}";

}