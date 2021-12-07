using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WeddingPlanner.Valiidations;

namespace WeddingPlanner.Models
{
    public class Event
    {
        [Required]
        public int EventId { get; set; }
        [Required]
        [Display(Name ="Wedder One:")]
        public string WedderOne { get; set; }
        [Required]
        public string Weddertwo { get; set; }
        [Required(ErrorMessage ="Wedding date is required")]
        [Future]
        public DateTime Date { get; set; }
        [Required]
        public string Address { get; set; }

        //the one to many relationship with user
        public int UserId { get; set; }
        public User Creator { get; set; }
        //End one to many relationship with User

        //Many to Many
        public List<RSVp> GuestList { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
