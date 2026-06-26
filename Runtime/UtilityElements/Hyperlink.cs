using Core;
using Core.Installers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UtilityElements
{
    [RequireComponent(typeof(Button))]
    public class Hyperlink : MonoBehaviour
    {
        [SerializeField] private LinkId _linkId;
        private Button _btn;

        private void Start()
        {
            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(OpenLink);
        }

        private void OnDestroy()
        {
            try
            {
                _btn.onClick.RemoveListener(OpenLink);
            }
            catch
            {
            }
        }

        private void OpenLink()
        {
            Application.OpenURL(LinkInstaller.GetLink(_linkId));
        }
    }
}
