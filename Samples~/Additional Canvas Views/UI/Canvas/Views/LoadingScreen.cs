using System;
using Core.Services.Purchasing;
using UI.Canvas.ViewBases;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Canvas.Views
{
    public class LoadingScreen : FadingView
    {
        public static ExpectedResult ExpectedScreen = ExpectedResult.None;

        [SerializeField, Range(0f, 1)] private float _fakeLoadTarget = 0.8f;
        [SerializeField] private float _fakeLoadSpeed = 1, _trueLoadSpeed = 1;
        [SerializeField] private Slider _loadingBar;
        private IView _nextScreen;
        private bool _isReady = false;

        protected override void OnAwakeLate()
        {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            IAP.ExecuteOnInit(() => _isReady = true);
        }

        protected override void OnDestroyView()
        {
            SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
        }

        protected override void OnStart()
        {
            ExpectedScreen = ExpectedResult.LoadingScreen;
        }
        
        protected override void OnShowStart()
        {
            base.OnShowStart();
            ExpectedScreen = ExpectedResult.LoadingScreen;
            _loadingBar.value = 0;
        }

        protected override void OnHideStart()
        {
            base.OnHideStart();
            ExpectedScreen = ExpectedResult.None;
        }

        private void Update()
        {
            if (ExpectedScreen is ExpectedResult.MainMenu or ExpectedResult.GameplayUI && _isReady)
            {
                _loadingBar.value = Mathf.Clamp(_loadingBar.value + _trueLoadSpeed * Time.deltaTime, 0, 1);
            }
            else if (ExpectedScreen == ExpectedResult.LoadingScreen || _isReady == false)
            {
                _loadingBar.value = Mathf.Lerp(_loadingBar.value, _fakeLoadTarget, _fakeLoadSpeed * Time.deltaTime);
            }
            
            #warning Edit desired screens to show
            switch (ExpectedScreen)
            {
                case ExpectedResult.MainMenu:
                    if (_loadingBar.value >= 1)
                    {
                        // UIManager.ShowScreenInstant(UIManager.Context.MainMenu);
                        ExpectedScreen = ExpectedResult.None;
                    }
                    break;
                case ExpectedResult.GameplayUI:
                    if (_loadingBar.value >= 1)
                    {
                        // UIManager.ShowScreenInstant(UIManager.Context.GameplayUI);
                        ExpectedScreen = ExpectedResult.None;
                    }
                    break;
            }
        }
        
        private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 1)
            {
                ExpectedScreen = ExpectedResult.MainMenu;
            }
            else if (scene.buildIndex > 1)
            {
                ExpectedScreen = ExpectedResult.GameplayUI;
            }
        }

        [Serializable]
        public enum ExpectedResult
        {
            None = 0,
            LoadingScreen = 1,
            MainMenu = 2,
            GameplayUI = 3,
        }
    }
}