using AForge.Video;
using AForge.Video.DirectShow;
using OtoLehimlemeVeProgramlamaMakinesi.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OtoLehimlemeVeProgramlamaMakinesi
{
    public partial class Main : DevExpress.XtraEditors.XtraForm
    {
        Programming1 Programming1Frm;
        Programming2 Programming2Frm;
        FormMessageF1 formMessage;
        FormMessageF2 formMessage2;

        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        DataGridViewComboBoxEditingControl cbec = null;
        DataGridViewComboBoxEditingControl cbecF2 = null;

        List<string> listLehim = new List<string>();
        List<KeyValuePair<string, bool>> listProcessBool = new List<KeyValuePair<string, bool>>();
        List<KeyValuePair<string, string>> listProcessString = new List<KeyValuePair<string, string>>();
        List<List<Button[,]>> mainList, mainList2, mainList3, mainList4;
        List<Button[,]> childButtonList;
        List<Button[,]> mainButtonList;

        Button[,] buttonArray, buttonArrayF2;
        Panel[] mainPanelArray, mainPanelArrayF2;
        Panel[,] panelArray, panelArrayF2;

        INIKaydet iniLoadF1; INIKaydet iniLoadF2;

        Thread threadLoopRead, threadLoopPicBox, threadLoopBtn, threadPlcConn, threadLoopProgramming, threadLoopLehim, threadWriteBool, threadWriteString, threadProcess, threadVideo;


        string[] f1PanelStatus = new string[400], f2PanelStatus = new string[400], f1ProgramStatus = new string[500], f2ProgramStatus = new string[500], axisPos = new string[5];

        public string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        string[] hardwareStatus ={ "emergencyStop", "f1StartButton","f2StartButton" ,"havyaPistonUp", "havyaPistonDown", "lehimPistonUp", "lehimPistonDown",
            "p1PistonUp", "p1PistonDown", "p2PistonUp", "p2PistonDown", "f1Bariyer1", "f1Bariyer2", "f2Bariyer1", "f2Bariyer2","solKapi","arkaKapi","sagKapi"};
        string[] havyaDeegreArray = new string[256];

        public string programingPathF1, programingPathF2;
        string createFilePath, modelFilePath, noktasalF1, noktasalF2, pos1 = "", pos2 = "", pos3 = "", pos4 = "", pos5 = "", pos6 = "", prgPnlX = "", prgPnlY = "", prgPcbItem = "", prgPcbStatus = "",
               lhmPnlX = "", lhmPnlY = "", lhmPcbItem = "", dgvValueF1 = "", dgvValueF2 = "", lhmDegree1F1 = "100", lhmDegree1F2 = "100", s1, s2, s3,lastHavyaDegree="4";

        public int msgCounter = 0, msgCounter2 = 0;
        int[] maxMainTxtValue = { 1, 1, 1, 1 }, maxMainTxtValueF2 = { 1, 1, 1, 1 };
        int txt_Row, txt_RowF2, txt_Column, txt_ColumnF2, txt_RowP, txt_ColumnP, txt_RowPF2, txt_ColumnPF2, colUserF1, rowUserF1, colUserF2, rowUserF2, matrisCounter = 0, matrisCounterF2 = 0,
            matrisCounterP = 0, matrisCounterPF2 = 0, pos = 0, modelSayisi = 1, readLoopButtonCounter = 0, readLoopProgramingCounter = 0, loopPrograming1Counter = 0, loopPrograming2Counter = 0,
            readLoopLehimCounter = 0, havyaOkunanDeger = 0, havyaSabitDeger = 0, copyPos = 0, counter = 0, plcConnCounter = 0, errorCounterPrg = 0, errorCounterLhm = 0, mainPosF1 = 1, mainPosF2 = 1,
            counterByte = 0, readLoopCounter = 0, readLoopCounter2 = 0,havyaCounter=0;

        public bool programing1TimeoutState, programing2TimeoutState, automodFlag, bakimModFlag;
        bool flagFikstür, f1ProgramOk, f1ProgramStart, f1PcbStatusRead, f1PcbProcess, f1ProgramReset, f2ProgramOk, f2ProgramStart, f2PcbStatusRead, f2ProgramReset, f2PcbProcess, boolReadStatus,
             f1LehimRead, f2LehimRead, emptyControlFlag, videoFlag, havyaOn, havyaOnFlag, flagHavyaOpen, flagHavyaStatus, flagHavyaReceiveData, startFlag = true, startFlag2 = true, cameraOpen;

        double[] minF1 = { 0, 0, 12, 320, -180, 60, 0, 0, 0, 1, 0, 12, 320, 1, 0 };
        double[] maxF1 = { 0, 0, 365, 890, 180, 150, 20, 20, 500, 100, 60, 365, 890, 100, 100 };
        double[] minF2 = { 0, 0, 560, 320, -180, 60, 0, 0, 0, 1, 0, 560, 320, 1, 0 };
        double[] maxF2 = { 0, 0, 900, 890, 180, 150, 20, 20, 500, 100, 60, 900, 890, 100, 100 };

        byte[] array = new byte[512];
        byte[] byteArray = new byte[10];
        byte[] byteArray2 = new byte[10];



        public Main()
        {
            string process = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcesses().Count(p => p.ProcessName == process) > 1)
            {
                MessageBox.Show("Uygulama zaten çalışıyor");
                System.Environment.Exit(1);
            }
            splasScreen();
            InitializeComponent();
            Programming1Frm = new Programming1(this);
            Programming2Frm = new Programming2(this);
        }
        private void Main_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            initParameters();

        }
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (videoCaptureDevice.IsRunning == true)
                {
                    threadVideo.Abort();
                    videoCaptureDevice.Stop();
                }


                havyaOn = false;
                threadWriteBool = new Thread(() => nxCompoletBoolWrite("connectionStart", false));
                threadWriteBool.Start();
                threadLoopRead.Abort();

                if (serialPort1.IsOpen)
                {

                    byteArray[0] = 79;
                    byteArray[1] = 13;
                    flagHavyaReceiveData = true;
                    serialPort1.Write(byteArray, 0, 2);
                    flagHavyaReceiveData = false;
                    serialPort1.Close();
                }

                System.Environment.Exit(1);
            }
            catch (Exception)
            {

                throw;
            }


        }
        private void initParameters()
        {
            sourceHayva();
            havyaOpen();
            cmbBxLhmAmountF1.SelectedIndex = 0;
            cmbBxLhmAmountF2.SelectedIndex = 0;
            programComboBoxData(cmbBxFileNameF1, cmbBxFileNameF2);
            numTxtSetToSpace(lhmTxtPCBXP2F1, lhmTxtPCBXP2F1, lhmTxtPCBYP2F1, lhmTxtPCBXP3F1, lhmTxtPCBYP3F1,
            lhmTxtPnlXP2F1, lhmTxtPnlYP2F1, lhmTxtPnlXP3F1, lhmTxtPnlYP3F1,
            lhmTxtPCBXP2F2, lhmTxtPCBXP2F2, lhmTxtPCBYP2F2, lhmTxtPCBXP3F2, lhmTxtPCBYP3F2,
            lhmTxtPnlXP2F2, lhmTxtPnlYP2F2, lhmTxtPnlXP3F2, lhmTxtPnlYP3F2,
            txtPrg1F1, txtPrg2F1, txtPrg3F1, txtPrg4F1, txtPrg5F1, txtPrg6F1, txtPrg9F1, txtPrg10F1, txtPrg11F1, txtPrg12F1,
            txtPrg1F2, txtPrg2F2, txtPrg3F2, txtPrg4F2, txtPrg5F2, txtPrg6F2, txtPrg9F2, txtPrg10F2, txtPrg11F2, txtPrg12F2);

            threadLoopRead = new Thread(() => loopMainRead(pctrBx1, pctrBx2, pctrBx3, pctrBx4, pctrBx5, pctrBx6, pctrBx7, pctrBx8, pctrBx9, pctrBx10,
                                                               pctrBx11, pctrBx12, pctrBx13, pctrBx14, pctrBx15, pctrBx16, pctrBx17, pctrBx18));
            threadVideo = new Thread(stopVideo);

            panelAdd(panelLeh1);
            panelAddF2(panelLeh2);
            objectAdd(mainPanelArray[0], lhmTxtPnlXAdtF1, lhmTxtPnlYAdtF1, lhmTxtPCBXAdtF1, lhmTxtPCBYAdtF1, (Int16)lhmTxtPnlXAdtF1.Value, 1);
            callObjectAdd(txtPrg13F1, txtPrg14F1, txtPrg7F1, txtPrg8F1);
            objectAddF2(mainPanelArrayF2[0], lhmTxtPnlXAdtF2, lhmTxtPnlYAdtF2, lhmTxtPCBXAdtF2, lhmTxtPCBYAdtF2, (Int16)lhmTxtPnlXAdtF2.Value, 3);
            callObjectAddF2(txtPrg13F2, txtPrg14F2, txtPrg7F2, txtPrg8F2);

            threadWriteBool = new Thread(() => nxCompoletBoolWrite("connectionStart", true));
            threadWriteBool.Start();
            Thread.Sleep(100);
            if (nxCompoletBoolRead("connectionOk") && boolReadStatus == false)
            {
                pctrBxPlcState.BackgroundImage = Resources.greenLight;
                threadLoopRead.Start();

            }
            else
            {
                pctrBxPlcState.BackgroundImage = Resources.redLight;
            }

            threadWriteString = new Thread(() => nxCompoletStringWrite("jogHiz", "20"));
            threadWriteString.Start();
            threadWriteBool = new Thread(() => nxCompoletBoolWrite("jogContinuous", true));
            threadWriteBool.Start();
            threadWriteString = new Thread(() => nxCompoletStringWrite("machineSpeed", "10"));
            threadWriteString.Start();
            Programming1Frm.programing1Init();
            Programming2Frm.programing2Init();
           // SourceCamera();
            ExecuteCommand(desktopPath + @"/Test_Application/Exit.bat");
        }

        /*******************************************LOOP METHODS*******************************************************************/
        public bool emptyControl(DataGridView dataGridView, DevExpress.XtraEditors.CheckButton checkBtnLhm, DevExpress.XtraEditors.CheckButton checkBtnPrg, params TextBox[] textBoxes)
        {
            emptyControlFlag = true;
            for (int i = 0; i < textBoxes.Length; i++)
            {
                if (checkBtnPrg.Checked == true && i < 11)
                {
                    if (textBoxes[i].Text == "")
                    {
                        emptyControlFlag = false;
                        break;
                    }
                }
                else if (checkBtnLhm.Checked == true && i > 10)
                {
                    if (textBoxes[i].Text == "")
                    {
                        emptyControlFlag = false;
                        break;
                    }
                }

            }
            if (checkBtnLhm.Checked == true && dataGridView.RowCount == 0)
            {
                emptyControlFlag = false;
            }
            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                for (int j = 0; j < dataGridView.ColumnCount; j++)
                {
                    if (checkBtnLhm.Checked == true)
                    {
                        if (j == 0 || j == 1 || j == 11 || j == 12 || j == 13)
                        {
                            continue;
                        }
                        else if (dataGridView.Rows[i].Cells[j].Value == "" || dataGridView.Rows[i].Cells[j].Value == null)
                        {
                            emptyControlFlag = false;
                            break;
                        }

                    }
                }
            }
            return emptyControlFlag;
        }
        private void loopMainRead(params PictureBox[] pictureBoxes)
        {
            while (true)
            {
                Thread.Sleep(100);
                if (readLoopCounter == 5)
                {
                    readLoopCounter = 0;
                    try
                    {
                        if (xtraTabControl1.SelectedTabPageIndex == 1 || xtraTabControl1.SelectedTabPageIndex == 2)
                        {
                            pos1 = nxCompoletDoubleRead("axisPos[" + 0 + "]");
                            pos2 = nxCompoletDoubleRead2("axisPos[" + 1 + "]");
                            pos3 = nxCompoletDoubleRead3("axisPos[" + 2 + "]");
                            pos4 = nxCompoletDoubleRead4("axisPos[" + 3 + "]");
                            pos5 = nxCompoletDoubleRead5("axisPos[" + 4 + "]");
                            pos6 = nxCompoletDoubleRead6("axisPos[" + 5 + "]");
                            if (pos1.Length > 7)
                            {
                                pos1 = pos1.Substring(0, 7);
                                this.lblXF1.Invoke(new Action(delegate ()
                                {
                                    lblXF1.Text = pos1;
                                }));
                                this.lblXF2.Invoke(new Action(delegate ()
                                {
                                    lblXF2.Text = pos1;
                                }));
                                this.lblManX.Invoke(new Action(delegate ()
                                {
                                    lblManX.Text = pos1;
                                }));

                            }
                            else
                            {
                                this.lblXF1.Invoke(new Action(delegate ()
                                {
                                    lblXF1.Text = pos1;
                                }));
                                this.lblXF2.Invoke(new Action(delegate ()
                                {
                                    lblXF2.Text = pos1;
                                }));
                                this.lblManX.Invoke(new Action(delegate ()
                                {
                                    lblManX.Text = pos1;
                                }));
                            }

                            if (pos2.Length > 7)
                            {
                                pos2 = pos2.Substring(0, 7);
                                this.lblY1F1.Invoke(new Action(delegate ()
                                {
                                    lblY1F1.Text = pos2;
                                }));
                                this.lblManY1.Invoke(new Action(delegate ()
                                {
                                    lblManY1.Text = pos2;
                                }));
                            }
                            else
                            {
                                this.lblY1F1.Invoke(new Action(delegate ()
                                {
                                    lblY1F1.Text = pos2;
                                }));
                                this.lblManY1.Invoke(new Action(delegate ()
                                {
                                    lblManY1.Text = pos2;
                                }));
                            }
                            if (pos3.Length > 7)
                            {
                                pos3 = pos3.Substring(0, 7);
                                this.lblY2F2.Invoke(new Action(delegate ()
                                {
                                    lblY2F2.Text = pos3;
                                }));
                                this.lblManY2.Invoke(new Action(delegate ()
                                {
                                    lblManY2.Text = pos3; ;
                                }));
                            }
                            else
                            {
                                this.lblY2F2.Invoke(new Action(delegate ()
                                {
                                    lblY2F2.Text = pos3;
                                }));
                                this.lblManY2.Invoke(new Action(delegate ()
                                {
                                    lblManY2.Text = pos3;
                                }));
                            }

                            if (pos4.Length > 7)
                            {
                                pos4 = pos4.Substring(0, 7);
                                this.lblZF1.Invoke(new Action(delegate ()
                                {
                                    lblZF1.Text = pos4;
                                }));
                                this.lblZF2.Invoke(new Action(delegate ()
                                {
                                    lblZF2.Text = pos4;
                                }));
                                this.lblManZ.Invoke(new Action(delegate ()
                                {
                                    lblManZ.Text = pos4;
                                }));
                            }
                            else
                            {
                                this.lblZF1.Invoke(new Action(delegate ()
                                {
                                    lblZF1.Text = pos4;
                                }));
                                this.lblZF2.Invoke(new Action(delegate ()
                                {
                                    lblZF2.Text = pos4;
                                }));
                                this.lblManZ.Invoke(new Action(delegate ()
                                {
                                    lblManZ.Text = pos4;
                                }));
                            }

                            if (pos5.Length > 7)
                            {
                                pos5 = pos5.Substring(0, 7);
                                this.lblWF1.Invoke(new Action(delegate ()
                                {
                                    lblWF1.Text = pos5;
                                }));
                                this.lblWF2.Invoke(new Action(delegate ()
                                {
                                    lblWF2.Text = pos5;
                                }));
                                this.lblManW.Invoke(new Action(delegate ()
                                {
                                    lblManW.Text = pos5;
                                }));
                            }
                            else
                            {
                                this.lblWF1.Invoke(new Action(delegate ()
                                {
                                    lblWF1.Text = pos5;
                                }));
                                this.lblWF2.Invoke(new Action(delegate ()
                                {
                                    lblWF2.Text = pos5;
                                }));
                                this.lblManW.Invoke(new Action(delegate ()
                                {
                                    lblManW.Text = pos5;
                                }));

                            }

                            if (pos6.Length > 7)
                            {
                                pos6 = pos6.Substring(0, 7);
                                this.lblPrgF1.Invoke(new Action(delegate ()
                                {
                                    lblPrgF1.Text = pos6;
                                }));
                                this.lblPrgF2.Invoke(new Action(delegate ()
                                {
                                    lblPrgF2.Text = pos6;
                                }));
                                this.lblManPrg.Invoke(new Action(delegate ()
                                {
                                    lblManPrg.Text = pos6;
                                }));
                            }
                            else
                            {
                                this.lblPrgF1.Invoke(new Action(delegate ()
                                {
                                    lblPrgF1.Text = pos6;
                                }));
                                this.lblPrgF2.Invoke(new Action(delegate ()
                                {
                                    lblPrgF2.Text = pos6;
                                }));
                                this.lblManPrg.Invoke(new Action(delegate ()
                                {
                                    lblManPrg.Text = pos6;
                                }));
                            }
                        }
                        /* Thread 2*/

                        if (xtraTabControl1.SelectedTabPageIndex == 2)
                        {

                            for (int i = 0; i < pictureBoxes.Length; i++)
                            {
                                if (nxCompoletBoolRead(hardwareStatus[i]) && !boolReadStatus)
                                {

                                    pictureBoxes[i].Invoke(new Action(delegate ()
                                    {
                                        pictureBoxes[i].BackgroundImage = Resources.greenLight;
                                    }));
                                }
                                else
                                {
                                    pictureBoxes[i].Invoke(new Action(delegate ()
                                    {
                                        pictureBoxes[i].BackgroundImage = Resources.redLight;
                                    }));

                                }

                            }
                        }

                        /********************Thread 3*********************/
                        if (!nxCompoletBoolRead("bakimMod") && !boolReadStatus && !bakimModFlag)
                        {

                            this.btnBkmF1.Invoke(new Action(delegate ()
                            {
                                btnBkmF1.Text = "Bakım Mod Pasif";
                                btnBkmF1.BackColor = Color.Red;
                            }));
                            this.btnBkmF2.Invoke(new Action(delegate ()
                            {
                                btnBkmF2.Text = "Bakım Mod Pasif";
                                btnBkmF2.BackColor = Color.Red;
                            }));
                            this.btnManBkm.Invoke(new Action(delegate ()
                            {
                                btnManBkm.Text = "Bakım Mod Pasif";
                                btnManBkm.BackColor = Color.Red;
                            }));

                            this.btnUsbF1.Invoke(new Action(delegate ()
                            {
                                btnUsbF1.Text = "USB AÇ";
                                btnUsbF1.BackColor = Color.Green;
                            }));
                            this.btnUsbF2.Invoke(new Action(delegate ()
                            {
                                btnUsbF2.Text = "USB AÇ";
                                btnUsbF2.BackColor = Color.Green;
                            }));
                            nxCompoletBoolWrite("f1Usb", false);
                            nxCompoletBoolWrite("f2Usb", false);
                        }

                        if (nxCompoletBoolRead("homeProcessing") && boolReadStatus == false)
                        {
                            this.btnHomeF1.Invoke(new Action(delegate ()
                            {
                                btnHomeF1.Text = "Home Yapılıyor...";
                                btnHomeF1.BackColor = Color.Yellow;
                                btnHomeF1.ForeColor = Color.Black;
                            }));
                            this.btnHomeF2.Invoke(new Action(delegate ()
                            {
                                btnHomeF2.Text = "Home Yapılıyor...";
                                btnHomeF2.BackColor = Color.Yellow;
                                btnHomeF2.ForeColor = Color.Black;
                            }));
                            this.btnManHome.Invoke(new Action(delegate ()
                            {
                                btnManHome.Text = "Home Yapılıyor...";
                                btnManHome.BackColor = Color.Yellow;
                                btnManHome.ForeColor = Color.Black;
                            }));
                            this.btnHome.Invoke(new Action(delegate ()
                            {
                                btnHome.Text = "Home Yapılıyor...";
                                btnHome.BackColor = Color.Yellow;
                                btnHome.ForeColor = Color.Black;
                            }));

                        }
                        else if (nxCompoletBoolRead("homeOk") && boolReadStatus == false)
                        {

                            this.btnHomeF1.Invoke(new Action(delegate ()
                            {
                                btnHomeF1.Text = "HOME OK";
                                btnHomeF1.BackColor = Color.Green;
                            }));
                            this.btnHomeF2.Invoke(new Action(delegate ()
                            {
                                btnHomeF2.Text = "HOME OK";
                                btnHomeF2.BackColor = Color.Green;
                            }));
                            this.btnManHome.Invoke(new Action(delegate ()
                            {
                                btnManHome.Text = "HOME OK";
                                btnManHome.BackColor = Color.Green;
                            }));
                            this.btnHome.Invoke(new Action(delegate ()
                            {
                                btnHome.Text = "HOME OK";
                                btnHome.BackColor = Color.Green;
                            }));

                        }
                        else if (boolReadStatus == false)
                        {

                            this.btnHomeF1.Invoke(new Action(delegate ()
                            {
                                btnHomeF1.Text = "HOME";
                                btnHomeF1.BackColor = Color.Orange;
                            }));
                            this.btnHomeF2.Invoke(new Action(delegate ()
                            {
                                btnHomeF2.Text = "HOME";
                                btnHomeF2.BackColor = Color.Orange;
                            }));
                            this.btnManHome.Invoke(new Action(delegate ()
                            {
                                btnManHome.Text = "HOME";
                                btnManHome.BackColor = Color.Orange;
                            }));
                            this.btnHome.Invoke(new Action(delegate ()
                            {
                                btnHome.Text = "HOME";
                                btnHome.BackColor = Color.Orange;
                            }));
                        }

                        if (!automodFlag)
                        {
                            if (nxCompoletBoolRead("autoMod") == false && boolReadStatus == false)
                            {
                                this.btnOto.Invoke(new Action(delegate ()
                                {
                                    btnOto.Text = "OTOMATİK PASİF";
                                    btnOto.BackColor = Color.Red;
                                    btnOto.ForeColor = Color.White;
                                }));
                            }
                            else if (nxCompoletBoolRead("autoMod") == true && boolReadStatus == false)
                            {
                                this.btnOto.Invoke(new Action(delegate ()
                                {
                                    btnOto.Text = "OTOMATİK AKTİF";
                                    btnOto.BackColor = Color.Green;
                                    btnOto.ForeColor = Color.White;
                                }));
                            }
                        }
                        ///////////////////////Thread 4 //////////////////////////

                        f1ProgramOk = nxCompoletBoolRead("f1ProgramOk");
                        f1PcbProcess = nxCompoletBoolRead("f1pcbProcess");

                        if (f1ProgramOk && boolReadStatus == false)
                        {
                            f1ProgramStart = nxCompoletBoolRead("f1ProgramStart");
                            if (f1ProgramStart && boolReadStatus == false)  //PLC-ARAYÜZ-programing
                            {
                                if (nxCompoletBoolWrite("f1ProgramStart", false))
                                {
                                    loopPrograming1Counter = 0;
                                    programing1TimeoutState = true;
                                    nxCompoletBoolWrite("f1ProgramSuccess", false);
                                    nxCompoletBoolWrite("f1ProgramFail", false);
                                    Programming1Frm.programming1Start(); ;
                                }
                            }

                            f1PcbStatusRead = nxCompoletBoolRead("f1PcbStatusRead");
                            if (f1PcbStatusRead && boolReadStatus == false)
                            {
                                if (callPcbValuesPrg("f1XProgramPanel", "f1YProgramPanel", "f1ProgramStatus", "f1ProgramResult") == false)
                                {
                                    sonucIsimBulF1Prg();
                                }
                            }
                            if (f1PcbProcess && boolReadStatus == false)
                            {

                                btnLhmWork.Invoke(new Action(delegate ()
                                {
                                    if (msgCounter == 0)
                                    {
                                        formMessage = new FormMessageF1(this);
                                        formMessage.Show();
                                        msgCounter = 1;
                                    }

                                }));

                            }
                        }
                        else if (!f1PcbProcess && !boolReadStatus)
                        {
                            btnLhmWork.Invoke(new Action(delegate ()
                            {
                                if (msgCounter == 1)
                                {
                                    formMessage.Close();
                                    msgCounter = 0;
                                }

                            }));
                        }

                        f1ProgramReset = nxCompoletBoolRead("f1StatusReset");
                        if (f1ProgramReset && boolReadStatus == false)
                        {
                            if (checkBtnSolderF1.Checked == true)
                            {
                                pcbStatusReset(lhmTxtPnlYAdtF1, lhmTxtPnlXAdtF1, lhmTxtPCBYAdtF1, lhmTxtPCBXAdtF1, mainList);
                            }
                            if (checkBtnProgF1.Checked == true)
                            {
                                pcbStatusReset(txtPrg14F1, txtPrg13F1, txtPrg8F1, txtPrg7F1, mainList2);
                            }

                            Thread.Sleep(100);
                            nxCompoletBoolWrite("f1StatusReset", false);
                        }

                        if (nxCompoletBoolRead("f1VeriOku") && boolReadStatus == false)
                        {
                            Thread.Sleep(100);
                            this.lblM1F1.Invoke(new Action(delegate ()
                            {
                                lblM1F1.Text = nxCompoletStringRead9("f1UretilenAdet");
                            }));
                            this.lblM2F1.Invoke(new Action(delegate ()
                            {
                                lblM2F1.Text = nxCompoletStringRead10("f1LehimSure");
                            }));
                            this.lblM3F1.Invoke(new Action(delegate ()
                            {
                                lblM3F1.Text = nxCompoletStringRead11("f1ProgramSure");
                            }));
                            this.lblM4F1.Invoke(new Action(delegate ()
                            {
                                lblM4F1.Text = nxCompoletStringRead12("f1TotalSure");
                            }));
                            nxCompoletBoolWrite("f1VeriOku", false);
                        }


                        /***********f2 programming***************/

                        f2ProgramOk = nxCompoletBoolRead("f2ProgramOk");

                        f2PcbProcess = nxCompoletBoolRead("f2pcbProcess");
                        if (f2ProgramOk && boolReadStatus == false)
                        {
                            f2ProgramStart = nxCompoletBoolRead("f2ProgramStart");
                            if (f2ProgramStart && boolReadStatus == false)  //PLC-ARAYÜZ-programing
                            {
                                if (nxCompoletBoolWrite("f2ProgramStart", false))
                                {
                                    loopPrograming2Counter = 0;
                                    programing2TimeoutState = true;
                                    nxCompoletBoolWrite("f2ProgramSuccess", false);
                                    nxCompoletBoolWrite("f2ProgramFail", false);
                                    Programming2Frm.programming2Start();
                                }
                            }


                            f2PcbStatusRead = nxCompoletBoolRead("f2PcbStatusRead");
                            if (f2PcbStatusRead && boolReadStatus == false)
                            {
                                if (callPcbValuesPrg("f2XProgramPanel", "f2YProgramPanel", "f2ProgramStatus", "f2ProgramResult") == false)
                                {
                                    sonucIsimBulF2Prg();
                                }
                            }

                            if (f2PcbProcess && boolReadStatus == false)
                            {

                                btnLhmWork2.Invoke(new Action(delegate ()
                                {
                                    if (msgCounter2 == 0)
                                    {
                                        formMessage2 = new FormMessageF2(this);
                                        formMessage2.Show();
                                        msgCounter2 = 1;
                                    }
                                }));

                            }
                        }
                        else if (!f2PcbProcess && !boolReadStatus)
                        {
                            btnLhmWork2.Invoke(new Action(delegate ()
                            {
                                if (msgCounter2 == 1)
                                {
                                    formMessage2.Close();
                                    msgCounter2 = 0;
                                }
                            }));
                        }

                        f2ProgramReset = nxCompoletBoolRead("f2StatusReset");
                        if (f2ProgramReset && boolReadStatus == false)
                        {
                            if (checkBtnSolderF2.Checked == true)
                            {
                                pcbStatusReset(lhmTxtPnlYAdtF2, lhmTxtPnlXAdtF2, lhmTxtPCBYAdtF2, lhmTxtPCBXAdtF2, mainList3);
                            }
                            if (checkBtnProgF2.Checked == true)
                            {
                                pcbStatusReset(txtPrg14F2, txtPrg13F2, txtPrg8F2, txtPrg7F2, mainList4);
                            }

                            Thread.Sleep(100);
                            nxCompoletBoolWrite("f2StatusReset", false);
                        }
                        if (nxCompoletBoolRead("f2VeriOku") && boolReadStatus == false)
                        {
                            Thread.Sleep(100);
                            this.lblM1F2.Invoke(new Action(delegate ()
                            {
                                lblM1F2.Text = nxCompoletStringRead13("f2UretilenAdet");
                            }));
                            this.lblM2F2.Invoke(new Action(delegate ()
                            {
                                lblM2F2.Text = nxCompoletStringRead14("f2LehimSure");
                            }));
                            this.lblM3F2.Invoke(new Action(delegate ()
                            {
                                lblM3F2.Text = nxCompoletStringRead15("f2ProgramSure");
                            }));
                            this.lblM4F2.Invoke(new Action(delegate ()
                            {
                                lblM4F2.Text = nxCompoletStringRead16("f2TotalSure");
                            }));

                            nxCompoletBoolWrite("f2VeriOku", false);
                        }


                        /***************************Thread5************************************/

                        f1LehimRead = nxCompoletBoolRead("f1LehimStatusRead");
                        if (f1LehimRead && boolReadStatus == false)  //PLC-ARAYÜZ-programing
                        {
                            if (callPcbValuesLhm("f1XlehimPanel", "f1YlehimPanel", "f1LehimStatus") == false)
                            {
                                sonucIsimBulF1Lhm();
                            }
                        }

                        f2LehimRead = nxCompoletBoolRead("f2LehimStatusRead");
                        if (f2LehimRead && boolReadStatus == false)  //PLC-ARAYÜZ-programing
                        {
                            if (callPcbValuesLhm("f2XlehimPanel", "f2YlehimPanel", "f2LehimStatus") == false)
                            {
                                sonucIsimBulF2Lhm();
                            }
                        }

                        /**********************************Thread 6 */////////////////////////////////////////////////////////////
                        if (nxCompoletBoolRead("f1Start"))
                        {
                            if (nxCompoletBoolRead("f1ReceteOk") && boolReadStatus == false)
                            {
                                this.btnRecipe.Invoke(new Action(delegate ()
                                {
                                    this.btnRecipe.Text = "Reçete Seçili";
                                    this.btnRecipe.BackColor = Color.Green;
                                }));
                            }
                            else if (!nxCompoletBoolRead("f1ReceteOk") && boolReadStatus == false)
                            {
                                this.btnRecipe.Invoke(new Action(delegate ()
                                {
                                    this.btnRecipe.Text = "Reçete Seçilmedi";
                                    this.btnRecipe.BackColor = Color.Red;
                                }));
                            }

                            if (nxCompoletBoolRead("f1SensorOk") && boolReadStatus == false)
                            {
                                this.btnSensor.Invoke(new Action(delegate ()
                                {
                                    this.btnSensor.Text = "Sensör Aktif";
                                    this.btnSensor.BackColor = Color.Green;
                                }));
                            }
                            else if (!nxCompoletBoolRead("f1SensorOk") && boolReadStatus == false)
                            {
                                this.btnSensor.Invoke(new Action(delegate ()
                                {
                                    this.btnSensor.Text = "Sensör Pasif";
                                    this.btnSensor.BackColor = Color.Red;
                                }));
                            }
                        }

                        if (nxCompoletBoolRead("f2Start"))
                        {
                            if (nxCompoletBoolRead("f2ReceteOk") && boolReadStatus == false)
                            {
                                this.btnRecipeF2.Invoke(new Action(delegate ()
                                {
                                    this.btnRecipeF2.Text = "Reçete Seçili";
                                    this.btnRecipeF2.BackColor = Color.Green;
                                }));
                            }
                            else if (!nxCompoletBoolRead("f2ReceteOk") && boolReadStatus == false)
                            {
                                this.btnRecipeF2.Invoke(new Action(delegate ()
                                {
                                    this.btnRecipeF2.Text = "Reçete Seçilmedi";
                                    this.btnRecipeF2.BackColor = Color.Red;
                                }));
                            }

                            if (nxCompoletBoolRead("f2SensorOk") && boolReadStatus == false)
                            {
                                this.btnSensorF2.Invoke(new Action(delegate ()
                                {
                                    this.btnSensorF2.Text = "Sensör Aktif";
                                    this.btnSensorF2.BackColor = Color.Green;
                                }));
                            }
                            else if (!nxCompoletBoolRead("f2SensorOk") && boolReadStatus == false)
                            {
                                this.btnSensorF2.Invoke(new Action(delegate ()
                                {
                                    this.btnSensorF2.Text = "Sensör Pasif";
                                    this.btnSensorF2.BackColor = Color.Red;
                                }));
                            }
                        }

                        if (nxCompoletBoolRead("f1StartButonOk") && boolReadStatus == false)
                        {
                            this.btnStartF1.Invoke(new Action(delegate ()
                            {
                                this.btnStartF1.BackColor = Color.LimeGreen;
                            }));
                        }
                        else if (!nxCompoletBoolRead("f1StartButonOk") && boolReadStatus == false)
                        {
                            this.btnStartF1.Invoke(new Action(delegate ()
                            {
                                this.btnStartF1.BackColor = Color.DarkGray;
                            }));
                        }

                        if (nxCompoletBoolRead("f2StartButonOk") && boolReadStatus == false)
                        {
                            this.btnStartF2.Invoke(new Action(delegate ()
                            {
                                this.btnStartF2.BackColor = Color.LimeGreen;
                            }));
                        }
                        else if (!nxCompoletBoolRead("f2StartButonOk") && boolReadStatus == false)
                        {
                            this.btnStartF2.Invoke(new Action(delegate ()
                            {
                                this.btnStartF2.BackColor = Color.DarkGray;
                            }));
                        }


                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                readLoopCounter++;
                if (readLoopCounter2 == 10)
                {
                    readLoopCounter2 = 0;
                    try
                    {
                        havyaOnFlag = nxCompoletBoolRead("havyaOn");
                        if (havyaOnFlag && boolReadStatus == false)
                        {
                            if (!flagHavyaOpen)
                            {
                                // havyaOpen();
                                flagHavyaOpen = true;
                            }

                            if (serialPort1.IsOpen && havyaOn)
                            {
                                try
                                {
                                    byteArray[0] = 69;
                                    byteArray[1] = 13;
                                    flagHavyaReceiveData = true;
                                    serialPort1.Write(byteArray, 0, 2);
                                    flagHavyaReceiveData = false;
                                    havyaOn = false;
                                    Thread.Sleep(1000);
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show("Havya açma hatası : " + e.ToString());
                                }

                            }
                          
                            if (lastHavyaDegree == nxCompoletStringRead("havyaStatus"))
							{
								if (havyaCounter < 3)
								{
                                    sendHavyaData();
                                    havyaCounter++;
                                }
							}
							else
							{
                                lastHavyaDegree = nxCompoletStringRead("havyaStatus");
                                havyaCounter = 0;
                                sendHavyaData();
                            }
                   
                        }
                        if (!havyaOnFlag && boolReadStatus == false)
                        {
                            if (serialPort1.IsOpen && !havyaOn)
                            {
                                byteArray[0] = 79;
                                byteArray[1] = 13;
                                flagHavyaReceiveData = true;
                                serialPort1.Write(byteArray, 0, 2);
                                //   flagHavyaReceiveData = false;
                                havyaOn = true;
                            }
                            this.lblHavyaDeegre.Invoke(new Action(delegate ()
                            {
                                lblHavyaDeegre.Text = "0";
                            }));
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                readLoopCounter2++;
            }
        }
        private void sendHavyaData()
		{
            if (nxCompoletStringRead("havyaStatus") == "0" && boolReadStatus == false)
            {
                sendHavyaDegreeData("150");
            }
            else if (nxCompoletStringRead("havyaStatus") == "1" && boolReadStatus == false)
            {
                if (!nxCompoletBoolRead("havyaStatusOk") && boolReadStatus == false)
                {
                    Thread.Sleep(100);
                    sendHavyaDegreeData(lhmDegree1F1);
                    try
                    {
                        havyaSabitDeger = Int16.Parse(lhmDegree1F1);
                        havyaOkunanDeger = Int16.Parse(lblHavyaDeegre.Text);

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                    if (havyaSabitDeger <= havyaOkunanDeger + 20 && havyaSabitDeger >= havyaOkunanDeger - 20)
                    {
                        nxCompoletBoolWrite("havyaStatusOk", true);
                    }
                }

            }
            else if (nxCompoletStringRead("havyaStatus") == "2" && boolReadStatus == false)
            {
                if (!nxCompoletBoolRead("havyaStatusOk") && boolReadStatus == false)
                {
                    Thread.Sleep(100);
                    sendHavyaDegreeData(lhmDegree1F2);
                    try
                    {
                        havyaSabitDeger = Int16.Parse(lhmDegree1F2);
                        havyaOkunanDeger = Int16.Parse(lblHavyaDeegre.Text);

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                    if (havyaSabitDeger <= havyaOkunanDeger + 20 && havyaSabitDeger >= havyaOkunanDeger - 20)
                    {
                        nxCompoletBoolWrite("havyaStatusOk", true);
                    }
                }
            }
            else if (nxCompoletStringRead("havyaStatus") == "3" && boolReadStatus == false)
            {
                sendHavyaDegreeData("385");
                Thread.Sleep(500);
                try
                {
                    havyaSabitDeger = 385;
                    havyaOkunanDeger = Int16.Parse(lblHavyaDeegre.Text);

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                if (havyaSabitDeger <= havyaOkunanDeger + 20 && havyaSabitDeger >= havyaOkunanDeger - 20)
                {
                    nxCompoletBoolWrite("havyaStatusOk", true);
                }
            }
        }
        private bool callPcbValuesLhm(params string[] strings)
        {
            lhmPnlX = nxCompoletStringRead17(strings[0]);
            lhmPnlY = nxCompoletStringRead18(strings[1]);
            lhmPcbItem = nxCompoletStringRead19(strings[2]);
            if (errorCounterLhm == 3)
            {
                errorCounterLhm = 0;
                return false;
            }
            if (lhmPnlX == "error" && lhmPnlY == "error" && lhmPcbItem == "error")
            {
                errorCounterLhm++;
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool callPcbValuesPrg(params string[] strings)
        {
            prgPnlX = nxCompoletStringRead20(strings[0]);
            prgPnlY = nxCompoletStringRead21(strings[1]);
            prgPcbItem = nxCompoletStringRead22(strings[2]);
            prgPcbStatus = nxCompoletStringRead23(strings[3]);
            if (errorCounterPrg == 3)
            {
                errorCounterPrg = 0;
                return false;
            }
            else if (prgPnlX == "error" && prgPnlY == "error" && prgPcbItem == "error" && prgPcbStatus == "error")
            {
                errorCounterPrg++;
                return true;
            }
            else
            {
                errorCounterPrg = 0;
                return false;
            }
        }

        private void sonucIsimBulF1Lhm()
        {
            int x = int.Parse(lhmPnlX);
            int y = int.Parse(lhmPnlY);
            findPcbIndexLhm(lhmPcbItem, x, y, lhmTxtPCBYAdtF1, lhmTxtPCBXAdtF1, mainList);
            Thread.Sleep(100);
            threadWriteBool = new Thread(() => nxCompoletStringWrite("f1LehimStatusRead", "false"));
            threadWriteBool.Start();
        }
        private void sonucIsimBulF2Lhm()
        {
            int x = int.Parse(lhmPnlX);
            int y = int.Parse(lhmPnlY);
            findPcbIndexLhm(lhmPcbItem, x, y, lhmTxtPCBYAdtF2, lhmTxtPCBXAdtF2, mainList3);
            Thread.Sleep(100);
            threadWriteBool = new Thread(() => nxCompoletStringWrite("f2LehimStatusRead", "false"));
            threadWriteBool.Start();
        }
        private void sonucIsimBulF1Prg()
        {
            int x = int.Parse(prgPnlX);
            int y = int.Parse(prgPnlY);
            findPcbIndex(prgPcbItem, x, y, txtPrg8F1, txtPrg7F1, prgPcbStatus, mainList2);
            Thread.Sleep(100);
            threadWriteBool = new Thread(() => nxCompoletStringWrite("f1PcbStatusRead", "false"));
            threadWriteBool.Start();
        }
        private void sonucIsimBulF2Prg()
        {
            int x = int.Parse(prgPnlX);
            int y = int.Parse(prgPnlY);
            findPcbIndex(prgPcbItem, x, y, txtPrg8F2, txtPrg7F2, prgPcbStatus, mainList4);
            Thread.Sleep(100);
            threadWriteBool = new Thread(() => nxCompoletStringWrite("f2PcbStatusRead", "false"));
            threadWriteBool.Start();
        }
        private void findPcbIndexLhm(string text, int pnlX, int pnlY, NumericUpDown numericUpDown0, NumericUpDown numericUpDown1, List<List<Button[,]>> list)
        {
            for (int i = 0; i < numericUpDown0.Value; i++)
            {
                for (int j = 0; j < numericUpDown1.Value; j++)
                {
                    if (text == list[pnlY][pnlX][i, j].Text)
                    {
                        list[pnlY][pnlX][i, j].Invoke(new Action(delegate ()
                        {
                            list[pnlY][pnlX][i, j].BackColor = Color.Green;
                        }));
                    }
                }
            }
        }
        private void findPcbIndex(string text, int pnlX, int pnlY, NumericUpDown numericUpDown0, NumericUpDown numericUpDown1, string pcbStatus, List<List<Button[,]>> list)
        {

            for (int i = 0; i < numericUpDown0.Value; i++)
            {
                for (int j = 0; j < numericUpDown1.Value; j++)
                {
                    if (text == list[pnlY][pnlX][i, j].Text)
                    {

                        if (pcbStatus == "0")
                        {
                            list[pnlY][pnlX][i, j].Invoke(new Action(delegate ()
                            {
                                list[pnlY][pnlX][i, j].BackColor = Color.Gray;
                            }));
                        }
                        else if (pcbStatus == "1")
                        {
                            list[pnlY][pnlX][i, j].Invoke(new Action(delegate ()
                            {
                                list[pnlY][pnlX][i, j].BackColor = Color.Green;
                            }));


                        }
                        else if (pcbStatus == "2")
                        {
                            list[pnlY][pnlX][i, j].Invoke(new Action(delegate ()
                            {
                                list[pnlY][pnlX][i, j].BackColor = Color.Red;
                            }));
                        }
                    }
                }
            }
        }
        private void pcbStatusReset(NumericUpDown numericUpDownY, NumericUpDown numericUpDownX, NumericUpDown numericUpDown0, NumericUpDown numericUpDown1, List<List<Button[,]>> list)
        {

            for (int x = 0; x < numericUpDownY.Value; x++)
            {
                for (int t = 0; t < numericUpDownX.Value; t++)
                {
                    for (int i = 0; i < numericUpDown0.Value; i++)
                    {
                        for (int j = 0; j < numericUpDown1.Value; j++)
                        {
                            list[x][t][i, j].Invoke(new Action(delegate ()
                            {
                                list[x][t][i, j].BackColor = Color.Gray;
                            }));
                        }
                    }

                }

            }

        }
        private void havyaOpen()
        {
            try
            {
                if (cmbBxHvy.Text != "" && cmbBxHvy.Items.Count > 0)
                {
                    serialPort1.PortName = cmbBxHvy.Text;
                    serialPort1.DtrEnable = true;
                    serialPort1.ReceivedBytesThreshold = 1;
                    serialPort1.Open();
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Havya bağlantısı sağlanamdı");
            }
        }

        /******************************************* Main Page Methods ************************************************************/
        private void splasScreen()
        {

            System.Threading.Thread thread_mail = new System.Threading.Thread(() => Application.Run(new SplashScreenForm()));
            thread_mail.Start();
            Random random = new Random();
            System.Threading.Thread.Sleep(random.Next(5000, 7000));
            thread_mail.Abort();
        }
        private void numTxtSetToSpace(params TextBox[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Text = "";
            }
        }
        private void addList(int RowX, Int16 panelNumber)
        {
            if (panelNumber == 1)
            {
                int constRow = RowX;
                mainButtonList = new List<Button[,]>();
                mainList = new List<List<Button[,]>>();
                for (int i = 0; i < childButtonList.Count; i++)
                {
                    if (i + 1 == RowX)
                    {
                        mainButtonList.Add(childButtonList[i]);
                        mainList.Add(mainButtonList);
                        mainButtonList = new List<Button[,]>();
                        RowX = RowX + constRow;
                    }
                    else
                    {
                        mainButtonList.Add(childButtonList[i]);
                    }
                }

            }
            else if (panelNumber == 2)
            {
                int constRow = RowX;
                mainButtonList = new List<Button[,]>();
                mainList2 = new List<List<Button[,]>>();
                for (int i = 0; i < childButtonList.Count; i++)
                {
                    if (i + 1 == RowX)
                    {
                        mainButtonList.Add(childButtonList[i]);
                        mainList2.Add(mainButtonList);
                        mainButtonList = new List<Button[,]>();
                        RowX = RowX + constRow;
                    }
                    else
                    {
                        mainButtonList.Add(childButtonList[i]);
                    }
                }
            }
            else if (panelNumber == 3)
            {
                int constRow = RowX;
                mainButtonList = new List<Button[,]>();
                mainList3 = new List<List<Button[,]>>();
                for (int i = 0; i < childButtonList.Count; i++)
                {
                    if (i + 1 == RowX)
                    {
                        mainButtonList.Add(childButtonList[i]);
                        mainList3.Add(mainButtonList);
                        mainButtonList = new List<Button[,]>();
                        RowX = RowX + constRow;
                    }
                    else
                    {
                        mainButtonList.Add(childButtonList[i]);
                    }
                }
            }
            else
            {
                int constRow = RowX;
                mainButtonList = new List<Button[,]>();
                mainList4 = new List<List<Button[,]>>();
                for (int i = 0; i < childButtonList.Count; i++)
                {
                    if (i + 1 == RowX)
                    {
                        mainButtonList.Add(childButtonList[i]);
                        mainList4.Add(mainButtonList);
                        mainButtonList = new List<Button[,]>();
                        RowX = RowX + constRow;
                    }
                    else
                    {
                        mainButtonList.Add(childButtonList[i]);
                    }
                }
            }


        }
        private void buttonAdd(Panel panel, NumericUpDown numericUpDownX, NumericUpDown numericUpDownY)
        {

            /*  pcbStatus[0] = "0";
              pcbStatus[1] = "1";
              pcbStatus[2] = "2";
              pcbStatus[3] = "0";
              pcbStatus[4] = "1";
              pcbStatus[5] = "2";
              pcbStatus[6] = "0";
              pcbStatus[7] = "1";
              pcbStatus[8] = "2";
              pcbStatus[9] = "0";
              pcbStatus[10] = "1";
              pcbStatus[11] = "2";*/

            try
            {
                panel.Controls.Clear();                //Paneli temizle
                matrisCounter = 0;                              //Sayacı sıfırla
                txt_Row = (Int16)numericUpDownX.Value;                     //Aldığımız Satır bilgisi.
                txt_Column = (Int16)numericUpDownY.Value;             //Aldığımız Sutün bilgisi

                int max_size_w = panel.Width;                              //Panelin Maksimum genişliği
                int max_size_h = panel.Height;                             //Panelin Maksimum yüksekliği

                int btn_size_h = max_size_h / txt_Row;             //Maksimum yüksekliğe satır sayısını girerek butonun boyutu belirlenir.
                int btn_size_w = max_size_w / txt_Column;         //Maksimum genişliğe sutun sayısını girerek butonun boyutu belirlenir.


                int loc_x;
                int loc_y;

                buttonArray = new Button[txt_Row, txt_Column];   //Dizi tanımlaması.
                if (txt_Row >= 1 && txt_Row < 20 && txt_Column >= 1 && txt_Column < 20)
                {
                    for (int row_index = 0; row_index < txt_Row; row_index++)
                    {
                        for (int coloumn_index = 0; coloumn_index < txt_Column; coloumn_index++)
                        {
                            matrisCounter++;

                            Button btn = new Button();
                            btn.Text = matrisCounter.ToString();                        //Buton text ismi
                            btn.Name = "btnF1" + matrisCounter.ToString();
                            btn.Enabled = false;                                //Butona tıklanmaması için.
                            btn.Size = new Size(btn_size_w, btn_size_h);        //Buton boyutu
                            btn.BackColor = Color.Gainsboro;
                            btn.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));

                            if (f1ProgramStatus[matrisCounter - 1] == "0")
                            {
                                btn.BackColor = Color.Blue;
                            }
                            else if (f1ProgramStatus[matrisCounter - 1] == "1")
                            {
                                btn.BackColor = Color.Green;
                            }
                            else if (f1ProgramStatus[matrisCounter - 1] == "2")
                            {
                                btn.BackColor = Color.Red;
                            }

                            loc_x = coloumn_index * btn_size_w;                 //Butonun X eksenindeki yeri
                            loc_y = row_index * btn_size_h;                     //Butonun Y eksenindeki yeri
                            btn.Location = new Point(loc_x, loc_y);

                            panel.Controls.Add(btn);                           //Panele ekle.
                            try
                            {
                                this.Invoke((MethodInvoker)delegate ()
                                {
                                    panel.Controls.Add(btn);                          //Panele ekle.
                                    buttonArray[row_index, coloumn_index] = btn;               //Diziye doldur.
                                });
                            }
                            catch (Exception)
                            {
                                //  otherConsoleAppendLine("Dizi Hatası: " + ex.Message, Color.Red);
                            }
                        }
                    }
                    childButtonList.Add(buttonArray);

                    //mainPanelList.Add(mainButtonList);

                }
                else
                {
                    /* textBox1.Text = "1";
                     textBox2.Text = "1";*/
                    MessageBox.Show("Satır ve Sütün Sayısını Lütfen 1-19 Arasında Giriniz.!");
                }
            }
            catch (Exception)
            {
                //otherConsoleAppendLine("matrisAdd: " + ex.Message, Color.Red);
            }
            //Dizi[i, j].BackColor = Color.Red;
        }
        private void buttonAddF2(Panel panel, NumericUpDown numericUpDownX, NumericUpDown numericUpDownY)
        {
            try
            {
                panel.Controls.Clear();                //Paneli temizle
                matrisCounterF2 = 0;                              //Sayacı sıfırla
                txt_RowF2 = (Int16)numericUpDownX.Value;                     //Aldığımız Satır bilgisi.
                txt_ColumnF2 = (Int16)numericUpDownY.Value;             //Aldığımız Sutün bilgisi

                int max_size_w = panel.Width;                              //Panelin Maksimum genişliği
                int max_size_h = panel.Height;                             //Panelin Maksimum yüksekliği

                int btn_size_h = max_size_h / txt_RowF2;             //Maksimum yüksekliğe satır sayısını girerek butonun boyutu belirlenir.
                int btn_size_w = max_size_w / txt_ColumnF2;         //Maksimum genişliğe sutun sayısını girerek butonun boyutu belirlenir.


                int loc_x;
                int loc_y;

                buttonArrayF2 = new Button[txt_RowF2, txt_ColumnF2];                                  //Dizi tanımlaması.
                if (txt_RowF2 >= 1 && txt_RowF2 < 20 && txt_ColumnF2 >= 1 && txt_ColumnF2 < 20)
                {
                    for (int row_index = 0; row_index < txt_RowF2; row_index++)
                    {
                        for (int coloumn_index = 0; coloumn_index < txt_ColumnF2; coloumn_index++)
                        {
                            matrisCounterF2++;

                            Button btn = new Button();
                            btn.Text = matrisCounterF2.ToString();                        //Buton text ismi

                            btn.Enabled = false;                                //Butona tıklanmaması için.
                            btn.Size = new Size(btn_size_w, btn_size_h);        //Buton boyutu
                            btn.BackColor = Color.Gainsboro;
                            btn.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));

                            if (f2ProgramStatus[matrisCounterF2 - 1] == "0")
                            {
                                btn.BackColor = Color.Blue;
                            }
                            else if (f2ProgramStatus[matrisCounterF2 - 1] == "1")
                            {
                                btn.BackColor = Color.Green;
                            }
                            else if (f2ProgramStatus[matrisCounterF2 - 1] == "2")
                            {
                                btn.BackColor = Color.Red;
                            }

                            loc_x = coloumn_index * btn_size_w;                 //Butonun X eksenindeki yeri
                            loc_y = row_index * btn_size_h;                     //Butonun Y eksenindeki yeri
                            btn.Location = new Point(loc_x, loc_y);

                            panel.Controls.Add(btn);                           //Panele ekle.
                            try
                            {
                                this.Invoke((MethodInvoker)delegate ()
                                {
                                    panel.Controls.Add(btn);                          //Panele ekle.
                                    buttonArrayF2[row_index, coloumn_index] = btn;               //Diziye doldur.
                                });
                            }
                            catch (Exception)
                            {
                                //  otherConsoleAppendLine("Dizi Hatası: " + ex.Message, Color.Red);
                            }
                        }
                    }
                    childButtonList.Add(buttonArrayF2);
                }
                else
                {
                    /* textBox1.Text = "1";
                     textBox2.Text = "1";*/
                    MessageBox.Show("Satır ve Sütün Sayısını Lütfen 1-19 Arasında Giriniz.!");
                }
            }
            catch (Exception)
            {
                //otherConsoleAppendLine("matrisAdd: " + ex.Message, Color.Red);
            }
            //Dizi[i, j].BackColor = Color.Red;
        }
        private void objectAdd(Panel panel, NumericUpDown numericUpDownPnlX, NumericUpDown numericUpDownPnlY, NumericUpDown numericUpDownPcbX, NumericUpDown numericUpDownPcbY, int rowNumber, Int16 panelNumber)
        {

            try
            {
                panel.Controls.Clear();                //Paneli temizle
                matrisCounterP = 0;                              //Sayacı sıfırla
                txt_RowP = (Int16)numericUpDownPnlY.Value;                     //Aldığımız Satır bilgisi.
                txt_ColumnP = (Int16)numericUpDownPnlX.Value;             //Aldığımız Sutün bilgisi

                int max_size_w = panel.Width;                              //Panelin Maksimum genişliği
                int max_size_h = panel.Height;                             //Panelin Maksimum yüksekliği

                int pnl_size_w = max_size_w / txt_ColumnP;         //Maksimum genişliğe sutun sayısını girerek butonun boyutu belirlenir.
                int pnl_size_h = max_size_h / txt_RowP;             //Maksimum yüksekliğe satır sayısını girerek butonun boyutu belirlenir.
                int loc_x;
                int loc_y;

                panelArray = new Panel[txt_RowP, txt_ColumnP];   //Dizi tanımlaması.
                childButtonList = new List<Button[,]>();
                if (txt_RowP >= 1 && txt_RowP < 20 && txt_ColumnP >= 1 && txt_ColumnP < 20)
                {
                    for (int row_index = 0; row_index < txt_RowP; row_index++)
                    {
                        for (int coloumn_index = 0; coloumn_index < txt_ColumnP; coloumn_index++)
                        {
                            matrisCounterP++;

                            Panel pnl = new Panel();
                            //  pnl.Text = matrisCounterP.ToString();                        //Buton text ismi
                            // pnl.Name = matrisCounterP.ToString();                        //Buton Design name

                            pnl.Enabled = false;                                //Butona tıklanmaması için.
                            pnl.Size = new Size(pnl_size_w, pnl_size_h);        //Buton boyutu
                            pnl.BackColor = Color.Gainsboro;
                            // pnl.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));

                            loc_x = coloumn_index * pnl_size_w;                 //Butonun X eksenindeki yeri
                            loc_y = row_index * pnl_size_h;                     //Butonun Y eksenindeki yeri
                            pnl.Location = new Point(loc_x, loc_y);

                            // panel.Controls.Add(pnl);                           //Panele ekle.
                            try
                            {
                                this.Invoke((MethodInvoker)delegate ()
                                {
                                    panel.Controls.Add(pnl);                          //Panele ekle.
                                    panelArray[row_index, coloumn_index] = pnl;               //Diziye doldur.
                                    pnl.BorderStyle = BorderStyle.FixedSingle;
                                    buttonAdd(pnl, numericUpDownPcbY, numericUpDownPcbX);


                                });
                            }
                            catch (Exception)
                            {
                                //  otherConsoleAppendLine("Dizi Hatası: " + ex.Message, Color.Red);
                            }
                        }
                    }
                    addList(rowNumber, panelNumber);
                }
                else
                {
                    //textBox3.Text = "1";
                    // textBox4.Text = "1";
                    MessageBox.Show("Satır ve Sütün Sayısını Lütfen 1-19 Arasında Giriniz.!");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            //Dizi[i, j].BackColor = Color.Red;
        }
        private void objectAddF2(Panel panel, NumericUpDown numericUpDownPnlX, NumericUpDown numericUpDownPnlY, NumericUpDown numericUpDownPcbX, NumericUpDown numericUpDownPcbY, int rowNumber, Int16 panelNumber)
        {

            try
            {
                panel.Controls.Clear();                //Paneli temizle
                matrisCounterPF2 = 0;                              //Sayacı sıfırla
                txt_RowPF2 = (Int16)numericUpDownPnlY.Value;                     //Aldığımız Satır bilgisi.
                txt_ColumnPF2 = (Int16)numericUpDownPnlX.Value;             //Aldığımız Sutün bilgisi

                int max_size_w = panel.Width;                              //Panelin Maksimum genişliği
                int max_size_h = panel.Height;                             //Panelin Maksimum yüksekliği

                int pnl_size_w = max_size_w / txt_ColumnPF2;         //Maksimum genişliğe sutun sayısını girerek butonun boyutu belirlenir.
                int pnl_size_h = max_size_h / txt_RowPF2;             //Maksimum yüksekliğe satır sayısını girerek butonun boyutu belirlenir.
                int loc_x;
                int loc_y;

                panelArrayF2 = new Panel[txt_RowPF2, txt_ColumnPF2];   //Dizi tanımlaması.                               
                childButtonList = new List<Button[,]>();
                if (txt_RowPF2 >= 1 && txt_RowPF2 < 20 && txt_ColumnPF2 >= 1 && txt_ColumnPF2 < 20)
                {
                    for (int row_index = 0; row_index < txt_RowPF2; row_index++)
                    {
                        for (int coloumn_index = 0; coloumn_index < txt_ColumnPF2; coloumn_index++)
                        {
                            matrisCounterPF2++;

                            Panel pnl = new Panel();
                            //  pnl.Text = matrisCounterP.ToString();                        //Buton text ismi
                            // pnl.Name = matrisCounterP.ToString();                        //Buton Design name

                            pnl.Enabled = false;                                //Butona tıklanmaması için.
                            pnl.Size = new Size(pnl_size_w, pnl_size_h);        //Buton boyutu
                            pnl.BackColor = Color.Gainsboro;
                            // pnl.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));

                            loc_x = coloumn_index * pnl_size_w;                 //Butonun X eksenindeki yeri
                            loc_y = row_index * pnl_size_h;                     //Butonun Y eksenindeki yeri
                            pnl.Location = new Point(loc_x, loc_y);

                            // panel.Controls.Add(pnl);                           //Panele ekle.
                            try
                            {
                                this.Invoke((MethodInvoker)delegate ()
                                {
                                    panel.Controls.Add(pnl);                          //Panele ekle.
                                    panelArrayF2[row_index, coloumn_index] = pnl;               //Diziye doldur.
                                    pnl.BorderStyle = BorderStyle.FixedSingle;
                                    buttonAddF2(pnl, numericUpDownPcbY, numericUpDownPcbX);

                                });
                            }
                            catch (Exception)
                            {
                                //  otherConsoleAppendLine("Dizi Hatası: " + ex.Message, Color.Red);
                            }
                        }
                    }
                    addList(rowNumber, panelNumber);
                }
                else
                {
                    //textBox3.Text = "1";
                    // textBox4.Text = "1";
                    MessageBox.Show("Satır ve Sütün Sayısını Lütfen 1-19 Arasında Giriniz.!");
                }
            }
            catch (Exception)
            {
                //otherConsoleAppendLine("matrisAdd: " + ex.Message, Color.Red);
            }
            //Dizi[i, j].BackColor = Color.Red;
        }
        private void panelAdd(Panel panel)
        {
            try
            {
                panel.Controls.Clear();
                int numCol = 1;
                if (checkBtnSolderF1.Checked == true && checkBtnProgF1.Checked == true)
                {
                    numCol = 2;
                }
                int max_size_w = panel.Width;
                int max_size_h = panel.Height;
                Color color;
                mainPanelArray = new Panel[2];
                for (int i = 0; i < numCol; i++)
                {
                    Panel pnl = new Panel();
                    pnl.Enabled = false;
                    pnl.Size = new Size(max_size_w / numCol, max_size_h);
                    if (i == 0)
                    {
                        color = Color.White;
                    }
                    else
                    {
                        color = Color.White;
                    }
                    pnl.BackColor = color;
                    pnl.Location = new Point((max_size_w / 2) * i, 0);
                    pnl.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                    pnl.BorderStyle = BorderStyle.FixedSingle;
                    try
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            panel.Controls.Add(pnl);
                            pnl.BorderStyle = BorderStyle.FixedSingle;
                            mainPanelArray[i] = pnl;
                        });
                    }
                    catch (Exception)
                    {
                        //  otherConsoleAppendLine("Dizi Hatası: " + ex.Message, Color.Red);
                    }
                }
            }
            catch (Exception)
            {
                //otherConsoleAppendLine("matrisAdd: " + ex.Message, Color.Red);
            }
        }
        private void panelAddF2(Panel panel)
        {
            try
            {
                panel.Controls.Clear();
                int numCol = 1;
                if (checkBtnSolderF2.Checked == true && checkBtnProgF2.Checked == true)
                {
                    numCol = 2;
                }
                int max_size_w = panel.Width;
                int max_size_h = panel.Height;
                Color color;
                mainPanelArrayF2 = new Panel[2];
                for (int i = 0; i < numCol; i++)
                {
                    Panel pnl = new Panel();
                    pnl.Enabled = false;
                    pnl.Size = new Size(max_size_w / numCol, max_size_h);
                    if (i == 0)
                    {
                        color = Color.White;
                    }
                    else
                    {
                        color = Color.White;
                    }
                    pnl.BackColor = color;
                    pnl.Location = new Point((max_size_w / 2) * i, 0);
                    pnl.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                    pnl.BorderStyle = BorderStyle.FixedSingle;
                    try
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            panel.Controls.Add(pnl);
                            pnl.BorderStyle = BorderStyle.FixedSingle;
                            mainPanelArrayF2[i] = pnl;
                        });
                    }
                    catch (Exception)
                    {
                        //  otherConsoleAppendLine("Dizi Hatası: " + ex.Message, Color.Red);
                    }
                }
            }
            catch (Exception)
            {
                //otherConsoleAppendLine("matrisAdd: " + ex.Message, Color.Red);
            }
        }
        private void lehimlemeActive()
        {
            if (checkBtnSolderF1.Checked == true)
            {
                checkBtnSolderF1.LookAndFeel.SkinMaskColor = Color.Green;
                checkBtnSolderF1.Text = "Lehimleme Aktif";
                enableObject(true, btnDGVAddF1, btnDGVCuteF1, btnDGVCopyF1, btnDGVPasteF1);
                dataGridViewSetupF1.Enabled = true;
            }
            else
            {
                checkBtnSolderF1.LookAndFeel.SkinMaskColor = Color.Red;
                checkBtnSolderF1.Text = "Lehimleme Kapalı";
                enableObject(false, btnDGVAddF1, btnDGVCuteF1, btnDGVCopyF1, btnDGVPasteF1, dataGridViewSetupF1);
                dataGridViewSetupF1.Enabled = false;
            }


            if (checkBtnProgF1.Checked == true)
            {
                checkBtnProgF1.LookAndFeel.SkinMaskColor = Color.Green;
                checkBtnProgF1.Text = "Programlama Aktif";
            }
            else
            {
                checkBtnProgF1.LookAndFeel.SkinMaskColor = Color.Red;
                checkBtnProgF1.Text = "Programlama Kapalı";
            }
            if (!checkBtnSolderF1.Checked && !checkBtnProgF1.Checked)
            {
                panelLeh1.Controls.Clear();
            }
        }
        private void callObjectAdd(NumericUpDown numericUpDownX, NumericUpDown numericUpDownY, NumericUpDown numericUpDownPcbX, NumericUpDown numericUpDownPcbY)
        {
            if (checkBtnProgF1.Checked && checkBtnSolderF1.Checked)
            {
                objectAdd(mainPanelArray[1], numericUpDownX, numericUpDownY, numericUpDownPcbX, numericUpDownPcbY, (Int16)txtPrg13F1.Value, 2);
            }
            else if (checkBtnProgF1.Checked)
            {
                objectAdd(mainPanelArray[0], numericUpDownX, numericUpDownY, numericUpDownPcbX, numericUpDownPcbY, (Int16)txtPrg13F1.Value, 2);
            }
            else if (checkBtnSolderF1.Checked)
            {

            }
        }
        private void lehimlemeActiveF2()
        {
            if (checkBtnSolderF2.Checked == true)
            {
                checkBtnSolderF2.LookAndFeel.SkinMaskColor = Color.Green;
                checkBtnSolderF2.Text = "Lehimleme Aktif";
                enableObject(true, btnDGVAddF2, btnDGVCuteF2, btnDGVCopyF1, btnDGVPasteF1);
                dataGridViewSetupF2.Enabled = true;

            }
            else
            {
                checkBtnSolderF2.LookAndFeel.SkinMaskColor = Color.Red;
                checkBtnSolderF2.Text = "Lehimleme Kapalı";
                enableObject(false, btnDGVAddF2, btnDGVCuteF2, btnDGVCopyF1, btnDGVPasteF1, dataGridViewSetupF2);
                dataGridViewSetupF2.Enabled = false;
            }


            if (checkBtnProgF2.Checked == true)
            {
                checkBtnProgF2.LookAndFeel.SkinMaskColor = Color.Green;
                checkBtnProgF2.Text = "Programlama Aktif";
            }
            else
            {
                checkBtnProgF2.LookAndFeel.SkinMaskColor = Color.Red;
                checkBtnProgF2.Text = "Programlama Kapalı";
            }
            if (!checkBtnSolderF2.Checked && !checkBtnProgF2.Checked)
            {
                panelLeh2.Controls.Clear();
            }
        }
        private void callObjectAddF2(NumericUpDown numericUpDownX, NumericUpDown numericUpDownY, NumericUpDown numericUpDownPcbX, NumericUpDown numericUpDownPcbY)
        {
            if (checkBtnProgF2.Checked && checkBtnSolderF2.Checked)
            {
                objectAddF2(mainPanelArrayF2[1], numericUpDownX, numericUpDownY, numericUpDownPcbX, numericUpDownPcbY, (Int16)txtPrg13F2.Value, 4);
            }
            else if (checkBtnProgF2.Checked)
            {
                objectAddF2(mainPanelArrayF2[0], numericUpDownX, numericUpDownY, numericUpDownPcbX, numericUpDownPcbY, (Int16)txtPrg13F2.Value, 4);
            }
        }
        //*****************************************************************//
        private void lhmTxtPnlXAdtF1_ValueChanged(object sender, EventArgs e)
        {
            objectAdd(mainPanelArray[0], lhmTxtPnlXAdtF1, lhmTxtPnlYAdtF1, lhmTxtPCBXAdtF1, lhmTxtPCBYAdtF1, (Int16)lhmTxtPnlXAdtF1.Value, 1);
        }
        private void lhmTxtPnlYAdtF1_ValueChanged(object sender, EventArgs e)
        {
            objectAdd(mainPanelArray[0], lhmTxtPnlXAdtF1, lhmTxtPnlYAdtF1, lhmTxtPCBXAdtF1, lhmTxtPCBYAdtF1, (Int16)lhmTxtPnlXAdtF1.Value, 1);
        }
        private void lhmTxtPCBXAdtF1_ValueChanged(object sender, EventArgs e)
        {
            objectAdd(mainPanelArray[0], lhmTxtPnlXAdtF1, lhmTxtPnlYAdtF1, lhmTxtPCBXAdtF1, lhmTxtPCBYAdtF1, (Int16)lhmTxtPnlXAdtF1.Value, 1);
        }
        private void lhmTxtPCBYAdtF1_ValueChanged(object sender, EventArgs e)
        {
            objectAdd(mainPanelArray[0], lhmTxtPnlXAdtF1, lhmTxtPnlYAdtF1, lhmTxtPCBXAdtF1, lhmTxtPCBYAdtF1, (Int16)lhmTxtPnlXAdtF1.Value, 1);
        }
        private void txtPrg7F1_ValueChanged(object sender, EventArgs e)
        {
            callObjectAdd(txtPrg13F1, txtPrg14F1, txtPrg7F1, txtPrg8F1);
        }
        private void txtPrg8F1_ValueChanged(object sender, EventArgs e)
        {
            callObjectAdd(txtPrg13F1, txtPrg14F1, txtPrg7F1, txtPrg8F1);
        }
        private void txtPrg13F1_ValueChanged(object sender, EventArgs e)
        {
            callObjectAdd(txtPrg13F1, txtPrg14F1, txtPrg7F1, txtPrg8F1);
        }
        private void txtPrg14F1_ValueChanged(object sender, EventArgs e)
        {
            callObjectAdd(txtPrg13F1, txtPrg14F1, txtPrg7F1, txtPrg8F1);
        }
        //*****************************************************************//
        private void lhmTxtPnlXAdtF2_ValueChanged(object sender, EventArgs e)
        {
            objectAddF2(mainPanelArrayF2[0], lhmTxtPnlXAdtF2, lhmTxtPnlYAdtF2, lhmTxtPCBXAdtF2, lhmTxtPCBYAdtF2, (Int16)lhmTxtPnlXAdtF2.Value, 3);
        }
        private void lhmTxtPnlYAdtF2_ValueChanged(object sender, EventArgs e)
        {
            objectAddF2(mainPanelArrayF2[0], lhmTxtPnlXAdtF2, lhmTxtPnlYAdtF2, lhmTxtPCBXAdtF2, lhmTxtPCBYAdtF2, (Int16)lhmTxtPnlXAdtF2.Value, 3);
        }
        private void lhmTxtPCBXAdtF2_ValueChanged(object sender, EventArgs e)
        {
            objectAddF2(mainPanelArrayF2[0], lhmTxtPnlXAdtF2, lhmTxtPnlYAdtF2, lhmTxtPCBXAdtF2, lhmTxtPCBYAdtF2, (Int16)lhmTxtPnlXAdtF2.Value, 3);
        }
        private void lhmTxtPCBYAdtF2_ValueChanged(object sender, EventArgs e)
        {
            objectAddF2(mainPanelArrayF2[0], lhmTxtPnlXAdtF2, lhmTxtPnlYAdtF2, lhmTxtPCBXAdtF2, lhmTxtPCBYAdtF2, (Int16)lhmTxtPnlXAdtF2.Value, 3);
        }
        private void txtPrg13F2_ValueChanged(object sender, EventArgs e)
        {
            callObjectAddF2(txtPrg13F2, txtPrg14F2, txtPrg7F2, txtPrg8F2);
        }
        private void txtPrg14F2_ValueChanged(object sender, EventArgs e)
        {
            callObjectAddF2(txtPrg13F2, txtPrg14F2, txtPrg7F2, txtPrg8F2);
        }
        private void txtPrg7F2_ValueChanged(object sender, EventArgs e)
        {
            callObjectAddF2(txtPrg13F2, txtPrg14F2, txtPrg7F2, txtPrg8F2);
        }
        private void txtPrg8F2_ValueChanged(object sender, EventArgs e)
        {
            callObjectAddF2(txtPrg13F2, txtPrg14F2, txtPrg7F2, txtPrg8F2);
        }
        private void clearPanel(Panel panel)
        {
            panel.Controls.Clear();
        }


        /******************************************* Setup Page Methods ************************************************************/
        private void addDataToGridView(DataGridView dataGridView, BindingSource bindingSource)
        {
            if (dataGridView.RowCount <= 499)
            {
                if (flagFikstür)
                {

                    newSetupData(bindingSource, (pos + 1).ToString(), "Noktasal", "", "", "", "", "", "", "", "", "", "", "", "");
                }
                else
                {
                    newSetupDataF2(bindingSource, (pos + 1).ToString(), "Noktasal", "", "", "", "", "", "", "", "", "", "", "", "");
                }

                dGVPaintRow(dataGridView, dataGridView.RowCount - 1);
                disableDGVSetupRowData(dataGridView, true, dataGridView.RowCount - 1, 11, 12, 13);
                dataGridViewListOrder(dataGridView);
            }
            else
            {
                MessageBox.Show("Satır sayısı aşıldı");
            }
        }

        private void programComboBoxData(ComboBox comboBox1, ComboBox comboBox2)
        {
            foreach (var item in Setting.Default.programListF1)
            {
                comboBox1.Items.Add(item);
                comboBox2.Items.Add(item);
            }
        }

        private void createModelF1(string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                File.Delete(iniFilePath);
            }
            INIKaydet ini = new INIKaydet(iniFilePath);
            ini.Yaz("LhmDurumF1", "Lehim Deger", checkBtnSolderF1.Checked.ToString());
            ini.Yaz("lhmTxtHavyaCF1", "Lehim Deger", lhmTxtHavyaCF1.Text);
            /**************pcb f1**********/
            ini.Yaz("lhmTxtPCBXP2F1", "Lehim Deger", lhmTxtPCBXP2F1.Text);
            ini.Yaz("lhmTxtPCBYP2F1", "Lehim Deger", lhmTxtPCBYP2F1.Text);
            ini.Yaz("lhmTxtPCBXAdtF1", "Lehim Deger", lhmTxtPCBXAdtF1.Text);

            ini.Yaz("lhmTxtPCBXP3F1", "Lehim Deger", lhmTxtPCBXP3F1.Text);
            ini.Yaz("lhmTxtPCBYP3F1", "Lehim Deger", lhmTxtPCBYP3F1.Text);
            ini.Yaz("lhmTxtPCBYAdtF1", "Lehim Deger", lhmTxtPCBYAdtF1.Text);

            /**************pnl f1**********/
            ini.Yaz("lhmTxtPnlXP2F1", "Lehim Deger", lhmTxtPnlXP2F1.Text);
            ini.Yaz("lhmTxtPnlYP2F1", "Lehim Deger", lhmTxtPnlYP2F1.Text);
            ini.Yaz("lhmTxtPnlXAdtF1", "Lehim Deger", lhmTxtPnlXAdtF1.Text);

            ini.Yaz("lhmTxtPnlXP3F1", "Lehim Deger", lhmTxtPnlXP3F1.Text);
            ini.Yaz("lhmTxtPnlYP3F1", "Lehim Deger", lhmTxtPnlYP3F1.Text);
            ini.Yaz("lhmTxtPnlYAdtF1", "Lehim Deger", lhmTxtPnlYAdtF1.Text);

            /**************dgv setup**********/


            if (dataGridViewSetupF1.RowCount > 0)
            {
                for (int i = 0; i < dataGridViewSetupF1.RowCount; i++)
                {
                    for (int j = 0; j < dataGridViewSetupF1.ColumnCount; j++)
                    {
                        ini.Yaz("dgvSetupRow" + i.ToString(), "Lehim Deger" + j.ToString(), (string)dataGridViewSetupF1.Rows[i].Cells[j].Value);
                    }
                }
            }

            /***********programlayıcı ***********/

            ini.Yaz("PrgDurumF1", "Programlama Deger", checkBtnProgF1.Checked.ToString());
            ini.Yaz("txtPrg1F1", "Programlama Deger", txtPrg1F1.Text);
            ini.Yaz("txtPrg2F1", "Programlama Deger", txtPrg2F1.Text);
            ini.Yaz("txtPrg3F1", "Programlama Deger", txtPrg3F1.Text);
            ini.Yaz("txtPrg4F1", "Programlama Deger", txtPrg4F1.Text);
            ini.Yaz("txtPrg5F1", "Programlama Deger", txtPrg5F1.Text);
            ini.Yaz("txtPrg6F1", "Programlama Deger", txtPrg6F1.Text);
            ini.Yaz("txtPrg7F1", "Programlama Deger", txtPrg7F1.Text);
            ini.Yaz("txtPrg8F1", "Programlama Deger", txtPrg8F1.Text);
            ini.Yaz("txtPrg9F1", "Programlama Deger", txtPrg9F1.Text);
            ini.Yaz("txtPrg10F1", "Programlama Deger", txtPrg10F1.Text);
            ini.Yaz("txtPrg11F1", "Programlama Deger", txtPrg11F1.Text);
            ini.Yaz("txtPrg12F1", "Programlama Deger", txtPrg12F1.Text);
            ini.Yaz("txtPrg13F1", "Programlama Deger", txtPrg13F1.Text);
            ini.Yaz("txtPrg14F1", "Programlama Deger", txtPrg14F1.Text);
            ini.Yaz("cmbBxFileNameF1", "Programlama Deger", cmbBxFileNameF1.Text);
            ini.Yaz("txtFilePathF1", "Programlama Deger", txtFilePathF1.Text);

        }
        private void createModelF2(string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                File.Delete(iniFilePath);
            }
            INIKaydet ini = new INIKaydet(iniFilePath);
            ini.Yaz("LhmDurumF2", "Lehim Deger", checkBtnSolderF2.Checked.ToString());
            ini.Yaz("lhmTxtHavyaCF2", "Lehim Deger", lhmTxtHavyaCF2.Text);
            /**************pcb f1**********/
            ini.Yaz("lhmTxtPCBXP2F2", "Lehim Deger", lhmTxtPCBXP2F2.Text);
            ini.Yaz("lhmTxtPCBYP2F2", "Lehim Deger", lhmTxtPCBYP2F2.Text);
            ini.Yaz("lhmTxtPCBXAdtF2", "Lehim Deger", lhmTxtPCBXAdtF2.Text);

            ini.Yaz("lhmTxtPCBXP3F2", "Lehim Deger", lhmTxtPCBXP3F2.Text);
            ini.Yaz("lhmTxtPCBYP3F2", "Lehim Deger", lhmTxtPCBYP3F2.Text);
            ini.Yaz("lhmTxtPCBYAdtF2", "Lehim Deger", lhmTxtPCBYAdtF2.Text);

            /**************pnl f1**********/
            ini.Yaz("lhmTxtPnlXP2F2", "Lehim Deger", lhmTxtPnlXP2F2.Text);
            ini.Yaz("lhmTxtPnlYP2F2", "Lehim Deger", lhmTxtPnlYP2F2.Text);
            ini.Yaz("lhmTxtPnlXAdtF2", "Lehim Deger", lhmTxtPnlXAdtF2.Text);

            ini.Yaz("lhmTxtPnlXP3F2", "Lehim Deger", lhmTxtPnlXP3F2.Text);
            ini.Yaz("lhmTxtPnlYP3F2", "Lehim Deger", lhmTxtPnlYP3F2.Text);
            ini.Yaz("lhmTxtPnlYAdtF2", "Lehim Deger", lhmTxtPnlYAdtF2.Text);

            /**************dgv setup**********/

            if (dataGridViewSetupF2.RowCount > 0)
            {
                for (int i = 0; i < dataGridViewSetupF2.RowCount; i++)
                {
                    for (int j = 0; j < dataGridViewSetupF2.ColumnCount; j++)
                    {
                        ini.Yaz("dgvSetupRow" + i.ToString(), "Lehim Deger" + j.ToString(), (string)dataGridViewSetupF2.Rows[i].Cells[j].Value.ToString());
                    }
                }
            }

            /***********programlayıcı ***********/

            ini.Yaz("PrgDurumF2", "Programlama Deger", checkBtnProgF2.Checked.ToString());
            ini.Yaz("txtPrg1F2", "Programlama Deger", txtPrg1F2.Text);
            ini.Yaz("txtPrg2F2", "Programlama Deger", txtPrg2F2.Text);
            ini.Yaz("txtPrg3F2", "Programlama Deger", txtPrg3F2.Text);
            ini.Yaz("txtPrg4F2", "Programlama Deger", txtPrg4F2.Text);
            ini.Yaz("txtPrg5F2", "Programlama Deger", txtPrg5F2.Text);
            ini.Yaz("txtPrg6F2", "Programlama Deger", txtPrg6F2.Text);
            ini.Yaz("txtPrg7F2", "Programlama Deger", txtPrg7F2.Text);
            ini.Yaz("txtPrg8F2", "Programlama Deger", txtPrg8F2.Text);
            ini.Yaz("txtPrg9F2", "Programlama Deger", txtPrg9F2.Text);
            ini.Yaz("txtPrg10F2", "Programlama Deger", txtPrg10F2.Text);
            ini.Yaz("txtPrg11F2", "Programlama Deger", txtPrg11F2.Text);
            ini.Yaz("txtPrg12F2", "Programlama Deger", txtPrg12F2.Text);
            ini.Yaz("txtPrg13F2", "Programlama Deger", txtPrg13F2.Text);
            ini.Yaz("txtPrg14F2", "Programlama Deger", txtPrg14F2.Text);
            ini.Yaz("cmbBxFileNameF2", "Programlama Deger", cmbBxFileNameF2.Text);
            ini.Yaz("txtFilePathF2", "Programlama Deger", txtFilePathF2.Text);

        }

        public void loadModelF1(string iniFilePath)
        {
            try
            {
                if (File.Exists(iniFilePath))
                {
                    iniLoadF1 = new INIKaydet(iniFilePath);

                    int rowIndex = 0;
                    dataGridViewSetupF1.Rows.Clear();

                    checkBtnSolderF1.Checked = Boolean.Parse(iniLoadF1.Oku("LhmDurumF1", "Lehim Deger"));
                    lhmTxtHavyaCF1.Text = iniLoadF1.Oku("lhmTxtHavyaCF1", "Lehim Deger");
                    lhmDegree1F1 = lhmTxtHavyaCF1.Text;


                    lhmTxtPCBXP2F1.Text = iniLoadF1.Oku("lhmTxtPCBXP2F1", "Lehim Deger");
                    lhmTxtPCBYP2F1.Text = iniLoadF1.Oku("lhmTxtPCBYP2F1", "Lehim Deger");
                    lhmTxtPCBXAdtF1.Text = iniLoadF1.Oku("lhmTxtPCBXAdtF1", "Lehim Deger");

                    lhmTxtPCBXP3F1.Text = iniLoadF1.Oku("lhmTxtPCBXP3F1", "Lehim Deger");
                    lhmTxtPCBYP3F1.Text = iniLoadF1.Oku("lhmTxtPCBYP3F1", "Lehim Deger");
                    lhmTxtPCBYAdtF1.Text = iniLoadF1.Oku("lhmTxtPCBYAdtF1", "Lehim Deger");

                    lhmTxtPnlXP2F1.Text = iniLoadF1.Oku("lhmTxtPnlXP2F1", "Lehim Deger");
                    lhmTxtPnlYP2F1.Text = iniLoadF1.Oku("lhmTxtPnlYP2F1", "Lehim Deger");
                    lhmTxtPnlXAdtF1.Text = iniLoadF1.Oku("lhmTxtPnlXAdtF1", "Lehim Deger");

                    lhmTxtPnlXP3F1.Text = iniLoadF1.Oku("lhmTxtPnlXP3F1", "Lehim Deger");
                    lhmTxtPnlYP3F1.Text = iniLoadF1.Oku("lhmTxtPnlYP3F1", "Lehim Deger");
                    lhmTxtPnlYAdtF1.Text = iniLoadF1.Oku("lhmTxtPnlYAdtF1", "Lehim Deger");

                    do
                    {
                        if (iniLoadF1.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger0") != "")
                        {
                            for (int j = 0; j < dataGridViewSetupF1.ColumnCount; j++)
                            {
                                listLehim.Add(iniLoadF1.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger" + j.ToString()));
                            }
                            newSetupData(setupBindingSource, listLehim[0], listLehim[1], listLehim[2], listLehim[3], listLehim[4], listLehim[5], listLehim[6], listLehim[7], listLehim[8],
                                listLehim[9], listLehim[10], listLehim[11], listLehim[12], listLehim[13], listLehim[14]);
                            mainPosF1 = int.Parse(listLehim[0]);
                            dGVPaintRow(dataGridViewSetupF1, dataGridViewSetupF1.RowCount - 1);
                            if ((string)listLehim[1] == "Noktasal")
                            {
                                disableDGVSetupRowData(dataGridViewSetupF1, true, dataGridViewSetupF1.RowCount - 1, 11, 12, 13);
                            }
                            else
                            {
                                disableDGVSetupRowData(dataGridViewSetupF1, false, dataGridViewSetupF1.RowCount - 1, 11, 12, 13);
                            }
                            listLehim.Clear();
                            rowIndex++;
                        }
                        else
                        {
                            break;
                        }
                    } while (true);

                    checkBtnProgF1.Checked = Boolean.Parse(iniLoadF1.Oku("PrgDurumF1", "Programlama Deger"));
                    txtPrg1F1.Text = iniLoadF1.Oku("txtPrg1F1", "Programlama Deger");
                    txtPrg2F1.Text = iniLoadF1.Oku("txtPrg2F1", "Programlama Deger");
                    txtPrg3F1.Text = iniLoadF1.Oku("txtPrg3F1", "Programlama Deger");
                    txtPrg4F1.Text = iniLoadF1.Oku("txtPrg4F1", "Programlama Deger");
                    txtPrg5F1.Text = iniLoadF1.Oku("txtPrg5F1", "Programlama Deger");
                    txtPrg6F1.Text = iniLoadF1.Oku("txtPrg6F1", "Programlama Deger");
                    txtPrg7F1.Text = iniLoadF1.Oku("txtPrg7F1", "Programlama Deger");
                    txtPrg8F1.Text = iniLoadF1.Oku("txtPrg8F1", "Programlama Deger");
                    txtPrg9F1.Text = iniLoadF1.Oku("txtPrg9F1", "Programlama Deger");
                    txtPrg10F1.Text = iniLoadF1.Oku("txtPrg10F1", "Programlama Deger");
                    txtPrg11F1.Text = iniLoadF1.Oku("txtPrg11F1", "Programlama Deger");
                    txtPrg12F1.Text = iniLoadF1.Oku("txtPrg12F1", "Programlama Deger");
                    txtPrg13F1.Text = iniLoadF1.Oku("txtPrg13F1", "Programlama Deger");
                    txtPrg14F1.Text = iniLoadF1.Oku("txtPrg14F1", "Programlama Deger");
                    cmbBxFileNameF1.Text = iniLoadF1.Oku("cmbBxFileNameF1", "Programlama Deger");
                    txtFilePathF1.Text = iniLoadF1.Oku("txtFilePathF1", "Programlama Deger");
                    if (checkBtnSolderF1.Checked)
                    {
                        panelAdd(panelLeh1);
                        objectAdd(mainPanelArray[0], lhmTxtPnlXAdtF1, lhmTxtPnlYAdtF1, lhmTxtPCBXAdtF1, lhmTxtPCBYAdtF1, (Int16)lhmTxtPnlXAdtF1.Value, 1);
                    }
                    if (checkBtnProgF1.Checked)
                    {
                        if (!checkBtnSolderF1.Checked)
                        {
                            panelAdd(panelLeh1);
                        }
                        callObjectAdd(txtPrg13F1, txtPrg14F1, txtPrg7F1, txtPrg8F1);
                    }


                    //** PLC Lehim F1 RECİPE **//
                    int rowIndex2 = 0;
                    string boolPrg, boolFlag2;
                    iniLoadF1 = new INIKaydet(iniFilePath);
                    if (iniLoadF1.Oku("LhmDurumF1", "Lehim Deger") == "True")
                    {
                        boolPrg = "0";
                    }
                    else
                    {
                        boolPrg = "1";
                    }
                    nxCompoletStringWrite("f1LehimVarYok", boolPrg);
                    nxCompoletStringWrite("f1HavyaSicaklikOk", iniLoadF1.Oku("lhmTxtHavyaCF1", "Lehim Deger"));

                    do
                    {
                        if (iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger0") != "")
                        {
                            nxCompoletStringWrite("f1PozisyonSayi", dataGridViewSetupF1.RowCount.ToString());
                            if (iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger1") == "Noktasal")
                            {
                                noktasalF1 = "0";
                            }
                            else
                            {
                                noktasalF1 = "1";
                            }
                            nxCompoletStringWrite("f1LehimAkis[" + rowIndex2 + "]", noktasalF1);
                            nxCompoletStringWrite("f1Xpos1[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger2"));
                            nxCompoletStringWrite("f1Ypos1[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger3"));
                            nxCompoletStringWrite("f1Wpos[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger4"));
                            nxCompoletStringWrite("f1Zpos[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger5"));
                            nxCompoletStringWrite("f1LehimOnceBekleme[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger6"));
                            nxCompoletStringWrite("f1LehimSonraBekleme[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger7"));
                            nxCompoletStringWrite("f1LehimMiktar[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger8"));
                            nxCompoletStringWrite("f1LehimHiz[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger9"));
                            nxCompoletStringWrite("f1MaxYukseklik[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger10"));
                            nxCompoletStringWrite("f1XbitisPos[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger11"));
                            nxCompoletStringWrite("f1YbitisPos[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger12"));
                            nxCompoletStringWrite("f1XyHiz[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger13"));
                            nxCompoletStringWrite("f1OndenLehim[" + rowIndex2 + "]", iniLoadF1.Oku("dgvSetupRow" + rowIndex2.ToString(), "Lehim Deger14"));
                            rowIndex2++;
                        }
                        else
                        {
                            break;
                        }
                    } while (true);


                    nxCompoletStringWrite("f1XpcbArrayPos2", iniLoadF1.Oku("lhmTxtPCBXP2F1", "Lehim Deger"));
                    nxCompoletStringWrite("f1YpcbArrayPos2", iniLoadF1.Oku("lhmTxtPCBYP2F1", "Lehim Deger"));
                    nxCompoletStringWrite("f1XpcbArray", iniLoadF1.Oku("lhmTxtPCBXAdtF1", "Lehim Deger"));


                    nxCompoletStringWrite("f1XpcbArrayPos3", iniLoadF1.Oku("lhmTxtPCBXP3F1", "Lehim Deger"));
                    nxCompoletStringWrite("f1YpcbArrayPos3", iniLoadF1.Oku("lhmTxtPCBYP3F1", "Lehim Deger"));
                    nxCompoletStringWrite("f1YpcbArray", iniLoadF1.Oku("lhmTxtPCBYAdtF1", "Lehim Deger"));


                    nxCompoletStringWrite("f1XpanelArrayPos2", iniLoadF1.Oku("lhmTxtPnlXP2F1", "Lehim Deger"));
                    nxCompoletStringWrite("f1YpanelArrayPos2", iniLoadF1.Oku("lhmTxtPnlYP2F1", "Lehim Deger"));
                    nxCompoletStringWrite("f1XpanelArray", iniLoadF1.Oku("lhmTxtPnlXAdtF1", "Lehim Deger"));


                    nxCompoletStringWrite("f1XpanelArrayPos3", iniLoadF1.Oku("lhmTxtPnlXP3F1", "Lehim Deger"));
                    nxCompoletStringWrite("f1YpanelArrayPos3", iniLoadF1.Oku("lhmTxtPnlYP3F1", "Lehim Deger"));
                    nxCompoletStringWrite("f1YpanelArray", iniLoadF1.Oku("lhmTxtPnlYAdtF1", "Lehim Deger"));


                    /* PLC Program F1 RECİPE */
                    if (iniLoadF1.Oku("PrgDurumF1", "Programlama Deger") == "True")
                    {
                        boolFlag2 = "0";
                    }
                    else
                    {
                        boolFlag2 = "1";
                    }
                    nxCompoletStringWrite("f1ProgramlayiciVarYok", boolFlag2);
                    nxCompoletStringWrite("f1ProgramXpos1", iniLoadF1.Oku("txtPrg1F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramYpos1", iniLoadF1.Oku("txtPrg2F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramXpos2", iniLoadF1.Oku("txtPrg3F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramYpos2", iniLoadF1.Oku("txtPrg4F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramXpos3", iniLoadF1.Oku("txtPrg5F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramYpos3", iniLoadF1.Oku("txtPrg6F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramXpcbArray", iniLoadF1.Oku("txtPrg7F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramYpcbArray", iniLoadF1.Oku("txtPrg8F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramPanelXpos2", iniLoadF1.Oku("txtPrg9F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramPanelYpos2", iniLoadF1.Oku("txtPrg10F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramPanelXpos3", iniLoadF1.Oku("txtPrg11F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramPanelYpos3", iniLoadF1.Oku("txtPrg12F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramXpanelArray", iniLoadF1.Oku("txtPrg13F1", "Programlama Deger"));
                    nxCompoletStringWrite("f1ProgramYpanelArray", iniLoadF1.Oku("txtPrg14F1", "Programlama Deger"));

                    nxCompoletBoolWrite("f1RecipeSend", true);
                    programingPathF1 = txtFilePathF1.Text;
                    mainTxtAddToOne(txtStartX1F1, txtStartX2F1, txtStartY1F1, txtStartY2F1, txtStartPF1);
                    mainTxtAddToMinValue(maxMainTxtValue, lhmTxtPnlXAdtF1, lhmTxtPCBXAdtF1, lhmTxtPnlYAdtF1, lhmTxtPCBYAdtF1);
                    nxCompoletBoolWrite("recipeSend", true);
                    sendHavyaDegreeData(lhmDegree1F1);
                    MessageBox.Show("Veriler Yüklendi");

                }
                else
                {
                    MessageBox.Show("Dosya Bulunamadı !");
                }
            }
            catch (Exception e)
            {

                MessageBox.Show("Yükleme hatası: " + e.ToString());
            }

        }

        public void loadModelF2(string iniFilePath)
        {
            try
            {
                if (File.Exists(iniFilePath))
                {

                    int rowIndex = 0;
                    iniLoadF2 = new INIKaydet(iniFilePath);
                    dataGridViewSetupF2.Rows.Clear();

                    checkBtnSolderF2.Checked = Boolean.Parse(iniLoadF2.Oku("LhmDurumF2", "Lehim Deger"));
                    lhmTxtHavyaCF2.Text = iniLoadF2.Oku("lhmTxtHavyaCF2", "Lehim Deger");
                    lhmDegree1F2 = lhmTxtHavyaCF2.Text;

                    lhmTxtPCBXP2F2.Text = iniLoadF2.Oku("lhmTxtPCBXP2F2", "Lehim Deger");
                    lhmTxtPCBYP2F2.Text = iniLoadF2.Oku("lhmTxtPCBYP2F2", "Lehim Deger");
                    lhmTxtPCBXAdtF2.Text = iniLoadF2.Oku("lhmTxtPCBXAdtF2", "Lehim Deger");

                    lhmTxtPCBXP3F2.Text = iniLoadF2.Oku("lhmTxtPCBXP3F2", "Lehim Deger");
                    lhmTxtPCBYP3F2.Text = iniLoadF2.Oku("lhmTxtPCBYP3F2", "Lehim Deger");
                    lhmTxtPCBYAdtF2.Text = iniLoadF2.Oku("lhmTxtPCBYAdtF2", "Lehim Deger");

                    lhmTxtPnlXP2F2.Text = iniLoadF2.Oku("lhmTxtPnlXP2F2", "Lehim Deger");
                    lhmTxtPnlYP2F2.Text = iniLoadF2.Oku("lhmTxtPnlYP2F2", "Lehim Deger");
                    lhmTxtPnlXAdtF2.Text = iniLoadF2.Oku("lhmTxtPnlXAdtF2", "Lehim Deger");

                    lhmTxtPnlXP3F2.Text = iniLoadF2.Oku("lhmTxtPnlXP3F2", "Lehim Deger");
                    lhmTxtPnlYP3F2.Text = iniLoadF2.Oku("lhmTxtPnlYP3F2", "Lehim Deger");
                    lhmTxtPnlYAdtF2.Text = iniLoadF2.Oku("lhmTxtPnlYAdtF2", "Lehim Deger");

                    do
                    {
                        if (iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger0") != "")
                        {
                            for (int j = 0; j < dataGridViewSetupF2.ColumnCount; j++)
                            {
                                listLehim.Add(iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger" + j.ToString()));
                            }
                            newSetupDataF2(setupF2BindingSource, listLehim[0], listLehim[1], listLehim[2], listLehim[3], listLehim[4], listLehim[5], listLehim[6], listLehim[7], listLehim[8],
                                listLehim[9], listLehim[10], listLehim[11], listLehim[12], listLehim[13], listLehim[14]);
                            mainPosF2 = int.Parse(listLehim[0]);
                            dGVPaintRow(dataGridViewSetupF2, dataGridViewSetupF2.RowCount - 1);
                            if ((string)listLehim[1] == "Noktasal")
                            {
                                disableDGVSetupRowData(dataGridViewSetupF2, true, dataGridViewSetupF2.RowCount - 1, 11, 12, 13);
                            }
                            else
                            {
                                disableDGVSetupRowData(dataGridViewSetupF2, false, dataGridViewSetupF2.RowCount - 1, 11, 12, 13);
                            }
                            dataGridViewListOrder(dataGridViewSetupF2);
                            listLehim.Clear();
                            rowIndex++;
                        }
                        else
                        {
                            break;
                        }
                    } while (true);

                    checkBtnProgF2.Checked = Boolean.Parse(iniLoadF2.Oku("PrgDurumF2", "Programlama Deger"));
                    txtPrg1F2.Text = iniLoadF2.Oku("txtPrg1F2", "Programlama Deger");
                    txtPrg2F2.Text = iniLoadF2.Oku("txtPrg2F2", "Programlama Deger");
                    txtPrg3F2.Text = iniLoadF2.Oku("txtPrg3F2", "Programlama Deger");
                    txtPrg4F2.Text = iniLoadF2.Oku("txtPrg4F2", "Programlama Deger");
                    txtPrg5F2.Text = iniLoadF2.Oku("txtPrg5F2", "Programlama Deger");
                    txtPrg6F2.Text = iniLoadF2.Oku("txtPrg6F2", "Programlama Deger");
                    txtPrg7F2.Text = iniLoadF2.Oku("txtPrg7F2", "Programlama Deger");
                    txtPrg8F2.Text = iniLoadF2.Oku("txtPrg8F2", "Programlama Deger");
                    txtPrg9F2.Text = iniLoadF2.Oku("txtPrg9F2", "Programlama Deger");
                    txtPrg10F2.Text = iniLoadF2.Oku("txtPrg10F2", "Programlama Deger");
                    txtPrg11F2.Text = iniLoadF2.Oku("txtPrg11F2", "Programlama Deger");
                    txtPrg12F2.Text = iniLoadF2.Oku("txtPrg12F2", "Programlama Deger");
                    txtPrg13F2.Text = iniLoadF2.Oku("txtPrg13F2", "Programlama Deger");
                    txtPrg14F2.Text = iniLoadF2.Oku("txtPrg14F2", "Programlama Deger");
                    cmbBxFileNameF2.Text = iniLoadF2.Oku("cmbBxFileNameF2", "Programlama Deger");
                    txtFilePathF2.Text = iniLoadF2.Oku("txtFilePathF2", "Programlama Deger");
                    if (checkBtnSolderF2.Checked)
                    {
                        panelAddF2(panelLeh2);
                        objectAddF2(mainPanelArrayF2[0], lhmTxtPnlXAdtF2, lhmTxtPnlYAdtF2, lhmTxtPCBXAdtF2, lhmTxtPCBYAdtF2, (Int16)lhmTxtPnlXAdtF2.Value, 3);
                    }
                    if (checkBtnProgF2.Checked)
                    {
                        if (!checkBtnSolderF2.Checked)
                        {
                            panelAddF2(panelLeh2);
                        }
                        callObjectAddF2(txtPrg13F2, txtPrg14F2, txtPrg7F2, txtPrg8F2);
                    }



                    //** PLC Lehim F2 RECİPE **//
                    rowIndex = 0;
                    string boolPrg, boolFlag2;
                    iniLoadF2 = new INIKaydet(iniFilePath);
                    if (iniLoadF2.Oku("LhmDurumF2", "Lehim Deger") == "True")
                    {
                        boolPrg = "0";
                    }
                    else
                    {
                        boolPrg = "1";
                    }
                    nxCompoletStringWrite("f2LehimVarYok", boolPrg);
                    nxCompoletStringWrite("f2MaxYukseklik", iniLoadF2.Oku("lhmTxtCompHF2", " Lehim Deger"));
                    nxCompoletStringWrite("f2HavyaSicaklikOk", iniLoadF2.Oku("lhmTxtHavyaCF2", "Lehim Deger"));

                    do
                    {
                        if (iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger0") != "")
                        {
                            nxCompoletStringWrite("f2PozisyonSayi", dataGridViewSetupF2.RowCount.ToString());
                            if (iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger1") == "Noktasal")
                            {
                                noktasalF2 = "0";
                            }
                            else
                            {
                                noktasalF2 = "1";
                            }
                            nxCompoletStringWrite("f2LehimAkis[" + rowIndex + "]", noktasalF2);
                            nxCompoletStringWrite("f2Xpos1[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger2"));
                            nxCompoletStringWrite("f2Ypos1[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger3"));
                            nxCompoletStringWrite("f2Wpos[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger4"));
                            nxCompoletStringWrite("f2Zpos[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger5"));
                            nxCompoletStringWrite("f2LehimOnceBekleme[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger6"));
                            nxCompoletStringWrite("f2LehimSonraBekleme[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger7"));
                            nxCompoletStringWrite("f2LehimMiktar[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger8"));
                            nxCompoletStringWrite("f2LehimHiz[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger9"));
                            nxCompoletStringWrite("f2MaxYukseklik[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger10"));
                            nxCompoletStringWrite("f2XbitisPos[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger11"));
                            nxCompoletStringWrite("f2YbitisPos[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger12"));
                            nxCompoletStringWrite("f2XyHiz[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger13"));
                            nxCompoletStringWrite("f2   OndenLehim[" + rowIndex + "]", iniLoadF2.Oku("dgvSetupRow" + rowIndex.ToString(), "Lehim Deger14")); ;
                            rowIndex++;
                        }
                        else
                        {
                            break;
                        }
                    } while (true);



                    nxCompoletStringWrite("f2XpcbArrayPos2", iniLoadF2.Oku("lhmTxtPCBXP2F2", "Lehim Deger"));
                    nxCompoletStringWrite("f2YpcbArrayPos2", iniLoadF2.Oku("lhmTxtPCBYP2F2", "Lehim Deger"));
                    nxCompoletStringWrite("f2XpcbArray", iniLoadF2.Oku("lhmTxtPCBXAdtF2", "Lehim Deger"));


                    nxCompoletStringWrite("f2XpcbArrayPos3", iniLoadF2.Oku("lhmTxtPCBXP3F2", "Lehim Deger"));
                    nxCompoletStringWrite("f2YpcbArrayPos3", iniLoadF2.Oku("lhmTxtPCBYP3F2", "Lehim Deger"));
                    nxCompoletStringWrite("f2YpcbArray", iniLoadF2.Oku("lhmTxtPCBYAdtF2", "Lehim Deger"));


                    nxCompoletStringWrite("f2XpanelArrayPos2", iniLoadF2.Oku("lhmTxtPnlXP2F2", "Lehim Deger"));
                    nxCompoletStringWrite("f2YpanelArrayPos2", iniLoadF2.Oku("lhmTxtPnlYP2F2", "Lehim Deger"));
                    nxCompoletStringWrite("f2XpanelArray", iniLoadF2.Oku("lhmTxtPnlXAdtF2", "Lehim Deger"));


                    nxCompoletStringWrite("f2XpanelArrayPos3", iniLoadF2.Oku("lhmTxtPnlXP3F2", "Lehim Deger"));
                    nxCompoletStringWrite("f2YpanelArrayPos3", iniLoadF2.Oku("lhmTxtPnlYP3F2", "Lehim Deger"));
                    nxCompoletStringWrite("f2YpanelArray", iniLoadF2.Oku("lhmTxtPnlYAdtF2", "Lehim Deger"));


                    //** PLC Program F2 RECİPE **//
                    if (iniLoadF2.Oku("PrgDurumF2", "Programlama Deger") == "True")
                    {
                        boolFlag2 = "0";
                    }
                    else
                    {
                        boolFlag2 = "1";
                    }
                    nxCompoletStringWrite("f2ProgramlayiciVarYok", boolFlag2);
                    nxCompoletStringWrite("f2ProgramXpos1", iniLoadF2.Oku("txtPrg1F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramYpos1", iniLoadF2.Oku("txtPrg2F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramXpos2", iniLoadF2.Oku("txtPrg3F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramYpos2", iniLoadF2.Oku("txtPrg4F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramXpos3", iniLoadF2.Oku("txtPrg5F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramYpos3", iniLoadF2.Oku("txtPrg6F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramXpcbArray", iniLoadF2.Oku("txtPrg7F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramYpcbArray", iniLoadF2.Oku("txtPrg8F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramPanelXpos2", iniLoadF2.Oku("txtPrg9F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramPanelYpos2", iniLoadF2.Oku("txtPrg10F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramPanelXpos3", iniLoadF2.Oku("txtPrg11F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramPanelYpos3", iniLoadF2.Oku("txtPrg12F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramXpanelArray", iniLoadF2.Oku("txtPrg13F2", "Programlama Deger"));
                    nxCompoletStringWrite("f2ProgramYpanelArray", iniLoadF2.Oku("txtPrg14F2", "Programlama Deger"));

                    nxCompoletBoolWrite("f2RecipeSend", true);
                    programingPathF2 = txtFilePathF2.Text;
                    mainTxtAddToOne(txtStartX1F2, txtStartX2F2, txtStartY1F2, txtStartY2F2, txtStartPF2);
                    mainTxtAddToMinValue(maxMainTxtValueF2, lhmTxtPnlXAdtF2, lhmTxtPCBXAdtF2, lhmTxtPnlYAdtF2, lhmTxtPCBYAdtF2);
                    nxCompoletBoolWrite("recipeSend", true);
                    sendHavyaDegreeData(lhmDegree1F2);
                    MessageBox.Show("Veriler Yüklendi");
                }
                else
                {
                    MessageBox.Show("Dosya Bulunamadı !");
                }
            }
            catch (Exception e)
            {

                MessageBox.Show("Yükleme hatası : " + e.ToString());
            }

        }
        private void modelSave(TextBox textBox, string modelName, string modelPath)
        {
            if (textBox.Text != "")
            {
                try
                {
                    modelFilePath = desktopPath + @"/Modeller/" + modelName + ".ini";
                    createFilePath = desktopPath + @"/" + modelPath + "/" + textBox.Text + ".ini";
                    if (File.Exists(modelFilePath))
                    {

                        textBox.Enabled = false;
                        if (modelName == "ModelF1")
                        {
                            createModelF1(createFilePath);
                        }
                        else if (modelName == "ModelF2")
                        {
                            createModelF2(createFilePath);
                        }
                        else
                        {
                            MessageBox.Show("Yanlış Model Adı");
                        }

                        INIKaydet iNIKaydet = new INIKaydet(modelFilePath);
                        modelSayisi = Int16.Parse(iNIKaydet.Oku("Model", "ModelSayisi"));
                        int modelCounter = modelSayisi;
                        bool flagModel = false;
                        for (int i = 0; i < modelCounter; i++)
                        {
                            if (textBox.Text + ".ini" == iNIKaydet.Oku("Model" + (i + 1).ToString(), "ModelAdi"))
                            {
                                flagModel = true;
                                modelCounter = i + 1;
                                modelSayisi = modelCounter;
                                break;
                            }
                        }
                        if (!flagModel)
                        {
                            modelSayisi++;
                            iNIKaydet.Yaz("Model", "ModelSayisi", modelSayisi.ToString());
                        }
                        //  MessageBox.Show("model sayisi : " + modelSayisi.ToString());
                        iNIKaydet.Yaz("Model" + modelSayisi.ToString(), "ModelAdi", textBox.Text + ".ini");
                        iNIKaydet.Yaz("Model" + modelSayisi.ToString(), "ModelYolu", createFilePath);

                        MessageBox.Show("Model Kaydedildi");


                    }
                    else
                    {
                        MessageBox.Show("Model Dosyaları Bulunmadı");
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                MessageBox.Show("Lütfen model adını boş bırakmayınız");
            }
        }

        private void mainTxtAddToOne(params TextBox[] textBoxes)
        {
            for (int i = 0; i < textBoxes.Length; i++)
            {
                textBoxes[i].Text = "1";
            }
        }
        private void mainTxtAddToMinValue(int[] txtmaxMinValues, params NumericUpDown[] numericUpDowns)
        {
            for (int i = 0; i < numericUpDowns.Length; i++)
            {
                txtmaxMinValues[i] = (int)numericUpDowns[i].Value;
            }
        }


        /*** Datagridview Events and Methods ***/
        private void dataGridViewSetupF1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewSetupF1.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Silmek istediğinize emin misiniz ?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    setupBindingSource.RemoveCurrent();
                    dataGridViewListOrder(dataGridViewSetupF1);
                }
            }
        }
        private void dataGridViewSetupF2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewSetupF2.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Silmek istediğinize emin misiniz ?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    setupF2BindingSource.RemoveCurrent();
                    dataGridViewListOrder(dataGridViewSetupF2);
                }
            }
        }
        private void dataGridViewSetupF1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            pos = Int16.Parse(e.RowIndex.ToString());
        }
        private void dataGridViewSetupF2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            pos = Int16.Parse(e.RowIndex.ToString());
        }
        private void newSetupData(BindingSource bindingSource, string PosN = "", string f1LehimAkisN = "", string f1XPozisyon1N = "", string f1YPozisyon1N = "",
                          string f1WPosN = "", string f1ZPosN = "", string f1LehimOnceBeklemeN = ""
                          , string f1LehimSonraBeklemeN = "", string f1LehimMiktarN = "", string f1LehimHizN = "", string f1KomponentYuksekligiN = "",
                           string f1XBitisPozisyonN = "", string f1YBitisPozisyonN = "", string f1XYHareketHizN = "", string f1OndenLehimMiktariN = "")
        {
            bindingSource.Add(new Setup()
            {
                POS = PosN,
                Lehim_Akis = f1LehimAkisN,
                X_Pozisyon_1 = f1XPozisyon1N,
                Y_Pozisyon_1 = f1YPozisyon1N,
                W_Pozisiyon = f1WPosN,
                Z_Pozisiyon = f1ZPosN,
                Lehim_Oncesi_Bekleme = f1LehimOnceBeklemeN,
                Lehim_Sonrasi_Bekleme = f1LehimSonraBeklemeN,
                Lehim_Miktar = f1LehimMiktarN,
                Lehim_Hiz = f1LehimHizN,
                Komponent_Yuksekligi = f1KomponentYuksekligiN,
                X_Bitis_Pozisyon = f1XBitisPozisyonN,
                Y_Bitis_Pozisyon = f1YBitisPozisyonN,
                XY_Hareket_Hızı = f1XYHareketHizN,
                Onden_Lehim_Miktarı = f1OndenLehimMiktariN,
            });
        }
        private void newSetupDataF2(BindingSource bindingSource, string PosN = "", string f1LehimAkisN = "", string f1XPozisyon1N = "", string f1YPozisyon1N = "",
                         string f1WPosN = "", string f1ZPosN = "", string f1LehimOnceBeklemeN = "", string f1LehimSonraBeklemeN = "",
                         string f1LehimMiktarN = "", string f1LehimHizN = "", string f2KomponentYuksekligiN = "", string f1XBitisPozisyonN = "", string f1YBitisPozisyonN = "",
                         string f1XYHareketHizN = "", string f1OndenLehimMiktariN = "")

        {
            bindingSource.Add(new SetupF2()
            {
                POS = PosN,
                Lehim_Akis = f1LehimAkisN,
                X_Pozisyon_1 = f1XPozisyon1N,
                Y_Pozisyon_1 = f1YPozisyon1N,
                W_Pozisiyon = f1WPosN,
                Z_Pozisiyon = f1ZPosN,
                Lehim_Oncesi_Bekleme = f1LehimOnceBeklemeN,
                Lehim_Sonrasi_Bekleme = f1LehimSonraBeklemeN,
                Lehim_Miktar = f1LehimMiktarN,
                Lehim_Hiz = f1LehimHizN,
                Komponent_Yuksekligi = f2KomponentYuksekligiN,
                X_Bitis_Pozisyon = f1XBitisPozisyonN,
                Y_Bitis_Pozisyon = f1YBitisPozisyonN,
                XY_Hareket_Hızı = f1XYHareketHizN,
                Onden_Lehim_Miktarı = f1OndenLehimMiktariN
            });
        }
        private void getDGVSetupRowData()
        {
            for (int j = 1; j < dataGridViewSetupF1.ColumnCount; j++)
            {
                MessageBox.Show(dataGridViewSetupF1.Rows[pos].Cells[j].Value.ToString());
            }
        }
        private void dataGridViewListOrder(DataGridView dataGridView)
        {
            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                dataGridView.Rows[i].Cells[0].Value = (i + 1);
                dGVPaintRow(dataGridView, i);
            }
        }
        private void dataGridViewSetupF1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewComboBoxEditingControl)
            {
                cbec = e.Control as DataGridViewComboBoxEditingControl;
                cbec.SelectedIndexChanged -= Cbec_SelectedIndexChanged;
                cbec.SelectedIndexChanged += Cbec_SelectedIndexChanged;
            }
        }
        private void dataGridViewSetupF2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewComboBoxEditingControl)
            {
                cbecF2 = e.Control as DataGridViewComboBoxEditingControl;
                cbecF2.SelectedIndexChanged -= CbecF2_SelectedIndexChanged;
                cbecF2.SelectedIndexChanged += CbecF2_SelectedIndexChanged;
            }
        }
        private void Cbec_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbec != null)
            {
                if (cbec.SelectedItem.ToString() == "Sürekli")
                {
                    disableDGVSetupRowData(dataGridViewSetupF1, false, cbec.EditingControlRowIndex, 11, 12, 13);
                }
                else
                {
                    disableDGVSetupRowData(dataGridViewSetupF1, true, cbec.EditingControlRowIndex, 11, 12, 13);
                }

            }
        }
        private void CbecF2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbecF2 != null)
            {
                if (cbecF2.SelectedItem.ToString() == "Sürekli")
                {
                    disableDGVSetupRowData(dataGridViewSetupF2, false, cbecF2.EditingControlRowIndex, 11, 12, 13);
                }
                else
                {
                    disableDGVSetupRowData(dataGridViewSetupF2, true, cbecF2.EditingControlRowIndex, 11, 12, 13);
                }

            }
        }
        private void dGVPaintRow(DataGridView dataGridView, int rowPos)
        {
            if (rowPos % 2 != 0)
            {
                for (int i = 0; i < dataGridView.ColumnCount; i++)
                {
                    dataGridView.Rows[rowPos].Cells[i].Style.BackColor = Color.LightBlue;
                }
            }
            else
            {
                for (int i = 0; i < dataGridView.ColumnCount; i++)
                {
                    dataGridView.Rows[rowPos].Cells[i].Style.BackColor = Color.White;
                }
            }
        }
        private void enableObject(bool flag, params dynamic[] objectArray)
        {
            foreach (var myObject in objectArray)
            {
                myObject.Enabled = flag;
            }
        }
        private void disableDGVSetupRowData(DataGridView dataGridView, bool flag, params int[] j)
        {
            // int posDGVSetupIndex = dataGridView.RowCount - 1;
            dataGridView.Rows[j[0]].Cells[0].ReadOnly = true;

            if (flag)
            {
                dataGridView.Rows[j[0]].Cells[j[1]].ReadOnly = true;
                dataGridView.Rows[j[0]].Cells[j[2]].ReadOnly = true;
                dataGridView.Rows[j[0]].Cells[j[3]].ReadOnly = true;
            }
            else
            {
                dataGridView.Rows[j[0]].Cells[j[1]].ReadOnly = false;
                dataGridView.Rows[j[0]].Cells[j[2]].ReadOnly = false;
                dataGridView.Rows[j[0]].Cells[j[3]].ReadOnly = false;
            }


        }

        /****************************************************************************************/
        /*** Common jog methods ***/
        private void jogSpeedValue(NumericUpDown numericUpDown0, NumericUpDown numericUpDown1, NumericUpDown numericUpDown2)
        {
            numericUpDown2.Value = numericUpDown1.Value = numericUpDown0.Value;
        }
        private void jogComboBoxValue(ComboBox comboBox0, ComboBox comboBox1, ComboBox comboBox2)
        {
            comboBox2.Text = comboBox1.Text = comboBox0.Text;
        }
        private void jogSpeedSendText(ComboBox comboBox0, ComboBox comboBox1, ComboBox comboBox2)
        {
            jogComboBoxValue(comboBox0, comboBox1, comboBox2);
            if (comboBox0.Text == "Sürekli")
            {
                nxCompoletBoolWrite("jogContinuous", true);
                nxCompoletBoolWrite("jog01", false);
                nxCompoletBoolWrite("jog05", false);
                nxCompoletBoolWrite("jog1", false);
            }
            else if (comboBox0.Text == "0.1 mm")
            {
                nxCompoletBoolWrite("jog01", true);
                nxCompoletBoolWrite("jogContinuous", false);
                nxCompoletBoolWrite("jog05", false);
                nxCompoletBoolWrite("jog1", false);
            }
            else if (comboBox0.Text == "0.5 mm")
            {
                nxCompoletBoolWrite("jog05", true);
                nxCompoletBoolWrite("jogContinuous", false);
                nxCompoletBoolWrite("jog01", false);
                nxCompoletBoolWrite("jog1", false);
            }
            else
            {
                nxCompoletBoolWrite("jog1", true);
                nxCompoletBoolWrite("jogContinuous", false);
                nxCompoletBoolWrite("jog05", false);
                nxCompoletBoolWrite("jog01", false);
            }

        }


        /*** Program select methods ***/
        private void selectPath(TextBox textBox)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bat Files (BAT)|*.BAT;";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = openFileDialog.FileName;
            }

        }
        private void addProgram(TextBox textBox, ComboBox comboBox, BindingList<string> list)
        {
            if (textBox.Text != "" && comboBox.Text != "")
            {
                if (list.Contains(comboBox.Text))
                {
                    MessageBox.Show("Aynı isimli program kaydedilemez");
                }
                else
                {
                    INIKaydet iNIKaydet = new INIKaydet(desktopPath + @"/Programlar/ProgramF1.ini");
                    iNIKaydet.Yaz(comboBox.Text, "Program Yolu", textBox.Text);
                    list.Add(comboBox.Text);
                    Setting.Default.Save();
                    MessageBox.Show("Kaydedildi");
                    comboBox.DataSource = list;
                }

            }
        }
        private void cmbBxFileNameF1_SelectedIndexChanged(object sender, EventArgs e)
        {
            INIKaydet ini = new INIKaydet(desktopPath + @"/Programlar/ProgramF1.ini");
            txtFilePathF1.Text = ini.Oku(cmbBxFileNameF1.Text, "Program Yolu");
            programingPathF1 = txtFilePathF1.Text;
        }
        private void cmbBxFileNameF2_SelectedIndexChanged(object sender, EventArgs e)
        {
            INIKaydet ini = new INIKaydet(desktopPath + @"/Programlar/ProgramF1.ini");
            txtFilePathF2.Text = ini.Oku(cmbBxFileNameF2.Text, "Program Yolu");
            programingPathF2 = txtFilePathF2.Text;
        }
        private void btnAddProgramF1_Click(object sender, EventArgs e)
        {
            addProgram(txtFilePathF1, cmbBxFileNameF1, Setting.Default.programListF1);
        }
        private void btnAddProgramF2_Click(object sender, EventArgs e)
        {
            addProgram(txtFilePathF2, cmbBxFileNameF2, Setting.Default.programListF1);
        }
        private void btnSelectF1_Click(object sender, EventArgs e)
        {
            selectPath(txtFilePathF1);
        }
        private void btnSelectF2_Click(object sender, EventArgs e)
        {
            selectPath(txtFilePathF2);
        }
        private void btnPrgTestF1_Click(object sender, EventArgs e)
        {

            callF1Start();

        }
        private void btnPrgTestF2_Click(object sender, EventArgs e)
        {
            callF2Start();
        }
        private void callF1Start()
        {
            if (nxCompoletBoolRead("bakimMod") && boolReadStatus == false)
            {
                programingPathF1 = txtFilePathF1.Text; ;
                Programming1Frm.programming1Start();
            }
            else
            {
                MessageBox.Show("Lütfen bakım modunu açınız");
            }
        }
        private void callF2Start()
        {
            if (nxCompoletBoolRead("bakimMod") && boolReadStatus == false)
            {//f2ProgramOK
                programingPathF2 = txtFilePathF2.Text; ;
                Programming2Frm.programming2Start();
            }
            else
            {
                MessageBox.Show("Lütfen bakım modunu açınız");
            }
        }

        /****************************************************************************************/
        /*** Jog Buttons Click Methods ***/

        private void btnRightF1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("xAxisForward", true);
        }
        private void btnRightF1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("xAxisForward", false);
        }
        private void btnLeftF1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("xAxisBackward", true);
        }
        private void btnLeftF1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("xAxisBackward", false);
        }
        private void btnUpZF1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("zAxisForward", true);
        }
        private void btnUpZF1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("zAxisForward", false);
        }
        private void btnDownZF1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("zAxisBackward", false);
        }
        private void btnDownZF1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("zAxisBackward", true);
        }
        private void btnUpY1F1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("y1AxisForward", true);
        }
        private void btnUpY1F1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("y1AxisForward", false);
        }
        private void btnDownY1F1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("y1AxisBackward", true);
        }
        private void btnDownY1F1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("y1AxisBackward", false);

        }
        private void btnUpY2F1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("y2AxisForward", true);
        }
        private void btnUpY2F1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("y2AxisForward", false);
        }
        private void btnUsbF1_Click(object sender, EventArgs e)
        {
            threadProcess = new Thread(() => btnUsbF1Click());
            threadProcess.Start();
        }
        private void btnUsbF1Click()
        {
            if (nxCompoletBoolRead("bakimMod") && boolReadStatus == false)
            {
                if (nxCompoletBoolRead("f1Usb") && boolReadStatus == false)
                {
                    btnUsbF1.Text = "USB AÇ";
                    btnUsbF1.BackColor = Color.Green;
                    nxCompoletBoolWrite("f1Usb", false);
                }
                else if (!nxCompoletBoolRead("f1Usb") && boolReadStatus == false)
                {
                    btnUsbF1.Text = "USB KAPAT";
                    btnUsbF1.BackColor = Color.Red;
                    if (nxCompoletBoolRead("f2Usb"))
                    {
                        nxCompoletBoolWrite("f2Usb", false);
                        btnUsbF2.Text = "USB AÇ";
                        btnUsbF2.BackColor = Color.Green;
                    }
                    nxCompoletBoolWrite("f1Usb", true);
                }
            }
            else
            {
                MessageBox.Show("Lütfen bakım modunu açınız");
            }
        }
        private void btnUsbF2_Click(object sender, EventArgs e)
        {
            threadProcess = new Thread(() => btnUsbF2Click());
            threadProcess.Start();

        }
        private void btnUsbF2Click()
        {
            if (nxCompoletBoolRead("bakimMod") && boolReadStatus == false)
            {
                if (nxCompoletBoolRead("f2Usb") && boolReadStatus == false)
                {
                    btnUsbF2.Text = "USB AÇ";
                    btnUsbF2.BackColor = Color.Green;
                    nxCompoletBoolWrite("f2Usb", false);
                }
                else if (!nxCompoletBoolRead("f2Usb") && boolReadStatus == false)
                {
                    btnUsbF2.Text = "USB KAPAT";
                    btnUsbF2.BackColor = Color.Red;
                    if (nxCompoletBoolRead("f1Usb"))
                    {
                        nxCompoletBoolWrite("f1Usb", false);
                        btnUsbF1.Text = "USB AÇ";
                        btnUsbF1.BackColor = Color.Green;
                    }
                    nxCompoletBoolWrite("f2Usb", true);
                }
            }
            else
            {
                MessageBox.Show("Lütfen bakım modunu açınız");
            }
        }
        private bool dgvMinMaxControl(DataGridView dataGridView, int col, int row, double[] min, double[] max)
        {
            bool isNumeric = float.TryParse((string)dataGridView[col, row].Value, out _);

            if (isNumeric && float.Parse(dataGridView[col, row].Value.ToString().Replace(".", ",")) <= max[col] && float.Parse(dataGridView[col, row].Value.ToString().Replace(".", ",")) >= min[col])
            {
                return false;
            }
            else
            {
                MessageBox.Show(min[col].ToString() + " - " + max[col].ToString() + " değer aralığında giriniz");
                return true;
            }


        }
        private void dataGridViewSetupF1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (dataGridViewSetupF1.RowCount > 0)
            {
                colUserF1 = dataGridViewSetupF1.CurrentCell.ColumnIndex;
                rowUserF1 = dataGridViewSetupF1.CurrentCell.RowIndex;
                DataGridViewCell tc = dataGridViewSetupF1[colUserF1, rowUserF1];
                dgvValueF1 = (string)tc.Value;
                if (dgvValueF1 != "" && dgvValueF1 != null)
                {
                    if (dgvValueF1.Contains(","))
                    {
                        dataGridViewSetupF1[colUserF1, rowUserF1].Value = tc.Value.ToString().Replace(",", ".");
                        if (dgvMinMaxControl(dataGridViewSetupF1, colUserF1, rowUserF1, minF1, maxF1))
                        {
                            dataGridViewSetupF1[colUserF1, rowUserF1].Value = "";
                        }
                    }
                    else if (colUserF1 > 1 && !startFlag && (string)dataGridViewSetupF1[colUserF1, rowUserF1].Value != "Sürekli" && (string)dataGridViewSetupF1[colUserF1, rowUserF1].Value != "Noktasal")
                    {
                        if (dgvMinMaxControl(dataGridViewSetupF1, colUserF1, rowUserF1, minF1, maxF1))
                        {
                            dataGridViewSetupF1[colUserF1, rowUserF1].Value = "";
                            startFlag = true;
                        }
                    }
                    startFlag = false;
                }

            }

        }

        private void dataGridViewSetupF2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewSetupF2.RowCount > 0)
            {
                colUserF2 = dataGridViewSetupF2.CurrentCell.ColumnIndex;
                rowUserF2 = dataGridViewSetupF2.CurrentCell.RowIndex;
                DataGridViewCell tc = dataGridViewSetupF2[colUserF2, rowUserF2];
                dgvValueF2 = (string)tc.Value;
                if (dgvValueF2 != "" && dgvValueF2 != null)
                {
                    if (dgvValueF2.Contains(","))
                    {
                        dataGridViewSetupF2[colUserF2, rowUserF2].Value = tc.Value.ToString().Replace(",", ".");
                        if (dgvMinMaxControl(dataGridViewSetupF2, colUserF2, rowUserF2, minF2, maxF2))
                        {
                            dataGridViewSetupF2[colUserF2, rowUserF2].Value = "";
                        }
                    }
                    else if (colUserF2 > 1 && !startFlag2 && (string)dataGridViewSetupF2[colUserF2, rowUserF2].Value != "Sürekli" && (string)dataGridViewSetupF2[colUserF2, rowUserF2].Value != "Noktasal")
                    {
                        if (dgvMinMaxControl(dataGridViewSetupF2, colUserF2, rowUserF2, minF2, maxF2))
                        {
                            dataGridViewSetupF2[colUserF2, rowUserF2].Value = "";
                            startFlag2 = true;
                        }
                    }
                    startFlag2 = false;
                }

            }

        }
        private void btnLhmWork_Click(object sender, EventArgs e)
        {
            threadWriteBool = new Thread(() => nxCompoletBoolWrite("lehimManuel", true));
            threadWriteBool.Start();
        }
        private void btnLhmClearF1_Click(object sender, EventArgs e)
        {
            if (nxCompoletBoolRead("bakimMod") && boolReadStatus == false)
            {
                threadWriteBool = new Thread(() => nxCompoletBoolWrite("lehimTemizle", true));
                threadWriteBool.Start();
            }
            else
            {
                MessageBox.Show("Lütfen bakım modunu açınız");
            }
        }
        private void btnHavyaF1_Click(object sender, EventArgs e)
        {
            if (nxCompoletBoolRead("bakimMod") && boolReadStatus == false)
            {
                threadWriteBool = new Thread(() => nxCompoletBoolWrite("havyaButton", true));
                threadWriteBool.Start();
            }
            else
            {
                MessageBox.Show("Lütfen bakım modunu açınız");
            }
        }
        private void btnDownY2F1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("y2AxisBackward", true);
        }
        private void btnDownY2F1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("y2AxisBackward", false);
        }
        private void btnDownWF1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("wAxisBackward", true);
        }
        private void btnDownWF1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("wAxisBackward", false);
        }
        private void btnUpWF1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("wAxisForward", true);
        }
        private void btnUpWF1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("wAxisForward", false);
        }
        private void btnDownPrF1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("progAxisBackward", true);
        }
        private void btnDownPrF1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("progAxisBackward", false);
        }
        private void btnUpPrF1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("progAxisForward", true);
        }
        private void btnUpPrF1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("progAxisForward", false);
        }
        private void btnUpLhF1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("lehimAxisForward", true);
        }
        private void btnUpLhF1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("lehimAxisForward", false);
        }
        private void btnDownLhF1_MouseDown(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("lehimAxisBackward", true);
        }
        private void btnDownLhF1_MouseUp(object sender, MouseEventArgs e)
        {
            nxCompoletBoolWrite("lehimAxisBackward", false);
        }

        /*** Setup Piston Butttons Methods ***/

        private void btnP1F1_Click(object sender, EventArgs e)
        {
            if (nxCompoletBoolRead("havyaZPistonOk") && boolReadStatus == false)
            {
                nxCompoletBoolWrite("havyaZPiston", false);
            }
        }

        private void btnP2F1_Click(object sender, EventArgs e)
        {
            if (nxCompoletBoolRead("havyaZPistonOk") && boolReadStatus == false)
            {
                nxCompoletBoolWrite("havyaZPiston", true);
            }
        }

        private void btnP3F1_Click(object sender, EventArgs e)
        {
            if (nxCompoletBoolRead("lehimZPistonOk") && boolReadStatus == false)
            {
                nxCompoletBoolWrite("lehimZPiston", false);
            }
        }

        private void btnP4F1_Click(object sender, EventArgs e)
        {
            if (nxCompoletBoolRead("lehimZPistonOk") && boolReadStatus == false)
            {
                nxCompoletBoolWrite("lehimZPiston", true);
            }
        }

        private void btnP5F1_Click(object sender, EventArgs e)
        {
            if (nxCompoletBoolRead("program1PistonOk") && boolReadStatus == false)
            {
                nxCompoletBoolWrite("program1Piston", false);
            }
        }

        private void btnP6F1_Click(object sender, EventArgs e)
        {
            if (nxCompoletBoolRead("program1PistonOk") && boolReadStatus == false)
            {
                nxCompoletBoolWrite("program1Piston", true);
            }
        }

        private void btnP7F1_Click(object sender, EventArgs e)
        {
            if (nxCompoletBoolRead("program2PistonOk") && boolReadStatus == false)
            {
                nxCompoletBoolWrite("program2Piston", false);
            }
        }

        private void btnP8F1_Click(object sender, EventArgs e)
        {
            if (nxCompoletBoolRead("program2PistonOk") && boolReadStatus == false)
            {
                nxCompoletBoolWrite("program2Piston", true);
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            threadProcess = new Thread(btnHomeClickEvent);
            threadProcess.Start();
        }
        private void btnHomeClickEvent()
        {
            if (nxCompoletBoolRead("homeButtonOk") && boolReadStatus == false)
            {
                nxCompoletBoolWrite("homeState", true);
            }
        }

        private void btnMotorClear_Click(object sender, EventArgs e)
        {
            threadProcess = new Thread(() => btnMotorClearClick());
            threadProcess.Start();
        }
        private void btnResetSycF1_Click(object sender, EventArgs e)
        {
            threadWriteBool = new Thread(() => nxCompoletBoolWrite("f1SayacReset", true));
            threadWriteBool.Start();
            lblSayacReset(lblM1F1, lblM2F1, lblM3F1, lblM4F1);
        }
        private void btnResetSycF2_Click(object sender, EventArgs e)
        {
            threadWriteBool = new Thread(() => nxCompoletBoolWrite("f2SayacReset", true));
            threadWriteBool.Start();
            lblSayacReset(lblM1F2, lblM2F2, lblM3F2, lblM4F2);
        }
        private void lblSayacReset(params Label[] labels)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].Text = "0";
            }
        }
        private void btnMotorClearClick()
        {
            if (nxCompoletBoolRead("cleaningMotorStartOk") && boolReadStatus == false)
            {
                if (nxCompoletBoolRead("cleaningMotorStart") && boolReadStatus == false)
                {
                    nxCompoletBoolWrite("cleaningMotorStart", false);
                    btnMotorClearF1.Text = "Temizle Aç";
                }
                else if (boolReadStatus == false)
                {
                    nxCompoletBoolWrite("cleaningMotorStart", true);
                    btnMotorClearF1.Text = "Temizle Kapa";
                }
            }
        }

        private void btnOto_Click(object sender, EventArgs e)
        {
            threadProcess = new Thread(() => btnOtoClick());
            threadProcess.Start();

        }

        private void btnOtoClick()
        {
            automodFlag = true;
            if (nxCompoletBoolRead("autoMod") && boolReadStatus == false)
            {

                nxCompoletBoolWrite("automod", false);
                btnOto.Text = "OTOMATİK PASİF";
                btnOto.BackColor = Color.Red;
            }
            else if (boolReadStatus == false)
            {
                nxCompoletBoolWrite("automod", true);
                btnOto.Text = "OTOMATİK AKTİF";
                btnOto.BackColor = Color.Green;
            }
            automodFlag = false;
        }

        private void btnBkmF1_Click(object sender, EventArgs e)
        {
            bakimModFlag = true;

            if (!nxCompoletBoolRead("bakimMod") && boolReadStatus == false)
            {
                //nxCompoletBoolWrite("bakimMod", true);
                threadWriteBool = new Thread(() => nxCompoletBoolWrite("bakimMod", true));
                threadWriteBool.Start();
                btnBkmF1.Text = "Bakım Mod Aktif";
                btnBkmF1.BackColor = Color.Green;
                btnBkmF2.Text = "Bakım Mod Aktif";
                btnBkmF2.BackColor = Color.Green;
                btnManBkm.Text = "Bakım Mod Aktif";
                btnManBkm.BackColor = Color.Green;
            }

            else if (nxCompoletBoolRead("bakimMod") && boolReadStatus == false)
            {
                //nxCompoletBoolWrite("bakimMod", false);
                threadWriteBool = new Thread(() => nxCompoletBoolWrite("bakimMod", false));
                threadWriteBool.Start();
                btnBkmF1.Text = "Bakım Mod Pasif";
                btnBkmF1.BackColor = Color.Red;
                btnBkmF2.Text = "Bakım Mod Pasif";
                btnBkmF2.BackColor = Color.Red;
                btnManBkm.Text = "Bakım Mod Pasif";
                btnManBkm.BackColor = Color.Red;
            }
            bakimModFlag = false;

        }

        private void txtJogSpeedF1_ValueChanged(object sender, EventArgs e)
        {
            threadWriteString = new Thread(() => nxCompoletStringWrite("jogHiz", txtJogSpeedF1.Value.ToString()));
            threadWriteString.Start();
            jogSpeedValue(txtJogSpeedF1, txtJogSpeedF2, txtManJogSpeed);
        }
        private void txtJogSpeedF2_ValueChanged(object sender, EventArgs e)
        {
            threadWriteString = new Thread(() => nxCompoletStringWrite("jogHiz", txtJogSpeedF2.Value.ToString()));
            threadWriteString.Start();
            jogSpeedValue(txtJogSpeedF2, txtManJogSpeed, txtJogSpeedF1);
        }
        private void txtManJogSpeed_ValueChanged(object sender, EventArgs e)
        {
            threadWriteString = new Thread(() => nxCompoletStringWrite("jogHiz", txtManJogSpeed.Value.ToString()));
            threadWriteString.Start();
            jogSpeedValue(txtManJogSpeed, txtJogSpeedF2, txtJogSpeedF1);
        }

        private void cmbBxJogSpacingF1_SelectedValueChanged(object sender, EventArgs e)
        {

            threadProcess = new Thread(() => jogSpeedSendText(cmbBxJogSpacingF1, cmbBxJogSpacingF2, cmbBxManJopSpacing));
            threadProcess.Start();
        }
        private void cmbBxJogSpacingF2_SelectedValueChanged(object sender, EventArgs e)
        {
            threadProcess = new Thread(() => jogSpeedSendText(cmbBxJogSpacingF2, cmbBxJogSpacingF1, cmbBxManJopSpacing));
            threadProcess.Start();
        }
        private void cmbBxManJopSpacing_SelectedValueChanged(object sender, EventArgs e)
        {
            threadProcess = new Thread(() => jogSpeedSendText(cmbBxManJopSpacing, cmbBxJogSpacingF1, cmbBxJogSpacingF2));
            threadProcess.Start();
        }


        /*** Setup Buttons Click Methods ***/
        private void btnNormal_MouseHover(object sender, EventArgs e)
        {
            btnNormal.BackgroundImage = (Image)Resources.blueBHover;
        }
        private void btnNormal_MouseLeave(object sender, EventArgs e)
        {
            btnNormal.BackgroundImage = (Image)Resources.blueB;

        }
        private void btnMin_MouseHover(object sender, EventArgs e)
        {
            btnMin.BackgroundImage = (Image)Resources.greenBHover;
        }
        private void btnMin_MouseLeave(object sender, EventArgs e)
        {
            btnMin.BackgroundImage = (Image)Resources.greenB;
        }
        private void exit_MouseHover(object sender, EventArgs e)
        {
            exit.BackgroundImage = (Image)Resources.redBHover;

        }
        private void exit_MouseLeave(object sender, EventArgs e)
        {
            exit.BackgroundImage = (Image)Resources.redB;
        }
        private void btnMin_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void btnNormal_Click(object sender, EventArgs e)
        {

            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;

            }
            else
            {

                WindowState = FormWindowState.Normal;
                this.Height = 800;
                this.Width = 1400;
                StartPosition = FormStartPosition.CenterScreen;

            }
        }
        private void exit_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Dispose();
            Application.Exit();
        }
        private void btnNewModel_Click(object sender, EventArgs e)
        {
            txtModelNameF1.Enabled = true;
            txtModelNameF1.Text = "";
            txtMainModelF1.Text = "";
            newModel(dataGridViewSetupF1, cmbBxFileNameF1, listLehim, lhmTxtPCBXAdtF1, lhmTxtPCBYAdtF1, lhmTxtPnlXAdtF1, lhmTxtPnlYAdtF1, txtPrg7F1, txtPrg8F1, txtPrg13F1, txtPrg14F1,
                txtFilePathF1, txtPrg1F1, txtPrg2F1, txtPrg3F1, txtPrg4F1, txtPrg5F1, txtPrg6F1, txtPrg9F1, txtPrg10F1, txtPrg11F1, txtPrg12F1,
                lhmTxtPCBXP2F1, lhmTxtPCBYP2F1, lhmTxtPCBXP3F1, lhmTxtPCBYP3F1, lhmTxtPnlXP2F1, lhmTxtPnlYP2F1, lhmTxtPnlXP3F1, lhmTxtPnlYP3F1);
        }
        private void btnNewModelF2_Click(object sender, EventArgs e)
        {
            txtModelNameF2.Enabled = true;
            txtModelNameF2.Text = "";
            txtMainModelF2.Text = "";
            newModel(dataGridViewSetupF2, cmbBxFileNameF2, listLehim, lhmTxtPCBXAdtF1, lhmTxtPCBYAdtF2, lhmTxtPnlXAdtF2, lhmTxtPnlYAdtF2, txtPrg7F2, txtPrg8F2, txtPrg13F2, txtPrg14F2,
            txtFilePathF2, txtPrg1F2, txtPrg2F2, txtPrg3F2, txtPrg4F2, txtPrg5F2, txtPrg6F2, txtPrg9F2, txtPrg10F2, txtPrg11F2, txtPrg12F2,
             lhmTxtPCBXP2F2, lhmTxtPCBYP2F2, lhmTxtPCBXP3F2, lhmTxtPCBYP3F2, lhmTxtPnlXP2F2, lhmTxtPnlYP2F2, lhmTxtPnlXP3F2, lhmTxtPnlYP3F2);
        }
        private void newModel(DataGridView dataGridView, ComboBox comboBox, List<string> list, NumericUpDown numericUpDown1, NumericUpDown numericUpDown2,
            NumericUpDown numericUpDown3, NumericUpDown numericUpDown4, NumericUpDown numericUpDown5, NumericUpDown numericUpDown6,
            NumericUpDown numericUpDown7, NumericUpDown numericUpDown8, params TextBox[] textBoxes)
        {
            dataGridView.Rows.Clear();
            list.Clear();
            comboBox.Text = "";
            numericUpDown1.Text = "1";
            numericUpDown2.Text = "1";
            numericUpDown3.Text = "1";
            numericUpDown4.Text = "1";
            numericUpDown5.Text = "1";
            numericUpDown6.Text = "1";
            numericUpDown7.Text = "1";
            numericUpDown8.Text = "1";
            for (int i = 0; i < textBoxes.Length; i++)
            {
                textBoxes[i].Text = "";
            }
        }
        private void btnModelSelect_Click(object sender, EventArgs e)
        {
            ModelForm modelForm = new ModelForm(this);
            modelForm.ShowDialog();
        }
        private void btnModelSelectF2_Click(object sender, EventArgs e)
        {
            ModelForm2 modelForm2 = new ModelForm2(this);
            modelForm2.ShowDialog();
        }
        private void checkBtnSolder_CheckedChanged(object sender, EventArgs e)
        {
            lehimlemeActive();
        }
        private void checkBtnProgF1_CheckedChanged(object sender, EventArgs e)
        {
            lehimlemeActive();
        }
        private void checkBtnSolderF2_CheckedChanged(object sender, EventArgs e)
        {
            lehimlemeActiveF2();
        }
        private void checkBtnProgF2_CheckedChanged(object sender, EventArgs e)
        {
            lehimlemeActiveF2();
        }
        private void btnDGVAdd_Click(object sender, EventArgs e)
        {
            flagFikstür = true;
            addDataToGridView(dataGridViewSetupF1, setupBindingSource);
        }
        private void btnDGVAddF2_Click(object sender, EventArgs e)
        {
            flagFikstür = false;
            addDataToGridView(dataGridViewSetupF2, setupF2BindingSource);
        }
        private void btnDGVCute_Click(object sender, EventArgs e)
        {
            if (dataGridViewSetupF1.RowCount > 0)
            {
                setupBindingSource.RemoveCurrent();
                dataGridViewListOrder(dataGridViewSetupF1);
            };
        }
        private void btnDGVCuteF2_Click(object sender, EventArgs e)
        {
            if (dataGridViewSetupF2.RowCount > 0)
            {
                setupF2BindingSource.RemoveCurrent();
                dataGridViewListOrder(dataGridViewSetupF2);
            }
        }

        private void btnDGVCopyF1_Click(object sender, EventArgs e)
        {
            if (pos > -1)
            {
                copyPos = pos;
            }
        }

        private void btnDGVPasteF1_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < dataGridViewSetupF1.ColumnCount; i++)
            {
                dataGridViewSetupF1[i, pos].Value = dataGridViewSetupF1[i, copyPos].Value;
            }
        }
        private void btnDGVCopyF2_Click(object sender, EventArgs e)
        {
            if (pos > -1)
            {
                copyPos = pos;
            }
        }

        private void btnDGVPasteF2_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < dataGridViewSetupF1.ColumnCount; i++)
            {
                dataGridViewSetupF2[i, pos].Value = dataGridViewSetupF2[i, copyPos].Value;
            }
        }

        private void btnModelSaveF1_Click(object sender, EventArgs e)
        {
            if (emptyControl(dataGridViewSetupF1, checkBtnSolderF1, checkBtnProgF1, txtFilePathF1, txtPrg1F1, txtPrg2F1, txtPrg3F1, txtPrg4F1, txtPrg5F1, txtPrg6F1,
                txtPrg9F1, txtPrg10F1, txtPrg11F1, txtPrg12F1, lhmTxtPCBXP2F1, lhmTxtPCBXP3F1, lhmTxtPCBYP3F1,
                lhmTxtPnlXP2F1, lhmTxtPnlYP2F1, lhmTxtPnlXP3F1, lhmTxtPnlYP3F1))
            {
                modelSave(txtModelNameF1, "ModelF1", "Lehimleyici_Modelleri");
            }
            else
            {
                MessageBox.Show("Lütfen alanları boş bırakmayınız");
            }

        }
        private void btnModelSaveF2_Click(object sender, EventArgs e)
        {
            if (emptyControl(dataGridViewSetupF2, checkBtnSolderF2, checkBtnProgF2, txtFilePathF2, txtPrg1F2, txtPrg2F2, txtPrg3F2, txtPrg4F2, txtPrg5F2, txtPrg6F2,
             txtPrg9F2, txtPrg10F2, txtPrg11F2, txtPrg12F2, lhmTxtPCBXP2F2, lhmTxtPCBXP3F2, lhmTxtPCBYP3F2,
             lhmTxtPnlXP2F2, lhmTxtPnlYP2F2, lhmTxtPnlXP3F2, lhmTxtPnlYP3F2))
            {
                modelSave(txtModelNameF2, "ModelF2", "Lehimleyici_Modelleri2");
            }
            else
            {
                MessageBox.Show("Lütfen alanları boş bırakmayınız");
            }

        }


        /*************************************************************************************************************************/
        /*** Main Click Event Read  ****/
        private void txtMachineSpeed_TextChanged(object sender, EventArgs e)
        {
            threadWriteString = new Thread(() => nxCompoletStringWrite("machineSpeed", txtMachineSpeed.Text));
            threadWriteString.Start();
        }

        /******************************************* Camera API ************************************************************/
        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap1 = (Bitmap)eventArgs.Frame.Clone();
            bitmap1.RotateFlip(RotateFlipType.Rotate180FlipY);
            if (xtraTabControl1.SelectedTabPageIndex == 0)
            {

                if (!videoFlag && pctrBxCameraMain.BackgroundImage != null)
                {
                    pctrBxCameraMain.BackgroundImage.Dispose();
                }


                pctrBxCameraMain.BackgroundImage = bitmap1;
            }
            else if (!videoFlag && xtraTabControl1.SelectedTabPageIndex == 1 && tabControl1.SelectedIndex == 0)
            {

                if (pctrBxCameraF1.BackgroundImage != null)
                {
                    pctrBxCameraF1.BackgroundImage.Dispose();
                }

                pctrBxCameraF1.BackgroundImage = bitmap1;
            }
            else if (!videoFlag && xtraTabControl1.SelectedTabPageIndex == 1 && tabControl1.SelectedIndex == 1)
            {

                if (pctrBxCameraF2.BackgroundImage != null)
                {
                    pctrBxCameraF2.BackgroundImage.Dispose();
                }

                pctrBxCameraF2.BackgroundImage = bitmap1;
            }
            else if (!videoFlag && xtraTabControl1.SelectedTabPageIndex == 2)
            {
                if (pctrBxCameraMan.BackgroundImage != null)
                {
                    pctrBxCameraMan.BackgroundImage.Dispose();
                }
                pctrBxCameraMan.BackgroundImage = bitmap1;
            }
            else if (!videoFlag && xtraTabControl1.SelectedTabPageIndex == 3)
            {
                if (pctrBxPg4Camera.BackgroundImage != null)
                {
                    pctrBxPg4Camera.BackgroundImage.Dispose();
                }
                pctrBxPg4Camera.BackgroundImage = bitmap1;
            }

        }

        private void SourceCamera()
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                comboBoxCameraSource.Items.Add(filterInfo.Name);
            if (comboBoxCameraSource.Items.Count > 0)
            {
                comboBoxCameraSource.SelectedIndex = 0;
                videoCaptureDevice = new VideoCaptureDevice();

                videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[comboBoxCameraSource.SelectedIndex].MonikerString);
                videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
                videoCaptureDevice.VideoResolution = videoCaptureDevice.VideoCapabilities[0];
                videoCaptureDevice.Start();
                threadVideo.Start();
                cameraOpen = true;
            }

        }

        /*************************************************************************************************************************/


        /******************************************* Compolet Methods ************************************************************/

        public string nxCompoletStringRead(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead2(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead3(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead4(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead5(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead6(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead7(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead8(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead9(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead10(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead11(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead12(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);;
                return "error";
            }

        }
        public string nxCompoletStringRead13(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead14(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead15(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead16(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead17(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead18(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }

        public string nxCompoletStringRead19(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead20(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead21(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead22(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }

        }
        public string nxCompoletStringRead23(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                //otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringRead" + "\nvariable = " + variable, Color.Red);
                return "error";
            }
        }
        public bool nxCompoletStringWrite(string variable, string value)  //NX STRING
        {
            try
            {
                nxCompolet1.WriteVariable(variable, value);
                return true;
            }
            catch
            {
                // otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : StringWrite" + "\nvariable = " + variable + "\nstate = " + value, Color.Red);
                // MessageBox.Show(e.ToString());
                return false;
            }
        }

        public bool nxCompoletBoolWrite(string variable, bool value)  //NX WRITE
        {

            try
            {

                nxCompolet1.WriteVariable(variable, value);
                return true;
            }
            catch
            {
                otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : BoolWrite" + "\nvariable = " + variable + "\nstate = " + value, Color.Red);
                return false;
            }
        }
        public bool nxCompoletBoolRead(string variable)  //NX READ
        {
            try
            {
                boolReadStatus = false;
                bool staticValue = Convert.ToBoolean(nxCompolet1.ReadVariable(variable));
                return staticValue;
            }
            catch
            {
                boolReadStatus = true;
                otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : BoolRead" + "\nvariable = " + variable, Color.Red); ;
                return false;
            }
        }
        public string nxCompoletDoubleRead(string variable)  //NX STRING
        {
            try
            {
                string s = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return s;
            }
            catch (Exception e)
            {
                // otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : DoubleRead" + "\nvariable = " + variable, Color.Red);
                return "-1";
            }
        }
        public string nxCompoletDoubleRead2(string variable)  //NX STRING
        {
            try
            {
                string s = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return s;
            }
            catch (Exception e)
            {
                // otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : DoubleRead" + "\nvariable = " + variable, Color.Red);
                return "-1";
            }
        }
        public string nxCompoletDoubleRead3(string variable)  //NX STRING
        {
            try
            {
                string s = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return s;
            }
            catch (Exception e)
            {
                // otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : DoubleRead" + "\nvariable = " + variable, Color.Red);
                return "-1";
            }
        }
        public string nxCompoletDoubleRead4(string variable)  //NX STRING
        {
            try
            {
                string s = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return s;
            }
            catch (Exception e)
            {
                // otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : DoubleRead" + "\nvariable = " + variable, Color.Red);
                return "-1";
            }
        }
        public string nxCompoletDoubleRead5(string variable)  //NX STRING
        {
            try
            {
                string s = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return s;
            }
            catch (Exception e)
            {
                // otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : DoubleRead" + "\nvariable = " + variable, Color.Red);
                return "-1";
            }
        }
        public string nxCompoletDoubleRead6(string variable)  //NX STRING
        {
            try
            {
                string s = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return s;
            }
            catch (Exception e)
            {
                // otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : DoubleRead" + "\nvariable = " + variable, Color.Red);
                return "-1";
            }
        }




        /********************************************** Diğer Konsol-1 ************************************************/
        private void rtbConsoleOther_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rtb2 = sender as RichTextBox;
            rtb2.SelectionStart = rtb2.Text.Length;
            rtb2.ScrollToCaret();
        }

        /*Kullanıcı Arayüzüne Temizlenir*/
        public void otherConsoleClean()
        {
            if (rtbConsoleOther.InvokeRequired)
            {
                rtbConsoleOther.Invoke(new Action(delegate ()
                {
                    rtbConsoleOther.Text = "";
                    rtbConsoleOther.Select(rtbConsoleOther.TextLength, 0);
                    rtbConsoleOther.SelectionColor = Color.White;
                }));
            }
            else
            {
                rtbConsoleOther.Text = "";
                rtbConsoleOther.Select(rtbConsoleOther.TextLength, 0);
                rtbConsoleOther.SelectionColor = Color.White;
            }
        }

        /*Kullanıcı Arayüzüne Yazı Yazılır*/
        public void otherConsoleAppendLine(string text, Color color)
        {
            /*  if (rtbConsoleOther.InvokeRequired)
              {
                  rtbConsoleOther.Invoke(new Action(delegate ()
                  {
                      rtbConsoleOther.Select(rtbConsoleOther.TextLength, 0);
                      rtbConsoleOther.SelectionColor = color;
                      rtbConsoleOther.AppendText(text + Environment.NewLine);
                      rtbConsoleOther.Select(rtbConsoleOther.TextLength, 0);
                      rtbConsoleOther.SelectionColor = Color.White;
                  }));
              }
              else
              {
                   rtbConsoleOther.Select(rtbConsoleOther.TextLength, 0);
                   rtbConsoleOther.SelectionColor = color;
                   rtbConsoleOther.AppendText(text + Environment.NewLine);
                   rtbConsoleOther.Select(rtbConsoleOther.TextLength, 0);
                   rtbConsoleOther.SelectionColor = Color.White;
              }*/
            //otherConsoleAppendLine_2(text, color);
        }

        /*Kullanıcı Arayüzünde Bir Satır Boşluk Bırakılır*/
        public void otherConsoleNewLine()
        {
            if (rtbConsoleOther.InvokeRequired)
            {
                rtbConsoleOther.Invoke(new Action(delegate () { rtbConsoleOther.AppendText(Environment.NewLine); }));
            }
            else
            {
                rtbConsoleOther.AppendText(Environment.NewLine);
            }
        }

        /********************************************** Diğer Konsol-2 ************************************************/
        private void rtbConsoleOther_2_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rtb2 = sender as RichTextBox;
            rtb2.SelectionStart = rtb2.Text.Length;
            rtb2.ScrollToCaret();
        }
        private void btnStartF1_Click(object sender, EventArgs e)
        {
            	threadProcess=new Thread(btnStartF1Click);
                threadProcess.Start();
        }

        private void btnStartF1Click()
        {
            if (nxCompoletBoolRead("f1StartButonOk"))
            {
                if (nxCompoletBoolWrite("f1StartButon", true))
                {
                    nxCompoletStringWrite("f1XpanelBas", txtStartX1F1.Text);
                    nxCompoletStringWrite("f1YpanelBas", txtStartY1F1.Text);
                    nxCompoletStringWrite("f1XpcbBas", txtStartX2F1.Text);
                    nxCompoletStringWrite("f1YpcbBas", txtStartY2F1.Text);
                    nxCompoletStringWrite("f1PosSayi", txtStartPF1.Text);
                    if (cmbBxLhmAmountF1.SelectedIndex == 0)
                    {
                        nxCompoletStringWrite("f1Lehim", "0");
                    }
                    else
                    {
                        nxCompoletStringWrite("f1Lehim", "1");
                    }
                }

            }
        }

        private void btnStartF2_Click(object sender, EventArgs e)
        {
            threadProcess = new Thread(btnStartF2Click);
            threadProcess.Start();
        }

        private void btnStartF2Click()
        {
            if (nxCompoletBoolRead("f2StartButonOk"))
            {
                if (nxCompoletBoolWrite("f2StartButon", true))
                {
                    nxCompoletStringWrite("f2XpanelBas", txtStartX1F2.Text);
                    nxCompoletStringWrite("f2YpanelBas", txtStartY1F2.Text);
                    nxCompoletStringWrite("f2YpanelBitis", tblLytMainManF2.Text);
                    nxCompoletStringWrite("f2XpcbBas", txtStartX2F2.Text);
                    nxCompoletStringWrite("f2YpcbBas", txtStartY2F2.Text);
                    nxCompoletStringWrite("f2PosSayi", txtStartPF2.Text);
                    if (cmbBxLhmAmountF2.SelectedIndex == 0)
                    {
                        nxCompoletStringWrite("f2Lehim", "0");
                    }
                    else
                    {
                        nxCompoletStringWrite("f2Lehim", "1");
                    }
                }

            }
        }

        private void txtStartX1F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            numControl(txtStartX1F1, e);
        }
        private void txtStartX2F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            numControl(txtStartX2F1, e);
        }
        private void txtStartY1F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            numControl(txtStartY1F1, e);
        }
        private void txtStartY2F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            numControl(txtStartY2F1, e);
        }
        private void txtStartPF1_KeyPress(object sender, KeyPressEventArgs e)
        {
            numControl(txtStartPF1, e);
        }
        private void txtStartX1F1_Leave(object sender, EventArgs e)
        {
            minMaxControlMainTextBox(txtStartX1F1, 1, maxMainTxtValue[0]);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!havyaOn)
            {

                sendHavyaDegreeRead();
            }

        }

        private void dataGridViewSetupF1_Scroll(object sender, ScrollEventArgs e)
        {
            stopVideoDtGrdV();

        }

        private void dataGridViewSetupF2_Scroll(object sender, ScrollEventArgs e)
        {
            stopVideoDtGrdV();

        }
        private void stopVideoDtGrdV()
        {
            if (cameraOpen)
            {
                videoCaptureDevice.Stop();
                Thread.Sleep(10);
                videoCaptureDevice.Start();
                videoFlag = false;
            }
        }

        private void txtStartX2F1_Leave(object sender, EventArgs e)
        {
            minMaxControlMainTextBox(txtStartX2F1, 1, maxMainTxtValue[1]);
        }
        private void txtStartY1F1_Leave(object sender, EventArgs e)
        {
            minMaxControlMainTextBox(txtStartY1F1, 1, maxMainTxtValue[2]);
        }
        private void txtStartY2F1_Leave(object sender, EventArgs e)
        {
            minMaxControlMainTextBox(txtStartY2F1, 1, maxMainTxtValue[3]);
        }
        private void txtStartPF1_Leave(object sender, EventArgs e)
        {
            minMaxControlMainTextBox(txtStartPF1, 1, mainPosF1);
        }

        private void numControl(TextBox textBox, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }

        }


        private void txtStartX1F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            numControl(txtStartX1F1, e);
        }

        private void txtStartX2F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            numControl(txtStartX2F2, e);
        }

        private void txtStartY1F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            numControl(txtStartY1F2, e);
        }

        private void txtStartY2F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            numControl(txtStartY2F2, e);
        }

        private void txtStartPF2_KeyPress(object sender, KeyPressEventArgs e)
        {
            numControl(txtStartPF2, e);
        }

        public void otherConsoleClean_2()
        {
            if (rtbConsoleOther_2.InvokeRequired)
            {
                rtbConsoleOther_2.Invoke(new Action(delegate ()
                {
                    rtbConsoleOther_2.Text = "";
                    rtbConsoleOther_2.Select(rtbConsoleOther_2.TextLength, 0);
                    rtbConsoleOther_2.SelectionColor = Color.White;
                }));
            }
            else
            {
                rtbConsoleOther_2.Text = "";
                rtbConsoleOther_2.Select(rtbConsoleOther_2.TextLength, 0);
                rtbConsoleOther_2.SelectionColor = Color.White;
            }
        }

        private void txtStartX1F2_Leave(object sender, EventArgs e)
        {
            minMaxControlMainTextBox(txtStartX1F2, 1, maxMainTxtValueF2[3]);
        }

        private void txtStartX2F2_Leave(object sender, EventArgs e)
        {
            minMaxControlMainTextBox(txtStartX2F2, 1, maxMainTxtValueF2[3]);
        }

        private void txtStartY1F2_Leave(object sender, EventArgs e)
        {
            minMaxControlMainTextBox(txtStartY1F2, 1, maxMainTxtValueF2[3]);
        }

        private void txtStartY2F2_Leave(object sender, EventArgs e)
        {
            minMaxControlMainTextBox(txtStartY2F2, 1, maxMainTxtValueF2[3]);
        }
        private void txtStartPF2_Leave(object sender, EventArgs e)
        {
            minMaxControlMainTextBox(txtStartPF2, 1, mainPosF2);
        }
        private void xtraTabControl1_Click(object sender, EventArgs e)
        {

        }

        private void xtraTabControl1_SelectedPageChanging(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            videoFlag = true;
            counter++;

        }
        private void stopVideo()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (counter > 0)
                {
                    counter--;

                }
                if (videoFlag && counter == 0)
                {
                    videoCaptureDevice.Stop();
                    Thread.Sleep(2000);
                    videoCaptureDevice.Start();
                    videoFlag = false;
                }
            }

        }

        private void sourceHayva()
        {
            string[] ports = SerialPort.GetPortNames();
            cmbBxHvy.Items.AddRange(ports);
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!flagHavyaReceiveData)
            {
                while (serialPort1.BytesToRead > 0)
                {
                    array[counterByte] = Convert.ToByte(serialPort1.ReadByte());
                    counterByte++;
                    Thread.Sleep(100);
                }
                this.Invoke(new EventHandler(getHavyaData));
            }
        }
        private void getHavyaData(object sender, EventArgs e)
        {
            try
            {
                if (counterByte > 5)
                {
                    for (int j = 0; j < counterByte; j++)
                    {
                        havyaDeegreArray[j] += array[j] + " ";
                    }
                    if (havyaDeegreArray[3] != null)
                    {
                        int n = 0;
                        s1 = Convert.ToChar(int.Parse(havyaDeegreArray[3])).ToString();
                        s2 = Convert.ToChar(int.Parse(havyaDeegreArray[4])).ToString();
                        s3 = Convert.ToChar(int.Parse(havyaDeegreArray[5])).ToString();
                        Array.Clear(havyaDeegreArray, 0, havyaDeegreArray.Length);
                        //	if (int.TryParse(s1,out n) && int.TryParse(s2, out n) && int.TryParse(s3, out n))
                        if (s1 != "X")
                        {
                            lblHavyaDeegre.Text = s1 + s2 + s3;
                        }
                    }
                    flagHavyaReceiveData = false;
                    counterByte = 0;
                    Array.Clear(array, 0, array.Length);
                    serialPort1.DiscardInBuffer();
                    serialPort1.DiscardOutBuffer();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("get havya deegre error : " + ex.ToString());
            }

        }
        private void sendHavyaDegreeData(string value)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    flagHavyaReceiveData = true;
                    byte[] byteArray3 = new byte[3];
                    byteArray3 = Encoding.UTF8.GetBytes(value);
                    //byteArray3[0] = 51;
                    //byteArray3[1] = 43;
                    //byteArray3[2] = 47;
                    byteArray[0] = 115;
                    byteArray[1] = 48;
                    byteArray[2] = 49;
                    byteArray[3] = byteArray3[0];
                    byteArray[4] = byteArray3[1];
                    byteArray[5] = byteArray3[2];
                    byteArray[6] = 48;
                    int ss = byteArray[0] + byteArray[1] + byteArray[2] + byteArray[3] + byteArray[4] + byteArray[5] + byteArray[6];
                    ss = ss - 256;
                    byteArray2 = BitConverter.GetBytes(ss);
                    byteArray[7] = byteArray2[0];
                    byteArray[8] = 13;
                    serialPort1.Write(byteArray, 0, 9);
                    flagHavyaReceiveData = false;
                }

                catch (Exception)
                {
                    MessageBox.Show("Havya sıcaklığı gönderilemedi");
                }
            }

        }
        private void sendHavyaDegreeRead()
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    flagHavyaReceiveData = true;
                    byteArray[0] = 73;
                    byteArray[1] = 53;
                    byteArray[2] = 49;
                    byteArray[3] = 13;
                    serialPort1.Write(byteArray, 0, 4);
                    flagHavyaReceiveData = false;

                }

                catch (Exception)
                {
                    MessageBox.Show("Havya sıcaklığı okunamadı");
                }
            }

        }



        public void otherConsoleAppendLine_2(string text, Color color)
        {
            if (rtbConsoleOther_2.InvokeRequired)
            {
                rtbConsoleOther_2.Invoke(new Action(delegate ()
                {
                    rtbConsoleOther_2.Select(rtbConsoleOther_2.TextLength, 0);
                    rtbConsoleOther_2.SelectionColor = color;
                    rtbConsoleOther_2.AppendText(text + Environment.NewLine);
                    rtbConsoleOther_2.Select(rtbConsoleOther_2.TextLength, 0);
                    rtbConsoleOther_2.SelectionColor = Color.White;
                }));
            }
            else
            {
                rtbConsoleOther_2.Select(rtbConsoleOther_2.TextLength, 0);
                rtbConsoleOther_2.SelectionColor = color;
                rtbConsoleOther_2.AppendText(text + Environment.NewLine);
                rtbConsoleOther_2.Select(rtbConsoleOther_2.TextLength, 0);
                rtbConsoleOther_2.SelectionColor = Color.White;
            }
        }
        public void otherConsoleNewLine_2()
        {
            if (rtbConsoleOther_2.InvokeRequired)
            {
                rtbConsoleOther_2.Invoke(new Action(delegate () { rtbConsoleOther_2.AppendText(Environment.NewLine); }));
            }
            else
            {
                rtbConsoleOther_2.AppendText(Environment.NewLine);
            }
        }


        /**********************TextBox Evens***************************************************************/
        private void dotControl(TextBox textBox, KeyPressEventArgs e)
        {
            int countDot = 0;
            for (int i = 0; i < textBox.Text.Length; i++)
            {
                if (textBox.Text.Contains(".") || textBox.Text.Contains(","))
                {
                    countDot++;
                }
            }

            if ((e.KeyChar == '.' || e.KeyChar == ',') && countDot <= 0)
            {
            }
            else if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }

        }
        private void lhmTxtPCBXP2F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPCBXP2F1, e);
        }
        private void lhmTxtPCBYP2F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPCBYP2F1, e);
        }
        private void lhmTxtPCBXP3F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPCBXP3F1, e);
        }
        private void lhmTxtPCBYP3F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPCBYP3F1, e);
        }
        private void lhmTxtPnlXP2F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPnlXP2F1, e);
        }
        private void lhmTxtPnlYP2F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPnlYP2F1, e);
        }
        private void lhmTxtPnlXP3F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPnlXP3F1, e);
        }
        private void lhmTxtPnlYP3F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPnlYP3F1, e);
        }
        private void txtPrg1F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg1F1, e);
        }
        private void txtPrg2F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg2F1, e);
        }
        private void txtPrg3F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg3F1, e);
        }
        private void txtPrg4F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg4F1, e);
        }
        private void txtPrg5F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg5F1, e);
        }
        private void txtPrg6F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg6F1, e);
        }
        private void txtPrg9F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg9F1, e);
        }
        private void txtPrg10F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg10F1, e);
        }
        private void txtPrg11F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg11F1, e);
        }
        private void txtPrg12F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg12F1, e);
        }
        private void lhmTxtPCBXP2F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPCBXP2F2, e);
        }
        private void lhmTxtPCBYP2F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPCBYP2F2, e);
        }
        private void lhmTxtPCBXP3F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPCBXP3F2, e);
        }
        private void lhmTxtPCBYP3F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPCBYP3F2, e);
        }
        private void lhmTxtPnlXP2F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPnlXP2F2, e);
        }
        private void lhmTxtPnlYP2F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPnlYP2F2, e);
        }
        private void lhmTxtPnlXP3F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPnlXP3F2, e);
        }
        private void lhmTxtPnlYP3F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(lhmTxtPnlYP3F2, e);
        }
        private void txtPrg1F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg1F2, e);
        }
        private void txtPrg2F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg2F2, e);
        }
        private void txtPrg3F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg3F2, e);
        }
        private void txtPrg4F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg4F2, e);
        }
        private void txtPrg5F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg5F2, e);
        }
        private void txtPrg6F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg6F2, e);
        }
        private void txtPrg9F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg9F2, e);
        }
        private void txtPrg10F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg10F2, e);
        }
        private void txtPrg11F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg11F2, e);
        }
        private void txtPrg12F2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dotControl(txtPrg12F2, e);
        }
        private void minMaxControl(TextBox textBox, double min, double max)
        {
            if (textBox.Text != "")
            {
                bool isNumeric = float.TryParse(textBox.Text, out _);
                if (isNumeric && double.Parse(textBox.Text.Replace(".", ",")) <= max && double.Parse(textBox.Text.Replace(".", ",")) >= min)
                {

                }
                else
                {
                    MessageBox.Show(min.ToString() + " - " + max.ToString() + " değer aralığında giriniz");
                    textBox.Text = "";
                }
            }
        }
        private void minMaxControlMainTextBox(TextBox textBox, double min, double max)
        {
            if (textBox.Text != "")
            {
                bool isNumeric = float.TryParse(textBox.Text, out _);
                if (isNumeric && double.Parse(textBox.Text.Replace(".", ",")) <= max && double.Parse(textBox.Text.Replace(".", ",")) >= min)
                {

                }
                else
                {
                    MessageBox.Show(min.ToString() + " - " + max.ToString() + " değer aralığında giriniz");
                    textBox.Text = "1";
                }
            }
        }
        private void lhmTxtPCBXP2F1_Leave(object sender, EventArgs e)
        {
            lhmTxtPCBXP2F1.Text = lhmTxtPCBXP2F1.Text.Replace(",", ".");
            minMaxControl(lhmTxtPCBXP2F1, 12, 365);
        }
        private void lhmTxtPCBYP2F1_Leave(object sender, EventArgs e)
        {
            lhmTxtPCBYP2F1.Text = lhmTxtPCBYP2F1.Text.Replace(",", ".");
            minMaxControl(lhmTxtPCBYP2F1, 320, 890);
        }
        private void lhmTxtPCBXP3F1_Leave(object sender, EventArgs e)
        {
            lhmTxtPCBXP3F1.Text = lhmTxtPCBXP3F1.Text.Replace(",", ".");
            minMaxControl(lhmTxtPCBXP3F1, 12, 365);
        }
        private void lhmTxtPCBYP3F1_Leave(object sender, EventArgs e)
        {
            lhmTxtPCBYP3F1.Text = lhmTxtPCBYP3F1.Text.Replace(",", ".");
            minMaxControl(lhmTxtPCBYP3F1, 320, 890);
        }
        private void lhmTxtPnlXP2F1_Leave(object sender, EventArgs e)
        {
            lhmTxtPnlXP2F1.Text = lhmTxtPnlXP2F1.Text.Replace(",", ".");
            minMaxControl(lhmTxtPnlXP2F1, 12, 365);
        }
        private void lhmTxtPnlYP2F1_Leave(object sender, EventArgs e)
        {
            lhmTxtPnlYP2F1.Text = lhmTxtPnlYP2F1.Text.Replace(",", ".");
            minMaxControl(lhmTxtPnlYP2F1, 320, 890);
        }
        private void lhmTxtPnlXP3F1_Leave(object sender, EventArgs e)
        {
            lhmTxtPnlXP3F1.Text = lhmTxtPnlXP3F1.Text.Replace(",", ".");
            minMaxControl(lhmTxtPnlXP3F1, 12, 365);
        }
        private void lhmTxtPnlYP3F1_Leave(object sender, EventArgs e)
        {
            lhmTxtPnlYP3F1.Text = lhmTxtPnlYP3F1.Text.Replace(",", ".");
            minMaxControl(lhmTxtPnlYP3F1, 320, 890);
        }
        private void txtPrg1F1_Leave(object sender, EventArgs e)
        {
            txtPrg1F1.Text = txtPrg1F1.Text.Replace(",", ".");
            minMaxControl(txtPrg1F1, 0, 330);
        }
        private void txtPrg2F1_Leave(object sender, EventArgs e)
        {
            txtPrg2F1.Text = txtPrg2F1.Text.Replace(",", ".");
            minMaxControl(txtPrg2F1, 660, 1200);
        }
        private void txtPrg3F1_Leave(object sender, EventArgs e)
        {
            txtPrg3F1.Text = txtPrg3F1.Text.Replace(",", ".");
            minMaxControl(txtPrg3F1, 0, 330);
        }
        private void txtPrg4F1_Leave(object sender, EventArgs e)
        {
            txtPrg4F1.Text = txtPrg4F1.Text.Replace(",", ".");
            minMaxControl(txtPrg4F1, 660, 1200);
        }
        private void txtPrg5F1_Leave(object sender, EventArgs e)
        {
            txtPrg5F1.Text = txtPrg5F1.Text.Replace(",", ".");
            minMaxControl(txtPrg5F1, 0, 330);
        }
        private void txtPrg6F1_Leave(object sender, EventArgs e)
        {
            txtPrg6F1.Text = txtPrg6F1.Text.Replace(",", ".");
            minMaxControl(txtPrg6F1, 660, 1200);
        }
        private void txtPrg9F1_Leave(object sender, EventArgs e)
        {
            txtPrg9F1.Text = txtPrg9F1.Text.Replace(",", ".");
            minMaxControl(txtPrg9F1, 0, 330);
        }
        private void txtPrg10F1_Leave(object sender, EventArgs e)
        {
            txtPrg10F1.Text = txtPrg10F1.Text.Replace(",", ".");
            minMaxControl(txtPrg10F1, 660, 1200);
        }
        private void txtPrg11F1_Leave(object sender, EventArgs e)
        {
            txtPrg11F1.Text = txtPrg11F1.Text.Replace(",", ".");
            minMaxControl(txtPrg11F1, 0, 330);
        }
        private void txtPrg12F1_Leave(object sender, EventArgs e)
        {
            txtPrg12F1.Text = txtPrg12F1.Text.Replace(",", ".");
            minMaxControl(txtPrg12F1, 660, 1200);
        }
        private void lhmTxtPCBXP2F2_Leave(object sender, EventArgs e)
        {
            lhmTxtPCBXP2F2.Text = lhmTxtPCBXP2F2.Text.Replace(",", ".");
            minMaxControl(lhmTxtPCBXP2F2, 560, 900);
        }
        private void lhmTxtPCBYP2F2_Leave(object sender, EventArgs e)
        {
            lhmTxtPCBYP2F2.Text = lhmTxtPCBYP2F2.Text.Replace(",", ".");
            minMaxControl(lhmTxtPCBYP2F2, 320, 890);
        }
        private void lhmTxtPCBXP3F2_Leave(object sender, EventArgs e)
        {
            lhmTxtPCBXP3F2.Text = lhmTxtPCBXP3F2.Text.Replace(",", ".");
            minMaxControl(lhmTxtPCBXP3F2, 560, 900);
        }
        private void lhmTxtPCBYP3F2_Leave(object sender, EventArgs e)
        {
            lhmTxtPCBYP3F2.Text = lhmTxtPCBYP3F2.Text.Replace(",", ".");
            minMaxControl(lhmTxtPCBYP3F2, 320, 890);
        }
        private void lhmTxtPnlXP2F2_Leave(object sender, EventArgs e)
        {
            lhmTxtPnlXP2F2.Text = lhmTxtPnlXP2F2.Text.Replace(",", ".");
            minMaxControl(lhmTxtPnlXP2F2, 560, 900);
        }
        private void lhmTxtPnlYP2F2_Leave(object sender, EventArgs e)
        {
            lhmTxtPnlYP2F2.Text = lhmTxtPnlYP2F2.Text.Replace(",", ".");
            minMaxControl(lhmTxtPnlYP2F2, 320, 890);
        }
        private void lhmTxtPnlXP3F2_Leave(object sender, EventArgs e)
        {
            lhmTxtPnlXP3F2.Text = lhmTxtPnlXP3F2.Text.Replace(",", ".");
            minMaxControl(lhmTxtPnlXP3F2, 560, 900);
        }
        private void lhmTxtPnlYP3F2_Leave(object sender, EventArgs e)
        {
            lhmTxtPnlYP3F2.Text = lhmTxtPnlYP3F2.Text.Replace(",", ".");
            minMaxControl(lhmTxtPnlYP3F2, 320, 890);
        }
        private void txtPrg1F2_Leave(object sender, EventArgs e)
        {
            txtPrg1F2.Text = txtPrg1F2.Text.Replace(",", ".");
            minMaxControl(txtPrg1F2, 56, 390);
        }
        private void txtPrg2F2_Leave(object sender, EventArgs e)
        {
            txtPrg2F2.Text = txtPrg2F2.Text.Replace(",", ".");
            minMaxControl(txtPrg2F2, 660, 1200);
        }
        private void txtPrg3F2_Leave(object sender, EventArgs e)
        {
            txtPrg3F2.Text = txtPrg3F2.Text.Replace(",", ".");
            minMaxControl(txtPrg3F2, 56, 390);
        }
        private void txtPrg4F2_Leave(object sender, EventArgs e)
        {
            txtPrg4F2.Text = txtPrg4F2.Text.Replace(",", ".");
            minMaxControl(txtPrg4F2, 660, 1200);
        }
        private void txtPrg5F2_Leave(object sender, EventArgs e)
        {
            txtPrg5F2.Text = txtPrg5F2.Text.Replace(",", ".");
            minMaxControl(txtPrg5F2, 56, 390);
        }
        private void txtPrg6F2_Leave(object sender, EventArgs e)
        {
            txtPrg6F2.Text = txtPrg6F2.Text.Replace(",", ".");
            minMaxControl(txtPrg6F2, 660, 1200);
        }
        private void txtPrg9F2_Leave(object sender, EventArgs e)
        {
            txtPrg9F2.Text = txtPrg9F2.Text.Replace(",", ".");
            minMaxControl(txtPrg9F2, 56, 390);
        }
        private void txtPrg10F2_Leave(object sender, EventArgs e)
        {
            txtPrg10F2.Text = txtPrg10F2.Text.Replace(",", ".");
            minMaxControl(txtPrg10F2, 660, 1200);
        }
        private void txtPrg11F2_Leave(object sender, EventArgs e)
        {
            txtPrg11F2.Text = txtPrg11F2.Text.Replace(",", ".");
            minMaxControl(txtPrg11F2, 56, 390);
        }
        private void txtPrg12F2_Leave(object sender, EventArgs e)
        {
            txtPrg12F2.Text = txtPrg12F2.Text.Replace(",", ".");
            minMaxControl(txtPrg12F2, 660, 1200);
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
            process.Close();
        }
    }
    public class INIKaydet
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public INIKaydet(string dosyaYolu)
        {
            DOSYAYOLU = dosyaYolu;
        }
        private string DOSYAYOLU = String.Empty;
        public string Varsayilan { get; set; }
        public string Oku(string bolum, string ayaradi)
        {
            Varsayilan = Varsayilan ?? string.Empty;
            StringBuilder StrBuild = new StringBuilder(256);
            GetPrivateProfileString(bolum, ayaradi, Varsayilan, StrBuild, 255, DOSYAYOLU);
            return StrBuild.ToString();
        }
        public long Yaz(string bolum, string ayaradi, string deger)
        {
            return WritePrivateProfileString(bolum, ayaradi, deger, DOSYAYOLU);
        }
        public long Sil(string bolum, string ayaradi, string deger)
        {
            return WritePrivateProfileString(bolum, ayaradi, deger, DOSYAYOLU);
        }
    }
}

