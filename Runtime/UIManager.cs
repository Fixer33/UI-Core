#if UNITY_WEBGL
using UnifiedTask = Cysharp.Threading.Tasks.UniTask;
#else
using UnifiedTask = System.Threading.Tasks.Task;
#endif
using System;
using System.Collections.Generic;
using System.Text;
using Core.Utilities;
using UnityEngine;

namespace UI
{
    public enum VisibilityChangeType
    {
        Regular,
        Instant
    }

    public enum UILayer
    {
        Main = 0,
        Custom1 = 1,
        Custom2 = 2,
        Popups = 3,
        Custom3 = 4,
        Custom4 = 5,
    }
    
    public class UIManager : SingletonBehaviour<UIManager>
    {
        public event EventHandler<ViewVisibilityArgs> ViewVisibilityChanged;
        
        public IView PreviousMainView { get; private set; }
        private static Type _startViewType;

        private readonly Dictionary<Type, IView> _singleViewDict = new();
        private readonly Dictionary<Type, Dictionary<int, IView>> _multipleViewDict = new();
        private readonly Dictionary<UILayer, IView> _activeLayers = new();
        private bool _canBeShown = true;
        
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                _activeLayers.Add(layer, null);
            }
            PreviousMainView = null;
        }

        private void Start()
        {
            foreach (var view in GetComponentsInChildren<IView>())
            {
                if (view.GetType() != _startViewType)
                    view.HideInstant();
                else
                    view.ShowInstant();

                RegisterView(view);
            }

            _activeLayers[UILayer.Main] = GetView(_startViewType);
        }

        public void RegisterView(IView view)
        {
            var type = view.GetType();

            view.VisibilityChanged += OnViewVisibilityChanged;

            // Register view if it allows multiple instances
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (view is IMultipleViewInstance multipleViewInstance)
            {
                Dictionary<int, IView> viewTypeDict;
                if (_multipleViewDict.TryGetValue(type, out viewTypeDict) == false)
                    _multipleViewDict.Add(type, viewTypeDict = new Dictionary<int, IView>());

                int viewId = multipleViewInstance.GetMultipleViewId();
                if (viewTypeDict.TryAdd(viewId, view) == false)
                    Debug.LogError(
                        $"Failed to register view {view.GameObject.name}({type.Name}) as multiple view with id {viewId}!\n" +
                        $"Instance with such id already exists: {viewTypeDict[viewId].GameObject.name}");

                _multipleViewDict[type] = viewTypeDict;
                return;
            }

            // Register view as regular single instance
            if (_singleViewDict.TryAdd(type, view) == false)
                Debug.LogError($"Failed to register view {view.GameObject.name}({type.Name}) as regular!\n" +
                               $"Instance with such id already exists: {_singleViewDict[type].GameObject.name}");
        }

        public void UnRegisterView(IView view)
        {
            var type = view.GetType();
            
            view.VisibilityChanged -= OnViewVisibilityChanged;
            
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (view is IMultipleViewInstance multipleViewInstance)
            {
                int instanceId = multipleViewInstance.GetMultipleViewId();
                if (_multipleViewDict.TryGetValue(type, out var instanceDict) == false || instanceDict.ContainsKey(instanceId) == false)
                {
                    Debug.LogError($"Failed to unregister view {view.GameObject.name}({type.Name}). No instances registered");
                    return;
                }

                instanceDict.Remove(instanceId);
                _multipleViewDict[type] = instanceDict;
                return;
            }

            if (_singleViewDict.ContainsKey(type) == false)
            {
                Debug.LogError($"Failed to unregister view {view.GameObject.name}({type.Name}). No instance registered");
                return;
            }

            _singleViewDict.Remove(type);
        }

        private void OnViewVisibilityChanged(object sender, ViewVisibilityArgs viewVisibilityArgs)
        {
            ViewVisibilityChanged?.Invoke(sender, viewVisibilityArgs);
        }

        public T GetView<T>(int? multipleViewInstanceId = null) where T : IView => (T)GetView(typeof(T), multipleViewInstanceId);

        public IView GetView(Type viewType, int? multipleViewInstanceId = null)
        {
            if (viewType.GetInterface(nameof(IMultipleViewInstance)) == null)
                return _singleViewDict.GetValueOrDefault(viewType);

            if (multipleViewInstanceId.HasValue == false)
            {
                Debug.LogError($"Failed to get view of type {viewType.Name}! No view id supplied!");
                return null;
            }

            if (_multipleViewDict.TryGetValue(viewType, out var instanceDict) == false)
                return null;
                
            return instanceDict.GetValueOrDefault(multipleViewInstanceId.Value);
        }

        public async void ShowViewAsync(Type viewType,
            VisibilityChangeType activeViewHideType = VisibilityChangeType.Regular,
            VisibilityChangeType newViewShowType = VisibilityChangeType.Regular,
            Action activeViewHidden = null,
            Action newViewShown = null,
            Action onError = null,
            UILayer layer = UILayer.Main,
            IViewData showData = null,
            int? multipleViewInstanceId = null
        )
        {
            void HideActiveView(IView activeView, Action callback)
            {
                activeView.Hide(() =>
                {
                    activeViewHidden?.Invoke();
                    callback?.Invoke();
                });
            }
            void ShowNewView(IView view, Action callback)
            {
                view.Show(callback, data: showData);
            }
            void OnLayerViewVisibilityChange(object sender, ViewVisibilityArgs viewVisibilityArgs)
            {
                if (viewVisibilityArgs.IsVisible)
                    return;
                
                if (sender is IView view)
                    view.VisibilityChanged -= OnLayerViewVisibilityChange;

                UILayer? layer = null;
                foreach (var keyValuePair in _activeLayers)
                {
                    if (keyValuePair.Value != sender)
                        continue;

                    layer = keyValuePair.Key;
                    break;
                }
                
                if (layer.HasValue == false)
                    return;

                _activeLayers[layer.Value] = null;
            }
            
            var view = GetView(viewType, multipleViewInstanceId);
            if (view == null)
            {
                string multipleInstanceIdS =
                    multipleViewInstanceId.HasValue ? multipleViewInstanceId.Value.ToString() : "null";
                Debug.LogError($"Failed to show new view! View of type {viewType.Name} is not registered!\n" +
                               $"Supplied multipleInstanceId: {multipleInstanceIdS}");
                onError?.Invoke();
                return;
            }
            
            await WaitUntilCanBeShown();

            view.VisibilityChanged += OnLayerViewVisibilityChange;
            
            var activeOnLayer = _activeLayers.GetValueOrDefault(layer, null);
            _canBeShown = false;
            if (activeOnLayer.IsAlive() == false) // In case if active view was destroyed
            {
                if (layer == UILayer.Main)
                    Debug.LogWarning("Active main view has been destroyed before switching to a new one!");
                if (newViewShowType == VisibilityChangeType.Instant)
                {
                    _activeLayers[layer] = view;
                    view.ShowInstant(data: showData);
                    _canBeShown = true;
                    newViewShown?.Invoke();
                    return;
                }
                
                _activeLayers[layer] = view;
                ShowNewView(view, () =>
                {
                    _canBeShown = true;
                    newViewShown?.Invoke();
                });
                return;
            }
            
            // Hide all higher layers
            for (int i = (int)layer + 1; i < _activeLayers.Count; i++)
            {
                UILayer layerToHide = (UILayer)i;
                if (_activeLayers[layerToHide].IsAlive() == false)
                    continue;
                
                if (_activeLayers[layerToHide].IsVisible() == false)
                    return;

                _activeLayers[layerToHide].VisibilityChanged -= OnLayerViewVisibilityChange;
                if (activeViewHideType == VisibilityChangeType.Regular)
                    _activeLayers[layerToHide].Hide();
                else
                    _activeLayers[layerToHide].HideInstant();

                _activeLayers[layerToHide] = null;
            }
            
            activeOnLayer.VisibilityChanged -= OnLayerViewVisibilityChange;
            switch (activeViewHideType: activeViewHideType, newViewShowType: newViewShowType)
            {
                case (VisibilityChangeType.Regular, VisibilityChangeType.Regular):
                    HideActiveView(activeOnLayer, () =>
                    {
                        if (layer == UILayer.Main)
                            PreviousMainView = activeOnLayer;
                        _activeLayers[layer] = view;
                        ShowNewView(view, () =>
                        {
                            _canBeShown = true;
                            newViewShown?.Invoke();
                        });
                    });
                    break;
                case (VisibilityChangeType.Regular, VisibilityChangeType.Instant):
                    HideActiveView(activeOnLayer, () =>
                    {
                        if (layer == UILayer.Main)
                            PreviousMainView = activeOnLayer;
                        _activeLayers[layer] = view;
                        view.ShowInstant(data: showData);
                        _canBeShown = true;
                        newViewShown?.Invoke();
                    });
                    break;
                case (VisibilityChangeType.Instant, VisibilityChangeType.Regular):
                    if (layer == UILayer.Main)
                        PreviousMainView = activeOnLayer;
                    activeOnLayer.HideInstant();
                    _activeLayers[layer] = view;
                    ShowNewView(view, () =>
                    {
                        _canBeShown = true;
                        newViewShown?.Invoke();
                    });
                    break;
                case (VisibilityChangeType.Instant, VisibilityChangeType.Instant):
                    if (layer == UILayer.Main)
                        PreviousMainView = activeOnLayer;
                    activeOnLayer.HideInstant();
                    _activeLayers[layer] = view;
                    view.ShowInstant(data: showData);
                    _canBeShown = true;
                    newViewShown?.Invoke();
                    break;
            }
        }

        public void ShowViewAsync<T>(
            VisibilityChangeType activeViewHideType = VisibilityChangeType.Regular,
            VisibilityChangeType newViewShowType = VisibilityChangeType.Regular,
            Action activeViewHidden = null,
            Action newViewShown = null,
            Action onError = null,
            UILayer layer = UILayer.Main,
            IViewData showData = null,
            int? multipleViewInstanceId = null
        ) where T : IView => ShowViewAsync(typeof(T), 
            activeViewHideType, newViewShowType, activeViewHidden, newViewShown, onError, layer, showData, multipleViewInstanceId);

        public void ShowViewAsPopupAsync<T>(
            Action onShown = null,
            Action onError = null,
            IViewData showData = null,
            int? multipleViewInstanceId = null
        ) where T : IView => ShowViewAsync<T>(newViewShown: onShown, onError: onError, showData: showData, layer: UILayer.Popups,
            multipleViewInstanceId: multipleViewInstanceId);
        
        public async UnifiedTask WaitUntilCanBeShown()
        {
            while (_canBeShown == false)
            {
                await UnifiedTask.Delay(100);
            }
        }
        
        public async UnifiedTask ExecuteWhenCanBeShown(Action callback)
        {
            await WaitUntilCanBeShown();
            callback?.Invoke();
        }

        public static UIManager Create(Type startViewType, params GameObject[] viewPrefabs)
        {
            GameObject uiObj = new GameObject("UI");
            _startViewType = startViewType;
            var result = uiObj.AddComponent<UIManager>();
            
            foreach (var viewPrefab in viewPrefabs)
            {
                var spawnedView = Instantiate(viewPrefab, uiObj.transform);
            }

            return result;
        }
        
#if UNITY_EDITOR
        [ContextMenu("Print all registered views")]
        private void EDITOR_PrintAllRegisteredViews()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("All registered views:");
            sb.AppendLine("=Single instance views:");
            foreach (var singleViewRecord in _singleViewDict)
            {
                sb.Append("=== ");
                sb.Append(singleViewRecord.Value.GameObject.name);
                sb.Append("(");
                sb.Append(singleViewRecord.Key.Name);
                sb.Append("). Is in hierarchy: ");
                sb.AppendLine(singleViewRecord.Value.IsInHierarchy.ToString());
            }

            sb.AppendLine("=Multiple instance views:");
            foreach (var multipleViewsRecord in _multipleViewDict)
            {
                sb.Append("=== ");
                sb.Append("Views of type ");
                sb.AppendLine(multipleViewsRecord.Key.Name);
                
                foreach (var viewInstance in multipleViewsRecord.Value)
                {
                    sb.Append("+++++ ");
                    sb.Append(viewInstance.Value.GameObject.name);
                    sb.Append(" with id: ");
                    sb.Append(((IMultipleViewInstance)viewInstance.Value).GetMultipleViewId().ToString());
                    sb.Append(". Is in hierarchy: ");
                    sb.AppendLine(viewInstance.Value.IsInHierarchy.ToString());
                }
            }
            
            Debug.Log(sb.ToString());
        }        
#endif
    }
}