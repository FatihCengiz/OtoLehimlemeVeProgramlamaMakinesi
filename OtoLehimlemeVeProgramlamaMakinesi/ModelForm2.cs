using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OtoLehimlemeVeProgramlamaMakinesi
{
    public partial class ModelForm2 : Form
    {
        Main main;
        string desktopPath = "", modelsPathF2 = "", currentModel="";
        int modelNumber = 0;
        Thread threadProcess;
        public ModelForm2(Main _main)
        {
            InitializeComponent();
            main = _main;
        }
        private void ModelForm2_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "Model Seçimi";
            lblMessage.ForeColor = Color.Red;
            desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            modelsPathF2 = desktopPath + @"\Modeller\ModelF2.ini";
            if (File.Exists(modelsPathF2))
            {
                INIKaydet ini = new INIKaydet(modelsPathF2);
                modelNumber = Int16.Parse(ini.Oku("Model", "ModelSayisi"));
                for (int i = 0; i < modelNumber; i++)
                {
                    cmbBxModelF2.Items.Add(ini.Oku("Model" + (i + 1).ToString(), "ModelAdi"));
                }
            }
        }

		private void btnDelete_Click(object sender, EventArgs e)
		{
            deleteModel();
		}

		private void btnDataSendF2_Click(object sender, EventArgs e)
        {
            if (cmbBxModelF2.Text != "")
            {
                if (main.txtModelNameF2.Text != cmbBxModelF2.Text)
                {

                    main.txtMainModelF2.Text = splitIniText(cmbBxModelF2.Text);
                    main.txtModelNameF2.Text = splitIniText(cmbBxModelF2.Text);
                    main.txtModelNameF2.Enabled = false;
                    currentModel = desktopPath + @"\Lehimleyici_Modelleri2\" + cmbBxModelF2.Text;
                    main.loadModelF2(currentModel);
                    main.nxCompoletBoolWrite("f2Recipe", true);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Lütfen Fikstür-2 İçin Farklı Model Seçiniz");
                }
            }
			if (cmbBxModelF2.Text == "")
			{
				threadProcess = new Thread(() => main.nxCompoletBoolWrite("f2Recipe", false));
				threadProcess.Start();
			}
		}
        private void deleteModel()
        {
            DialogResult dialogResult = MessageBox.Show("Seçili dosya silinsin mi ?", "Uyarı", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (cmbBxModelF2.Text != "")
                {
                    List<string> listFileName, listFilePath;
                    listFileName = new List<string>();
                    listFilePath = new List<string>();
                    modelsPathF2 = desktopPath + @"\Modeller\ModelF2.ini";
                    if (File.Exists(modelsPathF2))
                    {
                        INIKaydet ini = new INIKaydet(modelsPathF2);
                        ini.Sil("Model" + (cmbBxModelF2.SelectedIndex + 1).ToString(), null, null);
                        ini.Yaz("Model", "ModelSayisi", (cmbBxModelF2.Items.Count - 1).ToString());
                        for (int i = 0; i < cmbBxModelF2.Items.Count; i++)
                        {
                            if (cmbBxModelF2.SelectedIndex != i)
                            {
                                listFileName.Add(ini.Oku("Model" + (i + 1).ToString(), "ModelAdi"));
                                listFilePath.Add(ini.Oku("Model" + (i + 1).ToString(), "ModelYolu"));
                            }
                        }
                        for (int i = 0; i < listFileName.Count + 1; i++)
                        {
                            ini.Sil("Model" + (i + 1).ToString(), null, null);

                        }
                        cmbBxModelF2.Items.Clear();
                        for (int i = 0; i < listFileName.Count; i++)
                        {
                            ini.Yaz("Model" + (i + 1), "ModelAdi", listFileName[i]);
                            ini.Yaz("Model" + (i + 1), "ModelYolu", listFilePath[i]);
                            cmbBxModelF2.Items.Add(ini.Oku("Model" + (i + 1).ToString(), "ModelAdi"));
                        }

                    }
                    currentModel = desktopPath + @"\Lehimleyici_Modelleri2\" + cmbBxModelF2.Text;
                    if (File.Exists(currentModel))
                    {
                        File.Delete(currentModel);
                    }
                    cmbBxModelF2.Text = "";
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
