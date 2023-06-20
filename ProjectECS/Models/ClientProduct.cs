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
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System.Linq;
     
      
        public class UniqueProductNameAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var clientProduct = (ClientProduct)validationContext.ObjectInstance;
                var dbContext = new ECSWebEntities(); // Replace with your actual DbContext instance

                // Check if the product with the same name already exists for the client
                bool productExists = dbContext.ClientProducts.Any(cp =>
                    cp.ProductName == clientProduct.ProductName && cp.Client_id == clientProduct.Client_id &&
                    cp.ProductID != clientProduct.ProductID); // Exclude the current product from the check

                if (productExists)
                {
                    return new ValidationResult("Product is already added by you.");
                }

                return ValidationResult.Success;
            }
        }


        public partial class ClientProduct
        {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClientProduct()
        {
            this.PreferredServices = new HashSet<PreferredService>();
        }
    
        public int ProductID { get; set; }
        public int Client_id { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [UniqueProductName]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product Service Details are required.")]
        public string ProductServiceDetails { get; set; }
        public int Status { get; set; }
    
        public virtual Client Client { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PreferredService> PreferredServices { get; set; }
    }
}