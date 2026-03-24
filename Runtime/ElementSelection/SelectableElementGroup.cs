using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.ElementSelection
{
    [DefaultExecutionOrder(-100)]
    [AddComponentMenu("UI/Element Selection/Selectable Element Group")]
    public class SelectableElementGroup : MonoBehaviour
    {
        public event ButtonDelegate ElementSelected = delegate { };
        public delegate void ButtonDelegate(SelectableElement element);

        public SelectableElement ActiveSelectedElement => _activeSelectedElement;

        public IReadOnlyList<SelectableElement> Elements => _elements;

        private readonly List<SelectableElement> _elements = new();
        private SelectableElement _activeSelectedElement;

        private void Awake()
        {
            foreach (var element in GetComponentsInChildren<SelectableElement>(true))
            {
                RegisterElement(element);
            }
        }

        public void RegisterElement(SelectableElement element)
        {
            if (element == false || _elements.Contains(element))
                return;
            
            _elements.Add(element);
            element.SelectionStateChanged += isSelected =>
            {
                if (isSelected == false)
                {
                    if (_activeSelectedElement == element)
                    {
                        _activeSelectedElement = null;
                        ElementSelected?.Invoke(null);
                    }
                    return;
                }

                foreach (var selectableModeButton in _elements)
                {
                    if (selectableModeButton == element)
                        continue;
                            
                    selectableModeButton.SetSelected(false);
                }

                _activeSelectedElement = element;
                ElementSelected?.Invoke(element);
            };
        }

        public void DeRegisterElement(SelectableElement element)
        {
            if (_elements.Contains(element) == false)
                return;
            
            _elements.Remove(element);
        }

        public bool TryGetWithPayload<T>(out T script, out SelectableElement selectableButton, Predicate<T> predicate = null)
            where T : Behaviour
        {
            foreach (var button in _elements)
            {
                if (button == false)
                    continue;
                
                if (button.TryGetPayload<T>(out var payloadScript) == false)
                    continue;
                
                if (predicate != null && predicate(payloadScript) == false)
                    continue;

                script = payloadScript;
                selectableButton = button;
                return true;
            }

            script = default;
            selectableButton = default;
            return false;
        }

        public void ClearSelection()
        {
            _activeSelectedElement?.SetSelected(false);
            _activeSelectedElement = null;
            ElementSelected?.Invoke(null);
        }

        public void UpdateElements()
        {
            foreach (var element in GetComponentsInChildren<SelectableElement>(true))
            {
                RegisterElement(element);
            }
        }
    }
}
