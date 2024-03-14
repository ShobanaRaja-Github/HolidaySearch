using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HolidaySearch.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HolidaySearch.Tests
{
    [TestClass]
    public class HolidaySearchTests
    {
        private List<Flight> flights;
        private List<Hotel> hotels;

        private IHolidaySearch holidaySearch;

        [TestInitialize]
        public async Task Setup()
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            // Added relative for local execution
            var flightsJson = await File.ReadAllTextAsync(@"../../../Metadata/flights.json");
            flights = JsonSerializer.Deserialize<List<Flight>>(flightsJson, jsonOptions);

            // Added relative for local execution
            var hotelsJson = await File.ReadAllTextAsync(@"../../../Metadata/hotels.json");
            hotels = JsonSerializer.Deserialize<List<Hotel>>(hotelsJson, jsonOptions);

            this.holidaySearch = new HolidaySearch(flights, hotels);
        }

        [TestMethod]
        public void MAN_To_AGP_Jul_7Nights_Should_Return_Correct_Result()
        {
            // Arrange
            var expectedFlight = flights[1]; // Flight 2
            var expectedHotel = hotels[8]; // Hotel 9

            // Act
            var results = this.holidaySearch.Search("MAN", "AGP", DateTime.Parse("2023-07-01"), 7);

            // Assert
            AssertResult(expectedFlight, expectedHotel, results);
        }

        [TestMethod]
        public void AnyAirport_To_PMI_Jun_10Nights_Should_Return_Correct_Result()
        {
            // Arrange
            var expectedFlight = flights[5]; // Flight 6
            var expectedHotel = hotels[4]; // Hotel 5

            // Act
            var results = this.holidaySearch.Search(null, "PMI", DateTime.Parse("2023-06-15"), 10);

            // Assert
            AssertResult(expectedFlight, expectedHotel, results);
        }

        [TestMethod]
        public void AnyAirport_To_LPA_Nov_14Nights_Should_Return_Correct_Result()
        {
            // Arrange.
            var expectedFlight = flights[6]; // Flight 7
            var expectedHotel = hotels[5]; // Hotel 6

            // Act.
            var results = holidaySearch.Search(null, "LPA", DateTime.Parse("2022-11-10"), 14);

            // Assert.
            AssertResult(expectedFlight, expectedHotel, results);
        }

        private static void AssertResult(Flight expectedFlight, Hotel expectedHotel, List<SearchResponse> results)
        {
            var bestPrice = expectedFlight.Price + expectedHotel.PricePerNight * expectedHotel.Nights;
            
            //Comparing only Fist result to check Best match.
            Assert.IsNotNull(results);
            Assert.AreEqual(bestPrice, results.First().Price);
            Assert.AreEqual(expectedFlight.Id, results.First().Flight.Id);
            Assert.AreEqual(expectedFlight.From, results.First().Flight.From);
            Assert.AreEqual(expectedFlight.To, results.First().Flight.To);
            Assert.AreEqual(expectedFlight.Price, results.First().Flight.Price);
            Assert.AreEqual(expectedHotel.Id, results.First().Hotel.Id);
            Assert.AreEqual(expectedHotel.Name, results.First().Hotel.Name);
            Assert.AreEqual(expectedHotel.PricePerNight, results.First().Hotel.PricePerNight);
        }
    }
}
