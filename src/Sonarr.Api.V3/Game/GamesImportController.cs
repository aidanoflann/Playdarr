using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.Games;
using Sonarr.Http;

namespace Sonarr.Api.V3.Game
{
    [V3ApiController("games/import")]
    public class GameImportController : Controller
    {
        private readonly IAddGameService _addGameService;

        public GameImportController(IAddGameService addGameService)
        {
            _addGameService = addGameService;
        }

        [HttpPost]
        public object Import([FromBody] List<GameResource> resource)
        {
            // TODO: handle a GameResource instead
            var newGames = resource.ToModel();
            return _addGameService.AddGames(newGames).ToResource();
        }
    }
}
