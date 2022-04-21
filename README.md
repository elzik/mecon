# mecon
[![.NET](https://github.com/elzik/mecon/actions/workflows/continous-integration.yml/badge.svg)](https://github.com/elzik/mecon/actions/workflows/continous-integration.yml)

## Introduction
**Me**dia R**econ**ciler, or simply _mecon_, is a cross-platform command line tool which reconciles media within a directory with media in a Plex library. It helps answer simple questions such as:
- Given a list of files in a directory, which ones have failed to have been added to a Plex library?
- Given a list of files in a directory, which ones exist in a Plex library?

## Example Usage without Configuration

### Example 1
Display help text documenting reconcilation options:
```
mecon reconcile --help
```
### Example 2
Scan all files in the specified directory (`-d /path`) and list all files that are not found (`-L`) in a Plex TV or Movie library using the specified Plex server (`-p <url>`) and [your Plex auth token](https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/) (`-t <your-token>`):
```
mecon -d /Films -p http://192.168.0.12:32400 -t <your-token> -L
```
### Example 3
The same as Example 2 however, only scan `*.mkv` files in the specified directory (`-e mkv`).
```
mecon -d /Films -e mkv -p http://192.168.0.12:32400 -t <your-token> -L
```
### Example 4
The same as Example 2 however, don't specify a directory, simply scan the current directory.
```
mecon -p http://192.168.0.12:32400 -t <your-token> -L
```
### Example 5
The same as Example 2 however, only seach Plex libraries that contain movies (`-m movie`).
```
mecon -d /Films -p http://192.168.0.12:32400 -t <your-token> -m movie -L
```

## Reconsilitaion Options
- **`reconcile --help`**
  Displays help for general mecon reconciliation usage.
- **`-p|-plex-host <ip|host:port>`**
  Specifies the Plex server to use when reconciling media on disk with media in Plex libraries. This URL may be specified with or without a port as necessary.  e.g. `-p http://loacalhost:32400`
- **`-t|--plex-token <your-token>`**
  Specifies the Plex server authentication token. See the [Plex documentation for explanation on how to find your token](https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/).
- **`-d|--directory <directory-path-to-scan>`**
  Specifies the path on the file system that should be scanned. If neither this nor the `-n` option is supplied, the current working directory will be scanned. The scanning performed will be recursive unless the `-r false` option is supplied. e.g. `-d /Video/Films/`
- **`-e|--file-extensions <csv-list-of-extensions>`**
  Provide a list of file extensions to scan for in the file system. This can be used to improve performance or simplify output where only specific file extensions are of interest. The extensions are supplied as a comma separated list without dot prefixies. If this option is ommited, all filetypes will be included during scanning. e.g `-e mkv,mp4,ts`
- **`-n|--directory-definition-name <name>`**
  Where a preconfigured directory definition exists, it can be used as the dircteory for scanning by specifying it's name rather than having to explicitly specify the directory and any list of file extensions. If neither this nor the `-d` option is supplied, the current working directory will be scanned. The scanning performed will be recursive unless the `-r false` option is supplied. e.g. `-n Films`
- **`-r|--recurse`**
  By default, scanning of filesystem directories is recursive. This can be turned off and made non-recurive using `-r false` or the default behaviour of enabling recursion can be made explicit using `-r true`. 
- **`-m|--media-types`**
 Comma-separated list of Plex library media types that should be reconciled against to avoid searching through libraries that contain other media types. Possible options are 'Movie' or 'TvShow'. This option is only valid when the -d option (--directory-definition-name) is supplied. If this is omitted, libraries of all media types will be reconciled against. e.g. `-m movies`