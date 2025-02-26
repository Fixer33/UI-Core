using System;
using Core.Services.Ads;
using Core.Services.Purchasing;
using Core.Services.Purchasing.Products;
using TMPro;
using UI.Canvas.ViewBases;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace UI.Canvas.Views.Popups
{
    public class Paywall : AnimatedView
    {
        [Header("Main settings")] 
        [SerializeField] private int _defaultSubBtn;
        [Header("Main refs")]
        [SerializeField] private GameObject _progressIndicator;
        [SerializeField] private Button[] _closeBtns;
        [SerializeField] private SubscriptionButton[] _subBtns = Array.Empty<SubscriptionButton>();
        [Header("May be empty")] 
        [SerializeField] private Button _subscribeBtn;
        [Header("Sub terms")] 
        [SerializeField] private LocalizedString _subTermsTextTemplate;
        [SerializeField] private Button _subTermsBtn;
        [SerializeField] private Button _subTermsCloseBtn;
        [SerializeField] private GameObject _subTerms;
        [SerializeField] private TMP_Text _subTermsText;
        private IAPProductBase _selectedProduct;
        
        protected override void OnAwakeEarly()
        {
            AutomaticAdTrackingService.RegisterPaywall(this);
            base.OnAwakeEarly();

            _subTermsText.text = string.Format(_subTermsTextTemplate.GetLocalizedString(), "purchases initializing...", "purchases initializing...");

            for (int i = 0; i < _subBtns.Length; i++)
            {
                _subBtns[i].Clicked += OnSubBtnClicked;
            }
            _subTermsBtn.onClick.AddListener(() => _subTerms.SetActive(true));
            _subTermsCloseBtn.onClick.AddListener(() => _subTerms.SetActive(false));
            foreach (var closeBtn in _closeBtns)
            {
                if (closeBtn)
                    closeBtn.onClick.AddListener(() => Hide());
            }
            if (_subscribeBtn)
                _subscribeBtn.onClick.AddListener(() => IAP.Purchase(_selectedProduct));
            
            IAP.ProductPurchaseStarted += IAPManagerOnPurchaseStartedEvent;
            IAP.ProductPurchaseFailed += IAPManagerOnPurchaseFailedEvent;
            IAP.ProductPurchased += IAPManagerOnPurchaseSucceededEvent;
        }

        protected override void OnDestroyView()
        {
            base.OnDestroyView();
            
            IAP.ProductPurchaseStarted-= IAPManagerOnPurchaseStartedEvent;
            IAP.ProductPurchaseFailed -= IAPManagerOnPurchaseFailedEvent;
            IAP.ProductPurchased -= IAPManagerOnPurchaseSucceededEvent;
        }

        protected override void OnShowStart()
        {
            _progressIndicator.SetActive(false);
            _subBtns[_defaultSubBtn].InvokeClick();
        }

        #region Listeners

        private void OnSubBtnClicked(IAPProductBase product, string paymentPeriod)
        {
            _selectedProduct = product;
            _subTermsText.text = string.Format(_subTermsTextTemplate.GetLocalizedString(), product.GetPrice(), paymentPeriod);
            
            foreach (var subscriptionButton in _subBtns)
            {
                subscriptionButton.SetSelected(subscriptionButton.Product == product);
            }
        }

        private void IAPManagerOnPurchaseSucceededEvent(IAPProductBase iapProductBase)
        {
            _progressIndicator.SetActive(false);
            Hide();
        }

        private void IAPManagerOnPurchaseFailedEvent(IAPProductBase iapProductBase)
        {
            _progressIndicator.SetActive(false);
        }

        private void IAPManagerOnPurchaseStartedEvent()
        {
            _progressIndicator.SetActive(true);
        }

        #endregion

        private void OnValidate()
        {
            _defaultSubBtn = Mathf.Clamp(_defaultSubBtn, 0, _subBtns.Length);
        }
    }
}