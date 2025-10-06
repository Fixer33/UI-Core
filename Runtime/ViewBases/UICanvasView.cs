using System;
using System.Collections.Generic;
using Core.Services.Ads;
using Core.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Canvas
{
    [DefaultExecutionOrder(100)]
    public abstract class UICanvasView : MonoBehaviour, IView
    {
        public event EventHandler<ViewVisibilityArgs> VisibilityChanged;

        protected UIManager UIManager
        {
            get
            {
                _uiManager ??= UIManager.Instance;
                _uiManager ??= FindAnyObjectByType<UIManager>();

                return _uiManager;
            }
        }
        protected IViewData ViewData { get; private set; }
        public bool IsInHierarchy => _isInHierarchy;
        public GameObject GameObject => gameObject;
        bool IView.IsObjectAlive => this;

        private Action _onHide;
        private UIManager _uiManager;
        private bool _isInHierarchy;
        private bool _isObjectAlive;
        private object _viewDataCached;

        protected virtual void Awake()
        {
            List<(Button button, UnityAction onClick)> buttonCallbacks = new();
            AddButtonSubscriptions(buttonCallbacks);
            foreach (var buttonCallbackRecord in buttonCallbacks)
            {
                if (buttonCallbackRecord.button)
                    buttonCallbackRecord.button.onClick.AddListener(buttonCallbackRecord.onClick);
            }
            
            _isInHierarchy = GetComponentInParent<UIManager>();
            if (!UIManager || _isInHierarchy) 
                return;
            
            HideInstant();
            UIManager.RegisterView(this);
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
            if (UIManager && _isInHierarchy)
                UIManager.UnRegisterView(this);
        }
        
        public virtual void Show(Action onComplete = null, Action onHide = null, IViewData data = null)
        {
            ViewData = data;
            if (data != null)
                _viewDataCached = data;
            OnShowStart();
            _onHide = onHide;
            ShowVisually(() =>
            {
                onComplete?.Invoke();
                OnShown();
                VisibilityChanged?.Invoke(this, new ViewVisibilityArgs(true));
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
                VisibilityChanged?.Invoke(this, new ViewVisibilityArgs(false));
            });
        }

        protected abstract void ShowVisually(Action onComplete = null);
        protected abstract void HideVisually(Action onComplete = null);

        public virtual void ShowInstant(IViewData data = null)
        {
            ViewData = data;
            if (data != null)
                _viewDataCached = data;
            OnShowStart();
            ShowInstantVisually();
            OnShown();
            VisibilityChanged?.Invoke(this, new ViewVisibilityArgs(true));
        }

        public void HideInstant()
        {
            OnHideStart();
            HideInstantVisually();
            OnHidden();
            VisibilityChanged?.Invoke(this, new ViewVisibilityArgs(false));
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

        protected virtual void AddButtonSubscriptions(List<(Button button, UnityAction onClick)> callbacks){}

        protected void BackToPreviousView()
        {
            // TODO : Change it stack but create a way to clear latest stack element with most efficiency
            if (UIManager.PreviousMainView.IsAlive())
                UIManager.ShowViewAsync(UIManager.PreviousMainView.GetType());
        }

        protected T GetViewData<T>(bool useCached = true)
        {
            if (ViewData != null) 
                return ViewData.Get<T>();
            
            if (useCached && _viewDataCached is T dataCached)
                return dataCached;
            
            return default;
        }
        
        public abstract bool IsVisible();
    }

    public abstract class UICanvasView<T> : UICanvasView where T : UICanvasViewParameters
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
            OnDestroyView();
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
        protected virtual void OnDestroyView(){}
    }

    [Serializable]
    public abstract class UICanvasViewParameters
    {
        [field: SerializeField] public PlatformSpecification CallAdOnShowStart { get; private set; } = PlatformSpecification.None;

        [field: SerializeField] public Behaviour[] EnableOnStart { get; private set; } = Array.Empty<Behaviour>();
    }
}