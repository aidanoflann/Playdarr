using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;
using NzbDrone.Core.Languages;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(1)]
    public class InitialSetup : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.TableForModel("Config")
                  .WithColumn("Key").AsString().Unique()
                  .WithColumn("Value").AsString();

            Create.TableForModel("RootFolders")
                  .WithColumn("Path").AsString().Unique();

            Create.TableForModel("Series")
                .WithColumn("TvdbId").AsInt32().Unique()
                .WithColumn("TvRageId").AsInt32().Unique()
                .WithColumn("ImdbId").AsString().Unique()
                .WithColumn("Title").AsString()
                .WithColumn("TitleSlug").AsString().Unique()
                .WithColumn("CleanTitle").AsString()
                .WithColumn("Status").AsInt32()
                .WithColumn("Overview").AsString().Nullable()
                .WithColumn("AirTime").AsString().Nullable()
                .WithColumn("Images").AsString()
                .WithColumn("Path").AsString()
                .WithColumn("Monitored").AsBoolean()
                .WithColumn("QualityProfileId").AsInt32()
                .WithColumn("SeasonFolder").AsBoolean()
                .WithColumn("LastInfoSync").AsDateTime().Nullable()
                .WithColumn("LastDiskSync").AsDateTime().Nullable()
                .WithColumn("Runtime").AsInt32()
                .WithColumn("SeriesType").AsInt32()
                .WithColumn("BacklogSetting").AsInt32()
                .WithColumn("Network").AsString().Nullable()
                .WithColumn("CustomStartDate").AsDateTime().Nullable()
                .WithColumn("UseSceneNumbering").AsBoolean()
                .WithColumn("FirstAired").AsDateTime().Nullable()
                .WithColumn("NextAiring").AsDateTime().Nullable();
            Create.TableForModel("Games")
                .WithColumn("TvdbId").AsInt32().Nullable()
                .WithColumn("TvRageId").AsInt32().Nullable()
                .WithColumn("TvMazeId").AsInt32().Nullable()
                .WithColumn("ImdbId").AsString().Nullable()
                .WithColumn("Name").AsString()
                .WithColumn("TitleSlug").AsString().Nullable()
                .WithColumn("CleanTitle").AsString().Nullable()
                .WithColumn("Status").AsInt32().Nullable()
                .WithColumn("Overview").AsString().Nullable()
                .WithColumn("AirTime").AsString().Nullable()
                .WithColumn("Images").AsString()
                .WithColumn("Path").AsString()
                .WithColumn("Monitored").AsBoolean()
                .WithColumn("QualityProfileId").AsInt32().Nullable()
                .WithColumn("SeasonFolder").AsBoolean().Nullable()
                .WithColumn("LastInfoSync").AsDateTime().Nullable()
                .WithColumn("LastDiskSync").AsDateTime().Nullable()
                .WithColumn("Runtime").AsInt32().Nullable()
                .WithColumn("SeriesType").AsInt32().Nullable()
                .WithColumn("Network").AsString().Nullable().Nullable()
                .WithColumn("UseSceneNumbering").AsBoolean().Nullable()
                .WithColumn("FirstAired").AsDateTime().Nullable()
                .WithColumn("NextAiring").AsDateTime().Nullable()
                .WithColumn("SiteDetailURL").AsString().Nullable()
                .WithColumn("Platforms").AsString()
                .WithColumn("Deck").AsString().Nullable()
                .WithColumn("Description").AsString().Nullable()
                .WithColumn("GbId").AsInt32().Unique()
                .WithColumn("GbGuid").AsString().Unique()
                .WithColumn("OriginalReleaseDate").AsDateTime().Unique();

            // immediately do all the migrations done for Series
            Create.Column("SortTitle").OnTable("Games").AsString().Nullable();
            Alter.Table("Games").AddColumn("Year").AsInt32().Nullable();
            Alter.Table("Games").AddColumn("Seasons").AsString().Nullable();
            Alter.Table("Games").AddColumn("Actors").AsString().Nullable();
            Alter.Table("Games").AddColumn("Ratings").AsString().Nullable();
            Alter.Table("Games").AddColumn("Genres").AsString().Nullable();
            Alter.Table("Games").AddColumn("Certification").AsString().Nullable();
            Alter.Table("Games").AddColumn("ProfileId").AsInt32().Nullable();
            Alter.Table("Games").AddColumn("Tags").AsString().Nullable();
            Alter.Table("Games").AddColumn("Added").AsDateTime().Nullable();
            Alter.Table("Games").AddColumn("AddOptions").AsString().Nullable();
            Alter.Table("Games").AddColumn("OriginalLanguage").AsInt32().WithDefaultValue((int)Language.English);
            Alter.Table("Games").AddColumn("LastAired").AsDateTimeOffset().Nullable();
            Alter.Table("Games").AddColumn("MonitorNewItems").AsInt32().WithDefaultValue(0);
            Alter.Table("Games").AddColumn("TmdbId").AsInt32().WithDefaultValue(0);

            Alter.Table("Games").AddColumn("MalIds").AsString().WithDefaultValue("[]");
            Alter.Table("Games").AddColumn("AniListIds").AsString().WithDefaultValue("[]");

            Create.TableForModel("Seasons")
                .WithColumn("SeriesId").AsInt32()
                .WithColumn("SeasonNumber").AsInt32()
                .WithColumn("Ignored").AsBoolean();

            Create.TableForModel("Episodes")
                .WithColumn("TvDbEpisodeId").AsInt32().Unique()
                .WithColumn("SeriesId").AsInt32()
                .WithColumn("SeasonNumber").AsInt32()
                .WithColumn("EpisodeNumber").AsInt32()
                .WithColumn("Title").AsString().Nullable()
                .WithColumn("Overview").AsString().Nullable()
                .WithColumn("Ignored").AsBoolean().Nullable()
                .WithColumn("EpisodeFileId").AsInt32().Nullable()
                .WithColumn("AirDate").AsDateTime().Nullable()
                .WithColumn("AbsoluteEpisodeNumber").AsInt32().Nullable()
                .WithColumn("SceneAbsoluteEpisodeNumber").AsInt32().Nullable()
                .WithColumn("SceneSeasonNumber").AsInt32().Nullable()
                .WithColumn("SceneEpisodeNumber").AsInt32().Nullable();

            Create.TableForModel("EpisodeFiles")
                  .WithColumn("SeriesId").AsInt32()
                  .WithColumn("Path").AsString().Unique()
                  .WithColumn("Quality").AsString()
                  .WithColumn("Size").AsInt64()
                  .WithColumn("DateAdded").AsDateTime()
                  .WithColumn("SeasonNumber").AsInt32()
                  .WithColumn("SceneName").AsString().Nullable()
                  .WithColumn("ReleaseGroup").AsString().Nullable();

            Create.TableForModel("History")
                  .WithColumn("EpisodeId").AsInt32()
                  .WithColumn("SeriesId").AsInt32()
                  .WithColumn("NzbTitle").AsString()
                  .WithColumn("Date").AsDateTime()
                  .WithColumn("Quality").AsString()
                  .WithColumn("Indexer").AsString()
                  .WithColumn("NzbInfoUrl").AsString().Nullable()
                  .WithColumn("ReleaseGroup").AsString().Nullable();

            Create.TableForModel("Notifications")
                  .WithColumn("Name").AsString()
                  .WithColumn("OnGrab").AsBoolean()
                  .WithColumn("OnDownload").AsBoolean()
                  .WithColumn("Settings").AsString()
                  .WithColumn("Implementation").AsString();

            Create.TableForModel("ScheduledTasks")
                  .WithColumn("TypeName").AsString().Unique()
                  .WithColumn("Interval").AsInt32()
                  .WithColumn("LastExecution").AsDateTime();

            Create.TableForModel("Indexers")
                  .WithColumn("Enable").AsBoolean()
                  .WithColumn("Name").AsString().Unique()
                  .WithColumn("Implementation").AsString()
                  .WithColumn("Settings").AsString().Nullable();

            Create.TableForModel("QualityProfiles")
                  .WithColumn("Name").AsString().Unique()
                  .WithColumn("Cutoff").AsInt32()
                  .WithColumn("Allowed").AsString();

            Create.TableForModel("QualitySizes")
                  .WithColumn("QualityId").AsInt32().Unique()
                  .WithColumn("Name").AsString().Unique()
                  .WithColumn("MinSize").AsInt32()
                  .WithColumn("MaxSize").AsInt32();

            Create.TableForModel("SceneMappings")
                  .WithColumn("CleanTitle").AsString()
                  .WithColumn("SceneName").AsString()
                  .WithColumn("TvdbId").AsInt32()
                  .WithColumn("SeasonNumber").AsInt32();

            Create.TableForModel("NamingConfig")
                  .WithColumn("UseSceneName").AsBoolean()
                  .WithColumn("Separator").AsString()
                  .WithColumn("NumberStyle").AsInt32()
                  .WithColumn("IncludeSeriesTitle").AsBoolean()
                  .WithColumn("MultiEpisodeStyle").AsInt32()
                  .WithColumn("IncludeEpisodeTitle").AsBoolean()
                  .WithColumn("IncludeQuality").AsBoolean()
                  .WithColumn("ReplaceSpaces").AsBoolean()
                  .WithColumn("SeasonFolderFormat").AsString();
        }

        protected override void LogDbUpgrade()
        {
            Create.TableForModel("Logs")
                  .WithColumn("Message").AsString()
                  .WithColumn("Time").AsDateTime()
                  .WithColumn("Logger").AsString()
                  .WithColumn("Method").AsString().Nullable()
                  .WithColumn("Exception").AsString().Nullable()
                  .WithColumn("ExceptionType").AsString().Nullable()
                  .WithColumn("Level").AsString();
        }
    }
}
