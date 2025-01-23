## Changes

- notes are a bit more strict with how they handle references
- there are 2 kinds of references the user can make in a note:
  - a macro like `{<selection>}` which will expand to the selected verses when the note renders
  - or a tag like `#<selection>` which will display the name of the selection
- notes must include at least one reference. To force a note with no references to be made, use `--force`
- `note list` now can take the option `--by` to filter your input selection by either `macro`, `tag`, or `both`
- you can use `note get` to get notes by their IDs

### Examples

#### References being used in a note

```markdown
# This is an example note

- this is an *example* of a tag: #al-mulk
- this renders to #[Al-Mulk]
- this is an *example* of a selection: {67:30}
- this renders to قل أرأيتم إن أصبح ماؤكم غورا فمن يأتيكم بماء معين
```

#### Using `note list`

```bash
quran note list
# this will list all the notes that have macro references (--by has a default of macro)
quran note list --by both
# this will list all the notes
quran note list --by tag
# this will list all the notes that have tags
quran note list al-baqara --by tag
# this will list all the notes that have tags in Al-Baqara
```

### Database Download

The Quran CLI will download a pre-built database to `~/.config/quran-cli/data.db`.  You can also access the database at https://yojoecapital.github.io/quran-cli/.

### Installation

To install the Quran CLI on Linux, run the following command:

```bash
curl -L -o /tmp/quran https://github.com/yojoecapital/quran-cli/releases/latest/download/quran && chmod 755 /tmp/quran && sudo mv /tmp/quran /usr/local/bin/
```

This will download and install the executable to `/usr/local/bin/`, making it accessible from anywhere in your terminal.