using TMPro;
using UnityEngine;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/TMP Font SVM")]
    public class FontSelectionVisualModule : MonoBehaviour, ISelectableElementVisualModule
    {
        [SerializeField] private TMP_FontAsset _defaultFont, _selectedFont;
        [SerializeField] private TMP_Text _text;

        public void OnSelectionChanged(bool isSelected)
        {
            _text.font = isSelected ? _selectedFont : _defaultFont;
        }

        private void OnValidate()
        {
            _text ??= GetComponent<TMP_Text>();
        }
    }
}