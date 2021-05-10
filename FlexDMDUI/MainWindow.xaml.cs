using FlexDMD;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Trinet.Core.IO.Ntfs;

namespace FlexDMDUI
{
    public static class Command
    {
        public static RoutedCommand RunCmd = new RoutedCommand();
        public static RoutedCommand StopCmd = new RoutedCommand();
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _installPath;
        private string _packPath;
        private ScriptThread _testScript;
        private readonly KnownColor[] _allColors;

        public const string flexDMDclsid = "{766E10D3-DFE3-4E1B-AC99-C4D2BE16E91F}";
        public const string ultraDMDclsid = "{E1612654-304A-4E07-A236-EB64D6D4F511}";

        public const int udmdConfigTab = 1;
        public const int udmdDesignTab = 2;
        public const int fdmdDesignTab = 3;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow.Title = AppWindow.Title + " " + getRunningVersion();
            _packPath = Directory.GetCurrentDirectory();
            packerFolderLabel.Content = "Pack folder: " + _packPath;
            Array colorsArray = Enum.GetValues(typeof(KnownColor));
            _allColors = new KnownColor[colorsArray.Length - 28 - 7]; // Create a list of user color, skipping system ones
            Array.Copy(colorsArray, 28, _allColors, 0, _allColors.Length);
            for (int i = 0; i < _allColors.Length; i++)
            {
                SolidColorBrush brush = new SolidColorBrush();
                var col = System.Drawing.Color.FromName(_allColors[i].ToString());
                brush.Color = System.Windows.Media.Color.FromArgb(col.A, col.R, col.G, col.B);
                var rect = new System.Windows.Shapes.Rectangle { Width = 10, Height = 10, Fill = brush };
                var margin = rect.Margin;
                margin.Right = 5;
                rect.Margin = margin;
                var block = new TextBlock();
                block.Inlines.Add(new Run(_allColors[i].ToString() + " [RGB: " + col.ToArgb().ToString("X8").Substring(2) + "]"));
                var panel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal };
                panel.Children.Add(rect);
                panel.Children.Add(block);
                ultraDMDColors.Items.Add(panel);
            }
            var color = Registry.CurrentUser.OpenSubKey("Software")?.OpenSubKey("UltraDMD")?.GetValue("color");
            if (color == null || !(color is string))
            {
                color = "OrangeRed";
                Registry.CurrentUser.CreateSubKey("Software")?.CreateSubKey("UltraDMD")?.SetValue("color", color);
            }
            if (color != null && color is string c)
            {
                for (int i = 0; i < _allColors.Length; i++)
                {
                    if (_allColors[i].ToString().ToLowerInvariant().Equals(c.ToLowerInvariant()))
                    {
                        ultraDMDColors.SelectedItem = ultraDMDColors.Items[i];
                    }
                }
            }
            ultraDMDColors.ScrollIntoView(ultraDMDColors.SelectedItem);
            var fullcolor = Registry.CurrentUser.OpenSubKey("Software")?.OpenSubKey("UltraDMD")?.GetValue("fullcolor");
            if (fullcolor != null && fullcolor is string fc)
            {
                if ("True".Equals(fc, StringComparison.InvariantCultureIgnoreCase))
                    ultraDMDFullColor.IsChecked = true;
                else
                    ultraDMDMonochrome.IsChecked = true;
            }
            else
            {
                ultraDMDFullColor.IsChecked = true;
                Registry.CurrentUser.CreateSubKey("Software")?.CreateSubKey("UltraDMD")?.SetValue("fullcolor", "True");
            }

            var flexPath = GetComponentLocation(flexDMDclsid);
            if (flexPath != null)
            {
                _installPath = Directory.GetParent(flexPath).FullName;
            }
            else
            {
                _installPath = Path.GetFullPath(".");
            }
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(OnUpdateTimer);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            UpdateInstallPane();
        }

        private void OnUpdateTimer(object sender, EventArgs e)
        {
            UpdateInstallPane();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseDMD();
            // Window Class: WindowsForms10.Window.8.app.0.21af1a5_r9_ad1 Name: Virtual DMD
            var ultraDMDwnd = WindowHandle.FindWindow(wnd => wnd.GetWindowText() == "Virtual DMD");
            ultraDMDwnd?.SendMessage(0x0010, 0, 0); // WM_CLOSE
            new Thread(new ThreadStart(CloseRunnable))
            {
                IsBackground = true
            }.Start();
        }

        // Runnable to force exit after 1 second if the script threads don't close gracefully
        private void CloseRunnable()
        {
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        public void UpdateInstallPane()
        {
            installLocationLabel.Content = "Install location: " + _installPath;
            var flexDmdPath = Path.Combine(_installPath, @"FlexDMD.dll");
            var flexUDmdPath = Path.Combine(_installPath, @"FlexUDMD.dll");
            var dmdDevicePath = Path.Combine(_installPath, @"dmddevice.dll");
            var dmdDevice64Path = Path.Combine(_installPath, @"dmddevice64.dll");
            var uiVersion = (Assembly.GetExecutingAssembly().GetName().Version.Major << 16) | (Assembly.GetExecutingAssembly().GetName().Version.Minor);

            // See https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed?redirectedfrom=MSDN
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                bool upToDate = ndpKey != null && ndpKey.GetValue("Release") != null && ((int)ndpKey.GetValue("Release") >= 528040);
                missingNetFrameworkInfo.Visibility = upToDate ? Visibility.Collapsed : Visibility.Visible;
            }

            // FlexDMD & FlexUDMD dlls (must exist at the given location, with matching versions, and up to date with regards to this installer)
            if (!File.Exists(flexDmdPath))
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = "FlexDMD.dll was not found. Check that you have entered the folder where you have placed this file.";
            }
            else if (!File.Exists(flexUDmdPath))
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = "FlexUDMD.dll was not found. Check that you have entered the folder where you have placed this file.";
            }
            else if (GetFileVersion(flexDmdPath) != GetFileVersion(flexUDmdPath))
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = string.Format("FlexDMD.dll and FlexUDMD.dll have different versions: {0} vs {1}. Please reinstall matching files.", GetFileVersion(flexDmdPath), GetFileVersion(flexUDmdPath));
            }
            else if (GetFileMainVersion(flexDmdPath) < uiVersion)
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = string.Format("FlexDMD.dll and FlexUDMD.dll are outdated version {0} instead of {1}. Please download updated files.", GetFileVersion(flexDmdPath), Assembly.GetExecutingAssembly().GetName().Version);
            }
            else
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = string.Format("FlexDMD.dll & FlexUDMD.dll {0} were found in the provided location.", GetFileVersion(flexDmdPath));
            }

            // DmdDevice & DmdDevice64 dlls (must exist at the given location, with matching 1.8+ version)
            if (!File.Exists(dmdDevicePath))
            {
                dmdDeviceInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                dmdDeviceInstallLabel.Content = "DmdDevice.dll was not found alongside FlexDMD.dll. No rendering will happen.";
            }
            else if (!File.Exists(dmdDevice64Path))
            {
                dmdDeviceInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                dmdDeviceInstallLabel.Content = "DmdDevice64.dll was not found alongside FlexDMD.dll. 64 bit support will be missing (PinballY integration,...).";
            }
            else if (GetFileVersion(dmdDevicePath) != GetFileVersion(dmdDevice64Path))
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = string.Format("DmdDevice.dll and DmdDevice64.dll have different versions: {0} vs {1}. Please reinstall matching files.", GetFileVersion(dmdDevicePath), GetFileVersion(dmdDevice64Path));
            }
            else if (GetFileMainVersion(dmdDevicePath) < 0x010008)
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = string.Format("DmdDevice.dll and DmdDevice64.dll are outdated (version {0}). Please download updated files.", GetFileVersion(dmdDevicePath));
            }
            else
            {
                dmdDeviceInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                dmdDeviceInstallLabel.Content = string.Format("DmdDevice.dll & DmdDevice64.dll {0} were found alongside FlexDMD.", GetFileVersion(dmdDevicePath));
            }

            // Install state: must have unblocked DLL, at the right install position, with a matching version between file and registry.
            var flexDMDinstall = GetComponentLocation(flexDMDclsid);
            if (flexDMDinstall == null)
            {
                flexDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDInstallLabel.Content = "FlexDMD is not registered and may not be used on your system. Click 'Register' to register it.";
            }
            else if (!flexDMDinstall.Equals(flexDmdPath, StringComparison.InvariantCultureIgnoreCase))
            {
                flexDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDInstallLabel.Content = "Registered FlexDMD does not match your install path. Click 'Register' to fix it.";
            }
            else if (!File.Exists(flexDmdPath))
            {
                flexDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDInstallLabel.Content = "There seems to be a problem with FlexDMD registration (missing file). Click 'Register' to unblock it.";
            }
            else if (IsDLLBlocked(flexDmdPath))
            {
                flexDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDInstallLabel.Content = "FlexDMD dll is blocked. Click 'Register' to unblock it.";
            }
            else if (File.Exists(dmdDevicePath) && IsDLLBlocked(dmdDevicePath))
            {
                flexDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDInstallLabel.Content = "DmdDevice.dll is blocked. Click 'Register' to unblock it.";
            }
            else if (File.Exists(dmdDevice64Path) && IsDLLBlocked(dmdDevice64Path))
            {
                flexDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDInstallLabel.Content = "DmdDevice64.dll is blocked. Click 'Register' to unblock it.";
            }
            else if (!CheckRegisteredVersion(flexDMDclsid, GetFileVersion(flexDmdPath)))
            {
                flexDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDInstallLabel.Content = "Invalid registered version of FlexDMD detected. Click 'Register' to fix it.";
            }
            else
            {
                flexDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                flexDMDInstallLabel.Content = "FlexDMD is registered and ready to run.";
            }

            // Either deprecated UltraDMD.exe is registered or FlexUDMD with an unblocked DLL and a macthing version between registry and file
            var ultraDMDinstall = GetComponentLocation(ultraDMDclsid);
            if (ultraDMDinstall != null && ultraDMDinstall.ToUpperInvariant().EndsWith("ULTRADMD.EXE"))
            {
                ultraDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                ultraDMDInstallLabel.Content = "FlexDMD is not registered to be used instead of UltraDMD (UltraDMD is registered though).";
            }
            else if (ultraDMDinstall == null || !ultraDMDinstall.Equals(flexUDmdPath, StringComparison.InvariantCultureIgnoreCase))
            {
                ultraDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                ultraDMDInstallLabel.Content = "FlexDMD is not registered to be used instead of UltraDMD.";
            }
            else if (!File.Exists(flexUDmdPath))
            {
                ultraDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                ultraDMDInstallLabel.Content = "There seems to be a problem with UltraDMD registration (missing file). Click 'Register' to unblock it.";
            }
            else if (IsDLLBlocked(flexUDmdPath))
            {
                ultraDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                ultraDMDInstallLabel.Content = "FlexUDMD dll is blocked. Click 'Register' to unblock it.";
            }
            else if (!CheckRegisteredVersion(ultraDMDclsid, GetFileVersion(flexUDmdPath)))
            {
                ultraDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                ultraDMDInstallLabel.Content = "Invalid registered version of FlexDMD's UltraDMD replacement detected. Click 'Register' to fix it.";
            }
            else
            {
                ultraDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                ultraDMDInstallLabel.Content = "FlexDMD is registered to be used as an UltraDMD replacement.";
            }
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private string GetComponentLocation(string clsid)
        {
            var path = Registry.GetValue(@"HKEY_CLASSES_ROOT\Wow6432Node\CLSID\" + clsid + @"\InprocServer32", "CodeBase", null) ??
                        Registry.GetValue(@"HKEY_CLASSES_ROOT\CLSID\" + clsid + @"\InprocServer32", "CodeBase", null) ??
                        Registry.GetValue(@"HKEY_CLASSES_ROOT\Wow6432Node\CLSID\" + clsid + @"\LocalServer32", null, null) ??
                        Registry.GetValue(@"HKEY_CLASSES_ROOT\CLSID\" + clsid + @"\LocalServer32", null, null);
            if (path != null) return new Uri(path.ToString()).LocalPath;
            return null;
        }

        private bool CheckRegisteredVersion(string clsid, string version)
        {
            //Assembly key has the following syntax:
            //FlexDMD, Version=1.5.0.0, Culture=neutral, PublicKeyToken=7144ae60aec212e0
            var asm32 = Registry.GetValue(@"HKEY_CLASSES_ROOT\CLSID\" + clsid + @"\InprocServer32", "Assembly", null);
            var asm64 = Registry.GetValue(@"HKEY_CLASSES_ROOT\Wow6432Node\CLSID\" + clsid + @"\InprocServer32", "Assembly", null);
            var asm32v = Registry.GetValue(@"HKEY_CLASSES_ROOT\CLSID\" + clsid + @"\InprocServer32\" + version, "Assembly", null);
            var asm64v = Registry.GetValue(@"HKEY_CLASSES_ROOT\Wow6432Node\CLSID\" + clsid + @"\InprocServer32\" + version, "Assembly", null);
            if (asm32 == null || asm64 == null || asm32v == null || asm64v == null) return false;
            var asm = asm32.ToString();
            if (!asm.Equals(asm64.ToString()) || !asm.Equals(asm32v.ToString()) || !asm.Equals(asm64v.ToString())) return false;
            if (!asm.Contains("Version=" + version)) return false;
            return true;
        }

        private bool IsDLLBlocked(string path)
        {
            try
            {
                FileInfo file = new FileInfo(path);
                if (file.AlternateDataStreamExists("Zone.Identifier"))
                {
                    AlternateDataStreamInfo s = file.GetAlternateDataStream("Zone.Identifier", FileMode.Open);
                    using (TextReader reader = s.OpenText())
                    {
                        var zoneId = reader.ReadToEnd().ToUpperInvariant();
                        return zoneId.Contains("ZONEID=3") || zoneId.Contains("ZONEID=4");
                    }
                }
            }
            catch (Exception)
            {
                // FIXME This is an horrible exception swallowing when DLL blocking check fails. This should be properly reported (at leats in a log file). Not done since this action is performed periodically
            }
            return false;
        }

        private Version getRunningVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        private string GetFileVersion(string path)
        {
            return FileVersionInfo.GetVersionInfo(path).FileVersion;
        }

        private int GetFileMainVersion(string path)
        {
            return (FileVersionInfo.GetVersionInfo(path).FileMajorPart << 16) | (FileVersionInfo.GetVersionInfo(path).FileMinorPart);
        }

        public void OnRegisterFlex(object sender, RoutedEventArgs e)
        {
            Register("/register \"" + _installPath + "\"");
            UpdateInstallPane();
        }

        public void OnUnregisterFlex(object sender, RoutedEventArgs e)
        {
            Register("/unregister \"" + _installPath + "\"");
            UpdateInstallPane();
        }

        public void OnRegisterUltra(object sender, RoutedEventArgs e)
        {
            Register("/register-udmd \"" + _installPath + "\"");
            UpdateInstallPane();
        }

        public void OnUnregisterUltra(object sender, RoutedEventArgs e)
        {
            Register("/unregister-udmd \"" + _installPath + "\"");
            UpdateInstallPane();
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private static Process Register(string command)
        {
            Process source = Process.GetCurrentProcess();
            // Create a new process
            Process target = new Process();
            target.StartInfo = source.StartInfo;
            target.StartInfo.FileName = source.MainModule.FileName;
            target.StartInfo.WorkingDirectory = Path.GetDirectoryName(source.MainModule.FileName);
            // Required for UAC to work
            target.StartInfo.UseShellExecute = true;
            target.StartInfo.Verb = "runas";
            // Pass command
            target.StartInfo.Arguments = command;
            try
            {
                if (!target.Start())
                    return null;
            }
            catch (Win32Exception e)
            {
                // Cancelled by user
                if (e.NativeErrorCode == 1223)
                    return null;
                throw;
            };
            return target;
        }

        private void OnTabChanged(object sender, SelectionChangedEventArgs e)
        {
            CloseDMD();

            if (tabPanel.SelectedIndex == udmdConfigTab)
            {
                _testScript = new ScriptThread("UltraDMD Script Thread");
                _testScript.Post(@"
                Dim FlexDMD
                Set FlexDMD = CreateObject(""FlexDMD.FlexDMD"")
                FlexDMD.GameName = ""Designer""
                Dim UltraDMD
                Set UltraDMD = FlexDMD.NewUltraDMD()
                UltraDMD.Init
                UltraDMD.LoadSetup
                UltraDMD.DisplayScene00 ""FlexDMD.Resources.colors.png"", ""FlexDMD"", 15, ""UltraDMD API mode"", 15, 14, -1, 14
                ");
            }

            if (tabPanel.SelectedIndex == udmdDesignTab)
            {
                _testScript = new ScriptThread("UltraDMD Script Thread");
                _testScript.Post(@"
                Dim FlexDMD
                Set FlexDMD = CreateObject(""FlexDMD.FlexDMD"")
                FlexDMD.GameName = ""Designer""
                Dim UltraDMD
                'Set UltraDMD = CreateObject(""UltraDMD.DMDObject"")
                Set UltraDMD = FlexDMD.NewUltraDMD()
                UltraDMD.Init
                ");
            }

            if (tabPanel.SelectedIndex == fdmdDesignTab)
            {
                _testScript = new ScriptThread("FlexDMD Script Thread");
                _testScript.Post(@"
                Dim FlexDMD
                Set FlexDMD = CreateObject(""FlexDMD.FlexDMD"")
                FlexDMD.GameName = ""Designer""
                FlexDMD.Run = True
                ");
            }
        }

        public void OnUltraDMDRenderModeChanged(object sender, RoutedEventArgs e)
        {
            if (ultraDMDFullColor.IsChecked == true)
                Registry.CurrentUser.CreateSubKey("Software")?.CreateSubKey("UltraDMD")?.SetValue("fullcolor", "True");
            else
                Registry.CurrentUser.CreateSubKey("Software")?.CreateSubKey("UltraDMD")?.SetValue("fullcolor", "False");
            UpdateUltraDMDConfig();
        }

        private void OnUltraDMDColorChanged(object sender, SelectionChangedEventArgs e)
        {
            Registry.CurrentUser.CreateSubKey("Software")?.CreateSubKey("UltraDMD")?.SetValue("color", _allColors[ultraDMDColors.SelectedIndex].ToString());
            UpdateUltraDMDConfig();
        }

        private void UpdateUltraDMDConfig()
        {
            if (_testScript != null)
            {
                _testScript.Post(@"
                    If Not UltraDMD is Nothing Then
                        UltraDMD.CancelRendering
                        UltraDMD.Uninit
                        UltraDMD.LoadSetup
                        UltraDMD.Init
                        UltraDMD.DisplayScene00 ""FlexDMD.Resources.colors.png"", ""FlexDMD"", 15, ""UltraDMD API mode"", 15, 14, -1, 14
                    End If
                    ");
            }
        }

        public void OnSelectInstallFolder(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the directory in which FlexDMD is installed.";
                fbd.SelectedPath = _installPath;
                DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    _installPath = fbd.SelectedPath;
                    UpdateInstallPane();
                }
            }
        }

        private void CloseDMD()
        {
            if (_testScript != null)
            {
                _testScript.Interrupt();
                _testScript.Post(@"
                    If Not FlexDMD is Nothing Then
                        FlexDMD.Show = False
                        FlexDMD.Run = False
                        FlexDMD = NULL
                    ElseIf Not UltraDMD is Nothing Then
                        UltraDMD.Uninit
                        UltraDMD = NULL
                    End If
                    ");
                _testScript.Close(true, true);
                _testScript = null;
            }
        }

        private void OnRunCmd(object sender, ExecutedRoutedEventArgs args)
        {
            if (tabPanel.SelectedIndex == udmdDesignTab) _testScript.Post(ultraDMDScriptTextBox.Text);
            if (tabPanel.SelectedIndex == fdmdDesignTab) _testScript.Post(flexDMDScriptTextBox.Text);
        }

        private void OnStopCmd(object sender, ExecutedRoutedEventArgs args)
        {
            if (_testScript != null) _testScript.Interrupt();
        }

        public void OnRunFlexDMDScript(object sender, RoutedEventArgs e)
        {
            if (_testScript != null) _testScript.Post(flexDMDScriptTextBox.Text);
        }

        public void OnRunUltraDMDScript(object sender, RoutedEventArgs e)
        {
            if (_testScript != null) _testScript.Post(ultraDMDScriptTextBox.Text);
        }

        public void OnStopDMDScript(object sender, RoutedEventArgs e)
        {
            if (_testScript != null) _testScript.Interrupt();
        }

        public void OnSelectFrameFolder(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the directory where the frames are located.";
                fbd.SelectedPath = _packPath;
                DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    _packPath = fbd.SelectedPath;
                    packerFolderLabel.Content = "Pack folder: " + _packPath;
                }
            }
        }

        public void OnPackFrames(object sender, RoutedEventArgs e)
        {
            string atlasName = "DMD_Atlas";
            Packer packer = new Packer();
            packer.Process(_packPath, 8192, 0, false);
            if (packer.Atlasses.Count == 1)
            {
                var atlas = packer.Atlasses[0];
                var img = packer.CreateAtlasImage(atlas);

                // Create a sorted list of all the packed frames
                var frames = new SortedList<string, Node>();
                foreach (Node n in atlas.Nodes)
                    if (n.Texture != null)
                        foreach (string source in n.Texture.Sources)
                            frames[source] = n;

                StringBuilder tw = new StringBuilder();
                tw.Append("' This code was automatically generated from FlexDMDUI. Don't modify.\n");
                tw.AppendFormat("' {0} frames packed into a {1}x{2} atlas.\n", frames.Count, img.Width, img.Height);
                tw.Append("Public Sub PreloadFrames(flex)\n");
                string sequence = null;
                foreach (KeyValuePair<string, Node> kvp in frames)
                {
                    var shortName = Path.GetFileNameWithoutExtension(kvp.Key);
                    if (Regex.IsMatch(shortName, "(.+)_(\\d+)"))
                    {
                        var seqName = Regex.Match(shortName, "(.+)_(\\d+)").Groups[1].Value;
                        if (sequence == null || !sequence.Equals(seqName))
                            tw.AppendFormat("\tNewFrames flex, \"\", \"{0}\"\n", seqName);
                        sequence = seqName;
                    }
                    else
                    {
                        tw.AppendFormat("\tNewFrames flex, \"\", \"{0}\"\n", shortName);
                        sequence = null;
                    }
                }
                sequence = null;
                tw.Append("End Sub\n\n");
                tw.Append("Public Function NewFrames(flex, name, id)\n");
                tw.Append("\tSelect Case id\n");
                StringBuilder lw = new StringBuilder();
                foreach (KeyValuePair<string, Node> kvp in frames)
                {
                    var n = kvp.Value;
                    var shortName = Path.GetFileNameWithoutExtension(kvp.Key);
                    if (Regex.IsMatch(shortName, "(.+)_(\\d+)"))
                    {
                        var seqName = Regex.Match(shortName, "(.+)_(\\d+)").Groups[1].Value;
                        if (sequence == null || !sequence.Equals(seqName))
                        {
                            if (sequence != null) // End of previous sequence
                            {
                                lw.Remove(lw.Length - 1, 1);
                                tw.Append(lw.ToString());
                                tw.Append("\")\n");
                            }
                            lw = new StringBuilder();
                            lw.AppendFormat("\t\tCase \"{0}\": Set NewFrames = flex.NewImageSequence(name, 30, \"", seqName);
                        }
                        if (lw.Length > 80)
                        {
                            tw.Append(lw.ToString());
                            tw.Append("\" & _\n");
                            tw.Append("\t\t\t\t\"");
                            lw.Clear();
                        }
                        lw.AppendFormat("VPX.{0}&region={1},{2},{3},{4}&pad={5},{6},{7},{8}|", atlasName, n.Bounds.X, n.Bounds.Y, n.Bounds.Width, n.Bounds.Height, 
                            n.Texture.PackedX, n.Texture.PackedY, n.Texture.OriginalWidth - n.Texture.PackedWidth - n.Texture.PackedX, n.Texture.OriginalHeight - n.Texture.PackedHeight - n.Texture.PackedY);
                        sequence = seqName;
                    }
                    else
                    {
                        if (sequence != null) // End of previous sequence
                        {
                            lw.Remove(lw.Length - 1, 1);
                            tw.Append(lw.ToString());
                            tw.Append("\")\n");
                            sequence = null;
                        }
                        tw.AppendFormat("\t\tCase \"{0}\": Set NewFrames = flex.NewImage(name, \"VPX.{1}&region={2},{3},{4},{5}&pad={6},{7},{8},{9}\")\n", shortName, atlasName, n.Bounds.X, n.Bounds.Y, n.Bounds.Width, n.Bounds.Height,
                            n.Texture.PackedX, n.Texture.PackedY, n.Texture.OriginalWidth - n.Texture.PackedWidth - n.Texture.PackedX, n.Texture.OriginalHeight - n.Texture.PackedHeight - n.Texture.PackedY);
                    }
                }
                if (sequence != null) // End of previous sequence
                {
                    lw.Remove(lw.Length - 1, 1);
                    tw.Append(lw.ToString());
                    tw.Append("\")\n");
                }
                tw.Append("\tEnd Select\n");
                tw.Append("End Function\n");
                tw.Append("' End of generated block\n");
                flexDMDPackerScript.Text = tw.ToString();
                img.Save(_packPath + "/../" + atlasName + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                flexDMDPackerScript.Text = "Packing returned more than one image. You must select less frames to pack.";
            }
        }
    }
}
