# Binary Blister File Format

See [the original Blister spec](https://github.com/lolPants/Blister/blob/master/SPEC.md) for more details on what each value represents.

## File Encoding

The playlist data is GZip compressed and prepended with the magic number `Blist.v3`.
All integers are unsigned and little-endian. All strings are UTF-8.

## File Structure

- Integers are represented by their length in bits
- Strings are represented by `s`
- Binary data is represented by `b`
- Fixed size binary data is represented by its length prefixed with `b`
- Fields with constant values have it specified inside `[`brackets`]`
- When an optional value is ommited, its length is `0` and it has no data

### Playlist

```
Title
8 --- Length
s --- Data

Author
8 --- Length
s --- Data

Description (optional)
16 -- Length
s --- Data

Cover (optional)
32 -- Length
b --- Data

Maps
32 -- Count
b --- Data
```

### Beatmap

Beatmaps come in different variants identified by their type.

#### Beatsaver key

```
8 --- Type [0]

64 -- Date

32 -- Key
```

#### Beatmap hash

```
8 --- Type [1]

64 -- Date

b20 - Hash
```

#### Zip

```
8 --- Type [2]

64 -- Date

Zip
32 -- Length
b --- Data
```

#### Beatmap Level ID

```
8 --- Type [3]

64 -- Date

Level ID
8 --- Length
s --- Data
```
