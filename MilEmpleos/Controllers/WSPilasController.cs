using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MilEmpleos.Models;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Http.Headers;

namespace IdentitySample.Controllers
{
    [Authorize(Roles = "Admin, Centros, Consultores")]
    public class WSPilasController : ApiController
    {

        private MilEmpleosEntities1 db = new MilEmpleosEntities1();

        //[Route("api/{controller}/{documento}/{id}")]

        public IHttpActionResult GetWSPila(string id)
        {
            var USER_id = User.Identity.GetUserId();
            var existedocumento = (from p in db.Pila
                                   where p.NumeroDocumento == id
                                           select p).ToList();
            if (existedocumento.Count() == 0)
            {
                  Pila PilaNull = new Pila();
                  PilaNull.id = 2147483647;
                  PilaNull.TiempoTotalRegistroMeses=0;
                  PilaNull.PrimerApellido = "No registrado" ;
                  PilaNull.TipoDocumento = "cc";
                  PilaNull.SegundoApellido = "";
                  PilaNull.SegundoNombre = "";
                  PilaNull.PrimerNombre = "";
                  PilaNull.NumeroDocumento = id;
                  return Ok(PilaNull);
            }
            var CE = (from item in db.AspNetUsers
                      where item.Id == USER_id
                      select item.CentroId).First();
            var documentopila = (from p in db.Pila
                        where p.NumeroDocumento.Contains(id)
                        select p).First();
            Pila pila = documentopila;
            //save consulta
            Consulta consulta = new Consulta();
            consulta.DocumentoNumero = id;
            consulta.FechaConsulta = DateTime.Now;
            consulta.TipoDocumento = "cc";
            consulta.UsuerID = USER_id;
            consulta.CentroID = CE;
            db.Consulta.Add(consulta);
            db.SaveChanges();
            //save consulta
            if (pila == null)
            {
                return NotFound();
            }

            // aki   se consumo  al  web  service

            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://localhost/yourwebapi");


            //client.DefaultRequestHeaders.Accept.Add(
            //new MediaTypeWithQualityHeaderValue("application/json"));

            //    HttpResponseMessage response = client.GetAsync("api/yourcustomobjects").Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var yourcustomobjects = response.Content.ReadAsAsync<IEnumerable<WSPila>>().Result;
            //        foreach (var x in yourcustomobjects)
            //        {
            //                     //Call your store method and pass in your own object
            //            //SaveCustomObjectToDB(x);
            //        }
            //    }
            //    else
            //    {
            //        wSPila.NumeroDocumento = "nO HAY CONEXION CON EL SERVIDOR";
            //        return Ok(wSPila);
            //    }

            ///////////////////
            RespuestaPila respuestaPila = new RespuestaPila();
            respuestaPila.Registrado = pila.TiempoTotalRegistroMeses > 0;
            respuestaPila.Nombres = pila.PrimerNombre + " " + pila.SegundoNombre;
            respuestaPila.Apellidos = pila.PrimerApellido + " " + pila.SegundoApellido;
            respuestaPila.FechaRespuestaPila = DateTime.Now;
            respuestaPila.TiempoTotalRegistroMeses = pila.TiempoTotalRegistroMeses.Value;
            respuestaPila.Meses_UltimoPeriodo = pila.Meses_ultimo_periodo_cotizado.Value;
            respuestaPila.UltimoPeriodo_fecha_inicio = pila.ultimo_periodo_cotizado_fecha_inicio.Value;
            respuestaPila.UltimoPeriodo_fecha_fin = pila.ultimo_periodo_cotizado_fecha_fin.Value;
            respuestaPila.TipoDocumento = pila.TipoDocumento;
            respuestaPila.NoDocumento = pila.NumeroDocumento;
            respuestaPila.UserId = USER_id;
            respuestaPila.CentroId = CE;
            respuestaPila.ConsutaId = consulta.id;
            db.RespuestaPila.Add(respuestaPila);
            db.SaveChanges();
            return Ok(pila);
        }

        private bool PilaExists(int id)
        {
            return db.Pila.Count(e => e.id == id) > 0;
        }
    }
}