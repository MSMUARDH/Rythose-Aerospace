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
    public class ShippingDetail
    {
       
        [Key]
        public int ShippingID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public City City { get; set; }
        public string PostCode { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
        public ShippingDetail()
        {
            this.Orders = new HashSet<Orders>();
        }

    }
}
