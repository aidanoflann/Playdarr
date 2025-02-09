using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NLog;
using NzbDrone.Common.EnsureThat;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.MetadataSource;
using NzbDrone.Core.Organizer;

// using System.IO;
// using FluentValidation.Results;
// using NzbDrone.Core.Exceptions;
// using NzbDrone.Core.Parser;

namespace NzbDrone.Core.Games
{
    public interface IAddGameService
    {
        Game AddGame(Game newGame);
        List<Game> AddGames(List<Game> newGames, bool ignoreErrors = false);
    }

    public class AddGameService : IAddGameService
    {
        private readonly IGameService _gameService;
        private readonly IProvideSeriesInfo _seriesInfo;
        private readonly IBuildFileNames _fileNameBuilder;
        private readonly IAddGameValidator _addGameValidator;
        private readonly Logger _logger;

        public AddGameService(IGameService gameService,
                                IProvideSeriesInfo seriesInfo,
                                IBuildFileNames fileNameBuilder,
                                IAddGameValidator addSeriesValidator,
                                Logger logger)
        {
            _gameService = gameService;
            _seriesInfo = seriesInfo;
            _fileNameBuilder = fileNameBuilder;
            _addGameValidator = addSeriesValidator;
            _logger = logger;
        }

        public Game AddGame(Game newGame)
        {
            Ensure.That(newGame, () => newGame).IsNotNull();

            // newGame = AddSkyhookData(newGame);
            // newGame = SetPropertiesAndValidate(newGame);

            _logger.Info("Adding Series {0} Path: [{1}]", newGame, newGame.Path);
            _gameService.AddGame(newGame);

            return newGame;
        }

        public List<Game> AddGames(List<Game> newGames, bool ignoreErrors = false)
        {
            var added = DateTime.UtcNow;
            var gamesToAdd = new List<Game>();
            var existingGameGbIds = _gameService.AllGamesGbIds();

            foreach (var g in newGames)
            {
                if (g.Path.IsNullOrWhiteSpace())
                {
                    // TODO: make RootFolderPath get populated on the game search request (I disabled it because lazy)
                    _logger.Info("Adding Game {0} Root Folder Path: [{1}]", g, g.RootFolderPath);
                }
                else
                {
                    _logger.Info("Adding Game {0} Path: [{1}]", g, g.Path);
                }

                try
                {
                    // var series = AddSkyhookData(s);
                    var game = g;

                    // series = SetPropertiesAndValidate(series);
                    game.Added = added;
                    if (existingGameGbIds.Any(f => f == game.GbId))
                    {
                        _logger.Debug("GiantBomb ID {0} was not added due to validation failure: Game {1} already exists in database", g.GbId, g);
                        continue;
                    }

                    if (gamesToAdd.Any(f => f.GbId == game.GbId))
                    {
                        _logger.Trace("GiantBomb ID {0} was already added from another import list, not adding series {1} again", g.GbId, g);
                        continue;
                    }

                    // TODO: see if there's an equivalent to doing this check for games
                    // var duplicateSlug = gamesToAdd.FirstOrDefault(f => f.TitleSlug == game.TitleSlug);
                    // if (duplicateSlug != null)
                    // {
                    //     _logger.Debug("TVDB ID {0} was not added due to validation failure: Duplicate Slug {1} used by series {2}", g.TvdbId, g.TitleSlug, duplicateSlug.TvdbId);
                    //     continue;
                    // }

                    gamesToAdd.Add(game);
                }
                catch (ValidationException ex)
                {
                    if (!ignoreErrors)
                    {
                        throw;
                    }

                    _logger.Debug("Game {0} with GiantBomb ID {1} was not added due to validation failures. {2}", g, g.GbId, ex.Message);
                }
            }

            return _gameService.AddGames(gamesToAdd);
        }

        // private Series AddSkyhookData(Series newSeries)
        // {
        //     Tuple<Series, List<Episode>> tuple;
        //     try
        //     {
        //         tuple = _seriesInfo.GetSeriesInfo(newSeries.TvdbId);
        //     }
        //     catch (SeriesNotFoundException)
        //     {
        //         _logger.Error("Series {0} with TVDB ID {1} was not found, it may have been removed from TheTVDB. Path: {2}", newSeries, newSeries.TvdbId, newSeries.Path);
        //         throw new ValidationException(new List<ValidationFailure>
        //                                       {
        //                                           new ValidationFailure("TvdbId", $"A series with this ID was not found. Path: {newSeries.Path}", newSeries.TvdbId)
        //                                       });
        //     }
        //     var series = tuple.Item1;
        //     // If seasons were passed in on the new series use them, otherwise use the seasons from Skyhook
        //     newSeries.Seasons = newSeries.Seasons != null && newSeries.Seasons.Any() ? newSeries.Seasons : series.Seasons;
        //     series.ApplyChanges(newSeries);
        //     return series;
        // }

        // private Series SetPropertiesAndValidate(Series newSeries)
        // {
        //     if (string.IsNullOrWhiteSpace(newSeries.Path))
        //     {
        //         var folderName = _fileNameBuilder.GetSeriesFolder(newSeries);
        //         newSeries.Path = Path.Combine(newSeries.RootFolderPath, folderName);
        //     }
        //     newSeries.CleanTitle = newSeries.Title.CleanSeriesTitle();
        //     newSeries.SortTitle = SeriesTitleNormalizer.Normalize(newSeries.Title, newSeries.TvdbId);
        //     newSeries.Added = DateTime.UtcNow;
        //     if (newSeries.AddOptions != null && newSeries.AddOptions.Monitor == MonitorTypes.None)
        //     {
        //         newSeries.Monitored = false;
        //     }
        //     var validationResult = _addGameValidator.Validate(newSeries);
        //     if (!validationResult.IsValid)
        //     {
        //         throw new ValidationException(validationResult.Errors);
        //     }
        //     return newSeries;
        // }
    }
}
