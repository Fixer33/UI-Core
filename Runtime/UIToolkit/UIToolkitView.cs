using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
#if USE_LOCALIZATION_PACKAGE
using UnityEngine.Localization;
#endif

namespace UI.Core.UIToolkit
{
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIToolkitView : MonoBehaviour, IView
    {
        public event EventHandler<ScreenViewVisibilityArgs> VisibilityChanged;

        public UIManager UIManager
        {
            get
            {
                if (_uiManager == null)
                {
                    _uiManager = UIManager.Instance;
                }

                return _uiManager;
            }
        }
        public UIDocument Document
        {
            get
            {
                if (_document == null)
                {
                    _document = GetComponent<UIDocument>();
                }

                return _document;
            }
        }
        protected IViewData ShowData { get; private set; }

        public bool IsInHierarchy => _isInHierarchy;
        public GameObject GameObject => gameObject;

        bool IView.IsObjectAlive => this;

        [SerializeField] private float _animationTime = 0;
        [Inject] private StyleSheet _platformSpecificStyleSheet;
        private bool _isInHierarchy;
        private Action _onHide;
        private UIManager _uiManager;
        private UIDocument _document;
        private (FieldInfo field, string queryName)[] _fieldsToQuery;
        private (FieldInfo field, bool isSupported, bool useVisibility)[] _platformSpecificFields;
        private bool _isInitialized;
        private Action _onShown;
        private bool _isObjectAlive;

        protected void Awake()
        {
            _isInHierarchy = GetComponentInParent<UIManager>();
            if (!UIManager || _isInHierarchy) 
                return;
            
            HideInstant();
            UIManager.RegisterScreenView(this);
            
            _fieldsToQuery ??= GetElementsToQuery();
            _platformSpecificFields ??= GetPlatformSpecificFields();
            _isInitialized = true;
        }

        protected virtual void Start()
        {
            
        }

        private void OnEnable()
        {
            if (_isInitialized == false)
                return;
            
            if (_platformSpecificStyleSheet != null)
                Document.rootVisualElement.styleSheets.Add(_platformSpecificStyleSheet);

            OnEnablePreQuery();
            
            QueryElements();
            HandlePlatformSpecificElements();

#if USE_LOCALIZATION_PACKAGE
            OnEnablePreLocalization();
            foreach (var item in GetStaticLocalizations())
            {
                item.Localizable.InitLocalization(item.Localization);
            }
#endif
            
            OnEnableLocal();
            ApplyAnimationStyles();
            Invoke(nameof(AnimationCompleted), _animationTime);
        }

        private void AnimationCompleted()
        {
            _onShown?.Invoke();
            _onShown = null;
            OnShown();
        }

        private (FieldInfo field, string name)[] GetElementsToQuery()
        {
            List<(FieldInfo field, string name)> res = new();
            var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            
            foreach (var field in fields)
            {
                var queryAttribute = field.GetCustomAttribute<ElementQuery>();
                if (queryAttribute == null) 
                    continue;

                string queryName = queryAttribute.GetName(field);
                
                var element = Document.rootVisualElement.Q(queryName);
                if (element != null && field.FieldType.IsAssignableFrom(element.GetType()))
                {
                    field.SetValue(this, element);
                    res.Add((field, queryName));
                }
                else
                {
                    Debug.LogError($"Element '{queryAttribute.ElementName}' not found or incompatible with field '{field.Name}'");
                }
            }

            return res.ToArray();
        }

        private bool FieldsToQueryContainsField(FieldInfo field)
        {
            if (_fieldsToQuery == null)
                return false;

            for (var i = 0; i < _fieldsToQuery.Length; i++)
            {
                if (_fieldsToQuery[i].field.Equals(field))
                    return true;
            }

            return false;
        }
        
        private (FieldInfo field, bool isSupported, bool useVisibility)[] GetPlatformSpecificFields()
        {
            List<(FieldInfo field, bool isSupported, bool useVisibility)> res = new();
            var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            
            foreach (var field in fields)
            {
                var queryAttribute = field.GetCustomAttribute<ForPlatforms>();
                if (queryAttribute == null) 
                    continue;
                
                if (field.FieldType.IsSubclassOf(typeof(VisualElement)) == false &&
                    field.FieldType.IsAssignableFrom(typeof(VisualElement)) == false)
                {
                    Debug.LogError($"Platform specific attribute is incompatible with field '{field.Name}'");
                    continue;
                }

                if (FieldsToQueryContainsField(field) == false)
                {
                    Debug.LogError($"Field '{field.Name}' is not queried! Platform specific attribute is useless on this field");
                    continue;
                }

                res.Add((field, queryAttribute.IsValidForCurrentPlatform(), queryAttribute.UseVisibility));
            }

            return res.ToArray();
        }

        private void QueryElements()
        {
            _fieldsToQuery ??= GetElementsToQuery();

            foreach (var queryInfo in _fieldsToQuery)
            {
                queryInfo.field.SetValue(this, Document.rootVisualElement.Q(queryInfo.queryName));
            }
        }
        
        private void HandlePlatformSpecificElements()
        {
            _platformSpecificFields ??= GetPlatformSpecificFields();

            foreach (var platformSpecificField in _platformSpecificFields)
            {
                if (platformSpecificField.field.GetValue(this) is not VisualElement visualElement)
                    continue;
                
                if (platformSpecificField.isSupported)
                    continue;

                if (platformSpecificField.useVisibility)
                    visualElement.visible = false;
                else
                    visualElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
        }

        public void Show(Action onShown = null, Action onHide = null, IViewData data = null)
        {
            ShowData = data;
            _onHide = onHide;
            _onShown = onShown;
            gameObject.SetActive(true);
            OnShowStart();
            // OnShown();
            VisibilityChanged?.Invoke(this, new ScreenViewVisibilityArgs(true));
        }

        public void Hide(Action onComplete = null)
        {
            OnHideStart();
            gameObject.SetActive(false);
            onComplete?.Invoke();
            onComplete?.Invoke();
            _onHide?.Invoke();
            _onHide = null;
            OnHidden();
            VisibilityChanged?.Invoke(this, new ScreenViewVisibilityArgs(false));
        }

        public void ShowInstant(IViewData data = null)
        {
            ShowData = data;
            gameObject.SetActive(true);
            VisibilityChanged?.Invoke(this, new ScreenViewVisibilityArgs(true));
        }

        public void HideInstant()
        {
            gameObject.SetActive(false);
            VisibilityChanged?.Invoke(this, new ScreenViewVisibilityArgs(false));
        }

        protected virtual void ApplyAnimationStyles() {}
        protected virtual void OnShowStart() {}
        protected virtual void OnShown() {}
        protected virtual void OnHideStart() {}
        protected virtual void OnHidden() {}
        protected virtual void OnEnablePreQuery(){}
        protected virtual void OnEnableLocal(){}
        
        public bool IsVisible() => gameObject.activeSelf;

#if USE_LOCALIZATION_PACKAGE
        protected virtual void OnEnablePreLocalization(){}
        protected abstract LocalizationPair[] GetStaticLocalizations();
        protected struct LocalizationPair
        {
            public ILocalizable Localizable;
            public LocalizedString Localization;

            public LocalizationPair(ILocalizable localizable, LocalizedString localization)
            {
                Localizable = localizable;
                Localization = localization;
            }
        }
#endif
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ElementQuery : Attribute
    {
        public readonly string ElementName;

        public ElementQuery()
        {
            ElementName = null;
        }

        public ElementQuery(string elementName)
        {
            ElementName = elementName;
        }

        public string GetName(FieldInfo field)
        {
            if (string.IsNullOrEmpty(ElementName))
            {
                string name = field.Name;
                if (name.Length < 2)
                    return field.Name;

                if (name[0] == '_')
                {
                    string firstCapital = name[1].ToString().ToUpper();
                    name = name.Substring(2, name.Length - 2);
                    name = firstCapital + name;
                }

                return name;
            }

            return ElementName;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class ForPlatforms : Attribute
    {
        public readonly PlatformSpecification Platforms;
        public readonly bool UseVisibility;

        public ForPlatforms(PlatformSpecification platforms) : this(platforms, false)
        {
            
        }
        
        public ForPlatforms(PlatformSpecification platforms, bool useVisibility)
        {
            Platforms = platforms;
            UseVisibility = useVisibility;
        }

        public bool IsValidForCurrentPlatform()
        {
            return PlatformUtility.IsForCurrentPlatform(Platforms);
        }
    }
}