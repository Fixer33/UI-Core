using System;
using System.Linq;
using UnityEngine;

namespace UI
{
    public abstract class UIViewFactoryBase : ScriptableObject
    {
        [SerializeField] private GameObject[] _viewPrefabOverrides;
        
        public GameObject[] CreateViewPrefabArray(GameObject[] basePrefabs)
        {
            GameObject[] result = new GameObject[basePrefabs.Length];
            var viewOverrides = _viewPrefabOverrides
                .Select(i => i.GetComponent<IView>())
                .Where(i => i is not null)
                .ToArray();
            for (var i = 0; i < basePrefabs.Length; i++)
            {
                var prefabView = basePrefabs[i].GetComponent<IView>();

                int overrideIndexInArray = -1;
                
                for (var overrideIndex = 0; overrideIndex < viewOverrides.Length; overrideIndex++)
                {
                    if (prefabView.GetType() != viewOverrides[overrideIndex].GetType())
                        continue;
                    
                    if (prefabView is IMultipleViewInstance prefabMultipleViewInstance)
                    {
                        if (viewOverrides[overrideIndex] is not IMultipleViewInstance overrideMultipleViewInstance)
                            continue;
                        
                        if (prefabMultipleViewInstance.GetMultipleViewId() != overrideMultipleViewInstance.GetMultipleViewId())
                            continue;

                        overrideIndexInArray = overrideIndex;
                        break;
                    }
                    else
                    {
                        overrideIndexInArray = overrideIndex;
                    }
                }
                

                if (overrideIndexInArray < 0)
                {
                    result[i] = basePrefabs[i];
                    continue;
                }

                result[i] = viewOverrides[overrideIndexInArray].GameObject;
            }

            return result;
        }

        public abstract bool CanBeUsed();

        private void OnValidate()
        {
            for (var i = 0; i < _viewPrefabOverrides.Length; i++)
            {
                if (_viewPrefabOverrides[i].GetComponent<IView>().IsAlive() == false)
                    Debug.LogError($"Prefab on index {i} in {name} failed to validate as an IView");
            }
        }
    }
}
