using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LaPescaEnLinea.Models
{
    public class Configuracion
    {
		[Key]
		public int Id { get; set; }
		public string LogoPrincipal { get; set; }
		public string LogoFooter { get; set; }
		public string Direccion { get; set; }
		public string Telefono { get; set; }
		public string Twitter { get; set; }
		public string Facebook { get; set; }
		public string PaginaWeb { get; set; }
		public string Tripadvisor { get; set; }
		public string Linkedin { get; set; }
		public string AgenciaHotel { get; set; }
	}
}
