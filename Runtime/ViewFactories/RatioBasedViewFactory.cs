using UnityEngine;

namespace UI.Core.ViewFactories
{
    [CreateAssetMenu(fileName = "Ratio based factory", menuName = "Factory/View/Ratio based", order = 0)]
    public class RatioBasedViewFactory : UIViewFactoryBase
    {
        [SerializeField] private float _minScreenRatio, _maxScreenRatio;
        
        public override bool CanBeUsed()
        {
            float screenRatio = Screen.width * 1f / Screen.height;
            return screenRatio >= _minScreenRatio && screenRatio <= _maxScreenRatio;
        }
    }
}