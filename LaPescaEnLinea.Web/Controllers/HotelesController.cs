using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaPescaEnLinea.Models;
using LaPescaEnLinea.Tools;
using LaPescaEnLinea.Tools.Services;
using LaPescaEnLinea.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaPescaEnLinea.Web.Controllers
{
    [Authorize]
    public class HotelesController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly DataContextLPL _dbContext;
        public HotelesController(IEmailSender emailSender, DataContextLPL dbContext)
        {
            _emailSender = emailSender;
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.PaginaActual = "Hoteles";
            if (!HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
            ViewBag.Color = usuario.ColorTema == "w" ? Url.Content("~/admin/white") : Url.Content("~/admin/black");
            List<Pagina> p = new List<Pagina>();
            p.Add(new Pagina { Actual = true, Nombre = "Hoteles", Url = Url.Content("~/Hoteles") });
            ViewBag.Paginas = p;
            return View();
        }

        public async Task<IActionResult> ListaHoteles()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
            var hoteles = _dbContext.Hoteles.Where(c => c.UsuarioId == usuario.Id);
            return PartialView(hoteles);
        }

        public async Task<IActionResult> AddHotel(int? id)
        {
            ViewBag.PaginaActual = "Hoteles";
            if (!HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
            LaPescaEnLinea.Models.Hoteles hotel = new LaPescaEnLinea.Models.Hoteles();
            if (id != null)
            {
                hotel = _dbContext.Hoteles.Where(c => c.UsuarioId == usuario.Id && c.Id == id).FirstOrDefault();
                if (hotel == null)
                    hotel = new LaPescaEnLinea.Models.Hoteles();
            }
            ViewBag.Color = usuario.ColorTema == "w" ? Url.Content("~/admin/white") : Url.Content("~/admin/black");
            List<Pagina> p = new List<Pagina>();
            p.Add(new Pagina { Actual = false, Nombre = "Hoteles", Url = Url.Content("~/Hoteles") });
            p.Add(new Pagina { Actual = true, Nombre = "Agregar o modificar hotel", Url = Url.Content("~/Hoteles/AddHotel") });
            ViewBag.Paginas = p;

            var instalaciones = _dbContext.Cracteristicas.ToList().OrderBy(c => c.Nombre);
            ViewBag.Instalaciones = instalaciones;
            ViewBag.InstalacionesDivs = instalaciones.Count() / 3;

            return PartialView(hotel);
        }

        public async Task<IActionResult> ImagenesHotel(int id)
        {
            ViewBag.PaginaActual = "Hoteles";
            if (!HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
            ViewBag.Color = usuario.ColorTema == "w" ? Url.Content("~/admin/white") : Url.Content("~/admin/black");
            var imagenes = await _dbContext.ImagenesAsociadas.Where(c => c.HotelId == id && c.HabitacionId==null && c.Activo == true).Include(c => c.Imagenes).OrderByDescending(c=>c.Id).ToListAsync();
            return PartialView(imagenes);
        }

        [HttpPost]
        public async Task<IActionResult> AddHotel(Hotel hotel)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");
                var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
                Hoteles hoteldb = _dbContext.Hoteles.Where(c => c.UsuarioId == usuario.Id && c.Id == hotel.Id).FirstOrDefault();
                if (hoteldb != null)
                {
                    hoteldb.Ciudad = hotel.Ciudad;
                    hoteldb.CodigoPostal = hotel.CodigoPostal;
                    hoteldb.Descripcion = hotel.Descripcion;
                    hoteldb.Direccion = hotel.Direccion;
                    hoteldb.Email = hotel.Email;
                    hoteldb.Estado = hotel.Estado;
                    hoteldb.Facebook = hotel.Facebook;
                    hoteldb.Facturacion = hotel.Facturacion;
                    hoteldb.Instagram = hotel.Instagram;
                    hoteldb.Latitud = hotel.Latitud;
                    hoteldb.LinkedIn = hotel.LinkedIn;
                    hoteldb.Longitud = hotel.Longitud;
                    hoteldb.Nombre = hotel.Nombre;
                    hoteldb.PalabrasClave = hotel.PalabrasClave;
                    hoteldb.Pinterest = hotel.Pinterest;
                    hoteldb.Telefono = hotel.Telefono;
                    hoteldb.Twitter = hotel.Twitter;
                    hoteldb.Website = hotel.Website;
                    hoteldb.WhatsApp = hotel.WhatsApp;
                    _dbContext.Update(hoteldb);
                }
                else
                {
                    hoteldb = new Hoteles
                    {
                        Ciudad = hotel.Ciudad,
                        CodigoPostal = hotel.CodigoPostal,
                        Descripcion = hotel.Descripcion,
                        Direccion = hotel.Direccion,
                        Email = hotel.Email,
                        Estado = hotel.Estado,
                        Facebook = hotel.Facebook,
                        Facturacion = hotel.Facturacion,
                        Instagram = hotel.Instagram,
                        Latitud = hotel.Latitud,
                        LinkedIn = hotel.LinkedIn,
                        Longitud = hotel.Longitud,
                        Nombre = hotel.Nombre,
                        PalabrasClave = hotel.PalabrasClave,
                        Pinterest = hotel.Pinterest,
                        Telefono = hotel.Telefono,
                        Twitter = hotel.Twitter,
                        Website = hotel.Website,
                        WhatsApp = hotel.WhatsApp,
                        Calificacion = 5,
                        Estatus = true,
                        Fecha = DateTime.Now,
                        UsuarioId = usuario.Id
                    };
                    _dbContext.Add(hoteldb);
                }
                await _dbContext.SaveChangesAsync();
                if (hoteldb.Id > 0 && hotel.Caracteristica != null)
                {
                    var cars = _dbContext.HotelesHabitacionesCaracteristicas.Where(c => c.HotelId == hoteldb.Id && !hotel.Caracteristica.Contains(c.CaracteristicaId));
                    foreach (var item in cars)
                    {
                        item.Activo = false;
                        _dbContext.Update(item);
                    }
                    foreach (var item in hotel.Caracteristica)
                    {
                        if (item > 0)
                        {
                            HotelesHabitacionesCaracteristicas car = _dbContext.HotelesHabitacionesCaracteristicas.Where(c => c.HotelId == hoteldb.Id && c.CaracteristicaId == item).FirstOrDefault();
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
                                    HotelId = hoteldb.Id
                                };
                                _dbContext.Add(car);
                            }
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                }
                return new JsonResult(new Response { IsSuccess = true, Message = "Se guardaron los datos correctamente", Id = hoteldb.Id });
            }
            catch (Exception ex)
            {
                await _emailSender.SendEmailAsync("j.98.alejandro@gmail.com", "Error La Pesca en Línea ", ex.ToString());
                return new JsonResult(new Response { IsSuccess = false, Message = "No se pudieron guardar los datos, intentelo más tarde." });
            }

        }

        [HttpPost]
        public async Task<IActionResult> DesactivarActivar(int HotelId, string MovimientoActDesact)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");
                var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
                Hoteles hoteldb = _dbContext.Hoteles.Where(c => c.UsuarioId == usuario.Id && c.Id == HotelId).FirstOrDefault();
                if (hoteldb != null)
                {
                    Boolean movimiento = MovimientoActDesact == "A" ? true : false;
                    hoteldb.Estatus = movimiento;
                    _dbContext.Update(hoteldb);
                    await _dbContext.SaveChangesAsync();
                    string strmensaje = MovimientoActDesact == "A" ? "activo" : "desactivo";
                    return new JsonResult(new Response { IsSuccess = true, Message = "Se " + strmensaje + " el hotel correctamente", DivTabla= "#tblHoteles", Url= @Url.Content("~/Hoteles/ListaHoteles") });
                }
                else
                {
                    return new JsonResult(new Response { IsSuccess = false, Message = "No se encuentra el hotel que quiere desactivar" });
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