using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationNeo4j
{
    public class Definitions
    {

    }

    public class FactAvgRating
    {
        public String AvgRating { get; set; }
    }

    public class MovieDim
    {
        public int MovieSK { get; set; }
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string ImdbId { get; set; }
        public string TmdbId { get; set; }
    }

    public class FactTable
    {
        public int MovieSK { get; set; }
        public int UserSK { get; set; }
        public int TimeSK { get; set; }
        public float Rating { get; set; }
    }

    public class UserDim
    {
        public int UserSK { get; set; }
    }

    public class TagDim
    {
        public int TagSK { get; set; }
        public int TagId { get; set; }
        public string TagName { get; set; }
    }

    public class GenreDim
    {
        public int GenreSK { get; set; }
        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }

    public class DateDim
    {
        public int DateSK { get; set; }
        public string DayOfWeek { get; set; }
        public int Month { get; set; }
        public int DayOfMonth { get; set; }
        public string Time { get; set; }
        public int Year { get; set; }
    }
}