using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleApp21;

namespace WindowsFormsApp3
{
    public partial class Form6 : Form
    {
        public List<Stock1> d = new List<Stock1>();
        public static int idk= GetInstCount();
        public ComboboxItem[] d1 = new ComboboxItem[idk];
        public Form6()
        {
            InitializeComponent();
            Model1Container db = new Model1Container();
            d = db.Instruments1.OfType<Stock1>().ToList();
            for (int i = 1; i < idk + 1; i++)
            {

                string x;
                comboBox1.Items.Add(d[i - 1].CompanyName);
                ComboboxItem item = new ComboboxItem();
                item.Text = Convert.ToString(d[i - 1].CompanyName);
                item.Value = d[i - 1].Id;
                d1[i - 1] = item;

            }



        }
        public static int GetInstCount()
        {
            using (Model1Container db1 = new Model1Container())
            {
                return db1.Instruments1.OfType<Stock1>().Count();
            }

        }
        public class ComboboxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
        public double HP;
        public int selectedValue1;
        public DateTime Date;
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Inst = comboBox1.Text;
            for (int i = 0; i < idk; i++)
            {
                if (d1[i].Text == Inst)
                {
                    selectedValue1 = d1[i].Value;
                }

            }
            try
            {
                HP = Convert.ToDouble(textBox1.Text);
                Date = dateTimePicker1.Value.Date;
                MessageBox.Show("Historical Price was successfull added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch
            {
                MessageBox.Show("Please Enter the right type on input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Price1 x = new Price1
            {
                InstrumentsId = selectedValue1,
                Date = Date,
                ClosingPrice = HP,
            };
            using (Model1Container db1 = new Model1Container())
            {
                //Console.WriteLine("yes1");
                //db.SaveChanges();
                db1.Price1.Add(x);
                db1.SaveChanges();
                //Console.ReadLine();

            }

        }
    }
}
