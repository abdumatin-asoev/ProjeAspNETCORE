using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjeAspNETCORE.Models
{
	public enum BookCarStatus
	{
		Pending,
		Approved,
		Rejected
	}

	public class BookACar
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Required]
		public DateTime FromDate { get; set; }

		[Required]
		public DateTime ToDate { get; set; }

		public long Days { get; set; }

		public long Price { get; set; }

		public BookCarStatus BookCarStatus { get; set; } = BookCarStatus.Pending;

		[ForeignKey("User")]
		public long UserId { get; set; }

		[JsonIgnore]
		public virtual User User { get; set; }

		[ForeignKey("Car")]
		public long CarId { get; set; }

		[JsonIgnore]
		public virtual Car Car { get; set; }
	}
}