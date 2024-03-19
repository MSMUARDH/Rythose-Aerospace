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
    public class ShippingSetVM
    {
       public ShippingSet ShippingSet { get; set; }
       public IEnumerable<SelectListItem> CityList { get; set; }

    }
}
