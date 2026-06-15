using UI.CustomButtons;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Editor.CustomButtons
{
    public abstract class VisualComponentEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            DrawCustomInspector(root);

            return root;
        }

        protected virtual void DrawCustomInspector(VisualElement root)
        {
            // Default implementation just draws all properties
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
        }

        protected void CreateStateField(VisualElement root, string propertyName, string label)
        {
            var property = serializedObject.FindProperty(propertyName);
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
    }

    [CustomEditor(typeof(GraphicsColorVC))]
    public class GraphicsColorVCEditor : VisualComponentEditor
    {
        protected override void DrawCustomInspector(VisualElement root)
        {
            root.Add(new PropertyField(serializedObject.FindProperty("_graphic")));
            root.Add(new PropertyField(serializedObject.FindProperty("_normalColor")));
            
            CreateStateField(root, "_highlightedColor", "Highlighted");
            CreateStateField(root, "_pressedColor", "Pressed");
            CreateStateField(root, "_selectedColor", "Selected");
            CreateStateField(root, "_disabledColor", "Disabled");

            var animHeader = new Label("Animation");
            animHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            animHeader.style.marginTop = 10;
            root.Add(animHeader);

            root.Add(new PropertyField(serializedObject.FindProperty("_animate")));
            root.Add(new PropertyField(serializedObject.FindProperty("_duration")));
            root.Add(new PropertyField(serializedObject.FindProperty("_useUnscaledTime")));
        }
    }

    [CustomEditor(typeof(GameObjectToggleVC))]
    public class GameObjectToggleVCEditor : VisualComponentEditor
    {
        protected override void DrawCustomInspector(VisualElement root)
        {
            root.Add(new PropertyField(serializedObject.FindProperty("_target")));
            root.Add(new PropertyField(serializedObject.FindProperty("_normalValue")));

            CreateStateField(root, "_highlightedValue", "Highlighted");
            CreateStateField(root, "_pressedValue", "Pressed");
            CreateStateField(root, "_selectedValue", "Selected");
            CreateStateField(root, "_disabledValue", "Disabled");
        }
    }

    [CustomEditor(typeof(CanvasGroupVC))]
    public class CanvasGroupVCEditor : VisualComponentEditor
    {
        protected override void DrawCustomInspector(VisualElement root)
        {
            root.Add(new PropertyField(serializedObject.FindProperty("_canvasGroup")));
            root.Add(new PropertyField(serializedObject.FindProperty("_normalAlpha")));

            CreateStateField(root, "_highlightedAlpha", "Highlighted");
            CreateStateField(root, "_pressedAlpha", "Pressed");
            CreateStateField(root, "_selectedAlpha", "Selected");
            CreateStateField(root, "_disabledAlpha", "Disabled");

            var animHeader = new Label("Animation");
            animHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            animHeader.style.marginTop = 10;
            root.Add(animHeader);

            root.Add(new PropertyField(serializedObject.FindProperty("_animate")));
            root.Add(new PropertyField(serializedObject.FindProperty("_duration")));
            root.Add(new PropertyField(serializedObject.FindProperty("_useUnscaledTime")));
        }
    }

    [CustomEditor(typeof(FontVC))]
    public class FontVCEditor : VisualComponentEditor
    {
        protected override void DrawCustomInspector(VisualElement root)
        {
            root.Add(new PropertyField(serializedObject.FindProperty("_text")));
            root.Add(new PropertyField(serializedObject.FindProperty("_normalFont")));

            CreateStateField(root, "_highlightedFont", "Highlighted");
            CreateStateField(root, "_pressedFont", "Pressed");
            CreateStateField(root, "_selectedFont", "Selected");
            CreateStateField(root, "_disabledFont", "Disabled");
        }
    }
}
