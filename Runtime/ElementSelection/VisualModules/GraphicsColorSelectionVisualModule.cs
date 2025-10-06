using UnityEngine;
using UnityEngine.UI;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/Graphics color SVM")]
    public class GraphicsColorSelectionVisualModule : MonoBehaviour, ISelectableElementVisualModule
    {
        [SerializeField] private Graphic _graphic;
        [SerializeField] private Color _defaultColor, _selectedColor;

        public void OnSelectionChanged(bool isSelected)
        {
            _graphic.color = isSelected ? _selectedColor : _defaultColor;
        }
    }
}