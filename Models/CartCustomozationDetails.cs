using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CartCustomozationDetails
    {
        [Key]
        public int Id { get; set; }
        public int? AvailableCustomozationId { get; set; }
        [ForeignKey("AvailableCustomozationId")]
        public AvailableCustomozation AvailableCustomozation { get; set; }
        public int CartDetailsId { get; set; }
        [ForeignKey("CartDetailsId")]
        public CartDetails CartDetails { get; set; }
       
    }
}
