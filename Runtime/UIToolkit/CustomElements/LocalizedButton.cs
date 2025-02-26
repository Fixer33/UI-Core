#if USE_LOCALIZATION_PACKAGE
using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace UI.Core.UIToolkit.CustomElements 
{
    [UxmlElement]
    public partial class LocalizedButton : Button, ILocalizable
    {
        private LocalizedString _localization;
        
        public LocalizedButton() : base(new Background())
        {
        }
        
        public LocalizedButton(Background iconImage, Action clickEvent = null) : base(iconImage, clickEvent)
        {
        }
        
        public LocalizedButton(Action clickEvent) : base(clickEvent)
        {
        }

        ~LocalizedButton()
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