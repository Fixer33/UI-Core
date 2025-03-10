using System;

namespace UI.Canvas.ViewBases
{
    [Serializable]
    public class InstantCanvasViewParams : UICanvasViewParameters
    {
    }
    
    public abstract class InstantView : UICanvasView<InstantCanvasViewParams>
    {
        protected override void ShowVisually(Action onComplete = null)
        {
            ShowInstantVisually();
            onComplete?.Invoke();
        }

        protected override void HideVisually(Action onComplete = null)
        {
            HideInstantVisually();
            onComplete?.Invoke();
        }

        protected override void ShowInstantVisually()
        {
            gameObject.SetActive(true);
        }

        protected override void HideInstantVisually()
        {
            gameObject.SetActive(false);
        }

        public override bool IsVisible()
        {
            return gameObject.activeSelf;
        }
    }
}