# EFI-Runner
[![Debug build](https://github.com/Segilmez06/EFI-Runner/actions/workflows/debug-build.yml/badge.svg)](https://github.com/Segilmez06/EFI-Runner/actions/workflows/debug-build.yml)
[![Release build](https://github.com/Segilmez06/EFI-Runner/actions/workflows/release-build.yml/badge.svg)](https://github.com/Segilmez06/EFI-Runner/actions/workflows/release-build.yml)

EFI-Runner is a tool for running EFI files with Qemu Virtual Machine in fastest way. It's CLI app but can be used with Windows Explorer as click to run. Also supports file association.

## Requirements
This tool extracts 64-bit Qemu VM, OVMF BIOS image and Shell EFI app itself. But you must install .Net 7 Runtime.

For Powershell version, it probably needs Powershell and Qemu installed. Also you have to put OVMF BIOS image and Shell EFI app to the working directory. You can download them here: [EFI-Runner/powershell](https://github.com/Segilmez06/EFI-Runner/tree/main/powershell)

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
```
Examples:
- Directly booting into Shell:
  `efi-runner` or `efi-runner -d`

- Running your own file:
  `efi-runner .\bootx64.efi`
  
## Building
This tool is built on .Net 7 so it requires .Net SDK version >= 7 while building. [Also you can install Runtime for testing](#requirements). You can just open the solution file with your favorite IDE and build it.

## Contributing
You can create pull requests and issues to help development. Also starring the repo will give me motivation.
