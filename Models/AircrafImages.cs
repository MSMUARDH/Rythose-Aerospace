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
    public class AircrafImages
    {
        [Key]
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public int AircratfId { get; set; }
        [ForeignKey("AircratfId")]
        public Aircraft Aircraft { get; set; }

    }
}
