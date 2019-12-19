using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
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
            var flexPath = getComponentLocation(flexDMDclsid);
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
            var flexDMDinstall = getComponentLocation(flexDMDclsid);
            var ultraDMDinstall = getComponentLocation(ultraDMDclsid);
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
            if (ultraDMDinstall != null && ultraDMDinstall == flexDMDinstall)
            {
                ultraDMDInstallImage.Source = new BitmapImage(new Uri(@"Resources/check.png", UriKind.RelativeOrAbsolute));
                ultraDMDInstallLabel.Content = "FlexDMD is registered to be used instead of UltraDMD.";
                registerUltraDMDBtn.Content = "Unregister";
            }
            else if (ultraDMDinstall != null)
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

        private string getComponentLocation(string clsid)
        {
            using (var classesRootKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.ClassesRoot, Microsoft.Win32.RegistryView.Default))
            {
                var clsIdKey = classesRootKey.OpenSubKey(@"Wow6432Node\CLSID\" + clsid + @"\InprocServer32") ?? classesRootKey.OpenSubKey(@"CLSID\" + clsid + @"\InprocServer32");
                if (clsIdKey != null)
                {
                    var path = new Uri(clsIdKey.GetValue("CodeBase").ToString()).AbsolutePath;
                    clsIdKey.Dispose();
                    return path;
                }
                return null;
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
                    // System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                }
            }
        }

        public void OnRegisterFlex(object sender, RoutedEventArgs e)
        {
            Assembly asm = Assembly.LoadFile(System.IO.Path.Combine(_installPath, @"FlexDMD.dll"));
            RegistrationServices regAsm = new RegistrationServices();
            if (getComponentLocation(flexDMDclsid) != null)
            {
                regAsm.UnregisterAssembly(asm);
            }
            else
            {
                regAsm.RegisterAssembly(asm, AssemblyRegistrationFlags.SetCodeBase);
            }
            UpdateInstallPane();
        }

        public void OnRegisterUltra(object sender, RoutedEventArgs e)
        {
            Assembly asm = Assembly.LoadFile(System.IO.Path.Combine(_installPath, @"FlexDMD.dll"));
            RegistrationServices regAsm = new RegistrationServices();
            // FIXME register/unregister FlexDMD with the CLSID of UltraDMD
            UpdateInstallPane();
        }

        public void OnRun(object sender, RoutedEventArgs e)
        {
            // FIXME run the script with wscript.exe, prepending it with the object creation, running once per output
            // https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/wscript
        }
    }
}
