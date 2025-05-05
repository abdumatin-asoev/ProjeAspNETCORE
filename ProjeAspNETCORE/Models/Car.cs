using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjeAspNETCORE.Models
{
	public class Car
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Required(ErrorMessage = "This Field is Required!")]
		public string Brand { get; set; }

        [Required(ErrorMessage = "This Field is Required!")]
        public string Color { get; set; }

        [Required(ErrorMessage = "This Field is Required!")]
		public string Name { get; set; }

        [Required(ErrorMessage = "This Field is Required!")]
        public string Type { get; set; }

        [Required(ErrorMessage = "This Field is Required!")]
        public string Transmission { get; set; }

        [Required(ErrorMessage = "This Field is Required!")]
        public string Description { get; set; }

		[Range(0, long.MaxValue)]
        [Required(ErrorMessage = "This Field is Required!")]
        public long Price { get; set; }

		[Range(1900, 2025)]
        [Required(ErrorMessage = "This Field is Required!")]
        public int Year { get; set; }

		[Column("image_path")]
        public string ImagePath { get; set; }
	}
}