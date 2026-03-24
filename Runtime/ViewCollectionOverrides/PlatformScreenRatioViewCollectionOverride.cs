using Core.Utilities;
using UnityEngine;

namespace UI.ViewCollectionOverrides
{
    [CreateAssetMenu(fileName = "Platform screen ratio override", menuName = "UI/View collection overrides/Platform and screen ratio", order = 0)]
    public class PlatformScreenRatioViewCollectionOverride : UIViewCollectionOverrideBase
    {
        [SerializeField] private PlatformSpecification _platform;
        [SerializeField] private float _minScreenRatio, _maxScreenRatio;
        
        public override bool CanBeUsed()
        {
            if (PlatformUtility.IsForCurrentPlatform(_platform) == false)
                return false;
            
            float screenRatio = Screen.width * 1f / Screen.height;
            return screenRatio >= _minScreenRatio && screenRatio <= _maxScreenRatio;
        }
    }
}