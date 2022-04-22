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
As Example 2 however, only scan `*.mkv` files in the specified directory (`-e mkv`):
```
mecon -d /Films -e mkv -p http://192.168.0.12:32400 -t <your-token> -L
```
### Example 4
As Example 2 however, don't specify a directory, simply scan the current directory:
```
mecon -p http://192.168.0.12:32400 -t <your-token> -L
```
### Example 5
As Example 2 however, only seach Plex libraries that contain movies (`-m movie`):
```
mecon -d /Films -p http://192.168.0.12:32400 -t <your-token> -m movie -L
```

## Mecon Options
- **`reconcile --help`**
  Displays help for general mecon reconciliation usage.
- **`-p|--plex-host <ip|host:port>`**
  Specifies the Plex server to use when reconciling media on disk with media in Plex libraries. This URL may be specified with or without a port as necessary.  e.g. `-p http://loacalhost:32400`
- **`-t|--plex-token <your-token>`**
  Specifies the Plex server authentication token. See the [Plex documentation for explanation on how to find your token](https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/).
- **`-d|--directory <directory-path-to-scan>`**
  Specifies the path on the file system that should be scanned. If neither this nor the `-n` option is supplied, the current working directory will be scanned. The scanning performed will be recursive unless the `-r false` option is supplied. e.g. `-d /Video/Films/`
- **`-e|--file-extensions <csv-list-of-extensions>`**
  Provide a list of file extensions to scan for in the file system. This can be used to improve performance or simplify output where only specific file extensions are of interest. The extensions are supplied as a comma separated list without dot prefixes. If this option is ommited, all filetypes will be included during scanning. e.g `-e mkv,mp4,ts`
- **`-n|--directory-definition-name <name>`**
  Where a preconfigured directory definition exists, it can be used as the directory for scanning by specifying its name rather than having to explicitly specify the directory and any list of file extensions. If neither this nor the `-d` option is supplied, the current working directory will be scanned. The scanning performed will be recursive unless the `-r false` option is supplied. e.g. `-n Films`
- **`-r|--recurse`**
  By default, scanning of filesystem directories is recursive. This can be turned off and made non-recursive using `-r false` or the default behaviour of enabling recursion can be made explicit using `-r true`. 
- **`-m|--media-types`**
 Comma-separated list of Plex library media types that should be reconciled against to avoid searching through libraries that contain other media types. Possible options are 'Movie' or 'TvShow'. This option is only valid when the -d option (--directory-definition-name) is supplied. If this is omitted, libraries of all media types will be reconciled against. e.g. `-m movies`

## Output Options
In addition to the reconciliation options above, at least one output option must be supplied to control what is returned by mecon.
- **`-L|--missing-from-library`**
  Output a list of files that are present in the filesystem but missing from the any Plex library. The list could represent:
    - Files that the Plex scanner failed to add for some reason.
    - Files that were removed from the Plex library and _may_ no longer be needed on the file system.
- **`-l|--present-in-library`**
  Output a list of files that are present in the filesystem and also present in a Plex library. The list could represent:
    - Files that you believe shouldn't have been added to Plex and need investigating.
    - Files that have been added to the wrong Plex library when used in conjuction with the `-m` option.

## Configuration
Some options do not change very often and you may like to set them permanently rather than entering them every time on the command line. To do this, they can be pre-configured with using environment variables or in an appsettings.json file in the same folder as the mecon binary. In the case that a setting is configured or provided on the command line more than once there is an order of precedence where an option on the command line will trump all other configuration:
1. appsettings.json
2. Environment variable
3. Command line option
### Command Line Options also Available as Config

|Command Line Option|        Environment Variable         |           appsettings.json             |
|:-----------------:|:-----------------------------------:|:--------------------------------------:|
|  -p\|--plex-host  | mecon__plex__host = <ip\|host:port> | "Plex": {"BaseUrl": "<ip\|host:port>"} |
|  -t\|--plex-token |  mecon__plex__token = <your-token>  | "Plex": {"AuthToken": "<your-token>"}  |
### Folder Definitions
Since the directories that you wish to scan are likely to be reused over time, it is possible to define folder definitions that specify not only a path to scan but also the file extensions to scan for, the library media types to reconcile against and whether the scanning should be recursive. Each folder definition is then given a name that can be passed on the command line using the `-n` option. The folder definitions can be defined in either environment variables or an appsettngs.json file in the same directory as the mecon binary. The example below shows two folder definitions. One for storing movies and one for storing television shows:

|         appsettings.json            |       Environment Variables            |
|-------------------------------------|----------------------------------------|
| "FileSystem": {<br>    "FolderDefinitions": [<br>    {<br>        "Name": "Films",<br>            "FolderPath": "\\Video\\Films",<br>        "SupportedFileExtensions": [ "mkv", "ts", "mp4" ],<br>        "MediaTypes": [ "Movie" ]<br>    },<br>    {<br>        "Name": "TV",<br>            "FolderPath": "\\Video\\TV",<br>        "SupportedFileExtensions": [ "mkv", "ts", "mp4" ],<br>        "MediaTypes": [ "TvShow" ]<br>    }]<br>} | mecon__FileSystem__FolderDefinitions__1__Name = Films<br>mecon__FileSystem__FolderDefinitions__1__FolderPath = \Video\Films<br>mecon__FileSystem__FolderDefinitions__1__SupportedFileExtensions__1 = mkv<br>mecon__FileSystem__FolderDefinitions__1__SupportedFileExtensions__2 = ts<br>mecon__FileSystem__FolderDefinitions__1__SupportedFileExtensions__3 = mp4<br>mecon__FileSystem__FolderDefinitions__1__MediaTypes__1 = Movie<br>mecon__FileSystem__FolderDefinitions__2__Name = TV<br>mecon__FileSystem__FolderDefinitions__2__FolderPath = \Video\TV<br>mecon__FileSystem__FolderDefinitions__2__SupportedFileExtensions__1 = mkv<br>mecon__FileSystem__FolderDefinitions__2__SupportedFileExtensions__2 = ts<br>mecon__FileSystem__FolderDefinitions__2__SupportedFileExtensions__3 = mp4<br>mecon__FileSystem__FolderDefinitions__2__MediaTypes__1 = TvShow<br>|

## Limitations
mecon uses each file's name and size in bytes to reconcile files in the file system with items in Plex libraries. In the unlikely event that you have files that are considered different but have identical names and sizes, the reconciliation process will provide unreliable results.
