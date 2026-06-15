using System;
using System.Collections;
using Core.Services.Ads;
using Core.Services.Purchasing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.ElementSelection
{
    [DefaultExecutionOrder(-101)]
    [AddComponentMenu("UI/Element Selection/Selectable Element")]
    public class SelectableElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action Clicked = delegate { };
        
        public event SelectionStateChangeDelegate SelectionStateChanged = delegate { };
        public delegate void SelectionStateChangeDelegate(bool selected);
        
        public bool IsSelected { get; private set; }
        public bool IsHovered { get; private set; }
        public bool IsPremiumState => _isInPremiumState;

        public bool Interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                if (_interactable == false)
                    SetSelected(false);
            }
        }

        public bool Selectable
        {
            get => _selectable;
            set => _selectable = value;
        }

        [SerializeField] private bool _interactable = true;
        [SerializeField] private bool _selectable = true;
        [SerializeField] private bool _isPremium;
        [Header("Settings")]
        [SerializeField] private bool _isSelectedByDefault;
        [SerializeField] private bool _unselectOnClick;
        [SerializeField] private SelectableElementUnityEvents _events;
        private ISelectableElementVisualModule[] _modules;
        private Behaviour _payloadScriptCache;
        private SelectableElementGroup _group;
        private bool _isInitialized;
        private bool _isInPremiumState;
        
        private void Awake()
        {
            InitializeIfNeeded();
            SetSelected(_isSelectedByDefault, true);
        }

        private void OnEnable()
        {
            _group ??= GetComponentInParent<SelectableElementGroup>();
            if (_group)
                _group.RegisterElement(this);

            IAP.Initialized += OnIAPEvent;
            IAP.PremiumPurchased += OnIAPEvent;
            UpdatePremiumState();
        }

        private void OnDisable()
        {
            if (_group)
                _group.DeRegisterElement(this);

            IAP.Initialized -= OnIAPEvent;
            IAP.PremiumPurchased -= OnIAPEvent;
        }

        private void OnIAPEvent()
        {
            UpdatePremiumState();
        }

        private void UpdatePremiumState()
        {
            if (!_isPremium)
            {
                _isInPremiumState = false;
                UpdateModulesPremiumState();
                SetSelected(IsSelected, true);
                return;
            }

            bool hasPremium = IAP.IsInitialized && IAP.IsPremiumPurchased();
            _isInPremiumState = !hasPremium;

            UpdateModulesPremiumState();
            
            if (_isInPremiumState)
            {
                // Already updated via UpdateModulesPremiumState
            }
            else
            {
                SetSelected(IsSelected, true);
            }
        }

        private void UpdateModulesPremiumState()
        {
            InitializeIfNeeded();
            foreach (var module in _modules)
            {
                if (module.IsElementAlive)
                    module.OnPremium(_isInPremiumState);
            }
        }

        private void ApplyPremiumVisuals()
        {
            UpdateModulesPremiumState();
        }

        private void InitializeIfNeeded()
        {
            if (_isInitialized)
                return;
            
            _modules = GetComponentsInChildren<ISelectableElementVisualModule>();
            foreach (var module in _modules)
            {
                module.OnInitialized();
            }
            _isInitialized = true;
        }

        private void SetSelected(bool isSelected, bool force)
        {
            InitializeIfNeeded();

            if (force == false && _isInPremiumState)
                return;
            
            if (force == false && IsSelected == isSelected)
                return;

            IsSelected = isSelected;

            if (_isInPremiumState)
            {
                UpdateModulesPremiumState();
            }
            else
            {
                foreach (var module in _modules)
                {
                    if (module.IsElementAlive)
                        module.OnSelectionChanged(isSelected);
                }
            }
            
            SelectionStateChanged?.Invoke(isSelected);
            _events.OnSelectionChanged?.Invoke(isSelected);
        }

        public void SetSelected(bool isSelected) => SetSelected(isSelected, false);

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Interactable == false)
                return;

            if (_isInPremiumState)
            {
                new AutomaticAdRequest(true).Invoke();
                return;
            }

            if (Selectable == false)
            {
                StartCoroutine(InvokeClickedDelayed());
                return;
            }
            
            if (IsSelected)
            {
                if (_unselectOnClick)
                    SetSelected(false);
                
                StartCoroutine(InvokeClickedDelayed());
                return;
            }
            
            SetSelected(true);
            StartCoroutine(InvokeClickedDelayed());
        }

        private IEnumerator InvokeClickedDelayed()
        {
            if (_isInPremiumState)
            {
                new AutomaticAdRequest(true).Invoke();
                yield break;
            }

            InitializeIfNeeded();
            float maxDuration = 0;
            foreach (var module in _modules)
            {
                if (module != null && module.IsElementAlive)
                {
                    maxDuration = Mathf.Max(maxDuration, module.GetAnimationDuration());
                }
            }

            if (maxDuration > 0)
            {
                yield return new WaitForSecondsRealtime(maxDuration);
            }

            Clicked?.Invoke();
            _events.OnPointerClick?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Interactable == false)
                return;

            IsHovered = true;

            if (_isInPremiumState)
                return;

            foreach (var module in _modules)
            {
                if (module.IsElementAlive)
                    module.OnHoverChanged(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsHovered = false;

            if (_isInPremiumState)
                return;

            foreach (var module in _modules)
            {
                if (module.IsElementAlive)
                    module.OnHoverChanged(false);
            }
        }

        public bool TryGetPayload<T>(out T payloadScript)
            where T : Behaviour
        {
            InitializeIfNeeded();
            
            if (_payloadScriptCache is T cachedPayload)
            {
                payloadScript = cachedPayload;
                return true;
            }

            var script = GetComponent<T>();
            if (script != null)
            {
                payloadScript = script;
                _payloadScriptCache = script;
                return true;
            }

            script = GetComponentInChildren<T>();
            if (script != null)
            {
                payloadScript = script;
                _payloadScriptCache = script;
                return true;
            }

            payloadScript = default;
            return false;
        }

        public T GetPayload<T>()
            where T : Behaviour
        {
            InitializeIfNeeded();
            
            if (TryGetPayload(out T payload))
                return payload;

            return default;
        }
    }

    [Serializable]
    public class SelectableElementUnityEvents
    {
        [field: SerializeField] public UnityEvent<bool> OnSelectionChanged { get; private set; } = new();
        [field: SerializeField] public UnityEvent OnPointerClick { get; private set; } = new();
    }
}