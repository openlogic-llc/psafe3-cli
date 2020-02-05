# Psafe3 CLI

This is a combination of code from [WinAuth](https://github.com/winauth/winauth) and [Password Safe Reader for Windows Mobile](https://www.codeproject.com/Articles/34083/Password-Safe-Reader-for-Windows-Mobile). This is inspired by how [Strongbox](https://strongboxsafe.com/) has integrated Psafe3 files and TOTP codes. This is the most basic implementation to prove out the concept of parsing the Strongbox topt urls and getting the codes.    

Usage:  

`psafe3-cli [path to psafe3 file] [master password for psafe3 file]`  

Output:  

```
Cloudflare: 987877
Upcloud: 327872
Zoho: 283943
```