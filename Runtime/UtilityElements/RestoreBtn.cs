using Core.Services.Purchasing;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UtilityElements
{
    [RequireComponent(typeof(Button))]
    public class RestoreBtn : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(IAP.RestorePurchase);
        }
    }
}