using TMPro;
using UnityEngine;

namespace UI.CustomButtons
{
    [AddComponentMenu("UI/Custom Buttons/Font VC")]
    public class FontVC : MonoBehaviour, ICustomButtonVisualComponent
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_FontAsset _normalFont;
        [SerializeField] private CustomButtonState<TMP_FontAsset> _highlightedFont;
        [SerializeField] private CustomButtonState<TMP_FontAsset> _pressedFont;
        [SerializeField] private CustomButtonState<TMP_FontAsset> _selectedFont;
        [SerializeField] private CustomButtonState<TMP_FontAsset> _disabledFont;

        public void OnNormal() => SetFont(_normalFont);
        public void OnHighlighted() => SetFont(_highlightedFont.GetValue(_normalFont));
        public void OnPressed() => SetFont(_pressedFont.GetValue(_normalFont));
        public void OnSelected() => SetFont(_selectedFont.GetValue(_normalFont));
        public void OnDisabled() => SetFont(_disabledFont.GetValue(_normalFont));

        private void SetFont(TMP_FontAsset font)
        {
            if (_text != null && font != null)
                _text.font = font;
        }
    }
}
