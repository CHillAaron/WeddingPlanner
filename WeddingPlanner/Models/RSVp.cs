using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WeddingPlanner.Models
{
    public class RSVp
    {
        [Key]
        public int RsvpId { get; set; }
        //foreign Key
        public int UserId { get; set; }
        public int EventId { get; set; }

        //Nav Props

        public User Guest { get; set; }
        public Event Attending { get; set; }

    }
}
