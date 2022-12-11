using System;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace EFT_Pak_Loader
{
    public partial class MainMenu : Form
    {
        string hackPath = @"C:\Program Files\Windows Driver\platform_x64";
        string originalPaksCopyPath = @"C:\Program Files\Windows Driver\core_x64";
        string tarkovDataPath;

        Stopwatch sw = new Stopwatch();
        WebClient wc;

        public MainMenu()
        {
            InitializeComponent();

            tarkovDataPath = textBoxGameFolder.Text + "\\EFT\\EscapeFromTarkov_Data";

            if (Directory.Exists(hackPath))
            {
                groupBoxDownload.Enabled = false;
                lblProgressBar.Text = "Cheat downloaded!";
            }
            else
            {
                groupBoxGameFolder.Enabled = false;
                groupBoxFeatures.Enabled = false;
            }
        }

        // -----------------------------------------------------------------------------------------------------

        // ----- GroupBox Download -----
        #region Download Region
        // Download all files button
        private void btnDownload_Click(object sender, EventArgs e)
        {            
            using (wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

                Directory.CreateDirectory(hackPath);                

                wc.DownloadFileAsync(new Uri("ENTER_YOUR_URL_HERE"), hackPath);   // <- CHANGEME: Follow instructions
            }            
        }

        // The event that will fire whenever the progress of the WebClient is changed
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {           
            progressBar1.Value = e.ProgressPercentage;

            lblProgressBar.Text = e.ProgressPercentage.ToString() + "%";
        }

        // The event that will trigger when the WebClient is completed
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            sw.Reset();

            if (e.Cancelled == true)
            {
                MessageBox.Show("Cheat download has been cancelled!");
                lblProgressBar.Text = "Waiting for download...";
            }
            else
            {
                MessageBox.Show("Cheat download completed!");
                lblProgressBar.Text = "Cheat downloaded!";
                groupBoxGameFolder.Enabled = true;
                groupBoxFeatures.Enabled = true;
            }
        }
        #endregion

        // -----------------------------------------------------------------------------------------------------

        // ----- GroupBox Game folder -----
        private void textBoxGameFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select the 'Battlestate Games' folder!";

            if (fbd.ShowDialog() == DialogResult.OK && Directory.Exists(fbd.SelectedPath + "\\EFT\\EscapeFromTarkov_Data"))
            {
                MessageBox.Show("BattleState Games Folder:\n" + fbd.SelectedPath);
                textBoxGameFolder.Text = fbd.SelectedPath;
            }
            else
                MessageBox.Show("Thats the wrong folder! Try again..");
        }

        // -----------------------------------------------------------------------------------------------------

        // ----- GroupBox Features ------
        #region Install Button
        // Install the choosen hacks
        private void btnInstall_Click(object sender, EventArgs e)
        {        
            // Check if right game folder path
            if (!Directory.Exists(textBoxGameFolder.Text + "\\EFT\\EscapeFromTarkov_Data"))
            {
                MessageBox.Show("Wrong Game Folder selected!");
                return;
            }

            #region Save all (!) original files first
            // Save the original player files first
            if (!Directory.Exists(originalPaksCopyPath + "\\player"))
            {
                Directory.CreateDirectory(originalPaksCopyPath + "\\player");
            }

            foreach (var file in Directory.GetFiles(tarkovDataPath + "THE_RIGHT_FOLDER_1")) // CHANGEME: Enter the right folder
            {
                File.Copy(file, file.Replace(tarkovDataPath + "THE_RIGHT_FOLDER_1", originalPaksCopyPath + "\\player"), true); // CHANGEME: Enter the right folder
            }

            // Save the original item files first
            if (!Directory.Exists(originalPaksCopyPath + "\\item"))
            {
                Directory.CreateDirectory(originalPaksCopyPath + "\\item");
            }

            foreach (var file in Directory.GetFiles(tarkovDataPath + "THE_RIGHT_FOLDER_2")) // CHANGEME: Enter the right folder
            {
                File.Copy(file, file.Replace(tarkovDataPath + "THE_RIGHT_FOLDER_2", originalPaksCopyPath + "\\item"), true); // CHANGEME: Enter the right folder
            }

            // Save the original sound files first
            if (!Directory.Exists(originalPaksCopyPath + "\\sound"))
            {
                Directory.CreateDirectory(originalPaksCopyPath + "\\sound");
            }

            foreach (var file in Directory.GetFiles(tarkovDataPath + "THE_RIGHT_FOLDER_3")) // CHANGEME: Enter the right folder
            {
                File.Copy(file, file.Replace(tarkovDataPath + "THE_RIGHT_FOLDER_3", originalPaksCopyPath + "\\sound"), true); // CHANGEME: Enter the right folder
            }
            #endregion

            #region Replace with the modded ones
            // Check which features are enabled
            if (checkBoxPlayerESP.Checked)
            {
                // Replace the original files with the modded ones
                foreach (var file in Directory.GetFiles(hackPath + "\\player"))
                {
                    File.Copy(file, file.Replace(hackPath + "\\player", tarkovDataPath + "THE_RIGHT_FOLDER_1"), true); // CHANGEME: Enter the right folder
                }
            }

            if (checkBoxItemESP.Checked)
            {
                // Replace the original files with the modded ones
                foreach (var file in Directory.GetFiles(hackPath + "\\items"))
                {
                    File.Copy(file, file.Replace(hackPath + "\\items", tarkovDataPath + "THE_RIGHT_FOLDER_2"), true); // CHANGEME: Enter the right folder
                }
            }

            if (checkBoxNoSound.Checked)
            {
                // Replace the original files with the modded ones
                foreach (var file in Directory.GetFiles(hackPath + "\\sound"))
                {
                    File.Copy(file, file.Replace(hackPath + "\\sound", tarkovDataPath + "THE_RIGHT_FOLDER_3"), true); // CHANGEME: Enter the right folder
                }
            }
            #endregion

            lblInstallStatus.Visible = true;
            DialogResult result = MessageBox.Show("Ready to cheat!\n\n1. Start Tarkov via the BsgLauncher\n 2. The cheat will close automatically!", "Follow the instructions!", MessageBoxButtons.OK);
            if (result == DialogResult.OK)
            {
                findEFTexe.Start();
                lblGameStatus.Text = "Waiting for Game...";
            }
        }
        #endregion

        // -----------------------------------------------------------------------------------------------------

        #region Wait for EFT_BE.exe Timer
        // Wait for game to launch...
        private void findEFTexe_Tick(object sender, EventArgs e)
        {
            string process = "EscapeFromTarkov_BE";
            if (Process.GetProcessesByName(process).Length > 0)
            {
                lblGameStatus.Text = "GAME FOUND!!";
                findEFTexe.Stop();

                #region Copy all PAKS into EFT_Data
                // Copy all Orig PAKS into the target (EFT_Data)
                foreach (var file in Directory.GetFiles(originalPaksCopyPath + "\\player"))
                {
                    File.Copy(file, file.Replace(originalPaksCopyPath + "\\player", tarkovDataPath + "\\THE_RIGHT_FOLDER_1"), true); // CHANGEME: Enter the right folder
                }

                foreach (var file in Directory.GetFiles(originalPaksCopyPath + "\\items"))
                {
                    File.Copy(file, file.Replace(originalPaksCopyPath + "\\items", tarkovDataPath + "\\THE_RIGHT_FOLDER_2"), true); // CHANGEME: Enter the right folder
                }

                foreach (var file in Directory.GetFiles(originalPaksCopyPath + "\\sound"))
                {
                    File.Copy(file, file.Replace(originalPaksCopyPath + "\\sound", tarkovDataPath + "\\THE_RIGHT_FOLDER_3"), true); // CHANGEME: Enter the right folder
                }
                #endregion

                foreach (var me in Process.GetProcessesByName("FirevoxBrowser"))
                {
                    me.Kill();
                }
            }
        }
        #endregion

        // -----------------------------------------------------------------------------------------------------

        #region Application Exit Override
        // Exit Event Handler
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (ConfirmClose() == DialogResult.Yes)
            {
                Dispose(true);

                // ADD EACH FEATURE HERE!!
                // Copy all Orig PAKS into the target (EFT_Data)
                foreach (var file in Directory.GetFiles(originalPaksCopyPath))
                {
                    File.Copy(file, file.Replace(originalPaksCopyPath, tarkovDataPath), true);
                }

                Application.Exit();
            }
            else //no confirmation
            {
                e.Cancel = true;
            }
        }

        private DialogResult ConfirmClose()
        {
            DialogResult res = MessageBox.Show("Do you want to quit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return res;
        }
        #endregion
    }
}
