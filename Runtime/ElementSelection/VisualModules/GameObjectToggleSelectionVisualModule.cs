using UnityEngine;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/GameObject toggle SVM")]
    public class GameObjectToggleSelectionVisualModule : MonoBehaviour, ISelectableElementVisualModule
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private bool _activateOnSelection = true;
        [SerializeField] private SelectionModuleState<bool> _hoveredValue;
        [SerializeField] private SelectionModuleState<bool> _premiumValue;

        private bool _isSelected;
        private bool _isHovered;
        private bool _isPremium;
        
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

        public void OnPremium()
        {
            _isPremium = true;
            UpdateActive();
        }

        private void UpdateActive()
        {
            if (_gameObject == null) return;

            bool targetActive;

            if (_isPremium && _premiumValue.Enabled)
            {
                targetActive = _premiumValue.Value;
            }
            else
            {
                targetActive = _activateOnSelection ? _isSelected : !_isSelected;
                
                if (_isHovered && _hoveredValue.Enabled)
                {
                    targetActive = _hoveredValue.Value;
                }
            }

            _gameObject.SetActive(targetActive);
        }
    }
}