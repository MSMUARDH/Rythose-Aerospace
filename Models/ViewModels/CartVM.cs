using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class CartVM
    {
       public IEnumerable< Aircraft> AircraftList { get; set; }
       public IEnumerable<CartDetails> CartDetailList { get; set; }
       //public IEnumerable<Comments> Comments { get; set; }
       public Aircraft Aircrafts { get; set; }
       public Carts Carts{ get; set; }
       public CartDetailsVM CartDetails { get; set; }
       public IEnumerable <AvailableCustomozation> Availablecustomozation { get; set; }
       public IEnumerable<Specifcation> Specifcation { get; set; }
       public ShippingDetail ShippingDetail { get; set; }
       public Orders Order { get; set; }

        public IEnumerable<Orders> Orders { get; set; }
       public IEnumerable<ShippingDetail> ShippingDetailList { get; set; }
        public string CustName { get; set; }
        public string HasKey { get; set; }
        public IEnumerable<AircrafImages> AircrafImages { get; set; }
        public IEnumerable<SelectListItem> CityList { get; set; }


    }
    public class CartDetailsVM
    {
        public int AircraftId { get; set; }
        public int CartId { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public int ColorId { get; set; }
        public int SeatId { get; set; }
       
    }
}
