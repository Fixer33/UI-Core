using UI.Canvas.ViewBases;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Canvas.Views.Popups
{
    public class QuitPopup : FadingView
    {
        [SerializeField] private Button _yesBtn, _noBtn;

        protected override void OnAwakeLate()
        {
            _yesBtn.onClick.AddListener(YesPressed);
            _noBtn.onClick.AddListener(NoPressed);
        }

        private void NoPressed()
        {
            Hide();
        }

        private void YesPressed()
        {
            Application.Quit();
        }
    }
}