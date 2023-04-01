﻿using Guna.UI2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static CF_Game_Center.crackjson;
using static CF_Game_Center.Download;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static CF_Game_Center.DownloadManager;
using static System.Windows.Forms.LinkLabel;

namespace CF_Game_Center
{
    internal class Downloader
    {
        public void ShowDownloadPrompt(int Gameid, string GameType)
        {

            if (string.IsNullOrEmpty(GameType))
                return;
            Download downloadPrompt = new Download();
            downloadPrompt.Show();
            GameType = GameType.ToLower();
            switch (GameType)
            {
                case "cracked":
                    downloadPrompt.DownloadLBL.Text = "Download " + cracked.crack[Gameid].Gamename;
                    downloadPrompt.DownloadGameSizeLBL.Text = "Game Size : " + cracked.crack[Gameid].GameSize;
                    Main.downloadManager.DownloadGameIMG.Load(cracked.crack[Gameid].GamePoster);
                    Main.downloadManager.DownloadGameLBL.Text = cracked.crack[Gameid].Gamename;
                    break;
                case "official":
                    downloadPrompt.DownloadLBL.Text = "Download " + Official.crack[Gameid].Gamename;
                    downloadPrompt.DownloadGameSizeLBL.Text = "Game Size : " + Official.crack[Gameid].GameSize ;
                    Main.downloadManager.DownloadGameIMG.Load(Official.crack[Gameid].GamePoster);
                    Main.downloadManager.DownloadGameLBL.Text = Official.crack[Gameid].Gamename;
                    break;
                case "gfnpatch":
                    downloadPrompt.DownloadLBL.Text = "Download " + GFNPatch.crack[Gameid].Gamename;
                    downloadPrompt.DownloadGameSizeLBL.Text =  "Game Size : "+GFNPatch.crack[Gameid].GameSize;
                    Main.downloadManager.DownloadGameIMG.Load(GFNPatch.crack[Gameid].GamePoster);
                    Main.downloadManager.DownloadGameLBL.Text = GFNPatch.crack[Gameid].Gamename;
                    break;
            }
            while (Finished == false)
            {
                Application.DoEvents();
            }
            Finished = false;
            if (string.IsNullOrEmpty(mainpath))
            {
                return;
            }
            else
            {
                switch (GameType)
                {
                    case "cracked":
                        DownloadGame(Gameid, cracked.crack[Gameid].GameDownload);
                        break;
                    case "official":
                        DownloadGame(Gameid, Official.crack[Gameid].GameDownload);
                        break;
                    case "gfnpatch":
                        DownloadGame(Gameid, GFNPatch.crack[Gameid].GameDownload);
                        break;
                }

            }
        }
        private bool Done = true;
        public void DownloadGame(int Gameid, string DownloadURL)
        {

            if (!Done)
            {
                MessageBox.Show("Can't download while something is already downloading.");
                return;
            }

            string userName = Environment.UserName;
            WebClient webClient = new WebClient();

            // Check if the downloader.exe file exists in the download path
            if (!File.Exists(mainpath +  "downloader.exe"))
            {
                // If not, download rclone.conf and downloader.exe
                Directory.CreateDirectory($"C:\\Users\\{userName}\\.config\\rclone");
                webClient.DownloadFile("https://files.zortos.me/Files/CF%20GC%20Resources/rclone.conf", $"C:\\Users\\{userName}\\.config\\rclone\\rclone.conf");
                webClient.DownloadFile("https://picteon.dev/files/rclone.exe", mainpath + "downloader.exe");
            }



            Process process = new Process();
            process.StartInfo.FileName = mainpath + "downloader.exe";
            process.StartInfo.Arguments = $"copy -P --transfers=10 --checkers=16 {DownloadURL} {mainpath}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            // Set up events to monitor the progress of the download process
            process.OutputDataReceived += (sender, e) => process_OutputDataReceived(sender, e, Main.downloadManager);
            process.Exited += (sender, e) => p_Exited(sender,e , Main.downloadManager);
            process.EnableRaisingEvents = true;
            

           // Start the download process and show the progress bar
           Done = false;
            process.Start();
            process.BeginOutputReadLine();

            // Wait for the download to complete
            while (!Done)
            {
                Application.DoEvents();
            }

            // Update the UI to show that the game has been installed
            //Gameinstalled(Form1.currentjsonint);
            //await Task.Delay(5000);
            //    if (!string.IsNullOrEmpty(Form1.KeyAuthApp.user_data.username))
            //    {
            //        try
            //        {
            //            if (cloudsavemodule.SaveExists(crackjson.cloudsave.data.Count() - 1))
            //            {
            //                ((Control)(object)a.Download_Checksaves).Invoke((Delegate)(Action)delegate
            //                {
            //                    ((Control)(object)a.Download_Checksaves).Visible = true;
            //                });
            //                cloudsavemodule.RetrieveSavegame(crackjson.cloudsave.data.Count() - 1, Userpressed: false);
            //                ((Control)(object)a.Download_Checksaves).Invoke((Delegate)(Action)delegate
            //                {
            //                    ((Control)(object)a.Download_Checksaves).Text = "Save Found And Downloaded ;)";
            //                });
            //                await Task.Delay(2000);
            //                ((Control)(object)a.Download_Checksaves).Invoke((Delegate)(Action)delegate
            //                {
            //                    ((Control)(object)a.Download_Checksaves).Visible = false;
            //                });
            //                ((Control)(object)a.Download_Checksaves).Invoke((Delegate)(Action)delegate
            //                {
            //                    ((Control)(object)a.Download_Checksaves).Text = "Checking For Saves";
            //                });
            //            }
            //        }
            //        catch (Exception)
            //        {
            //        }
            //    }
            //    ((Control)(object)FilePlaybtn).Invoke((Delegate)(Action)delegate
            //    {
            //        ((Control)(object)FilePlaybtn).Visible = true;
            //    });
            //}
        }


        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e ,DownloadManager downloadMNGR)
        {
            try
            {

                

                if (Process.GetProcessesByName("downloader").Length != 0 )
                {
                    if (downloadMNGR.Visible)
                    {
                        int lines = 0;
                        downloadMNGR.FilesRichTXT.Invoke(new Action(() =>
                        {
                            lines = downloadMNGR.FilesRichTXT.Lines.Length;
                        }));
                        if (e.Data.Contains("ETA"))
                        {
                            string data = e.Data;
                            string text = data.Substring(data.IndexOf("Transferred:"));
                            string[] split = text.Split(',');
                            int progress = 0;
                            int num = text.IndexOf('%');
                            (downloadMNGR.DownloadGBLeftLBL).Invoke((Action)(() => downloadMNGR.DownloadGBLeftLBL.Text = split[0].Remove(0, 18)));
                            (downloadMNGR.DownloadSpeedLBL).Invoke((Action)(() => downloadMNGR.DownloadSpeedLBL.Text = split[2]));
                            (downloadMNGR.DownloadETALBL).Invoke((Action)(() => downloadMNGR.DownloadETALBL.Text = split[3]));
                            if (num >= 0)
                            {
                                string text2 = text.Substring(0, num);
                                int.TryParse(text2.Substring(text2.IndexOf(",") + 1), out progress);
                            }


                                (downloadMNGR.DownloadProgress).Invoke((Action)(() => downloadMNGR.DownloadProgress.Value = progress));

                        }
                        else if (e.Data.Contains("*") && !e.Data.Contains("ETA"))
                        {
                            try
                            {
                                int maxLines = 10;
                                var richTextBox = downloadMNGR.FilesRichTXT;

                                if (richTextBox.InvokeRequired)
                                {
                                    richTextBox.Invoke(new Action(() =>
                                    {
                                        if (richTextBox.Lines.Length >= maxLines)
                                        {
                                            // Remove the oldest line if there are already 10 lines displayed
                                            List<string> linesList = new List<string>(richTextBox.Lines);
                                            linesList.RemoveAt(0);
                                            richTextBox.Lines = linesList.ToArray();
                                        }

                                        // Add the new data
                                        if (!string.IsNullOrEmpty(e.Data?.ToString()))
                                        {
                                            richTextBox.AppendText(Environment.NewLine);
                                            richTextBox.AppendText(e.Data.ToString());
                                        }
                                    }));
                                }
                                else
                                {
                                    if (richTextBox.Lines.Length >= maxLines)
                                    {
                                        // Remove the oldest line if there are already 10 lines displayed
                                        List<string> linesList = new List<string>(richTextBox.Lines);
                                        linesList.RemoveAt(0);
                                        richTextBox.Lines = linesList.ToArray();
                                    }

                                    // Add the new data
                                    if (!string.IsNullOrEmpty(e.Data?.ToString()))
                                    {
                                        richTextBox.AppendText(Environment.NewLine);
                                        richTextBox.AppendText(e.Data.ToString());
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.ToString());
                            }

                        }

                    }
                }
               
            }
            catch (NullReferenceException)
            {
                // do nothing
            }
        }

        private async void p_Exited(object sender, EventArgs e, DownloadManager downloadMNGR)
        {
            try
            {
                downloadMNGR.DownloadETALBL.Invoke((Action)(() => downloadMNGR.DownloadETALBL.Text = string.Empty));
                downloadMNGR.DownloadSpeedLBL.Invoke((Action)(() => downloadMNGR.DownloadSpeedLBL.Text = string.Empty));
                downloadMNGR.DownloadProgress.Invoke((Action)(() => downloadMNGR.DownloadProgress.Value = 0));
            }
            catch (Exception)
            {
                return;
            }

            Done = true;
        }
    }
}
