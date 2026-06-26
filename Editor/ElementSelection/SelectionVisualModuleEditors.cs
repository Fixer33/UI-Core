using UI.ElementSelection.VisualModules;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Editor.ElementSelection
{
    [CustomPropertyDrawer(typeof(GraphicsColorSelectionVisualModule))]
    public class GraphicsColorSelectionVisualModuleDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.Add(new PropertyField(property.FindPropertyRelative("_graphic")));
            root.Add(new PropertyField(property.FindPropertyRelative("_defaultColor")));
            root.Add(new PropertyField(property.FindPropertyRelative("_selectedColor")));
            
            SelectionVisualModuleEditor.CreateStateField(root, property.FindPropertyRelative("_hoveredColor"), "Hovered Color");
            SelectionVisualModuleEditor.CreateStateField(root, property.FindPropertyRelative("_premiumColor"), "Premium Color");

            SelectionVisualModuleEditor.DrawAnimationSettings(root, property);
            return root;
        }
    }

    [CustomPropertyDrawer(typeof(CanvasGroupSelectionVisualModule))]
    public class CanvasGroupSelectionVisualModuleDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.Add(new PropertyField(property.FindPropertyRelative("_group")));
            root.Add(new PropertyField(property.FindPropertyRelative("_defaultState")));
            root.Add(new PropertyField(property.FindPropertyRelative("_selectedState")));
            
            SelectionVisualModuleEditor.CreateStateField(root, property.FindPropertyRelative("_hoveredState"), "Hovered State");
            SelectionVisualModuleEditor.CreateStateField(root, property.FindPropertyRelative("_premiumState"), "Premium State");

            SelectionVisualModuleEditor.DrawAnimationSettings(root, property);
            return root;
        }
    }

    [CustomPropertyDrawer(typeof(FontSelectionVisualModule))]
    public class FontSelectionVisualModuleDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.Add(new PropertyField(property.FindPropertyRelative("_text")));
            root.Add(new PropertyField(property.FindPropertyRelative("_defaultFont")));
            root.Add(new PropertyField(property.FindPropertyRelative("_selectedFont")));
            
            SelectionVisualModuleEditor.CreateStateField(root, property.FindPropertyRelative("_hoveredFont"), "Hovered Font");
            SelectionVisualModuleEditor.CreateStateField(root, property.FindPropertyRelative("_premiumFont"), "Premium Font");
            return root;
        }
    }

    [CustomPropertyDrawer(typeof(GameObjectToggleSelectionVisualModule))]
    public class GameObjectToggleSelectionVisualModuleDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.Add(new PropertyField(property.FindPropertyRelative("_gameObject")));
            
            SelectionVisualModuleEditor.CreateStateField(root, property.FindPropertyRelative("_normalValue"), "Normal Value");
            SelectionVisualModuleEditor.CreateStateField(root, property.FindPropertyRelative("_selectedValue"), "Selected Value");
            SelectionVisualModuleEditor.CreateStateField(root, property.FindPropertyRelative("_hoveredValue"), "Hovered Value");
            SelectionVisualModuleEditor.CreateStateField(root, property.FindPropertyRelative("_premiumValue"), "Premium Value");
            return root;
        }
    }

    public static class SelectionVisualModuleEditor
    {
        public static void CreateStateField(VisualElement root, SerializedProperty property, string label)
        {
            if (property == null) return;

            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.marginTop = 2;

            var enabledProp = property.FindPropertyRelative("Enabled");
            var valueProp = property.FindPropertyRelative("Value");

            var toggle = new PropertyField(enabledProp, "");
            toggle.style.width = 20;
            
            var field = new PropertyField(valueProp, label);
            field.style.flexGrow = 1;
            
            // Sync enabled state
            field.SetEnabled(enabledProp.boolValue);
            toggle.RegisterValueChangeCallback(evt => {
                field.SetEnabled(evt.changedProperty.boolValue);
            });

            container.Add(toggle);
            container.Add(field);
            root.Add(container);
        }

        public static void DrawAnimationSettings(VisualElement root, SerializedProperty property)
        {
            var animHeader = new Label("Animation");
            animHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            animHeader.style.marginTop = 10;
            animHeader.style.marginBottom = 2;
            root.Add(animHeader);

            root.Add(new PropertyField(property.FindPropertyRelative("_animate")));
            root.Add(new PropertyField(property.FindPropertyRelative("_duration")));
            root.Add(new PropertyField(property.FindPropertyRelative("_useUnscaledTime")));
        }
    }
}
