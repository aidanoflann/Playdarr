using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using NLog;
using NzbDrone.Common.Cloud;
using NzbDrone.Common.Extensions;
using NzbDrone.Common.Http;
using NzbDrone.Core.DataAugmentation.DailySeries;
using NzbDrone.Core.Exceptions;
using NzbDrone.Core.Games;
using NzbDrone.Core.Languages;
using NzbDrone.Core.MediaCover;
using NzbDrone.Core.MetadataSource.SkyHook.Resource;
using NzbDrone.Core.Parser;
using NzbDrone.Core.Tv;

namespace NzbDrone.Core.MetadataSource.SkyHook
{
    public class SkyHookProxy : IProvideSeriesInfo, ISearchForNewSeries
    {
        private readonly IHttpClient _httpClient;
        private readonly Logger _logger;
        private readonly ISeriesService _seriesService;
        private readonly IDailySeriesService _dailySeriesService;
        private readonly IHttpRequestBuilderFactory _requestBuilder;

        public SkyHookProxy(IHttpClient httpClient,
                            ISonarrCloudRequestBuilder requestBuilder,
                            ISeriesService seriesService,
                            IDailySeriesService dailySeriesService,
                            Logger logger)
        {
            _httpClient = httpClient;
            _requestBuilder = requestBuilder.GiantBombdb;
            _logger = logger;
            _seriesService = seriesService;
            _dailySeriesService = dailySeriesService;
            _requestBuilder = requestBuilder.GiantBombdb;
        }

        public Tuple<Series, List<Episode>> GetSeriesInfo(int tvdbSeriesId)
        {
            var httpRequest = _requestBuilder.Create()
                                             .SetSegment("route", "shows")
                                             .Resource(tvdbSeriesId.ToString())
                                             .Build();

            httpRequest.AllowAutoRedirect = true;
            httpRequest.SuppressHttpError = true;

            var httpResponse = _httpClient.Get<ShowResource>(httpRequest);

            if (httpResponse.HasHttpError)
            {
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new SeriesNotFoundException(tvdbSeriesId);
                }
                else
                {
                    throw new HttpException(httpRequest, httpResponse);
                }
            }

            var episodes = httpResponse.Resource.Episodes.Select(MapEpisode);
            var series = MapSeries(httpResponse.Resource);

            return new Tuple<Series, List<Episode>>(series, episodes.ToList());
        }

        public List<Series> SearchForNewSeriesByImdbId(string imdbId)
        {
            imdbId = Parser.Parser.NormalizeImdbId(imdbId);

            if (imdbId == null)
            {
                return new List<Series>();
            }

            var results = SearchForNewSeries($"imdb:{imdbId}");

            return results;
        }

        public List<Series> SearchForNewSeriesByAniListId(int aniListId)
        {
            var results = SearchForNewSeries($"anilist:{aniListId}");

            return results;
        }

        public List<Series> SearchForNewSeriesByMyAnimeListId(int malId)
        {
            var results = SearchForNewSeries($"mal:{malId}");

            return results;
        }

        public List<Series> SearchForNewSeriesByTmdbId(int tmdbId)
        {
            var results = SearchForNewSeries($"tmdb:{tmdbId}");

            return results;
        }

        public List<Series> SearchForNewSeries(string title)
        {
            try
            {
                var lowerTitle = title.ToLowerInvariant();

                if (lowerTitle.StartsWith("tvdb:") || lowerTitle.StartsWith("tvdbid:"))
                {
                    var slug = lowerTitle.Split(':')[1].Trim();

                    if (slug.IsNullOrWhiteSpace() || slug.Any(char.IsWhiteSpace) || !int.TryParse(slug, out var tvdbId) || tvdbId <= 0)
                    {
                        return new List<Series>();
                    }

                    try
                    {
                        var existingSeries = _seriesService.FindByTvdbId(tvdbId);
                        if (existingSeries != null)
                        {
                            return new List<Series> { existingSeries };
                        }

                        return new List<Series> { GetSeriesInfo(tvdbId).Item1 };
                    }
                    catch (SeriesNotFoundException)
                    {
                        return new List<Series>();
                    }
                }

                var httpRequest = _requestBuilder.Create()
                                                 .SetSegment("route", "search")
                                                 .AddQueryParam("term", title.ToLower().Trim())
                                                 .Build();

                var httpResponse = _httpClient.Get<List<ShowResource>>(httpRequest);

                return httpResponse.Resource.SelectList(MapSearchResult);
            }
            catch (HttpException ex)
            {
                _logger.Warn(ex);
                throw new SkyHookException("Search for '{0}' failed. Unable to communicate with SkyHook. {1}", ex, title, ex.Message);
            }
            catch (WebException ex)
            {
                _logger.Warn(ex);
                throw new SkyHookException("Search for '{0}' failed. Unable to communicate with SkyHook. {1}", ex, title, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                throw new SkyHookException("Search for '{0}' failed. Invalid response received from SkyHook. {1}", ex, title, ex.Message);
            }
        }

        public List<Game> SearchForNewGame(string title)
        {
            try
            {
                var lowerTitle = title.ToLowerInvariant();

                if (lowerTitle.StartsWith("tvdb:") || lowerTitle.StartsWith("tvdbid:"))
                {
                    var slug = lowerTitle.Split(':')[1].Trim();

                    if (slug.IsNullOrWhiteSpace() || slug.Any(char.IsWhiteSpace) || !int.TryParse(slug, out var tvdbId) || tvdbId <= 0)
                    {
                        return new List<Game>();
                    }

                    // try
                    // {
                    //     var existingSeries = _seriesService.FindByTvdbId(tvdbId);
                    //     if (existingSeries != null)
                    //     {
                    //         return new List<Game> { existingSeries };
                    //     }

                    // return new List<Game> { GetSeriesInfo(tvdbId).Item1 };
                    // }
                    // catch (SeriesNotFoundException)
                    // {
                    //     return new List<Series>();
                    // }
                }

                var httpRequest = _requestBuilder.Create()
                                                .SetSegment("route", "games")
                                                .AddQueryParam("filter", $"name:{title.ToLower().Trim()}")
                                                .Build();
                var httpResponse = _httpClient.Get<GamesResource>(httpRequest);

                return httpResponse.Resource.Results.SelectList(MapGame);
            }
            catch (HttpException ex)
            {
                _logger.Warn(ex);
                throw new SkyHookException("Search for '{0}' failed. Unable to communicate with SkyHook. {1}", ex, title, ex.Message);
            }
            catch (WebException ex)
            {
                _logger.Warn(ex);
                throw new SkyHookException("Search for '{0}' failed. Unable to communicate with SkyHook. {1}", ex, title, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                throw new SkyHookException("Search for '{0}' failed. Invalid response received from SkyHook. {1}", ex, title, ex.Message);
            }
        }

        private Series MapSearchResult(ShowResource show)
        {
            var series = _seriesService.FindByTvdbId(show.TvdbId);

            if (series == null)
            {
                series = MapSeries(show);
            }

            return series;
        }

        private Series MapSeries(ShowResource show)
        {
            var series = new Series();
            series.TvdbId = show.TvdbId;

            if (show.TvRageId.HasValue)
            {
                series.TvRageId = show.TvRageId.Value;
            }

            if (show.TvMazeId.HasValue)
            {
                series.TvMazeId = show.TvMazeId.Value;
            }

            if (show.TmdbId.HasValue)
            {
                series.TmdbId = show.TmdbId.Value;
            }

            series.ImdbId = show.ImdbId;
            series.MalIds = show.MalIds;
            series.AniListIds = show.AniListIds;
            series.Title = show.Title;
            series.CleanTitle = Parser.Parser.CleanSeriesTitle(show.Title);
            series.SortTitle = SeriesTitleNormalizer.Normalize(show.Title, show.TvdbId);

            series.OriginalLanguage = show.OriginalLanguage.IsNotNullOrWhiteSpace() ?
                IsoLanguages.Find(show.OriginalLanguage.ToLower())?.Language ?? Language.English :
                Language.English;

            if (show.FirstAired != null)
            {
                series.FirstAired = DateTime.ParseExact(show.FirstAired, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                series.Year = series.FirstAired.Value.Year;
            }

            if (show.LastAired != null)
            {
                series.LastAired = DateTime.ParseExact(show.LastAired, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }

            series.Overview = show.Overview;

            if (show.Runtime != null)
            {
                series.Runtime = show.Runtime.Value;
            }

            series.Network = show.Network;

            if (show.TimeOfDay != null)
            {
                series.AirTime = string.Format("{0:00}:{1:00}", show.TimeOfDay.Hours, show.TimeOfDay.Minutes);
            }

            series.TitleSlug = show.Slug;
            series.Status = MapSeriesStatus(show.Status);
            series.Ratings = MapRatings(show.Rating);
            series.Genres = show.Genres;

            if (show.ContentRating.IsNotNullOrWhiteSpace())
            {
                series.Certification = show.ContentRating.ToUpper();
            }

            if (_dailySeriesService.IsDailySeries(series.TvdbId))
            {
                series.SeriesType = SeriesTypes.Daily;
            }

            series.Actors = show.Actors.Select(MapActors).ToList();
            series.Seasons = show.Seasons.Select(MapSeason).ToList();
            series.Images = show.Images.Select(MapImage).ToList();
            series.Monitored = true;

            return series;
        }

        private Game MapGame(GameResource gameResource)
        {
            var game = new Game();

            // game.TvdbId = showResource.TvdbId;

            // if (showResource.TvRageId.HasValue)
            // {
            //     game.TvRageId = showResource.TvRageId.Value;
            // }

            // if (showResource.TvMazeId.HasValue)
            // {
            //     game.TvMazeId = showResource.TvMazeId.Value;
            // }

            // if (showResource.TmdbId.HasValue)
            // {
            //     game.TmdbId = showResource.TmdbId.Value;
            // }

            // game.ImdbId = showResource.ImdbId;
            // game.MalIds = showResource.MalIds;
            // game.AniListIds = showResource.AniListIds;
            game.Name = gameResource.Name;
            game.GbId = gameResource.Id;
            game.GbGuid = gameResource.guid;
            game.SiteDetailURL = gameResource.Site_Detail_url;
            if (gameResource.Platforms != null)
            {
                game.Platforms = gameResource.Platforms.Select(MapPlatforms).ToList();
            }

            game.Deck = gameResource.Deck;
            game.Description = gameResource.Description;

            // game.CleanTitle = Parser.Parser.CleanSeriesTitle(showResource.Title);
            // game.SortTitle = SeriesTitleNormalizer.Normalize(showResource.Title, showResource.TvdbId);

            // game.OriginalLanguage = showResource.OriginalLanguage.IsNotNullOrWhiteSpace() ?
            //     IsoLanguages.Find(showResource.OriginalLanguage.ToLower())?.Language ?? Language.English :
            //     Language.English;

            if (gameResource.Original_Release_Date != null)
            {
                game.OriginalReleaseDate = DateTime.ParseExact(gameResource.Original_Release_Date, "yyyy-mm-dd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                game.Year = game.OriginalReleaseDate.Value.Year;
            }
            else if (gameResource.Expected_Release_Year.HasValue)
            {
                game.Year = gameResource.Expected_Release_Year.Value;
            }
            else
            {
                game.Year = 9999;
            }

            // if (showResource.LastAired != null)
            // {
            //     game.LastAired = DateTime.ParseExact(showResource.LastAired, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            // }

            // game.Overview = showResource.Overview;

            // if (showResource.Runtime != null)
            // {
            //     game.Runtime = showResource.Runtime.Value;
            // }

            // game.Network = showResource.Network;

            // if (showResource.TimeOfDay != null)
            // {
            //     game.AirTime = string.Format("{0:00}:{1:00}", showResource.TimeOfDay.Hours, showResource.TimeOfDay.Minutes);
            // }

            // game.TitleSlug = showResource.Slug;
            // game.Status = MapSeriesStatus(showResource.Status);
            // game.Ratings = MapRatings(showResource.Rating);
            // game.Genres = showResource.Genres;

            // if (showResource.ContentRating.IsNotNullOrWhiteSpace())
            // {
            //     game.Certification = showResource.ContentRating.ToUpper();
            // }

            // if (_dailySeriesService.IsDailySeries(game.TvdbId))
            // {
            //     game.SeriesType = SeriesTypes.Daily;
            // }

            // game.Images = showResource.Images.Select(MapImage).ToList();
            // game.Monitored = true;

            // game.Actors = showResource.Actors.Select(MapActors).ToList();
            // game.Seasons = showResource.Seasons.Select(MapSeason).ToList();
            game.Images = MapGameImages(gameResource.Image);

            return game;
        }

        private static Actor MapActors(ActorResource arg)
        {
            var newActor = new Actor
            {
                Name = arg.Name,
                Character = arg.Character
            };

            if (arg.Image != null)
            {
                newActor.Images = new List<MediaCover.MediaCover>
                {
                    new MediaCover.MediaCover(MediaCoverTypes.Headshot, arg.Image)
                };
            }

            return newActor;
        }

        private static Episode MapEpisode(EpisodeResource oracleEpisode)
        {
            var episode = new Episode();
            episode.TvdbId = oracleEpisode.TvdbId;
            episode.Overview = oracleEpisode.Overview;
            episode.SeasonNumber = oracleEpisode.SeasonNumber;
            episode.EpisodeNumber = oracleEpisode.EpisodeNumber;
            episode.AbsoluteEpisodeNumber = oracleEpisode.AbsoluteEpisodeNumber;
            episode.Title = oracleEpisode.Title;
            episode.AiredAfterSeasonNumber = oracleEpisode.AiredAfterSeasonNumber;
            episode.AiredBeforeSeasonNumber = oracleEpisode.AiredBeforeSeasonNumber;
            episode.AiredBeforeEpisodeNumber = oracleEpisode.AiredBeforeEpisodeNumber;

            episode.AirDate = oracleEpisode.AirDate;
            episode.AirDateUtc = oracleEpisode.AirDateUtc;
            episode.Runtime = oracleEpisode.Runtime;
            episode.FinaleType = oracleEpisode.FinaleType;

            episode.Ratings = MapRatings(oracleEpisode.Rating);

            // Don't include series fanart images as episode screenshot
            if (oracleEpisode.Image != null)
            {
                episode.Images.Add(new MediaCover.MediaCover(MediaCoverTypes.Screenshot, oracleEpisode.Image));
            }

            return episode;
        }

        private static Season MapSeason(SeasonResource seasonResource)
        {
            return new Season
            {
                SeasonNumber = seasonResource.SeasonNumber,
                Images = seasonResource.Images.Select(MapImage).ToList(),
                Monitored = seasonResource.SeasonNumber > 0
            };
        }

        private static SeriesStatusType MapSeriesStatus(string status)
        {
            if (status.Equals("ended", StringComparison.InvariantCultureIgnoreCase))
            {
                return SeriesStatusType.Ended;
            }

            if (status.Equals("upcoming", StringComparison.InvariantCultureIgnoreCase))
            {
                return SeriesStatusType.Upcoming;
            }

            return SeriesStatusType.Continuing;
        }

        private static Platform MapPlatforms(PlatformResource platformResource)
        {
            if (platformResource == null)
            {
                return new Platform();
            }

            return new Platform
            {
                APIDetailURL = platformResource.API_Detail_url,
                Identifier = platformResource.ID,
                Name = platformResource.Name,
                SiteDetailURL = platformResource.Site_Detail_url,
                Abbreviation = platformResource.Abbreviation
            };
        }

        private static Ratings MapRatings(RatingResource rating)
        {
            if (rating == null)
            {
                return new Ratings();
            }

            return new Ratings
            {
                Votes = rating.Count,
                Value = rating.Value
            };
        }

        private static List<MediaCover.MediaCover> MapGameImages(GameImageResource gameImageResource)
        {
            return
            [
                new MediaCover.MediaCover(MediaCoverTypes.Poster, gameImageResource.small_url)
            ];
        }

        private static MediaCover.MediaCover MapImage(ImageResource arg)
        {
            return new MediaCover.MediaCover
            {
                RemoteUrl = arg.Url,
                CoverType = MapCoverType(arg.CoverType)
            };
        }

        private static MediaCoverTypes MapCoverType(string coverType)
        {
            switch (coverType.ToLower())
            {
                case "poster":
                    return MediaCoverTypes.Poster;
                case "banner":
                    return MediaCoverTypes.Banner;
                case "fanart":
                    return MediaCoverTypes.Fanart;
                case "clearlogo":
                    return MediaCoverTypes.Clearlogo;
                default:
                    return MediaCoverTypes.Unknown;
            }
        }
    }
}
