//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MilEmpleos.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ponderacion
    {
        public int id { get; set; }
        public int PreguntaId { get; set; }
        public int RespuestaId { get; set; }
        public double PorcentageRespuesta { get; set; }
    }
}
