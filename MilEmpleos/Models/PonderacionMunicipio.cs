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
    
    public partial class PonderacionMunicipio
    {
        public string IdCodigo { get; set; }
        public double Ponderacion { get; set; }
    
        public virtual municipality municipality { get; set; }
    }
}
