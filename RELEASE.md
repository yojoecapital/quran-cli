## Changes

- the `<selection>` argument gives a couple more ways of selecting. This can be used in the `verse` command, in notes, or in searches
  - select by pages by using `p<number>`
  - select by Juz by using `j<number>`
- the `search` command has some new features too
  - search by translation by passing the `--translation` flag
  - filter your search to a particular spot by passing a selection option like `--selection <selection>`
- search your notes' text with `note search`

### Examples

#### Using pages in selections

```bash
quran verse p1
# returns all the verses on page 1
quran verse p1..p3
# returns all the verses between pages 1 and 3
quran verse p600..
# returns all the verses from page 600 to the end (604)
quran verse j1
# returns all the verses in Juz 1
quran verse j1..j2
# returns all the verses between Juz 1 and 2
quran verse p10..j2
# returns all the verses between page 10 and Juz 2
```

#### Searching

Both of these commands will return this verse:

```yaml
- result:
    chapter: 2
    number: 156
    text: الذين إذا أصابتهم مصيبة قالوا إنا لله وإنا إليه راجعون
    translation: 'Who, when disaster strikes them, say, "Indeed we belong to Allah, and indeed to Him we will return'
  score: 82
```

##### By translation

```bash
quran search Those who when disaster strikes, they say Indeed we belong to Allah and we will return to Him! --translation --limit 1
```

##### Filtering by selection

```bash
# note that you can also pipe input to the 'search' command
echo انا لله وانا اليه راجعون| quran search --selection al-baqara --limit 1
```

### Installation

To install the Quran CLI on Linux, run the following command:

```bash
curl -L -o /tmp/quran https://github.com/yojoecapital/quran-cli/releases/latest/download/quran && chmod 755 /tmp/quran && sudo mv /tmp/quran /usr/local/bin/
```

This will download and install the executable to `/usr/local/bin/`, making it accessible from anywhere in your terminal.