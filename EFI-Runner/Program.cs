using EFI_Runner.Properties;
using System.Diagnostics;
using System.Reflection;
using System.IO.Compression;

namespace EFI_Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] help_identifier = new string[] { "--help", "-h", "/?", "/h", "/help", "-?" };
            string[] version_identifier = new string[] { "--version", "-v", "/v", "/version", "-version", "--ver", "-ver" };
            string[] default_identifier = new string[] { "--default", "-d", "/d", "/default", "-default" };

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
            
            string command = "qemu-system-x86_64.exe";
            int memory = 1; // In GB scale
            bool blockNetBoot = true;
            bool stdOut = true;
            string bios_file = "OVMF.fd";
            string default_file = "Shell.efi";
            string file = "";

            string ExecuteLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string vmPath = Path.Combine(ExecuteLocation, command);

            if (args.Length > 0)
            {
                string arg = args[0];
                if (File.Exists(arg))
                {
                    file = Path.GetFullPath(arg);
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
                    else if (default_identifier.Contains(arg))
                    {
                        Console.WriteLine("Using Shell file.");
                        file = default_file;
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
                Console.WriteLine("No file specified, using default file.");
                file = default_file;
            }

            foreach (string f in Assembly.GetExecutingAssembly().GetManifestResourceNames().Skip(1))
            {
                string filename = string.Join('.', f.Split('.').Skip(2).ToArray());
                if (!File.Exists(filename))
                {
                    FileStream fileStream = File.Create(Path.Combine(ExecuteLocation, filename));
                    Console.WriteLine($"Resource {filename} not found! Copying...");
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(f).CopyTo(fileStream);
                    fileStream.Close();
                }
            }

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

            string argString = "";
            argString += $"-m {memory}G ";
            if (blockNetBoot)
            {
                argString += "-net none ";
            }
            if (stdOut)
            {
                argString += "-serial stdio ";
            }
            argString += $"-bios {Path.Combine(ExecuteLocation, bios_file)} ";
            argString += $"-kernel {Path.Combine(ExecuteLocation, file)}";

            var psi = new ProcessStartInfo()
            {
                FileName = vmPath,
                Arguments = argString
            };

            var p = new Process()
            {
                StartInfo = psi
            };
            p.Start();
            p.WaitForExit();
        }
    }
}