using Progra5Jaz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Progra5Jaz.Controllers
{
    public class HomeController : Controller
    {
        Datos datos=new Datos();
        // GET: Home
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


        public ActionResult Menu()
        {
            return View();
        }



        public ActionResult Registro()
        {
            if (Request.Form.AllKeys.Contains("Enviar")) { 
                string Ide = Request.Form["Identificacion"];
                string Nombre = Request.Form["Nombre"];
                string Correo = Request.Form["Correo"];
                string Telefono = Request.Form["Telefono"];
                string Contrasena = Request.Form["Contrasena"];

                datos.Registrar( Ide,  Nombre,  Correo,  Telefono,  Contrasena);
            }
            return View();
        }

    }

}