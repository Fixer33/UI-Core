using UnityEngine;
using UnityEngine.UI;

namespace UI.CustomButtons
{
    [AddComponentMenu("UI/Custom Buttons/Graphics Color VC")]
    public class GraphicsColorVC : StandaloneAnimatedVC
    {
        [SerializeField] private Graphic _graphic;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private CustomButtonState<Color> _highlightedColor;
        [SerializeField] private CustomButtonState<Color> _pressedColor;
        [SerializeField] private CustomButtonState<Color> _selectedColor;
        [SerializeField] private CustomButtonState<Color> _disabledColor;

        private Color _startColor;
        private Color _targetColor;

        public override void OnNormal() => SetColor(_normalColor);
        public override void OnHighlighted() => SetColor(_highlightedColor.GetValue(_normalColor));
        public override void OnPressed() => SetColor(_pressedColor.GetValue(_normalColor));
        public override void OnSelected() => SetColor(_selectedColor.GetValue(_normalColor));
        public override void OnDisabled() => SetColor(_disabledColor.GetValue(_normalColor));

        private void SetColor(Color targetColor)
        {
            if (_graphic == null) return;

            _targetColor = targetColor;
            _startColor = _graphic.color;

            StartAnimation(t =>
            {
                _graphic.color = Color.Lerp(_startColor, _targetColor, t);
            });
        }
    }
}
