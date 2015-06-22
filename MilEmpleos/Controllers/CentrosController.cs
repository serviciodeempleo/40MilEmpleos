using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MilEmpleos.Models;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MilEmpleos.Controllers
{
    public class CentrosController : Controller
    {
        private MilEmpleosEntities1 db = new MilEmpleosEntities1();

        // GET: Centros
        public ActionResult Index()
        {
            var centros = db.Centros.Include(c => c.AspNetUsers);
            return View(centros.ToList());
        }

        // GET: Centros/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Centros centros = db.Centros.Find(id);
            if (centros == null)
            {
                return HttpNotFound();
            }
            return View(centros);
        }

        // GET: Centros/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Address");
            return View();
        }

        // POST: Centros/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CentroEmpleo,UserID")] Centros centros)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                var existeCentro = (from item in db.Centros
                                     where item.CentroEmpleo == centros.CentroEmpleo
                                     select item).ToList();
                if (existeCentro.Count() == 0)
                {
                    centros.UserID = User.Identity.GetUserId();
                    db.Centros.Add(centros);
                    db.SaveChanges();

                    var CentroID = (from item in db.Centros
                                     where item.CentroEmpleo == centros.CentroEmpleo
                                     select item.Id).First();
                    AspNetUsers UsurioCentro = db.AspNetUsers.Find(User.Identity.GetUserId());
                    UsurioCentro.CentroId = CentroID;
                    db.Entry(UsurioCentro).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");                
                }else{                {
                    message = "El Centro de Empleo Ya Existe!!";
                }
            }
            ViewBag.message = message;

            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Address", centros.UserID);
            return View(centros);
        }

        // GET: Centros/Edit/5
        public ActionResult Edit(int? id)
        {
            var USER_id = User.Identity.GetUserId();
            var CE = (from item in db.AspNetUsers
                      where item.Id == USER_id
                      select item.CentroId).First();
            Centros centros = db.Centros.Find(CE);
            if (centros == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Address", centros.UserID);
            return View(centros);
        }

        // POST: Centros/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CentroEmpleo,UserID")] Centros centros)
        {
            if (ModelState.IsValid)
            {
                db.Entry(centros).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Address", centros.UserID);
            return View(centros);
        }

        // GET: Centros/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Centros centros = db.Centros.Find(id);
            if (centros == null)
            {
                return HttpNotFound();
            }
            return View(centros);
        }

        // POST: Centros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Centros centros = db.Centros.Find(id);
            db.Centros.Remove(centros);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
