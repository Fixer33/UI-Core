using UnityEngine;

namespace UI.ElementSelection.VisualModules
{
    [AddComponentMenu("UI/Element Selection/Modules/GameObject toggle SVM")]
    public class GameObjectToggleSelectionVisualModule : MonoBehaviour, ISelectableElementVisualModule
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private bool _activateOnSelection = true;
        
        public void OnSelectionChanged(bool isSelected)
        {
            _gameObject.SetActive(_activateOnSelection ? isSelected : isSelected == false);
        }
    }
}