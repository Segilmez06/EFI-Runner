using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace EFI_Runner
{
    internal class Program
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        
        // Save last foreground and background colors
        static ConsoleColor f = Console.ForegroundColor;
        static ConsoleColor b = Console.BackgroundColor;

        static void Main(string[] args)
        {
            string envPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Zyex_Software", "EFI-Runner");
            string exePath = Path.Combine(envPath, "EFI-Runner.exe");


            #if DEBUG
            #else
            // If first run then install app
            if (!Path.Exists(exePath))
            {
                SetLoading(true);
                Console.Title = "Installing - EFI-Runner";
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("First run detected. Installing EFI-Runner");
                Console.WriteLine();

                
                WriteStatus(1, 4, "Prepearing");
                string SourceFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!Directory.Exists(envPath))
                {
                    Directory.CreateDirectory(envPath);
                }
                Console.WriteLine();

                
                WriteStatus(2, 4, "Copying base files");
                foreach (string file in Directory.GetFiles(SourceFolder))
                {
                    File.Copy(file, Path.Combine(envPath, Path.GetFileName(file)));
                }
                Console.WriteLine();

                
                WriteStatus(3, 4, "Copying external tools");
                Console.ForegroundColor = ConsoleColor.Yellow;
                ExtractResources(envPath);
                Console.WriteLine();

                
                WriteStatus(4, 4, "Registering extensions");
                if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                {
                    FileAssociation(".efi", "EFI Executable", "EFI_File", Path.Combine(Assembly.GetEntryAssembly().Location));
                }
                
                
                SetLoading(false);
                Exit();
            }
            #endif


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
";
            string version_msg = $"EFI-Runner {Assembly.GetEntryAssembly().GetName().Version}";

            
            // VM settings
            int memory = 1; // In GB scale
            string vmPath = Path.Combine(envPath, "qemu-system-x86_64.exe");
            string biosPath = Path.Combine(envPath, "OVMF.fd");
            string kernelPath = "";

            
            // Check for args
            if (args.Length > 0)
            {
                string arg = args[0];
                if (File.Exists(arg))
                {
                    Console.Title = $"{arg} - EFI-Runner";
                    kernelPath = Path.GetFullPath(arg);
                }
                else
                {
                    if (help_identifier.Contains(arg))
                    {
                        Console.WriteLine(version_msg);
                        Console.WriteLine(help_msg);
                        Exit();
                    }
                    else if (version_identifier.Contains(arg))
                    {
                        Console.WriteLine(version_msg);
                        Exit();
                    }
                    else
                    {
                        
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("File not found!");
                        Exit(false);
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No file specified.");
                Exit(false);
            }


            // Check file before run
            if (!File.Exists(vmPath))
            {
                ExtractResources(envPath);
                while (!File.Exists(vmPath));
            }
            try
            {
                Process.Start(vmPath).Kill();
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to start Qemu!");
                Exit(false);
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


            // Exit
            Exit();
        }

        private static void ExtractResources(string envPath)
        {
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
        }
    
        private static void FileAssociation(string FileExtension, string FriendlyDescription, string Key, string ExecutablePath)
        {
            // Create registry key for file type
            RegistryKey keyFileType = Registry.ClassesRoot.CreateSubKey(Key);
            keyFileType.SetValue("", FriendlyDescription);
            keyFileType.CreateSubKey("DefaultIcon").SetValue("", '"' + ExecutablePath + "\",0");

            // Create command key for file type
            RegistryKey keyShell = keyFileType.CreateSubKey("shell");
            keyShell.CreateSubKey("open").CreateSubKey("command").SetValue("", "\"" + ExecutablePath + "\"" + " \"%1\"");

            // Apply for local user
            RegistryKey keyLocal = Registry.CurrentUser.CreateSubKey(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + FileExtension);
            keyLocal.DeleteSubKey("UserChoice", false);
            keyLocal.CreateSubKey("UserChoice");
            keyLocal = keyLocal.OpenSubKey("UserChoice", RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);
            keyLocal.SetValue("Progid", Key, RegistryValueKind.String);

            // Close keys
            keyFileType.Close();
            keyShell.Close();
            keyLocal.Close();

            // Refresh explorer
            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }

        private static void Exit(bool Success = true)
        {
            Console.ForegroundColor = f;
            Console.BackgroundColor = b;
            if (Success)
            {
                Environment.Exit(0);
            }
            else
            {
                Environment.Exit(1);
            }
        }

        private static void WriteStatus(int Value, int MaxValue, string Message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"[{Value}/{MaxValue}] ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Message);
        }
    
        private static void SetLoading(bool State)
        {
            if (State)
            {
                Console.Write($"{(char)27}]9;4;3{(char)27}\\");
            }
            else
            {
                Console.Write($"{(char)27}]9;4;0{(char)27}\\");
            }
        }
    }
}