# OOPARTS

A file drop for friends.

## Concept

OOPARTS aims to be, primarily, easy to use and set up.
It is not intended to handle incredibly large amounts of users or data.
Instead, it aims to be usable by friend groups who want to circumvent file size
limitations on e.g. instant messaging services.

## Implementation

OOPARTS uses ASP.NET with Native AOT, to support ease of development while
keeping deployment reasonably simple. This results in a single binary
that should be usable wherever .NET is.

It is developed using Nix, to support reproducible builds
and development environments.

## Can I use it?

**No!** Please don't! OOPARTS isn't ready for public use, just yet.
Things that are missing include:

- **Uploading and downloading files**
- File size and type limitations
  (to prevent abuse)
- Built-in HTTPS
  (to avoid forcing the use of a reverse proxy for simplicity's sake)
- Any kind of authentication
  (as it is now, upload batches can be deleted by anybody)
- Linux distribution packages
- API documentation

If OOPARTS as a concept sounds useful to you, it may be worth keeping an eye on
the project, to know when it has improved enough for general use.

----
<sup>created on linux by hobbyists. [problem,](https://isdotnetopen.com/) ms? :trollface:</sup>
