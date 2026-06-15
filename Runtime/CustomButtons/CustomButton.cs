using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CustomButtons
{
    public class CustomButton : Button
    {
        private ICustomButtonVisualComponent[] _visualComponents;
        private bool _isInitialized;

        protected override void Awake()
        {
            base.Awake();
            InitializeIfNeeded();
        }

        private void InitializeIfNeeded()
        {
            if (_isInitialized) return;
            _visualComponents = GetComponentsInChildren<ICustomButtonVisualComponent>();
            _isInitialized = true;
        }

        public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (eventData.button != UnityEngine.EventSystems.PointerEventData.InputButton.Left)
                return;

            if (!IsActive() || !IsInteractable())
                return;

            StartCoroutine(InvokeOnClickDelayed());
        }

        private IEnumerator InvokeOnClickDelayed()
        {
            InitializeIfNeeded();
            float maxDuration = 0;
            foreach (var component in _visualComponents)
            {
                if (component == null || (component is MonoBehaviour mono && mono == null)) continue;
                
                if (component is ICustomButtonAnimatedVisualComponent animated)
                {
                    maxDuration = Mathf.Max(maxDuration, animated.GetAnimationDuration());
                }
            }

            if (maxDuration > 0)
            {
                yield return new WaitForSecondsRealtime(maxDuration);
            }

            onClick.Invoke();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            // base.DoStateTransition(state, instant); // We don't want base transition logic
            
            InitializeIfNeeded();
            
            foreach (var component in _visualComponents)
            {
                if (component == null || (component is MonoBehaviour mono && mono == null)) continue;

                switch (state)
                {
                    case SelectionState.Normal:
                        component.OnNormal();
                        break;
                    case SelectionState.Highlighted:
                        component.OnHighlighted();
                        break;
                    case SelectionState.Pressed:
                        component.OnPressed();
                        break;
                    case SelectionState.Selected:
                        component.OnSelected();
                        break;
                    case SelectionState.Disabled:
                        component.OnDisabled();
                        break;
                }
            }
        }
    }

    public interface ICustomButtonVisualComponent
    {
        void OnNormal();
        void OnHighlighted();
        void OnPressed();
        void OnSelected();
        void OnDisabled();
    }

    public interface ICustomButtonAnimatedVisualComponent : ICustomButtonVisualComponent
    {
        float GetAnimationDuration();
    }

    [Serializable]
    public struct CustomButtonState<T>
    {
        public bool Enabled;
        public T Value;

        public T GetValue(T defaultValue) => Enabled ? Value : defaultValue;
    }
}