using System.Diagnostics;

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
            string version_msg = "efi-runner 1.0.0";
            string command = "qemu-system-x86_64.exe";
            string default_file = ".\\Shell.efi";

            try
            {
                Process.Start(command);
            }
            catch (Exception)
            {
                Console.WriteLine("Qemu not found!");
                Environment.Exit(0);
            }

            string arg = args[0];
            if (arg != null)
            {
                if (File.Exists(arg))
                {
                    string file = Path.GetFullPath(arg);
                }
                else
                {
                    if (help_identifier.Contains(arg))
                    {
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
                        string file = default_file;
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
                string file = default_file;
            }

            Process.Start(command, "-m 1G -net none -serial stdio -bios .\\OVMF.fd -kernel " + file);
        }
    }
}