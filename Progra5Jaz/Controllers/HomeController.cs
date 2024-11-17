using Progra5Jaz.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Progra5Jaz.Controllers
{
    public class HomeController : Controller
    {
        Datos datos=new Datos();


        [HttpPost]
        public ContentResult AjaxLogin(string correo, string contrasena)
        {
            if (datos.login(correo, contrasena))
            {
                datos.CorreoCodigo(correo);
                Session["Email"] = correo;
                return Content("{\"success\": true, \"message\": \"\"}", "application/json");
            }
            else
            {
                return Content("{\"success\": false, \"message\": \"Credenciales Incorrectas\"}", "application/json");
            }
        }

        public string LeerJson()
        {
            string filePath = HostingEnvironment.MapPath("~/App_Data/locations.json");

            // Leer el contenido del archivo
            string jsonData = System.IO.File.ReadAllText(filePath);

            // Devolver el JSON como resultado
            return jsonData;
        }


        // GET: Login
        public ActionResult Index()
        {
            datos.EscribirJson();
            return View();
       }




        //Doble factor 
        public ActionResult DobleFact()
        {
            ViewBag.Message = "Se envio un código a su correo";
            if (Request.Form.AllKeys.Contains("1"))
            {
                string codigo = Request.Form["1"] + Request.Form["2"] + Request.Form["3"] + Request.Form["4"] + Request.Form["5"];
                datos.RevisarCod(Session["Email"].ToString(), codigo);
                if (datos.Mensaje1 == "Correcto")
                {
                    return RedirectToAction("Menu", "Home");
                }
                else
                {
                    ViewBag.Message = datos.Mensaje1;
                }
            }
            return View();
        }


        //CambioContra 
        public ActionResult CambioContra()
        {
            ViewBag.Message = Session["Temporal"];
            Session["Temporal"] = "";
            if (Request.Form.AllKeys.Contains("Contrasena"))
            {
                string contra = Request.Form["Contrasena"];
                string concontra = Request.Form["ConContrasena"];
                if (contra == concontra)
                {
                    string pattern = @"^(?=.*[A-Z].*[A-Z].*[A-Z].*[A-Z])(?=.*[a-z].*[a-z].*[a-z].*[a-z])(?=.*[0-9].*[0-9].*[0-9].*[0-9])(?=.*[!#$%^&*()\-_=+\[\]{}|;:,.<>?].*[!#$%^&*()\-_=+\[\]{}|;:,.<>?]).{14,20}$";

                    if (System.Text.RegularExpressions.Regex.IsMatch(Request.Form["Contrasena"], pattern))
                    {
                        datos.NewContra(Session["Usuario"].ToString(), Request.Form["temporal"], Request.Form["Palabra"], Request.Form["Contrasena"]);
                        ViewBag.Message = datos.Mensaje1;
                        if (datos.Mensaje1 == "Tu solicitud se proceso correctamente")
                        {
                            Session["Recupero"] = ViewBag.Message;
                            Session["Email"] = Session["Usuario"].ToString();
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ViewBag.Message = "La contraseña no es valida, cumpla con lo requerido";
                    }
                }
                else
                {
                    ViewBag.Message = "Las contraseñas no coinsiden";
                }
            }
            return View();
        }

        //CambioContra 
        public ActionResult OlvideContra()
        {
            if (Request.Form.AllKeys.Contains("Correo"))
            {
                datos.CambioContra(Request.Form["Correo"]);
                ViewBag.Message = datos.Mensaje1;
                if (datos.Mensaje1 == "Se ha enviado una contraseña temporal a su Email")
                {
                    Session["Temporal"] = "Se ha enviado una clave temporal a su Email";
                    Session["Usuario"] = Request.Form["Correo"];
                    return RedirectToAction("CambioContra", "Home");
                }
            }
                return View();
        }

        //Menu
        public ActionResult Menu()
        {
            DataTable tabla = datos.Promos();
            if (!tabla.Columns.Contains("ImagenDataUrl"))
            {
                // Si no existe, agregar la nueva columna
                tabla.Columns.Add("ImagenDataUrl", typeof(string));
            }
            foreach (DataRow row in tabla.Rows)
            {
                byte[] imagenBytes = (byte[])row["Imagen"];
                string imagenBase64 = Convert.ToBase64String(imagenBytes);
                string imagenDataUrl = $"data:image/jpeg;base64,{imagenBase64}";
                row["ImagenDataUrl"] = imagenDataUrl;
            }
            ViewBag.Promos = tabla;
            return View();
        }

        //Promos Vista
        public ActionResult PromosCli()
        {
            DataTable tabla = datos.Promos();
            if (!tabla.Columns.Contains("ImagenDataUrl"))
            {
                // Si no existe, agregar la nueva columna
                tabla.Columns.Add("ImagenDataUrl", typeof(string));
            }
            foreach (DataRow row in tabla.Rows)
            {
                byte[] imagenBytes = (byte[])row["Imagen"];
                string imagenBase64 = Convert.ToBase64String(imagenBytes);
                string imagenDataUrl = $"data:image/jpeg;base64,{imagenBase64}";
                row["ImagenDataUrl"] = imagenDataUrl;
            }
            ViewBag.Promos = tabla;
            return View();
        }


        //Gestion Promos
        public ActionResult PromosAdm()
        {
            if (Request.Form.AllKeys.Contains("Registrar"))
            {
                string Nombre = Request.Form["Nombre"];
                string Descripcion = Request.Form["Descripcion"];
                string Precio = Request.Form["Precio"];
                HttpPostedFileBase image = Request.Files["picture"];

                string Mensaje = datos.RegistrarPromo(Nombre, Descripcion, Precio, image);
                if (Mensaje == "Promoción registrada")
                {
                    ViewBag.icono = "success";
                }
                else
                {
                    ViewBag.icono = "error";
                }
                ViewBag.Message = Mensaje;
            }
            return View();
        }

        //Actividades Vista
        public ActionResult ActividadesCli()
        {
            DataTable tabla = datos.Actividades();
            if (!tabla.Columns.Contains("ImagenDataUrl"))
            {
                // Si no existe, agregar la nueva columna
                tabla.Columns.Add("ImagenDataUrl", typeof(string));
            }
            foreach (DataRow row in tabla.Rows)
            {
                byte[] imagenBytes = (byte[])row["Imagen"];
                string imagenBase64 = Convert.ToBase64String(imagenBytes);
                string imagenDataUrl = $"data:image/jpeg;base64,{imagenBase64}";
                row["ImagenDataUrl"] = imagenDataUrl;
            }
            ViewBag.Actividades=tabla;
            return View();
        }


        //Gestion Actividades
        public ActionResult ActividadesAdm()
        {
            if (Request.Form.AllKeys.Contains("Registrar"))
            {
                string Nombre = Request.Form["Nombre"];
                string Descripcion = Request.Form["Descripcion"];
                string MinPer = Request.Form["MinPer"];
                string Precio = Request.Form["Precio"];
                HttpPostedFileBase image = Request.Files["picture"];
                datos.RegistrarActi(Nombre, Descripcion,MinPer,Precio, image);

                string Mensaje = datos.RegistrarPromo(Nombre, Descripcion, Precio, image);
                if (Mensaje == "Promo registrada")
                {
                    ViewBag.icono = "success";
                }
                else
                {
                    ViewBag.icono = "error";
                }
                ViewBag.Message = Mensaje;

        }
            
            return View();
        }


        //Gestion Servicios(spa,manipedi,estetica)
        public ActionResult SpaAdm()
        {
            if (Request.Form.AllKeys.Contains("Registrar"))
            {
                string Nombre = Request.Form["Nombre"];
                string Descripcion = Request.Form["Descripcion"];
                string Precio = Request.Form["Precio"];
                string IdCategoriaSer = Request.Form["Categoria"];
                HttpPostedFileBase image = Request.Files["picture"];

                string Mensaje = datos.RegistrarServ( Nombre, Descripcion, Precio, IdCategoriaSer, image);
                if (Mensaje == "Servicio registrado")
                {
                    ViewBag.icono = "success";
                }
                else
                {
                    ViewBag.icono = "error";
                }
                ViewBag.Message = Mensaje;
            }
            return View();
        }


        //MOSTRAR SERVICIOS SPA
        public ActionResult SpaCli()
        {
            DataTable tabla = datos.Spa();
            if (tabla.ToString().Any())
            {
                if (!tabla.Columns.Contains("ImagenDataUrl"))
                {
                    // Si no existe, agregar la nueva columna
                    tabla.Columns.Add("ImagenDataUrl", typeof(string));
                }
                foreach (DataRow row in tabla.Rows)
                {
                    string imagenDataUrl = $"data:image/jpeg;base64,{row["Imagen"]}";
                    row["ImagenDataUrl"] = imagenDataUrl;
                }
                ViewBag.Spa = tabla;
            }
            else { ViewBag.Spa = null; }
            return View();
        }




        //Vista ManiPedi
        public ActionResult ManPedCli()
        {
            DataTable tabla = datos.ManPed();
            if (tabla.ToString().Any())
            {
                if (!tabla.Columns.Contains("ImagenDataUrl"))
                {
                    // Si no existe, agregar la nueva columna
                    tabla.Columns.Add("ImagenDataUrl", typeof(string));
                }
                foreach (DataRow row in tabla.Rows)
                {
                    string imagenDataUrl = $"data:image/jpeg;base64,{row["Imagen"]}";
                    row["ImagenDataUrl"] = imagenDataUrl;
                }
                ViewBag.ManPed = tabla;
            }
            else { ViewBag.ManPed = null; }

            return View();
        }


        // Vista Estetica
        public ActionResult EsteticaCli()
        {
            DataTable tabla = datos.Estetica();
            if (tabla.ToString().Any())
            {
                if (!tabla.Columns.Contains("ImagenDataUrl"))
                {
                    // Si no existe, agregar la nueva columna
                    tabla.Columns.Add("ImagenDataUrl", typeof(string));
                }
                foreach (DataRow row in tabla.Rows)
                {
                    string imagenDataUrl = $"data:image/jpeg;base64,{row["Imagen"]}";
                    row["ImagenDataUrl"] = imagenDataUrl;
                }
                ViewBag.Estetica = tabla;
            }
            else {ViewBag.Estetica = null; }
            
            return View();
        }

        //Contactenos
        public ActionResult Contactenos()
        {
            if (Request.Form.AllKeys.Contains("Registrar"))
            {
                string Correo = Request.Form["Correo"];
                string Men = Request.Form["Descripcion"];

                string Mensaje = datos.EnvioMensaje(Correo, Men);
                if (Mensaje == "Mensaje enviado")
                {
                    ViewBag.icono = "success";
                }
                else
                {
                    ViewBag.icono = "error";
                }
                ViewBag.Message = Mensaje;
            }
            return View();
        }
        

        //Registro Clientes
        public ActionResult Registro()
        {

            if (Request.Form.AllKeys.Contains("Registrar"))
            {

                if (Request.Form["contrasena"] == Request.Form["concontrasena"])
                {
                    string pattern = @"^(?=.*[A-Z].*[A-Z].*[A-Z].*[A-Z])(?=.*[a-z].*[a-z].*[a-z].*[a-z])(?=.*[0-9].*[0-9].*[0-9].*[0-9])(?=.*[!#$%^&*()\-_=+\[\]{}|;:,.<>?].*[!#$%^&*()\-_=+\[\]{}|;:,.<>?]).{14,20}$";

                    if (System.Text.RegularExpressions.Regex.IsMatch(Request.Form["contrasena"], pattern)){
                        string Ide = Request.Form["Identificacion"];
                        string Nombre = Request.Form["Nombre"];
                        string Correo = Request.Form["Correo"];
                        string Telefono = Request.Form["Telefono"];
                        string Contrasena = Request.Form["Contrasena"];
                        string FechaNa = Request.Form["FechaNa"];
                        string Vigencia = Request.Form["value-radio"].Split('-')[1];
                        string PalabraClave = Request.Form["PalabraClave"];



                        string Mensaje = datos.Registrar(Ide, Nombre, Correo, Telefono, Contrasena, FechaNa, Vigencia, PalabraClave);
                        if (Mensaje == "Tu solicitud se proceso correctamente")

                        {
                            ViewBag.icono = "success";
                        }
                        else {
                            ViewBag.icono = "error";
                        }
                        ViewBag.Message = Mensaje;
                    }
                }

            }
            return View();
        }

        //Agendar
        public ActionResult ReservasPromos()
        {

            return View();
        }

        //Agendar
        public ActionResult ReservasActividades()
        {

            return View();
        }

        //Agendar
        public ActionResult ReservasServicios()
        {

            return View();
        }

        //Nosotros
        public ActionResult Nosotros()
        {

            return View();
        }

    }

}