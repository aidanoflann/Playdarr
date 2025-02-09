using System.Collections.Generic;
using System.Linq;
using NLog;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Tv.Events;

namespace NzbDrone.Core.Games
{
    public interface IGameService
    {
        List<Game> AddGames(List<Game> newGames);
        Game AddGame(Game newGame);
        List<int> AllGamesGbIds();
    }

    public class GameService : IGameService
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        public GameService(IGamesRepository gamesRepository,
                             IEventAggregator eventAggregator,
                             Logger logger)
        {
            _gamesRepository = gamesRepository;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public List<Game> AddGames(List<Game> newGames)
        {
            _gamesRepository.InsertMany(newGames);

            // TODO: game event here
            _eventAggregator.PublishEvent(new SeriesImportedEvent(newGames.Select(s => s.Id).ToList()));

            return newGames;
        }

        public Game AddGame(Game newGame)
        {
            _gamesRepository.Insert(newGame);

            // TODO: game event here
            // _eventAggregator.PublishEvent(new SeriesAddedEvent(GetSeries(newSeries.Id)));
            return newGame;
        }

        public List<int> AllGamesGbIds()
        {
            return new List<int>();
        }
    }
}
