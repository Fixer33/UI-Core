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

            // PropertyField for m_Interactable
            var interactable = new PropertyField(serializedObject.FindProperty("m_Interactable"));
            root.Add(interactable);

            // Hide transition field by not adding it
            // var transition = new PropertyField(serializedObject.FindProperty("m_Transition"));
            // root.Add(transition);

            // Add navigation
            var navigation = new PropertyField(serializedObject.FindProperty("m_Navigation"));
            root.Add(navigation);

            // Add onClick
            var onClick = new PropertyField(serializedObject.FindProperty("m_OnClick"));
            root.Add(onClick);

            return root;
        }
    }
}
