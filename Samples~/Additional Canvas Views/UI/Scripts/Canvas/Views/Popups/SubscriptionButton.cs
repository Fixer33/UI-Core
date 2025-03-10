using System;
using Core.Services.Purchasing;
using Core.Services.Purchasing.Products;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace UI.Canvas.Views.Popups
{
    public class SubscriptionButton : MonoBehaviour
    {
        public event Action<IAPProductBase, string> Clicked;
        public IAPProductBase Product => _product;

        [SerializeField] private bool _callPurchaseOnBtn;
        [SerializeField] private int _daysFree = 0;
        [SerializeField] private LocalizedString _paymentPeriod;
        [SerializeField] private LocalizedString _priceTemplate;
        [SerializeField] private LocalizedString _btnTextTemplate;
        [SerializeField] private IAPProductBase _product;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _btnText;
        [SerializeField] private Button _btn;

        private void Awake()
        {
            _btn.interactable = false;
            IAP.ExecuteOnInit(OnIAPInitialized);
            
            _btn.onClick.AddListener(() =>
            {
                InvokeClick();
                if (_callPurchaseOnBtn)
                    IAP.Purchase(_product);
            });
        }

        private void OnIAPInitialized()
        {
            _btn.interactable = true;
        }

        private string GetFormattedTemplate(LocalizedString template)
        {
            string result = template.GetLocalizedString();
            result = result.Replace("{price}", _product.GetPrice());
            result = result.Replace("{period}", _paymentPeriod.GetLocalizedString());
            result = result.Replace("{days_free}", _daysFree.ToString());
            return result;
        }

        public void InvokeClick()
        {
            _priceText.text = GetFormattedTemplate(_priceTemplate);
            _btnText.text = GetFormattedTemplate(_btnTextTemplate);
            
            Clicked?.Invoke(_product, _paymentPeriod.GetLocalizedString());
        }

        public void SetSelected(bool isSelected)
        {
            _btn.interactable = isSelected == false;
        }
    }
}