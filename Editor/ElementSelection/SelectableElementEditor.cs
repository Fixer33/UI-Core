using UI.ElementSelection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UI.Editor.ElementSelection
{
    [CustomEditor(typeof(SelectableElement))]
    [CanEditMultipleObjects]
    public class SelectableElementEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            // Draw base properties
            var iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (iterator.name == "_modules" || iterator.name == "m_Script")
                    continue;

                root.Add(new PropertyField(iterator));
            }

            // Draw modules
            var modulesProp = serializedObject.FindProperty("_modules");
            ModuleListEditorHelper.CreateModuleList(root, modulesProp, typeof(ISelectableElementVisualModule), "Visual Modules");

            return root;
        }
    }
}
