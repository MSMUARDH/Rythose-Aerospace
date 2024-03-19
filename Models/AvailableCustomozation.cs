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
    public class AvailableCustomozation
    {
        [Key]
        public int Id { get; set; }
        public string CustomzationType { get; set; }
        public string CustomzationValue { get; set; }
        public double Price { get; set; }

        public int AircratfId { get; set; }
        [ForeignKey("AircratfId")]
        public Aircraft Aircraft { get; set; }

    }
}
