using UnityEngine;

namespace UI.CustomButtons
{
    [AddComponentMenu("UI/Custom Buttons/Canvas Group VC")]
    public class CanvasGroupVC : StandaloneAnimatedVC
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _normalAlpha = 1f;
        [SerializeField] private CustomButtonState<float> _highlightedAlpha;
        [SerializeField] private CustomButtonState<float> _pressedAlpha;
        [SerializeField] private CustomButtonState<float> _selectedAlpha;
        [SerializeField] private CustomButtonState<float> _disabledAlpha;
        [SerializeField] private CustomButtonState<float> _premiumAlpha;

        private float _startAlpha;
        private float _targetAlpha;

        private CustomButtonState<float>? _overriddenNormal;
        private CustomButtonState<float>? _overriddenHighlighted;
        private CustomButtonState<float>? _overriddenPressed;
        private CustomButtonState<float>? _overriddenSelected;
        private CustomButtonState<float>? _overriddenDisabled;
        private CustomButtonState<float>? _overriddenPremium;

        public override void OnNormal() => SetAlpha(_overriddenNormal?.GetValue(_normalAlpha) ?? _normalAlpha);
        public override void OnHighlighted() => SetAlpha((_overriddenHighlighted ?? _highlightedAlpha).GetValue(_normalAlpha));
        public override void OnPressed() => SetAlpha((_overriddenPressed ?? _pressedAlpha).GetValue(_normalAlpha));
        public override void OnSelected() => SetAlpha((_overriddenSelected ?? _selectedAlpha).GetValue(_normalAlpha));
        public override void OnDisabled() => SetAlpha((_overriddenDisabled ?? _disabledAlpha).GetValue(_normalAlpha));
        public override void OnPremium() => SetAlpha((_overriddenPremium ?? _premiumAlpha).GetValue(_normalAlpha));

        public override void SetAlphaOverride(CustomButtonVisualState state, bool enabled, float value)
        {
            CustomButtonState<float> overrideState = new CustomButtonState<float> { Enabled = enabled, Value = value };
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

        private void SetAlpha(float targetAlpha)
        {
            if (_canvasGroup == null) return;

            _targetAlpha = targetAlpha;
            _startAlpha = _canvasGroup.alpha;

            StartAnimation(t =>
            {
                _canvasGroup.alpha = Mathf.Lerp(_startAlpha, _targetAlpha, t);
            });
        }
    }
}
