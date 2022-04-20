# mecon
[![.NET](https://github.com/elzik/mecon/actions/workflows/continous-integration.yml/badge.svg)](https://github.com/elzik/mecon/actions/workflows/continous-integration.yml)

## Introduction
**Me**dia R**econ**ciler, or simply _mecon_, is a cross-platform command line tool which reconciles media within a directory with media in a Plex library. It helps answer simple questions such as:
- Given a list of files in a directory, which ones have failed to have been added to a Plex library?
- Given a list of files in a directory, which ones exist in a Plex library?

## Example Usage without Configuration

### Example 1
Scan all files in the specified directory (`-d /path`) and list all files that are not found (`-L`) in a Plex TV or Movie library using the specified Plex server (`-p <url>`) and [your Plex auth token](https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/) (`-t <your-token>`):
```
mecon -d /Films -p http://192.168.0.12:32400 -t <your-token> -L
```
### Example 2
The same as Example 1 however, only scan `*.mkv` files in the specified directory (`-e mkv`).
```
mecon -d /Films -e mkv -p http://192.168.0.12:32400 -t <your-token> -L
```
### Example 3
The same as Example 1 however, don't specify a directory, simply scan the current directory.
```
mecon -p http://192.168.0.12:32400 -t <your-token> -L
```
### Example 4
The same as Example 1 however, only seach Plex libraries that contain movies (`-m movie`).
```
mecon -d /Films -p http://192.168.0.12:32400 -t <your-token> -m movie -L
```