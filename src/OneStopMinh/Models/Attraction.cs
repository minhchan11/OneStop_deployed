using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OneStopMinh.Models
{
    [Table("Attractions")]
    public class Attraction
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Tourist Tourist { get; set; }
    }
}
