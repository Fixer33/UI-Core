using System;
using System.Collections;
using Core.Services.Ads;
using Core.Services.Purchasing;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CustomButtons
{
    public class CustomButton : Button
    {
        [SerializeField] private bool _isPremium;
        
        private ICustomButtonVisualComponent[] _visualComponents;
        private bool _isInitialized;
        private bool _isInPremiumState;

        protected override void Awake()
        {
            base.Awake();
            InitializeIfNeeded();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            IAP.Initialized += OnIAPEvent;
            IAP.PremiumPurchased += OnIAPEvent;
            UpdatePremiumState();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            IAP.Initialized -= OnIAPEvent;
            IAP.PremiumPurchased -= OnIAPEvent;
        }

        private void OnIAPEvent()
        {
            UpdatePremiumState();
        }

        private void UpdatePremiumState()
        {
            if (!_isPremium)
            {
                _isInPremiumState = false;
                DoStateTransition(currentSelectionState, false);
                return;
            }
            
            bool hasPremium = IAP.IsInitialized && IAP.IsPremiumPurchased();
            _isInPremiumState = !hasPremium;
            
            if (_isInPremiumState)
            {
                ApplyPremiumVisuals();
            }
            else
            {
                DoStateTransition(currentSelectionState, false);
            }
        }

        private void ApplyPremiumVisuals()
        {
            InitializeIfNeeded();
            foreach (var component in _visualComponents)
            {
                if (component == null || (component is MonoBehaviour mono && mono == null)) continue;
                component.OnPremium();
            }
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

            if (_isInPremiumState)
            {
                new AutomaticAdRequest(true).Invoke();
                return;
            }

            StartCoroutine(InvokeOnClickDelayed());
        }

        private IEnumerator InvokeOnClickDelayed()
        {
            if (_isInPremiumState)
            {
                new AutomaticAdRequest(true).Invoke();
                yield break;
            }

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

            if (_isInPremiumState)
            {
                ApplyPremiumVisuals();
                return;
            }
            
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
        void OnPremium();
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