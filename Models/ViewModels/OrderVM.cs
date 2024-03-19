using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class OrderVM
    {
       public IEnumerable< Aircraft> AircraftList { get; set; }
       public IEnumerable<OrderDetails> OrderDetailList { get; set; }
       public IEnumerable<Orders> OrderList { get; set; }
       public Aircraft Aircrafts { get; set; }
       public Orders Orders{ get; set; }
       public OrderDetails OrderDetails { get; set; }
       public ApplicationUser User { get; set; }
     
        public IEnumerable<AvailableCustomozation> AvailableCustomozationList { get; set; }
        public ShippingDetail ShippingDetail { get; set; }
      
    }
}
