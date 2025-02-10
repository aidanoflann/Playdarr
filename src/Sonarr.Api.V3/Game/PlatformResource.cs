#nullable enable
using Sonarr.Http.REST;

namespace Sonarr.Api.V3.Game;

public class PlatformResource : RestResource
{
    public string? APIDetailURL { get; set; }
    public int Identifier { get; set; }
    public string? Name { get; set; }
    public string? SiteDetailURL { get; set; }
    public string? Abbreviation { get; set; }
}

public static class PlatformResourceMapper
{
    public static PlatformResource ToResource(this NzbDrone.Core.Games.Platform model, bool includeSeasonImages = false)
    {
        return new PlatformResource
        {
            APIDetailURL = model.APIDetailURL,
            Identifier = model.Identifier,
            Name = model.Name,
            SiteDetailURL = model.SiteDetailURL,
            Abbreviation = model.Abbreviation
        };
    }
}
