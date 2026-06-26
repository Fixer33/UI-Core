using UI.CustomButtons;
using UnityEditor;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UI.Editor.CustomButtons
{
    [CustomEditor(typeof(CustomButton))]
    [CanEditMultipleObjects]
    public class CustomButtonEditor : ButtonEditor
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
                
                // Hide modules and script
                if (iterator.name == "_visualComponents" || iterator.name == "m_Script")
                    continue;

                // Hide transition fields from base Button
                if (iterator.name == "m_Transition" || 
                    iterator.name == "m_Colors" || 
                    iterator.name == "m_SpriteState" || 
                    iterator.name == "m_AnimationTriggers" ||
                    iterator.name == "m_TargetGraphic")
                {
                    continue;
                }

                root.Add(new PropertyField(iterator));
            }

            // Draw modules
            var modulesProp = serializedObject.FindProperty("_visualComponents");
            ModuleListEditorHelper.CreateModuleList(root, modulesProp, typeof(ICustomButtonVisualComponent), "Visual Components (Modules)");

            return root;
        }
    }
}
