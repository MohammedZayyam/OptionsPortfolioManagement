using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox3.Checked==true)
            {
                Program.M_uni = "M";
            }
            if (checkBox1.Checked == true)
            {
                Program.A_uni = "A";
            }
            if (checkBox2.Checked == true)
            {
                Program.CV_uni = "CV";
            }
            try
            {
                Program.Trial_uni = Convert.ToInt32(textBox6.Text);
                Program.Steps_uni = Convert.ToInt32(textBox7.Text);
                MessageBox.Show("Settings Changed Successfully", "Done", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
            catch
            {
                MessageBox.Show("Please select the right kind of Input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
