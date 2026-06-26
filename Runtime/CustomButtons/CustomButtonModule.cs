using System;
using System.Collections;
using UnityEngine;

namespace UI.CustomButtons
{
    [Serializable]
    public abstract class CustomButtonModule : ICustomButtonAnimatedVisualComponent
    {
        [Header("Animation")]
        [SerializeField] protected bool _animate;
        [SerializeField] protected float _duration = 0.1f;
        [SerializeField] protected bool _useUnscaledTime = true;

        protected MonoBehaviour _owner;
        private Coroutine _animationCoroutine;

        public virtual void OnInitialized(MonoBehaviour owner)
        {
            _owner = owner;
        }

        public abstract void OnNormal();
        public abstract void OnHighlighted();
        public abstract void OnPressed();
        public abstract void OnSelected();
        public abstract void OnDisabled();
        public abstract void OnPremium();
        
        public virtual void SetColorOverride(CustomButtonVisualState state, bool enabled, Color value) {}
        public virtual void SetAlphaOverride(CustomButtonVisualState state, bool enabled, float value) {}
        public virtual void SetFontOverride(CustomButtonVisualState state, bool enabled, TMPro.TMP_FontAsset value) {}
        public virtual void SetToggleOverride(CustomButtonVisualState state, bool enabled, bool value) {}
        public virtual void SetScaleOverride(CustomButtonVisualState state, bool enabled, Vector3 value) {}
        
        public abstract void ClearOverrides();

        public float GetAnimationDuration() => _animate ? _duration : 0;

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
