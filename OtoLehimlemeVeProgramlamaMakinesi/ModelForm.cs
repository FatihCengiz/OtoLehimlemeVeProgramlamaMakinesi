using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OtoLehimlemeVeProgramlamaMakinesi
{
    public partial class ModelForm : Form
    {
        Main main;
        public static string currentModelNameF1;
        string currentModel = "", desktopPath = "", modelsPathF1 = "";
        int modelNumber = 0;
        Thread threadProcess;
        bool boolDeleteFlag;
        public ModelForm()
        {
            InitializeComponent();
        }
        public ModelForm(Main _main)
        {
            main = _main;
            InitializeComponent();
        }
        private void ModelForm_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "Model Seçimi";
            lblMessage.ForeColor = Color.Red;
            desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);//
            modelsPathF1 = desktopPath + @"\Modeller\ModelF1.ini";
            if (File.Exists(modelsPathF1))
            {
                INIKaydet ini = new INIKaydet(modelsPathF1);
                modelNumber = Int16.Parse(ini.Oku("Model", "ModelSayisi"));
                for (int i = 0; i < modelNumber; i++)
                {
                    cmbBxModelF1.Items.Add(ini.Oku("Model" + (i + 1).ToString(), "ModelAdi"));
                }
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            deleteModel();
        }

		private void btnDataSend_Click(object sender, EventArgs e)
        {
            if (cmbBxModelF1.Text != "")
            {
                if (main.txtModelNameF1.Text != cmbBxModelF1.Text)
                {
                    main.txtMainModelF1.Text = splitIniText(cmbBxModelF1.Text);
                    main.txtModelNameF1.Text = splitIniText(cmbBxModelF1.Text);
                    main.txtModelNameF1.Enabled = false;
                    currentModel = desktopPath + @"\Lehimleyici_Modelleri\" + cmbBxModelF1.Text;
                    main.loadModelF1(currentModel);
                    main.nxCompoletBoolWrite("f1Recipe", true);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Lütfen Fikstür-1 İçin Farklı Model Seçiniz");
                }
            }   

            if (cmbBxModelF1.Text=="")
            {
                threadProcess = new Thread(()=> main.nxCompoletBoolWrite("f1Recipe", false));
                threadProcess.Start();
            }
            
        }
        private void deleteModel() 
        {
            DialogResult dialogResult = MessageBox.Show("Seçili dosya silinsin mi ?", "Uyarı", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (cmbBxModelF1.Text != "")
                {
                    List<string> listFileName, listFilePath;
                    listFileName = new List<string>();
                    listFilePath = new List<string>();
                    modelsPathF1 = desktopPath + @"\Modeller\ModelF1.ini";
                    if (File.Exists(modelsPathF1))
                    {
                        INIKaydet ini = new INIKaydet(modelsPathF1);
                        ini.Sil("Model" + (cmbBxModelF1.SelectedIndex + 1).ToString(), null, null);
                        ini.Yaz("Model", "ModelSayisi", (cmbBxModelF1.Items.Count - 1).ToString());
                        for (int i = 0; i < cmbBxModelF1.Items.Count; i++)
                        {
                            if (cmbBxModelF1.SelectedIndex != i)
                            {
                                listFileName.Add(ini.Oku("Model" + (i + 1).ToString(), "ModelAdi"));
                                listFilePath.Add(ini.Oku("Model" + (i + 1).ToString(), "ModelYolu"));
                            }
                        }
                        for (int i = 0; i < listFileName.Count + 1; i++)
                        {
                            ini.Sil("Model" + (i + 1).ToString(), null, null);

                        }
                        cmbBxModelF1.Items.Clear();
                        for (int i = 0; i < listFileName.Count; i++)
                        {
                            ini.Yaz("Model" + (i + 1), "ModelAdi", listFileName[i]);
                            ini.Yaz("Model" + (i + 1), "ModelYolu", listFilePath[i]);
                            cmbBxModelF1.Items.Add(ini.Oku("Model" + (i + 1).ToString(), "ModelAdi"));
                        }

                    }
                    currentModel = desktopPath + @"\Lehimleyici_Modelleri\" + cmbBxModelF1.Text;
                    if (File.Exists(currentModel))
                    {
                        File.Delete(currentModel);
                    }
                    cmbBxModelF1.Text = "";
                    MessageBox.Show("Seçili model silindi");
                }
                else
                {
                    MessageBox.Show("Model seçiniz");
                }
            }
        }
        private string splitIniText(string value)
		{
            string[] stringArray = value.Split('.');
            return stringArray[0];
		}
    }

}
