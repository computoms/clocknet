# Command-line time tracker written in dotnet core

This is a small command-line utility to track your time using the command line.

## How to install

Download the executable from the build artifacts. Then, alias the executable `clocknet` with your favorite shell.

### Mac OS

On macos, if you use zsh, you can add the following line to your [profile](https://www.gnu.org/software/bash/manual/html_node/Bash-Startup-Files.html), _i.e._ `~/.zshenv` file (if you downloaded the package into the downloads folder):

```zsh
alias clock="~/Downloads/clocknet/clocknet"
```

Remark: on macos, it is possible that the OS prevents you from running the software. In this case, you can use the `dotnet` tool to run it with the command: `dotnet ~/Downloads/clocknet/clocknet.dll`

### Windows

On windows, if you use Powershell, you can add the following line to your [profile](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_profiles?view=powershell-7.2):

```pwsh
function Invoke-Clock { "C:/path/to/the/package/clocknet.exe" }
New-Alias -Name clock -Value Invoke-Clock
```

# Usage

This simple utility uses a text file to store tasks with date/time information. Each time you start working on a task, a new line is created on the file with the current time and a description of the task you are starting to work on.

At the end of the day, or anytime, you can then generate reports and statistics based on the file.

## File structure

The file structure is very simple and can be edited using the script or directly with your favorite text editor.
Here is an example file:

```
[2022-01-01]
10:00 Starting project X +projectX
11:23 Starting documentation of project X +projectX +doc
12:00 [Stop]
[2022-01-02]
08:05 Starting workday, checking emails +office +emails
09:00 Back on documentation +projectX +doc
10:00 [Stop]
```

## Tags and ids

An entry in this file can be associated with tags if you start the tag with a `+` (`+tag`) or ID if you start with a `.` (`.456`). 

Tags allow for powerful filtering and reporting. They are ordered, meaning that `+project +doc` is different from `+doc +project` (see reports and filters below).

IDs allow to track time of tasks from an external tool, such as Jira. Entries with an ID are automatically assigned a default tag (`+jira`).

## Special tasks

The `[Stop]` task is used to stop the last task. It is not required if you switch tasks without taking a break.

## Settings

Some settings can be configured, in `~/.clock/settings.yml`:

```yml
File: /Users/thomas/clock.txt # Path to the file we use to store our tasks 
DefaultTask: Admin +internal # Default task using when using command add without any other parameters
```

## Adding new entries

You can add a new entry by using the `add` command:

```
$ clock add Definition of the prototype +myapp +proto
```

To switch to a new task, just use the same command:

```
$ clock add Switching to new task
```

This will automatically stop the last task and start a new one. When you have finished working, use the `stop` command:

```
$ clock stop
```

If you forgot to add a task, you can add it later using the `--at` option:

```
$ clock add --at 10:00 Forgot to add a task
```

### Restarting last entry

After you took a break by using the `clock stop` command, you can restart the latest task using the `restart` command.

### Restarting a task by id

You can also only specify the id of a task that has already been tracked before, and this will add the corresponding title/tags automatically:

```
$ clock add This is a new task with an id +tag .123
$ clock stop
$ clock add This is a second task
$ clock add .123 # Will automatically add the entry 'This is a new task with an id +tag .123'
```

## Reports

You can show reports/statistics with the `show` command:

```
$ clock show
```

#### Details

By default, the details of today are shown.

To show the details for yesterday, use the `--yesterday` (or `-y`) switch. 

To show the details of all tasks in the file, use the `--all` (or `-a`) switch.

#### Time worked

You can also show the time worked per day for the entire week using the `--week` option (or `-w`) or the time worked per week for the entire period using the `--all` in combination with the `--worktimes` switches (equivalent to `-aw`).