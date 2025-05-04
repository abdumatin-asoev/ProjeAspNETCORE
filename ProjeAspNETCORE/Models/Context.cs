using ProjeAspNETCORE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ProjeAspNETCORE.Models
{
	public class Context : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public Context(DbContextOptions<Context> options)
       : base(options)
        {
        }
        public DbSet<BookACar> _BookACar { get; set; }
		public DbSet<User> _User { get; set; }
		public DbSet<Car> _Car { get; set; }
	}
}
