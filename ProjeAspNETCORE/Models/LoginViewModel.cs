using System.ComponentModel.DataAnnotations;

namespace ProjeAspNETCORE.Models
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}