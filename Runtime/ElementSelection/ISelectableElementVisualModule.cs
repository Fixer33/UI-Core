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
        public bool IsElementAlive { get; }

        public void OnInitialized(MonoBehaviour owner) {}
        public void OnSelectionChanged(bool isSelected);
        public void OnHoverChanged(bool isHovered) {}
        public void OnPremium(bool isInPremiumState) {}
        public float GetAnimationDuration() => 0f;
        
        public void SetColorOverride(SelectionVisualState state, bool enabled, Color value) {}
        public void SetAlphaOverride(SelectionVisualState state, bool enabled, float value) {}
        public void SetFontOverride(SelectionVisualState state, bool enabled, TMPro.TMP_FontAsset value) {}
        public void SetToggleOverride(SelectionVisualState state, bool enabled, bool value) {}
        public void SetScaleOverride(SelectionVisualState state, bool enabled, Vector3 value) {}
        
        public void ClearOverrides() {}

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            return true;
        }
    }
}