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
    
    public partial class ActividadNivEduHab
    {
        public int id { get; set; }
        public int ActividadId { get; set; }
        public int HabilidadId { get; set; }
        public double ValorHabilidad { get; set; }
    
        public virtual ActividadesPuestoDeTrabajo ActividadesPuestoDeTrabajo { get; set; }
        public virtual Habilidad Habilidad { get; set; }
    }
}
