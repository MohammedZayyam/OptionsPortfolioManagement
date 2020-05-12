using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleApp21;

namespace WindowsFormsApp3
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }
        public Double Tenor, IR;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Tenor = Convert.ToDouble(textBox2.Text);
                IR = Convert.ToDouble(textBox1.Text);
            }
            catch
            {
                MessageBox.Show("Please Enter the right kind of Input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                InterestRate1 x = new InterestRate1
                {
                    Rate = IR,
                    Tenor = Tenor
                };
                using(Model1Container db = new Model1Container() )
                {
                    db.InterestRate1.Add(x);
                    db.SaveChanges();
                }
                MessageBox.Show("Interest Rate successfull added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );

            }
            catch
            {
                MessageBox.Show("Please Enter the right king of input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
    }
}
