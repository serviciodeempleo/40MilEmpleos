//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
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
