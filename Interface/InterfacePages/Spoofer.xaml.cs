using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Windows.Threading;
using System.IO;
using Path = System.IO.Path;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Management;


namespace NezurAimbot.Interface.InterfacePages
{
    /// <summary>
    /// Interaction logic for Spoofer.xaml
    /// </summary>

    public partial class Spoofer : Page
    {
        private DispatcherTimer timer;
        private Random random;

        private ThemeManager themeManager;

        public Spoofer()
        {
            InitializeComponent();

            themeManager = ((App)System.Windows.Application.Current).ThemeManager;
            DataContext = themeManager;

            themeManager.ApplyTheme(this);

            Color themeColor = (Color)ColorConverter.ConvertFromString(GlobalSettings.Theme);

            TriggerBotOffset2.Color = themeColor;
            TriggerBotOffset1.Color = themeColor;
            TriggerBotOffset3.Color = themeColor;
            TriggerBotOffset4.Color = themeColor;
            TriggerBotOffset5.Color = themeColor;
            TriggerBotOffset6.Color = themeColor;
        }

        private void Button1Spoofer_Click(object sender, RoutedEventArgs e)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", true))
            {
                if (key != null)
                {
                    string newInstallationID = Guid.NewGuid().ToString();
                    key.SetValue("InstallationID", newInstallationID);
                    key.Close();
                    MessageBox.Show("Spoofed");
                }
            }
        }

        static void KillProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }
       
        static void DisableNetworkAdapter(int index)
        {
            string command = $"wmic path win32_networkadapter where index={index} call disable";
            RunCommand("cmd", $"/c {command}");
        }

        static void EnableNetworkAdapter(int index)
        {
            string command = $"wmic path win32_networkadapter where index={index} call enable";
            RunCommand("cmd", $"/c {command}");
        }
        static void RunCommand(string command, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(command);
            startInfo.Arguments = arguments;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();
        }
     
        private void Button2Spoofer_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Spoofed");
          
        }
        public static string RandomId(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string result = "";
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result += chars[random.Next(chars.Length)];
            }

            return result;
        }


        public static string RandomMac()
        {
            string chars = "ABCDEF0123456789";
            string windows = "26AE";
            string result = "";
            Random random = new Random();

            result += chars[random.Next(chars.Length)];
            result += windows[random.Next(windows.Length)];

            for (int i = 0; i < 5; i++)
            {
                result += "-";
                result += chars[random.Next(chars.Length)];
                result += chars[random.Next(chars.Length)];

            }

            return result;
        }
        public static void Enable_LocalAreaConection(string adapterId, bool enable = true)
        {
            string interfaceName = "Ethernet";
            foreach (NetworkInterface i in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (i.Id == adapterId)
                {
                    interfaceName = i.Name;
                    break;
                }
            }

            string control;
            if (enable)
                control = "enable";
            else
                control = "disable";

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("netsh", $"interface set interface \"{interfaceName}\" {control}");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();
        }

        private void Button2Spoofer1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Spoofed");
            using RegistryKey ScsiPorts = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\Scsi");
            foreach (string port in ScsiPorts.GetSubKeyNames())
            {
                using RegistryKey ScsiBuses = Registry.LocalMachine.OpenSubKey($"HARDWARE\\DEVICEMAP\\Scsi\\{port}");
                foreach (string bus in ScsiBuses.GetSubKeyNames())
                {
                    using RegistryKey ScsuiBus = Registry.LocalMachine.OpenSubKey($"HARDWARE\\DEVICEMAP\\Scsi\\{port}\\{bus}\\Target Id 0\\Logical Unit Id 0", true);
                    if (ScsuiBus != null)
                    {
                        if (ScsuiBus.GetValue("DeviceType").ToString() == "DiskPeripheral")
                        {
                            string identifier = RandomId(14);
                            string serialNumber = RandomId(14);

                            ScsuiBus.SetValue("DeviceIdentifierPage", Encoding.UTF8.GetBytes(serialNumber));
                            ScsuiBus.SetValue("Identifier", identifier);
                            ScsuiBus.SetValue("InquiryData", Encoding.UTF8.GetBytes(identifier));
                            ScsuiBus.SetValue("SerialNumber", serialNumber);
                        }
                    }
                }
            }
        }

        private void Button2Spoofer1_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Spoofed");
            using RegistryKey HardwareGUID = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\IDConfigDB\\Hardware Profiles\\0001", true);
            HardwareGUID.SetValue("HwProfileGuid", $"{{{Guid.NewGuid()}}}");

            using RegistryKey MachineGUID = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography", true);
            MachineGUID.SetValue("MachineGuid", Guid.NewGuid().ToString());

            using RegistryKey MachineId = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\SQMClient", true);
            MachineId.SetValue("MachineId", $"{{{Guid.NewGuid()}}}");

            using RegistryKey SystemInfo = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", true);

            Random rnd = new Random();
            int day = rnd.Next(1, 31);
            string dayStr = "";
            if (day < 10) dayStr = $"0{day}";
            else dayStr = day.ToString();

            int month = rnd.Next(1, 13);
            string monthStr = "";
            if (month < 10) monthStr = $"0{month}";
            else monthStr = month.ToString();

            SystemInfo.SetValue("BIOSReleaseDate", $"{dayStr}/{monthStr}/{rnd.Next(2000, 2023)}");
            SystemInfo.SetValue("BIOSVersion", RandomId(10));
            SystemInfo.SetValue("ComputerHardwareId", $"{{{Guid.NewGuid()}}}");
            SystemInfo.SetValue("ComputerHardwareIds", $"{{{Guid.NewGuid()}}}\n{{{Guid.NewGuid()}}}\n{{{Guid.NewGuid()}}}\n");
            SystemInfo.SetValue("SystemManufacturer", RandomId(15));
            SystemInfo.SetValue("SystemProductName", RandomId(6));

            using RegistryKey Update = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate", true);
            Update.SetValue("SusClientId", Guid.NewGuid().ToString());
            Update.SetValue("SusClientIdValidation", Encoding.UTF8.GetBytes(RandomId(25)));
        }

        private void Button2Spoofer3_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cleared");
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string ubisoftPath = Path.Combine("Ubisoft Game Launcher", "cache");
            string ubisoftLogsPath = Path.Combine("Ubisoft Game Launcher", "logs");
            string ubisoftSavegamesPath = Path.Combine("Ubisoft Game Launcher", "savegames");
            string ubisoftSpoolPath = Path.Combine("Ubisoft Game Launcher", "spool");

            DirectoryInfo di = new DirectoryInfo(Path.Combine("C:", "Program Files (x86)", "Ubisoft", ubisoftPath));
            DirectoryInfo di2 = new DirectoryInfo(Path.Combine("C:", "Program Files (x86)", "Ubisoft", ubisoftLogsPath));
            DirectoryInfo di3 = new DirectoryInfo(Path.Combine("C:", "Program Files (x86)", "Ubisoft", ubisoftSavegamesPath));
            DirectoryInfo di4 = new DirectoryInfo(Path.Combine(appDataPath, "Ubisoft Game Launcher", ubisoftSpoolPath));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            foreach (FileInfo file in di2.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di2.GetDirectories())
            {
                dir.Delete(true);
            }
            foreach (FileInfo file in di3.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di3.GetDirectories())
            {
                dir.Delete(true);
            }
            foreach (FileInfo file in di4.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di4.GetDirectories())
            {
                dir.Delete(true);
            }
        }


        private void Button1Spoofer1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open a cmd and type C:\\Windows\\System32\\Sysprep\\sysprep.exe ");

        }

       private void Button2Spoofer4_Click(object sender, RoutedEventArgs e)
{
    try
    {

                string command = @"powershell -ExecutionPolicy Bypass -Command ""Checkpoint-Computer -Description 'Nezur' -RestorePointType 'MODIFY_SETTINGS'""";


                ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe")
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process process = new Process
                {
                    StartInfo = processStartInfo
                };
                process.Start();

                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine("exit"); // Exit the command prompt

                process.WaitForExit();

                MessageBox.Show("Restore point created successfully!");
    }
    catch (Exception ex)
    {
     
        MessageBox.Show("Error creating restore point: " + ex.Message);
    }
}

        private void Button2Spoofer6_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Spoofed");
            string keyName = @"SYSTEM\CurrentControlSet\Enum\PCI\VEN_10DE&DEV_0DE1&SUBSYS_37621462&REV_A1";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, true))
            {
                if (key != null)
                {
                    string newHardwareID = "PCIVEN_8086&DEV_1234&SUBSYS_5678ABCD&REV_01";
                    string oldHardwareID = key.GetValue("HardwareID") as string;

                    key.SetValue("HardwareID", newHardwareID);
                    key.SetValue("CompatibleIDs", new string[] { newHardwareID });
                    key.SetValue("Driver", "pci.sys");
                    key.SetValue("ConfigFlags", 0x00000000, RegistryValueKind.DWord);
                    key.SetValue("ClassGUID", "{4d36e968-e325-11ce-bfc1-08002be10318}");
                    key.SetValue("Class", "Display");

                    key.Close();
                }
            }
        }

        private void Button2Spoofer5_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Spoofed");
            try
            {
                RegistryKey efiVariables = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Nsi\\{eb004a03-9b1a-11d4-9123-0050047759bc}\\26", true);
                if (efiVariables != null)
                {
                    string efiVariableId = Guid.NewGuid().ToString();
                    efiVariables.SetValue("VariableId", efiVariableId);
                    efiVariables.Close();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("\n[X] Start the spoofer in admin mode to spoof your MAC address!");
            }
        }

        private void Button2Spoofer7_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Spoofed");
            try
            {
                RegistryKey smbiosData = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\BIOS", true);

                if (smbiosData != null)
                {
                    string serialNumber = RandomId(10);
                    smbiosData.SetValue("SystemSerialNumber", serialNumber);
                    smbiosData.Close();
                }
                else
                {
                    Console.WriteLine("\n[X] Cant find the SMBIOS");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("\n[X] Start the spoofer in admin mode to spoof your MAC address!");
            }
        }

        private void Button1Spoofer2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Spoofed");
            // Create a DispatcherTimer to update the window title every 0.01 seconds
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += UpdateWindowTitle;

            // Start the timer
            timer.Start();
        }

        private void UpdateWindowTitle(object sender, object e)
        {
            // Generate a random string of 8 characters
            string randomString = Path.GetRandomFileName().Replace(".", "").Substring(0, 10);

            // Get all open windows
            foreach (Window window in Application.Current.Windows)
            {
                // Check if the window is of type Window (or your specific window type)
                if (window is Window)
                {
                    // Set the window title to the random string
                    window.Title = randomString;


                }
            }
        }

        private void Button2Spoofer8_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cleared");
            // Specify the command to be executed
            string command = "cd /d %LocalAppData% && rmdir /s /q Roblox";

            // Create a new process start info
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            // Start the process
            Process process = new Process
            {
                StartInfo = processStartInfo
            };
            process.Start();

            // Execute the command
            process.StandardInput.WriteLine(command);
            process.StandardInput.WriteLine("exit"); // Exit the command prompt

            // Wait for the process to finish
            process.WaitForExit();
        }

        private void Button2Spoofer10_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the current process path
                string currentProcessPath = Process.GetCurrentProcess().MainModule.FileName;

                // Create a copy of the executable in the current directory
                string copyPath = Path.Combine(Path.GetDirectoryName(currentProcessPath), "Roblox.exe");
                File.Copy(currentProcessPath, copyPath, true);

                // Modify the copy (for demonstration purposes, this example just appends a timestamp)
                File.AppendAllText(copyPath, $"\nModified at: {DateTime.Now}");

                // Generate a new hash for the modified process
                string newHash = CalculateFileHash(copyPath);

                // Display the original and new hash
                MessageBox.Show($"Original Hash: {CalculateFileHash(currentProcessPath)}\nNew Hash: {newHash}", "Hash Information");

                // Launch the modified process
                Process.Start(copyPath);

                // You can perform other actions here, such as saving the new hash
                // Keep in mind the ethical and legal implications of such actions
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private string CalculateFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                }
            }
        }

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);

        private void Button2Spoofer9_Click(object sender, RoutedEventArgs e)
        {
            // Display 3 message boxes confirming the user wants to proceed.
            var result1 = MessageBox.Show("Are you sure you want to proceed? This action will bluescreen your computer.", "Confirmation", MessageBoxButton.YesNo);
            if (result1 != MessageBoxResult.Yes)
            {
                return;
            }

            var result2 = MessageBox.Show("This action will bluescreen your computer. Are you absolutely sure?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result2 != MessageBoxResult.Yes)
            {
                return;
            }

            var result3 = MessageBox.Show("Proceeding will bluescreen your computer. Are you certain?", "Final Warning", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result3 != MessageBoxResult.Yes)
            {
                return;
            }

            // fucking kill their pc with a bsod
            System.Diagnostics.Process.EnterDebugMode();
            RtlSetProcessIsCritical(1, 0, 0);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void Button2Spoofer11_Click(object sender, RoutedEventArgs e)
        {
            // Specify the commands to uninstall and install Roblox
            string uninstallCommand = "wmic product where name='Roblox Player' call uninstall";
            string installCommand = "YOUR_INSTALL_COMMAND_HERE"; // Replace with the actual install command

            try
            {
                // Create a process start info for uninstalling Roblox
                ProcessStartInfo uninstallProcessStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Start the uninstall process
                Process uninstallProcess = new Process
                {
                    StartInfo = uninstallProcessStartInfo
                };
                uninstallProcess.Start();

                // Write the uninstall command to the command prompt
                uninstallProcess.StandardInput.WriteLine(uninstallCommand);
                uninstallProcess.WaitForExit();

                // Create a process start info for installing Roblox
                ProcessStartInfo installProcessStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Start the install process
                Process installProcess = new Process
                {
                    StartInfo = installProcessStartInfo
                };
                installProcess.Start();

                // Write the install command to the command prompt
                installProcess.StandardInput.WriteLine(installCommand);
                installProcess.WaitForExit();

                // Optionally, display a message indicating success
                MessageBox.Show("Roblox reinstallation completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the process
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button1Spoofer3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();

                startInfo.FileName = "ipconfig";
                startInfo.Arguments = "/flushdns";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                MessageBox.Show("DNS Cache has been cleared.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button2Spoofer12_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();

                startInfo.FileName = "wevtutil";
                startInfo.Arguments = "el";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                MessageBox.Show("Windows Logs have been cleared. ");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button2Spoofer14_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tempFolderPath = Path.GetTempPath();
                DirectoryInfo tempDirectory = new DirectoryInfo(tempFolderPath);

                DateTime thresholdDate = DateTime.Now.AddDays(-7);

                foreach (FileInfo file in tempDirectory.GetFiles())
                {
                    if (file.LastWriteTime < thresholdDate)
                    {
                        file.Delete();
                    }
                }

                foreach (DirectoryInfo subDirectory in tempDirectory.GetDirectories())
                {
                    subDirectory.Delete(true);
                }

                MessageBox.Show("Temporary cache cleared.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button2Spoofer13_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tempFolderPath = Path.GetTempPath();
                DirectoryInfo tempDirectory = new DirectoryInfo(tempFolderPath);

                foreach (FileInfo file in tempDirectory.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo subDirectory in tempDirectory.GetDirectories())
                {
                    subDirectory.Delete(true);
                }

                MessageBox.Show("Windows Temp folder cleared.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button2Spoofer15_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();

                startInfo.FileName = "netsh";
                startInfo.Arguments = "int ip reset";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                MessageBox.Show("TCP/IP reset successful.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button1Spoofer4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string chromeCookiesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google\\Chrome\\User Data\\Default\\Cookies");

                if (File.Exists(chromeCookiesPath))
                {
                    File.Delete(chromeCookiesPath);
                    MessageBox.Show("Chrome cookies cleared.");
                }
                else
                {
                    MessageBox.Show("Chrome cookies not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button2Spoofer16_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string firefoxCookiesPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Mozilla\\Firefox\\Profiles");

                string[] profileFolders = Directory.GetDirectories(firefoxCookiesPath);

                if (profileFolders.Length > 0)
                {
                    string selectedProfileFolder = profileFolders[0];

                    string cookiesFilePath = Path.Combine(selectedProfileFolder, "cookies.sqlite");

                    if (File.Exists(cookiesFilePath))
                    {
                        File.Delete(cookiesFilePath);
                        MessageBox.Show("Firefox cookies cleared.");
                    }
                    else
                    {
                        MessageBox.Show("Firefox cookies not found.");
                    }
                }
                else
                {
                    MessageBox.Show("No Firefox profiles found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button2Spoofer18_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunCommand("reg", "add \"HKLM\\System\\CurrentControlSet\\Services\\BFE\" /v \"Start\" /t REG_DWORD /d \"2\" /f");
                RunCommand("reg", "add \"HKLM\\System\\CurrentControlSet\\Services\\Dnscache\" /v \"Start\" /t REG_DWORD /d \"2\" /f");
                RunCommand("reg", "add \"HKLM\\System\\CurrentControlSet\\Services\\MpsSvc\" /v \"Start\" /t REG_DWORD /d \"2\" /f");
                RunCommand("reg", "add \"HKLM\\System\\CurrentControlSet\\Services\\WinHttpAutoProxySvc\" /v \"Start\" /t REG_DWORD /d \"3\" /f");
                RunCommand("sc", "config Dhcp start= auto");
                RunCommand("sc", "config DPS start= auto");
                RunCommand("sc", "config lmhosts start= auto");
                RunCommand("sc", "config NlaSvc start= auto");
                RunCommand("sc", "config nsi start= auto");
                RunCommand("sc", "config RmSvc start= auto");
                RunCommand("sc", "config Wcmsvc start= auto");
                RunCommand("sc", "config WdiServiceHost start= demand");
                RunCommand("sc", "config Winmgmt start= auto");
                RunCommand("sc", "config NcbService start= demand");
                RunCommand("sc", "config Netman start= demand");
                RunCommand("sc", "config netprofm start= demand");
                RunCommand("sc", "config WlanSvc start= auto");
                RunCommand("sc", "config WwanSvc start= demand");
                RunCommand("net", "start Dhcp");
                RunCommand("net", "start DPS");
                RunCommand("net", "start NlaSvc");
                RunCommand("net", "start nsi");
                RunCommand("net", "start RmSvc");
                RunCommand("net", "start Wcmsvc");

                DisableNetworkAdapter(0);
                DisableNetworkAdapter(1);
                DisableNetworkAdapter(2);
                DisableNetworkAdapter(3);
                DisableNetworkAdapter(4);
                DisableNetworkAdapter(5);

                Thread.Sleep(6000);

                EnableNetworkAdapter(0);
                EnableNetworkAdapter(1);
                EnableNetworkAdapter(2);
                EnableNetworkAdapter(3);
                EnableNetworkAdapter(4);
                EnableNetworkAdapter(5);

                RunCommand("arp", "-d *");
                RunCommand("route", "-f");
                RunCommand("nbtstat", "-R");
                RunCommand("nbtstat", "-RR");
                RunCommand("netsh", "advfirewall reset");
                RunCommand("netcfg", "-d");
                RunCommand("netsh", "winsock reset");
                RunCommand("netsh", "int 6to4 reset all");
                RunCommand("netsh", "int httpstunnel reset all");
                RunCommand("netsh", "int ip reset");
                RunCommand("netsh", "int isatap reset all");
                RunCommand("netsh", "int portproxy reset all");
                RunCommand("netsh", "int tcp reset all");
                RunCommand("netsh", "int teredo reset all");
                RunCommand("ipconfig", "/release");
                RunCommand("ipconfig", "/flushdns");
                RunCommand("ipconfig", "/flushdns");
                RunCommand("ipconfig", "/flushdns");
                RunCommand("ipconfig", "/renew");
            }
            catch
            { }
        }

        private void Button2Spoofer17_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KillProcess("epicgameslauncher.exe");
                KillProcess("FortniteClient-Win64-Shipping_EAC.exe");
                KillProcess("FortniteClient-Win64-Shipping.exe");
                KillProcess("FortniteClient-Win64-Shipping_BE.exe");
                KillProcess("FortniteLauncher.exe");
                KillProcess("EpicGamesLauncher.exe");
                KillProcess("FortniteClient-Win64-Shipping.exe");
                KillProcess("EpicGamesLauncher.exe");
                KillProcess("EasyAntiCheat_Setup.exe");
                KillProcess("FortniteLauncher.exe");
                KillProcess("EpicWebHelper.exe");
                KillProcess("FortniteClient-Win64-Shipping.exe");
                KillProcess("EasyAntiCheat.exe");
                KillProcess("BEService_x64.exe");
                KillProcess("EpicGamesLauncher.exe");
                KillProcess("FortniteClient-Win64-Shipping_BE.exe");
                KillProcess("FortniteClient-Win64-Shipping_EAC.exe");
                KillProcess("EasyAntiCheat.exe");
                KillProcess("dnf.exe");
                KillProcess("DNF.exe");
                KillProcess("BattleEye.exe");
                KillProcess("BEService.exe");
                KillProcess("BEServices.exe");
            

                MessageBox.Show("Fortnite Anti Cheat successfully cleaned.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button2Spoofer19_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KillProcess("FiveM.exe");
                KillProcess("FiveM_b2802_DumpServer.exe");
                KillProcess("FiveM_b2802_GTAProcess.exe");
                KillProcess("FiveM_ChromeBrowser.exe");
                KillProcess("FiveM_ROSLauncher.exe");
                KillProcess("FiveM_FiveM_ROSService.exe");
                KillProcess("Steam.exe");
                KillProcess("steam.exe");
                KillProcess("EpicGamesLauncher.exe");
                KillProcess("EpicGamesLauncher.exe");
                KillProcess("EpicWebHelper.exe");
                KillProcess("EpicGamesLauncher.exe");
                KillProcess("smartscreen.exe");
                KillProcess("dnf.exe");
                KillProcess("DNF.exe");
                KillProcess("CrossProxy.exe");
                KillProcess("tensafe_1.exe");
                KillProcess("TenSafe_1.exe");
                KillProcess("tensafe_2.exe");
                KillProcess("tencentdl.exe");
                KillProcess("TenioDL.exe");
                KillProcess("uishell.exe");
                KillProcess("BackgroundDownloader.exe");
                KillProcess("conime.exe");
                KillProcess("QQDL.EXE");
                KillProcess("qqlogin.exe");
                KillProcess("dnfchina.exe");
                KillProcess("dnfchinatest.exe");
                KillProcess("dnf.exe");
                KillProcess("txplatform.exe");
                KillProcess("TXPlatform.exe");
                KillProcess("Launcher.exe");
                KillProcess("LauncherPatcher.exe");
                KillProcess("SocialClubHelper.exe");
                KillProcess("RockstarErrorHandler.exe");
                KillProcess("RockstarService.exe");


                MessageBox.Show("FiveM Anti Cheat successfully cleaned.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button1Spoofer5_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KillProcess("EasyAntiCheat_Setup.exe");
                KillProcess("EasyAntiCheat.exe");
                KillProcess("BEService_x64.exe");
                KillProcess("smartscreen.exe");
                KillProcess("EasyAntiCheat.exe");
                KillProcess("dnf.exe");
                KillProcess("DNF.exe");
                KillProcess("CrossProxy.exe");
                KillProcess("tensafe_1.exe");
                KillProcess("TenSafe_1.exe");
                KillProcess("tensafe_2.exe");
                KillProcess("tencentdl.exe");
                KillProcess("TenioDL.exe");
                KillProcess("uishell.exe");
                KillProcess("BackgroundDownloader.exe");
                KillProcess("conime.exe");
                KillProcess("QQDL.EXE");
                KillProcess("qqlogin.exe");
                KillProcess("dnfchina.exe");
                KillProcess("dnfchinatest.exe");
                KillProcess("dnf.exe");
                KillProcess("txplatform.exe");
                KillProcess("TXPlatform.exe");
                KillProcess("Launcher.exe");
                KillProcess("LauncherPatcher.exe");
                KillProcess("OriginWebHelperService.exe");
                KillProcess("Origin.exe");
                KillProcess("OriginClientService.exe");
                KillProcess("OriginER.exe");
                KillProcess("OriginThinSetupInternal.exe");
                KillProcess("OriginLegacyCLI.exe");
                KillProcess("Agent.exe");
                KillProcess("Client.exe");
                KillProcess("BattleEye.exe");
                KillProcess("BEService.exe");
                KillProcess("BEServices.exe");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                MessageBox.Show("Valorant ANTI Cheat successfully terminated.");
            }
        }
        private void DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    directoriesDeleted++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting directory '{path}': {ex.Message}");
            }
        }
        private void DeleteRegistryKey(string keyPath)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath, true))
            {
                if (key != null)
                {
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        key.DeleteSubKeyTree(subKeyName);
                    }
                    Registry.LocalMachine.DeleteSubKey(keyPath, false);
                }
            }
        }

        // Currently updating...
        private int directoriesDeleted = 0;
        private void Button2Spoofer20_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "documents\\Call of Duty Modern Warfare"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Blizzard Entertainment"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Battle.net"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Battle.net"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Blizzard Entertainment"));

                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents\\Call of Duty Black Ops Cold War\\report"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents\\Call of Duty Black Ops Cold War"));

                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache\\data_0.dcache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache\\data_1.dcache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache\\data_2.dcache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache\\data_3.dcache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache\\f_000001.dcache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache\\index.dcache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\Cache\\index"));

                //thats boring to code. hust.

                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache\\data_0"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache\\data_1"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache\\data_2"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache\\data_3"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache\\f_000001"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\GPUCache\\index"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\Cache\\index.dcache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\Cache\\data_0.dcache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\Cache\\data_1.dcache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\Cache\\data_2.dcache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\Cache\\data_3.dcache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\Cache\\data_0"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\Cache\\data_1"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\Cache\\data_2"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\BrowserCache\\Cache\\data_3"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\Cache"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\Logs"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\WidevineCdm"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Battle.net\\CachedData"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Blizzard Entertainment"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Roaming\\Battle.net"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Battle.net"));
                DeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Blizzard Entertainment"));

                DeleteRegistryKey("SOFTWARE\\WOW6432Node\\EasyAntiCheat");
                DeleteRegistryKey("SYSTEM\\ControlSet001\\Services\\EasyAntiCheat");
                DeleteRegistryKey("SYSTEM\\ControlSet001\\Services\\EasyAntiCheat\\Security");
                DeleteRegistryKey("SOFTWARE\\WOW6432Node\\EasyAntiCheat");
                DeleteRegistryKey("SYSTEM\\ControlSet001\\Services\\EasyAntiCheat");
                DeleteRegistryKey("SOFTWARE\\WOW6432Node\\EasyAntiCheat");

                MessageBox.Show("All Anti Cheat traces have been successfully cleaned!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cleaning Anti-Cheat traces: " + ex.Message);
            }
        }
          private static void ModifyHosts(string domain, string ipAddress)
        {
            string hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", "etc", "hosts");
            string lineToAdd = $"{ipAddress} {domain}";

            File.AppendAllText(hostsPath, lineToAdd + Environment.NewLine);
        }


        private static void ShellTankstelle(string command)
        {
            var processInfo = new ProcessStartInfo("powershell.exe")
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = $"-ExecutionPolicy Bypass -NoProfile -Command '{command}'"
            };
        }

        private static void StopAndDeleteService(string serviceName)
        {
            RunCommand("sc", $"stop {serviceName}");
            RunCommand("sc", $"delete {serviceName}");
        }
        private static void ScheduledTask(string taskName)
            {
                RunCommand("schtasks", $"/Change /TN \"{taskName}\" /disable");
            }

          

            private void Button2Spoofer22_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShellTankstelle("Get-AppxPackage -AllUsers xbox | Remove-AppxPackage");
                StopAndDeleteService("XblAuthManager");
                StopAndDeleteService("XblGameSave");
                StopAndDeleteService("XboxNetApiSvc");
                StopAndDeleteService("XboxGipSvc");
                DeleteRegistryKey(@"HKLM\SYSTEM\CurrentControlSet\Services\xbgm");
                ScheduledTask("Microsoft\\XblGameSave\\XblGameSaveTask");
                ScheduledTask("Microsoft\\XblGameSave\\XblGameSaveTaskLogon");
                ModifyHosts("xboxlive.com", "127.0.0.1");
                ModifyHosts("user.auth.xboxlive.com", "127.0.0.1");
                ModifyHosts("presence-heartbeat.xboxlive.com", "127.0.0.1");

                MessageBox.Show("Xbox successfully terminated.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void RenameDir(string IODJwadsioamvdosas, string dwaNIOdsmadiowaios)
        {
            try
            {
                if (Directory.Exists(IODJwadsioamvdosas))
                {
                    string DIWAJNEWNDOWAS = Path.Combine(Path.GetDirectoryName(IODJwadsioamvdosas), dwaNIOdsmadiowaios);
                    Directory.Move(IODJwadsioamvdosas, DIWAJNEWNDOWAS);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while unlinking discord: " + ex.Message);
            }
        }

        private void Button2Spoofer23_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string IODJwadsioamvdosas = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord", "app-1.0.9015", "modules", "discord_rpc-1");
                string dwaNIOdsmadiowaios = "nezur.io";
                RenameDir(IODJwadsioamvdosas, dwaNIOdsmadiowaios);
                MessageBox.Show("Discord successfully terminated.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

  
