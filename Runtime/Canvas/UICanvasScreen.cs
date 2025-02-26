using System;
using Core.Services.Ads;
using Core.Utilities;
using UnityEngine;

namespace UI.Core.Canvas
{
    [DefaultExecutionOrder(100)]
    public abstract class UICanvasView : MonoBehaviour, IView
    {
        public event EventHandler<ScreenViewVisibilityArgs> VisibilityChanged;

        protected UIManager UIManager
        {
            get
            {
                _uiManager ??= UIManager.Instance;
                _uiManager ??= FindAnyObjectByType<UIManager>();

                return _uiManager;
            }
        }
        protected IViewData ShowData { get; private set; }
        public bool IsInHierarchy => _isInHierarchy;
        public GameObject GameObject => gameObject;
        bool IView.IsObjectAlive => this;

        private Action _onHide;
        private UIManager _uiManager;
        private bool _isInHierarchy;
        private bool _isObjectAlive;

        protected virtual void Awake()
        {
            _isInHierarchy = GetComponentInParent<UIManager>();
            if (!UIManager || _isInHierarchy) 
                return;
            
            HideInstant();
            UIManager.RegisterScreenView(this);
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
            if (UIManager && _isInHierarchy)
                UIManager.UnRegisterScreen(this);
        }
        
        public virtual void Show(Action onComplete = null, Action onHide = null, IViewData data = null)
        {
            ShowData = data;
            OnShowStart();
            _onHide = onHide;
            ShowVisually(() =>
            {
                onComplete?.Invoke();
                OnShown();
                VisibilityChanged?.Invoke(this, new ScreenViewVisibilityArgs(true));
            });
        }

        public void Hide(Action onComplete = null)
        {
            OnHideStart();
            HideVisually(() =>
            {
                onComplete?.Invoke();
                _onHide?.Invoke();
                _onHide = null;
                OnHidden();
                VisibilityChanged?.Invoke(this, new ScreenViewVisibilityArgs(false));
            });
        }

        protected abstract void ShowVisually(Action onComplete = null);
        protected abstract void HideVisually(Action onComplete = null);

        public virtual void ShowInstant(IViewData data = null)
        {
            ShowData = data;
            OnShowStart();
            ShowInstantVisually();
            OnShown();
            VisibilityChanged?.Invoke(this, new ScreenViewVisibilityArgs(true));
        }

        public void HideInstant()
        {
            OnHideStart();
            HideInstantVisually();
            OnHidden();
            VisibilityChanged?.Invoke(this, new ScreenViewVisibilityArgs(false));
        }

        protected abstract void ShowInstantVisually();
        protected abstract void HideInstantVisually();

        protected virtual void OnShowStart()
        {
        }

        protected virtual void OnShown()
        {
        }

        protected virtual void OnHideStart()
        {
        }

        protected virtual void OnHidden()
        {
        }

        protected void BackToPreviousScreen()
        {
            // TODO : Change it stack but create a way to clear latest stack element with most efficiency
            if (UIManager.PreviousMainScreen.IsAlive())
                UIManager.ShowScreenAsync(UIManager.PreviousMainScreen.GetType());
        }
        
        public abstract bool IsVisible();
    }

    public abstract class UICanvasView<T> : UICanvasView where T : UIScreenParameters
    {
        protected T Parameters => _parameters;
        
        [SerializeField] private T _parameters;
        private AutomaticAdRequest _showStartAdRequest;
        
        protected sealed override void Awake()
        {
            OnAwakeEarly();
            _showStartAdRequest = new AutomaticAdRequest(Parameters.CallAdOnShowStart);
            foreach (var behaviour in _parameters.EnableOnStart)
            {
                behaviour.enabled = true;
            }
            base.Awake();
            OnAwakeLate();
        }

        protected sealed override void OnDestroy()
        {
            base.OnDestroy();
            OnDestroyScreen();
        }

        protected sealed override void Start()
        {
            base.Start();
            OnStart();
        }

        public sealed override void Show(Action onComplete = null, Action onHide = null, IViewData data = null)
        {
            _showStartAdRequest?.Invoke();
            base.Show(onComplete, onHide, data);
        }
        
        public sealed override void ShowInstant(IViewData data = null)
        {
            _showStartAdRequest?.Invoke();
            base.ShowInstant(data);
        }
        
        protected virtual void OnAwakeEarly(){}
        protected virtual void OnAwakeLate(){}
        protected virtual void OnStart(){}
        protected virtual void OnDestroyScreen(){}
    }

    [Serializable]
    public abstract class UIScreenParameters
    {
        [field: SerializeField] public PlatformSpecification CallAdOnShowStart { get; private set; } = PlatformSpecification.None;

        [field: SerializeField] public Behaviour[] EnableOnStart { get; private set; } = Array.Empty<Behaviour>();
    }
}