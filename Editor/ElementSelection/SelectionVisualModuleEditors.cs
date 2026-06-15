using UI.ElementSelection.VisualModules;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Editor.ElementSelection
{
    public abstract class SelectionVisualModuleEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            DrawCustomInspector(root);
            return root;
        }

        protected virtual void DrawCustomInspector(VisualElement root)
        {
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

        protected void DrawAnimationSettings(VisualElement root)
        {
            var animHeader = new Label("Animation");
            animHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            animHeader.style.marginTop = 10;
            animHeader.style.marginBottom = 2;
            root.Add(animHeader);

            root.Add(new PropertyField(serializedObject.FindProperty("_animate")));
            root.Add(new PropertyField(serializedObject.FindProperty("_duration")));
            root.Add(new PropertyField(serializedObject.FindProperty("_useUnscaledTime")));
        }
    }

    [CustomEditor(typeof(GraphicsColorSelectionVisualModule))]
    public class GraphicsColorSelectionVisualModuleEditor : SelectionVisualModuleEditor
    {
        protected override void DrawCustomInspector(VisualElement root)
        {
            root.Add(new PropertyField(serializedObject.FindProperty("_graphic")));
            root.Add(new PropertyField(serializedObject.FindProperty("_defaultColor")));
            root.Add(new PropertyField(serializedObject.FindProperty("_selectedColor")));
            
            CreateStateField(root, "_hoveredColor", "Hovered Color");
            CreateStateField(root, "_premiumColor", "Premium Color");

            DrawAnimationSettings(root);
        }
    }

    [CustomEditor(typeof(CanvasGroupSelectionVisualModule))]
    public class CanvasGroupSelectionVisualModuleEditor : SelectionVisualModuleEditor
    {
        protected override void DrawCustomInspector(VisualElement root)
        {
            root.Add(new PropertyField(serializedObject.FindProperty("_group")));
            root.Add(new PropertyField(serializedObject.FindProperty("_defaultState")));
            root.Add(new PropertyField(serializedObject.FindProperty("_selectedState")));
            
            CreateStateField(root, "_hoveredState", "Hovered State");
            CreateStateField(root, "_premiumState", "Premium State");

            DrawAnimationSettings(root);
        }
    }

    [CustomEditor(typeof(FontSelectionVisualModule))]
    public class FontSelectionVisualModuleEditor : SelectionVisualModuleEditor
    {
        protected override void DrawCustomInspector(VisualElement root)
        {
            root.Add(new PropertyField(serializedObject.FindProperty("_text")));
            root.Add(new PropertyField(serializedObject.FindProperty("_defaultFont")));
            root.Add(new PropertyField(serializedObject.FindProperty("_selectedFont")));
            
            CreateStateField(root, "_hoveredFont", "Hovered Font");
            CreateStateField(root, "_premiumFont", "Premium Font");
        }
    }

    [CustomEditor(typeof(GameObjectToggleSelectionVisualModule))]
    public class GameObjectToggleSelectionVisualModuleEditor : SelectionVisualModuleEditor
    {
        protected override void DrawCustomInspector(VisualElement root)
        {
            root.Add(new PropertyField(serializedObject.FindProperty("_gameObject")));
            
            CreateStateField(root, "_normalValue", "Normal Value");
            CreateStateField(root, "_selectedValue", "Selected Value");
            CreateStateField(root, "_hoveredValue", "Hovered Value");
            CreateStateField(root, "_premiumValue", "Premium Value");
        }
    }
}
