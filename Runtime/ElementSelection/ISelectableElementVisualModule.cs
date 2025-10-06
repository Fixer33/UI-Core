using UnityEngine;

namespace UI.ElementSelection
{
    public interface ISelectableElementVisualModule
    {
        public bool IsElementAlive => this is MonoBehaviour monoBehaviour ? monoBehaviour : true;

        public void OnInitialized() {}
        public void OnSelectionChanged(bool isSelected);
    }
}