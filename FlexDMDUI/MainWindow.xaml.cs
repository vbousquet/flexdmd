using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace FlexDMDUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string flexDMDclsid = "{766E10D3-DFE3-4E1B-AC99-C4D2BE16E91F}";
        const string ultraDMDclsid = "{E1612654-304A-4E07-A236-EB64D6D4F511}";
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

        public void UpdateInstallPane()
        {
            installLocationLabel.Content = "Install location: " + _installPath;
            if (File.Exists(System.IO.Path.Combine(_installPath, @"FlexDMD.dll")))
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = "FlexDMD.dll was found in the provided location. Everything is fine.";
            }
            else
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = "FlexDMD.dll was not found. Check that you have entered the right folder.";
            }
            if (File.Exists(System.IO.Path.Combine(_installPath, @"dmddevice.dll")))
            {
                dmdDeviceInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                dmdDeviceInstallLabel.Content = "dmddevice.dll was found alongside flexdmd.dll. Everything is fine.";
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
            else if (ultraDMDinstall != null)
            {
                ultraDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                ultraDMDInstallLabel.Content = "FlexDMD is not registered to be used instead of UltraDMD (UltraDMD seems to be registered though).";
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
                        Registry.GetValue(@"HKEY_CLASSES_ROOT\CLSID\" + clsid + @"\InprocServer32", "CodeBase", null);
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

        public void OnRun(object sender, RoutedEventArgs e)
        {
            // FIXME run the script with wscript.exe, prepending it with the object creation, running once per output
            // https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/wscript
            StringBuilder script = new StringBuilder();
            if (renderFlexDMDBtn.IsChecked == true)
            {
                script.Append("Dim DMD\nSet DMD = CreateObject(\"FlexDMD.DMDObject\")\nDMD.InitForGame \"debug\"\n");
                script.Append(scriptTextBox.Text);
                script.Append("WScript.Sleep 5000");
                string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".vbs";
                System.IO.File.WriteAllText(fileName, script.ToString());
                var launcher = new WSHLauncher(fileName, "");
                launcher.Launch();
            }
            if (renderUltraDMDBtn.IsChecked == true)
            {
                script.Append("Dim DMD\nSet DMD = CreateObject(\"UltraDMD.DMDObject\")\nDMD.Init\n");
                script.Append(scriptTextBox.Text);
                script.Append("WScript.Sleep 5000");
                string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".vbs";
                System.IO.File.WriteAllText(fileName, script.ToString());
                var launcher = new WSHLauncher(fileName, "");
                launcher.Launch();
            }
        }

        class WSHLauncher
        {
            // file path for "GUI" version of Windows Script host
            private const string wScript = @"%SystemRoot%\System32\wscript.exe";
            // file path for console version of Windows Script host
            private const string cScript = @"%SystemRoot%\System32\cscript.exe";
            // B: Batch mode; suppresses command-line display of user prompts and script errors. Default is Interactive mode.
            // T Enables time-out: the maximum number of seconds the script can run. The default is no limit. 
            // The //T parameter prevents excessive execution of scripts by setting a timer. When execution time exceeds the specified value, CScript interrupts the script engine using the IActiveScript::InterruptThread method and terminates the process.
            private const string standardArgs = @"//Nologo";
            private const string cScriptArgs = @"//B";
            private const string timeoutFmt = @" //T:{0}";
            private ProcessStartInfo psi_ = null;
            private Process _process = null;
            private bool _waitForExit = true;

            // Constructor for WScript with script name and arguments
            public WSHLauncher(string scriptName, string scriptArgs)
            {
                CreatePsi(scriptName, scriptArgs, false, true, 0);
            }

            // Constructor for CScript with script name, script arguments and non-blocking option
            public WSHLauncher(string scriptName, string scriptArgs, bool waitForExit)
            {
                CreatePsi(scriptName, scriptArgs, true, waitForExit, 0);
            }

            // Constructor for CScript with script name, script arguments, non-blocking option, and timeout
            public WSHLauncher(string scriptName, string scriptArgs, bool waitForExit, uint maxSecs)
            {
                CreatePsi(scriptName, scriptArgs, true, waitForExit, maxSecs);
            }

            public int ExitCode
            {
                get
                {
                    if (!HasExited) return Int32.MinValue;
                    return _process.ExitCode;
                }
            }

            public bool HasExited
            {
                get
                {
                    return (null != _process && _process.HasExited) ? true : false;
                }
            }

            public void Launch()
            {
                _process = Process.Start(psi_);
                if (_waitForExit) _process.WaitForExit();
            }

            private void CreatePsi(string scriptName, string scriptArgs, bool console, bool waitForExit, uint maxSecs)
            {
                StringBuilder args = new StringBuilder();
                args.Append(standardArgs); // arguments for WSH
                if (console)
                    args.Append(" " + cScriptArgs); // arguments for cscript.exe
                if (0U < maxSecs)
                    args.Append(string.Format(timeoutFmt, maxSecs));
                args.Append(" " + scriptName);
                if (!scriptArgs.Equals(string.Empty))
                    args.Append(" " + scriptArgs);
                string fullPath =
                Environment.ExpandEnvironmentVariables(console ? cScript : wScript);
                psi_ = new ProcessStartInfo(fullPath, args.ToString())
                {
                    CreateNoWindow = true,
                    UseShellExecute = false, // propagate environment variables
                    ErrorDialog = (console ? false : true)
                };
                _waitForExit = waitForExit;
            }
        }
    }
}
