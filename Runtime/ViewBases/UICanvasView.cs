using System;
using System.Collections.Generic;
using Core.Services.Ads;
using Core.Utilities;
using UI.ViewExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.ViewBases
{
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(Canvas))]
    public abstract class UICanvasView : MonoBehaviour, IView
    {
        public event EventHandler<ViewVisibilityArgs> VisibilityChanged;

        public event Action ShowStart;
        public event Action Shown;
        public event Action HideStart;
        public event Action Hidden;

        protected UIManager UIManager
        {
            get
            {
                _uiManager ??= UIManager.Instance;
                _uiManager ??= FindAnyObjectByType<UIManager>();

                return _uiManager;
            }
        }
        protected Canvas Canvas => _canvas ??= GetComponent<Canvas>();
        protected IViewData ViewData { get; private set; }
        public bool IsInHierarchy => _isInHierarchy;
        public GameObject GameObject => gameObject;
        bool IView.IsObjectAlive => this;

        private Action _onHide;
        private UIManager _uiManager;
        private bool _isInHierarchy;
        private bool _isObjectAlive;
        private object _viewDataCached;
        private Canvas _canvas;
        private IViewExtension[] _extensions;

        protected virtual void Awake()
        {
            _extensions = GetComponents<IViewExtension>();
            List<(Button button, UnityAction onClick)> buttonCallbacks = new();
            SubscribeAttributeButtons(buttonCallbacks);
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

        private void SubscribeAttributeButtons(List<(Button button, UnityAction onClick)> callbacks)
        {
            var type = GetType();
            var fields = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(ViewShowButtonAttribute)) as ViewShowButtonAttribute;
                if (attribute == null) continue;

                if (field.FieldType != typeof(Button))
                {
                    Debug.LogError($"Field {field.Name} in {type.Name} has ViewShowButtonAttribute but is not a Button.");
                    continue;
                }

                if (attribute.ViewType == null || !typeof(IView).IsAssignableFrom(attribute.ViewType))
                {
                    // Already logged in attribute constructor but checking again here for safety
                    continue;
                }

                var button = field.GetValue(this) as Button;
                if (button == null)
                {
                    Debug.LogWarning($"Button field {field.Name} in {type.Name} is null. ViewShowButtonAttribute will not work.");
                    continue;
                }

                callbacks.Add((button, () => UIManager.ShowViewAsync(attribute.ViewType)));
            }
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
            if (UIManager)
                UIManager.UnRegisterView(this);
        }
        
        public virtual void Show(Action onComplete = null, Action onHide = null, IViewData data = null)
        {
            ViewData = data;
            if (data != null)
                _viewDataCached = data;
            
            NotifyShowStart();
            ShowStart?.Invoke();
            
            _onHide = onHide;
            ShowVisually(() =>
            {
                onComplete?.Invoke();
                
                NotifyShown();
                
                Shown?.Invoke();
                VisibilityChanged?.Invoke(this, new ViewVisibilityArgs(true));
            });
        }

        public void Hide(Action onComplete = null)
        {
            NotifyHideStart();
            
            HideStart?.Invoke();
            HideVisually(() =>
            {
                onComplete?.Invoke();
                _onHide?.Invoke();
                _onHide = null;
                
                NotifyHidden();
                
                Hidden?.Invoke();
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
            
            NotifyShowStart();
            ShowInstantVisually();
            NotifyShown();
            
            VisibilityChanged?.Invoke(this, new ViewVisibilityArgs(true));
        }

        public void HideInstant()
        {
            NotifyHideStart();
            HideInstantVisually();
            NotifyHidden();
            
            VisibilityChanged?.Invoke(this, new ViewVisibilityArgs(false));
        }

        protected abstract void ShowInstantVisually();
        protected abstract void HideInstantVisually();

        private void NotifyShowStart()
        {
            OnShowStart();
            if (_extensions == null) return;
            foreach (var extension in _extensions)
                extension.ShowStart();
        }

        private void NotifyShown()
        {
            OnShown();
            if (_extensions == null) return;
            foreach (var extension in _extensions)
                extension.Shown();
        }

        private void NotifyHideStart()
        {
            OnHideStart();
            if (_extensions == null) return;
            foreach (var extension in _extensions)
                extension.HideStart();
        }

        private void NotifyHidden()
        {
            OnHidden();
            if (_extensions == null) return;
            foreach (var extension in _extensions)
                extension.Hidden();
        }

        protected virtual void OnShowStart() {}
        protected virtual void OnShown() {}
        protected virtual void OnHideStart() {}
        protected virtual void OnHidden() {}

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

        protected void SetActive(bool isActive)
        {
            Canvas.enabled = isActive;
            gameObject.SetActive(isActive);
        }
        
        public virtual bool IsVisible() => gameObject.activeSelf && Canvas.enabled;
    }

    public abstract class UICanvasView<T> : UICanvasView where T : UICanvasViewParameters
    {
        protected T Parameters => _parameters;
        
        [SerializeField] private T _parameters;
        private AutomaticAdRequest _showStartAdRequest;
        
        protected sealed override void Awake()
        {
            VisibilityChanged += (sender, args) => Parameters.InvokeUnityEvent(Parameters.UnityEvents.OnVisibilityChanged, args.IsVisible);
            Shown += () => Parameters.InvokeUnityEvent(Parameters.UnityEvents.OnShown);
            Hidden += () => Parameters.InvokeUnityEvent(Parameters.UnityEvents.OnHidden);
            ShowStart += () => Parameters.InvokeUnityEvent(Parameters.UnityEvents.OnShowStart);
            HideStart += () => Parameters.InvokeUnityEvent(Parameters.UnityEvents.OnHideStart);
            
            OnAwakeEarly();
            _showStartAdRequest = new AutomaticAdRequest(Parameters.CallAdOnShowStart);
            foreach (var behaviour in _parameters.EnableOnStart)
            {
                if (behaviour)
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
        [field: SerializeField] public bool CallUnityEvents { get; private set; } = false;
        [field: SerializeField] public UICanvasViewUnityEvents UnityEvents { get; private set; }
        
        internal void InvokeUnityEvent(UnityEvent unityEvent)
        {
            if (!CallUnityEvents) 
                return;
            
            unityEvent?.Invoke();
        }
        
        internal void InvokeUnityEvent(UnityEvent<bool> unityEvent, bool value)
        {
            if (!CallUnityEvents) 
                return;
            
            unityEvent?.Invoke(value);
        }
    }

    [Serializable]
    public struct UICanvasViewUnityEvents
    {
        public UnityEvent OnShowStart;
        public UnityEvent OnShown;
        public UnityEvent OnHideStart;
        public UnityEvent OnHidden;
        
        public UnityEvent<bool> OnVisibilityChanged;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ViewShowButtonAttribute : Attribute
    {
        public Type ViewType { get; }

        public ViewShowButtonAttribute(Type viewType)
        {
            if (viewType == null)
            {
                Debug.LogError("ViewType cannot be null in ViewShowButtonAttribute.");
                return;
            }

            if (!typeof(IView).IsAssignableFrom(viewType))
            {
                Debug.LogError($"Type {viewType.Name} does not implement IView. ViewShowButtonAttribute requires a type that implements IView.");
            }

            ViewType = viewType;
        }
    }
}