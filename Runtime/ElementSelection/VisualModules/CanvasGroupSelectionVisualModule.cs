using System;
using UnityEngine;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/Canvas Group SVM")]
    public class CanvasGroupSelectionVisualModule : StandaloneAnimatedSVM
    {
        [SerializeField] private CanvasGroupData _defaultState, _selectedState;
        [SerializeField] private SelectionModuleState<CanvasGroupData> _hoveredState;
        [SerializeField] private SelectionModuleState<CanvasGroupData> _premiumState;
        [SerializeField] private CanvasGroup _group;

        private float _startAlpha;
        private float _targetAlpha;
        private bool _isSelected;
        private bool _isHovered;
        private bool _isPremium;

        private void OnValidate()
        {
            _group ??= GetComponent<CanvasGroup>();
        }

        private void UpdateState(bool instant)
        {
            if (_group == null) return;

            CanvasGroupData targetState;
            
            if (_isPremium && _premiumState.Enabled)
            {
                targetState = _premiumState.Value;
            }
            else
            {
                targetState = _isSelected ? _selectedState : _defaultState;
                if (_isHovered && _hoveredState.Enabled)
                {
                    targetState = _hoveredState.Value;
                }
            }

            _group.interactable = targetState.Interactable;
            _group.blocksRaycasts = targetState.BlockRaycasts;
            _group.ignoreParentGroups = targetState.IgnoreParentGroups;

            _targetAlpha = targetState.Alpha;
            _startAlpha = _group.alpha;

            if (instant || !_animate || !Application.isPlaying)
            {
                StopAnimation();
                _group.alpha = _targetAlpha;
            }
            else
            {
                StartAnimation(t =>
                {
                    _group.alpha = Mathf.Lerp(_startAlpha, _targetAlpha, t);
                });
            }
        }

        public override void OnSelectionChanged(bool isSelected)
        {
            _isSelected = isSelected;
            UpdateState(false);
        }

        public override void OnHoverChanged(bool isHovered)
        {
            _isHovered = isHovered;
            UpdateState(false);
        }

        public override void OnPremium(bool isInPremiumState)
        {
            _isPremium = isInPremiumState;
            UpdateState(false);
        }
        
        [Serializable]
        public struct CanvasGroupData
        {
            [field: SerializeField] public float Alpha { get; private set; }
            [field: SerializeField] public bool Interactable { get; private set; }
            [field: SerializeField] public bool BlockRaycasts { get; private set; }
            [field: SerializeField] public bool IgnoreParentGroups { get; private set; }
        }
    }
}