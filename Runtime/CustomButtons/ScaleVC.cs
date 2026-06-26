using System;
using UnityEngine;

namespace UI.CustomButtons
{
    [Serializable]
    public class ScaleVC : CustomButtonModule
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _normalScale = Vector3.one;
        [SerializeField] private CustomButtonState<Vector3> _highlightedScale = new CustomButtonState<Vector3> { Enabled = false, Value = Vector3.one };
        [SerializeField] private CustomButtonState<Vector3> _pressedScale = new CustomButtonState<Vector3> { Enabled = false, Value = Vector3.one };
        [SerializeField] private CustomButtonState<Vector3> _selectedScale = new CustomButtonState<Vector3> { Enabled = false, Value = Vector3.one };
        [SerializeField] private CustomButtonState<Vector3> _disabledScale = new CustomButtonState<Vector3> { Enabled = false, Value = Vector3.one };
        [SerializeField] private CustomButtonState<Vector3> _premiumScale = new CustomButtonState<Vector3> { Enabled = false, Value = Vector3.one };

        private Vector3 _startScale;
        private Vector3 _targetScale;

        private CustomButtonState<Vector3>? _overriddenNormal;
        private CustomButtonState<Vector3>? _overriddenHighlighted;
        private CustomButtonState<Vector3>? _overriddenPressed;
        private CustomButtonState<Vector3>? _overriddenSelected;
        private CustomButtonState<Vector3>? _overriddenDisabled;
        private CustomButtonState<Vector3>? _overriddenPremium;

        public override void OnNormal() => SetScale(_overriddenNormal?.GetValue(_normalScale) ?? _normalScale);
        public override void OnHighlighted() => SetScale((_overriddenHighlighted ?? _highlightedScale).GetValue(_normalScale));
        public override void OnPressed() => SetScale((_overriddenPressed ?? _pressedScale).GetValue(_normalScale));
        public override void OnSelected() => SetScale((_overriddenSelected ?? _selectedScale).GetValue(_normalScale));
        public override void OnDisabled() => SetScale((_overriddenDisabled ?? _disabledScale).GetValue(_normalScale));
        public override void OnPremium() => SetScale((_overriddenPremium ?? _premiumScale).GetValue(_normalScale));

        public override void SetScaleOverride(CustomButtonVisualState state, bool enabled, Vector3 value)
        {
            CustomButtonState<Vector3> overrideState = new CustomButtonState<Vector3> { Enabled = enabled, Value = value };
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
            if (_target == null)
            {
                errorMessage = "Target Transform is not assigned";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private void SetScale(Vector3 targetScale)
        {
            if (_target == null) return;

            _targetScale = targetScale;
            _startScale = _target.localScale;

            StartAnimation(t =>
            {
                if (_target != null)
                    _target.localScale = Vector3.Lerp(_startScale, _targetScale, t);
            });
        }
    }
}
