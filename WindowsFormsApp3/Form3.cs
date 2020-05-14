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
    public partial class Form3 : Form
    {
        public string C, com, tic, exch, BT;

        private void Form3_Load(object sender, EventArgs e)
        {

        }
        public List<Stock1> d = new List<Stock1>();
        public static int idk = GetInstCount();
        public int U;
        public List<ComboboxItem> d1 = new List<ComboboxItem>();
        public double S, T, R, B;
        public Form3()
        {
            //groupBox7.Enabled = false;
            //groupBox8.Enabled = false;
            
            InitializeComponent();
            Model1Container db = new Model1Container();
            d = db.Instruments1.OfType<Stock1>().ToList();
            for (int i = 0; i < d.Count(); i++)
            {

                //string x;
                comboBox1.Items.Add(d[i].CompanyName);
                ComboboxItem item = new ComboboxItem();
                item.Text = Convert.ToString(d[i].CompanyName);
                item.Value = d[i].Id;
                d1.Add(item);

            }



        }
        public static int GetInstCount()
        {
            using (Model1Container db1 = new Model1Container())
            {
                return db1.Instruments1.OfType<Stock1>().Count();
            }

        }


        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true || radioButton2.Checked == true)
            {
                if (radioButton1.Checked == true)
                {
                    C = "Call";
                }
                if (radioButton2.Checked == true)
                {
                    C = "Put";
                }
            }
            else
            {
                MessageBox.Show("Please select either Call or Put Option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                com = Convert.ToString(textBox1.Text);
                tic = Convert.ToString(textBox4.Text);
                exch = Convert.ToString(textBox5.Text);
                S = Convert.ToDouble(textBox2.Text);
                T = Convert.ToDouble(textBox3.Text);
                //R = Convert.ToDouble(textBox10);
                //B = Convert.ToDouble(textBox9);


            }
            catch
            {
                MessageBox.Show("Please check the input and enterasd the right type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (radioButton4.Checked == true)
            {
                try
                {
                    B = Convert.ToDouble(textBox9.Text);
                }
                catch
                {
                    MessageBox.Show("Please check the input and entera the right type for Barrier Price", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (radioButton9.Checked == true || radioButton10.Checked == true || radioButton11.Checked == true || radioButton12.Checked == true)
                {
                    if (radioButton9.Checked == true)
                    {
                        BT = "Up and In";
                    }
                    if (radioButton10.Checked == true)
                    {
                        BT = "Up and Out";
                    }
                    if (radioButton11.Checked == true)
                    {
                        BT = "Down and Out";
                    }
                    if (radioButton12.Checked == true)
                    {
                        BT = "Down and In";
                    }
                }
                else
                {
                    MessageBox.Show("Please select either One type of Barrier Option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if(radioButton7.Checked== true)
            {
                try
                {
                    R = Convert.ToDouble(textBox10.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter the right input type for Rebate Price", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            double u1=0;
            if (radioButton13.Checked == true)
            {
                try
                {
                    u1 = Convert.ToDouble(comboBox1.Text);
                    DateTime y = DateTime.Today;
                    Stock1 x = new Stock1
                    {
                        Underlying = u1,
                        //Strike = 90,
                        //IsCall = "call",
                        Ticker = tic,
                        Exchange = exch,
                        CompanyName = com
                        //ExpirationDate = y
                    };
                    using (Model1Container db1 = new Model1Container())
                    {
                        //Console.WriteLine("yes1");
                        //db.SaveChanges();
                        db1.Instruments1.Add(x);
                        db1.SaveChanges();
                        //Console.ReadLine();
                        //Model1Container db = new Model1Container();

                        d = db1.Instruments1.OfType<Stock1>().ToList();
                        comboBox1.Items.Clear();

                        for (int i = d1.Count() - 1; i < d.Count() + 1; i++)
                        {

                            //string x;

                            comboBox1.Items.Add(d[i - 1].CompanyName);
                            ComboboxItem item = new ComboboxItem();
                            item.Text = Convert.ToString(d[i - 1].CompanyName);
                            item.Value = d[i - 1].Id;
                            d1.Add(item);

                        }
                    }

                        MessageBox.Show("Instrument was successfull added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                catch
                {
                    MessageBox.Show("Please enter the Underlying in the combobox for a stock", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    string Inst = comboBox1.SelectedItem.ToString();
                    for (int i = 0; i < idk; i++)
                    {
                        if (d1[i].Text == Inst)
                        {
                            U = Convert.ToInt32(d1[i].Value);
                            if (radioButton3.Checked == true)
                            {
                                DateTime y = DateTime.Today;
                                European1 x = new European1
                                {
                                    Underlying = U,
                                    Strike = S,
                                    IsCall = C,
                                    Ticker = tic,
                                    Exchange = exch,
                                    //CompanyName = co
                                    ExpirationDate = T
                                };
                                using (Model1Container db = new Model1Container())
                                {
                                    //Console.WriteLine("yes1");
                                    //db.SaveChanges();
                                    db.Instruments1.Add(x);
                                    db.SaveChanges();
                                    //Console.ReadLine();

                                }
                                MessageBox.Show("Instrument was successfull added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            if (radioButton8.Checked == true)
                            {
                                DateTime y = DateTime.Today;
                                Asian1 x = new Asian1
                                {
                                    Underlying = U,
                                    Strike = S,
                                    IsCall = C,
                                    Ticker = tic,
                                    Exchange = exch,
                                    //CompanyName = com
                                    ExpirationDate = T
                                };
                                using (Model1Container db = new Model1Container())
                                {
                                    //Console.WriteLine("yes1");
                                    //db.SaveChanges();
                                    db.Instruments1.Add(x);
                                    db.SaveChanges();
                                    //Console.ReadLine();

                                }
                                MessageBox.Show("Instrument was successfull added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            if (radioButton6.Checked == true)
                            {
                                DateTime y = DateTime.Today;
                                Range1 x = new Range1
                                {
                                    Underlying = U,
                                    Strike = S,
                                    //IsCall = C,
                                    Ticker = tic,
                                    Exchange = exch,
                                    //CompanyName = com
                                    ExpirationDate = T
                                };
                                using (Model1Container db = new Model1Container())
                                {
                                    //Console.WriteLine("yes1");
                                    //db.SaveChanges();
                                    db.Instruments1.Add(x);
                                    db.SaveChanges();
                                    //Console.ReadLine();

                                }
                                MessageBox.Show("Instrument was successfull added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            if (radioButton5.Checked == true)
                            {
                                DateTime y = DateTime.Today;
                                LookBack1 x = new LookBack1
                                {
                                    Underlying = U,
                                    Strike = S,
                                    IsCall = C,
                                    Ticker = tic,
                                    Exchange = exch,
                                    //CompanyName = com
                                    ExpirationDate = T
                                };
                                using (Model1Container db = new Model1Container())
                                {
                                    //Console.WriteLine("yes1");
                                    //db.SaveChanges();
                                    db.Instruments1.Add(x);
                                    db.SaveChanges();
                                    //Console.ReadLine();

                                }
                                MessageBox.Show("Instrument was successfull added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            //public double r;
                            if (radioButton7.Checked == true)
                            {
                                //groupBox7.Enabled = true;
                                DateTime y = DateTime.Today;

                                Digital1 x = new Digital1
                                {
                                    Underlying = U,
                                    Strike = S,
                                    IsCall = C,
                                    Ticker = tic,
                                    Exchange = exch,
                                    Rebate = R,
                                    //CompanyName = com
                                    ExpirationDate = T
                                };
                                using (Model1Container db = new Model1Container())
                                {
                                    //Console.WriteLine("yes1");
                                    //db.SaveChanges();
                                    db.Instruments1.Add(x);
                                    db.SaveChanges();
                                    //Console.ReadLine();

                                }
                                MessageBox.Show("Instrument was successfull added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            if (radioButton4.Checked == true)
                            {
                                //groupBox7.Enabled = true;
                                DateTime y = DateTime.Today;

                                Barrier1 x = new Barrier1
                                {
                                    Underlying = U,
                                    Strike = S,
                                    IsCall = C,
                                    Ticker = tic,
                                    Exchange = exch,
                                    BarrierPrice = B,
                                    BarrierType = BT,
                                    //CompanyName = com
                                    ExpirationDate = T
                                };
                                using (Model1Container db = new Model1Container())
                                {
                                    //Console.WriteLine("yes1");
                                    //db.SaveChanges();
                                    db.Instruments1.Add(x);
                                    db.SaveChanges();
                                    //Console.ReadLine();

                                }
                                MessageBox.Show("Instrument was successfull added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }


                        }
                    }
                }
                catch
                {

                    MessageBox.Show("Please select an Underlying Stock for the Option Prices", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }

                }
            }

            
            



        }
            
       
            
}

