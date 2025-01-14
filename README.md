# Quran CLI

> This project is in beta but it is usable (I think)...

The Quran CLI is a command-line tool for exploring, annotating, and linking verses of the Quran.

## Installation

You can execute the following command to install the beta release.

```bash
curl -L -o /tmp/quran-cli_linux-x64.zip https://github.com/yojoecapital/quran-cli/releases/download/v1.0.0-beta/quran-cli_linux-x64.zip && unzip -o /tmp/quran-cli_linux-x64.zip -d /tmp/quran-cli_temp && sudo mkdir -p /opt/quran-cli && sudo mv /tmp/quran-cli_temp/* /opt/quran-cli/ && sudo ln -sf /opt/quran-cli/quran /usr/local/bin/quran
```

## Usage

Use `quran -h` to get a help message. I will write more documentation later, إن شاء الله.

## Building

To compile the project and run it as a self-contained executable:

```bash
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true
```

This will generate a single executable file. Replace `linux-x64` with whatever OS your using.
