using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Models
{
    /// <summary>
    /// Model class for Order
    /// holds properties:
    /// int OrderId
    /// decimal Total
    /// DateTime OrderDate
    /// DateTime ReturnDate
    /// string UserOrderId
    /// int ShippingDetailsId
    /// int FineRefId
    /// int PaymentRefId
    /// int OrderStatus
    /// </summary>
    public class Order
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        [Display(Name = "Order Total")]
        [DataType(DataType.Currency)]
        public decimal Total { get; set; }

        [Required]
        [Display(Name = "Loan Date")]
        public DateTime OrderDate { get; set; }
        [Required]
        [Display(Name = "Return Date")]
        public DateTime ReturnDate { get; set; }

        [Required]
        public string UserOrderId { get; set; }

        [Required]
        public int ShippingDetailsId { get; set; }

        public int FineRefId { get; set; }

        public int PaymentRefID { get; set; }

        public int? OrderStatus { get; set; }

        //navigation properties
        [ForeignKey("ShippingDetailsId")]
        public virtual ShippingDetails ShippingDetails { get; set; }

        [ForeignKey("UserOrderId")]
        public virtual ApplicationUser User { get; set; }


        public virtual ICollection<OrderLine> OrderLine { get; set; }

      
        public virtual Fine fine { get; set; }

        [Required]
        [ForeignKey("PaymentRefID")]
        public virtual Payment Payment { get; set; }



    }
}