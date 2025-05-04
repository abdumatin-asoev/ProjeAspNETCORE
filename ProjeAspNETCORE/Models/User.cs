using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjeAspNETCORE.Models
{
    public enum UserRole
    {
        Admin,
        Customer
    }

    [Table("users")]
    public class User : IdentityUser<long>
    {
        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column(TypeName = "varchar(20)")]
        public UserRole Role { get; set; }

        public List<string> GetAuthorities()
        {
            return new List<string> { $"ROLE_{Role.ToString().ToUpper()}" };
        }
    }
}
