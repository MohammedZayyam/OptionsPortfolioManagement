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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            Model1Container db = new Model1Container();
            dataGridView2.DataSource = db.Trade1.ToList();
            
            //dataGridViewColumn x = 
           
        }
        public void GenerateMarkPrice()
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            Program.Vuniversal = Convert.ToDouble(textBox1.Text);//place holder

        }
        
    }
}
