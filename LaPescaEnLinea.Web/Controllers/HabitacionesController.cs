using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaPescaEnLinea.Models;
using LaPescaEnLinea.Tools;
using LaPescaEnLinea.Tools.Services;
using LaPescaEnLinea.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LaPescaEnLinea.Web.Controllers
{
    public class HabitacionesController : Controller
    {
        private readonly DataContextLPL _dbContext;
        private readonly IEmailSender _emailSender;


        public HabitacionesController(DataContextLPL dataContext, IEmailSender emailSender)
        {
            _dbContext = dataContext;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> Index(int? Id)
        {
            ViewBag.PaginaActual = "Habitaciones";
            if (!HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
            ViewBag.Color = usuario.ColorTema == "w" ? Url.Content("~/admin/white") : Url.Content("~/admin/black");
            List<Pagina> p = new List<Pagina>();
            p.Add(new Pagina { Actual = true, Nombre = "Habitaciones", Url = Url.Content("~/Habitaciones") });
            ViewBag.Paginas = p;
            ViewBag.HotelId = Id;
            return View();
        }

        public async Task<IActionResult> ListaHabitaciones(int? IdHotel)
        {
            ViewBag.HotelId = IdHotel;
            if (!HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
            if (IdHotel != null)
            {
                var habitaciones = _dbContext.Hoteles.Where(c => c.Id == IdHotel && c.UsuarioId == usuario.Id).Include(c => c.Habitaciones);
                return PartialView(habitaciones);
            }
            else
            {
                var habitaciones = _dbContext.Hoteles.Where(c => c.UsuarioId == usuario.Id).Include(c => c.Habitaciones);
                return PartialView(habitaciones);
            }
        }
        public async Task<IActionResult> AddHabitacion(int? id)
        {
            ViewBag.PaginaActual = "Habitaciones";
            if (!HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
            LaPescaEnLinea.Models.Habitaciones habitacion = new LaPescaEnLinea.Models.Habitaciones();
            var hotel = _dbContext.Hoteles.Where(c => c.UsuarioId == usuario.Id && c.Estatus==true);
            ViewBag.Hoteles = hotel;
            if (id != null)
            {
                var ids = hotel.Select(c => c.Id);
                habitacion = _dbContext.Habitaciones.Where(c => ids.Contains(c.HotelId) && c.Id == id).FirstOrDefault();
                if (habitacion == null)
                    habitacion = new LaPescaEnLinea.Models.Habitaciones();
            }
            ViewBag.Color = usuario.ColorTema == "w" ? Url.Content("~/admin/white") : Url.Content("~/admin/black");
            List<Pagina> p = new List<Pagina>();
            p.Add(new Pagina { Actual = false, Nombre = "Habitaciones", Url = Url.Content("~/Habitaciones") });
            p.Add(new Pagina { Actual = true, Nombre = "Agregar o modificar habitación", Url = Url.Content("~/Habitaciones/AddHabitacion") });
            ViewBag.Paginas = p;

            var instalaciones = _dbContext.Cracteristicas.ToList().OrderBy(c => c.Nombre);
            ViewBag.Instalaciones = instalaciones;
            ViewBag.InstalacionesDivs = instalaciones.Count() / 3;

            return PartialView(habitacion);
        }

        [HttpPost]
        public async Task<IActionResult> AddHabitacion(Habitacion hab)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");
                var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
                Hoteles hoteldb = _dbContext.Hoteles.Where(c => c.UsuarioId == usuario.Id && c.Id == hab.HotelId).Include(c => c.Habitaciones).FirstOrDefault();
                Habitaciones habita = new Habitaciones();
                if (hoteldb != null)
                {
                    habita = hoteldb.Habitaciones.Where(c => c.Id == hab.Id).FirstOrDefault();
                    if (habita != null)
                    {
                        habita.Adultos = hab.Adultos;
                        habita.CamaDoble = hab.CamaDoble;
                        habita.CamaIndividual = hab.CamaIndividual;
                        habita.CamaKingSize = hab.CamaKingSize;
                        habita.CamaQueenSize = hab.CamaQueenSize;
                        habita.Costo = hab.Costo;
                        habita.CostoAdicionalAdulto = hab.CostoAdicionalAdulto;
                        habita.CostoAdicionalInfante = hab.CostoAdicionalInfante;
                        habita.CostoAdicionalNino = hab.CostoAdicionalNino;
                        habita.Descripcion = hab.Descripcion ?? "";
                        habita.Infantes = hab.Infantes;
                        habita.MaximoAdultos = hab.MaximoAdultos;
                        habita.MaximoInfantes = hab.MaximoInfantes;
                        habita.MaximoNinos = hab.MaximoNinos;
                        habita.Ninos = hab.Ninos;
                        habita.Titulo = hab.Titulo;
                        habita.TotalHabitaciones = hab.TotalHabitaciones;
                        _dbContext.Update(habita);
                    }
                    else
                    {
                        habita = new Habitaciones
                        {
                            Adultos = hab.Adultos,
                            CamaDoble = hab.CamaDoble,
                            CamaIndividual = hab.CamaIndividual,
                            CamaKingSize = hab.CamaKingSize,
                            CamaQueenSize = hab.CamaQueenSize,
                            Costo = hab.Costo,
                            CostoAdicionalAdulto = hab.CostoAdicionalAdulto,
                            CostoAdicionalInfante = hab.CostoAdicionalInfante,
                            CostoAdicionalNino = hab.CostoAdicionalNino,
                            Descripcion = hab.Descripcion ?? "",
                            Infantes = hab.Infantes,
                            MaximoAdultos = hab.MaximoAdultos,
                            MaximoInfantes = hab.MaximoInfantes,
                            MaximoNinos = hab.MaximoNinos,
                            Ninos = hab.Ninos,
                            Titulo = hab.Titulo,
                            TotalHabitaciones = hab.TotalHabitaciones,
                            Activo = true,
                            Calificacion = 0,
                            HotelId = hab.HotelId,
                            PalabrasClave = ""
                        };
                        _dbContext.Add(habita);
                    }
                    await _dbContext.SaveChangesAsync();
                    if (habita.Id > 0 && hab.Caracteristica != null)
                    {
                        var cars = _dbContext.HotelesHabitacionesCaracteristicas.Where(c => c.HabitacionId == habita.Id && !hab.Caracteristica.Contains(c.CaracteristicaId));
                        foreach (var item in cars)
                        {
                            item.Activo = false;
                            _dbContext.Update(item);
                        }
                        foreach (var item in hab.Caracteristica)
                        {
                            if (item > 0)
                            {
                                HotelesHabitacionesCaracteristicas car = _dbContext.HotelesHabitacionesCaracteristicas.Where(c => c.HabitacionId == habita.Id && c.CaracteristicaId == item).FirstOrDefault();
                                if (car != null)
                                {
                                    car.Activo = true;
                                    _dbContext.Update(car);
                                }
                                else
                                {
                                    car = new HotelesHabitacionesCaracteristicas
                                    {
                                        Activo = true,
                                        CaracteristicaId = item,
                                        HabitacionId = habita.Id
                                    };
                                    _dbContext.Add(car);
                                }
                            }
                        }
                        await _dbContext.SaveChangesAsync();
                    }
                    return new JsonResult(new Response { IsSuccess = true, Message = "Se guardaron los datos correctamente", Id = habita.Id });
                }
                return new JsonResult(new Response { IsSuccess = false, Message = "No se encontro el hotel, intentelo más tarde." });
            }
            catch (Exception ex)
            {
                await _emailSender.SendEmailAsync("j.98.alejandro@gmail.com", "Error La Pesca en Línea ", ex.ToString());
                return new JsonResult(new Response { IsSuccess = false, Message = "No se pudieron guardar los datos, intentelo más tarde." });
            }

        }

        public async Task<IActionResult> ImagenesHabitacion(int id)
        {
            ViewBag.PaginaActual = "Habitaciones";
            if (!HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
            ViewBag.Color = usuario.ColorTema == "w" ? Url.Content("~/admin/white") : Url.Content("~/admin/black");
            var imagenes = await _dbContext.ImagenesAsociadas.Where(c => c.HabitacionId == id && c.Activo == true).Include(c => c.Imagenes).OrderByDescending(c => c.Id).ToListAsync();
            return PartialView(imagenes);
        }

        [HttpPost]
        public async Task<IActionResult> DesactivarActivar(int HabitacionId, string MovimientoActDesact)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");
                var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
                var hoteldb = _dbContext.Hoteles.Where(c => c.UsuarioId == usuario.Id).Select(c=>c.Id);
                var habita = _dbContext.Habitaciones.Where(c => hoteldb.Contains(c.HotelId) && c.Id == HabitacionId).FirstOrDefault();
                if (habita != null)
                {
                    
                    Boolean movimiento = MovimientoActDesact == "A" ? true : false;
                    habita.Activo = movimiento;
                    _dbContext.Update(habita);
                    await _dbContext.SaveChangesAsync();
                    string strmensaje = MovimientoActDesact == "A" ? "activo" : "desactivo";
                    return new JsonResult(new Response { IsSuccess = true, Message = "Se " + strmensaje + " la habitación correctamente", DivTabla = "#tblHabitaciones", Url = @Url.Content("~/Habitaciones/ListaHabitaciones") });
                }
                else
                {
                    return new JsonResult(new Response { IsSuccess = false, Message = "No se encuentra la habitación que quiere desactivar" });
                }


            }
            catch (Exception ex)
            {
                await _emailSender.SendEmailAsync("j.98.alejandro@gmail.com", "Error La Pesca en Línea ", ex.ToString());
                return new JsonResult(new Response { IsSuccess = false, Message = "No se pudieron guardar los datos, intentelo más tarde." });
            }

        }
    }
}