## Changes

You can now select by pages in the `<selection>` argument by use `p\d+`. This can be used in the `verse` command as well as in notes.

### Examples

```bash
quran verse p1
# returns all the verses on page 1
quran verse p1..p3
# returns all the verses between pages 1 and 3
quran verse p600..
# returns all the verses from page 600 to the end (604)
```

### Installation

To install the Quran CLI on Linux, run the following command:

```bash
curl -L -o /tmp/quran https://github.com/yojoecapital/quran-cli/releases/latest/download/quran && chmod 755 /tmp/quran && sudo mv /tmp/quran /usr/local/bin/
```

This will download and install the executable to `/usr/local/bin/`, making it accessible from anywhere in your terminal.