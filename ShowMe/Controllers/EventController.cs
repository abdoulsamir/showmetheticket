using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using GogoKit;
using GogoKit.Exceptions;
using GogoKit.Models.Request;
using GogoKit.Models.Response;
using ShowMe.Models;
using X.PagedList;

namespace ShowMe.Controllers
{
    public class EventController : Controller
    {
        private readonly ViagogoClient _api;
        public EventController()
        {
            const string clientId = "TaRJnBcw1ZvYOXENCtj5";
            const string clientSecret = "ixGDUqRA5coOHf3FQysjd704BPptwbk6zZreELW2aCYSmIT8XJ9ngvN1MuKV";
            _api = new ViagogoClient(new ProductHeaderValue("ShowMeApp"), clientId, clientSecret);
        }

        [Route("events")]
        public async Task<ActionResult> Index(string searchString, DateTime? searchDate, int? categoryId = null, int? page = 1)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return View(new EventsViewModel());
            }

            var eventsCategoryId = categoryId;
            if (eventsCategoryId == null)
            {
                var searchResults = await _api.Search.GetAllAsync(searchString);
                if (searchResults.Count < 1)
                {
                    return View(new EventsViewModel());
                }

                eventsCategoryId = GetIdFromLink(searchResults[0].CategoryLink.HRef);
            }

            Category category;
            try
            {
                category = await _api.Categories.GetAsync(eventsCategoryId.Value);
            }
            catch (ResourceNotFoundException)
            {
                return HttpNotFound();
            }

            var eventsResult = await _api.Events.GetByCategoryAsync(category.Id.Value,
                new EventRequest { Page = page, MinStartDate = searchDate, MaxStartDate = searchDate?.AddDays(1) });
            var cheapestEventsPerCountry = eventsResult.Items
                .Where(x => x.Id.HasValue && x.MinTicketPrice?.Amount != null)
                .GroupBy(x => x.Venue.Country.Code)
                .Where(x=>x.Count() > 1)
                .Select(x => x.OrderBy(ev => ev.MinTicketPrice?.Amount).First().Id.Value)
                .ToList();

            var eventsViewModel = new EventsViewModel
            {
                SearchString = searchString,
                SearchDate = searchDate,
                CategoryId = eventsCategoryId,
                CheapestEventsPerCountry = cheapestEventsPerCountry,
                Events = new StaticPagedList<Event>(eventsResult.Items, eventsResult.Page ?? 1, eventsResult.PageSize.Value, eventsResult.TotalItems.Value)
            };

            return View(eventsViewModel);
        }

        [Route("events/{eventId}")]
        public async Task<ActionResult> Details(int eventId, int? quantity = null, int? page = 1)
        {
            Event eventDetails;
            try
            {
                eventDetails = await _api.Events.GetAsync(eventId);
            }
            catch (ResourceNotFoundException)
            {
                return HttpNotFound();
            }

            var listingsResult = await _api.Listings.GetByEventAsync(eventDetails.Id.Value, new ListingRequest { Page = page, NumberOfTickets = quantity });

            var vm = new EventDetailsViewModel
            {
                Details = eventDetails,
                Quantity = quantity,
                Listings = new StaticPagedList<Listing>(listingsResult.Items, listingsResult.Page ?? 1, listingsResult.PageSize.Value, listingsResult.TotalItems.Value)
            };
            return View(vm);
        }

        private int GetIdFromLink(string link)
        {
            //This is a work around as they API does not actually provide the ID as per documentation.
            var idText = link.Substring(link.LastIndexOf('/') + 1);
            int id;
            int.TryParse(idText, out id);
            return id;
        }
    }
}
