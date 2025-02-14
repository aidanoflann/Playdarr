#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Games;
using NzbDrone.Core.Languages;
using NzbDrone.Core.MediaCover;
using NzbDrone.Core.Tv;
using Sonarr.Http.REST;

namespace Sonarr.Api.V3.Game;

public class GameResource : RestResource
{
    public string? Name { get; set; }

    // public List<AlternateTitleResource>? AlternateTitles { get; set; }
    public string? SortTitle { get; set; }
    public SeriesStatusType Status { get; set; }
    public bool Ended => true;
    public string? ProfileName { get; set; }
    public string? Overview { get; set; }
    public DateTime? NextAiring { get; set; }
    public DateTime? PreviousAiring { get; set; }
    public string? Network { get; set; }
    public string? AirTime { get; set; }
    public List<MediaCover>? Images { get; set; }
    public Language? OriginalLanguage { get; set; }
    public string? RemotePoster { get; set; }

    // public List<SeasonResource>? Seasons { get; set; }
    public int Year { get; set; }
    public string? Path { get; set; }
    public int QualityProfileId { get; set; }
    public bool SeasonFolder { get; set; }
    public bool Monitored { get; set; }
    public NewItemMonitorTypes MonitorNewItems { get; set; }
    public bool UseSceneNumbering { get; set; }
    public int Runtime { get; set; }
    public int TvdbId { get; set; }
    public int TvRageId { get; set; }
    public int TvMazeId { get; set; }
    public int TmdbId { get; set; }
    public DateTime? OriginalReleaseDate { get; set; }
    public DateTime? LastAired { get; set; }
    public SeriesTypes SeriesType { get; set; }
    public string? CleanTitle { get; set; }
    public string? ImdbId { get; set; }
    public string? TitleSlug { get; set; }
    public string? RootFolderPath { get; set; }
    public string? Folder { get; set; }
    public string? Certification { get; set; }
    public List<string>? Genres { get; set; }
    public HashSet<int>? Tags { get; set; }
    public DateTime Added { get; set; }
    public AddSeriesOptions? AddOptions { get; set; }
    public Ratings? Ratings { get; set; }

    // public SeriesStatisticsResource? Statistics { get; set; }
    public bool? EpisodesChanged { get; set; }
    public string? SiteDetailURL { get; set; }
    public List<Platform>? Platforms { get; set; }
    public string? Deck { get; set; }
    public string? Description { get; set; }
    public int GbId { get; set; }
    public string? GbGuid { get; set; }
}

public class AlternateTitleResource
{
}

public static class GameResourceMapper
{
    public static GameResource ToResource(this NzbDrone.Core.Games.Game model, bool includeSeasonImages = false)
    {
        return new GameResource
        {
            Id = model.Id,
            Name = model.Name,
            SortTitle = model.SortTitle,
            Status = model.Status,
            Overview = model.Overview,
            Network = model.Network,
            AirTime = model.AirTime,
            Images = model.Images.JsonClone(),
            Year = model.Year,
            OriginalLanguage = model.OriginalLanguage,
            Path = model.Path,
            QualityProfileId = model.QualityProfileId,
            SeasonFolder = model.SeasonFolder,
            Monitored = model.Monitored,
            MonitorNewItems = model.MonitorNewItems,
            UseSceneNumbering = model.UseSceneNumbering,
            Runtime = model.Runtime,
            TvdbId = model.TvdbId,
            TvRageId = model.TvRageId,
            TvMazeId = model.TvMazeId,
            TmdbId = model.TmdbId,
            OriginalReleaseDate = model.OriginalReleaseDate,
            LastAired = model.LastAired,
            SeriesType = model.SeriesType,
            CleanTitle = model.CleanTitle,
            ImdbId = model.ImdbId,
            TitleSlug = model.TitleSlug,
            Certification = model.Certification,
            Genres = model.Genres,
            Tags = model.Tags,
            Added = model.Added,
            AddOptions = model.AddOptions,
            Ratings = model.Ratings,
            SiteDetailURL = model.SiteDetailURL,
            Platforms = model.Platforms.JsonClone(),
            Deck = model.Deck,
            Description = model.Description,
            GbId = model.GbId,
            GbGuid = model.GbGuid,
        };
    }

    public static NzbDrone.Core.Games.Game ToModel(this GameResource resource)
    {
        return new NzbDrone.Core.Games.Game
        {
            Id = resource.Id,
            Name = resource.Name,
            SortTitle = resource.SortTitle,
            Status = resource.Status,
            Overview = resource.Overview,
            Network = resource.Network,
            AirTime = resource.AirTime,
            Images = resource.Images.JsonClone(),
            Seasons = new List<Season>(),
            Year = resource.Year,
            OriginalLanguage = resource.OriginalLanguage,
            Path = resource.Path,
            QualityProfileId = resource.QualityProfileId,
            SeasonFolder = resource.SeasonFolder,
            Monitored = resource.Monitored,
            MonitorNewItems = resource.MonitorNewItems,
            UseSceneNumbering = resource.UseSceneNumbering,
            Runtime = resource.Runtime,
            TvdbId = resource.TvdbId,
            TvRageId = resource.TvRageId,
            TvMazeId = resource.TvMazeId,
            TmdbId = resource.TmdbId,
            OriginalReleaseDate = resource.OriginalReleaseDate,
            LastAired = resource.LastAired,
            SeriesType = resource.SeriesType,
            CleanTitle = resource.CleanTitle,
            ImdbId = resource.ImdbId,
            TitleSlug = resource.TitleSlug,
            Certification = resource.Certification,
            Genres = resource.Genres,
            Tags = resource.Tags,
            Added = resource.Added,
            AddOptions = resource.AddOptions,
            Ratings = resource.Ratings,
            SiteDetailURL = resource.SiteDetailURL,
            Platforms = resource.Platforms.JsonClone(),
            Deck = resource.Deck,
            Description = resource.Description,
            GbId = resource.GbId,
            GbGuid = resource.GbGuid,
        };
    }

    public static List<NzbDrone.Core.Games.Game> ToModel(this IEnumerable<GameResource> resources)
    {
        return resources.Select(ToModel).ToList();
    }

    public static List<GameResource> ToResource(this IEnumerable<NzbDrone.Core.Games.Game> games,
        bool includeSeasonImages = false)
    {
        return games.Select(s => ToResource(s, includeSeasonImages)).ToList();
    }
}
