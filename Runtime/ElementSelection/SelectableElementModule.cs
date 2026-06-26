using System;
using System.Collections;
using UnityEngine;

namespace UI.ElementSelection
{
    [Serializable]
    public abstract class SelectableElementModule : ISelectableElementVisualModule
    {
        [Header("Animation")]
        [SerializeField] protected bool _animate;
        [SerializeField] protected float _duration = 0.1f;
        [SerializeField] protected bool _useUnscaledTime = true;

        protected MonoBehaviour _owner;
        private Coroutine _animationCoroutine;

        public bool IsElementAlive => _owner != null;

        public virtual void OnInitialized(MonoBehaviour owner)
        {
            _owner = owner;
        }

        public abstract void OnSelectionChanged(bool isSelected);
        public virtual void OnHoverChanged(bool isHovered) {}
        public virtual void OnPremium(bool isInPremiumState) {}
        public virtual float GetAnimationDuration() => _animate ? _duration : 0f;

        public virtual void SetColorOverride(SelectionVisualState state, bool enabled, Color value) {}
        public virtual void SetAlphaOverride(SelectionVisualState state, bool enabled, float value) {}
        public virtual void SetFontOverride(SelectionVisualState state, bool enabled, TMPro.TMP_FontAsset value) {}
        public virtual void SetToggleOverride(SelectionVisualState state, bool enabled, bool value) {}
        public virtual void SetScaleOverride(SelectionVisualState state, bool enabled, Vector3 value) {}

        public virtual void ClearOverrides() {}

        public virtual bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            return true;
        }

        protected void StartAnimation(Action<float> updateAction)
        {
            StopAnimation();

            if (_animate && Application.isPlaying && _owner != null && _owner.gameObject.activeInHierarchy)
            {
                _animationCoroutine = _owner.StartCoroutine(AnimationRoutine(updateAction));
            }
            else
            {
                updateAction(1f);
            }
        }

        protected void StopAnimation()
        {
            if (_animationCoroutine != null && _owner != null)
            {
                _owner.StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }
        }

        private IEnumerator AnimationRoutine(Action<float> updateAction)
        {
            float elapsed = 0;
            while (elapsed < _duration)
            {
                float t = _duration > 0 ? elapsed / _duration : 1f;
                updateAction(t);
                
                if (_useUnscaledTime)
                    elapsed += Time.unscaledDeltaTime;
                else
                    elapsed += Time.deltaTime;
                
                yield return null;
            }

            updateAction(1f);
            _animationCoroutine = null;
        }
    }
}
