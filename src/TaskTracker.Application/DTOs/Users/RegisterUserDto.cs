using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.DTOs.Users;

public class RegisterUserDto
{
    [Required(ErrorMessage = "Login is required.")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "Email has invalid format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;
}