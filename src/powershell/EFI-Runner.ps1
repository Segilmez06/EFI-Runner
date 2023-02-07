$help_identifier = "--help", "-h", "/?", "/h", "/help", "-?"
$version_identifier = "--version", "-v", "/v", "/version", "-version", "--ver", "-ver"
$default_identifier = "--default", "-d", "/d", "/default", "-default"

$help_msg = @"
Usage: 
    efi-runner [file]
    efi-runner [option]

Options:
    --help, -h          Show this help message
    --version, -v       Show version information
    --default, -d       Use default file (Shell.efi)
"@
$version_msg = "efi-runner 1.0.0"

$command = "qemu-system-x86_64.exe"
$default_file = ".\Shell.efi"

try {
    Get-Command $command -ErrorAction Stop | Out-Null
}
catch {
    Write-Host "Qemu not found!"
    exit
}

$arg = $args[0]
if ($arg -ne $null) {
    if (Test-Path $arg) {
        $file = (Get-Item $arg).FullName
    }
    else {
        if ($help_identifier.Contains($arg)) {
            Write-Host $help_msg
            exit
        }
        elseif ($version_identifier.Contains($arg)) {
            Write-Host $version_msg
            exit
        }
        elseif ($default_identifier.Contains($arg)) {
            Write-Host "Using Shell file."
            $file = $default_file
        }
        else {
            Write-Host "File not found!"
            exit
        }
    }
}
else {
    Write-Host "No file specified, using default file."
    $file = $default_file
}

& $command -m 1G -net none -serial stdio -bios .\OVMF.fd -kernel $file