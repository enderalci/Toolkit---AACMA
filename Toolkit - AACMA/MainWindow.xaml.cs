using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;



/// Copyright (c) Enter-Consult BV - Created by Ender Alci

namespace Toolkit___AACMA { 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


    /// Creates the AzCmAgent Command because this is not built-in, so we call this up 
    private string RunAzCmAgentCommand(string args)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "azcmagent",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    return string.IsNullOrWhiteSpace(output) ? error : output;
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }




      
        /// Cals the show Agent Version, Logfile, Status and Last Heartbeat
        private void btnVersion_Click(object sender, RoutedEventArgs e)
        {
            string AgentversionCheck = "show \"Agent Version\" \"Agent Logfile\" \"Agent Status\" \"Agent Last Heartbeat\" ";
            /// Test string output -- txtOutput.Text = Agentversion;
            txtOutput.Text = RunAzCmAgentCommand(AgentversionCheck);
        }

        /// Calls the show Agent Error Code, Details and Timestamp
        private void btnAgentErrorCheck_Click(object sender, RoutedEventArgs e)
        {
            string AgentErrorCheck = "show \"Agent Error Code\" \"Agent Error Details\" \"Agent Error Timestamp\"  ";
            /// Test string output -- txtOutput.Text = Agentversion;
            txtOutput.Text = RunAzCmAgentCommand(AgentErrorCheck);
        }

        /// Calls the show Agent Configuration
        private void btnConfigList_Click(object sender, RoutedEventArgs e)
        {
            txtOutput.Text = RunAzCmAgentCommand("config list");
        }

        private void btnAgentMode_Click(object sender, RoutedEventArgs e)
        {
            txtOutput.Text = RunAzCmAgentCommand("config get config.mode");
        }

        private void btnExportLogs_Click(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("Enter path, ex: c:\temp\azcmagentlogs");
            string ExportLogsPath = Console.ReadLine();
            ///MessageBox.Show("Azcmagent logs --full --output " + "\"" + ExportLogsPath + "\"");
            txtOutput.Text = RunAzCmAgentCommand("logs --full --output" + "\"" + ExportLogsPath + "\"");
        }

        private void btnUpdateAACMA_Click(object sender, RoutedEventArgs e)
        {
                string installerUrl = "https://aka.ms/AzureConnectedMachineAgent";
                string tempPath = Path.GetTempPath();
                string installerPath = Path.Combine(tempPath, "AzureConnectedMachineAgent.msi");

                try
                {
                    using (WebClient client = new WebClient())
                    {
                        MessageBox.Show("Downloading Azure Connected Machine Agent...");
                        client.DownloadFile(installerUrl, installerPath);
                    }

                    Process installer = new Process();
                    installer.StartInfo.FileName = "msiexec.exe";
                    installer.StartInfo.Arguments = $"/i \"{installerPath}\" /quiet /qn /norestart";
                    installer.StartInfo.Verb = "runas"; // Admin rights
                    installer.StartInfo.UseShellExecute = true;
                    installer.Start();

                    installer.WaitForExit();

                    MessageBox.Show("Installation completed.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
    }
}
