using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CustomButtons
{
    [Serializable]
    public class GraphicsColorVC : CustomButtonModule
    {
        [SerializeField] private Graphic _graphic;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private CustomButtonState<Color> _highlightedColor;
        [SerializeField] private CustomButtonState<Color> _pressedColor;
        [SerializeField] private CustomButtonState<Color> _selectedColor;
        [SerializeField] private CustomButtonState<Color> _disabledColor;
        [SerializeField] private CustomButtonState<Color> _premiumColor;

        private Color _startColor;
        private Color _targetColor;

        private CustomButtonState<Color>? _overriddenNormal;
        private CustomButtonState<Color>? _overriddenHighlighted;
        private CustomButtonState<Color>? _overriddenPressed;
        private CustomButtonState<Color>? _overriddenSelected;
        private CustomButtonState<Color>? _overriddenDisabled;
        private CustomButtonState<Color>? _overriddenPremium;

        public override void OnNormal() => SetColor(_overriddenNormal?.GetValue(_normalColor) ?? _normalColor);
        public override void OnHighlighted() => SetColor((_overriddenHighlighted ?? _highlightedColor).GetValue(_normalColor));
        public override void OnPressed() => SetColor((_overriddenPressed ?? _pressedColor).GetValue(_normalColor));
        public override void OnSelected() => SetColor((_overriddenSelected ?? _selectedColor).GetValue(_normalColor));
        public override void OnDisabled() => SetColor((_overriddenDisabled ?? _disabledColor).GetValue(_normalColor));
        public override void OnPremium() => SetColor((_overriddenPremium ?? _premiumColor).GetValue(_normalColor));

        public override void SetColorOverride(CustomButtonVisualState state, bool enabled, Color value)
        {
            CustomButtonState<Color> overrideState = new CustomButtonState<Color> { Enabled = enabled, Value = value };
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
            if (_graphic == null)
            {
                errorMessage = "Graphic is not assigned";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private void SetColor(Color targetColor)
        {
            if (_graphic == null) return;

            _targetColor = targetColor;
            _startColor = _graphic.color;

            StartAnimation(t =>
            {
                if (_graphic != null)
                    _graphic.color = Color.Lerp(_startColor, _targetColor, t);
            });
        }
    }
}
