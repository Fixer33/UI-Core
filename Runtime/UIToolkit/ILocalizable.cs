#if USE_LOCALIZATION_PACKAGE
using UnityEngine.Localization;

namespace UI.UIToolkit
{
    public interface ILocalizable
    {
        public void InitLocalization(LocalizedString localization);
    }
}
#endif