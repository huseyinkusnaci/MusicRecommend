using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicRecommend.Models
{
    
    public class AnalysModel
    {
        public int ArtistId { get; set; }
        public int UserCount { get; set; }
        public int[] Users { get; set; }
    }

     
    public class UserSelectModel
    {
        public int UserId { get; set; }
        public int[] FirstFiveArtist { get; set; }

        public List<AnalysModel> SuggestArtistList { get; set; }
    }

    public class TrainingDataModel
    {
        public int UserId { get; set; }
        public int ArtistId { get; set; }
        public string Liked { get; set; }
    }
}