# Changelog

All notable changes to this package are documented in this file.

## [1.0.0] - 2026-07-28

- Added a Unity Package Manager manifest.
- Updated the runtime implementation from the latest project source.
- Added `MonoViewObserver` to cover presenter disposal before `Awake`.
- Added safe presenter lookup through `TryGetPresenter`.
- Split the custom `MonoView` inspector into an Editor-only assembly.
- Added Runtime and Editor assembly definitions for package isolation.
- Preserved the original package script GUIDs for compatibility.
- Rebuilt `DisposablePoco` around the package's actual requirements and removed the unused collection implementation.
