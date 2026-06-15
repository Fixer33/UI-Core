using UnityEngine;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/GameObject toggle SVM")]
    public class GameObjectToggleSelectionVisualModule : MonoBehaviour, ISelectableElementVisualModule
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private SelectionModuleState<bool> _normalValue = new SelectionModuleState<bool> { Enabled = true, Value = false };
        [SerializeField] private SelectionModuleState<bool> _selectedValue = new SelectionModuleState<bool> { Enabled = true, Value = true };
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
                targetActive = _isSelected ? _selectedValue.GetValue(true) : _normalValue.GetValue(false);
                
                if (_isHovered && _hoveredValue.Enabled)
                {
                    targetActive = _hoveredValue.Value;
                }
            }

            _gameObject.SetActive(targetActive);
        }
    }
}