# Quran CLI

The Quran CLI is a command-line tool for exploring, annotating, and linking verses of the Quran.

## Features

- `verse <selection>`: display a verse or range of verses from the Quran
- `chapter <selection>`: output information for a chapter or range of chapters from the Quran
- `search <query>`: search for a verse from the Quran
- `note [command]`: you can create and manage notes
  - these notes are indexed based on what verses you reference
  - you can reference verses with expansions like `{<selection>}` (which will expand to the selected verse or chapter) or hashtags like `#<selection>`
  - you can then filter on these references to quickly scan your notes
  - notes use basic markdown syntax and console output is color-coded accordingly
  - for more help on notes, use the `--help` flag

## Installation

You can execute the following command to install the beta release.

```bash
curl -L -o /tmp/quran https://github.com/yojoecapital/quran-cli/releases/latest/download/quran && chmod 755 /tmp/quran && sudo mv /tmp/quran /usr/local/bin/
```

## Usage

Use `quran -h` to get a help message. I will write more documentation later, إن شاء الله.

## Building

To compile the project and run it as a self-contained executable:

```bash
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true
```

This will generate a single executable file. Replace `linux-x64` with whatever OS your using.
