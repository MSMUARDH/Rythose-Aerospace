using Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }
        public string PaymentType { get; set; }
        public DateTime PaymentOn { get; set; }
        public string PaymentBy { get; set; }

        public int OrdersID { get; set; }
        [ForeignKey("OrdersID")]
        public Orders Orders { get; set; }
    }
}
