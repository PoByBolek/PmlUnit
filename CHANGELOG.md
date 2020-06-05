# Changelog

## [v0.4.0] - 2020-06-05

### Added
- Allow to open files by double clicking on tests and clicking on links in stack traces. (See [Opening Files].)
- Add customization files for Hull & Outfitting 12.1. Thanks yang827!

### Changed
- Use the last test grouping when starting a new instance of PDMS, OH, or E3D (instead of defaulting to group by test result).

### Fixed
- Try to use the same font as the other windows in PDMS, OH, and E3D regardless of the fonts and locale configured in Windows. (See [issue #2].)

  [Opening Files]: https://github.com/PoByBolek/PmlUnit#opening-files
  [issue #2]: https://github.com/PoByBolek/PmlUnit/issues/2


## [v0.3.0] - 2020-01-12

### Added
- Allow to change test grouping behavior.
- Allow to move the focus in the test list with the <kbd>PageUp</kbd>, <kbd>PageDown</kbd>, <kbd>Home</kbd>, and <kbd>End</kbd> keys.

### Changed
- Group tests by their test result by default (instead of by test case name).


## [v0.2.0] - 2019-08-09

### Added
- Add a custom tree view that allows to select multiple entries at once.
- Add an About dialog that shows copyright and license information.

### Changed
- Load the test cases during addin startup instead of when the test runner window first opens.

### Fixed
- Fix a startup crash in E3D 1.1.
- Show the test cases when restarting E3D with the test runner window still open.


## [v0.1.0] - 2019-02-22

Initial Release


  [Unreleased]: https://github.com/PoByBolek/PmlUnit/compare/master...develop
  [v0.4.0]: https://github.com/PoByBolek/PmlUnit/releases/tag/v0.4.0
  [v0.3.0]: https://github.com/PoByBolek/PmlUnit/releases/tag/v0.3.0
  [v0.2.0]: https://github.com/PoByBolek/PmlUnit/releases/tag/v0.2.0
  [v0.1.0]: https://github.com/PoByBolek/PmlUnit/releases/tag/v0.1.0
