using System;
using UnityEngine;

namespace UI.ElementSelection.VisualModules
{
    [Serializable]
    public class ScaleSelectionVisualModule : SelectableElementModule
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _defaultScale = Vector3.one;
        [SerializeField] private Vector3 _selectedScale = Vector3.one;
        [SerializeField] private SelectionModuleState<Vector3> _hoveredScale = new SelectionModuleState<Vector3> { Enabled = false, Value = Vector3.one };
        [SerializeField] private SelectionModuleState<Vector3> _premiumScale = new SelectionModuleState<Vector3> { Enabled = false, Value = Vector3.one };

        private Vector3 _startScale;
        private Vector3 _targetScale;
        private bool _isSelected;
        private bool _isHovered;
        private bool _isPremium;

        private SelectionModuleState<Vector3>? _overriddenDefault;
        private SelectionModuleState<Vector3>? _overriddenSelected;
        private SelectionModuleState<Vector3>? _overriddenHovered;
        private SelectionModuleState<Vector3>? _overriddenPremium;

        public override void OnSelectionChanged(bool isSelected)
        {
            _isSelected = isSelected;
            UpdateScale();
        }

        public override void OnHoverChanged(bool isHovered)
        {
            _isHovered = isHovered;
            UpdateScale();
        }

        public override void OnPremium(bool isInPremiumState)
        {
            _isPremium = isInPremiumState;
            UpdateScale();
        }

        public override void SetScaleOverride(SelectionVisualState state, bool enabled, Vector3 value)
        {
            SelectionModuleState<Vector3> overrideState = new SelectionModuleState<Vector3> { Enabled = enabled, Value = value };
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
            if (_target == null)
            {
                errorMessage = "Target Transform is not assigned";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private void UpdateScale()
        {
            if (_target == null) return;

            if (_isPremium && (_overriddenPremium ?? _premiumScale).Enabled)
            {
                SetScale((_overriddenPremium ?? _premiumScale).Value);
                return;
            }

            Vector3 targetScale = _isSelected ? (_overriddenSelected?.GetValue(_defaultScale) ?? _selectedScale) : (_overriddenDefault?.GetValue(_defaultScale) ?? _defaultScale);
            
            if (_isHovered && (_overriddenHovered ?? _hoveredScale).Enabled)
            {
                targetScale = (_overriddenHovered ?? _hoveredScale).Value;
            }

            SetScale(targetScale);
        }

        private void SetScale(Vector3 targetScale)
        {
            _targetScale = targetScale;
            _startScale = _target != null ? _target.localScale : Vector3.one;

            StartAnimation(t =>
            {
                if (_target != null)
                    _target.localScale = Vector3.Lerp(_startScale, _targetScale, t);
            });
        }
    }
}
