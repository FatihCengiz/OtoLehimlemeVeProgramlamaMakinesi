using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OtoLehimlemeVeProgramlamaMakinesi
{
    public class Programming2
    {
        public Main mainFrm;
        string[] batchFileFeedback2 = new string[5];

        private Thread programing2BatchThread = null;
        Process processBatchPrograming2 = new Process();
        public Thread programing2ErrorTh = null;
        bool programing2Sonuc = false;
        public Programming2(Main main)
        {
            mainFrm = main;
        }

        /********************************************** Programing Init ************************************************/
        public void programing2Init()
        {/*
            batchFileFeedback2[0] = "P R O G R A M M I N G    O K";
            batchFileFeedback2[1] = "Succeeded";
            batchFileFeedback2[2] = "color ce";
            batchFileFeedback2[3] = "!!! H A T A !!!";
            batchFileFeedback2[4] = "FAILED"; */

            batchFileFeedback2[0] = "P R O G R A M M I N G    O K";
            batchFileFeedback2[1] = "Succeeded";
            batchFileFeedback2[2] = "color ce";
            batchFileFeedback2[3] = "!!! H A T A !!!";
            batchFileFeedback2[4] = "FAILED"; 
        }

        /********************************************** Programing Start ************************************************/
        public void programming2Start()
        {
            this.mainFrm.lblSonuc2.Visible = true;
            this.mainFrm.lblSonuc2.BackColor = Color.Yellow;
            this.mainFrm.lblSonuc2.ForeColor = Color.Black;
            this.mainFrm.lblSonuc2.Text = "PROGRAM YÜKLENİYOR";

            this.mainFrm.lblSonucF2.BackColor = Color.Yellow;
            this.mainFrm.lblSonucF2.ForeColor = Color.Black;
            this.mainFrm.lblSonucF2.Text = "PROGRAM YÜKLENİYOR";
            // Clean console
            mainFrm.otherConsoleClean_2();

            string product_no = string.Empty;

            // Show message box
            mainFrm.otherConsoleAppendLine_2("Harvest Fresh Kartı Programlama 2: " + mainFrm.programingPathF2, Color.White);
            mainFrm.otherConsoleNewLine_2();

            program2Product();
        }

        /*Barkoddan ayıklanan ürün için .bat Seçilir*/
        private void program2Product()
        {
            try
            {
                if (mainFrm.programingPathF2 != "")  //Yolu değiştir
                {
                    String batchPath = String.Empty;
                    batchPath = mainFrm.programingPathF2; //+ ".bat";    // C:\Users\serkan.baki\Desktop\MIND-BATCH-FILES\
                    runBatch(batchPath, this.mainFrm.cmbBxFileNameF2.Text);
                }
                else
                {
                    //CustomMessageBox.ShowMessage("Dosya Yolu Boş Kalamaz!", MainFrm.customMessageBoxTitle, MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
                    mainFrm.otherConsoleAppendLine_2("Dosya Yolu Boş Kalamaz", Color.Red);
                    this.mainFrm.lblSonuc2.Text = "Dosya Yolu Boş Kalamaz";
                }
            }
            catch (Exception ex)
            {
                mainFrm.otherConsoleAppendLine_2("program2Product: " + ex.Message, Color.Red);
            }
        }

        /*Seçilen .bat Çalıştırılır- Kontrol Edilir ve Kapatılır*/
        private void runBatch(string batch_path, string batch_name)
        {
            programing2BatchThread = new Thread(programing2BatchThreadFunction);
            programing2BatchThread.Start(batch_path);
        }

        private void programing2BatchThreadFunction(object batch_path)
        {
            processBatchPrograming2.StartInfo.UseShellExecute = false;
            processBatchPrograming2.StartInfo.RedirectStandardOutput = true;
            processBatchPrograming2.StartInfo.CreateNoWindow = true;
            processBatchPrograming2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processBatchPrograming2.StartInfo.FileName = (string)batch_path;
            //processBatchPrograming2.StartInfo.Arguments = string.Format("");
            processBatchPrograming2.Start();

            StreamReader strmReader = processBatchPrograming2.StandardOutput;
            string batchTempRow = string.Empty;
            // get all lines of batch
            while ((batchTempRow = strmReader.ReadLine()) != null)
            {
                // Write batch operation to the console
                mainFrm.otherConsoleAppendLine_2(batchTempRow, Color.Black);

                // check programming is successful.
                // if succesfully finished.
                if (((batchTempRow.IndexOf("pause", StringComparison.OrdinalIgnoreCase) >= 0) || (batchTempRow.IndexOf(batchFileFeedback2[0], StringComparison.OrdinalIgnoreCase) >= 0)))  // color ae
                {
                    mainFrm.otherConsoleNewLine_2();
                    mainFrm.otherConsoleAppendLine_2("Harvest Fresh Kartı2:" + " programing İşlemi Başarıyla Tamamlanmıştır!", Color.Green);
                   // this.mainFrm.lblTestResF2.Text = "Programlama Başarılı";
                    programing2Sonuc = true;
                    break;
                }
                else if (((batchTempRow.IndexOf("pause", StringComparison.OrdinalIgnoreCase) >= 0) || (batchTempRow.IndexOf(batchFileFeedback2[1], StringComparison.OrdinalIgnoreCase) >= 0)))  // color ae
                {
                    mainFrm.otherConsoleNewLine_2();
                    mainFrm.otherConsoleAppendLine_2("Harvest Fresh Kartı2:" + " programing İşlemi Başarıyla Tamamlanmıştır!", Color.Green);
                   // this.mainFrm.lblTestResF2.Text = "Programlama Başarılı";
                    programing2Sonuc = true;
                    break;
                }
                else if (((batchTempRow.IndexOf("pause", StringComparison.OrdinalIgnoreCase) >= 0) || (batchTempRow.IndexOf(batchFileFeedback2[2], StringComparison.OrdinalIgnoreCase) >= 0))) //Could not start CPU core.
                {
                    mainFrm.otherConsoleNewLine_2();
                    mainFrm.otherConsoleAppendLine_2("Harvest Fresh Kartı2:" + " programing İşlemi Başarısız1.", Color.Red);  // programing Soketi Düzgün Takılı Değil!
                   // this.mainFrm.lblTestResF2.Text = "Programlama Başarısız 1";
                    programing2Sonuc = false;
                    break;
                }
                else if (((batchTempRow.IndexOf("pause", StringComparison.OrdinalIgnoreCase) >= 0) || (batchTempRow.IndexOf(batchFileFeedback2[3], StringComparison.OrdinalIgnoreCase) >= 0)))  // Cannot connect to target.
                {
                    mainFrm.otherConsoleNewLine_2();
                    mainFrm.otherConsoleAppendLine_2("Harvest Fresh Kartı2:" + " programing İşlemi Başarısız2.", Color.Red); // programing Soketi Takılı Değil!
                    //this.mainFrm.lblTestResF2.Text = "Programlama Başarısız 2";
                    programing2Sonuc = false;
                    break;
                }
                else if (((batchTempRow.IndexOf("pause", StringComparison.OrdinalIgnoreCase) >= 0) || (batchTempRow.IndexOf(batchFileFeedback2[4], StringComparison.OrdinalIgnoreCase) >= 0))) //FAILED
                {
                    mainFrm.otherConsoleNewLine_2();
                    mainFrm.otherConsoleAppendLine_2("Harvest Fresh Kartı2:" + " programing İşlemi Başarısız3.", Color.Red);  //  USB Takılı Değil!
                    //this.mainFrm.lblTestResF2.Text = "Programlama Başarısız 3";
                    programing2Sonuc = false;
                    break;
                }
            }

            // if batch didn't closed kill it.
            if (!processBatchPrograming2.HasExited)
            {
                processBatchPrograming2.Kill();
            }
            ExecuteCommand(mainFrm.desktopPath + @"/Test_Application/Exit.bat");
            program2SonucFunction();
        }

        public void program2SonucFunction()
        {
            if (programing2Sonuc)
            {
                this.mainFrm.lblSonucF2.Invoke(new Action(delegate ()
                {
                    this.mainFrm.lblSonucF2.BackColor = Color.Green;
                    this.mainFrm.lblSonucF2.Text = "2.PROGRAMLAMA BAŞARILI";
                }));
                this.mainFrm.lblSonuc2.Invoke(new Action(delegate ()
                {
                    this.mainFrm.lblSonuc2.Visible = true;
                    this.mainFrm.lblSonuc2.BackColor = Color.Green;
                    this.mainFrm.lblSonuc2.Text = "2.PROGRAMLAMA BAŞARILI";
                }));
                programing2Success();

            }
            else
            {
                this.mainFrm.lblSonucF2.Invoke(new Action(delegate ()
                {
                    this.mainFrm.lblSonucF2.BackColor = Color.Red;
                    this.mainFrm.lblSonucF2.Text = "1.PROGRAMLAMA BAŞARISIZ";
                }));
                this.mainFrm.lblSonuc2.Invoke(new Action(delegate ()
                {
                    this.mainFrm.lblSonuc2.Visible = true;
                    this.mainFrm.lblSonuc2.BackColor = Color.Red;
                    this.mainFrm.lblSonuc2.Text = "2.PROGRAMLAMA BAŞARISIZ";
                }));
                programing2Error();
            }
            programing2Restart();
        }

        public void programing2Restart()  //PROGRAMLAMA RESTART
        {
            mainFrm.programing2TimeoutState = false;
        }

        public void programing2Success()  //PROGRAMLAMA BAŞARILI
        {
            try
            {
                if (mainFrm.nxCompoletBoolWrite("f2ProgramSuccess", true))
                {
                    programing2Restart();
                }
                else
                {
                    programing2Error();
                }
            }
            catch (Exception ex)
            {
               /* programing2Error();
                mainFrm.otherConsoleAppendLine_2("program2Success: " + ex.Message, Color.Red);*/
            }
        }

        public void programing2Error()  //PROGRAMLAMA BAŞARISIZ
        {
            programing2ErrorTh = new Thread(programing2ErrorFunction);
            programing2ErrorTh.Start();
        }

        private void programing2ErrorFunction()  //PROGRAMLAMA BAŞARISIZ  //İlgili Programı Kapatmak İçin???
        {
            try
            {
                mainFrm.nxCompoletBoolWrite("f2ProgramFail", true);
                mainFrm.otherConsoleAppendLine_2("program2Fail Gönderildi: ", Color.Red);
                programing2Restart();
            }
            catch (Exception ex)
            {
                mainFrm.otherConsoleAppendLine_2("programing2Error: " + ex.Message, Color.Red);
            }
        }

        static void ExecuteCommand(string command)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
            Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
            Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            process.Close();
        }


    }
}
