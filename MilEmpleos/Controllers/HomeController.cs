using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MilEmpleos.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentitySample.Controllers
{
     [Authorize(Roles = "Admin, Centros, Consultores")]
    public class HomeController : Controller
    {
         public MilEmpleosEntities1 db = new MilEmpleosEntities1();
        public ActionResult Index()
        {
            var USER_id = User.Identity.GetUserId();
            var ChangePassword = (from cp in db.AspNetUsers where cp.Id == USER_id select cp.PasswordChange).First();
            if (ChangePassword==true)
            {
                return RedirectToAction("ChangePassword", "Manage");
            }
            ViewBag.fecha_Actualizacion = (from f in db.actulizacionPila select f).OrderByDescending(f=>f.Fecha_Actualizacion).First().Fecha_Actualizacion.ToString(@"yyyy/MM/dd");
            ViewBag.TipoDocumento = new SelectList(db.TipoDocumento, "id", "TipoDocumento1");
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
