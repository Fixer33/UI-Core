using System;
using TMPro;
using UnityEngine;

namespace UI.CustomButtons
{
    [Serializable]
    public class FontVC : CustomButtonModule
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_FontAsset _normalFont;
        [SerializeField] private CustomButtonState<TMP_FontAsset> _highlightedFont;
        [SerializeField] private CustomButtonState<TMP_FontAsset> _pressedFont;
        [SerializeField] private CustomButtonState<TMP_FontAsset> _selectedFont;
        [SerializeField] private CustomButtonState<TMP_FontAsset> _disabledFont;
        [SerializeField] private CustomButtonState<TMP_FontAsset> _premiumFont;

        private CustomButtonState<TMP_FontAsset>? _overriddenNormal;
        private CustomButtonState<TMP_FontAsset>? _overriddenHighlighted;
        private CustomButtonState<TMP_FontAsset>? _overriddenPressed;
        private CustomButtonState<TMP_FontAsset>? _overriddenSelected;
        private CustomButtonState<TMP_FontAsset>? _overriddenDisabled;
        private CustomButtonState<TMP_FontAsset>? _overriddenPremium;

        public override void OnNormal() => SetFont(_overriddenNormal?.GetValue(_normalFont) ?? _normalFont);
        public override void OnHighlighted() => SetFont((_overriddenHighlighted ?? _highlightedFont).GetValue(_normalFont));
        public override void OnPressed() => SetFont((_overriddenPressed ?? _pressedFont).GetValue(_normalFont));
        public override void OnSelected() => SetFont((_overriddenSelected ?? _selectedFont).GetValue(_normalFont));
        public override void OnDisabled() => SetFont((_overriddenDisabled ?? _disabledFont).GetValue(_normalFont));
        public override void OnPremium() => SetFont((_overriddenPremium ?? _premiumFont).GetValue(_normalFont));

        public override void SetFontOverride(CustomButtonVisualState state, bool enabled, TMP_FontAsset value)
        {
            CustomButtonState<TMP_FontAsset> overrideState = new CustomButtonState<TMP_FontAsset> { Enabled = enabled, Value = value };
            switch (state)
            {
                case CustomButtonVisualState.Normal: _overriddenNormal = overrideState; break;
                case CustomButtonVisualState.Highlighted: _overriddenHighlighted = overrideState; break;
                case CustomButtonVisualState.Pressed: _overriddenPressed = overrideState; break;
                case CustomButtonVisualState.Selected: _overriddenSelected = overrideState; break;
                case CustomButtonVisualState.Disabled: _overriddenDisabled = overrideState; break;
                case CustomButtonVisualState.Premium: _overriddenPremium = overrideState; break;
            }
        }

        public override void ClearOverrides()
        {
            _overriddenNormal = null;
            _overriddenHighlighted = null;
            _overriddenPressed = null;
            _overriddenSelected = null;
            _overriddenDisabled = null;
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

        private void SetFont(TMP_FontAsset font)
        {
            if (_text != null && font != null)
                _text.font = font;
        }
    }
}
