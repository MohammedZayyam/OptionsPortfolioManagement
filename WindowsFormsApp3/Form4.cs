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
    public partial class Form4 : Form
    {
        public int idk;
        ComboboxItem[] d1;
        public Form4()
        {
            InitializeComponent();
            idk = GetInstCount();
            ComboboxItem[] d = new ComboboxItem[idk];
            using (Model1Container db = new Model1Container())
            {
                
                for (int i = 1; i < idk+1; i++)
                {
                    
                    string x;
                    x = db.Instruments1.Find(i).Ticker;
                    comboBox1.Items.Add(x);
                    
                    ComboboxItem item = new ComboboxItem();
                    item.Text = Convert.ToString(x);
                    item.Value = i;
                    d[i-1] = item;
                    //Console.WriteLine("instid:{0}, {1}", d[i-1].Value, d[i-1].Text);
                    //Console.ReadLine();
                }
                d1 = d;

            }
            
        }
        public int GetInstCount()
        {
            using (Model1Container db = new Model1Container())
            {
                return db.Instruments1.Count();
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
        public int selectedIndex, selectedValue1, Q;
        public string Inst, C;
        public double P;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ComboBox cmb = (ComboBox)sender;


            //ComboboxItem selectedCar = (ComboboxItem)cmb.SelectedItem;
            //Inst = selectedCar.Text;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Inst = comboBox1.Text;
           // MessageBox.Show(Inst);
            for (int i = 0; i < idk ; i++)
            {

                //Console.WriteLine("instid:{0}, {1}, INSt: {2}", d1[i ].Value, d1[i ].Text, Inst);
               // Console.ReadLine();
                if (d1[i].Text == Inst)
                {
                    selectedValue1 = i + 1;
                    //Console.WriteLine("instid:{0}", selectedValue1);
                    //Console.ReadLine();
                }

            }
            //Console.WriteLine("{0}: V:{1}", selectedIndex, selectedValue);
            if (radioButton1.Checked == true || radioButton2.Checked == true)
            {
                if (radioButton1.Checked == true)
                {
                    C = "Buy";
                }
                if (radioButton2.Checked == true)
                {
                    C = "Sell";
                }
            }
            else
            {
                MessageBox.Show("Please select either Call or Put Option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                Q = Convert.ToInt32(textBox1.Text);
                P = Convert.ToDouble(textBox2.Text);
            }
            catch
            {
                MessageBox.Show("Please Enter the right type on input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            DateTime y = DateTime.Today;
            Trade1 x = new Trade1
            {
                InstrumentsId = selectedValue1,
                IsBuy = C,
                Direction =C,
                Quantity= Q,
                Price=P,
                TimeStamp= y


                
            };
            using (Model1Container db = new Model1Container())
            {
                //Console.WriteLine("yes1");
                //db.SaveChanges();
                db.Trade1.Add(x);
                db.SaveChanges();
                //Console.ReadLine();

            }

        }
    }
}
