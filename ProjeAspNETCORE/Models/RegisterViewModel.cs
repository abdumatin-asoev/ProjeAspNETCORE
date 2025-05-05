using System.ComponentModel.DataAnnotations;

namespace ProjeAspNETCORE.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name cannot be empty!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name cannot be empty!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email cannot be empty!")]
        [EmailAddress(ErrorMessage = "Invalid email format!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username cannot be empty!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password cannot be empty!")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password cannot be empty!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        public string ConfirmPassword { get; set; }
    }
}
