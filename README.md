# EFI-Runner
[![Debug build](https://github.com/Segilmez06/EFI-Runner/actions/workflows/debug-build.yml/badge.svg)](https://github.com/Segilmez06/EFI-Runner/actions/workflows/debug-build.yml)
[![Release build](https://github.com/Segilmez06/EFI-Runner/actions/workflows/release-build.yml/badge.svg)](https://github.com/Segilmez06/EFI-Runner/actions/workflows/release-build.yml)

EFI-Runner is a tool for running EFI files with Qemu Virtual Machine in fastest way. It's CLI app but can be used with Windows Explorer as click to run. Also supports file association.

## Requirements
This tool requires Qemu preinstalled. It extracts the OVMF BIOS image and Shell EFI app itself. For Powershell version, it probably needs Powershell to be installed.

## Installation
No installation required. Just extract the zip to anywhere you want and execute it.

## Usage
```
Usage: 
    efi-runner [file]
    efi-runner [option]

Options:
    --help, -h          Show this help message
    --version, -v       Show version information
    --default, -d       Use default file (Shell.efi)
```
Examples:
- Directly booting into Shell:
  `efi-runner` or `efi-runner -d`

- Running your own file:
  `efi-runner .\bootx64.efi`
  
## Building
This tool is built on .Net 7 so it requires .Net version >= 7 while building. You can just open the solution file with your favorite IDE and build it.

## Contributing
You can create pull requests and issues to help development.

