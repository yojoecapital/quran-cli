I'm excited to announce the official release of the Quran CLI. This version is stable and ready for use, أنشاء الله. It brings key features for interacting with the Quran and managing notes related to specific verses or chapters.

## Features

- `verse <selection>`: display a verse or range of verses from the Quran
- `chapter <selection>`: output information for a chapter or range of chapters from the Quran
- `search <query>`: search for a verse from the Quran
- `note [command]`: you can create and manage notes
  - these notes are indexed based on what verses you reference
  - you can reference verses with macros like `{<selection>}` (which will expand to the selected verse or chapter) or hashtags like `#<selection>`
  - you can then filter on these references to quickly scan your notes
  - notes use basic markdown syntax and console output is color-coded accordingly
  - for more help on notes, use the `--help` flag

### Database Download
The Quran CLI will download a pre-built database to `~/.config/quran-cli/data.db`.  You can also access the database at https://yojoecapital.github.io/quran-cli/.

### Installation

To install the Quran CLI on Linux, run the following command:

```bash
curl -L -o /tmp/quran https://github.com/yojoecapital/quran-cli/releases/latest/download/quran && chmod 755 /tmp/quran && sudo mv /tmp/quran /usr/local/bin/
```

This will download and install the executable to `/usr/local/bin/`, making it accessible from anywhere in your terminal.