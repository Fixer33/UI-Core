using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Editor
{
    public static class ModuleListEditorHelper
    {
        private static HashSet<string> _collapsedModules = new HashSet<string>();

        public static void CreateModuleList(VisualElement root, SerializedProperty listProperty, Type baseModuleType, string title)
        {
            var container = new VisualElement();
            container.style.marginTop = 10;
            container.style.borderTopWidth = 1;
            container.style.borderTopColor = Color.gray;
            container.style.paddingTop = 5;

            var headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            headerRow.style.justifyContent = Justify.SpaceBetween;
            headerRow.style.marginBottom = 5;

            var label = new Label(title);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.fontSize = 14;
            headerRow.Add(label);

            var collapseAllButton = new Button(() => {
                for (int i = 0; i < listProperty.arraySize; i++)
                {
                    _collapsedModules.Add(GetPropertyKey(listProperty.GetArrayElementAtIndex(i)));
                }
                RebuildList(container.Q("list-container"), listProperty, baseModuleType);
            }) { text = "Collapse All" };
            headerRow.Add(collapseAllButton);

            container.Add(headerRow);

            var listContainer = new VisualElement();
            listContainer.name = "list-container";
            container.Add(listContainer);

            Action refreshList = () => RebuildList(listContainer, listProperty, baseModuleType);
            refreshList();

            var addButton = new Button(() => ShowAddModuleMenu(listProperty, baseModuleType, refreshList)) { text = "Add Module" };
            container.Add(addButton);

            root.Add(container);
            root.Bind(listProperty.serializedObject);
            
            // Rebuild only if array size changes (e.g. undo/redo of add/remove)
            var lastArraySize = listProperty.arraySize;
            root.schedule.Execute(() => {
                if (listProperty.serializedObject != null && listProperty.serializedObject.targetObject != null)
                {
                    listProperty.serializedObject.Update();
                    if (listProperty.arraySize != lastArraySize)
                    {
                        lastArraySize = listProperty.arraySize;
                        refreshList();
                    }
                }
            }).Every(100);
        }

        private static string GetPropertyKey(SerializedProperty property)
        {
            return property.serializedObject.targetObject.GetInstanceID() + property.propertyPath;
        }

        private static void RebuildList(VisualElement container, SerializedProperty listProperty, Type baseModuleType)
        {
            container.Clear();
            listProperty.serializedObject.Update();

            for (int i = 0; i < listProperty.arraySize; i++)
            {
                int index = i;
                var property = listProperty.GetArrayElementAtIndex(index);
                string key = GetPropertyKey(property);
                bool isCollapsed = _collapsedModules.Contains(key);
                
                var box = new Box();
                box.style.marginBottom = 10;
                box.style.paddingTop = 5;
                box.style.paddingBottom = 5;
                box.style.paddingLeft = 5;
                box.style.paddingRight = 5;
                
                var header = new VisualElement();
                header.style.flexDirection = FlexDirection.Row;
                header.style.justifyContent = Justify.SpaceBetween;
                header.style.marginBottom = 5;
                header.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.2f);
                header.style.paddingLeft = 5;

                var leftHeader = new VisualElement();
                leftHeader.style.flexDirection = FlexDirection.Row;

                var foldoutToggle = new Button(() => {
                    if (_collapsedModules.Contains(key))
                        _collapsedModules.Remove(key);
                    else
                        _collapsedModules.Add(key);
                    RebuildList(container, listProperty, baseModuleType);
                }) { text = isCollapsed ? "▶" : "▼" };
                foldoutToggle.style.width = 20;
                foldoutToggle.style.backgroundColor = Color.clear;
                foldoutToggle.style.borderLeftWidth = 0;
                foldoutToggle.style.borderRightWidth = 0;
                foldoutToggle.style.borderTopWidth = 0;
                foldoutToggle.style.borderBottomWidth = 0;
                leftHeader.Add(foldoutToggle);

                string typeName = "Missing Type";
                if (!string.IsNullOrEmpty(property.managedReferenceFullTypename))
                {
                    typeName = property.managedReferenceFullTypename.Split(' ').Last().Split('.').Last();
                }
                
                var typeLabel = new Label(typeName);
                typeLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                typeLabel.style.alignSelf = Align.Center;
                leftHeader.Add(typeLabel);
                header.Add(leftHeader);

                var controls = new VisualElement();
                controls.style.flexDirection = FlexDirection.Row;

                var upButton = new Button(() => {
                    MoveItem(listProperty, index, -1, () => RebuildList(container, listProperty, baseModuleType));
                }) { text = "▲" };
                var downButton = new Button(() => {
                    MoveItem(listProperty, index, 1, () => RebuildList(container, listProperty, baseModuleType));
                }) { text = "▼" };
                var removeButton = new Button(() => 
                {
                    listProperty.serializedObject.Update();
                    if (index < listProperty.arraySize)
                    {
                        listProperty.DeleteArrayElementAtIndex(index);
                        
                        if (index < listProperty.arraySize && listProperty.GetArrayElementAtIndex(index).managedReferenceValue == null)
                        {
                            listProperty.DeleteArrayElementAtIndex(index);
                        }
                        
                        listProperty.serializedObject.ApplyModifiedProperties();
                        RebuildList(container, listProperty, baseModuleType);
                    }
                }) { text = "X" };

                upButton.style.width = 20;
                downButton.style.width = 20;
                removeButton.style.width = 20;

                controls.Add(upButton);
                controls.Add(downButton);
                controls.Add(removeButton);
                header.Add(controls);
                
                box.Add(header);

                if (!isCollapsed)
                {
                    // Validation Warning
                    var module = property.managedReferenceValue;
                    if (module != null)
                    {
                        var isValidMethod = module.GetType().GetMethod("IsValid");
                        if (isValidMethod != null)
                        {
                            object[] args = new object[] { null };
                            bool isValid = (bool)isValidMethod.Invoke(module, args);
                            if (!isValid)
                            {
                                var warning = new HelpBox((string)args[0], HelpBoxMessageType.Error);
                                box.Add(warning);
                            }
                        }
                    }

                    var propertyField = new PropertyField(property);
                    propertyField.label = "";
                    propertyField.BindProperty(property);
                    box.Add(propertyField);
                }

                container.Add(box);
            }
        }

        private static void MoveItem(SerializedProperty listProperty, int index, int direction, Action onComplete)
        {
            int newIndex = index + direction;
            if (newIndex >= 0 && newIndex < listProperty.arraySize)
            {
                listProperty.MoveArrayElement(index, newIndex);
                listProperty.serializedObject.ApplyModifiedProperties();
                onComplete?.Invoke();
            }
        }

        private static void ShowAddModuleMenu(SerializedProperty listProperty, Type baseModuleType, Action onComplete)
        {
            var menu = new GenericMenu();
            var types = TypeCache.GetTypesDerivedFrom(baseModuleType)
                .Where(t => !t.IsAbstract && t.IsClass);

            foreach (var type in types)
            {
                menu.AddItem(new GUIContent(type.Name), false, () => 
                {
                    int index = listProperty.arraySize;
                    listProperty.InsertArrayElementAtIndex(index);
                    var element = listProperty.GetArrayElementAtIndex(index);
                    element.managedReferenceValue = Activator.CreateInstance(type);
                    listProperty.serializedObject.ApplyModifiedProperties();
                    onComplete?.Invoke();
                });
            }
            menu.ShowAsContext();
        }
    }
}
