﻿using Progra5Jaz.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Progra5Jaz.Controllers
{
    public class HomeController : Controller
    {
        Datos datos=new Datos();


        // GET: Login
        public ActionResult Index()
        {
            if (Request.Form.AllKeys.Contains("Ingresar"))
            {
                string Correo = Request.Form["Correo"];
                string Contrasena = Request.Form["Contrasena"];

                if (datos.login(Correo, Contrasena))
                {
                    return RedirectToAction("Menu", "Home");
                }
                else
                {
                    ViewBag.Message = "Credenciales Incorectas";
                    ViewBag.icono = "error";
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




        // Vista SPA
        public ActionResult SpaCli()
        {
            DataTable tabla = datos.Spa();
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
            ViewBag.Spa = tabla;
            return View();
        }


        //Vista ManiPedi
        public ActionResult ManPedCli()
        {
            DataTable tabla = datos.ManPed();
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
            ViewBag.ManPed = tabla;
            return View();
        }


        // Vista Estetica
        public ActionResult EsteticaCli()
        {
            DataTable tabla = datos.Estetica();
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
            ViewBag.Estetica = tabla;
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
        

        //Registro
        public ActionResult Registro()
        {
            if (Request.Form.AllKeys.Contains("Registrar")) {
                string Ide = Request.Form["Identificacion"];
                string Nombre = Request.Form["Nombre"];
                string Correo = Request.Form["Correo"];
                string Telefono = Request.Form["Telefono"];
                string Contrasena = Request.Form["Contrasena"];
                string FechaNa = Request.Form["FechaNa"];

                string Mensaje = datos.Registrar(Ide, Nombre, Correo, Telefono, Contrasena, FechaNa);
                if (Mensaje == "El usuario registrado")
                {
                    ViewBag.icono = "success";
                }
                else {
                    ViewBag.icono = "error";
                }
                ViewBag.Message = Mensaje;
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