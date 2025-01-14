# Quran CLI

> This project is in beta but it is usable (I think)...

The Quran CLI is a command-line tool for exploring, annotating, and linking verses of the Quran.

## Installation

Check out the releases page.

## Usage

Use `quran -h` to get a help message. I will write more documentation later, إن شاء الله.

## Building

To compile the project and run it as a self-contained executable:

```bash
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true
```

This will generate a single executable file. Replace `linux-x64` with whatever OS your using.