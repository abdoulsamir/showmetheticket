using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GogoKit.Models.Response;
using X.PagedList;

namespace ShowMe.Models
{
    public class EventsViewModel
    {
        public EventsViewModel()
        {
            CheapestEventsPerCountry = new List<int>();
            Events = new StaticPagedList<Event>(new List<Event>(), 1, 1, 0);
        }

        public StaticPagedList<Event> Events { get; set; }

        public List<int> CheapestEventsPerCountry { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? SearchDate { get; set; }

        public int? CategoryId { get; set; }

        public string SearchString { get; set; }

        public bool HasSearch => !string.IsNullOrEmpty(SearchString);
    }
}