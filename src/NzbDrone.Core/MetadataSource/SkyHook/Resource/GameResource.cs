using System.Collections.Generic;

namespace NzbDrone.Core.MetadataSource.SkyHook.Resource
{
    public class GamesResource
    {
        public List<GameResource> Results { get; set; }
    }

    public class GameResource
    {
        public string Name { get; set; }
        public string Original_Release_Date { get; set; }
        public int? Expected_Release_Year { get; set; }
        public string Site_Detail_url { get; set; }
        public List<PlatformResource> Platforms { get; set; }
        public string Deck { get; set; }
        public string Description { get; set; }
        public GameImageResource Image { get; set; }
    }

    public class PlatformResource
    {
        public string API_Detail_url { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Site_Detail_url { get; set; }
        public string Abbreviation { get; set; }
    }

    public class GameImageResource
    {
        public string icon_url { get; set; }
        public string medium_url { get; set; }
        public string screen_url { get; set; }
        public string screen_large_url { get; set; }
        public string small_url { get; set; }
        public string super_url { get; set; }
        public string thumb_url { get; set; }
        public string tiny_url { get; set; }
        public string original_url { get; set; }
    }
}
