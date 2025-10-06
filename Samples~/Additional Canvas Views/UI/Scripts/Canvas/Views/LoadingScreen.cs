using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UI.Canvas;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace UI.Views
{
    [Serializable]
    public class LoadingScreenParams : UICanvasViewParameters
    {
        [field: SerializeField, Range(0f, 1)] public float FakeLoadTarget { get; private set; } = 0.8f;
        [field: SerializeField] public float FakeLoadSpeed {get; private set; } = 1;
        [field: SerializeField] public float TrueLoadSpeed {get; private set; } = 1;
    }
    
    public class LoadingScreen : UICanvasView<LoadingScreenParams>
    {
        [SerializeField] private Slider _loadingBar;
        [Header("Background switching")]
        [SerializeField] private float _bgChangeTime = 3;
        [SerializeField] private Sprite[] _bgs;
        [SerializeField] private Image _bgHolder;
        [Header("Hint switching")]
        [SerializeField] private float _hintChangeTime = 2;
        [SerializeField] private GameObject[] _hintObjects;
        [Header("Header animation")]
        [SerializeField] private float _dotChangeTime = 1;
        [SerializeField] private LocalizedString _headerLoc;
        [SerializeField] private TMP_Text _headerText;
        private float _dotChangeTimer;
        private int _dotCount;
        private float _hintChangeTimer;
        private int _activeHint = -1;
        private float _bgChangeTimer;
        private int _activeBg = -1;

        public override bool IsVisible() => gameObject.activeSelf;

        protected override void ShowVisually(Action onComplete = null)
        {
            gameObject.SetActive(true);
            onComplete?.Invoke();
        }

        protected override async void HideVisually(Action onComplete = null)
        {
            while (_loadingBar.value < 0.9f)
            {
                _loadingBar.value = Mathf.Clamp(_loadingBar.value + Parameters.TrueLoadSpeed * Time.deltaTime, 0, 1);
                await UniTask.DelayFrame(1);
            }
            gameObject.SetActive(false);
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

        protected override void OnShowStart()
        {
            base.OnShowStart();
            _loadingBar.value = 0;

            _dotChangeTimer = 0;
            _dotCount = 0;
        }

        private void Update()
        {
            _loadingBar.value = Mathf.Lerp(_loadingBar.value, Parameters.FakeLoadTarget, Parameters.FakeLoadSpeed * Time.deltaTime);

            _dotChangeTimer -= Time.deltaTime;
            if (_dotChangeTimer <= 0)
            {
                _dotCount = ++_dotCount % 4;
                _headerText.text = _headerLoc.GetLocalizedString() + new string('.', _dotCount);
                _dotChangeTimer = _dotChangeTime;
            }

            _hintChangeTimer -= Time.deltaTime;
            if (_hintChangeTimer <= 0)
            {
                _hintChangeTimer = _hintChangeTime;
                _activeHint = ++_activeHint % _hintObjects.Length;
                for (var i = 0; i < _hintObjects.Length; i++)
                {
                    _hintObjects[i].SetActive(i == _activeHint);
                }
            }
            
            _bgChangeTimer -= Time.deltaTime;
            if (_bgChangeTimer <= 0)
            {
                _bgChangeTimer = _bgChangeTime;
                _activeBg = ++_activeBg % _bgs.Length;
                _bgHolder.sprite = _bgs[_activeBg];
            }
        }
    }
}