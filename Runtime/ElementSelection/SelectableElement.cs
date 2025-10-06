using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.ElementSelection
{
    [DefaultExecutionOrder(-101)]
    [AddComponentMenu("UI/Element Selection/Selectable Element")]
    public class SelectableElement : MonoBehaviour, IPointerClickHandler
    {
        public event Action Clicked = delegate { };
        
        public event SelectionStateChangeDelegate SelectionStateChanged = delegate { };
        public delegate void SelectionStateChangeDelegate(bool selected);
        
        public bool IsSelected { get; private set; }

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
        [Header("Settings")]
        [SerializeField] private bool _isSelectedByDefault;
        [SerializeField] private bool _unselectOnClick;
        [SerializeField] private SelectableElementUnityEvents _events;
        private ISelectableElementVisualModule[] _modules;
        private Behaviour _payloadScriptCache;
        private SelectableElementGroup _group;
        
        private void Awake()
        {
            _modules = GetComponentsInChildren<ISelectableElementVisualModule>();
            foreach (var module in _modules)
            {
                module.OnInitialized();
            }
            SetSelected(_isSelectedByDefault, true);
        }

        private void OnEnable()
        {
            _group ??= GetComponentInParent<SelectableElementGroup>();
            if (_group)
                _group.RegisterElement(this);
        }

        private void OnDisable()
        {
            if (_group)
                _group.DeRegisterElement(this);
        }

        private void SetSelected(bool isSelected, bool force)
        {
            if (force == false && IsSelected == isSelected)
                return;

            IsSelected = isSelected;
            foreach (var module in _modules)
            {
                if (module.IsElementAlive)
                    module.OnSelectionChanged(isSelected);
            };
            
            SelectionStateChanged?.Invoke(isSelected);
            _events.OnSelectionChanged?.Invoke(isSelected);
        }

        public void SetSelected(bool isSelected) => SetSelected(isSelected, false);

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Interactable == false)
                return;

            if (Selectable == false)
            {
                Clicked?.Invoke();
                _events.OnPointerClick?.Invoke();
                return;
            }
            
            if (IsSelected)
            {
                if (_unselectOnClick)
                    SetSelected(false);
                
                Clicked?.Invoke();
                return;
            }
            
            SetSelected(true);
            Clicked?.Invoke();
            _events.OnPointerClick?.Invoke();
        }

        public bool TryGetPayload<T>(out T payloadScript)
            where T : Behaviour
        {
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