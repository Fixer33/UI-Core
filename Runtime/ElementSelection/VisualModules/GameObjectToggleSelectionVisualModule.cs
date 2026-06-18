using UnityEngine;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/GameObject toggle SVM")]
    public class GameObjectToggleSelectionVisualModule : StandaloneAnimatedSVM
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private SelectionModuleState<bool> _normalValue = new SelectionModuleState<bool> { Enabled = true, Value = false };
        [SerializeField] private SelectionModuleState<bool> _selectedValue = new SelectionModuleState<bool> { Enabled = true, Value = true };
        [SerializeField] private SelectionModuleState<bool> _hoveredValue;
        [SerializeField] private SelectionModuleState<bool> _premiumValue;

        private bool _isSelected;
        private bool _isHovered;
        private bool _isPremium;
        
        private SelectionModuleState<bool>? _overriddenDefault;
        private SelectionModuleState<bool>? _overriddenSelected;
        private SelectionModuleState<bool>? _overriddenHovered;
        private SelectionModuleState<bool>? _overriddenPremium;

        public override void OnSelectionChanged(bool isSelected)
        {
            _isSelected = isSelected;
            UpdateActive();
        }

        public override void OnHoverChanged(bool isHovered)
        {
            _isHovered = isHovered;
            UpdateActive();
        }

        public override void OnPremium(bool isInPremiumState)
        {
            _isPremium = isInPremiumState;
            UpdateActive();
        }

        public override void SetToggleOverride(SelectionVisualState state, bool enabled, bool value)
        {
            SelectionModuleState<bool> overrideState = new SelectionModuleState<bool> { Enabled = enabled, Value = value };
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

        private void UpdateActive()
        {
            if (_gameObject == null) return;

            bool targetActive;

            if (_isPremium && (_overriddenPremium ?? _premiumValue).Enabled)
            {
                targetActive = (_overriddenPremium ?? _premiumValue).Value;
            }
            else
            {
                targetActive = _isSelected ? (_overriddenSelected?.GetValue(_normalValue.Value) ?? _selectedValue.GetValue(true)) : (_overriddenDefault?.GetValue(_normalValue.Value) ?? _normalValue.GetValue(false));
                
                if (_isHovered && (_overriddenHovered ?? _hoveredValue).Enabled)
                {
                    targetActive = (_overriddenHovered ?? _hoveredValue).Value;
                }
            }

            _gameObject.SetActive(targetActive);
        }
    }
}