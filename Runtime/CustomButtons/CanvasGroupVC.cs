using UnityEngine;

namespace UI.CustomButtons
{
    [AddComponentMenu("UI/Custom Buttons/Canvas Group VC")]
    public class CanvasGroupVC : StandaloneAnimatedVC
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _normalAlpha = 1f;
        [SerializeField] private CustomButtonState<float> _highlightedAlpha;
        [SerializeField] private CustomButtonState<float> _pressedAlpha;
        [SerializeField] private CustomButtonState<float> _selectedAlpha;
        [SerializeField] private CustomButtonState<float> _disabledAlpha;
        [SerializeField] private CustomButtonState<float> _premiumAlpha;

        private float _startAlpha;
        private float _targetAlpha;

        public override void OnNormal() => SetAlpha(_normalAlpha);
        public override void OnHighlighted() => SetAlpha(_highlightedAlpha.GetValue(_normalAlpha));
        public override void OnPressed() => SetAlpha(_pressedAlpha.GetValue(_normalAlpha));
        public override void OnSelected() => SetAlpha(_selectedAlpha.GetValue(_normalAlpha));
        public override void OnDisabled() => SetAlpha(_disabledAlpha.GetValue(_normalAlpha));
        public override void OnPremium() => SetAlpha(_premiumAlpha.GetValue(_normalAlpha));

        private void SetAlpha(float targetAlpha)
        {
            if (_canvasGroup == null) return;

            _targetAlpha = targetAlpha;
            _startAlpha = _canvasGroup.alpha;

            StartAnimation(t =>
            {
                _canvasGroup.alpha = Mathf.Lerp(_startAlpha, _targetAlpha, t);
            });
        }
    }
}
