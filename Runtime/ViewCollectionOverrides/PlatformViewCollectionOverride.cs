using Core.Utilities;
using UnityEngine;

namespace UI.ViewCollectionOverrides
{
    [CreateAssetMenu(fileName = "Platform override", menuName = "UI/View collection overrides/Platform", order = 0)]
    public class PlatformViewCollectionOverride : UIViewCollectionOverrideBase
    {
        [SerializeField] private PlatformSpecification _platform;
        
        public override bool CanBeUsed() => PlatformUtility.IsForCurrentPlatform(_platform);
    }
}