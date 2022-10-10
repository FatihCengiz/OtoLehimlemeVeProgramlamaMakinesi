using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OtoLehimlemeVeProgramlamaMakinesi
{
    public partial class FormMessageF1 : Form
    {
        Main main;
        public FormMessageF1(Main _main)
        {
            InitializeComponent();
            main = _main;
        }

        private void btnAgain_Click(object sender, EventArgs e)
        {
            main.nxCompoletBoolWrite("f1pcbProcessAgain", true);
            main.msgCounter = 0;
            this.Close();
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            main.nxCompoletBoolWrite("f1pcbProcessContinue", true);
            main.msgCounter = 0;
            this.Close();
        }

		private void FormMessageF1_Load(object sender, EventArgs e)
		{
          //  ((Form)((Control)sender).Parent).ShowInTaskbar = false;
        }
	}
}
