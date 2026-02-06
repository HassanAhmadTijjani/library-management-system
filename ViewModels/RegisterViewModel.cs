using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be atleast 8 characters length!")]

    public string Password { get; set; } = string.Empty;
    [Required]
    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password does not match!")]    public string ConfirmPassword { get; set; } = string.Empty;

}
