using FlexDMD;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

namespace FlexDMDUI
{
    public static class Command
    {
        public static RoutedCommand RunCmd = new RoutedCommand();
        public static RoutedCommand StopCmd = new RoutedCommand();
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// TODO fix register/unregister
    /// For the time begin, registration will fail depending on the security setup of the computer (even when run with admin rights).
    /// On computer where it fails, regasm from x64 framework will fail too, whereas regasm from x86 framework will succeeded.
    /// </summary>
    public partial class MainWindow : Window //, IActiveScriptSite
    {
        private const bool useLocalMachine = true;
        private const string flexDMDclsid = "{766E10D3-DFE3-4E1B-AC99-C4D2BE16E91F}";
        private const string ultraDMDclsid = "{E1612654-304A-4E07-A236-EB64D6D4F511}";
        private string _installPath;
        private ScriptThread _flexScript, _ultraScript;

        public MainWindow()
        {
            InitializeComponent();
            scriptTextBox.Text =
@"' Demo script

Public Sub FlexDemo()
    DMD.SetProjectFolder(""D:\Games\Visual Pinball\Tables"")

    DMD.DmdHeight = 36
    DMD.RenderMode = 2
    DMD.TableFile = ""Miraculous.vpx""
    'DMD.SetTwoLineFonts ""FlexDMD.Resources.font-12.fnt"", ""FlexDMD.Resources.font-7.fnt""
    'DMD.SetSingleLineFonts Array(CStr(""FlexDMD.Resources.font-12.fnt""), CStr(""FlexDMD.Resources.font-7.fnt""), CStr(""FlexDMD.Resources.font-5.fnt""))
    
    Dim topLine(16)
    Dim botLine(20)
    For i = 0 to 15
        topLine(i) = CStr(""VPX.d0&dmd=2"")
    Next
    For i = 0 to 19
        botLine(i) = CStr(""VPX.d1&dmd=2"")
    Next
    DMD.DisplayJPSScene ""jps"", ""VPX.extraball&dmd=2"", topLine, botLine, 14, 10000, 14

    DMD.DisplayScene00 ""VPX.extraball&dmd=2"", ""FlexDMD"", 15, ""."", 15, 14, 5000, 14
    DMD.DisplayScene00 ""VPX.gameover&dmd=2"", ""FlexDMD"", 15, ""."", 15, 14, 5000, 14
    DMD.DisplayScene00 ""VPX.tilt&dmd=2"", ""FlexDMD"", 15, ""."", 15, 14, 5000, 14
End Sub

' Check that FlexDMD renders the same as UltraDMD
Public Sub CompareUltraFlex()
    ' Animations :
    '  FadeIn = 0, // Fade from black to scene
    '  FadeOut = 1, // Fade from scene to black
    '  ZoomIn = 2, // zoom from a centered small dmd to full size
    '  ZoomOut = 3, // zoom from a full sized dmd to an oversize one
    '  ScrollOffLeft = 4,
    '  ScrollOffRight = 5,
    '  ScrollOnLeft = 6,
    '  ScrollOnRight = 7,
    '  ScrollOffUp = 8,
    '  ScrollOffDown = 9,
    '  ScrollOnUp = 10,
    '  ScrollOnDown = 11,
    '  FillFadeIn = 12, // fade from black to white (the scene won't be seen)
    '  FillFadeOut = 13, // fade from white to black (the scene won't be seen)
    '  None = 14
    DMD.DisplayScene00 """", ""Fade In / Out"", 15, "".."", 15, 0, 1000, 1
    DMD.DisplayScene00 """", ""Scroll On/Off Right"", 15, ""..."", 15, 7, 1000, 5
    DMD.DisplayScene00 """", ""Scroll On/Off Left"", 15, ""..."", 15, 6, 1000, 4
    DMD.DisplayScene00 """", ""Scroll On/Off Down"", 15, ""..."", 15, 11, 1000, 9
    DMD.DisplayScene00 """", ""Scroll On/Off Up"", 15, ""..."", 15, 10, 1000, 8
    DMD.DisplayScene00 """", ""Fill Fade In / Out"", 15, "".."", 15, 12, 1000, 13

    ' Scrolling text scene
    DMD.DisplayScene01 """", """", ""Scrolling Text"", 15, -1, 14, 5000, 14

End Sub

If FlexDMDMode And UltraDMDMode Then
    CompareUltraFlex()
ElseIf FlexDMDMode Then
    FlexDemo()
Else
    DMD.DisplayScene00 """", ""UltraDMD"", 15, ""."", 15, 14, 1000, 14
End If
";
            var flexPath = GetComponentLocation(flexDMDclsid);
            if (flexPath != null)
            {
                _installPath = Directory.GetParent(flexPath).FullName;
            }
            else
            {
                _installPath = Path.GetFullPath("./");
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
            OnFlexDMDUnselected(null, null);
            OnUltraDMDUnselected(null, null);
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
            // HKEY_CLASSES_ROOT provides a merged view of HKEY_LOCAL_MACHINE and HKEY_CURRENT_USER (it should not be used for writing)
            // HKEY_LOCAL_MACHINE\Software\Classes if for all users and needs administrator privileges
            // HKEY_CURRENT_USER\Software\Classes if for all current user and do not need any privileges
            // see https://docs.microsoft.com/en-us/windows/win32/sysinfo/hkey-classes-root-key for merge informations
            var codebase = new Uri(asm.CodeBase).AbsolutePath;
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(codebase);
            var version = string.Format("{0}.{1}.{2}.{3}", fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
            string[] roots = useLocalMachine ? new string[] { @"HKEY_LOCAL_MACHINE\Software\Classes\", @"HKEY_LOCAL_MACHINE\Software\Classes\Wow6432Node\" } :
                new string[] { @"HKEY_CURRENT_USER\Software\Classes\", @"HKEY_CURRENT_USER\Software\Classes\Wow6432Node\" };
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
            var key = useLocalMachine ? Registry.LocalMachine : Registry.CurrentUser;
            key = key.OpenSubKey("Software", true).OpenSubKey("Classes", true);
            key.DeleteSubKeyTree(className, false);
            key.OpenSubKey("CLSID", true).DeleteSubKeyTree(guid, false);
            key.OpenSubKey("Wow6432Node", true).DeleteSubKeyTree(className, false);
            key.OpenSubKey("Wow6432Node", true).OpenSubKey("CLSID", true).DeleteSubKeyTree(guid, false);
        }

        private void OnFlexDMDSelected(object sender, RoutedEventArgs args)
        {
            _flexScript = new ScriptThread("FlexDMD Script Thread");
            _flexScript.Post(@"
                Dim DMD
                Set DMD = CreateObject(""FlexDMD.DMDObject"")
                DMD.Init
                ");
        }

        private void OnFlexDMDUnselected(object sender, RoutedEventArgs args)
        {
            if (_flexScript != null)
            {
                _flexScript.Interrupt();
                _flexScript.Post("If Not DMD is Nothing Then DMD.Uninit");
                _flexScript.Close(true, true);
                _flexScript = null;
            }
        }

        private void OnUltraDMDSelected(object sender, RoutedEventArgs args)
        {
            _ultraScript = new ScriptThread("FlexDMD Script Thread");
            _ultraScript.Post(@"
                Dim DMD
                Set DMD = CreateObject(""UltraDMD.DMDObject"")
                DMD.Init
                ");
        }

        private void OnUltraDMDUnselected(object sender, RoutedEventArgs args)
        {
            if (_ultraScript != null)
            {
                _ultraScript.Interrupt();
                _ultraScript.Post("If Not DMD is Nothing Then DMD.Uninit");
                _ultraScript.Close(true, true);
                _ultraScript = null;
            }
        }

        private void OnRunCmd(object sender, ExecutedRoutedEventArgs args)
        {
            OnRunScript(sender, null);
        }

        public void OnRunScript(object sender, RoutedEventArgs e)
        {
            var script = string.Format(@"
                FlexDMDMode = {0}
                UltraDMDMode = {1}
                {2}
                ", (renderFlexDMDBtn.IsChecked == true ? "True" : "False"), (renderUltraDMDBtn.IsChecked == true ? "True" : "False"), scriptTextBox.Text);
            if (_flexScript != null) _flexScript.Post(script);
            if (_ultraScript != null) _ultraScript.Post(script);
        }

        private void OnStopCmd(object sender, ExecutedRoutedEventArgs args)
        {
            OnStopScript(sender, null);
        }

        public void OnStopScript(object sender, RoutedEventArgs e)
        {
            if (_flexScript != null) _flexScript.Interrupt();
            if (_ultraScript != null) _ultraScript.Interrupt();
        }

    }
}
