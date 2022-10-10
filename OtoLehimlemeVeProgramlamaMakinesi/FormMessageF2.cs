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
    public partial class FormMessageF2 : Form
    {
        Main main;
        public FormMessageF2(Main _main)
        {
            InitializeComponent();
            main = _main;
        }

        private void btnAgain_Click(object sender, EventArgs e)
        {
            main.nxCompoletBoolWrite("f2pcbProcessAgain", true);
            main.msgCounter2 = 0;
            this.Close();
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            main.nxCompoletBoolWrite("f2pcbProcessContinue", true);
            main.msgCounter2 = 0;
            this.Close();
        }
    }
}
