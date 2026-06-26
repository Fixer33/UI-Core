using System;
using UnityEngine;

namespace UI.CustomButtons
{
    [Serializable]
    public class GameObjectToggleVC : CustomButtonModule
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private bool _normalValue;
        [SerializeField] private CustomButtonState<bool> _highlightedValue;
        [SerializeField] private CustomButtonState<bool> _pressedValue;
        [SerializeField] private CustomButtonState<bool> _selectedValue;
        [SerializeField] private CustomButtonState<bool> _disabledValue;
        [SerializeField] private CustomButtonState<bool> _premiumValue;

        private CustomButtonState<bool>? _overriddenNormal;
        private CustomButtonState<bool>? _overriddenHighlighted;
        private CustomButtonState<bool>? _overriddenPressed;
        private CustomButtonState<bool>? _overriddenSelected;
        private CustomButtonState<bool>? _overriddenDisabled;
        private CustomButtonState<bool>? _overriddenPremium;

        public override void OnNormal() => SetActive(_overriddenNormal?.GetValue(_normalValue) ?? _normalValue);
        public override void OnHighlighted() => SetActive((_overriddenHighlighted ?? _highlightedValue).GetValue(_normalValue));
        public override void OnPressed() => SetActive((_overriddenPressed ?? _pressedValue).GetValue(_normalValue));
        public override void OnSelected() => SetActive((_overriddenSelected ?? _selectedValue).GetValue(_normalValue));
        public override void OnDisabled() => SetActive((_overriddenDisabled ?? _disabledValue).GetValue(_normalValue));
        public override void OnPremium() => SetActive((_overriddenPremium ?? _premiumValue).GetValue(_normalValue));

        public override void SetToggleOverride(CustomButtonVisualState state, bool enabled, bool value)
        {
            CustomButtonState<bool> overrideState = new CustomButtonState<bool> { Enabled = enabled, Value = value };
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
                errorMessage = "Target GameObject is not assigned";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private void SetActive(bool active)
        {
            if (_target != null)
                _target.SetActive(active);
        }
    }
}
