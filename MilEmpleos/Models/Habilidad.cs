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
    
    public partial class Habilidad
    {
        public Habilidad()
        {
            this.ActividadNivEduHab = new HashSet<ActividadNivEduHab>();
            this.RespuestasCalculadora = new HashSet<RespuestasCalculadora>();
        }
    
        public int id { get; set; }
        public string HabilidadRequerida { get; set; }
    
        public virtual ICollection<ActividadNivEduHab> ActividadNivEduHab { get; set; }
        public virtual ICollection<RespuestasCalculadora> RespuestasCalculadora { get; set; }
    }
}
