using UnityEngine;
using UnityEngine.UI;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/Graphics color SVM")]
    public class GraphicsColorSelectionVisualModule : MonoBehaviour, ISelectableElementVisualModule
    {
        [SerializeField] private Graphic _graphic;
        [SerializeField] private Color _defaultColor, _selectedColor;
        private Color? _defaultColorOverride, _selectedColorOverride;
        
        public void OnSelectionChanged(bool isSelected)
        {
            _graphic.color = isSelected ? (_selectedColorOverride ?? _selectedColor) : (_defaultColorOverride ?? _defaultColor);
        }
        
        public void SetDefaultColorOverride(Color color) => _defaultColorOverride = color;
        public void SetSelectedColorOverride(Color color) => _selectedColorOverride = color;
    }
}