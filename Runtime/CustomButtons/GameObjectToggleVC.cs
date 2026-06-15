using UnityEngine;

namespace UI.CustomButtons
{
    [AddComponentMenu("UI/Custom Buttons/GameObject Toggle VC")]
    public class GameObjectToggleVC : MonoBehaviour, ICustomButtonVisualComponent
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private bool _normalValue;
        [SerializeField] private CustomButtonState<bool> _highlightedValue;
        [SerializeField] private CustomButtonState<bool> _pressedValue;
        [SerializeField] private CustomButtonState<bool> _selectedValue;
        [SerializeField] private CustomButtonState<bool> _disabledValue;

        public void OnNormal() => SetActive(_normalValue);
        public void OnHighlighted() => SetActive(_highlightedValue.GetValue(_normalValue));
        public void OnPressed() => SetActive(_pressedValue.GetValue(_normalValue));
        public void OnSelected() => SetActive(_selectedValue.GetValue(_normalValue));
        public void OnDisabled() => SetActive(_disabledValue.GetValue(_normalValue));

        private void SetActive(bool active)
        {
            if (_target != null)
                _target.SetActive(active);
        }
    }
}
