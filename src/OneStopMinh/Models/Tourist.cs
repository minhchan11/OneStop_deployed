using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OneStopMinh.Models
{
    [Table("Tourists")]
    public class Tourist
    {
        [Key]
        public int TouristId { get; set; }
        public string Name { get; set; }
        public byte[] Pic { get; set; }
        public virtual string UserName { get; set; }
        public virtual ICollection<Attraction> Attractions { get; set; }
    }
}
