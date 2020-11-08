using System;
using System.Collections.Generic;
using System.Text;

namespace LaPescaEnLinea.ViewModels
{
    public class Habitacion
    {
		public int Id { get; set; }
		public string Titulo { get; set; }
		public string PalabrasClave { get; set; }
		public string Descripcion { get; set; }
		public decimal Costo { get; set; }
		public int Adultos { get; set; }
		public int Ninos { get; set; }
		public int Infantes { get; set; }
		public int MaximoAdultos { get; set; }
		public int MaximoNinos { get; set; }
		public int MaximoInfantes { get; set; }
		public int CostoAdicionalAdulto { get; set; }
		public int CostoAdicionalNino { get; set; }
		public int CostoAdicionalInfante { get; set; }
		public int TotalHabitaciones { get; set; }
		public int CamaIndividual { get; set; }
		public int CamaDoble { get; set; }
		public int CamaQueenSize { get; set; }
		public int CamaKingSize { get; set; }
		public decimal Calificacion { get; set; }
		public bool Activo { get; set; }
		public int HotelId { get; set; }
		public int[]? Caracteristica { get; set; }
	}
}
