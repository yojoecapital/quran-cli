This is a pre-release version of the Quran CLI. It is still in beta, but functional and ready for use.

## Features

- `verse <selection>`: display a verse or range of verses from the Quran
- `chapter <selection>`: output information for a chapter or range of chapters from the Quran
- `search <query>`: search for a verse from the Quran
- `note [command]`: you can now create and manage notes
  - these notes are indexed based on what verses you reference
  - you can reference verses with expansions like `{<selection>}` (which will expand to the selected verse or chapter) or hashtags like `#<selection>`
  - you can then filter on these references to quickly scan your notes
  - notes use basic markdown syntax and console output is color-coded accordingly
  - for more help on notes, use the `--help` flag


The Quran CLI downloads a pre-built database to `~/.config/quran-cli/data.db`, available at https://yojoecapital.github.io/quran-cli/.

## Installation

```bash
curl -L -o /tmp/quran https://github.com/yojoecapital/quran-cli/releases/download/1.0.0-beta/quran && chmod 755 /tmp/quran && sudo mv /tmp/quran /usr/local/bin/
```
