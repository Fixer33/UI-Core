using UnityEngine;

namespace UI.ViewCollectionOverrides
{
    [CreateAssetMenu(fileName = "Ratio override", menuName = "UI/View collection overrides/Screen ratio", order = 0)]
    public class ScreenRatioViewCollectionOverride : UIViewCollectionOverrideBase
    {
        [SerializeField] private float _minScreenRatio, _maxScreenRatio;
        
        public override bool CanBeUsed()
        {
            float screenRatio = Screen.width * 1f / Screen.height;
            return screenRatio >= _minScreenRatio && screenRatio <= _maxScreenRatio;
        }
    }
}