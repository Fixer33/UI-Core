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

namespace UI.Core
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
        public IView PreviousMainScreen { get; private set; }
        private static Type _startScreenType;

        private readonly Dictionary<Type, IView> _singleScreenDict = new();
        private readonly Dictionary<Type, Dictionary<int, IView>> _multipleScreenDict = new();
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
            PreviousMainScreen = null;
        }

        private void Start()
        {
            foreach (var screen in GetComponentsInChildren<IView>())
            {
                if (screen.GetType() != _startScreenType)
                    screen.HideInstant();
                else
                    screen.ShowInstant();

                RegisterScreenView(screen);
            }

            _activeLayers[UILayer.Main] = GetScreen(_startScreenType);
        }

        public void RegisterScreenView(IView screen)
        {
            var type = screen.GetType();

            // Register screen if it allows multiple instances
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (screen is IMultipleViewInstance multipleScreenInstance)
            {
                Dictionary<int, IView> screenTypeDict;
                if (_multipleScreenDict.TryGetValue(type, out screenTypeDict) == false)
                    _multipleScreenDict.Add(type, screenTypeDict = new Dictionary<int, IView>());

                int screenId = multipleScreenInstance.GetMultipleScreenId();
                if (screenTypeDict.TryAdd(screenId, screen) == false)
                    Debug.LogError(
                        $"Failed to register screen {screen.GameObject.name}({type.Name}) as multiple screen with id {screenId}!\n" +
                        $"Instance with such id already exists: {screenTypeDict[screenId].GameObject.name}");

                _multipleScreenDict[type] = screenTypeDict;
                return;
            }

            // Register screen as regular single instance
            if (_singleScreenDict.TryAdd(type, screen) == false)
                Debug.LogError($"Failed to register screen {screen.GameObject.name}({type.Name}) as regular!\n" +
                               $"Instance with such id already exists: {_singleScreenDict[type].GameObject.name}");
        }

        public void UnRegisterScreen(IView screen)
        {
            var type = screen.GetType();
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (screen is IMultipleViewInstance multipleScreenInstance)
            {
                int instanceId = multipleScreenInstance.GetMultipleScreenId();
                if (_multipleScreenDict.TryGetValue(type, out var instanceDict) == false || instanceDict.ContainsKey(instanceId) == false)
                {
                    Debug.LogError($"Failed to unregister screen {screen.GameObject.name}({type.Name}). No instances registered");
                    return;
                }

                instanceDict.Remove(instanceId);
                _multipleScreenDict[type] = instanceDict;
                return;
            }

            if (_singleScreenDict.ContainsKey(type) == false)
            {
                Debug.LogError($"Failed to unregister screen {screen.GameObject.name}({type.Name}). No instance registered");
                return;
            }

            _singleScreenDict.Remove(type);
        }

        public T GetScreen<T>(int? multipleScreenInstanceId = null) where T : IView => (T)GetScreen(typeof(T), multipleScreenInstanceId);

        public IView GetScreen(Type screenType, int? multipleScreenInstanceId = null)
        {
            if (screenType.GetInterface(nameof(IMultipleViewInstance)) == null)
                return _singleScreenDict.GetValueOrDefault(screenType);

            if (multipleScreenInstanceId.HasValue == false)
            {
                Debug.LogError($"Failed to get screen of type {screenType.Name}! No screen id supplied!");
                return null;
            }

            if (_multipleScreenDict.TryGetValue(screenType, out var instanceDict) == false)
                return null;
                
            return instanceDict.GetValueOrDefault(multipleScreenInstanceId.Value);
        }

        public async void ShowScreenAsync(Type screenType,
            VisibilityChangeType activeScreenHideType = VisibilityChangeType.Regular,
            VisibilityChangeType newScreenShowType = VisibilityChangeType.Regular,
            Action activeScreenHidden = null,
            Action newScreenShown = null,
            Action onError = null,
            UILayer layer = UILayer.Main,
            IViewData showData = null,
            int? multipleScreenInstanceId = null
        )
        {
            void HideActiveScreen(IView activeScreen, Action callback)
            {
                activeScreen.Hide(() =>
                {
                    activeScreenHidden?.Invoke();
                    callback?.Invoke();
                });
            }
            void ShowNewScreen(IView screen, Action callback)
            {
                screen.Show(callback, data: showData);
            }
            void OnLayerScreenVisibilityChange(object sender, ScreenViewVisibilityArgs screenViewVisibilityArgs)
            {
                if (screenViewVisibilityArgs.IsVisible)
                    return;
                
                if (sender is IView screenView)
                    screenView.VisibilityChanged -= OnLayerScreenVisibilityChange;

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
            
            var screen = GetScreen(screenType, multipleScreenInstanceId);
            if (screen == null)
            {
                string multipleInstanceIdS =
                    multipleScreenInstanceId.HasValue ? multipleScreenInstanceId.Value.ToString() : "null";
                Debug.LogError($"Failed to show new screen! Screen of type {screenType.Name} is not registered!\n" +
                               $"Supplied multipleInstanceId: {multipleInstanceIdS}");
                onError?.Invoke();
                return;
            }
            
            await WaitUntilCanBeShown();

            screen.VisibilityChanged += OnLayerScreenVisibilityChange;
            
            var activeOnLayer = _activeLayers.GetValueOrDefault(layer, null);
            _canBeShown = false;
            if (activeOnLayer.IsAlive() == false) // In case if active screen was destroyed
            {
                if (layer == UILayer.Main)
                    Debug.LogWarning("Active main screen has been destroyed before switching to a new one!");
                if (newScreenShowType == VisibilityChangeType.Instant)
                {
                    _activeLayers[layer] = screen;
                    screen.ShowInstant(data: showData);
                    _canBeShown = true;
                    newScreenShown?.Invoke();
                    return;
                }
                
                _activeLayers[layer] = screen;
                ShowNewScreen(screen, () =>
                {
                    _canBeShown = true;
                    newScreenShown?.Invoke();
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

                _activeLayers[layerToHide].VisibilityChanged -= OnLayerScreenVisibilityChange;
                if (activeScreenHideType == VisibilityChangeType.Regular)
                    _activeLayers[layerToHide].Hide();
                else
                    _activeLayers[layerToHide].HideInstant();

                _activeLayers[layerToHide] = null;
            }
            
            activeOnLayer.VisibilityChanged -= OnLayerScreenVisibilityChange;
            switch (activeScreenHideType, newScreenShowType)
            {
                case (VisibilityChangeType.Regular, VisibilityChangeType.Regular):
                    HideActiveScreen(activeOnLayer, () =>
                    {
                        if (layer == UILayer.Main)
                            PreviousMainScreen = activeOnLayer;
                        _activeLayers[layer] = screen;
                        ShowNewScreen(screen, () =>
                        {
                            _canBeShown = true;
                            newScreenShown?.Invoke();
                        });
                    });
                    break;
                case (VisibilityChangeType.Regular, VisibilityChangeType.Instant):
                    HideActiveScreen(activeOnLayer, () =>
                    {
                        if (layer == UILayer.Main)
                            PreviousMainScreen = activeOnLayer;
                        _activeLayers[layer] = screen;
                        screen.ShowInstant(data: showData);
                        _canBeShown = true;
                        newScreenShown?.Invoke();
                    });
                    break;
                case (VisibilityChangeType.Instant, VisibilityChangeType.Regular):
                    if (layer == UILayer.Main)
                        PreviousMainScreen = activeOnLayer;
                    activeOnLayer.HideInstant();
                    _activeLayers[layer] = screen;
                    ShowNewScreen(screen, () =>
                    {
                        _canBeShown = true;
                        newScreenShown?.Invoke();
                    });
                    break;
                case (VisibilityChangeType.Instant, VisibilityChangeType.Instant):
                    if (layer == UILayer.Main)
                        PreviousMainScreen = activeOnLayer;
                    activeOnLayer.HideInstant();
                    _activeLayers[layer] = screen;
                    screen.ShowInstant(data: showData);
                    _canBeShown = true;
                    newScreenShown?.Invoke();
                    break;
            }
        }

        public void ShowScreenAsync<T>(
            VisibilityChangeType activeScreenHideType = VisibilityChangeType.Regular,
            VisibilityChangeType newScreenShowType = VisibilityChangeType.Regular,
            Action activeScreenHidden = null,
            Action newScreenShown = null,
            Action onError = null,
            UILayer layer = UILayer.Main,
            IViewData showData = null,
            int? multipleScreenInstanceId = null
        ) where T : IView => ShowScreenAsync(typeof(T), 
            activeScreenHideType, newScreenShowType, activeScreenHidden, newScreenShown, onError, layer, showData, multipleScreenInstanceId);

        public void ShowScreenAsPopupAsync<T>(
            Action onShown = null,
            Action onError = null,
            IViewData showData = null,
            int? multipleScreenInstanceId = null
        ) where T : IView => ShowScreenAsync<T>(newScreenShown: onShown, onError: onError, showData: showData, layer: UILayer.Popups,
            multipleScreenInstanceId: multipleScreenInstanceId);
        
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

        public static UIManager Create(Type startScreenType, params GameObject[] screenPrefabs)
        {
            GameObject uiObj = new GameObject("UI");
            _startScreenType = startScreenType;
            var result = uiObj.AddComponent<UIManager>();
            
            foreach (var screenPrefab in screenPrefabs)
            {
                var spawnedScreen = Instantiate(screenPrefab, uiObj.transform);
            }

            return result;
        }
        
#if UNITY_EDITOR
        [ContextMenu("Print all registered screens")]
        private void EDITOR_PrintAllRegisteredScreens()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("All registered screens:");
            sb.AppendLine("=Single instance screens:");
            foreach (var singleScreenRecord in _singleScreenDict)
            {
                sb.Append("=== ");
                sb.Append(singleScreenRecord.Value.GameObject.name);
                sb.Append("(");
                sb.Append(singleScreenRecord.Key.Name);
                sb.Append("). Is in hierarchy: ");
                sb.AppendLine(singleScreenRecord.Value.IsInHierarchy.ToString());
            }

            sb.AppendLine("=Multiple instance screens:");
            foreach (var multipleScreenRecord in _multipleScreenDict)
            {
                sb.Append("=== ");
                sb.Append("Screens of type ");
                sb.AppendLine(multipleScreenRecord.Key.Name);
                
                foreach (var screenInstance in multipleScreenRecord.Value)
                {
                    sb.Append("+++++ ");
                    sb.Append(screenInstance.Value.GameObject.name);
                    sb.Append(" with id: ");
                    sb.Append(((IMultipleViewInstance)screenInstance.Value).GetMultipleScreenId().ToString());
                    sb.Append(". Is in hierarchy: ");
                    sb.AppendLine(screenInstance.Value.IsInHierarchy.ToString());
                }
            }
            
            Debug.Log(sb.ToString());
        }        
#endif
    }
}