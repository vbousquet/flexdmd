using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows;

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

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private static void Register(string path, string command)
        {
            var reg = new RegistrationServices();
            var file = "";
            var success = false;
            try
            {
                switch (command)
                {
                    case "/register":
                        file = "FlexDMD.dll";
                        success = reg.RegisterAssembly(Assembly.LoadFile(System.IO.Path.Combine(path, file)), AssemblyRegistrationFlags.SetCodeBase);
                        break;
                    case "/unregister":
                        file = "FlexDMD.dll";
                        success = reg.UnregisterAssembly(Assembly.LoadFile(System.IO.Path.Combine(path, file)));
                        break;
                    case "/register-udmd":
                        file = "FlexUDMD.dll";
                        success = reg.RegisterAssembly(Assembly.LoadFile(System.IO.Path.Combine(path, file)), AssemblyRegistrationFlags.SetCodeBase);
                        break;
                    case "/unregister-udmd":
                        file = "FlexUDMD.dll";
                        success = reg.UnregisterAssembly(Assembly.LoadFile(System.IO.Path.Combine(path, file)));
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
            }
            Environment.Exit(0);
        }

    }
}

