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
    public class Programming1
    {
        public Main mainFrm;
        string[] batchFileFeedback1 = new string[5];
        private Thread programing1BatchThread = null;
        Process processBatchPrograming1 = new Process();
        public Thread programing1ErrorTh = null;
        bool programing1Sonuc = false;

        public Programming1(Main main)
        {
            mainFrm = main;
        }

        /********************************************** Programing Init ************************************************/
        public void programing1Init()
        {/*
            batchFileFeedback1[0] = "P R O G R A M M I N G    O K";
            batchFileFeedback1[1] = "Succeeded";
            batchFileFeedback1[2] = "color ce";
            batchFileFeedback1[3] = "!!! H A T A !!!";
            batchFileFeedback1[4] = "FAILED";*/

            batchFileFeedback1[0] = "P R O G R A M M I N G    O K";
            batchFileFeedback1[1] = "Succeeded";
            batchFileFeedback1[2] = "color ce";
            batchFileFeedback1[3] = "!!! H A T A !!!";
            batchFileFeedback1[4] = "FAILED"; 
        }

        /********************************************** Programing Start ************************************************/
        public void programming1Start()
        {
            this.mainFrm.lblSonuc1.Visible = true;
            this.mainFrm.lblSonuc1.Text = "PROGRAM YÜKLENİYOR";
            this.mainFrm.lblSonuc1.BackColor = Color.Yellow;
            this.mainFrm.lblSonuc1.ForeColor = Color.Black;
   ;

            this.mainFrm.lblSonucF1.BackColor = Color.Yellow;
            this.mainFrm.lblSonucF1.ForeColor = Color.Black;
            this.mainFrm.lblSonucF1.Text = "PROGRAM YÜKLENİYOR";

            //Clean console
            mainFrm.otherConsoleClean_2();

            string product_no = string.Empty;

            // Show message box
            mainFrm.otherConsoleAppendLine_2("Harvest Fresh Kartı Programlama 1:" + mainFrm.programingPathF1, Color.White);
            mainFrm.otherConsoleNewLine_2();
            program1Product();
        }

        /*Barkoddan ayıklanan ürün için .bat Seçilir*/
        private void program1Product()
        {
            try
            {
                
                if (mainFrm.programingPathF1 != "")  //Yolu değiştir
                {
                    String batchPath = String.Empty;
                    batchPath = mainFrm.programingPathF1;  // C:\Users\serkan.baki\Desktop\MIND-BATCH-FILES\
                    runBatch(batchPath, this.mainFrm.cmbBxFileNameF1.Text);
                }
                else
                {
                    //CustomMessageBox.ShowMessage("Dosya Yolu Boş Kalamaz!", MainFrm.customMessageBoxTitle, MessageBoxButtons.OK, CustomMessageBoxIcon.Error, Color.Red);
                    mainFrm.otherConsoleAppendLine_2("Dosya Yolu Boş Kalamaz", Color.Red);
                    this.mainFrm.lblSonuc1.Text = "Dosya Yolu Boş Kalamaz";

                }
            }
            catch (Exception ex)
            {
                mainFrm.otherConsoleAppendLine_2("program1Product: " + ex.Message, Color.Red);
            }
        }

        /*Seçilen .bat Çalıştırılır- Kontrol Edilir ve Kapatılır*/
        private void runBatch(string batch_path, string batch_name)
        {
            programing1BatchThread = new Thread(programing1BatchThreadFunction);
            programing1BatchThread.Start(batch_path);
        }

        private void programing1BatchThreadFunction(object batch_path)
        {
            processBatchPrograming1.StartInfo.UseShellExecute = false;
            processBatchPrograming1.StartInfo.RedirectStandardOutput = true;
            processBatchPrograming1.StartInfo.CreateNoWindow = true;
            processBatchPrograming1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processBatchPrograming1.StartInfo.FileName = (string)batch_path;
            //processBatchPrograming1.StartInfo.Arguments = string.Format("");
            processBatchPrograming1.Start();

            StreamReader strmReader = processBatchPrograming1.StandardOutput;
            string batchTempRow = string.Empty;
            // get all lines of batch
            while ((batchTempRow = strmReader.ReadLine()) != null)
            {
                // Write batch operation to the console
                mainFrm.otherConsoleAppendLine_2(batchTempRow, Color.White);

                // check programming is successful.
                // if succesfully finished.
                if (((batchTempRow.IndexOf("pause", StringComparison.OrdinalIgnoreCase) >= 0) || (batchTempRow.IndexOf(batchFileFeedback1[0], StringComparison.OrdinalIgnoreCase) >= 0)))  // color ae
                {
                    mainFrm.otherConsoleNewLine_2();
                    mainFrm.otherConsoleAppendLine_2("Harvest Fresh Kartı:" + " programing İşlemi Başarıyla Tamamlanmıştır!", Color.Green);
                  //  this.mainFrm.lblTestResF1.Text = "Programlama Başarılı";
                   programing1Sonuc = true;
                    break;
                }
                else if (((batchTempRow.IndexOf("pause", StringComparison.OrdinalIgnoreCase) >= 0) || (batchTempRow.IndexOf(batchFileFeedback1[1], StringComparison.OrdinalIgnoreCase) >= 0)))  // color ae
                {
                    mainFrm.otherConsoleNewLine_2();
                    mainFrm.otherConsoleAppendLine_2("Harvest Fresh Kartı:" + " programing İşlemi Başarıyla Tamamlanmıştır!", Color.Green);
                  //  this.mainFrm.lblTestResF1.Text = "Programlama Başarılı";
                    programing1Sonuc = true;
                    break;
                }
                else if (((batchTempRow.IndexOf("pause", StringComparison.OrdinalIgnoreCase) >= 0) || (batchTempRow.IndexOf(batchFileFeedback1[2], StringComparison.OrdinalIgnoreCase) >= 0))) //Could not start CPU core.
                {
                    mainFrm.otherConsoleNewLine_2();
                    mainFrm.otherConsoleAppendLine_2("Harvest Fresh Kartı:" + " programing İşlemi Başarısız1.", Color.Red);  // programing Soketi Düzgün Takılı Değil!
                 //   this.mainFrm.lblTestResF1.Text = "Programlama Başarısız 1";
                    programing1Sonuc = false;
                    break;
                }
                else if (((batchTempRow.IndexOf("pause", StringComparison.OrdinalIgnoreCase) >= 0) || (batchTempRow.IndexOf(batchFileFeedback1[3], StringComparison.OrdinalIgnoreCase) >= 0)))  // Cannot connect to target.
                {
                    mainFrm.otherConsoleNewLine_2();
                    mainFrm.otherConsoleAppendLine_2("Harvest Fresh Kartı:" + " programing İşlemi Başarısız2.", Color.Red); // programing Soketi Takılı Değil!
                 //   this.mainFrm.lblTestResF1.Text = "Programlama Başarısız 2";
                    programing1Sonuc = false;
                    break;
                }
                else if (((batchTempRow.IndexOf("pause", StringComparison.OrdinalIgnoreCase) >= 0) || (batchTempRow.IndexOf(batchFileFeedback1[4], StringComparison.OrdinalIgnoreCase) >= 0))) //FAILED
                {
                    mainFrm.otherConsoleNewLine_2();
                    mainFrm.otherConsoleAppendLine_2("Harvest Fresh Kartı" + " programing İşlemi Başarısız3.", Color.Red);  //  USB Takılı Değil!
                    //this.mainFrm.lblTestResF1.Text = "Programlama Başarısız 3";
                    programing1Sonuc = false;
                    break;
                }
            }

            // if batch didn't closed kill it.
            if (!processBatchPrograming1.HasExited)
            {
                processBatchPrograming1.Kill();
            }
            ExecuteCommand(mainFrm.desktopPath + @"/Test_Application/Exit.bat");
            program1SonucFunction();
        }

        public void program1SonucFunction()
        {
            if (programing1Sonuc)
            {
                this.mainFrm.lblSonucF1.Invoke(new Action(delegate ()
                {
                    this.mainFrm.lblSonucF1.BackColor = Color.Green;
                    this.mainFrm.lblSonucF1.Text = "1.PROGRAMLAMA BAŞARILI";
                }));
                this.mainFrm.lblSonuc1.Invoke(new Action(delegate ()
                {
                    this.mainFrm.lblSonuc1.Visible = true;
                    this.mainFrm.lblSonuc1.BackColor = Color.Green;
                    this.mainFrm.lblSonuc1.Text = "1.PROGRAMLAMA BAŞARILI";
                }));
                programing1Success();

            }
            else
            {
                this.mainFrm.lblSonucF1.Invoke(new Action(delegate ()
                {
                    this.mainFrm.lblSonucF1.BackColor = Color.Red;
                    this.mainFrm.lblSonucF1.Text = "1.PROGRAMLAMA BAŞARISIZ";
                }));
                this.mainFrm.lblSonuc1.Invoke(new Action(delegate ()
                {
                    this.mainFrm.lblSonuc1.Visible = true;
                    this.mainFrm.lblSonuc1.BackColor = Color.Red;
                    this.mainFrm.lblSonuc1.Text = "1.PROGRAMLAMA BAŞARISIZ";
                }));

                programing1Error();
            }
            programing1Restart();
        }

        public void programing1Restart()  //PROGRAMLAMA RESTART
        {
            mainFrm.programing1TimeoutState = false;
        }

        public void programing1Success()  //PROGRAMLAMA BAŞARILI
        {
            try
            {
                if (mainFrm.nxCompoletBoolWrite("f1ProgramSuccess", true))   //???
                {
                    programing1Restart();
                }
              /*  else
                {
                    programing1Error();
                }*/
            }
            catch (Exception ex)
            {
                programing1Error();
                mainFrm.otherConsoleAppendLine_2("f1PogramSuccess: " + ex.Message, Color.Red);
            }
        }

        public void programing1Error()  //PROGRAMLAMA BAŞARISIZ
        {
            programing1ErrorTh = new Thread(programing1ErrorFunction);
            programing1ErrorTh.Start();
        }

        private void programing1ErrorFunction()  //PROGRAMLAMA BAŞARISIZ  //İlgili Programı Kapatmak İçin???
        {
            try
            {
                mainFrm.nxCompoletBoolWrite("f1ProgramFail", true);
                mainFrm.otherConsoleAppendLine_2("program1Fail Gönderildi: ", Color.Red);
                programing1Restart();
            }
            catch (Exception ex)
            {
                mainFrm.otherConsoleAppendLine_2("programing1Error: " + ex.Message, Color.Red);
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
