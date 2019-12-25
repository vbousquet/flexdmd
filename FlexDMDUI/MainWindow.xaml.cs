using FlexDMD;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace FlexDMDUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// FIXME
    /// For the time begin, registration will fail depending on the security setup of the computer (even when run with admin rights).
    /// On computer where it fails, regasm from x64 framework will fail too, whereas regasm from x86 framework will succeeded.
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string flexDMDclsid = "{766E10D3-DFE3-4E1B-AC99-C4D2BE16E91F}";
        private const string ultraDMDclsid = "{E1612654-304A-4E07-A236-EB64D6D4F511}";
        private readonly string _flexScriptFileName = Path.GetTempPath() + "dmd_flex.vbs";
        private readonly string _ultraScriptFileName = Path.GetTempPath() + "dmd_ultra.vbs";
        private Process _flexDMDProcess = null;
        private Process _ultraDMDProcess = null;
        private string _installPath;

        public MainWindow()
        {
            InitializeComponent();
            var flexPath = GetComponentLocation(flexDMDclsid);
            if (flexPath != null)
            {
                _installPath = Directory.GetParent(flexPath).FullName;
            }
            else
            {
                _installPath = Path.GetFullPath("./");
            }
            UpdateInstallPane();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnStopScript(sender, null);
        }

        public void UpdateInstallPane()
        {
            installLocationLabel.Content = "Install location: " + _installPath;
            if (File.Exists(System.IO.Path.Combine(_installPath, @"FlexDMD.dll")))
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = "FlexDMD.dll was found in the provided location.";
            }
            else
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = "FlexDMD.dll was not found. Check that you have entered the folder where you have placed this file.";
            }
            if (File.Exists(System.IO.Path.Combine(_installPath, @"dmddevice.dll")))
            {
                dmdDeviceInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                dmdDeviceInstallLabel.Content = "dmddevice.dll was found alongside flexdmd.dll.";
            }
            else
            {
                dmdDeviceInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                dmdDeviceInstallLabel.Content = "dmddevice.dll was not found alongside flexdmd.dll. No rendering will happen.";
            }
            var flexDMDinstall = GetComponentLocation(flexDMDclsid);
            var ultraDMDinstall = GetComponentLocation(ultraDMDclsid);
            if (flexDMDinstall != null)
            {
                flexDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                flexDMDInstallLabel.Content = "FlexDMD is registered and ready to run.";
                registerFlexDMDBtn.Content = "Unregister";
            }
            else
            {
                flexDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDInstallLabel.Content = "FlexDMD is not registered and may not be used on your system.";
                registerFlexDMDBtn.Content = "Register";
            }
            if (ultraDMDinstall != null && ultraDMDinstall.EndsWith("/FlexDMD.dll"))
            {
                ultraDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                ultraDMDInstallLabel.Content = "FlexDMD is registered to be used instead of UltraDMD.";
                registerUltraDMDBtn.Content = "Unregister";
            }
            else if (ultraDMDinstall != null && ultraDMDinstall.EndsWith("/UltraDMD.exe"))
            {
                ultraDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                ultraDMDInstallLabel.Content = "FlexDMD is not registered to be used instead of UltraDMD (UltraDMD is registered though).";
                registerUltraDMDBtn.Content = "Register";
            }
            else
            {
                ultraDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                ultraDMDInstallLabel.Content = "FlexDMD is not registered to be used instead of UltraDMD.";
                registerUltraDMDBtn.Content = "Register";
            }
        }

        private string GetComponentLocation(string clsid)
        {
            var path = Registry.GetValue(@"HKEY_CLASSES_ROOT\Wow6432Node\CLSID\" + clsid + @"\InprocServer32", "CodeBase", null) ??
                        Registry.GetValue(@"HKEY_CLASSES_ROOT\CLSID\" + clsid + @"\InprocServer32", "CodeBase", null) ??
                        Registry.GetValue(@"HKEY_CLASSES_ROOT\Wow6432Node\CLSID\" + clsid + @"\LocalServer32", null, null) ??
                        Registry.GetValue(@"HKEY_CLASSES_ROOT\CLSID\" + clsid + @"\LocalServer32", null, null);
            if (path != null) return new Uri(path.ToString()).AbsolutePath;
            return null;
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
                    // System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                }
            }
        }

        public void OnRegisterFlex(object sender, RoutedEventArgs e)
        {
            try
            {
                Assembly asm = Assembly.LoadFile(System.IO.Path.Combine(_installPath, "FlexDMD.dll"));
                if (GetComponentLocation(flexDMDclsid) != null)
                {
                    UnRegisterCOMObject("FlexDMD.DMDObject", flexDMDclsid);
                }
                else
                {
                    // Using RegisterAssembly would be ok if we would not use ILMerge but it will fail for the merged assembly as it contains all the NAudio COM objects.
                    RegisterCOMObject("FlexDMD.DMDObject", flexDMDclsid, asm);
                }
            }
            catch (Exception)
            {
            }
            UpdateInstallPane();
        }

        public void OnRegisterUltra(object sender, RoutedEventArgs e)
        {
            try
            {
                Assembly asm = Assembly.LoadFile(System.IO.Path.Combine(_installPath, "FlexDMD.dll"));
                var ultraDMDinstall = GetComponentLocation(ultraDMDclsid);
                if (ultraDMDinstall != null && ultraDMDinstall.EndsWith("/FlexDMD.dll"))
                {
                    // unregister UltraDMD (it will need to be reregistered either form UltraDMD or FlexDMD)
                    UnRegisterCOMObject("UltraDMD.DMDObject", ultraDMDclsid);
                }
                else
                {
                    // Register the delegate object to avoid conflicts
                    RegisterCOMObject("UltraDMD.DMDObject", ultraDMDclsid, asm);
                }
            }
            catch (Exception)
            {
            }
            UpdateInstallPane();
        }

        private void RegisterCOMObject(string className, string guid, Assembly asm)
        {
            // See answer #3 https://www.codeproject.com/questions/67756/register-a-com-dll-programatically-in-c-without-ad
            // and https://github.com/rubberduck-vba/Rubberduck/wiki/COM-Registration
            var codebase = new Uri(asm.CodeBase).AbsolutePath;
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(codebase);
            var version = string.Format("{0}.{1}.{2}.{3}", fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
            // HKEY_CLASSES_ROOT needs administrator privileges, while HKEY_CURRENT_USER\Software\Classes doesn't
            // HKEY_CLASSES_ROOT = HKEY_LOCAL_MACHINE\Software\Classes or 
            // string[] roots = { @"HKEY_CURRENT_USER\Software\Classes\", @"HKEY_CURRENT_USER\Software\Classes\Wow6432Node\"};
            string[] roots = { @"HKEY_CLASSES_ROOT\", @"HKEY_CLASSES_ROOT\Wow6432Node\" };
            foreach (string root in roots)
            {
                Registry.SetValue(root + className, null, className);
                Registry.SetValue(root + className + @"\CLSID", null, guid);
                Registry.SetValue(root + @"CLSID\" + guid, null, className);
                Registry.SetValue(root + @"CLSID\" + guid + @"\InprocServer32", null, @"mscoree.dll");
                Registry.SetValue(root + @"CLSID\" + guid + @"\InprocServer32", @"ThreadingModel", @"Both");
                Registry.SetValue(root + @"CLSID\" + guid + @"\InprocServer32", @"Class", className);
                Registry.SetValue(root + @"CLSID\" + guid + @"\InprocServer32", @"Assembly", asm.FullName);
                Registry.SetValue(root + @"CLSID\" + guid + @"\InprocServer32", @"RuntimeVersion", asm.ImageRuntimeVersion);
                Registry.SetValue(root + @"CLSID\" + guid + @"\InprocServer32", @"CodeBase", codebase);
                Registry.SetValue(root + @"CLSID\" + guid + @"\InprocServer32\" + version, @"Class", className);
                Registry.SetValue(root + @"CLSID\" + guid + @"\InprocServer32\" + version, @"Assembly", asm.FullName);
                Registry.SetValue(root + @"CLSID\" + guid + @"\InprocServer32\" + version, @"RuntimeVersion", asm.ImageRuntimeVersion);
                Registry.SetValue(root + @"CLSID\" + guid + @"\InprocServer32\" + version, @"CodeBase", codebase);
                Registry.SetValue(root + @"CLSID\" + guid + @"\Implemented Categories\{62C8FE65-4EBB-45E7-B440-6E39B2CDBF29}", null, @"");
                Registry.SetValue(root + @"CLSID\" + guid + @"\ProgId", null, className);
            }
        }

        private void UnRegisterCOMObject(string className, string guid)
        {
            Registry.ClassesRoot.DeleteSubKeyTree(className, false);
            Registry.ClassesRoot.OpenSubKey("CLSID", true).DeleteSubKeyTree(guid, false);
            Registry.ClassesRoot.OpenSubKey("Wow6432Node", true).DeleteSubKeyTree(className, false);
            Registry.ClassesRoot.OpenSubKey("Wow6432Node", true).OpenSubKey("CLSID", true).DeleteSubKeyTree(guid, false);
        }

        // TODO use ActiveScripting instead (and understand why UltraDMD won't run simultaneously with FlexDMD)
        public void OnRunScript(object sender, RoutedEventArgs e)
        {
            OnStopScript(sender, e);
            if (renderFlexDMDBtn.IsChecked == true)
            {
                var script = "Dim DMD\nSet DMD = CreateObject(\"FlexDMD.DMDObject\")\nDMD.Init\n" + scriptTextBox.Text + "\nWScript.Sleep 100000";
                System.IO.File.WriteAllText(_flexScriptFileName, script);
                var psi = new ProcessStartInfo(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\wscript.exe"), @"//Nologo " + _flexScriptFileName)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false, // propagate environment variables
                    ErrorDialog = true
                };
                _flexDMDProcess = Process.Start(psi);
            }
            if (renderUltraDMDBtn.IsChecked == true)
            {
                var script = "Dim DMD\nSet DMD = CreateObject(\"UltraDMD.DMDObject\")\nDMD.Init\n" + scriptTextBox.Text + "\nWScript.Sleep 100000";
                System.IO.File.WriteAllText(_ultraScriptFileName, script);
                var psi = new ProcessStartInfo(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\wscript.exe"), @"//Nologo " + _ultraScriptFileName)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    ErrorDialog = true
                };
                _ultraDMDProcess = Process.Start(psi);
            }
        }

        public void OnStopScript(object sender, RoutedEventArgs e)
        {
            if (_flexDMDProcess != null && !_flexDMDProcess.HasExited)
            {
                _flexDMDProcess.Kill();
                _flexDMDProcess = null;
            }
            if (File.Exists(_flexScriptFileName)) File.Delete(_flexScriptFileName);
            if (_ultraDMDProcess != null && !_ultraDMDProcess.HasExited)
            {
                _ultraDMDProcess.Kill();
                _ultraDMDProcess = null;
                // Window Class: WindowsForms10.Window.8.app.0.21af1a5_r9_ad1 Name: Virtual DMD
                var ultraDMDwnd = WindowHandle.FindWindow(wnd => wnd.GetWindowText() == "Virtual DMD");
                if (ultraDMDwnd != null) ultraDMDwnd.SendMessage(0x0010, 0, 0); // WM_CLOSE
                if (File.Exists(_ultraScriptFileName)) File.Delete(_ultraScriptFileName);
            }
        }

        private void scriptTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
