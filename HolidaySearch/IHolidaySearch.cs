using HolidaySearch.Models;
using System;
using System.Collections.Generic;

namespace HolidaySearch
{
    public interface IHolidaySearch
    {
        List<SearchResponse> Search(
            string departingFrom,
            string travelingTo,
            DateTime departureDate,
            int duration);
    }
}
