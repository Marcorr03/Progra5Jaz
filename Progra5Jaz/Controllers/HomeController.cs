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

        //Menu
        public ActionResult Menu()
        {

            return View();
        }

        //Promos
        public ActionResult PromosCli()
        {

            return View();
        }

        public ActionResult PromosAdm()
        {

            return View();
        }

        //Actividades
        public ActionResult ActividadesCli()
        {

            return View();
        }

        public ActionResult ActividadesAdm()
        {

            return View();
        }

        //SPA
        public ActionResult SpaCli()
        {

            return View();
        }

        public ActionResult SpaAdm()
        {

            return View();
        }



        //ManiPedi
        public ActionResult ManPedCli()
        {

            return View();
        }

        public ActionResult ManPedAdm()
        {

            return View();
        }

        //Estetica
        public ActionResult EsteticaCli()
        {

            return View();
        }

        public ActionResult EsteticaAdm()
        {

            return View();
        }


        //Registro
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


        public ActionResult RegistroAdm()
        {
            if (Request.Form.AllKeys.Contains("Enviar"))
            {
                //string Ide = Request.Form["Identificacion"];
                //string Nombre = Request.Form["Nombre"];
                //string Correo = Request.Form["Correo"];
                //string Telefono = Request.Form["Telefono"];
                //string Contrasena = Request.Form["Contrasena"];

                //datos.Registrar(Ide, Nombre, Correo, Telefono, Contrasena);
            }
            return View();
        }

        public ActionResult RegistroSup()
        {
            if (Request.Form.AllKeys.Contains("Enviar"))
            {
                //string Ide = Request.Form["Identificacion"];
                //string Nombre = Request.Form["Nombre"];
               //string Correo = Request.Form["Correo"];
                //string Telefono = Request.Form["Telefono"];
                //string Contrasena = Request.Form["Contrasena"];

                //datos.Registrar(Ide, Nombre, Correo, Telefono, Contrasena);
            }
            return View();
        }

    }

}