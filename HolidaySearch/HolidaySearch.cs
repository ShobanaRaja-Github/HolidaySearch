using HolidaySearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HolidaySearch
{
    public class HolidaySearch: IHolidaySearch
    {
        private readonly List<Flight> flights;
        private readonly List<Hotel> hotels;

        // Injecting Flight and Hotel details as input so this search will be stand along for small task.
        public HolidaySearch(List<Flight> flights, List<Hotel> hotels)
        {
            this.flights = flights;
            this.hotels = hotels;
        }

        public List<SearchResponse> Search(
            string departingFrom,
            string travelingTo,
            DateTime departureDate,
            int duration)
        {
            var results = new List<SearchResponse>();

            var filteredFlights = FilterFlights(departingFrom, travelingTo, departureDate);
            var filteredHotels = FilterHotels(travelingTo, departureDate, duration);

            foreach (var flight in filteredFlights)
            {
                foreach (var hotel in filteredHotels)
                {
                    results.Add(new SearchResponse
                    {
                        Flight = flight,
                        Hotel = hotel,
                        Price = flight.Price + hotel.PricePerNight * hotel.Nights // Calculate Total Price.
                    });
                }
            }

            return results.OrderBy(result => result.Price).ToList();  // Order by Best Price.
        }

        // Filter Flights using input filters.
        private List<Flight> FilterFlights(string departingFrom, string travelingTo, DateTime departureDate)
        {
            //The departingFrom null condition in lambda expression will return all records if it is null.
            return this.flights.Where(flight =>
                (departingFrom == null || flight.From == departingFrom) &&
                flight.To == travelingTo &&
                flight.DepartureDate.Date == departureDate.Date
            ).ToList();
        }

        // Filter hotels using input filters.
        private List<Hotel> FilterHotels(string travelingTo, DateTime departureDate, int duration)
        {
            // Arrival date can be greater than departureDate,  Hence Greater than or equal to.
            return this.hotels.Where(hotel =>
                hotel.LocalAirports.Contains(travelingTo) &&
                hotel.ArrivalDate.Date >= departureDate.Date &&
                hotel.Nights == duration
            ).ToList();
        }
    }
}
