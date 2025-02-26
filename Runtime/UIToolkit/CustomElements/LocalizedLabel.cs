#if USE_LOCALIZATION_PACKAGE
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace UI.Core.UIToolkit.CustomElements
{
    [UxmlElement]
    public partial class LocalizedLabel : Label, ILocalizable
    {
        private LocalizedString _localization;

        ~LocalizedLabel()
        {
            LocalizationSettings.SelectedLocaleChanged -= LocalizationSettingsOnSelectedLocaleChanged;
        }

        private void LocalizationSettingsOnSelectedLocaleChanged(Locale locale)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            if (_localization == null)
                return;

            text = _localization.GetLocalizedString();
        }

        public void InitLocalization(LocalizedString localization)
        {
            if (localization == null)
                return;
            
            _localization = localization;
            LocalizationSettings.SelectedLocaleChanged += LocalizationSettingsOnSelectedLocaleChanged;
            
            UpdateText();
        }
    }
}
#endif