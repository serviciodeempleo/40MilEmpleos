using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MilEmpleos.Models;

namespace MilEmpleos.Controllers
{
    public class ResultadoCalculadorasController : Controller
    {
        private MilEmpleosEntities1 db = new MilEmpleosEntities1();

        // GET: ResultadoCalculadoras
        public ActionResult Index()
        {
            var resultadoCalculadora = db.ResultadoCalculadora.Include(r => r.AspNetUsers).Include(r => r.Centros).Include(r => r.RespuestasCalculadora);
            return View(resultadoCalculadora.ToList());
        }

        // GET: ResultadoCalculadoras/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResultadoCalculadora resultadoCalculadora = db.ResultadoCalculadora.Find(id);
            if (resultadoCalculadora == null)
            {
                return HttpNotFound();
            }
            return View(resultadoCalculadora);
        }

        // GET: ResultadoCalculadoras/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Address");
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "CentroEmpleo");
            ViewBag.RespuestasCalculadoraId = new SelectList(db.RespuestasCalculadora, "id", "RazonSocial");
            return View();
        }

        // POST: ResultadoCalculadoras/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,CapitalHumano,CaracteristicasEmpresa,CaracteristicasPuesto,Total,UserId,CentroId,RespuestasCalculadoraId")] ResultadoCalculadora resultadoCalculadora)
        {
            if (ModelState.IsValid)
            {
                resultadoCalculadora.id = Guid.NewGuid();
                db.ResultadoCalculadora.Add(resultadoCalculadora);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Address", resultadoCalculadora.UserId);
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "CentroEmpleo", resultadoCalculadora.CentroId);
            ViewBag.RespuestasCalculadoraId = new SelectList(db.RespuestasCalculadora, "id", "RazonSocial", resultadoCalculadora.RespuestasCalculadoraId);
            return View(resultadoCalculadora);
        }

        // GET: ResultadoCalculadoras/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResultadoCalculadora resultadoCalculadora = db.ResultadoCalculadora.Find(id);
            if (resultadoCalculadora == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Address", resultadoCalculadora.UserId);
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "CentroEmpleo", resultadoCalculadora.CentroId);
            ViewBag.RespuestasCalculadoraId = new SelectList(db.RespuestasCalculadora, "id", "RazonSocial", resultadoCalculadora.RespuestasCalculadoraId);
            return View(resultadoCalculadora);
        }

        // POST: ResultadoCalculadoras/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,CapitalHumano,CaracteristicasEmpresa,CaracteristicasPuesto,Total,UserId,CentroId,RespuestasCalculadoraId")] ResultadoCalculadora resultadoCalculadora)
        {
            if (ModelState.IsValid)
            {
                db.Entry(resultadoCalculadora).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Address", resultadoCalculadora.UserId);
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "CentroEmpleo", resultadoCalculadora.CentroId);
            ViewBag.RespuestasCalculadoraId = new SelectList(db.RespuestasCalculadora, "id", "RazonSocial", resultadoCalculadora.RespuestasCalculadoraId);
            return View(resultadoCalculadora);
        }

        // GET: ResultadoCalculadoras/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResultadoCalculadora resultadoCalculadora = db.ResultadoCalculadora.Find(id);
            if (resultadoCalculadora == null)
            {
                return HttpNotFound();
            }
            return View(resultadoCalculadora);
        }

        // POST: ResultadoCalculadoras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ResultadoCalculadora resultadoCalculadora = db.ResultadoCalculadora.Find(id);
            
            db.ResultadoCalculadora.Remove(resultadoCalculadora);
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
