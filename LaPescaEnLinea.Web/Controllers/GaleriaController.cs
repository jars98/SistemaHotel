using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LaPescaEnLinea.Models;
using LaPescaEnLinea.Tools;
using LaPescaEnLinea.Tools.Services;
using LaPescaEnLinea.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LaPescaEnLinea.Web.Controllers
{
    public class GaleriaController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly DataContextLPL _dbContext;
        public GaleriaController(IEmailSender emailSender, DataContextLPL dbContext)
        {
            _emailSender = emailSender;
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.PaginaActual = "Galería";
            if (!HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
            ViewBag.Color = usuario.ColorTema == "w" ? Url.Content("~/admin/white") : Url.Content("~/admin/black");
            List<Pagina> p = new List<Pagina>();
            p.Add(new Pagina { Actual = true, Nombre = "Galería", Url = Url.Content("~/Galeria") });
            ViewBag.Paginas = p;
            return View();
        }

        public async Task<IActionResult> Imagenes(int page, string strSearch, string opcion = "")
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
            ViewBag.Color = usuario.ColorTema == "w" ? Url.Content("~/admin/white") : Url.Content("~/admin/black");
            int skyp = page * 9;
            List<Imagenes> img = new List<Imagenes>();
            if (!string.IsNullOrEmpty(strSearch))
            {
                img = _dbContext.Imagenes.Where(c => c.Activo == true && c.UsuarioId == usuario.Id && c.Nombre.Contains(strSearch)).OrderByDescending(c => c.Id).Skip(skyp).Take(9).ToList();
            }
            else
            {
                img = _dbContext.Imagenes.Where(c => c.Activo == true && c.UsuarioId == usuario.Id).OrderByDescending(c => c.Id).Skip(skyp).Take(9).ToList();
            }
            ViewBag.Opcion = opcion;
            return PartialView(img);
        }

        public async Task<IActionResult> UploadImagen(string Nombre, int? Id, IFormFile file)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");
                if (Id != null && file == null)
                    return new JsonResult(new Response
                    {
                        IsSuccess = false,
                        Message = "Debe seleccionar la foto que quiere subir al sistema."
                    });
                string strFile = "";
                bool saveImage = false;
                var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
                if (file != null && file.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesUpload");
                    strFile = await ImagenesHelper.UploadAsync(path, file, usuario.Id, _emailSender);
                    if (string.IsNullOrEmpty(strFile))
                        return new JsonResult(new Response
                        {
                            IsSuccess = false,
                            Message = "No se pudo subir la foto al sistema, intentelo más tarde."
                        });
                    var pathSource = Path.Combine(path, strFile);
                    path = Path.Combine(path, "mini", strFile);
                    saveImage = await ImagenesHelper.ResizeSaveImage(pathSource, path, Convert.ToUInt32(600), Convert.ToUInt32(300), _emailSender);
                    if (saveImage == false)
                        return new JsonResult(new Response
                        {
                            IsSuccess = false,
                            Message = "No se pudo subir la foto al sistema, intentelo más tarde."
                        });
                }

                if ((file != null && saveImage == true) || (file == null && saveImage == false))
                {
                    if (Id == null)
                    {
                        var img = new Imagenes
                        {
                            Activo = true,
                            BlogId = null,
                            HabitacionId = null,
                            HotelId = null,
                            Imagen = strFile,
                            Nombre = Nombre,
                            ServicioId = null,
                            UsuarioId = usuario.Id
                        };
                        _dbContext.Add(img);
                    }
                    else
                    {
                        var img = _dbContext.Imagenes.Where(c => c.Id == Id && c.UsuarioId == usuario.Id).FirstOrDefault();
                        if (img != null)
                        {
                            if (!string.IsNullOrEmpty(strFile))
                                img.Imagen = strFile;
                            img.Nombre = Nombre;
                            _dbContext.Update(img);
                        }
                        else
                        {
                            return new JsonResult(new Response
                            {
                                IsSuccess = false,
                                Message = "No se encontro la foto que intenta actualizar."
                            });
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                    return new JsonResult(new Response
                    {
                        IsSuccess = true,
                        Message = "Se guardo la foto correctamente.",
                        DivTabla = "#demo-test-gallery",
                        Url = Url.Content("~/Galeria/Imagenes?page=0")
                    });
                }
                else
                {
                    return new JsonResult(new Response
                    {
                        IsSuccess = false,
                        Message = "No se pudo subir la foto al sistema, intentelo más tarde."
                    });
                }
            }
            catch (Exception ex)
            {
                await _emailSender.SendEmailAsync("jiestrada@live.com.mx", "Error La Pesca en Línea ", ex.ToString());
                return new JsonResult(new Response
                {
                    IsSuccess = false,
                    Message = "No se pudo subir la foto al sistema, intentelo más tarde."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AsignarImagenes(int HotelId, int ImagenId, int? HabitacionId)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");
                var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
                var hotel = _dbContext.Hoteles.Where(c => c.Id == HotelId && c.UsuarioId == usuario.Id).FirstOrDefault();
                if (hotel == null)
                {
                    return new JsonResult(new Response
                    {
                        IsSuccess = false,
                        Message = "No se encontro el hotel."
                    });
                }
                var img = _dbContext.ImagenesAsociadas.Where(c => c.ImagenId == ImagenId && c.HotelId == HotelId && c.HabitacionId== HabitacionId && c.Activo==true).FirstOrDefault();
                if (img != null)
                {
                    return new JsonResult(new Response
                    {
                        IsSuccess = false,
                        Message = "La imagen ya se encuentra asociada."
                    });
                }
                var nuevaImagen = new ImagenesAsociadas
                {
                    Activo = true,
                    HotelId = HotelId,
                    HabitacionId= HabitacionId,
                    ImagenId = ImagenId
                };

                _dbContext.Add(nuevaImagen);
                await _dbContext.SaveChangesAsync();
                if (nuevaImagen.Id > 0)
                {
                    if (HabitacionId != null)
                    {
                        return new JsonResult(new Response
                        {
                            IsSuccess = true,
                            Message = "Se agrego correctamente la imagen.",
                            DivTabla = "#fotosmihotel",
                            Url = Url.Content("~/Habitaciones/ImagenesHabitacion?id=" + HabitacionId)
                        });
                    }
                    return new JsonResult(new Response
                    {
                        IsSuccess = true,
                        Message = "Se agrego correctamente la imagen.",
                        DivTabla= "#fotosmihotel", 
                        Url=Url.Content("~/Hoteles/ImagenesHotel?id=" + HotelId) + "&opcion=seleccionar&h="
                    });
                }

            }
            catch (Exception ex)
            {
                await _emailSender.SendEmailAsync("jiestrada@live.com.mx", "Error La Pesca en Línea ", ex.ToString());
                return new JsonResult(new Response
                {
                    IsSuccess = false,
                    Message = "No se pudo asociar la foto al sistema, intentelo más tarde."
                });
            }
            return new JsonResult(new Response
            {
                IsSuccess = false,
                Message = "No se pudo asociar la foto al sistema, intentelo más tarde."
            });
        }

        [HttpPost]
        public async Task<IActionResult> EliminarRelacion(int HotelId, int ImagenId, int? HabitacionId)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");
                var usuario = await AutenticacionHelper.GetUsuario(HttpContext, _emailSender);
                var hotel = _dbContext.Hoteles.Where(c => c.Id == HotelId && c.UsuarioId == usuario.Id).FirstOrDefault();
                if (hotel == null)
                {
                    return new JsonResult(new Response
                    {
                        IsSuccess = false,
                        Message = "No se encontro el hotel."
                    });
                }
                var img = _dbContext.ImagenesAsociadas.Where(c => c.Id == ImagenId && c.HotelId == HotelId && c.HabitacionId==HabitacionId && c.Activo == true).FirstOrDefault();
                if (img != null)
                {
                    img.Activo = false;
                    _dbContext.Update(img);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    return new JsonResult(new Response
                    {
                        IsSuccess = false,
                        Message = "No se encontro la imagen que se queria eliminar."
                    });
                }
                if (HabitacionId != null)
                {
                    return new JsonResult(new Response
                    {
                        IsSuccess = true,
                        Message = "Se eliminó correctamente la imagen.",
                        DivTabla = "#fotosmihotel",
                        Url = Url.Content("~/Habitaciones/ImagenesHabitacion?id=" + HabitacionId)
                    });
                }
                return new JsonResult(new Response
                {
                    IsSuccess = true,
                    Message = "Se eliminó correctamente la imagen.",
                    DivTabla = "#fotosmihotel",
                    Url = Url.Content("~/Hoteles/ImagenesHotel?id=" + HotelId) 
                });

            }
            catch (Exception ex)
            {
                await _emailSender.SendEmailAsync("jiestrada@live.com.mx", "Error La Pesca en Línea ", ex.ToString());
                return new JsonResult(new Response
                {
                    IsSuccess = false,
                    Message = "No se pudo eliminar la foto del hotel, intentelo más tarde."
                });
            }
        }
    }
}