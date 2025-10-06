using System;
using UnityEngine;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/Canvas Group SVM")]
    public class CanvasGroupSelectionVisualModule : MonoBehaviour, ISelectableElementVisualModule
    {
        [SerializeField] private CanvasGroupData _defaultState, _selectedState;
        [SerializeField] private CanvasGroup _group;

        private void OnValidate()
        {
            _group ??= GetComponent<CanvasGroup>();
        }

        private void Set(CanvasGroupData stateData)
        {
            _group.alpha = stateData.Alpha;
            _group.interactable = stateData.Interactable;
            _group.blocksRaycasts = stateData.BlockRaycasts;
            _group.ignoreParentGroups = stateData.IgnoreParentGroups;
        }

        public void OnSelectionChanged(bool isSelected)
        {
            Set(isSelected ? _selectedState : _defaultState);
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