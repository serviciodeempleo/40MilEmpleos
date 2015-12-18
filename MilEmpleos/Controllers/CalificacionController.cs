using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MilEmpleos.Models;
using System.Web.Http.Cors;
using System.Web.Script.Serialization; 


namespace MilEmpleos.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CalificacionController : ApiController
    {
        private MilEmpleosEntities db = new MilEmpleosEntities();
        public IHttpActionResult Get(string id)
        {
            string str;
            ResultadoCalculadorascollection resultadoCalculadorascollection = new ResultadoCalculadorascollection();
            if (TryStrToGuid(id))
            {
                 Guid value = new Guid(id);
                var existCalificacion = (from r in db.ResultadoCalculadora
                                         where r.id.Equals(value)
                                         select r.id).ToList().Count();
                if (existCalificacion > 0)
                {
                    ResultadoCalculadora Calificacion = (from r in db.ResultadoCalculadora
                                                         where r.id.Equals(value)
                                                         select r).First();
                    resultadoCalculadorascollection.Id = Calificacion.RespuestasCalculadoraId;
                    resultadoCalculadorascollection.FechaCalculo = Calificacion.FechaCalculo;
                    resultadoCalculadorascollection.Total = Calificacion.Total;
                    resultadoCalculadorascollection.CapitalHumano = Calificacion.CapitalHumano;
                    resultadoCalculadorascollection.CaracteristicasEmpresa = Calificacion.CaracteristicasEmpresa;
                    resultadoCalculadorascollection.CaracteristicasPuesto = Calificacion.CaracteristicasPuesto;
                }
                else
                {
                    resultadoCalculadorascollection.Id = 0;
                }
            }
            else
            {
                resultadoCalculadorascollection.Id = 0;
            }
                List<ResultadoCalculadorascollection> serialiseresutado = new List<ResultadoCalculadorascollection>();
                JavaScriptSerializer ser = new JavaScriptSerializer();
                serialiseresutado.Add(resultadoCalculadorascollection);
                str = ser.Serialize(serialiseresutado);
                return Ok(str);
        }
        public static Boolean TryStrToGuid(String s)
        {
            try
            {
                Guid value = new Guid(s);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public partial class ResultadoCalculadorascollection
        {
            public int Id { get; set; }
            public double CapitalHumano { get; set; }
            public double CaracteristicasEmpresa { get; set; }
            public double CaracteristicasPuesto { get; set; }
            public double Total { get; set; }
            public System.DateTime FechaCalculo { get; set; }
        }

    }
}
