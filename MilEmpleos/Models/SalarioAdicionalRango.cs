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
    
    public partial class SalarioAdicionalRango
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SalarioAdicionalRango()
        {
            this.RespuestasCalculadora = new HashSet<RespuestasCalculadora>();
        }
    
        public int id { get; set; }
        public string SalarioAdicionalRango1 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RespuestasCalculadora> RespuestasCalculadora { get; set; }
    }
}
