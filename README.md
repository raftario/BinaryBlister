# Binary Blister

**BLISTER HAS BEEN ABANDONNED** - [**NEW PLAYLIST FORMAT**](https://github.com/raftario/blist)

---

*Beat Saber playlist handling library*

## About

This library is a C# implementation of a custom binary format for Beat Saber playlists based on [the Blister format](https://github.com/lolPants/Blister/blob/master/SPEC.md).
The tech specs for the file format can be found in [SPEC.md](SPEC.md).

## Why

Newtonsoft's BSON deserialiser causes pretty important performance issues when used in Beat Saber mods for unknown reasons.
Given the simplicity of the Blister format, ditching BSON and creating a custom binary format was simpler than trying to fix the deserialisation issues.
