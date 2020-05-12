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
        public double S, T, R, B;
        public Form3()
        {
            //groupBox7.Enabled = false;
            //groupBox8.Enabled = false;
            
            InitializeComponent();
            
            
                
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
                com = Convert.ToString(textBox1);
                tic = Convert.ToString(textBox4);
                exch = Convert.ToString(textBox5);
                S = Convert.ToDouble(textBox2);
                T = Convert.ToDouble(textBox3);
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
                    B = Convert.ToDouble(textBox9);
                }
                catch
                {
                    MessageBox.Show("Please check the input and enterasd the right type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (radioButton9.Checked == true || radioButton10.Checked == true || radioButton11.Checked == true || radioButton12.Checked == true)
                {
                    if (radioButton9.Checked == true)
                    {
                        BT = "Up and In";
                    }
                    if (radioButton10.Checked == true)
                    {
                        C = "Up and Out";
                    }
                    if (radioButton11.Checked == true)
                    {
                        C = "Down and Out";
                    }
                    if (radioButton12.Checked == true)
                    {
                        C = "Down and In";
                    }
                }
                else
                {
                    MessageBox.Show("Please select either One type of Barrier Option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
                if ( radioButton13.Checked== true)
            {
                DateTime y = DateTime.Today;
                Stock1 x = new Stock1
                {
                    Underlying = 0,
                    //Strike = 90,
                    //IsCall = "call",
                    Ticker = tic,
                    Exchange = exch,
                    CompanyName= com
                    //ExpirationDate = y
                };
                using (Model1Container db = new Model1Container())
                {
                    //Console.WriteLine("yes1");
                    //db.SaveChanges();
                    db.Instruments1.Add(x);
                    db.SaveChanges();
                    //Console.ReadLine();

                }
            }
            if (radioButton3.Checked == true)
            {
                DateTime y = DateTime.Today;
                European1 x = new European1
                {
                    Underlying = 0,
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
            }
            if (radioButton8.Checked == true)
            {
                DateTime y = DateTime.Today;
                Asian1 x = new Asian1
                {
                    Underlying = 0,
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
            }
            if (radioButton6.Checked == true)
            {
                DateTime y = DateTime.Today;
                European1 x = new European1
                {
                    Underlying = 0,
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
            }
            if (radioButton5.Checked == true)
            {
                DateTime y = DateTime.Today;
                LookBack1 x = new LookBack1
                {
                    Underlying = 0,
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
            }
            //public double r;
            if (radioButton7.Checked == true)
            {
                //groupBox7.Enabled = true;
                DateTime y = DateTime.Today;
                
                Digital1 x = new Digital1
                {
                    Underlying = 0,
                    Strike = S,
                    IsCall = C,
                    Ticker = tic,
                    Exchange = exch,
                    Rebate= R,
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
            }
            if (radioButton4.Checked == true)
            {
                //groupBox7.Enabled = true;
                DateTime y = DateTime.Today;

                Barrier1 x = new Barrier1
                {
                    Underlying = 0,
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
            }



        }
            
       
            

    }
}

