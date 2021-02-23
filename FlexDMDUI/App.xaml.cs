using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using Trinet.Core.IO.Ntfs;

namespace FlexDMDUI
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (e.Args.Length == 2)
                Register(e.Args[1], e.Args[0].ToLowerInvariant());
            StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
        }

        private static string GetRegAsmPath()
        {
            var basepath = "";
            var regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NetFramework", false);
            if (regkey != null)
            {
                basepath = (string)regkey.GetValue("InstallRoot");
                regkey.Close();
            }
            regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NetFramework\Policy\v4.0", false);
            if (regkey != null)
            {
                var version = "v4.0";
                foreach (string valuename in regkey.GetValueNames())
                {
                    var regAsmPath = Path.Combine(basepath, version + "." + valuename);
                    if (Directory.Exists(regAsmPath)) return regAsmPath;
                }
                regkey.Close();
            }
            return null;
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private static void Register(string path, string command)
        {
            switch (command)
            {
                case "/register":
                case "/unregister":
                    Registry.ClassesRoot.DeleteSubKeyTree(@"FlexDMD.FlexDMD", false);
                    Registry.ClassesRoot.OpenSubKey(@"CLSID", true).DeleteSubKeyTree("{766E10D3-DFE3-4E1B-AC99-C4D2BE16E91F}", false);
                    Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\CLSID", true).DeleteSubKeyTree("{766E10D3-DFE3-4E1B-AC99-C4D2BE16E91F}", false);
                    break;
                case "/register-udmd":
                case "/unregister-udmd":
                    Registry.ClassesRoot.DeleteSubKeyTree(@"UltraDMD.DMDObject", false);
                    Registry.ClassesRoot.OpenSubKey(@"CLSID", true).DeleteSubKeyTree("{E1612654-304A-4E07-A236-EB64D6D4F511}", false);
                    Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\CLSID", true).DeleteSubKeyTree("{E1612654-304A-4E07-A236-EB64D6D4F511}", false);
                    break;
            }
            switch (command)
            {
                case "/register":
                    UnblockDll(Path.Combine(path, @"FlexDMD.dll"));
                    UnblockDll(Path.Combine(path, @"dmddevice.dll"));
                    UnblockDll(Path.Combine(path, @"dmddevice64.dll"));
                    RegisterAssembly(path, "FlexDMD.dll", true);
                    break;
                case "/register-udmd":
                    UnblockDll(Path.Combine(path, @"FlexUDMD.dll"));
                    RegisterAssembly(path, "FlexUDMD.dll", true);
                    break;
            }
            Environment.Exit(0);
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private static void UnblockDll(string path)
        {
            try
            {
                FileInfo file = new FileInfo(path);
                if (file.Exists) file.DeleteAlternateDataStream("Zone.Identifier");
            } catch (Exception)
            {
                // FIXME this is an horrible exception swallowing when DLL unblocking fails. This should be properly reported (at leats in a log file)
            }
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private static void RegisterAssembly(string path, string file, bool register)
        {
            var regAsmPath = GetRegAsmPath();
            if (regAsmPath == null)
            {
                System.Windows.Forms.MessageBox.Show("Failed to register " + file + ".\n\nThe .NET framework was not found.", "Failed to register " + file);
            }
            else
            {
                var regAsm = Path.Combine(regAsmPath, "regasm.exe");
                var regAsm64 = Path.Combine(regAsmPath.Replace(@"\Framework\", @"\Framework64\"), "regasm.exe");
                if (File.Exists(regAsm))
                {
                    var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = regAsm;
                    process.StartInfo.WorkingDirectory = path;
                    if (register)
                        process.StartInfo.Arguments = file + @" /codebase";
                    else
                        process.StartInfo.Arguments = file + @" /u";
                    process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    process.Start();
                    process.WaitForExit();
                    process.Close();
                    process.Dispose();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Failed to register " + file + " for x86.\n\nMissing x86 version of RegAsm.exe.", "Failed to register " + file);
                }
                if (File.Exists(regAsm64))
                {
                    var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = regAsm64;
                    process.StartInfo.WorkingDirectory = path;
                    if (register)
                        process.StartInfo.Arguments = file + @" /codebase";
                    else
                        process.StartInfo.Arguments = file + @" /u";
                    process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    process.Start();
                    process.WaitForExit();
                    process.Close();
                    process.Dispose();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Failed to register " + file + " for x64.\n\nMissing x64 version of RegAsm.exe.", "Failed to register " + file);
                }
            }
            /* Registration service will not register both the x86 and the x64 version. Therefore we use the fallback of directly referencing regasm
            var reg = new RegistrationServices();
            var file = "";
            var success = false;
            try
            {
                switch (command)
                {
                    case "/register":
                        file = "FlexDMD.dll";
                        success = reg.RegisterAssembly(Assembly.LoadFile(Path.Combine(path, file)), AssemblyRegistrationFlags.SetCodeBase);
                        break;
                    case "/unregister":
                        file = "FlexDMD.dll";
                        success = reg.UnregisterAssembly(Assembly.LoadFile(Path.Combine(path, file)));
                        break;
                    case "/register-udmd":
                        file = "FlexUDMD.dll";
                        success = reg.RegisterAssembly(Assembly.LoadFile(Path.Combine(path, file)), AssemblyRegistrationFlags.SetCodeBase);
                        break;
                    case "/unregister-udmd":
                        file = "FlexUDMD.dll";
                        success = reg.UnregisterAssembly(Assembly.LoadFile(Path.Combine(path, file)));
                        break;
                }
                if (!success) System.Windows.Forms.MessageBox.Show("Failed to register " + file + ".\n\nUnknown error.", "Failed to register " + file);
            }
            catch (UnauthorizedAccessException)
            {
                System.Windows.Forms.MessageBox.Show("Failed to register " + file + " due to missing permissions.\n\nThis action must be performed with administrator privileges.", "Failed to register " + file);
            }
            catch (FileNotFoundException)
            {
                System.Windows.Forms.MessageBox.Show("Failed to register " + file + " due to missing file.\n\nFile was expected at: " + path, "Failed to register " + file);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Failed to register " + file + ".\n\nUnhandled exception: " + e.Message, "Failed to register " + file);
            }*/
        }

    }
}

