using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MilEmpleos.Models;
using Newtonsoft.Json;
using System.Web.Script.Serialization; 

namespace MilEmpleos.Controllers
{
    [Authorize(Roles = "Admin, Centros, Consultores, Unidad, Soporte")]
    public class ResultadoCalculadorasController : Controller
    {
        private MilEmpleosEntities db = new MilEmpleosEntities();

        // GET: ResultadoCalculadoras
        public ActionResult Index()
        {
            var resultadoCalculadora = db.ResultadoCalculadora.Take(20).Include(r => r.AspNetUsers).Include(r => r.Centros).Include(r => r.RespuestasCalculadora);
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
        [Authorize(Roles = "Admin, Soporte")]
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
                return RedirectToAction("Nofoud/" + id);
            }
            return View(resultadoCalculadora);
        }

        // POST: ResultadoCalculadoras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            LogsAudit logsAudit = new LogsAudit();
            logsAudit.Id = Guid.NewGuid();
            logsAudit.InstanciaServidor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            logsAudit.UserName = User.Identity.Name;
            logsAudit.FechaHora = DateTime.Now;
            logsAudit.IpRequest = HttpContext.Request.UserHostAddress;
            logsAudit.DataRequest = HttpContext.Request.AppRelativeCurrentExecutionFilePath + " | " + HttpContext.Request.AnonymousID + " | " + HttpContext.Request.CurrentExecutionFilePathExtension + " | " + HttpContext.Request.UrlReferrer;
            logsAudit.Agent = HttpContext.Request.UserAgent;
            logsAudit.JsonRequest = "|Código calificacíon = |" + id;
            logsAudit.Operacion = "Eliminación de calificacion";
            ResultadoCalculadora resultadoCalculadora = db.ResultadoCalculadora.Find(id);
            RespuestasCalculadora respuestasCalculadora = db.RespuestasCalculadora.Find(resultadoCalculadora.RespuestasCalculadoraId);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            ResultadoCalculadoraser resultadoCalculadoraSer = new ResultadoCalculadoraser();
            RespuestasCalculadoraaser respuestasCalculadoraSer = new RespuestasCalculadoraaser();
            resultadoCalculadoraSer.id = resultadoCalculadora.id;
            resultadoCalculadoraSer.CapitalHumano = resultadoCalculadora.CapitalHumano;
            resultadoCalculadoraSer.CaracteristicasEmpresa = resultadoCalculadora.CaracteristicasEmpresa;
            resultadoCalculadoraSer.CaracteristicasPuesto = resultadoCalculadora.CaracteristicasPuesto;
            resultadoCalculadoraSer.Total = resultadoCalculadora.Total;
            resultadoCalculadoraSer.UserId = resultadoCalculadora.UserId;
            resultadoCalculadoraSer.CentroId = resultadoCalculadora.CentroId;
            resultadoCalculadoraSer.RespuestasCalculadoraId = resultadoCalculadora.RespuestasCalculadoraId;
            resultadoCalculadoraSer.FechaCalculo = resultadoCalculadora.FechaCalculo;

            respuestasCalculadoraSer.id = respuestasCalculadora.id;
            respuestasCalculadoraSer.RazonSocial = respuestasCalculadora.RazonSocial;
            respuestasCalculadoraSer.Nit = respuestasCalculadora.Nit;
            respuestasCalculadoraSer.TelefonoEmpresa = respuestasCalculadora.TelefonoEmpresa;
            respuestasCalculadoraSer.NombreContacto = respuestasCalculadora.NombreContacto;
            respuestasCalculadoraSer.DescripcionVacante = respuestasCalculadora.DescripcionVacante;
            respuestasCalculadoraSer.NivelEducativoRequeridoVacante = respuestasCalculadora.NivelEducativoRequeridoVacante;
            respuestasCalculadoraSer.NumeroPuestosTrabajo = respuestasCalculadora.NumeroPuestosTrabajo;
            respuestasCalculadoraSer.OficinaRecursosHumanos = respuestasCalculadora.OficinaRecursosHumanos;
            respuestasCalculadoraSer.TamanoPlantaEmpresa = respuestasCalculadora.TamanoPlantaEmpresa;
            respuestasCalculadoraSer.HabilidadRequerida = respuestasCalculadora.HabilidadRequerida;
            respuestasCalculadoraSer.DestrezasRequeridas = respuestasCalculadora.DestrezasRequeridas;
            respuestasCalculadoraSer.TipoConocimientoRequerido = respuestasCalculadora.TipoConocimientoRequerido;
            respuestasCalculadoraSer.ActividadesPuestoDeTrabajo = respuestasCalculadora.ActividadesPuestoDeTrabajo;
            respuestasCalculadoraSer.JovenesPorTutor = respuestasCalculadora.JovenesPorTutor;
            respuestasCalculadoraSer.VacanteNueva = respuestasCalculadora.VacanteNueva;
            respuestasCalculadoraSer.PuestoTrabajoPermanente = respuestasCalculadora.PuestoTrabajoPermanente;
            respuestasCalculadoraSer.ContenidoActividadFormacion = respuestasCalculadora.ContenidoActividadFormacion;
            respuestasCalculadoraSer.SalarioAdicional = respuestasCalculadora.SalarioAdicional;
            respuestasCalculadoraSer.SalarioAdicionalRango = respuestasCalculadora.SalarioAdicionalRango;
            respuestasCalculadoraSer.Municipio = respuestasCalculadora.Municipio;
            respuestasCalculadoraSer.UserId = respuestasCalculadora.UserId;
            respuestasCalculadoraSer.CentroId = respuestasCalculadora.CentroId;
            respuestasCalculadoraSer.FechaRegistro = respuestasCalculadora.FechaRegistro;

            string str = ser.Serialize(respuestasCalculadoraSer) + " | " + ser.Serialize(resultadoCalculadoraSer);
            logsAudit.JsonResponse = "La calificación a sido eliminada con exito|" + str;
            db.RespuestasCalculadora.Remove(respuestasCalculadora);
            db.ResultadoCalculadora.Remove(resultadoCalculadora);
            db.LogsAudit.Add(logsAudit);
            db.SaveChanges();
            return RedirectToAction("ExitoEliminacion/" + id);
        }
        // GET: Exito Eliminacion
        public ActionResult ExitoEliminacion(Guid? id)
        {
            ViewBag.codigo = id;
            return View();
        }

        // GET: Código de  vacante  no encontrado
        public ActionResult Nofoud(Guid? id)
        {
            ViewBag.codigo = id;
            return View();
        }
        public ActionResult BuscarCalificacion()
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public partial class ResultadoCalculadoraser
        {
            public System.Guid id { get; set; }
            public double CapitalHumano { get; set; }
            public double CaracteristicasEmpresa { get; set; }
            public double CaracteristicasPuesto { get; set; }
            public double Total { get; set; }
            public string UserId { get; set; }
            public int CentroId { get; set; }
            public int RespuestasCalculadoraId { get; set; }
            public System.DateTime FechaCalculo { get; set; }
        }
        public partial class RespuestasCalculadoraaser
        {
            public int id { get; set; }
            public string RazonSocial { get; set; }
            public string Nit { get; set; }
            public string TelefonoEmpresa { get; set; }
            public string NombreContacto { get; set; }
            public string DescripcionVacante { get; set; }
            public int NivelEducativoRequeridoVacante { get; set; }
            public int NumeroPuestosTrabajo { get; set; }
            public bool OficinaRecursosHumanos { get; set; }
            public int TamanoPlantaEmpresa { get; set; }
            public int HabilidadRequerida { get; set; }
            public int DestrezasRequeridas { get; set; }
            public int TipoConocimientoRequerido { get; set; }
            public int ActividadesPuestoDeTrabajo { get; set; }
            public int JovenesPorTutor { get; set; }
            public bool VacanteNueva { get; set; }
            public bool PuestoTrabajoPermanente { get; set; }
            public int ContenidoActividadFormacion { get; set; }
            public bool SalarioAdicional { get; set; }
            public int SalarioAdicionalRango { get; set; }
            public string Municipio { get; set; }
            public string UserId { get; set; }
            public int CentroId { get; set; }
            public System.DateTime FechaRegistro { get; set; }
        }

    }
}
