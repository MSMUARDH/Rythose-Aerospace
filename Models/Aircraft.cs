using Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Models
{
    public class Aircraft
    {
        [Key]
        public int AircratfId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public AircraftCat Category { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }
        public double Price { get; set; }
        public double Qty { get; set; }
        public string CurStatus { get; set; } = SD.Active;
    }
}
