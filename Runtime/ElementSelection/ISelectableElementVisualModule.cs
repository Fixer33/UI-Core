using UnityEngine;

namespace UI.ElementSelection
{
    public interface ISelectableElementVisualModule
    {
        public bool IsElementAlive => this is MonoBehaviour monoBehaviour ? monoBehaviour : true;

        public void OnInitialized() {}
        public void OnSelectionChanged(bool isSelected);
        public void OnHoverChanged(bool isHovered) {}
        public float GetAnimationDuration() => 0f;
    }
}