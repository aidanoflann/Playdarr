using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.MediaCover;
using NzbDrone.Core.MetadataSource;
using NzbDrone.Core.Organizer;
using NzbDrone.Core.SeriesStats;
using Sonarr.Api.V5.Game;
using Sonarr.Http;

namespace Sonarr.Api.V5.Series;

[V5ApiController("series/lookup")]
public class SeriesLookupController : Controller
{
    private readonly ISearchForNewSeries _searchProxy;
    private readonly IBuildFileNames _fileNameBuilder;
    private readonly IMapCoversToLocal _coverMapper;

    public SeriesLookupController(ISearchForNewSeries searchProxy, IBuildFileNames fileNameBuilder, IMapCoversToLocal coverMapper)
    {
        _searchProxy = searchProxy;
        _fileNameBuilder = fileNameBuilder;
        _coverMapper = coverMapper;
    }

    [HttpGet]
    public IEnumerable<GameResource> Search([FromQuery] string term)
    {
        var tvDbResults = _searchProxy.SearchForNewGame(term);
        return MapToResource(tvDbResults);
    }

    private IEnumerable<GameResource> MapToResource(IEnumerable<NzbDrone.Core.Games.Game> games)
    {
        foreach (var currentGame in games)
        {
            var resource = currentGame.ToResource();
            if (currentGame.Platforms != null && resource.Platforms != null)
            {
                foreach (var platform in currentGame.Platforms)
                {
                    resource.Platforms.Add(platform.ToResource());
                }
            }

            _coverMapper.ConvertToLocalUrls(resource.Id, resource.Images);

            var poster = currentGame.Images.FirstOrDefault(c => c.CoverType == MediaCoverTypes.Poster);

            if (poster != null)
            {
                resource.RemotePoster = poster.RemoteUrl;
            }

            resource.Folder = _fileNameBuilder.GetGameFolder(currentGame);
            resource.Statistics = new SeriesStatistics().ToResource(resource.Seasons);

            yield return resource;
        }
    }
}
