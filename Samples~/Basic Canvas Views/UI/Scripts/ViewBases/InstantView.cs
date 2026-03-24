using System;

namespace UI.ViewBases
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
            SetActive(true);
        }

        protected override void HideInstantVisually()
        {
            SetActive(false);
        }
    }
}