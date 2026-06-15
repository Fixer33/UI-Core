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

        private Color _startColor;
        private Color _targetColor;
        private bool _isSelected;
        private bool _isHovered;
        
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

        private void UpdateColor()
        {
            if (_graphic == null) return;

            Color targetColor = _isSelected ? _selectedColor : _defaultColor;
            
            if (_isHovered && _hoveredColor.Enabled)
            {
                targetColor = _hoveredColor.Value;
            }

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