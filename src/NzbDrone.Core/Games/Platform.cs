using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Games
{
    public class Platform : IEmbeddedDocument
    {
        public Platform()
        {
        }

        public string APIDetailURL { get; set; }
        public int Identifier { get; set; }
        public string Name { get; set; }
        public string SiteDetailURL { get; set; }
        public string Abbreviation { get; set; }
    }
}
