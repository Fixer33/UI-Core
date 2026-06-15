using TMPro;
using UnityEngine;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/TMP Font SVM")]
    public class FontSelectionVisualModule : MonoBehaviour, ISelectableElementVisualModule
    {
        [SerializeField] private TMP_FontAsset _defaultFont, _selectedFont;
        [SerializeField] private SelectionModuleState<TMP_FontAsset> _hoveredFont;
        [SerializeField] private SelectionModuleState<TMP_FontAsset> _premiumFont;
        [SerializeField] private TMP_Text _text;

        private bool _isSelected;
        private bool _isHovered;
        private bool _isPremium;

        public void OnSelectionChanged(bool isSelected)
        {
            _isSelected = isSelected;
            UpdateFont();
        }

        public void OnHoverChanged(bool isHovered)
        {
            _isHovered = isHovered;
            UpdateFont();
        }

        public void OnPremium()
        {
            _isPremium = true;
            UpdateFont();
        }

        private void UpdateFont()
        {
            if (_text == null) return;

            TMP_FontAsset targetFont;

            if (_isPremium && _premiumFont.Enabled)
            {
                targetFont = _premiumFont.Value;
            }
            else
            {
                targetFont = _isSelected ? _selectedFont : _defaultFont;
                if (_isHovered && _hoveredFont.Enabled)
                {
                    targetFont = _hoveredFont.Value;
                }
            }

            if (targetFont != null)
                _text.font = targetFont;
        }

        private void OnValidate()
        {
            _text ??= GetComponent<TMP_Text>();
        }
    }
}