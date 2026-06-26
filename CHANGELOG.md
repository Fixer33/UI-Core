## [1.6.0]

### Added
- Modular visual system for `SelectableElement` and `CustomButton` using `[SerializeReference]`.
- Unified UIToolkit-based custom editor for managing visual modules (Add, Remove, Reorder, Collapse).
- New `ScaleSelectionVisualModule` and `ScaleVC` for scale-based transitions.
- Automatic validation and warnings for modules with missing references in the inspector.

### Changed
- Converted all SVMs (Selection Visual Modules) and VCs (Visual Components) from `MonoBehaviour` to serializable classes.
- `CustomButton` inspector now hides standard Unity Button transition fields to focus on modular visuals.
- Improved runtime stability by gracefully handling destroyed target objects in visual modules.

### Fixed
- Inspector focus loss when editing module field values.
- Multi-click requirement for module management buttons.

## [1.5.0]

### Added
- Parameter overrides

## [1.4.0]

### Added
- Standalone animation system for visual components (DOTween independent).
- `SelectableElement` now supports hover detection and animated visual modules.
- New `StandaloneAnimatedVC` and `StandaloneAnimatedSVM` base classes for custom animations.
- Configurable timescale dependence for animations.
- UIToolkit-based custom editors for visual components and selection modules for improved UX.
- Custom buttons
- Premium states for CustomButton and SelectableElement
- Scriptable property overrides for all visual components and selection modules.

## [1.3.0] - 2026-05-04

### Added
- `ViewShowButtonAttribute`: Allows declarative view navigation by marking `Button` fields in `UICanvasView`.

## [1.2.0] - 2026-03-24

### Added
- View Extensions system: `IViewExtension` and `ViewExtensionBase` to extend view lifecycle.
- New `UIViewCollectionOverride` types for platform and screen ratio based view selection.

### Changed
- `SelectableElement`: Initialization moved to on-demand to avoid unnecessary setup.
- `UICanvasView`: Canvas component is now toggled alongside GameObject for accurate visibility detection.

## [1.1.0] - 2025-10-06

### Added
- Selectable element logic

### Changed
- Loading screen sample updated

## [1.0.0] - 2025-03-10

### Fixed
- Samples script folder for canvas scripts

## [1.0.2] - 2025-03-07

### Changed
- Removed custom localized button and label because Unity already has binding for Localization package

## [1.0.1] - 2025-03-02

### Changed
- Added dependency for core package

## [1.0.0] - 2025-02-24

### Added
- Created first version of package