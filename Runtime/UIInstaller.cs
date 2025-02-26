using System;
using Core.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Core
{
    [CreateAssetMenu(fileName = "UI Installer", menuName = "Installers/UI", order = 0)]
    public class UIInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private EventSystem _eventSystemPrefab;
        [SerializeField] private TypeReference<IView> _startViewType;
        [SerializeField] private GameObject[] _baseViewPrefabs = Array.Empty<GameObject>();
        [SerializeField] private UIViewFactoryBase[] _viewFactories = Array.Empty<UIViewFactoryBase>();
        [SerializeField] private PlatformDependentReference<StyleSheet> _additionalUIStyle;

        public override void InstallBindings()
        {
            Container.Bind<StyleSheet>().FromInstance(_additionalUIStyle.Get());

            GameObject[] screenPrefabs = _baseViewPrefabs;
            foreach (var viewFactoryBase in _viewFactories)
            {
                if (viewFactoryBase.CanBeUsed() == false)
                    continue;

                screenPrefabs = viewFactoryBase.CreateScreenPrefabArray(_baseViewPrefabs);
                break;
            }
            
            var ui = UIManager.Create(_startViewType.Get(), screenPrefabs).gameObject;
            Instantiate(_eventSystemPrefab, ui.transform);
            Container.InjectGameObject(ui);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            bool isModified = false;

            for (var i = 0; i < _baseViewPrefabs.Length; i++)
            {
                if (_baseViewPrefabs[i] && _baseViewPrefabs[i].GetComponent<IView>() != null)
                    continue;

                isModified = true;
                _baseViewPrefabs[i] = null;
            }

            if (isModified)
                UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}