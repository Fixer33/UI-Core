using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/Graphics color SVM")]
    public class GraphicsColorSelectionVisualModule : StandaloneAnimatedSVM
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

        private void UpdateColor()
        {
            if (_graphic == null) return;

            if (_isPremium && _premiumColor.Enabled)
            {
                SetColor(_premiumColor.Value);
                return;
            }

            Color targetColor = _isSelected ? _selectedColor : _defaultColor;
            
            if (_isHovered && _hoveredColor.Enabled)
            {
                targetColor = _hoveredColor.Value;
            }

            SetColor(targetColor);
        }

        private void SetColor(Color targetColor)
        {
            _targetColor = targetColor;
            _startColor = _graphic.color;

            StartAnimation(t =>
            {
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