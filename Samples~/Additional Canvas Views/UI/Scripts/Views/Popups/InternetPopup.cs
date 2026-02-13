#if UNITY_WEBGL
using UnifiedTask = Cysharp.Threading.Tasks.UniTask;
#else
using UnifiedTask = System.Threading.Tasks.Task;
#endif
using UI.Canvas.ViewBases;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Canvas.Views.Popups
{
    public class InternetPopup : FadingView
    {
        [SerializeField] private Button _closeBtn;

        protected override void OnAwakeLate()
        {
            if (_closeBtn)
                _closeBtn.onClick.AddListener(() => Hide());
            CheckForInternet();
        }

        private async void CheckForInternet()
        {
            while (Application.isPlaying)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable && IsVisible() == false)
                {
                    Show();
                    
                    await UnifiedTask.Delay(Mathf.FloorToInt(Parameters.AppearanceTime * 1000) + 100);
                }
                await UnifiedTask.Delay(100);
            }
        }
    }
}