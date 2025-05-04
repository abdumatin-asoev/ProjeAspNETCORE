using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjeAspNETCORE.Models
{
	public class Car
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Required]
		[StringLength(50)]
		public string Brand { get; set; }

		[StringLength(30)]
		public string Color { get; set; }

		[Required]
		[StringLength(50)]
		public string Name { get; set; }

		[StringLength(30)]
		public string Type { get; set; }

		[StringLength(20)]
		public string Transmission { get; set; }

		[StringLength(500)]
		public string Description { get; set; }

		[Range(0, long.MaxValue)]
		public long Price { get; set; }

		[Range(1900, 2100)]
		public int Year { get; set; }

		[Column("image_path")]
		[StringLength(255)]
		public string ImagePath { get; set; }
	}
}