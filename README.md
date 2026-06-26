# UI Core

![Unity](https://img.shields.io/badge/Unity-UPM%20Package-blue)
![GitHub](https://img.shields.io/github/license/Fixer33/UI-Core)

Foundational abstractions and base classes for building modular UI systems in Unity.

## Features

- View lifecycle with events: `Show`, `Hide`, `ShowInstant`, `HideInstant`, and visibility notifications.
- Canvas Views that properly toggle both `GameObject` and `Canvas.enabled` for accurate visibility.
- View Extensions system: implement `IViewExtension` or inherit `ViewExtensionBase` to hook into view lifecycle.
- Navigation Attributes: `ViewShowButtonAttribute` for automatic view navigation via UI Buttons.
- Modular `SelectableElement` and `CustomButton` with a serializable module system for flexible visual transitions.
- Integrated UIToolkit editors for managing visual modules directly on components.
- Selectable groups with on-demand initialization to avoid unnecessary setup.
- View Collection Overrides for platform and screen-ratio specific view selection.
- Standalone animation system for visual transitions, supporting DOTween-independent logic.

## Installation

### Using UPM (Unity Package Manager)

1. Open Unity and go to **Window > Package Manager**.
2. Click the **+** button and select **Add package from git URL**.
3. Enter the repository URL:
   ```
   https://github.com/Fixer33/UI-Core.git
   ```
4. Click **Add** and wait for Unity to install the package.

## Usage

### View Extensions

Create a component that extends `ViewExtensionBase<TView>` and attach it to the same GameObject as your view.

```csharp
public class AnalyticsViewExt : ViewExtensionBase<UICanvasView>
{
    protected override void OnShowStart() { }
    protected override void OnShown() { }
    protected override void OnHideStart() { }
    protected override void OnHidden() { }
}
```

### Selectable Elements & Custom Buttons

Use `SelectableElement` or `CustomButton` to handle UI interactions. Visuals are managed via a modular system:
1. Add `SelectableElement` or `CustomButton` component.
2. In the inspector, use the **Add Module** button to attach visual behaviors (Color, Scale, Canvas Group, Font, etc.).
3. Modules are serializable and do not require separate `MonoBehaviour` components on your GameObject.

### Navigation Attributes

Use `ViewShowButtonAttribute` on a `Button` field in your `UICanvasView` to automatically subscribe it to show another view.

```csharp
public class MainMenuView : UICanvasView
{
    [SerializeField, ViewShowButton(typeof(SettingsView))] 
    private Button _settingsButton;
}
```

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.