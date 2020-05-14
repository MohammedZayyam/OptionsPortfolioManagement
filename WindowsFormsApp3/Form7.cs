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
        public Model1Container db = new Model1Container();
        public List<Instruments1> list1 = new List<Instruments1>();

        public Form7()
        {
            InitializeComponent();
            Model1Container db = new Model1Container();
            //List<Instruments1> list1 = new List<Instruments1>;
            comboBox1.Items.Add("All Instruments");
            comboBox1.Items.Add("Stocks");
            comboBox1.Items.Add("European Options");
            comboBox1.Items.Add("Asian Options");
            comboBox1.Items.Add("Range Options");
            comboBox1.Items.Add("LookBack Options");
            comboBox1.Items.Add("Digital Options");
            comboBox1.Items.Add("Barrier Options");
            comboBox1.Items.Add("Historical Prices");
            comboBox1.Items.Add("Interest Rate");
            list1 = db.Instruments1.OfType<Instruments1>().ToList();
            //dataGridView1.DataSource = db.Instruments1.OfType<Instruments1>().ToList();
            //dataGridView1.DataBindings
        }
        private void Form7_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            string u="null";
            Model1Container db = new Model1Container();
            try
            {
                u = comboBox1.SelectedItem.ToString();
            }
            catch
            {
                MessageBox.Show("Select rows to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            if (u == "All Instruments" || u == "Stocks" || u == "European Options" || u == "Asian Options" || u == "Range Options" || u == "LookBack Options" || u == "Digital Options" || u == "Barrier Options")
            {

                foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                {
                    int fk = Convert.ToInt32(r.Cells["Id"].Value);
                    List<Price1> p = new List<Price1>();
                    List<Trade1> T = new List<Trade1>();
                    T = db.Trade1.Where(m => m.InstrumentsId == fk).ToList();
                    p = db.Price1.Where(m => m.InstrumentsId == fk).ToList();
                    DialogResult dialogResult = MessageBox.Show("Deleting this Instrument will cause all Historical Prices and Trades related to the Instrument to get deleted as well", "Do You Wish to Contine?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        foreach (Trade1 y in T )
                        {
                            //Console.WriteLine("r:{0}, id:{1}", r, id);
                            var g1 = db.Trade1.Find(y.Id);
                            db.Entry(g1).State = System.Data.Entity.EntityState.Deleted;
                            db.SaveChanges();

                        }
                        foreach (Price1 y in p)
                        {
                            //Console.WriteLine("r:{0}, id:{1}", r, id);
                            var g1 = db.Price1.Find(y.Id);
                            db.Entry(g1).State = System.Data.Entity.EntityState.Deleted;
                            db.SaveChanges();

                        }

                        int id = Convert.ToInt32(r.Cells["Id"].Value);
                        Console.WriteLine("r:{0}, id:{1}", r, id);
                        var g = db.Instruments1.SingleOrDefault(h => h.Id == id);
                        db.Entry(g).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        break;
                    }

                    

                }
            }
            if (u == "Historical Prices")
            {
                foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                {

                    int id = Convert.ToInt32(r.Cells["Id"].Value);
                    Console.WriteLine("r:{0}, id:{1}", r, id);
                    var g = db.Price1.SingleOrDefault(h => h.Id == id);
                    db.Entry(g).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();

                }
            }
            if (u == "Interest Rate")
            {
                foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                {

                    int id = Convert.ToInt32(r.Cells["Id"].Value);
                    Console.WriteLine("r:{0}, id:{1}", r, id);
                    var g = db.InterestRate1.SingleOrDefault(h => h.Id == id);
                    db.Entry(g).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();

                }
            }

            /*string x;
                
                String type = Convert.ToString(db.Instruments1.GetType());
                x = Convert.ToString(type[34]);
                if (x=="s")
                {

                   // db.Instruments1.Attach(g);
                    //db.Instruments1.Remove(g);
                    //db.SaveChanges();
                }
                if (x == "E")
                {
                    var g = new European1 { Id = id };
                    db.Instruments1.Attach(g);
                    db.Instruments1.Remove(g);
                    db.SaveChanges();
                }
                if (x == "E")
                {
                    var g = new Asian1 { Id = id };
                    db.Instruments1.Attach(g);
                    db.Instruments1.Remove(g);
                    db.SaveChanges();
                }
                if (x == "L")
                {
                    var g = new LookBack1 { Id = id };
                    db.Instruments1.Attach(g);
                    db.Instruments1.Remove(g);
                    db.SaveChanges();
                }
                if (x == "R")
                {
                    var g = new Range1 { Id = id };
                    db.Instruments1.Attach(g);
                    db.Instruments1.Remove(g);
                    db.SaveChanges();
                }
                if (x == "D")
                {
                    var g = new Digital1 { Id = id };
                    db.Instruments1.Attach(g);
                    db.Instruments1.Remove(g);
                    db.SaveChanges();
                }
                if (x == "B")
                {
                    var g = new Barrier1 { Id = id };
                    db.Instruments1.Attach(g);
                    db.Instruments1.Remove(g);
                    db.SaveChanges();
                }


            }


        }*/

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = db.Instruments1.OfType<Instruments1>().ToList();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem.ToString()=="All Instruments")
            {
                list1 = db.Instruments1.OfType<Instruments1>().ToList();
                dataGridView1.DataSource = db.Instruments1.OfType<Instruments1>().ToList();
                dataGridView1.Columns["Prices"].Visible = false;
                dataGridView1.Columns["Trades"].Visible = false;
            }
            if (comboBox1.SelectedItem.ToString() == "Stocks")
            {
                dataGridView1.DataSource = db.Instruments1.OfType<Stock1>().ToList();
                dataGridView1.Columns["Prices"].Visible = false;
                dataGridView1.Columns["Trades"].Visible = false;
            }
            if (comboBox1.SelectedItem.ToString() == "European Options")
            {
                dataGridView1.DataSource = db.Instruments1.OfType<European1>().ToList();
                dataGridView1.Columns["Prices"].Visible = false;
                dataGridView1.Columns["Trades"].Visible = false;
            }
            if (comboBox1.SelectedItem.ToString() == "Asian Options")
            {
                dataGridView1.DataSource = db.Instruments1.OfType<Asian1>().ToList();
                dataGridView1.Columns["Prices"].Visible = false;
                dataGridView1.Columns["Trades"].Visible = false;
            }
            if (comboBox1.SelectedItem.ToString() == "Range Options")
            {
                dataGridView1.DataSource = db.Instruments1.OfType<Range1>().ToList();
                dataGridView1.Columns["Prices"].Visible = false;
                dataGridView1.Columns["Trades"].Visible = false;
            }
            if (comboBox1.SelectedItem.ToString() == "LookBack Options")
            {
                dataGridView1.DataSource = db.Instruments1.OfType<LookBack1>().ToList();
                dataGridView1.Columns["Prices"].Visible = false;
                dataGridView1.Columns["Trades"].Visible = false;
            }
            if (comboBox1.SelectedItem.ToString() == "Digital Options")
            {
                dataGridView1.DataSource = db.Instruments1.OfType<Digital1>().ToList();
                dataGridView1.Columns["Prices"].Visible = false;
                dataGridView1.Columns["Trades"].Visible = false;
            }
            if (comboBox1.SelectedItem.ToString() == "Barrier Options")
            {
                dataGridView1.DataSource = db.Instruments1.OfType<Barrier1>().ToList();
                dataGridView1.Columns["Prices"].Visible = false;
                dataGridView1.Columns["Trades"].Visible = false;
            }
            if (comboBox1.SelectedItem.ToString() == "Historical Prices")
            {
                dataGridView1.DataSource = db.Price1.ToList();
                dataGridView1.Columns["Instrument"].Visible = false;
            }
            if (comboBox1.SelectedItem.ToString() == "Interest Rate")
            {
                dataGridView1.DataSource = db.InterestRate1.ToList();
            }


        }
    }
}
