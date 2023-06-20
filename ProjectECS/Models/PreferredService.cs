//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectECS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
     

    public partial class PreferredService
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PreferredService()
        {
            this.Payments = new HashSet<Payment>();
        }
    
        public int PreferredServiceID { get; set; }  
        public int Client_id { get; set; } 
        public int ProductID { get; set; } 
        public int ServicePreferred { get; set; } 
        public int Charges_id { get; set; } 
        public int ServiceDays { get; set; }
        public string Status { get; set; } 
        public System.DateTime StartDate { get; set; } 
        public System.DateTime EndDate { get; set; }
        
        public virtual Charge Charge { get; set; }
        public virtual ClientProduct ClientProduct { get; set; }

        
        public virtual Client Client { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual Service Service { get; set; }
    }
}