using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.Tv;
using Sonarr.Http;

namespace Sonarr.Api.V3.Series
{
    [V3ApiController("series/import")]
    public class SeriesImportController : Controller
    {
        private readonly IAddSeriesService _addSeriesService;

        public SeriesImportController(IAddSeriesService addSeriesService)
        {
            _addSeriesService = addSeriesService;
        }

        // [HttpPost]
        // public object Import([FromBody] List<Sonarr.Api.V5.SeriesResource> resource)
        // {
        //     // TODO: move this to V5
        //     // TODO: handle a GameResource instead
        //     var newGames = resource.ToModel();
        //     return (_addSeriesService as AddSeriesService).AddGames(newGames).ToResource();
        // }
    }
}
