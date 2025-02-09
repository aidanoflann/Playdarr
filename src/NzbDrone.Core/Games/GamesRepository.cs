using System.Collections.Generic;
using System.Linq;
using Dapper;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;

namespace NzbDrone.Core.Games
{
    public interface IGamesRepository : IBasicRepository<Game>
    {
        bool SeriesPathExists(string path);
        Game FindByTitle(string cleanTitle);
        Game FindByTitle(string cleanTitle, int year);
        List<Game> FindByTitleInexact(string cleanTitle);
        Game FindByTvdbId(int tvdbId);
        Game FindByTvRageId(int tvRageId);
        Game FindByImdbId(string imdbId);
        Game FindByPath(string path);
        List<int> AllSeriesTvdbIds();
        Dictionary<int, string> AllSeriesPaths();
        Dictionary<int, List<int>> AllSeriesTags();
    }

    public class GamesRepository : BasicRepository<Game>, IGamesRepository
    {
        public GamesRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public bool SeriesPathExists(string path)
        {
            return Query(c => c.Path == path).Any();
        }

        public Game FindByTitle(string cleanTitle)
        {
            cleanTitle = cleanTitle.ToLowerInvariant();

            var series = Query(s => s.CleanTitle == cleanTitle)
                                        .ToList();

            return ReturnSingleSeriesOrThrow(series);
        }

        public Game FindByTitle(string cleanTitle, int year)
        {
            cleanTitle = cleanTitle.ToLowerInvariant();

            var series = Query(s => s.CleanTitle == cleanTitle && s.Year == year).ToList();

            return ReturnSingleSeriesOrThrow(series);
        }

        public List<Game> FindByTitleInexact(string cleanTitle)
        {
            var builder = Builder().Where($"instr(@cleanTitle, \"Series\".\"CleanTitle\")", new { cleanTitle = cleanTitle });

            if (_database.DatabaseType == DatabaseType.PostgreSQL)
            {
                builder = Builder().Where($"(strpos(@cleanTitle, \"Series\".\"CleanTitle\") > 0)", new { cleanTitle = cleanTitle });
            }

            return Query(builder).ToList();
        }

        public Game FindByTvdbId(int tvdbId)
        {
            return Query(s => s.TvdbId == tvdbId).SingleOrDefault();
        }

        public Game FindByTvRageId(int tvRageId)
        {
            return Query(s => s.TvRageId == tvRageId).SingleOrDefault();
        }

        public Game FindByImdbId(string imdbId)
        {
            return Query(s => s.ImdbId == imdbId).SingleOrDefault();
        }

        public Game FindByPath(string path)
        {
            return Query(s => s.Path == path)
                        .FirstOrDefault();
        }

        public List<int> AllSeriesTvdbIds()
        {
            using (var conn = _database.OpenConnection())
            {
                return conn.Query<int>("SELECT \"TvdbId\" FROM \"Series\"").ToList();
            }
        }

        public Dictionary<int, string> AllSeriesPaths()
        {
            using (var conn = _database.OpenConnection())
            {
                var strSql = "SELECT \"Id\" AS Key, \"Path\" AS Value FROM \"Series\"";
                return conn.Query<KeyValuePair<int, string>>(strSql).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public Dictionary<int, List<int>> AllSeriesTags()
        {
            using (var conn = _database.OpenConnection())
            {
                var strSql = "SELECT \"Id\" AS Key, \"Tags\" AS Value FROM \"Series\" WHERE \"Tags\" IS NOT NULL";
                return conn.Query<KeyValuePair<int, List<int>>>(strSql).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        private Game ReturnSingleSeriesOrThrow(List<Game> games)
        {
            if (games.Count == 0)
            {
                return null;
            }

            if (games.Count == 1)
            {
                return games.First();
            }

            throw new MultipleGamesFoundException(games, "Expected one series, but found {0}. Matching series: {1}", games.Count, string.Join(", ", games));
        }
    }
}
