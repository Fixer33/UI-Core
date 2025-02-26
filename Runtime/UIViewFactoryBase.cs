using System;
using System.Linq;
using UnityEngine;

namespace UI.Core
{
    public abstract class UIViewFactoryBase : ScriptableObject
    {
        [SerializeField] private GameObject[] _viewPrefabOverrides;
        
        public GameObject[] CreateScreenPrefabArray(GameObject[] basePrefabs)
        {
            GameObject[] result = new GameObject[basePrefabs.Length];
            var viewScreenTypes = _viewPrefabOverrides.Select(i => i.GetComponent<IView>().GetType()).ToArray();
            for (var i = 0; i < basePrefabs.Length; i++)
            {
                int overrideIndex =
                    Array.IndexOf(viewScreenTypes, basePrefabs[i].GetComponent<IView>().GetType());

                if (overrideIndex < 0)
                {
                    result[i] = basePrefabs[i];
                    continue;
                }

                result[i] = _viewPrefabOverrides[overrideIndex];
            }

            return result;
        }

        public abstract bool CanBeUsed();
    }
}