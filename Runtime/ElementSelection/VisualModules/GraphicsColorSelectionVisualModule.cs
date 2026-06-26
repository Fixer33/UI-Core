using System;
using UI.ElementSelection;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ElementSelection.VisualModules
{
    [Serializable]
    public class GraphicsColorSelectionVisualModule : SelectableElementModule
    {
        [SerializeField] private Graphic _graphic;
        [SerializeField] private Color _defaultColor = Color.white, _selectedColor = Color.white;
        [SerializeField] private SelectionModuleState<Color> _hoveredColor;
        [SerializeField] private SelectionModuleState<Color> _premiumColor;

        private Color _startColor;
        private Color _targetColor;
        private bool _isSelected;
        private bool _isHovered;
        private bool _isPremium;

        private SelectionModuleState<Color>? _overriddenDefault;
        private SelectionModuleState<Color>? _overriddenSelected;
        private SelectionModuleState<Color>? _overriddenHovered;
        private SelectionModuleState<Color>? _overriddenPremium;

        public override void OnSelectionChanged(bool isSelected)
        {
            _isSelected = isSelected;
            UpdateColor();
        }

        public override void OnHoverChanged(bool isHovered)
        {
            _isHovered = isHovered;
            UpdateColor();
        }

        public override void OnPremium(bool isInPremiumState)
        {
            _isPremium = isInPremiumState;
            UpdateColor();
        }

        public override void SetColorOverride(SelectionVisualState state, bool enabled, Color value)
        {
            SelectionModuleState<Color> overrideState = new SelectionModuleState<Color> { Enabled = enabled, Value = value };
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
            if (_graphic == null)
            {
                errorMessage = "Graphic is not assigned";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private void UpdateColor()
        {
            if (_graphic == null) return;

            if (_isPremium && (_overriddenPremium ?? _premiumColor).Enabled)
            {
                SetColor((_overriddenPremium ?? _premiumColor).Value);
                return;
            }

            Color targetColor = _isSelected ? (_overriddenSelected?.GetValue(_defaultColor) ?? _selectedColor) : (_overriddenDefault?.GetValue(_defaultColor) ?? _defaultColor);
            
            if (_isHovered && (_overriddenHovered ?? _hoveredColor).Enabled)
            {
                targetColor = (_overriddenHovered ?? _hoveredColor).Value;
            }

            SetColor(targetColor);
        }

        private void SetColor(Color targetColor)
        {
            _targetColor = targetColor;
            _startColor = _graphic != null ? _graphic.color : Color.white;

            StartAnimation(t =>
            {
                if (_graphic != null)
                    _graphic.color = Color.Lerp(_startColor, _targetColor, t);
            });
        }
    }

    [Serializable]
    public struct SelectionModuleState<T>
    {
        public bool Enabled;
        public T Value;

        public T GetValue(T defaultValue) => Enabled ? Value : defaultValue;
    }
}