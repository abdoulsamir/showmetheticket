using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GogoKit;
using GogoKit.Models.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace ShowMe.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private const string ClientId = "TaRJnBcw1ZvYOXENCtj5";
        private const string ClientSecret = "ixGDUqRA5coOHf3FQysjd704BPptwbk6zZreELW2aCYSmIT8XJ9ngvN1MuKV";

        [TestMethod]
        public async Task SearchForArtistEvents()
        {
            var api = new ViagogoClient(new ProductHeaderValue("ShowMeApp"), ClientId, ClientSecret);
            //var root = await api.Hypermedia.GetRootAsync();
            var searchResults = await api.Search.GetAllAsync("john legend");

            searchResults.ShouldNotBeEmpty();

            searchResults[0].Type.ShouldBe("Category");

            var categoryId = GetIdFromLink(searchResults[0].CategoryLink.HRef);
            categoryId.ShouldBeGreaterThan(0);
            var category = await api.Categories.GetAsync(categoryId);

            var events = await api.Events.GetAllByCategoryAsync(category.Id.Value);
            events.ShouldNotBeEmpty();
        }

        [TestMethod]
        public async Task GetEventDetails()
        {
            var api = new ViagogoClient(new ProductHeaderValue("ShowMeApp"), ClientId, ClientSecret);
            var searchResults = await api.Search.GetAllAsync("john legend");
            var categoryId = GetIdFromLink(searchResults[0].CategoryLink.HRef);
            var events = await api.Events.GetAllByCategoryAsync(categoryId);
            events.ShouldNotBeEmpty();

            var eventDetails = await api.Events.GetAsync(events[0].Id.Value);
            eventDetails.Id.ShouldNotBeNull();

            var listings = await api.Listings.GetAllByEventAsync(eventDetails.Id.Value);
            listings.Count.ShouldBeGreaterThanOrEqualTo(0);
        }


        private int GetIdFromLink(string link)
        {
            var idText = link.Substring(link.LastIndexOf('/') + 1);
            int id;
            int.TryParse(idText, out id);
            return id;
        }
    }
}
