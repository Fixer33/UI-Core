using System;
using DG.Tweening;
using UnityEngine;

namespace UI.Canvas.ViewBases
{
    [Serializable]
    public class PanelScaleCanvasViewParams : UICanvasViewParameters
    {
        [field: SerializeField] public float VerticalScaleTime { get; private set; }= .25f;
        [field: SerializeField] public float HorizontalScaleTime { get; private set; }= .25f;
        [field: SerializeField] public Ease Ease { get; private set; }= Ease.Linear;
        [field: SerializeField] public RectTransform[] Panels { get; private set; } = Array.Empty<RectTransform>();
    }
    
    public abstract class PanelScaleView : UICanvasView<PanelScaleCanvasViewParams>
    {
        private Sequence _animation;
        
        protected override void ShowVisually(Action onComplete = null)
        {
            _animation?.Kill();
            gameObject.SetActive(true);
            
            _animation = DOTween.Sequence().SetUpdate(true);
            foreach (var panel in Parameters.Panels)
            {
                _animation.Join(panel.DOScaleX(1, Parameters.HorizontalScaleTime).From(0).SetEase(Parameters.Ease).SetUpdate(true));
                _animation.Join(panel.DOScaleY(1, Parameters.VerticalScaleTime).From(0).SetEase(Parameters.Ease).SetUpdate(true));
            }

            _animation.OnComplete(() =>
            {
                _animation = null;
                onComplete?.Invoke();
            });
        }

        protected override void HideVisually(Action onComplete = null)
        {
            _animation?.Kill();
            
            _animation = DOTween.Sequence().SetUpdate(true);
            _animation.AppendInterval(0);
            foreach (var panel in Parameters.Panels)
            {
                if (Parameters.HorizontalScaleTime > 0)
                    _animation.Join(panel.DOScaleX(0, Parameters.HorizontalScaleTime).SetEase(Parameters.Ease).SetUpdate(true));
                if (Parameters.VerticalScaleTime > 0)
                    _animation.Join(panel.DOScaleY(0, Parameters.VerticalScaleTime).SetEase(Parameters.Ease).SetUpdate(true));
            }

            _animation.OnComplete(() =>
            {
                _animation = null;
                onComplete?.Invoke();
                gameObject.SetActive(false);
            });
        }

        protected override void ShowInstantVisually()
        {
            _animation?.Kill();
            _animation = null;
            foreach (var panel in Parameters.Panels)
            {
                panel.DOScaleX(1, 0).SetUpdate(true);
                panel.DOScaleY(1, 0).SetUpdate(true);
            }
            gameObject.SetActive(true);
        }

        protected override void HideInstantVisually()
        {
            _animation?.Kill();
            _animation = null;
            gameObject.SetActive(false);
        }

        public override bool IsVisible() => gameObject.activeSelf && _animation == null;
    }
}