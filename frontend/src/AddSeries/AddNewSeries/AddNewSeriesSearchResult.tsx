import React, { useCallback, useState } from 'react';
import { useSelector } from 'react-redux';
import { AddSeries } from 'App/State/AddSeriesAppState';
import HeartRating from 'Components/HeartRating';
import Icon from 'Components/Icon';
import Label from 'Components/Label';
import Link from 'Components/Link/Link';
import MetadataAttribution from 'Components/MetadataAttribution';
import { icons, kinds, sizes } from 'Helpers/Props';
import { Statistics } from 'Series/Series';
import SeriesGenres from 'Series/SeriesGenres';
import SeriesPoster from 'Series/SeriesPoster';
import createDimensionsSelector from 'Store/Selectors/createDimensionsSelector';
import createExistingSeriesSelector from 'Store/Selectors/createExistingSeriesSelector';
import translate from 'Utilities/String/translate';
import AddNewSeriesModal from './AddNewSeriesModal';
import styles from './AddNewSeriesSearchResult.css';

type AddNewSeriesSearchResultProps = AddSeries;

function AddNewSeriesSearchResult({
  tvdbId,
  titleSlug,
  name,
  year,
  siteDetailURL,
  statistics = {} as Statistics,
  folder,
  overview,
  seriesType,
  images,
  deck,
  description,
  platforms,
  gbId,
  gbGuid
}: AddNewSeriesSearchResultProps) {
  const isExistingSeries = useSelector(createExistingSeriesSelector(tvdbId));
  const { isSmallScreen } = useSelector(createDimensionsSelector());
  const [isNewAddSeriesModalOpen, setIsNewAddSeriesModalOpen] = useState(false);

  const seasonCount = statistics.seasonCount;
  const handlePress = useCallback(() => {
    setIsNewAddSeriesModalOpen(true);
  }, []);

  const handleAddSeriesModalClose = useCallback(() => {
    setIsNewAddSeriesModalOpen(false);
  }, []);

  const handleTvdbLinkPress = useCallback((event: React.SyntheticEvent) => {
    event.stopPropagation();
  }, []);

  const linkProps = isExistingSeries
    ? { to: `/series/${titleSlug}` }
    : { onPress: handlePress };
  let seasons = translate('OneSeason');

  if (seasonCount > 1) {
    seasons = translate('CountSeasons', { count: seasonCount });
  }

  return (
    <div className={styles.searchResult}>
      <Link className={styles.underlay} {...linkProps} />

      <div className={styles.overlay}>
        {isSmallScreen ? null : (
          <SeriesPoster
            className={styles.poster}
            images={images}
            size={250}
            overflow={true}
            lazy={false}
          />
        )}

        <div className={styles.content}>
          <div className={styles.titleRow}>
            <div className={styles.titleContainer}>
              <div className={styles.title}>
                {name}
                
                {!name.includes(String(year)) && year ? (
                  <span className={styles.year}>({year})</span>
                ) : null}
              </div>
            </div>

            <div className={styles.icons}>
              {isExistingSeries ? (
                <Icon
                  className={styles.alreadyExistsIcon}
                  name={icons.CHECK_CIRCLE}
                  size={36}
                  title={translate('AlreadyInYourLibrary')}
                />
              ) : null}

              <Link
                className={styles.tvdbLink}
                to={siteDetailURL}
                onPress={handleTvdbLinkPress}
              >
                <Icon
                  className={styles.tvdbLinkIcon}
                  name={icons.EXTERNAL_LINK}
                  size={28}
                />
              </Link>
            </div>
          </div>

          <div>
            {platforms ? (
              platforms.map((platform, index) => (
                <Link
                className={styles.tvdbLink}
                to={platform.siteDetailURL}
                onPress={handleTvdbLinkPress}
                key={index}
                >
                  <Label size={sizes.LARGE}>
                    <Icon name={icons.CIRCLE_OUTLINE} size={13} />

                    <span className={styles.network}>{platform.abbreviation}</span>
                  </Label>
                </Link>
              ))
            ) : null}
          </div>

          <div className={styles.overview}>{deck}</div>

          <MetadataAttribution />
        </div>
      </div>

      <AddNewSeriesModal
        isOpen={isNewAddSeriesModalOpen && !isExistingSeries}
        gbId={gbId}
        gbGuid={gbGuid}
        tvdbId={tvdbId}
        name={name}
        year={year}
        overview={overview}
        folder={folder}
        initialSeriesType={seriesType}
        images={images}
        onModalClose={handleAddSeriesModalClose}
      />
    </div>
  );
}

export default AddNewSeriesSearchResult;
