using System;
using DG.Tweening;
using UnityEngine;

namespace UI.Canvas.ViewBases
{
    [Serializable]
    public class FadingViewParams : UICanvasViewParameters
    {
        [field: SerializeField] public float AppearanceTime { get; private set; } = .5f;
    }
    
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class FadingView : UICanvasView<FadingViewParams>
    {
        private CanvasGroup _canvasGroup;

        protected override void OnAwakeEarly()
        {
            base.OnAwakeEarly();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void ShowVisually(Action onComplete = null)
        {
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(1, Parameters.AppearanceTime).SetUpdate(true).OnComplete(() =>
            {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
                onComplete?.Invoke();
            });
        }

        protected override void HideVisually(Action onComplete = null)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(0, Parameters.AppearanceTime).SetUpdate(true).OnComplete(() =>
            {
                _canvasGroup.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        protected override void ShowInstantVisually()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.gameObject.SetActive(true);
        }

        protected override void HideInstantVisually()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.gameObject.SetActive(false);
        }

        public override bool IsVisible()
        {
            return _canvasGroup.gameObject.activeSelf && _canvasGroup.alpha >= 1 && _canvasGroup.interactable &&
                   _canvasGroup.blocksRaycasts;
        }
    }
}