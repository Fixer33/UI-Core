using System;
using UnityEngine;

namespace UI.Canvas.ViewBases
{
    [Serializable]
    public class AnimatedCanvasViewParams : UICanvasViewParameters
    {
        [field: SerializeField] public float VisibilityAnimSpeedMultiplier { get; private set; } = 1;
    }
    
    [RequireComponent(typeof(Animator))]
    public abstract class AnimatedView : UICanvasView<AnimatedCanvasViewParams>
    {
        private static readonly int AnimatorBoolId = Animator.StringToHash("IsVisible");
        private static readonly int AnimatorSpeedId = Animator.StringToHash("VisibilitySpeed");
        private const string AnimatorOpenStateName = "Visible";

        private bool _isAnimating;
        private Action _onComplete;
        private Animator _animator;
        private bool _isVisible;

        protected override void OnAwakeEarly()
        {
            _animator = GetComponent<Animator>();
        }

        protected override void ShowVisually(Action onComplete = null)
        {
            HideInstantVisually();
            _onComplete = onComplete;
            _isAnimating = true;

            gameObject.SetActive(true);
            _animator.SetFloat(AnimatorSpeedId, Parameters.VisibilityAnimSpeedMultiplier);
            _animator.SetBool(AnimatorBoolId, true);
        }

        protected override void HideVisually(Action onComplete = null)
        {
            _animator.SetFloat(AnimatorSpeedId, Parameters.VisibilityAnimSpeedMultiplier);
            
            if (IsVisible() == false && _isAnimating == false)
            {
                return;
            }

            _onComplete = () =>
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            };
            _animator.SetBool(AnimatorBoolId, false);
            _isAnimating = true;
        }

        protected override void ShowInstantVisually()
        {
            gameObject.SetActive(true);
            _animator.SetBool(AnimatorBoolId, true);
            _animator.Play(AnimatorOpenStateName);
            _isVisible = true;
        }

        protected override void HideInstantVisually()
        {
            _animator.SetBool(AnimatorBoolId, false);
            _isVisible = false;
            gameObject.SetActive(false);
        }

        public override bool IsVisible()
        {
            return _isVisible;
        }

#region Animation events

        public void AE_OnShown()
        {
            _onComplete?.Invoke();
            _onComplete = null;
            _isVisible = true;
            _isAnimating = false;
        }

        public void AE_OnHidden()
        {
            _onComplete?.Invoke();
            _onComplete = null;
            _isVisible = false;
            _isAnimating = false;
        }

#endregion
    }
}