using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.Mvc;
using MilEmpleos;
using MilEmpleos.Models;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System.Reflection;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.ComponentModel;
using OfficeOpenXml.Style;


namespace MilEmpleos.Controllers
{
    public class RespuestasCalculadorasController : Controller
    {
        private MilEmpleosEntities db = new MilEmpleosEntities();

        // GET: RespuestasCalculadoras
        [Authorize(Roles = "Admin, Centros, Consultores, Unidad, Soporte")]
        public ActionResult Listado()
        {
            var USER_id = User.Identity.GetUserId();
            var ChangePassword = (from cp in db.AspNetUsers where cp.Id == USER_id select cp.PasswordChange).First();
            if (ChangePassword==true)
            {
                return RedirectToAction("ChangePassword", "Manage");
            }

            var CE = (from item in db.AspNetUsers
                      where item.Id == USER_id
                      select item.CentroId).First();

            var primer_CE = (from item in db.Centros
                             select item).First();

            var result_centrosEmpleo = from centrosEmpleo in db.Centros select centrosEmpleo;

            ViewBag.id_centro_empleo = CE;
            ViewBag.primer_CE = primer_CE;
            ViewBag.centro_empleo_seleccionado = primer_CE.Id;

            ViewBag.centroEmpleos = result_centrosEmpleo.ToArray();

            if (Request.IsAuthenticated && (User.IsInRole("Admin") || User.IsInRole("Unidad")))
            {
                //.Where(x => x.CentroId.Equals(primer_CE.Id))
                var resultadoCalculadora = db.ResultadoCalculadora.OrderByDescending(x => x.FechaCalculo).Include(r => r.AspNetUsers).Include(r => r.Centros).Include(r => r.RespuestasCalculadora);
                return View(resultadoCalculadora.ToList());
            }
            else {
                var resultadoCalculadora = db.ResultadoCalculadora.Where(x => x.CentroId.Equals(CE)).OrderByDescending(x => x.FechaCalculo).Include(r => r.AspNetUsers).Include(r => r.Centros).Include(r => r.RespuestasCalculadora);
                return View(resultadoCalculadora.ToList());
            }
        }

        // GET: ResultadoCalculadoras/Details/5
        public ActionResult Index(Guid? id)
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

        public ActionResult BarcodeImage(String barcodeText)
        {
            // generating a barcode here. Code is taken from QrCode.Net library
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(barcodeText, out qrCode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(4, QuietZoneModules.Four), Brushes.Black, Brushes.White);

            Stream memoryStream = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);

            // very important to reset memory stream to a starting position, otherwise you would get 0 bytes returned
            memoryStream.Position = 0;
            var resultStream = new FileStreamResult(memoryStream, "image/png");
            resultStream.FileDownloadName = String.Format("{0}.png", barcodeText);

            return resultStream;
        }
        public ActionResult BarcodeImageDownload(String barcodeText)
        {
            // generating a barcode here. Code is taken from QrCode.Net library
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(barcodeText, out qrCode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(4, QuietZoneModules.Four), Brushes.Black, Brushes.White);

            Stream memoryStream = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);

            // very important to reset memory stream to a starting position, otherwise you would get 0 bytes returned
            memoryStream.Position = 0;
            Uri uri = new Uri(System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
            String host = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port + "/RespuestasCalculadoras/Index/";
            String hostdescarga = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port + "/";
            string idresultado = barcodeText.Replace(host, "");
            idresultado = idresultado.Replace(hostdescarga, "");
            var RazonSocial = (from item in db.ResultadoCalculadora
                               where item.id.ToString() == idresultado
                               select item.RespuestasCalculadora.RazonSocial).First();
            var ResulForEmployer = (from item in db.ResultadoCalculadora
                                    where item.RespuestasCalculadora.RazonSocial == RazonSocial
                                    select item).ToList();
            string NombreArcivo = RazonSocial + "-" + ResulForEmployer.Count().ToString();
            var resultStream = new FileStreamResult(memoryStream, "image/png");
            resultStream.FileDownloadName = String.Format("{0}.png", NombreArcivo);

            return resultStream;
        }
        // GET: RespuestasCalculadoras1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RespuestasCalculadora respuestasCalculadora = db.RespuestasCalculadora.Find(id);
            if (respuestasCalculadora == null)
            {
                return HttpNotFound();
            }
            return View(respuestasCalculadora);
        }

        // GET: RespuestasCalculadoras1/Create
        public ActionResult Crear()
        {
            var USER_id = User.Identity.GetUserId();
            var ChangePassword = (from cp in db.AspNetUsers where cp.Id == USER_id select cp.PasswordChange).First();
            if (ChangePassword==true)
            {
                return RedirectToAction("ChangePassword", "Manage");
            }
            ViewBag.ActividadesPuestoDeTrabajo = new SelectList(db.ActividadesPuestoDeTrabajo, "id", "ActividadesPuestoDeTrabajo1");
            ViewBag.ContenidoActividadFormacion = new SelectList(db.ContenidoActividadFormacion, "id", "ContenidoActividadFormacion1");
            ViewBag.DestrezasRequeridas = new SelectList(db.Destrezas, "id", "Destreza");
            ViewBag.HabilidadRequerida = new SelectList(db.Habilidad, "id", "HabilidadRequerida");
            ViewBag.JovenesPorTutor = new SelectList(db.JovenesPorTutor, "id", "JovenesPorTutor1");
            ViewBag.Municipio = new SelectList(db.municipality, "IdCodigo", "nombre");
            ViewBag.NivelEducativoRequeridoVacante = new SelectList(db.NivelEducativo, "id", "NivelEducativo1");
            ViewBag.SalarioAdicionalRango = new SelectList(db.SalarioAdicionalRango, "id", "SalarioAdicionalRango1");
            ViewBag.TipoConocimientoRequerido = new SelectList(db.TipoConocimientoRequerido, "id", "TipoConocimientoRequerido1");
            return View();
        }

        // POST: RespuestasCalculadoras1/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "id,RazonSocial,Nit,TelefonoEmpresa,NombreContacto,DescripcionVacante,NivelEducativoRequeridoVacante,NumeroPuestosTrabajo,OficinaRecursosHumanos,TamanoPlantaEmpresa,HabilidadRequerida,DestrezasRequeridas,TipoConocimientoRequerido,ActividadesPuestoDeTrabajo,JovenesPorTutor,VacanteNueva,PuestoTrabajoPermanente,ContenidoActividadFormacion,SalarioAdicional,SalarioAdicionalRango,Municipio,UserId,CentroId")] RespuestasCalculadora respuestasCalculadora)
        {
            if (ModelState.IsValid)
            {
                var USER_id = User.Identity.GetUserId();
                var CE = (from item in db.AspNetUsers
                          where item.Id == USER_id
                          select item.CentroId).First();
                respuestasCalculadora.CentroId = CE;
                respuestasCalculadora.UserId = USER_id;
                db.RespuestasCalculadora.Add(respuestasCalculadora);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ActividadesPuestoDeTrabajo = new SelectList(db.ActividadesPuestoDeTrabajo, "id", "ActividadesPuestoDeTrabajo1", respuestasCalculadora.ActividadesPuestoDeTrabajo);
            ViewBag.ContenidoActividadFormacion = new SelectList(db.ContenidoActividadFormacion, "id", "ContenidoActividadFormacion1", respuestasCalculadora.ContenidoActividadFormacion);
            ViewBag.DestrezasRequeridas = new SelectList(db.Destrezas, "id", "Destreza", respuestasCalculadora.DestrezasRequeridas);
            ViewBag.HabilidadRequerida = new SelectList(db.Habilidad, "id", "HabilidadRequerida", respuestasCalculadora.HabilidadRequerida);
            ViewBag.JovenesPorTutor = new SelectList(db.JovenesPorTutor, "id", "JovenesPorTutor1", respuestasCalculadora.JovenesPorTutor);
            ViewBag.Municipio = new SelectList(db.municipality, "IdCodigo", "nombre", respuestasCalculadora.Municipio);
            ViewBag.NivelEducativoRequeridoVacante = new SelectList(db.NivelEducativo, "id", "NivelEducativo1", respuestasCalculadora.NivelEducativoRequeridoVacante);
            ViewBag.SalarioAdicionalRango = new SelectList(db.SalarioAdicionalRango, "id", "SalarioAdicionalRango1", respuestasCalculadora.SalarioAdicionalRango);
            ViewBag.TipoConocimientoRequerido = new SelectList(db.TipoConocimientoRequerido, "id", "TipoConocimientoRequerido1", respuestasCalculadora.TipoConocimientoRequerido);
            return View(respuestasCalculadora);
        }
        [Authorize(Roles = "Admin, Centros, Consultores, Unidad, Soporte")]
        // GET: RespuestasCalculadoras1/Create
        public ActionResult Create()
        {
            var USER_id = User.Identity.GetUserId();
            var ChangePassword = (from cp in db.AspNetUsers where cp.Id == USER_id select cp.PasswordChange).First();
            if (ChangePassword == true)
            {
                return RedirectToAction("ChangePassword", "Manage");
            }
            ViewBag.ActividadesPuestoDeTrabajo = new SelectList(db.ActividadesPuestoDeTrabajo, "id", "ActividadesPuestoDeTrabajo1");
            ViewBag.ContenidoActividadFormacion = new SelectList(db.ContenidoActividadFormacion, "id", "ContenidoActividadFormacion1");
            ViewBag.DestrezasRequeridas = new SelectList(db.Destrezas, "id", "Destreza");
            ViewBag.HabilidadRequerida = new SelectList(db.Habilidad, "id", "HabilidadRequerida");
            ViewBag.JovenesPorTutor = new SelectList(db.JovenesPorTutor, "id", "JovenesPorTutor1");
            ViewBag.NivelEducativoRequeridoVacante = new SelectList(db.NivelEducativo, "id", "NivelEducativo1");
            ViewBag.SalarioAdicionalRango = new SelectList(db.SalarioAdicionalRango, "id", "SalarioAdicionalRango1");
            ViewBag.TipoConocimientoRequerido = new SelectList(db.TipoConocimientoRequerido, "id", "TipoConocimientoRequerido1");
            ViewBag.Municipio  = db.municipality.ToList().Select(t => new GroupedSelectListItem
            {
                GroupKey = t.department.IdCodigo,
                GroupName = t.department.nombre,
                Text = t.nombre,
                Value = t.IdCodigo.ToString()
            });
            return View();
        }

        // POST: RespuestasCalculadoras1/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,RazonSocial,Nit,TelefonoEmpresa,NombreContacto,DescripcionVacante,NivelEducativoRequeridoVacante,NumeroPuestosTrabajo,OficinaRecursosHumanos,TamanoPlantaEmpresa,HabilidadRequerida,DestrezasRequeridas,TipoConocimientoRequerido,ActividadesPuestoDeTrabajo,JovenesPorTutor,VacanteNueva,PuestoTrabajoPermanente,ContenidoActividadFormacion,SalarioAdicional,SalarioAdicionalRango,Municipio,UserId,CentroId")] RespuestasCalculadora respuestasCalculadora)
        {
            if (!respuestasCalculadora.SalarioAdicional)
            {
                respuestasCalculadora.SalarioAdicionalRango = 1;
            }
            if (ModelState.IsValid || !respuestasCalculadora.SalarioAdicional)
            {
                var USER_id = User.Identity.GetUserId();
                var CE = (from item in db.AspNetUsers
                          where item.Id == USER_id
                          select item.CentroId).First();
                if (respuestasCalculadora.SalarioAdicional) {
                    int salarioNivEdu = 0;
                    if (respuestasCalculadora.NivelEducativoRequeridoVacante == 1) { salarioNivEdu = 644350; }
                    if (respuestasCalculadora.NivelEducativoRequeridoVacante == 2) { salarioNivEdu = 700000; }
                    if (respuestasCalculadora.NivelEducativoRequeridoVacante == 3) { salarioNivEdu = 750000; }
                    if (respuestasCalculadora.NivelEducativoRequeridoVacante == 4) { salarioNivEdu = 900000; }
                    int Salario = respuestasCalculadora.SalarioAdicionalRango;
                    if (Salario - salarioNivEdu > 1000000) { respuestasCalculadora.SalarioAdicionalRango = 4; }
                    else if (Salario - salarioNivEdu >= 500001 && Salario - salarioNivEdu <= 1000000) { respuestasCalculadora.SalarioAdicionalRango = 3; }
                    else if (Salario - salarioNivEdu >= 300001 && Salario - salarioNivEdu <= 500000) { respuestasCalculadora.SalarioAdicionalRango = 2; }
                    else if (Salario - salarioNivEdu >= 100000 && Salario - salarioNivEdu <= 300000) { respuestasCalculadora.SalarioAdicionalRango = 1; }
                    else { respuestasCalculadora.SalarioAdicionalRango = 1; respuestasCalculadora.SalarioAdicional = false; }
                }

                respuestasCalculadora.CentroId = CE;
                respuestasCalculadora.UserId = USER_id;
                respuestasCalculadora.FechaRegistro = DateTime.Now;
                db.RespuestasCalculadora.Add(respuestasCalculadora);
                db.SaveChanges();
                double TotalCalificacion=0;
                var RecursosHumanos = (from p in db.Ponderacion
                                      where p.PreguntaId == 7 
                                 select p.PorcentageRespuesta).First();
               if(respuestasCalculadora.OficinaRecursosHumanos){
                   TotalCalificacion =  1 * RecursosHumanos;
                }
                //
               var Formacion = (from p in db.Ponderacion
                                where p.PreguntaId == 16 && p.RespuestaId == respuestasCalculadora.ContenidoActividadFormacion
                                select p.PorcentageRespuesta).First();

                   TotalCalificacion = TotalCalificacion + 1 * Formacion;
               //
               var JovenesPorTutor = (from p in db.Ponderacion
                                      where p.PreguntaId == 13 && p.RespuestaId == respuestasCalculadora.JovenesPorTutor
                                      select p.PorcentageRespuesta).First();
                   TotalCalificacion = TotalCalificacion + JovenesPorTutor;

                double TotalCapitalHumano = TotalCalificacion;
                //Relación de 0 a 20%
                //Entre 20 y 49%
                //Mas del 50%
                double TotalCaracteristicasEmpresa = 0;
               int TamanoempresaNodePuestosId = 0;
               double PorcentajeTamanoempresaNodePuestos = (double)respuestasCalculadora.NumeroPuestosTrabajo / (double)respuestasCalculadora.TamanoPlantaEmpresa;
               if (PorcentajeTamanoempresaNodePuestos<=.2)
               {
                   TamanoempresaNodePuestosId = 1;
                }
               if (   0.20 <PorcentajeTamanoempresaNodePuestos && PorcentajeTamanoempresaNodePuestos<= .49)
               {
                   TamanoempresaNodePuestosId = 2;
               }
               if (PorcentajeTamanoempresaNodePuestos >.50)
               {
                   TamanoempresaNodePuestosId = 3;
               }
                var TamanoempresaNodePuestos = (from p in db.Ponderacion
                                                where p.PreguntaId == 8 && p.RespuestaId == TamanoempresaNodePuestosId
                                                select p.PorcentageRespuesta).First();

                   TotalCalificacion = TotalCalificacion + TamanoempresaNodePuestos;
                   TotalCaracteristicasEmpresa = TamanoempresaNodePuestos;
                //
               var NovedadVacante  = (from p in db.Ponderacion
                                      where p.PreguntaId == 14
                                      select p.PorcentageRespuesta).First();
               if (respuestasCalculadora.VacanteNueva)
               {
                   TotalCalificacion = TotalCalificacion + NovedadVacante;
                   TotalCaracteristicasEmpresa  += NovedadVacante;
               }
               
               //
               var PermanenciaVacante = (from p in db.Ponderacion
                                     where p.PreguntaId == 15
                                     select p.PorcentageRespuesta).First();
               if (respuestasCalculadora.PuestoTrabajoPermanente)
               {
                   TotalCalificacion = TotalCalificacion + PermanenciaVacante;
                   TotalCaracteristicasEmpresa  += PermanenciaVacante;
               }
               
                 //
               double TotalCaracteristicasPuesto=0;
               var PorcentSalarioAdicional = (from p in db.Ponderacion
                                      where p.PreguntaId == 18 && p.RespuestaId == respuestasCalculadora.SalarioAdicionalRango
                                      select p.PorcentageRespuesta).First();
               if (respuestasCalculadora.SalarioAdicional)
               {
                   TotalCalificacion = TotalCalificacion + PorcentSalarioAdicional;
                   TotalCaracteristicasPuesto = PorcentSalarioAdicional;
               }
                //ciudad Ponderacion
               var PonderacionMunicipio = (from p in db.PonderacionMunicipio
                                              where p.IdCodigo == respuestasCalculadora.Municipio
                                              select p.Ponderacion).First();
               TotalCalificacion += PonderacionMunicipio;
               TotalCaracteristicasPuesto += PonderacionMunicipio;


                   //------------------Relación funciones habilidades----------------------------
                   var ActividadDestreza = (from p in db.ActividadNivEduDes
                                            where p.ActividadId == respuestasCalculadora.ActividadesPuestoDeTrabajo && p.DestrezaId == respuestasCalculadora.DestrezasRequeridas
                                           select p.ValorDestreza).First();
                   var ActividadHabilidad = (from p in db.ActividadNivEduHab
                                            where p.ActividadId == respuestasCalculadora.ActividadesPuestoDeTrabajo && p.HabilidadId == respuestasCalculadora.HabilidadRequerida
                                            select p.ValorHabilidad).First();
                   var ActividadNivelEducativo= (from p in db.ActNivEdu
                                                 where p.ActividadId == respuestasCalculadora.ActividadesPuestoDeTrabajo && p.NivelEducativoId == respuestasCalculadora.NivelEducativoRequeridoVacante
                                                 select p.Mutipliacdor).First();
                   double Relaciónfuncioneshabilidades = ActividadNivelEducativo * (ActividadDestreza + ActividadHabilidad)*.15;
                //Suma Final
                   TotalCalificacion = TotalCalificacion + Relaciónfuncioneshabilidades;
                   TotalCaracteristicasPuesto  += Relaciónfuncioneshabilidades;

                //
                ResultadoCalculadora resutadoCalculadora =new ResultadoCalculadora();

                resutadoCalculadora.id = Guid.NewGuid();
                resutadoCalculadora.UserId = USER_id;
                resutadoCalculadora.Total = TotalCalificacion;
                resutadoCalculadora.CentroId = CE;
                resutadoCalculadora.CapitalHumano = TotalCapitalHumano;
                resutadoCalculadora.CaracteristicasEmpresa = TotalCaracteristicasEmpresa;
                resutadoCalculadora.CaracteristicasPuesto = TotalCaracteristicasPuesto;
                resutadoCalculadora.RespuestasCalculadoraId = respuestasCalculadora.id;
                resutadoCalculadora.FechaCalculo = DateTime.Now;
                db.ResultadoCalculadora.Add(resutadoCalculadora);
                db.SaveChanges();

                return RedirectToAction("Index/" + resutadoCalculadora.id);
            }

            ViewBag.ActividadesPuestoDeTrabajo = new SelectList(db.ActividadesPuestoDeTrabajo, "id", "ActividadesPuestoDeTrabajo1");
            ViewBag.ContenidoActividadFormacion = new SelectList(db.ContenidoActividadFormacion, "id", "ContenidoActividadFormacion1");
            ViewBag.DestrezasRequeridas = new SelectList(db.Destrezas, "id", "Destreza");
            ViewBag.HabilidadRequerida = new SelectList(db.Habilidad, "id", "HabilidadRequerida");
            ViewBag.JovenesPorTutor = new SelectList(db.JovenesPorTutor, "id", "JovenesPorTutor1");
            ViewBag.NivelEducativoRequeridoVacante = new SelectList(db.NivelEducativo, "id", "NivelEducativo1");
            ViewBag.SalarioAdicionalRango = new SelectList(db.SalarioAdicionalRango, "id", "SalarioAdicionalRango1");
            ViewBag.TipoConocimientoRequerido = new SelectList(db.TipoConocimientoRequerido, "id", "TipoConocimientoRequerido1");
            ViewBag.Municipio = db.municipality.ToList().Select(t => new GroupedSelectListItem
            {
                GroupKey = t.department.IdCodigo,
                GroupName = t.department.nombre,
                Text = t.nombre,
                Value = t.IdCodigo.ToString()
            });
            return View(respuestasCalculadora);
        }
        // GET: RespuestasCalculadoras1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RespuestasCalculadora respuestasCalculadora = db.RespuestasCalculadora.Find(id);
            if (respuestasCalculadora == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActividadesPuestoDeTrabajo = new SelectList(db.ActividadesPuestoDeTrabajo, "id", "ActividadesPuestoDeTrabajo1", respuestasCalculadora.ActividadesPuestoDeTrabajo);
            ViewBag.ContenidoActividadFormacion = new SelectList(db.ContenidoActividadFormacion, "id", "ContenidoActividadFormacion1", respuestasCalculadora.ContenidoActividadFormacion);
            ViewBag.DestrezasRequeridas = new SelectList(db.Destrezas, "id", "Destreza", respuestasCalculadora.DestrezasRequeridas);
            ViewBag.HabilidadRequerida = new SelectList(db.Habilidad, "id", "HabilidadRequerida", respuestasCalculadora.HabilidadRequerida);
            ViewBag.JovenesPorTutor = new SelectList(db.JovenesPorTutor, "id", "JovenesPorTutor1", respuestasCalculadora.JovenesPorTutor);
            ViewBag.Municipio = new SelectList(db.municipality, "IdCodigo", "nombre", respuestasCalculadora.Municipio);
            ViewBag.NivelEducativoRequeridoVacante = new SelectList(db.NivelEducativo, "id", "NivelEducativo1", respuestasCalculadora.NivelEducativoRequeridoVacante);
            ViewBag.SalarioAdicionalRango = new SelectList(db.SalarioAdicionalRango, "id", "SalarioAdicionalRango1", respuestasCalculadora.SalarioAdicionalRango);
            ViewBag.TipoConocimientoRequerido = new SelectList(db.TipoConocimientoRequerido, "id", "TipoConocimientoRequerido1", respuestasCalculadora.TipoConocimientoRequerido);
            return View(respuestasCalculadora);
        }

        // POST: RespuestasCalculadoras1/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,RazonSocial,Nit,TelefonoEmpresa,NombreContacto,DescripcionVacante,NivelEducativoRequeridoVacante,NumeroPuestosTrabajo,OficinaRecursosHumanos,TamanoPlantaEmpresa,HabilidadRequerida,DestrezasRequeridas,TipoConocimientoRequerido,ActividadesPuestoDeTrabajo,JovenesPorTutor,VacanteNueva,PuestoTrabajoPermanente,ContenidoActividadFormacion,SalarioAdicional,SalarioAdicionalRango,Municipio,UserId,CentroId")] RespuestasCalculadora respuestasCalculadora)
        {
            if (ModelState.IsValid)
            {
                db.Entry(respuestasCalculadora).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ActividadesPuestoDeTrabajo = new SelectList(db.ActividadesPuestoDeTrabajo, "id", "ActividadesPuestoDeTrabajo1", respuestasCalculadora.ActividadesPuestoDeTrabajo);
            ViewBag.ContenidoActividadFormacion = new SelectList(db.ContenidoActividadFormacion, "id", "ContenidoActividadFormacion1", respuestasCalculadora.ContenidoActividadFormacion);
            ViewBag.DestrezasRequeridas = new SelectList(db.Destrezas, "id", "Destreza", respuestasCalculadora.DestrezasRequeridas);
            ViewBag.HabilidadRequerida = new SelectList(db.Habilidad, "id", "HabilidadRequerida", respuestasCalculadora.HabilidadRequerida);
            ViewBag.JovenesPorTutor = new SelectList(db.JovenesPorTutor, "id", "JovenesPorTutor1", respuestasCalculadora.JovenesPorTutor);
            ViewBag.Municipio = new SelectList(db.municipality, "IdCodigo", "nombre", respuestasCalculadora.Municipio);
            ViewBag.NivelEducativoRequeridoVacante = new SelectList(db.NivelEducativo, "id", "NivelEducativo1", respuestasCalculadora.NivelEducativoRequeridoVacante);
            ViewBag.SalarioAdicionalRango = new SelectList(db.SalarioAdicionalRango, "id", "SalarioAdicionalRango1", respuestasCalculadora.SalarioAdicionalRango);
            ViewBag.TipoConocimientoRequerido = new SelectList(db.TipoConocimientoRequerido, "id", "TipoConocimientoRequerido1", respuestasCalculadora.TipoConocimientoRequerido);
            return View(respuestasCalculadora);
        }

        // GET: RespuestasCalculadoras1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RespuestasCalculadora respuestasCalculadora = db.RespuestasCalculadora.Find(id);
            if (respuestasCalculadora == null)
            {
                return HttpNotFound();
            }
            return View(respuestasCalculadora);
        }

        // POST: RespuestasCalculadoras1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RespuestasCalculadora respuestasCalculadora = db.RespuestasCalculadora.Find(id);
            db.RespuestasCalculadora.Remove(respuestasCalculadora);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: RespuestasCalculadoras1/Pdf/5
        public ActionResult Pdf(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RespuestasCalculadora respuestasCalculadora = db.RespuestasCalculadora.Find(id);
            if (respuestasCalculadora == null)
            {
                return HttpNotFound();
            }
                     
            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            MemoryStream memStream = new MemoryStream();
            PdfWriter wri = PdfWriter.GetInstance(doc, memStream);
            try
            {

                doc.Open();//Open Document to write
                iTextSharp.text.Font font8 = FontFactory.GetFont("ARIAL", 6);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                //Write some content
                iTextSharp.text.Image imagen;
                imagen = iTextSharp.text.Image.GetInstance(Server.MapPath("~/") + "Content/Images/zapatos.png");
                imagen.ScalePercent(32f);
                imagen.SetAbsolutePosition(30, 720);
                doc.Add(imagen);
                Paragraph paragraph = new Paragraph("VALIDACIÓN DE REQUISITOS MÍNIMOS DE LAS VACANTES", boldFont);
                doc.AddTitle("VALIDACIÓN DE REQUISITOS MÍNIMOS DE LAS VACANTES");
                doc.AddCreator("Unidad del Servicio Público de Empleo");
                paragraph.Alignment = Element.ALIGN_CENTER;
                doc.Add(paragraph);
                paragraph = new Paragraph("PROGRAMA 40 MIL PRIMEROS EMPLEOS", boldFont);
                paragraph.Alignment = Element.ALIGN_CENTER;
                doc.Add(paragraph);
                PdfPTable tblheader = new PdfPTable(2);
                tblheader.WidthPercentage = 85;
                tblheader.DefaultCell.Padding = 10f;
                tblheader.SpacingBefore = 15f;
                float[] widths = new float[] { 75f, 25f,};
                tblheader.SetWidths(widths);
                int idrespuesta = Convert.ToInt32(id);
                var resultadoCalculadora = (from r in db.ResultadoCalculadora where r.RespuestasCalculadoraId == idrespuesta select r).First();
                // Configuramos el título de las columnas de la tabla
                PdfPCell clencabezados = new PdfPCell(new Phrase("I. INFORMACIÓN GENERAL", boldFont));
                clencabezados.BorderWidth = 0;
                clencabezados.PaddingBottom = 5f;
                clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                Uri uri = Request.Url;
                string dirVirtual = uri.Segments[1];
                if (dirVirtual.Contains("RespuestasCalculadoras"))
                {
                    dirVirtual = "/";
                }
                else {
                    dirVirtual = "/" + dirVirtual;
                }
                String host = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port + dirVirtual;
                String textQR =  @host.ToLower() + "RespuestasCalculadoras/Index/" + resultadoCalculadora.id.ToString();
                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                QrCode qrCode = new QrCode();
                qrEncoder.TryEncode(textQR, out qrCode);
                GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(2, QuietZoneModules.Four), Brushes.Black, Brushes.White);
                Stream memoryStream = new MemoryStream();
                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);
                // very important to reset memory stream to a starting position, otherwise you would get 0 bytes returned
                memoryStream.Position = 0;
                string color = "";
                if (resultadoCalculadora.Total > .6) { color = "green"; } else if (resultadoCalculadora.Total > .5) { color = "orange"; } else { color = "red"; }
                string imagensemaforo = @host + "/Content/images/semaforo" + color + ".png";
                iTextSharp.text.Image semaforo = iTextSharp.text.Image.GetInstance(imagensemaforo);
                semaforo.ScaleAbsolute(80f, 125f);
                PdfPCell clQRcode = new PdfPCell(semaforo);
                clQRcode.BorderWidth = 0;
                clQRcode.Rowspan = 9;
                clQRcode.HorizontalAlignment = Element.ALIGN_CENTER;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                tblheader.AddCell(clQRcode);
                // Llenamos la tabla con información
                var titleChunk = new Chunk("Centro de Empleo: ", boldFont);
                var descriptionChunk = new Chunk(resultadoCalculadora.Centros.CentroEmpleo, normalFont);
                var phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.PaddingBottom = 5f;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                titleChunk = new Chunk("Razón Social: ", boldFont);
                descriptionChunk = new Chunk(respuestasCalculadora.RazonSocial, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.PaddingBottom = 5f;
                clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                titleChunk = new Chunk("Fecha de diligenciamiento: ", boldFont);
                descriptionChunk = new Chunk(resultadoCalculadora.FechaCalculo.ToString(@"yyyy/MM/dd HH\:mm\:ss tt"), normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.PaddingBottom = 5f;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                titleChunk = new Chunk("Total calificación: ", boldFont);
                descriptionChunk = new Chunk(resultadoCalculadora.Total.ToString(), normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.PaddingBottom = 5f;
                clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);

                // Llenamos la tabla con información
                titleChunk = new Chunk("Calificación Capital Humano: ", boldFont);
                descriptionChunk = new Chunk(resultadoCalculadora.CapitalHumano.ToString(), normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.PaddingBottom = 5f;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);

                // Llenamos la tabla con información
                titleChunk = new Chunk("Calificación características de la empresa: ", boldFont);
                descriptionChunk = new Chunk(resultadoCalculadora.CaracteristicasEmpresa.ToString(), normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.PaddingBottom = 5f;
                clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);

                // Llenamos la tabla con información
                titleChunk = new Chunk("Calificación características del puesto de trabajo: ", boldFont);
                descriptionChunk = new Chunk(resultadoCalculadora.CaracteristicasPuesto.ToString(), normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.PaddingBottom = 5f;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);

                // Llenamos la tabla con información
                titleChunk = new Chunk("Codigo de calificación: ", boldFont);
                descriptionChunk = new Chunk(resultadoCalculadora.id.ToString(), normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.PaddingBottom = 5f;
                clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);

                // Llenamos la tabla con información
                titleChunk = new Chunk("Descripción vacante: ", boldFont);
                descriptionChunk = new Chunk(respuestasCalculadora.DescripcionVacante, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                 // Llenamos la tabla con información
                titleChunk = new Chunk("Nombre de contacto: ", boldFont);
                descriptionChunk = new Chunk(respuestasCalculadora.NombreContacto, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                titleChunk = new Chunk("Nivel educativo requerido para la vacante ", boldFont);
                descriptionChunk = new Chunk("" + respuestasCalculadora.NivelEducativo.NivelEducativo1, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                titleChunk = new Chunk("Numero de puestos de trabajo solicitados: ", boldFont);
                descriptionChunk = new Chunk("" + respuestasCalculadora.NumeroPuestosTrabajo, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                string  rta = (respuestasCalculadora.OficinaRecursosHumanos) ? "SI" : "NO";
                titleChunk = new Chunk("Tiene la empresa una oficina de recursos humanos? ", boldFont);
                descriptionChunk = new Chunk("" + rta, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                titleChunk = new Chunk("Cuál es el tamaño de la planta de su empresa? ", boldFont);
                descriptionChunk = new Chunk("" + respuestasCalculadora.TamanoPlantaEmpresa, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);                
                // Llenamos la tabla con información
                titleChunk = new Chunk("Cuál es la principal habilidad que requiere el jóven en esta vacante? ", boldFont);
                descriptionChunk = new Chunk("" + respuestasCalculadora.Habilidad.HabilidadRequerida, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                titleChunk = new Chunk("Cuál es la principal destreza que requiere el jóven para desempeñarse en esta vacante? ", boldFont);
                descriptionChunk = new Chunk("" + respuestasCalculadora.Destrezas.Destreza, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                titleChunk = new Chunk("Cuál es el principal tipo de conocimientos que requiere el jóven para desempeñarse en esta vacante? ", boldFont);
                descriptionChunk = new Chunk("" + respuestasCalculadora.TipoConocimientoRequerido1.TipoConocimientoRequerido1, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                titleChunk = new Chunk("Cuáles son las actividades generales de este puesto de trabajo?  ", boldFont);
                descriptionChunk = new Chunk("" + respuestasCalculadora.ActividadesPuestoDeTrabajo1.ActividadesPuestoDeTrabajo1, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                 rta = (respuestasCalculadora.VacanteNueva) ? "SI" : "NO";
                titleChunk = new Chunk("Esta es una vacante nueva en la empresa? ", boldFont);
                descriptionChunk = new Chunk("" + rta, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                 // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                titleChunk = new Chunk("Cuántos jóvenes serán asignados a cada tutor? ", boldFont);
                descriptionChunk = new Chunk("" + respuestasCalculadora.JovenesPorTutor1.JovenesPorTutor1, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                rta = (respuestasCalculadora.PuestoTrabajoPermanente) ? "SI" : "NO";
                titleChunk = new Chunk("Este puesto de trabajo será permanente en su empresa? ", boldFont);
                descriptionChunk = new Chunk("" + rta, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                 // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                titleChunk = new Chunk("Indique cuál sería el principal contenido para las actividades de formación de los jóvenes en el programa \"40 Mil primeros empleos\" en este puesto de trabajo? ", boldFont);
                descriptionChunk = new Chunk("\n " + respuestasCalculadora.ContenidoActividadFormacion1.ContenidoActividadFormacion1, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                // Llenamos la tabla con información
                rta = (respuestasCalculadora.SalarioAdicional) ? "SI" : "NO";
                titleChunk = new Chunk("Está dispuesto a ofrecer un salario superior al establecido en el programa 40 Mil Primeros Empleos? ", boldFont);
                descriptionChunk = new Chunk("" + rta, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                if (respuestasCalculadora.SalarioAdicional) {
                    // Llenamos la tabla con información
                    titleChunk = new Chunk("Rango salario adicional que se asignarará al joven: ", boldFont);
                    descriptionChunk = new Chunk("" + respuestasCalculadora.SalarioAdicionalRango1.SalarioAdicionalRango1, normalFont);
                    phrase = new Phrase(titleChunk);
                    phrase.Add(descriptionChunk);
                    clencabezados = new PdfPCell(phrase);
                    clencabezados.BorderWidth = 0;
                    clencabezados.Colspan = 2;
                    clencabezados.PaddingBottom = 5f;
                    clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    // Añadimos las celdas a la tabla
                    tblheader.AddCell(clencabezados);
                }
                // Llenamos la tabla con información
                titleChunk = new Chunk("En qué ciudad se encuentra el puesto de trabajo? ", boldFont);
                descriptionChunk = new Chunk("" + respuestasCalculadora.municipality.nombre + "/" + respuestasCalculadora.municipality.department.nombre, normalFont);
                phrase = new Phrase(titleChunk);
                phrase.Add(descriptionChunk);
                clencabezados = new PdfPCell(phrase);
                clencabezados.BorderWidth = 0;
                clencabezados.Colspan = 2;
                clencabezados.PaddingBottom = 5f;
                if (!respuestasCalculadora.SalarioAdicional)
                {
                    clencabezados.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                }
                // Añadimos las celdas a la tabla
                tblheader.AddCell(clencabezados);
                doc.Add(tblheader);

                //tbl footer
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.TotalWidth = doc.PageSize.Width;

                Chunk myFooter = new Chunk( DateTime.Now.ToString(@"yyyy/MM/dd HH\:mm\:ss"), FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8));

                PdfPCell footer = new PdfPCell(new Phrase(myFooter));
                footer.Border = iTextSharp.text.Rectangle.NO_BORDER;
                footer.HorizontalAlignment = Element.ALIGN_RIGHT;
                footer.PaddingRight = 26;
                footerTbl.AddCell(footer);

                //img footer
                string footerFilePath = Server.MapPath("~/") + "Content/Images/footer.png";

                iTextSharp.text.Image foot = iTextSharp.text.Image.GetInstance(footerFilePath);
                foot.ScalePercent(45);
                footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cell = new PdfPCell(foot);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;

                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 0, (doc.BottomMargin + 70), wri.DirectContent);
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(memoryStream);
                gif.SetAbsolutePosition(250, 13);
                doc.Add(gif);
            }
            finally
            {
                //Close document and writer
                doc.Close();

            }

            byte[] fileBytes = memStream.ToArray();
            string fileName = respuestasCalculadora.Nit.ToString() + "_" + respuestasCalculadora.id.ToString() + "_" + DateTime.Now.ToString(@"yyyyMMddHHmmss") + ".pdf";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        // GET: RespuestasCalculadoras1/Excelrespuestas/5
        public ActionResult Excelrespuestas(int? p_idCentro)
        {
            var USER_id = User.Identity.GetUserId();

            var CE = (from item in db.AspNetUsers
                      where item.Id == USER_id
                      select item.CentroId).First();

            List<ResCalcollection> myListresPila = new List<ResCalcollection>();
            //if (User.Identity.Name.ToString().ToLower() == "admin40mil@mintrabajo.gov.co")
            if (User.IsInRole("Admin") || User.IsInRole("Unidad"))
            {
                var result = (from res in db.ResultadoCalculadora where res.CentroId == p_idCentro select res).OrderByDescending(x => x.FechaCalculo).ToList();

                foreach (var rescal in result)
                {
                    ResCalcollection resCalcollection = new ResCalcollection();
                    resCalcollection.id = rescal.RespuestasCalculadoraId;
                    resCalcollection.Usuario_Correo = rescal.AspNetUsers.Email;
                    resCalcollection.Prestador = rescal.Centros.CentroEmpleo;
                    resCalcollection.Capital_Humano = rescal.CapitalHumano.ToString();
                    resCalcollection.Caracteristicas_Empresa = rescal.CaracteristicasEmpresa.ToString();
                    resCalcollection.Caracteristicas_Puesto = rescal.CaracteristicasPuesto.ToString();
                    string rta = (rescal.Total >= 0.6) ? "SI" : "NO";
                    resCalcollection.VacanteValida = rta;
                    resCalcollection.Total_Calificacion = rescal.Total.ToString();
                    resCalcollection.Razon_Social = rescal.RespuestasCalculadora.RazonSocial;
                    resCalcollection.Nit = rescal.RespuestasCalculadora.Nit;
                    resCalcollection.Telefono_Empresa = rescal.RespuestasCalculadora.TelefonoEmpresa.ToString();
                    resCalcollection.Nombre_Contacto = rescal.RespuestasCalculadora.NombreContacto;
                    resCalcollection.Descripcion_Vacante = rescal.RespuestasCalculadora.DescripcionVacante;
                    resCalcollection.Nivel_Educativo_Requerido_Vacante = rescal.RespuestasCalculadora.NivelEducativo.NivelEducativo1;
                    resCalcollection.Numero_Puestos_Trabajo = rescal.RespuestasCalculadora.NumeroPuestosTrabajo.ToString();
                    rta = (rescal.RespuestasCalculadora.OficinaRecursosHumanos) ? "SI" : "NO";
                    resCalcollection.Oficina_Recursos_Humanos = rta;
                    resCalcollection.Tamano_Planta_Empresa = rescal.RespuestasCalculadora.TamanoPlantaEmpresa.ToString();
                    resCalcollection.Habilidad_Requerida = rescal.RespuestasCalculadora.Habilidad.HabilidadRequerida;
                    resCalcollection.Destrezas_Requeridas = rescal.RespuestasCalculadora.Destrezas.Destreza;
                    resCalcollection.Tipo_Conocimiento_Requerido = rescal.RespuestasCalculadora.ContenidoActividadFormacion1.ContenidoActividadFormacion1;
                    resCalcollection.Actividades_Puesto_De_Trabajo = rescal.RespuestasCalculadora.ActividadesPuestoDeTrabajo1.ActividadesPuestoDeTrabajo1;
                    resCalcollection.Jovenes_Por_Tutor = rescal.RespuestasCalculadora.JovenesPorTutor1.JovenesPorTutor1;
                    rta = (rescal.RespuestasCalculadora.VacanteNueva) ? "SI" : "NO";
                    resCalcollection.Vacante_Nueva = rta;
                    rta = (rescal.RespuestasCalculadora.PuestoTrabajoPermanente) ? "SI" : "NO";
                    resCalcollection.Puesto_De_Trabajo_Permanente = rta;
                    resCalcollection.Contenido_Actividad_Formacion = rescal.RespuestasCalculadora.ContenidoActividadFormacion1.ContenidoActividadFormacion1;
                    rta = (rescal.RespuestasCalculadora.SalarioAdicional) ? "SI" : "NO";
                    resCalcollection.Salario_Adicional = rta;
                    if (rescal.RespuestasCalculadora.SalarioAdicional)
                    {
                        resCalcollection.Salario_Adicional_Rango = rescal.RespuestasCalculadora.SalarioAdicionalRango1.SalarioAdicionalRango1;
                    }
                    else {
                        resCalcollection.Salario_Adicional_Rango = "NA";
                    }
                    resCalcollection.Salario_Adicional_Rango = rescal.RespuestasCalculadora.SalarioAdicionalRango1.SalarioAdicionalRango1;
                    resCalcollection.Municipio = rescal.RespuestasCalculadora.municipality.nombre + " / " + rescal.RespuestasCalculadora.municipality.department.nombre;
                    resCalcollection.Fecha_Registro = rescal.RespuestasCalculadora.FechaRegistro.ToString(@"yyyy/MM/dd HH\:mm\:ss tt");
                    resCalcollection.CodigoCalificacion = rescal.id;
                    myListresPila.Add(resCalcollection);
                }
            }
            else
            {
                var result = (from res in db.ResultadoCalculadora where res.CentroId.Equals(CE) select res).OrderByDescending(x => x.FechaCalculo).ToList();

                foreach (var rescal in result)
                {
                    ResCalcollection resCalcollection = new ResCalcollection();
                    resCalcollection.id = rescal.RespuestasCalculadoraId;
                    resCalcollection.Usuario_Correo = rescal.AspNetUsers.Email;
                    resCalcollection.Prestador = rescal.Centros.CentroEmpleo;
                    resCalcollection.Capital_Humano = rescal.CapitalHumano.ToString();
                    resCalcollection.Caracteristicas_Empresa = rescal.CaracteristicasEmpresa.ToString();
                    resCalcollection.Caracteristicas_Puesto = rescal.CaracteristicasPuesto.ToString();
                    string rta = (rescal.Total >= 0.6) ? "SI" : "NO";
                    resCalcollection.VacanteValida = rta;
                    resCalcollection.Total_Calificacion = rescal.Total.ToString();
                    resCalcollection.Razon_Social = rescal.RespuestasCalculadora.RazonSocial;
                    resCalcollection.Nit = rescal.RespuestasCalculadora.Nit;
                    resCalcollection.Telefono_Empresa = rescal.RespuestasCalculadora.TelefonoEmpresa.ToString();
                    resCalcollection.Nombre_Contacto = rescal.RespuestasCalculadora.NombreContacto;
                    resCalcollection.Descripcion_Vacante = rescal.RespuestasCalculadora.DescripcionVacante;
                    resCalcollection.Nivel_Educativo_Requerido_Vacante = rescal.RespuestasCalculadora.NivelEducativo.NivelEducativo1;
                    resCalcollection.Numero_Puestos_Trabajo = rescal.RespuestasCalculadora.NumeroPuestosTrabajo.ToString();
                    rta = (rescal.RespuestasCalculadora.OficinaRecursosHumanos) ? "SI" : "NO";
                    resCalcollection.Oficina_Recursos_Humanos = rta;
                    resCalcollection.Tamano_Planta_Empresa = rescal.RespuestasCalculadora.TamanoPlantaEmpresa.ToString();
                    resCalcollection.Habilidad_Requerida = rescal.RespuestasCalculadora.Habilidad.HabilidadRequerida;
                    resCalcollection.Destrezas_Requeridas = rescal.RespuestasCalculadora.Destrezas.Destreza;
                    resCalcollection.Tipo_Conocimiento_Requerido = rescal.RespuestasCalculadora.ContenidoActividadFormacion1.ContenidoActividadFormacion1;
                    resCalcollection.Actividades_Puesto_De_Trabajo = rescal.RespuestasCalculadora.ActividadesPuestoDeTrabajo1.ActividadesPuestoDeTrabajo1;
                    resCalcollection.Jovenes_Por_Tutor = rescal.RespuestasCalculadora.JovenesPorTutor1.JovenesPorTutor1;
                    rta = (rescal.RespuestasCalculadora.VacanteNueva) ? "SI" : "NO";
                    resCalcollection.Vacante_Nueva = rta;
                    rta = (rescal.RespuestasCalculadora.PuestoTrabajoPermanente) ? "SI" : "NO";
                    resCalcollection.Puesto_De_Trabajo_Permanente = rta;
                    resCalcollection.Contenido_Actividad_Formacion = rescal.RespuestasCalculadora.ContenidoActividadFormacion1.ContenidoActividadFormacion1;
                    rta = (rescal.RespuestasCalculadora.SalarioAdicional) ? "SI" : "NO";
                    resCalcollection.Salario_Adicional = rta;
                    if (rescal.RespuestasCalculadora.SalarioAdicional)
                    {
                        resCalcollection.Salario_Adicional_Rango = rescal.RespuestasCalculadora.SalarioAdicionalRango1.SalarioAdicionalRango1;
                    }
                    else {
                        resCalcollection.Salario_Adicional_Rango = "NA";
                    }
                    resCalcollection.Salario_Adicional_Rango = rescal.RespuestasCalculadora.SalarioAdicionalRango1.SalarioAdicionalRango1;
                    resCalcollection.Municipio = rescal.RespuestasCalculadora.municipality.nombre + " / " + rescal.RespuestasCalculadora.municipality.department.nombre;
                    resCalcollection.Fecha_Registro = rescal.RespuestasCalculadora.FechaRegistro.ToString(@"yyyy/MM/dd HH\:mm\:ss tt");
                    resCalcollection.CodigoCalificacion = rescal.id;
                    myListresPila.Add(resCalcollection);
                }
            }

            DataTable dt = ConvertToDataTable(myListresPila);

            MemoryStream memoryStream = new MemoryStream();
            var package = new ExcelPackage(memoryStream);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Resultados Calculadora");
            //Step 3 : Start loading datatable form A1 cell of worksheet.
            worksheet.Cells["A1"].LoadFromDataTable(dt, true);
            worksheet.Cells.Style.Font.SetFromFont(new System.Drawing.Font("Calibri", 10));    
            worksheet.Cells.AutoFitColumns();    
            //Format the header    
            using (ExcelRange objRange = worksheet.Cells["A1:XFD1"])    
            {    
               objRange.Style.Font.Bold = true;    
               objRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;    
               objRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;    
               objRange.Style.Fill.PatternType = ExcelFillStyle.Solid;    
               objRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(221, 23, 33));    
            }    
            //Step 4 : (Optional) Set the file properties like title, author and subject
            package.Workbook.Properties.Title = @"Consolidado Resultados Calculadora";
            package.Workbook.Properties.Author = "2015 - Unidad del Servicio Público de Empleo";
            package.Workbook.Properties.Subject = @"Programa 40 Mil Primeros Empleos";

            //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
            package.Save();

            byte[] fileBytes = memoryStream.ToArray();
            string fileName = "Resultados_al_" + DateTime.Now.ToString(@"yyyyMMddHHmmss") + ".xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        // GET: RespuestasCalculadoras1/ExcelconsultasPila/5
        public ActionResult ExcelConsultasPila(int? p_idCentro)
        {
            var USER_id = User.Identity.GetUserId();

            var CE = (from item in db.AspNetUsers
                      where item.Id == USER_id
                      select item.CentroId).First();

            List<ConsulPilacollection> myListresPila = new List<ConsulPilacollection>();

            //if (User.Identity.Name.ToString().ToLower() == "admin40mil@mintrabajo.gov.co")
            if (User.IsInRole("Admin") || User.IsInRole("Unidad"))
            {

                var result = (from res in db.RespuestaPila where res.CentroId == p_idCentro select res).ToList();

                foreach (var resPila in result)
                {
                    ConsulPilacollection consulPilacollection = new ConsulPilacollection();
                    consulPilacollection.id = resPila.id;
                    consulPilacollection.No_Documento = resPila.NoDocumento;
                    if (resPila.AspNetUsers.Email == null)
                    {
                        consulPilacollection.Usuario_Correo = "";
                    }
                    else
                    {
                        consulPilacollection.Usuario_Correo = resPila.AspNetUsers.Email;
                    }
                    if (resPila.Centros.CentroEmpleo == null)
                    {
                        consulPilacollection.Prestador = "";
                    }
                    else
                    {
                        consulPilacollection.Prestador = resPila.Centros.CentroEmpleo;
                    }

                    string rta = (resPila.Registrado) ? "SI" : "NO";
                    consulPilacollection.Registrado = rta;
                    consulPilacollection.Nombres = resPila.Nombres;
                    consulPilacollection.Apellidos = resPila.Apellidos;
                    consulPilacollection.Fecha_Consulta_Pila = resPila.FechaRespuestaPila.ToString(@"yyyy/MM/dd HH\:mm\:ss tt");
                    consulPilacollection.Total_Cotizado_en_Meses = resPila.TiempoTotalRegistroMeses.ToString();
                    consulPilacollection.Meses_UltimoPeriodo = resPila.Meses_UltimoPeriodo.ToString();
                    consulPilacollection.UltimoPeriodo_fecha_inicio = resPila.UltimoPeriodo_fecha_inicio.ToString(@"yyyy/MM/dd HH\:mm\:ss tt");
                    consulPilacollection.UltimoPeriodo_fecha_fin = resPila.UltimoPeriodo_fecha_fin.ToString(@"yyyy/MM/dd HH\:mm\:ss tt");
                    myListresPila.Add(consulPilacollection);
                }

            }
            else
            {

                var result = (from res in db.RespuestaPila where res.CentroId.Equals(CE) select res).ToList();

                foreach (var resPila in result)
                {
                    ConsulPilacollection consulPilacollection = new ConsulPilacollection();
                    consulPilacollection.id = resPila.id;
                    consulPilacollection.No_Documento = resPila.NoDocumento;
                    if (resPila.AspNetUsers.Email == null)
                    {
                        consulPilacollection.Usuario_Correo = "";
                    }
                    else
                    {
                        consulPilacollection.Usuario_Correo = resPila.AspNetUsers.Email;
                    }
                    if (resPila.Centros.CentroEmpleo == null)
                    {
                        consulPilacollection.Prestador = "";
                    }
                    else
                    {
                        consulPilacollection.Prestador = resPila.Centros.CentroEmpleo;
                    }

                    string rta = (resPila.Registrado) ? "SI" : "NO";
                    consulPilacollection.Registrado = rta;
                    consulPilacollection.Nombres = resPila.Nombres;
                    consulPilacollection.Apellidos = resPila.Apellidos;
                    consulPilacollection.Fecha_Consulta_Pila = resPila.FechaRespuestaPila.ToString(@"yyyy/MM/dd HH\:mm\:ss tt");
                    consulPilacollection.Total_Cotizado_en_Meses = resPila.TiempoTotalRegistroMeses.ToString();
                    consulPilacollection.Meses_UltimoPeriodo = resPila.Meses_UltimoPeriodo.ToString();
                    consulPilacollection.UltimoPeriodo_fecha_inicio = resPila.UltimoPeriodo_fecha_inicio.ToString(@"yyyy/MM/dd HH\:mm\:ss tt");
                    consulPilacollection.UltimoPeriodo_fecha_fin = resPila.UltimoPeriodo_fecha_fin.ToString(@"yyyy/MM/dd HH\:mm\:ss tt");
                    myListresPila.Add(consulPilacollection);
                }

            }

            DataTable dt = ConvertToDataTable(myListresPila);

            MemoryStream memoryStream = new MemoryStream();
            var package = new ExcelPackage(memoryStream);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Respuesta Pila");
            //Step 3 : Start loading datatable form A1 cell of worksheet.
            worksheet.Cells["A1"].LoadFromDataTable(dt, true);
            worksheet.Cells.Style.Font.SetFromFont(new System.Drawing.Font("Calibri", 10));
            worksheet.Cells.AutoFitColumns();
            //Format the header    
            using (ExcelRange objRange = worksheet.Cells["A1:XFD1"])
            {
                objRange.Style.Font.Bold = true;
                objRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                objRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                objRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                objRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(221, 23, 33));
            }
            //Step 4 : (Optional) Set the file properties like title, author and subject
            package.Workbook.Properties.Title = @"Consolidado consultas a Pila";
            package.Workbook.Properties.Author = "2015 - Unidad del Servicio Público de Empleo";
            package.Workbook.Properties.Subject = @"Programa 40 Mil Primeros Empleos";

            //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
            package.Save();

            byte[] fileBytes = memoryStream.ToArray();
            string fileName = "Consultas_Pila_" + DateTime.Now.ToString(@"yyyyMMddHHmmss") + ".xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public partial class ResCalcollection
        {
            public int id { get; set; }
            public string Usuario_Correo { get; set; }
            public string Prestador { get; set; }
            public string Capital_Humano { get; set; }
            public string Caracteristicas_Empresa { get; set; }
            public string Caracteristicas_Puesto { get; set; }
            public string Total_Calificacion { get; set; }
            public string VacanteValida { get; set; }
            public string Razon_Social { get; set; }
            public string Nit { get; set; }
            public string Telefono_Empresa { get; set; }
            public string Nombre_Contacto { get; set; }
            public string Descripcion_Vacante { get; set; }
            public string Nivel_Educativo_Requerido_Vacante { get; set; }
            public string Numero_Puestos_Trabajo { get; set; }
            public string Oficina_Recursos_Humanos { get; set; }
            public string Tamano_Planta_Empresa { get; set; }
            public string Habilidad_Requerida { get; set; }
            public string Destrezas_Requeridas { get; set; }
            public string Tipo_Conocimiento_Requerido { get; set; }
            public string Actividades_Puesto_De_Trabajo { get; set; }
            public string Jovenes_Por_Tutor { get; set; }
            public string Vacante_Nueva { get; set; }
            public string Puesto_De_Trabajo_Permanente { get; set; }
            public string Contenido_Actividad_Formacion { get; set; }
            public string Salario_Adicional { get; set; }
            public string Salario_Adicional_Rango { get; set; }
            public string Municipio { get; set; }
            public string Fecha_Registro { get; set; }
            public Guid CodigoCalificacion { get; set; }
        }
        public partial class ConsulPilacollection
        {
            public int id { get; set; }
            public string Usuario_Correo { get; set; }
            public string Prestador { get; set; }
            public string Registrado { get; set; }
            public string Nombres { get; set; }
            public string Apellidos { get; set; }
            public string Fecha_Consulta_Pila { get; set; }
            public string Total_Cotizado_en_Meses { get; set; }
            public string Meses_UltimoPeriodo { get; set; }
            public string UltimoPeriodo_fecha_inicio { get; set; }
            public string UltimoPeriodo_fecha_fin { get; set; }
            public string No_Documento { get; set; }
        }
         public DataTable ConvertToDataTable<T>(IList<T> data)
         {
             PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
             DataTable table = new DataTable();
             foreach (PropertyDescriptor prop in properties)
                 table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
             foreach (T item in data)
             {
                 DataRow row = table.NewRow();
                 foreach (PropertyDescriptor prop in properties)
                     row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                 table.Rows.Add(row);
             }
             return table;

         }
    }
}
