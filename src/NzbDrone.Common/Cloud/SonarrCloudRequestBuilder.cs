using NzbDrone.Common.Http;

namespace NzbDrone.Common.Cloud
{
    public interface ISonarrCloudRequestBuilder
    {
        IHttpRequestBuilderFactory Services { get; }
        IHttpRequestBuilderFactory SkyHookTvdb { get; }

        IHttpRequestBuilderFactory GiantBombdb { get; }
    }

    public class SonarrCloudRequestBuilder : ISonarrCloudRequestBuilder
    {
        public SonarrCloudRequestBuilder()
        {
            Services = new HttpRequestBuilder("https://services.sonarr.tv/v1/")
                .CreateFactory();

            SkyHookTvdb = new HttpRequestBuilder("https://skyhook.sonarr.tv/v1/tvdb/{route}/{language}/")
                .SetSegment("language", "en")
                .CreateFactory();

            // TODO: non-hardcoded API keys
            // going to try GIantBomb as our meta provider, seems gud and isn't amazon
            GiantBombdb = new HttpRequestBuilder("https://www.giantbomb.com/api/{route}/")
                .AddQueryParam("api_key", "a990c44ba7d847499c090647f7d85da94b7626ca")
                .AddQueryParam("format", "json")
                .CreateFactory();

            // See https://github.com/Radarr/Radarr/commit/0715962ec5a7a9bb4ed3a9d8b06788fc03f6186e for an example of where this was done for movies
        }

        public IHttpRequestBuilderFactory Services { get; }

        public IHttpRequestBuilderFactory SkyHookTvdb { get; }

        public IHttpRequestBuilderFactory GiantBombdb { get; }
    }
}
