using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class OrderCustomozationDetails
    {
        [Key]
        public int Id { get; set; }
        public int? AvailableCustomozationId { get; set; }
        [ForeignKey("AvailableCustomozationId")]
        public AvailableCustomozation AvailableCustomozation { get; set; }
        public int OrderDetailsId { get; set; }
        [ForeignKey("OrderDetailsId")]
        public OrderDetails OrderDetails { get; set; }

    }
}
