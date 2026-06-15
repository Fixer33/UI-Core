using System;
using System.Collections;
using UnityEngine;

namespace UI.ElementSelection
{
    public abstract class StandaloneAnimatedSVM : MonoBehaviour, ISelectableElementVisualModule
    {
        [Header("Animation")]
        [SerializeField] protected bool _animate;
        [SerializeField] protected float _duration = 0.1f;
        [SerializeField] protected bool _useUnscaledTime = true;

        private Coroutine _animationCoroutine;

        public bool IsElementAlive => this;

        public virtual void OnInitialized() {}
        public abstract void OnSelectionChanged(bool isSelected);
        public virtual void OnHoverChanged(bool isHovered) {}
        public virtual float GetAnimationDuration() => _animate ? _duration : 0f;

        protected void StartAnimation(Action<float> updateAction)
        {
            StopAnimation();

            if (_animate && Application.isPlaying && gameObject.activeInHierarchy)
            {
                _animationCoroutine = StartCoroutine(AnimationRoutine(updateAction));
            }
            else
            {
                updateAction(1f);
            }
        }

        protected void StopAnimation()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
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

        protected virtual void OnDisable()
        {
            StopAnimation();
        }
    }
}
