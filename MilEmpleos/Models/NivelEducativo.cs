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
    
    public partial class NivelEducativo
    {
        public NivelEducativo()
        {
            this.ActNivEdu = new HashSet<ActNivEdu>();
            this.RespuestasCalculadora = new HashSet<RespuestasCalculadora>();
        }
    
        public int id { get; set; }
        public string NivelEducativo1 { get; set; }
    
        public virtual ICollection<ActNivEdu> ActNivEdu { get; set; }
        public virtual ICollection<RespuestasCalculadora> RespuestasCalculadora { get; set; }
    }
}
