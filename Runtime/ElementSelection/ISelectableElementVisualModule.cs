using UnityEngine;

namespace UI.ElementSelection
{
    public enum SelectionVisualState
    {
        Default,
        Selected,
        Hovered,
        Premium
    }

    public interface ISelectableElementVisualModule
    {
        public bool IsElementAlive => this is MonoBehaviour monoBehaviour ? monoBehaviour : true;

        public void OnInitialized() {}
        public void OnSelectionChanged(bool isSelected);
        public void OnHoverChanged(bool isHovered) {}
        public void OnPremium(bool isInPremiumState) {}
        public float GetAnimationDuration() => 0f;
        
        public void SetColorOverride(SelectionVisualState state, bool enabled, Color value) {}
        public void SetAlphaOverride(SelectionVisualState state, bool enabled, float value) {}
        public void SetFontOverride(SelectionVisualState state, bool enabled, TMPro.TMP_FontAsset value) {}
        public void SetToggleOverride(SelectionVisualState state, bool enabled, bool value) {}
        
        public void ClearOverrides() {}
    }
}