# FileSys-Checker
A tool which scans all current drives on a windows system and all sub directories for directories and files with weak permissions. 
Written in c#

## Description

This tool enumerates all drives (drive letters) and then enumerates all directories and fileson those drives. 
The configured permissions for each directory and file are checked for write permissions for the following:

* BUILTIN\\Users
* NT AUTHORITY\\Authenticated Users
* NT AUTHORITY\\INTERACTIVE
* EVERYONE

## Usage
If you are able to create executables then compile on your own system and move the executable across. No flags are required:

```
filesys-check.exe
```

## Compile
Should compile with .NET Framework and .NET Software Development Kit (SDK), no further SDKs should be needed. Download from here: You can download it from the official Microsoft website: https://dotnet.microsoft.com/download

Compile using CSC.EXE from the .NET Framework:

    32-bit systems: The default location is C:\Windows\Microsoft.NET\Framework\[version]\csc.exe, where [version] corresponds to the installed .NET Framework version (e.g., v4.0.30319).

    64-bit systems: The default location is C:\Windows\Microsoft.NET\Framework64\[version]\csc.exe.
    
i.e. compiling with csc.exe:

```
csc.exe /target:exe /out:filesys-check.exe c:\temp\filesys-check.cs
```
