using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class AircraftVM
    {
       public Aircraft Aircrafts { get; set; }
       public Specifcation Specifcation { get; set; }
       public AvailableCustomozation Availablecustomozation { get; set; }

        public IEnumerable<Specifcation> SpecifcationList { get; set; }
        public IEnumerable<AircrafImages> AircrafImages { get; set; }

    }
}
