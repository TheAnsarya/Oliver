using System;
using System.ComponentModel.DataAnnotations;

namespace Oliver.Domain {
	public abstract class Entity {
		[Key]
		public Guid Id { get; set; }

		public DateTime CreatedDate { get; set; }

		public DateTime UpdatedDate { get; set; }
	}
}
