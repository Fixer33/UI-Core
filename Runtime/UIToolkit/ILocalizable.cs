#if USE_LOCALIZATION_PACKAGE
using UnityEngine.Localization;

namespace UI.Core.UIToolkit
{
    public interface ILocalizable
    {
        public void InitLocalization(LocalizedString localization);
    }
}
#endif