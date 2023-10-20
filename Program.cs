using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using VMware.Vim;

namespace VMwareInfoExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Contains("/?") || args.Contains("-?") || args.Contains("-help") || args.Contains("-h"))
            {
                DisplayHelp();
                return;
            }

            string server = null;
            string outputPath = null;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-server":
                        if (i + 1 < args.Length)
                        {
                            server = args[i + 1];
                            i++;
                        }
                        break;
                    case "-outputpath":
                        if (i + 1 < args.Length)
                        {
                            outputPath = args[i + 1];
                            i++;
                        }
                        break;
                }
            }

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(outputPath))
            {
                DisplayHelp();
                return;
            }

            Console.WriteLine($"Server: {server}");
            Console.WriteLine($"Output Path: {outputPath}");

            Console.WriteLine($"Enter username for {server}:");
            string username = Console.ReadLine();

            Console.WriteLine($"Enter password for {server}:");
            SecureString password = GetPassword();

            var client = new VimClientImpl();
            client.Connect($"https://{server}/sdk");
            client.Login(username, ToInsecureString(password));

            var vms = client.FindEntityViews(typeof(VirtualMachine), null, null, null);
            List<VmInfo> output = new List<VmInfo>();

            foreach (VirtualMachine vm in vms)
            {
                var guest = vm.Guest;
                var info = new VmInfo
                {
                    VMName = vm.Name,
                    IPAddress = guest.IpAddress,
                    GuestOS = guest.GuestFullName,
                    Notes = vm.Config.Annotation
                };
                output.Add(info);
            }

            WriteToCsv(outputPath, output);

            client.Disconnect();
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("VMwareInfoExporter - Exports VM info from vCenter or ESXi to a CSV file.");
            Console.WriteLine("Usage:");
            Console.WriteLine("VMwareInfoExporter.exe -server <server_address> -outputpath <path_to_csv>");
            Console.WriteLine("Switches:");
            Console.WriteLine("-server     : The address of the vCenter or ESXi host.");
            Console.WriteLine("-outputpath : The path to the CSV file where data should be saved.");
            Console.WriteLine("-? /? -help -h: Displays this help information.");
        }

        private static SecureString GetPassword()
        {
            SecureString password = new SecureString();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password.AppendChar(key.KeyChar);
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.RemoveAt(password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        private static string ToInsecureString(SecureString secureString)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return System.Runtime.InteropServices.Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        private static void WriteToCsv(string path, List<VmInfo> vms)
        {
            using (var sw = new StreamWriter(path))
            {
                sw.WriteLine("VM Name\tIP Address\tGuest OS\tNotes");
                foreach (var vm in vms)
                {
                    sw.WriteLine($"{vm.VMName}\t{vm.IPAddress}\t{vm.GuestOS}\t{vm.Notes}");
                }
            }
        }

        public class VmInfo
        {
            public string VMName { get; set; }
            public string IPAddress { get; set; }
            public string GuestOS { get; set; }
            public string Notes { get; set; }
        }
    }
}
