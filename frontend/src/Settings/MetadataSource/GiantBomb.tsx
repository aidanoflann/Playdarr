import React from 'react';
import InlineMarkdown from 'Components/Markdown/InlineMarkdown';
import translate from 'Utilities/String/translate';
import styles from './GiantBomb.css';

function GiantBomb() {
  return (
    <div className={styles.container}>
      <img
        className={styles.image}
        src={`${window.Sonarr.urlBase}/Content/Images/giant-bomb-seeklogo.png`}
      />

      <div className={styles.info}>
        <div className={styles.title}>{translate('MetaDataProviderNameExplicit')}</div>
        <InlineMarkdown
          data={translate('SeriesAndEpisodeInformationIsProvidedByProvider', {
            url: 'https://www.giantbomb.com/get-premium/',
          })}
        />
      </div>
    </div>
  );
}

export default GiantBomb;
