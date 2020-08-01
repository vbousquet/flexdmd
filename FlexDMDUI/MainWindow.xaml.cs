using FlexDMD;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            /*            scriptTextBox.Text =
            @"' Demo script

            Public Sub FlexDemo()
                ' Use DMD object for FlexDMD or UDMD object for UltraDMD
                DMD.GameName = NULL
                DMD.GameName = ""ft""

                Set font = DMD.NewFont(""FlexDMD.Resources.teeny_tiny_pixls-5.fnt"", 1.0, -1.0)

                Set scene1 = DMD.NewGroup(""Scene 1"")
                scene1.AddActor DMD.NewImage(""Diablo"", ""Diablo.UltraDMD/black.jpg"")
                scene1.AddActor DMD.NewLabel(""Label"", font, ""Test"")
                scene1.GetLabel(""Label"").SetAlignedPosition 64, 16, 4

                Set scene2 = DMD.NewGroup(""Scene 2"")
                scene2.AddActor DMD.NewVideo(""Video"", ""Diablo.UltraDMD/act1.wmv"")

                Set sequence = DMD.NewGroup(""Sequence"")
                sequence.SetSize 128, 32
                Set af = sequence.ActionFactory
                Set list = af.Sequence()
                list.Add af.AddChild(scene1)
                list.Add af.Wait(5)
                list.Add af.RemoveChild(scene1)
                list.Add scene2.GetVideo(""Video"").ActionFactory.Seek(0)
                list.Add af.AddChild(scene2)
                list.Add af.Wait(5)
                list.Add af.RemoveChild(scene2)
                sequence.AddAction af.Repeat(list, -1)

                DMD.LockRenderThread
                DMD.Stage.RemoveAll
                DMD.Stage.AddActor sequence
                DMD.UnlockRenderThread
            End Sub

            ' Check that FlexDMD renders the same as UltraDMD
            Public Sub CompareUltraFlex()
                ' Animations :
                '  FadeIn = 0, // Fade from black to scene
                '  FadeOut = 1, // Fade from scene to black
                '  ZoomIn = 2, // zoom from a centered small dmd to full size [Not supported yet]
                '  ZoomOut = 3, // zoom from a full sized dmd to an oversize one [Not supported yet]
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
                UDMD.DisplayScene00 """", ""Fade In / Out"", 15, "".."", 15, 0, 1000, 1
                UDMD.DisplayScene00 """", ""Scroll On/Off Right"", 15, ""..."", 15, 7, 1000, 5
                UDMD.DisplayScene00 """", ""Scroll On/Off Left"", 15, ""..."", 15, 6, 1000, 4
                UDMD.DisplayScene00 """", ""Scroll On/Off Down"", 15, ""..."", 15, 11, 1000, 9
                UDMD.DisplayScene00 """", ""Scroll On/Off Up"", 15, ""..."", 15, 10, 1000, 8
                UDMD.DisplayScene00 """", ""Fill Fade In / Out"", 15, "".."", 15, 12, 1000, 13

                ' Scrolling text scene
                UDMD.DisplayScene01 """", """", ""Scrolling Text"", 5, 15, 14, 5000, 14

                ' Scrolling credits scene
                UDMD.ScrollingCredits """", ""Scrolling|Credits||Multiple|lines|of text"", 15, 14, 5000, 14
            End Sub

            If FlexDMDMode And UltraDMDMode Then
                CompareUltraFlex()
            ElseIf FlexDMDMode Then
                FlexDemo()
            Else
                UDMD.DisplayScene00 """", ""UltraDMD"", 15, ""."", 15, 14, 1000, 14
            End If
            "; */
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
            if (File.Exists(Path.Combine(_installPath, @"FlexDMD.dll")))
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = "FlexDMD.dll was found in the provided location.";
            }
            else
            {
                flexDMDDllInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                flexDMDDllInstallLabel.Content = "FlexDMD.dll was not found. Check that you have entered the folder where you have placed this file.";
            }
            if (File.Exists(Path.Combine(_installPath, @"dmddevice.dll")))
            {
                dmdDeviceInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                dmdDeviceInstallLabel.Content = "DmdDevice.dll was found alongside FlexDMD.dll.";
            }
            else
            {
                dmdDeviceInstallImage.Source = new BitmapImage(new Uri(@"Resources/cross.png", UriKind.RelativeOrAbsolute));
                dmdDeviceInstallLabel.Content = "DmdDevice.dll was not found alongside FlexDMD.dll. No rendering will happen.";
            }

            var flexDMDinstall = GetComponentLocation(flexDMDclsid);
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

            var ultraDMDinstall = GetComponentLocation(ultraDMDclsid);
            if (ultraDMDinstall != null && ultraDMDinstall.ToUpperInvariant().EndsWith("FLEXUDMD.DLL"))
            {
                ultraDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                ultraDMDInstallLabel.Content = "FlexDMD is registered to be used instead of UltraDMD.";
                registerUltraDMDBtn.Content = "Unregister";
            }
            else if (ultraDMDinstall != null && ultraDMDinstall.ToUpperInvariant().EndsWith("ULTRADMD.EXE"))
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
            if (path != null) return new Uri(path.ToString()).LocalPath;
            return null;
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

        public void OnRegisterFlex(object sender, RoutedEventArgs e)
        {
            if (GetComponentLocation(flexDMDclsid) != null)
                Register("/unregister \"" + _installPath + "\"");
            else
                Register("/register \"" + _installPath + "\"");
            UpdateInstallPane();
        }

        public void OnRegisterUltra(object sender, RoutedEventArgs e)
        {
            var ultraDMDinstall = GetComponentLocation(ultraDMDclsid);
            if (ultraDMDinstall != null && ultraDMDinstall.ToUpperInvariant().EndsWith("FLEXUDMD.DLL"))
                Register("/unregister-udmd \"" + _installPath + "\"");
            else
                Register("/register-udmd \"" + _installPath + "\"");
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
    }
}
