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
        
        private SelectionModuleState<TMP_FontAsset>? _overriddenDefault;
        private SelectionModuleState<TMP_FontAsset>? _overriddenSelected;
        private SelectionModuleState<TMP_FontAsset>? _overriddenHovered;
        private SelectionModuleState<TMP_FontAsset>? _overriddenPremium;

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

        public void OnPremium(bool isInPremiumState)
        {
            _isPremium = isInPremiumState;
            UpdateFont();
        }

        public void SetFontOverride(SelectionVisualState state, bool enabled, TMP_FontAsset value)
        {
            SelectionModuleState<TMP_FontAsset> overrideState = new SelectionModuleState<TMP_FontAsset> { Enabled = enabled, Value = value };
            switch (state)
            {
                case SelectionVisualState.Default: _overriddenDefault = overrideState; break;
                case SelectionVisualState.Selected: _overriddenSelected = overrideState; break;
                case SelectionVisualState.Hovered: _overriddenHovered = overrideState; break;
                case SelectionVisualState.Premium: _overriddenPremium = overrideState; break;
            }
        }

        public void ClearOverrides()
        {
            _overriddenDefault = null;
            _overriddenSelected = null;
            _overriddenHovered = null;
            _overriddenPremium = null;
        }

        private void UpdateFont()
        {
            if (_text == null) return;

            TMP_FontAsset targetFont;

            if (_isPremium && (_overriddenPremium ?? _premiumFont).Enabled)
            {
                targetFont = (_overriddenPremium ?? _premiumFont).Value;
            }
            else
            {
                targetFont = _isSelected ? (_overriddenSelected?.GetValue(_defaultFont) ?? _selectedFont) : (_overriddenDefault?.GetValue(_defaultFont) ?? _defaultFont);
                if (_isHovered && (_overriddenHovered ?? _hoveredFont).Enabled)
                {
                    targetFont = (_overriddenHovered ?? _hoveredFont).Value;
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