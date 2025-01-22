# Quran CLI

> This project is in beta but it is usable (I think)...

The Quran CLI is a command-line tool for exploring, annotating, and linking verses of the Quran.

## Installation

You can execute the following command to install the beta release.

```bash
curl -L -o /tmp/quran https://github.com/yojoecapital/quran-cli/releases/download/1.0.0-beta/quran && chmod 755 /tmp/quran && sudo mv /tmp/quran /usr/local/bin/
```

## Usage

Use `quran -h` to get a help message. I will write more documentation later, إن شاء الله.

## Building

To compile the project and run it as a self-contained executable:

```bash
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true
```

This will generate a single executable file. Replace `linux-x64` with whatever OS your using.
