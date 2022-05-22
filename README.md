# mecon

![Build](https://img.shields.io/github/workflow/status/elzik/mecon/Continuous%20Integration?color=95BE1A)
![Publish](https://img.shields.io/github/workflow/status/elzik/mecon/Publish?color=95BE1A&label=publish)
![Coverage](https://raw.githubusercontent.com/gist/elzik/527882e89a938dc78f61a08c300edec4/raw/c145ab2e9e64bcf0feae9e9f1005010cff4503b7/mecon-code-coverage-main.svg)

## Introduction
**Me**dia R**econ**ciler, or simply _mecon_, is a cross-platform command line tool which reconciles media within a directory with media in a Plex library. It helps answer simple questions such as:
-   Given a list of files in a directory, which ones have failed to have been added to a Plex library?
-   Given a list of files in a directory, which ones exist in a Plex library?

## Example Usage without Configuration

### Example 1
Display help text documenting reconcilation options:
```console
mecon reconcile --help
```console
### Example 2
Scan all files in the specified directory (`-d /path`) and list all files that are not found (`-L`) in a Plex TV or Movie library using the specified Plex server (`-p <url>`) and [your Plex auth token](https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/) (`-t <your-token>`):
```
mecon -d /Films -p http://192.168.0.12:32400 -t <your-token> -L
```console
### Example 3
As Example 2 however, only scan `*.mkv` files in the specified directory (`-e mkv`):
```console
mecon -d /Films -e mkv -p http://192.168.0.12:32400 -t <your-token> -L
```
### Example 4
As Example 2 however, don't specify a directory, simply scan the current directory:
```console
mecon -p http://192.168.0.12:32400 -t <your-token> -L
```
### Example 5
As Example 2 however, only seach Plex libraries that contain movies (`-m movie`):
```console
mecon -d /Films -p http://192.168.0.12:32400 -t <your-token> -m movie -L
```

### Example 6
As Example 2 however, only return files that do not contain the word "sample":
```
mecon -d /Films -p http://192.168.0.12:32400 -L -f (?i)^(?!.*sample).*$
```

## Mecon Options
-   **`reconcile --help`**
 
Displays help for general mecon reconciliation usage.
-   **`-p|--plex-host <ip|host:port>`** Specifies the Plex server to use when reconciling media on disk with media in Plex libraries. This URL may be specified with or without a port as necessary.  e.g. `-p http://loacalhost:32400`
-   **`-t|--plex-token <your-token>`** Specifies the Plex server authentication token. See the [Plex documentation for explanation on how to find your token](https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/).
-   **`-d|--directory <directory-path-to-scan>`** Specifies the path on the file system that should be scanned. If neither this nor the `-n` option is supplied, the current working directory will be scanned. The scanning performed will be recursive unless the `-r false` option is supplied. e.g. `-d /Video/Films/`
-   **`-e|--file-extensions <csv-list-of-extensions>`** Provide a list of file extensions to scan for in the file system. This can be used to improve performance or simplify output where only specific file extensions are of interest. The extensions are supplied as a comma separated list without dot prefixes. If this option is ommited, all filetypes will be included during scanning. e.g `-e mkv,mp4,ts`
-   **`-n|--directory-definition-name <name>`** Where a preconfigured directory definition exists, it can be used as the directory for scanning by specifying its name rather than having to explicitly specify the directory and any list of file extensions. If neither this nor the `-d` option is supplied, the current working directory will be scanned. The scanning performed will be recursive unless the `-r false` option is supplied. e.g. `-n Films`
-   **`-r|--recurse`** By default, scanning of filesystem directories is recursive. This can be turned off and made non-recursive using `-r false` or the default behaviour of enabling recursion can be made explicit using `-r true`. 
-   **`-m|--media-types`** Comma-separated list of Plex library media types that should be reconciled against to avoid searching through libraries that contain other media types. Possible options are 'Movie' or 'TvShow'. This option is only valid when the -d option (--directory-definition-name) is supplied. If this is omitted, libraries of all media types will be reconciled against. e.g. `-m movies`
-   **`-f|--regex-match-filter <regular-expression>`** When scanning the file system, filter to only show files where the filepath matches a regular expression. For example, by specifying `(?i)^(?!.*sample).*$`, the list of files scanned will be filtered to i only shows files that do not contain the word "sample". 

## Output Options
In addition to the reconciliation options above, at least one library option must be supplied to control what is returned by mecon.
-   **`-L|--missing-from-library`** Output a list of files that are present in the filesystem but missing from the any Plex library. The list could represent:
    -   Files that the Plex scanner failed to add for some reason.
    -   Files that were removed from the Plex library and _may_ no longer be needed on the file system.
-   **`-l|--present-in-library`** Output a list of files that are present in the filesystem and also present in a Plex library. The list could represent:
    -   Files that you believe shouldn't have been added to Plex and need investigating.
    -   Files that have been added to the wrong Plex library when used in conjuction with the `-m` option.

## Configuration
Some options do not change very often and you may like to set them permanently rather than entering them every time on the command line. To do this, they can be pre-configured with using environment variables or in an appsettings.json file in the same directory as the mecon binary. Ensure that the case for any settings is correct and that environment variables parts are separated by double underscores (`__`). In the case that a setting is configured or provided on the command line more than once there is an order of precedence where an option on the command line will trump all other configuration:
1. appsettings.json
2. Environment variable
3. Command line option
### Command Line Options also Available as Config

|Command Line Option|        Environment Variable         |           appsettings.json             |
|:-----------------:|:-----------------------------------:|:--------------------------------------:|
|  -p\|--plex-host  | Mecon__Plex__BaseUrl=<ip\|host:port> | "Plex": {"BaseUrl": "<ip\|host:port>"} |
|  -t\|--plex-token |  Mecon__Plex__AuthToken=<your-token>  | "Plex": {"AuthToken": "<your-token>"}  |
### Directory Definitions
Since the directories that you wish to scan are likely to be reused over time, it is possible to define directory definitions that specify not only a path to scan but also the file extensions to scan for, the library media types to reconcile against, whether the scanning should be recursive and a regular expression filter. Each directory definition is then given a name that can be passed on the command line using the `-n` option. The directory definitions can be defined in either environment variables or an appsettngs.json file in the same directory as the mecon binary. The example below shows two directory definitions. One for storing movies and one for storing television shows:

|                                appsettings.json                                |       Environment Variables            |
|----------------------------------------------------------------------------|----------------------------------------|
| "FileSystem": {<br>    "DirectoryDefinitions": {}<br>        "Films": {<br>            "DirectoryPath": "\\Video\\Films",<br>            "SupportedFileExtensions": [ "mkv", "ts", "mp4" ],<br>            "MediaTypes": [ "Movie" ],<br>            "DirectoryFilterRegexPattern": "(?i)^(?!.*sample).*$"<br>        },<br>        "TV": {<br>            "DirectoryPath": "\\Video\\TV",<br>            "SupportedFileExtensions": [ "mkv", "ts", "mp4" ],<br>            "MediaTypes": [ "TvShow" ]<br>        }<br>    }<br>} | Mecon__FileSystem__DirectoryDefinitions__Films__DirectoryPath=\Video\Films<br>Mecon__FileSystem__DirectoryDefinitions__Films__SupportedFileExtensions__1=mkv<br>Mecon__FileSystem__DirectoryDefinitions__Films__SupportedFileExtensions__2=ts<br>Mecon__FileSystem__DirectoryDefinitions__Films__SupportedFileExtensions__3=mp4<br>Mecon__FileSystem__DirectoryDefinitions__Films__MediaTypes__1=Movie<br>Mecon__FileSystem__DirectoryDefinitions__Films__DirectoryFilterRegexPattern=(?!^(?!.*sample).*\$)<br>Mecon__FileSystem__DirectoryDefinitions__TV__DirectoryPath=\Video\TV<br>Mecon__FileSystem__DirectoryDefinitions__TV__SupportedFileExtensions__1=mkv<br>Mecon__FileSystem__DirectoryDefinitions__TV__SupportedFileExtensions__2=ts<br>Mecon__FileSystem__DirectoryDefinitions__TV__SupportedFileExtensions__3=mp4<br>Mecon__FileSystem__DirectoryDefinitions__TV__MediaTypes__1=TvShow<br>|
### Logging
Logging by default is implemented using a single-line simple console logger with a log level of `Warning`. This can be reconfigured in many ways. However, this configuration is not in the scope of this documentation; instead, refer to [Microsoft's documentation for Console logging and its various options](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.console?view=dotnet-plat-ext-6.0). 
### Listing Configuration
Since configuration can be performed by leaving defaults as they are, adding environment variables, editing an appsettings.json file or any combination of these layered together, it can be useful to view a list of all of these combinations resolved using the [order of precedence](#configuration) as described at the beginning of this section. This can be performed using mecon's `config` verb and its `-l|--list` option:
```
mecon config -l
```
This will display all configuration in a JSON format regardless of whether it came from default settings, environment variables or the appsettings.json file.

## Limitations
mecon uses each file's name and size in bytes to reconcile files in the file system with items in Plex libraries. In the unlikely event that you have files that are considered different but have identical names and sizes, the reconciliation process will provide unreliable results.

## Versioning & Features Slated for v1.0.0
This application should be considered to be in beta until it reaches a v1.0.0+ version number. The version number can be confirmed using:
```console
mecon --version
```
Features slated for v1.0.0:
-   Progress feedback/spinner
-   Regex file filter (e.g. for ignoring all filenames containing 'sample')
-   File size output filter (e.g. for ignoring all files under 0.5MB)
-   User watched output filter (e.g. show only files watched by a list of users)
-   Packages, installers or manual install instructions
