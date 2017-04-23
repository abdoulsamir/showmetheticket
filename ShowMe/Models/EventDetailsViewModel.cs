using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GogoKit.Models.Response;
using X.PagedList;

namespace ShowMe.Models
{
    public class EventDetailsViewModel
    {
        public EventDetailsViewModel()
        {
            Listings = new StaticPagedList<Listing>(new List<Listing>(), 1, 1, 0);
        }

        public Event Details { get; set; }
        public StaticPagedList<Listing> Listings { get; set; }
        public int? Quantity { get; set; }

        public bool HasSearch => Quantity.GetValueOrDefault() > 0;
    }
}