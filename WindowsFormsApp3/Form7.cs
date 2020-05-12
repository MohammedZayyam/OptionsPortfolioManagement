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
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
            Model1Container db = new Model1Container();
            dataGridView1.DataSource = db.Instruments1.OfType<Stock1>().ToList();
            //dataGridView1.DataBindings
        }

        private void Form7_Load(object sender, EventArgs e)
        {

        }
    }
}
