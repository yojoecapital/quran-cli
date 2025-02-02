## Changes

You can pass the `--index` flag to the `search` command to output verses along with their indexes.

### Examples

```bash
echo وَلا تضرونه ش | quran search --index --limit 1
```

The above command will return this result:

```yaml
 - result:
    chapter: 11
    number: 57
    text: فإن [0] تولوا [1] فقد [2] أبلغتكم [3] ما [4] أرسلت [5] به [6] إليكم [7] ويستخلف [8] ربي [9] قوما [10] غيركم [11] ولا [12] تضرونه [13] شيئا [14] إن [15] ربي [16] على [17] كل [18] شيء [19] حفيظ [20]
    translation: 'But if they turn away, [say], "I have already conveyed that with which I was sent to you. My Lord will give succession to a people other than you, and you will not harm Him at all. Indeed my Lord is, over all things, Guardian'
  score: 77

```


### Installation

To install the Quran CLI on Linux, run the following command:

```bash
curl -L -o /tmp/quran https://github.com/yojoecapital/quran-cli/releases/latest/download/quran && chmod 755 /tmp/quran && sudo mv /tmp/quran /usr/local/bin/
```

This will download and install the executable to `/usr/local/bin/`, making it accessible from anywhere in your terminal.