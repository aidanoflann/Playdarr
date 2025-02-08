import React from 'react';
import PageContent from 'Components/Page/PageContent';
import PageContentBody from 'Components/Page/PageContentBody';
import SettingsToolbar from 'Settings/SettingsToolbar';
import translate from 'Utilities/String/translate';
import GiantBomb from './GiantBomb';

function MetadataSourceSettings() {
  return (
    <PageContent title={translate('MetadataSourceSettings')}>
      <SettingsToolbar showSave={false} />

      <PageContentBody>
        <GiantBomb />
      </PageContentBody>
    </PageContent>
  );
}

export default MetadataSourceSettings;
