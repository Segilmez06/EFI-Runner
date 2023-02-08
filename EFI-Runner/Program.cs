using System.Diagnostics;
using System.Reflection;

namespace EFI_Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Command line arguments
            string[] help_identifier = new string[] { "--help", "-h", "/?", "/h", "/help", "-?" };
            string[] version_identifier = new string[] { "--version", "-v", "/v", "/version", "-version", "--ver", "-ver" };

            string help_msg = @"
Usage: 
    efi-runner [file]
    efi-runner [option]

Options:
    --help, -h          Show this help message
    --version, -v       Show version information
    --default, -d       Use default file (Shell.efi)
";
            string version_msg = $"EFI-Runner {Assembly.GetEntryAssembly().GetName().Version}";

            
            // VM settings
            int memory = 1; // In GB scale
            string envPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Zyex_Software", "EFI-Runner");
            string vmPath = Path.Combine(envPath, "qemu-system-x86_64.exe");
            string biosPath = Path.Combine(envPath, "OVMF.fd");
            string kernelPath = "";

            
            // Check for args
            if (args.Length > 0)
            {
                string arg = args[0];
                if (File.Exists(arg))
                {
                    kernelPath = Path.GetFullPath(arg);
                }
                else
                {
                    if (help_identifier.Contains(arg))
                    {
                        Console.WriteLine(version_msg);
                        Console.WriteLine(help_msg);
                        Environment.Exit(0);
                    }
                    else if (version_identifier.Contains(arg))
                    {
                        Console.WriteLine(version_msg);
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("File not found!");
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                Console.WriteLine("No file specified.");
                Environment.Exit(0);
            }

            
            // Extract files
            if (Directory.Exists(envPath))
            {
                Directory.CreateDirectory(envPath);
            }
            foreach (string f in Assembly.GetExecutingAssembly().GetManifestResourceNames().Skip(1))
            {
                string filepath = Path.Combine(envPath, string.Join('.', f.Split('.').Skip(2).ToArray()));
                if (!File.Exists(filepath))
                {
                    Console.Write($"Copying {Path.GetFileName(filepath)}... ");
                    FileStream fileStream = File.Create(filepath);
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(f).CopyTo(fileStream);
                    fileStream.Close();
                    Console.WriteLine("OK");
                }
            }

            // Check file before run
            while (!File.Exists(vmPath)) ;
            try
            {
                Process.Start(vmPath).Kill();
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to start Qemu!");
                Environment.Exit(0);
            }

            
            // Set process arguments
            string argString = "";
            argString += $"-m {memory}G ";
            argString += $"-serial stdio ";
            argString += $"-no-reboot -no-shutdown ";
            argString += $"-net none ";
            argString += $"-bios {biosPath} ";
            argString += $"-kernel {kernelPath} ";
            var psi = new ProcessStartInfo()
            {
                FileName = vmPath,
                Arguments = argString,
                WorkingDirectory = envPath
            };
            var p = new Process()
            {
                StartInfo = psi
            };

            
            // Run
            p.Start();
            p.WaitForExit();
        }
    }
}