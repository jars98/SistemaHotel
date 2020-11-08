using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LaPescaEnLinea.Models
{
	public class ImagenesAsociadas
    {
		[Key]
		public int Id { get; set; }
		[ForeignKey("Imagenes")]
		public int ImagenId { get; set; }
		[ForeignKey("Hoteles")]
		public int? HotelId { get; set; }
		[ForeignKey("Habitaciones")]
		public int? HabitacionId { get; set; }
		[ForeignKey("ServiciosExtras")]
		public int? ServicioId { get; set; }
		[ForeignKey("Blog")]
		public int? BlogId { get; set; }
		public bool Activo { get; set; }		
		public virtual Imagenes Imagenes { get; set; }
		public virtual Blog Blog { get; set; }
		public virtual Habitaciones Habitaciones { get; set; }
		public virtual Hoteles Hoteles { get; set; }
		public virtual ServiciosExtras ServiciosExtras { get; set; }
	}
}
