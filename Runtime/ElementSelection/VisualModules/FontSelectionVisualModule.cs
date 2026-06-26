using System;
using TMPro;
using UI.ElementSelection;
using UnityEngine;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/TMP Font SVM")]
    [Serializable]
    public class FontSelectionVisualModule : SelectableElementModule
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

        public override void OnSelectionChanged(bool isSelected)
        {
            _isSelected = isSelected;
            UpdateFont();
        }

        public override void OnHoverChanged(bool isHovered)
        {
            _isHovered = isHovered;
            UpdateFont();
        }

        public override void OnPremium(bool isInPremiumState)
        {
            _isPremium = isInPremiumState;
            UpdateFont();
        }

        public override void SetFontOverride(SelectionVisualState state, bool enabled, TMP_FontAsset value)
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

        public override void ClearOverrides()
        {
            _overriddenDefault = null;
            _overriddenSelected = null;
            _overriddenHovered = null;
            _overriddenPremium = null;
        }

        public override bool IsValid(out string errorMessage)
        {
            if (_text == null)
            {
                errorMessage = "TMP Text is not assigned";
                return false;
            }

            errorMessage = null;
            return true;
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
    }
}