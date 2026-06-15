using UnityEngine;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/GameObject toggle SVM")]
    public class GameObjectToggleSelectionVisualModule : MonoBehaviour, ISelectableElementVisualModule
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private bool _activateOnSelection = true;
        [SerializeField] private SelectionModuleState<bool> _hoveredValue;

        private bool _isSelected;
        private bool _isHovered;
        
        public void OnSelectionChanged(bool isSelected)
        {
            _isSelected = isSelected;
            UpdateActive();
        }

        public void OnHoverChanged(bool isHovered)
        {
            _isHovered = isHovered;
            UpdateActive();
        }

        private void UpdateActive()
        {
            if (_gameObject == null) return;

            bool targetActive = _activateOnSelection ? _isSelected : !_isSelected;
            
            if (_isHovered && _hoveredValue.Enabled)
            {
                targetActive = _hoveredValue.Value;
            }

            _gameObject.SetActive(targetActive);
        }
    }
}