using UI.CustomButtons;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Editor.CustomButtons
{
    [CustomPropertyDrawer(typeof(GraphicsColorVC))]
    public class GraphicsColorVCDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.Add(new PropertyField(property.FindPropertyRelative("_graphic")));
            root.Add(new PropertyField(property.FindPropertyRelative("_normalColor")));
            
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_highlightedColor"), "Highlighted");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_pressedColor"), "Pressed");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_selectedColor"), "Selected");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_disabledColor"), "Disabled");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_premiumColor"), "Premium");

            VisualComponentEditor.DrawAnimationSettings(root, property);
            return root;
        }
    }

    [CustomPropertyDrawer(typeof(GameObjectToggleVC))]
    public class GameObjectToggleVCDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.Add(new PropertyField(property.FindPropertyRelative("_target")));
            root.Add(new PropertyField(property.FindPropertyRelative("_normalValue")));

            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_highlightedValue"), "Highlighted");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_pressedValue"), "Pressed");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_selectedValue"), "Selected");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_disabledValue"), "Disabled");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_premiumValue"), "Premium");
            return root;
        }
    }

    [CustomPropertyDrawer(typeof(CanvasGroupVC))]
    public class CanvasGroupVCDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.Add(new PropertyField(property.FindPropertyRelative("_canvasGroup")));
            root.Add(new PropertyField(property.FindPropertyRelative("_normalAlpha")));

            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_highlightedAlpha"), "Highlighted");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_pressedAlpha"), "Pressed");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_selectedAlpha"), "Selected");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_disabledAlpha"), "Disabled");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_premiumAlpha"), "Premium");

            VisualComponentEditor.DrawAnimationSettings(root, property);
            return root;
        }
    }

    [CustomPropertyDrawer(typeof(FontVC))]
    public class FontVCDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.Add(new PropertyField(property.FindPropertyRelative("_text")));
            root.Add(new PropertyField(property.FindPropertyRelative("_normalFont")));

            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_highlightedFont"), "Highlighted");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_pressedFont"), "Pressed");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_selectedFont"), "Selected");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_disabledFont"), "Disabled");
            VisualComponentEditor.CreateStateField(root, property.FindPropertyRelative("_premiumFont"), "Premium");
            return root;
        }
    }

    public static class VisualComponentEditor
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
            root.Add(animHeader);

            root.Add(new PropertyField(property.FindPropertyRelative("_animate")));
            root.Add(new PropertyField(property.FindPropertyRelative("_duration")));
            root.Add(new PropertyField(property.FindPropertyRelative("_useUnscaledTime")));
        }
    }
}
