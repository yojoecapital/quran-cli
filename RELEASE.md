This is a pre-release version of the Quran CLI. It is still in beta, but functional and ready for use.

## Features

- **`verse <selection>`**: Display a verse or range of verses from the Quran.
- **`chapter <selection>`**: Output information for a chapter or range of chapters from the Quran.
- **`search <query>`**: Search for a verse from the Quran.

The Quran CLI downloads a pre-built database to `~/.config/quran-cli/data.db`, available at https://yojoecapital.github.io/quran-cli/.

## Installation

```bash
curl -L -o /tmp/quran https://github.com/yojoecapital/quran-cli/releases/download/0.1.0-beta/quran && chmod 755 /tmp/quran && sudo mv /tmp/quran /usr/local/bin/
```
