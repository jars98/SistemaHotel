using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LaPescaEnLinea.Web.Models;
using LaPescaEnLinea.Models;
using LaPescaEnLinea.Tools.Services;
using LaPescaEnLinea.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LaPescaEnLinea.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContextLPL _dbContext;
        private readonly IEmailSender _emailSender;


        public HomeController(ILogger<HomeController> logger, DataContextLPL dataContext, IEmailSender emailSender)
        {
            _logger = logger;
            _dbContext = dataContext;
            _emailSender = emailSender;
        }

        public IActionResult Index(string ReturnUrl = null)
        {

            ViewData["ReturnUrl"] = ReturnUrl;
            var blog = _dbContext.Blog.OrderByDescending(c => c.FechaModificacion).FirstOrDefault() ?? new Blog();
            var config = _dbContext.Configuracion.FirstOrDefault() ?? new Configuracion();
            ViewBag.Config = config;
            HotelesTop(config);
            HotelesRemcomendados(config);
            BlogsMasRecientes();
            return View(blog);
        }

        public void BlogsMasRecientes()
        {
            ViewBag.BlogRecientes = _dbContext.Blog.Where(c => c.Activo == true).OrderByDescending(c=>c.FechaModificacion).Take(5);
        }
        public void HotelesRemcomendados(Configuracion config)
        {
            if (config.AgenciaHotel == "Agencia")
            {
                List<Hoteles> hoteles = _dbContext.Hoteles.Where(c => c.Estatus == true).OrderByDescending(c => c.Calificacion).Take(5).Include(c => c.Habitaciones).ToList();
                foreach (var item in hoteles)
                {
                    item.ImagenesAsociadas = _dbContext.ImagenesAsociadas.Where(c => c.Activo == true && c.HotelId == item.Id).ToList();
                    foreach (var itemchild in item.ImagenesAsociadas)
                    {
                        itemchild.Imagenes = _dbContext.Imagenes.Where(c => c.Activo == true && c.Id == itemchild.ImagenId).FirstOrDefault();

                    }
                }
                ViewBag.HotelesRecomendados = hoteles;
            }
            else
            {
                List<Habitaciones> habitaciones = _dbContext.Habitaciones.Where(c => c.Activo == true).OrderByDescending(c => c.Calificacion).Take(5).Include(c => c.ImagenesAsociadas).ToList();
                foreach (var item in habitaciones)
                {
                    foreach (var itemchild in item.ImagenesAsociadas)
                    {
                        itemchild.Imagenes = _dbContext.Imagenes.Where(c => c.Activo == true && c.Id == itemchild.ImagenId).FirstOrDefault();

                    }
                }
                ViewBag.HabitacionesRecomendados = habitaciones;
            }
        }

        public void HotelesTop(Configuracion config)
        {
            if (config.AgenciaHotel == "Agencia")
            {
                List<int> ids = _dbContext.Reservas.GroupBy(c => c.HotelId).Select(c => new { Id = c.Key, Count = c.Count() }).OrderByDescending(c => c.Count).Select(c => c.Id).Take(5).ToList();
                int idsSeleccionados = ids.Count();
                int idsFaltantes = 5 - idsSeleccionados;
                if (idsFaltantes > 0)
                {
                    ids.AddRange(_dbContext.Hoteles.Where(c => c.Estatus == true && !ids.Contains(c.Id)).Select(c => c.Id).Take(idsFaltantes));
                }
                List<Hoteles> hoteles = _dbContext.Hoteles.Where(c => c.Estatus == true && ids.Contains(c.Id)).Take(5).Include(c => c.Habitaciones).ToList();
                foreach (var item in hoteles)
                {
                    item.ImagenesAsociadas = _dbContext.ImagenesAsociadas.Where(c => c.Activo == true && c.HotelId == item.Id).ToList();
                    foreach (var itemchild in item.ImagenesAsociadas)
                    {
                        itemchild.Imagenes = _dbContext.Imagenes.Where(c => c.Activo == true && c.Id == itemchild.ImagenId).FirstOrDefault();

                    }
                }
                ViewBag.HotelesTop = hoteles;
            }
            else
            {
                List<int> ids = _dbContext.ReservaDetalle.GroupBy(c => c.HabitacionId).Select(c => new { Id = c.Key, Count = c.Count() }).OrderByDescending(c => c.Count).Select(c => c.Id).Take(5).ToList();
                int idsSeleccionados = ids.Count();
                int idsFaltantes = 5 - idsSeleccionados;
                if (idsFaltantes > 0)
                {
                    ids.AddRange(_dbContext.Habitaciones.Where(c => c.Activo == true && !ids.Contains(c.Id)).Select(c => c.Id).Take(idsFaltantes));
                }
                List<Habitaciones> habitaciones = _dbContext.Habitaciones.Where(c => c.Activo == true && ids.Contains(c.Id)).Take(5).Include(c => c.ImagenesAsociadas).ToList();
                foreach (var item in habitaciones)
                {
                    foreach (var itemchild in item.ImagenesAsociadas)
                    {
                        itemchild.Imagenes = _dbContext.Imagenes.Where(c => c.Activo == true && c.Id == itemchild.ImagenId).FirstOrDefault();

                    }
                }
                ViewBag.HabitacionesTop = habitaciones;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Suscribirme(string txtEmail)
        {
            try
            {
                var objSuscripcion = _dbContext.Suscripciones.Where(c => c.Email == txtEmail).FirstOrDefault();
                if (objSuscripcion != null)
                {
                    objSuscripcion.Activo = true;
                    _dbContext.Update(objSuscripcion);
                }
                else
                {
                    _dbContext.Add(new Suscripciones
                    {
                        Activo = true,
                        Email = txtEmail
                    });
                }
                await _dbContext.SaveChangesAsync();
                return new JsonResult(new Response { IsSuccess = true, Message = "Se suscribió al boletín correctamente." });
            }
            catch (Exception ex)
            {
                await _emailSender.SendEmailAsync("j.98.alejandro@gmail.com", "Error La Pesca en Línea ", ex.Message);
                return new JsonResult(new Response { IsSuccess = false, Message = "No se pudo suscribir por el momento, intentelo más tarde." });
            }

        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
