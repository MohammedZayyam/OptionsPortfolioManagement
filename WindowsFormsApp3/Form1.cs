using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using ConsoleApp21;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;



namespace WindowsFormsApp3
{
    
    public partial class Form1 : Form

    {
        Stopwatch watch = new Stopwatch();
        
        public double U, S, T, R, V, B, RP;
        int Trials, Steps;
        private double[,] getmatrix1(int Trials, int Steps)
        {
            double[,] matrix1;


            matrix1 = Program.RandomNumberGenerator.NormalRandomNumbers(Trials, Steps);
                        
            return matrix1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }




        public static bool CheckDatabaseExists(string dataBase)
        {
            string conStr = "Server=localhost;Integrated security=SSPI;database=master";
            string cmdText = "SELECT * FROM master.dbo.sysdatabases WHERE name ='" + dataBase + "'";
            bool isExist = false;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(cmdText, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        isExist = reader.HasRows;
                    }
                }
                con.Close();
            }
            return isExist;
        }

        public void CreateDatabase(string dataBase)
        {
            string conStr = "Server=localhost;Integrated security=SSPI;database=master";
            SqlConnection con = new SqlConnection(conStr);
            string str = "CREATE DATABASE " + dataBase;
            SqlCommand cmd = new SqlCommand(str, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            //creating database and table
            string h = "Ornagedb"; //textBox8.Text;
            String str, str1, str2;
            SqlConnection myConn = new SqlConnection("Server=localhost;Integrated security=SSPI;database=master");
            str = "CREATE DATABASE " + h + ";";
            
            SqlCommand myCommand = new SqlCommand(str, myConn);

            str1 = "CREATE TABLE "+h+".dbo.European_Option (" +
                "ID INT PRIMARY KEY IDENTITY (1,1)," +
                "CallorPut VARCHAR NOT NULL," +
                "Underlying DECIMAL NOT NULL," +
                "Strike DECIMAL NOT NULL," +
                "Tenor DECIMAL NOT NULL," +
                "Option_Price DECIMAL NOT NULL," +
                "Delta DECIMAL NOT NULL," +
                "Gamma DECIMAL NOT NULL," +
                "Theta DECIMAL NOT NULL," +
                "Vega DECIMAL NOT NULL," +
                "Rho DECIMAL NOT NULL);";
            SqlCommand myCommand1 = new SqlCommand(str1, myConn);
            //SqlCommand myCommand2 = new SqlCommand(str2, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
                //myConn.Close();
                //myConn1.Open();
                // myConn.Open();
                //myCommand2.ExecuteNonQuery();
                myCommand1.ExecuteNonQuery();
                MessageBox.Show("DataBase is Created Successfully", "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }

        }

        double[,] matrix;
        int progress = 0;
        public delegate void IncrementProgress();
        //public delegate void IncrementProgress1();
        public IncrementProgress myDelegate;
        public IncrementProgress myDelegate1;

        String C, A, CV, M, O, BT;


        //Action updateprogress, calcFinished;

        BackgroundWorker _backgroundWorker = new BackgroundWorker();
        public static int GetInstCount()
        {
            using (Model1Container db1 = new Model1Container())
            {
                return db1.Instruments1.OfType<Stock1>().Count();
            }

        }
        public List<Stock1> d = new List<Stock1>();
        public static int idk = GetInstCount();
        public ComboboxItem[] d1 = new ComboboxItem[idk];
        public Form1()
        {
            InitializeComponent();
            
            Model1Container db = new Model1Container();
            d = db.Instruments1.OfType<Stock1>().ToList();
            for (int i = 0; i < idk ; i++)
            {

                string x;
                comboBox1.Items.Add(d[i ].CompanyName);
                ComboboxItem item = new ComboboxItem();
                item.Text = Convert.ToString(d[i ].CompanyName);
                item.Value = d[i ].Id;
                d1[i] = item;

            }
            if (CheckDatabaseExists("DataModelforTrades"))
            {
                //MessageBox.Show("Database already Created!", "Database Exists", MessageBoxButtons.OK, MessageBoxIcon.Information);
 
            }
            else
            {
                CreateDatabase("DataModelforTrades");
                //MessageBox.Show("DataBase is Created Successfully!", "Database Exists", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            progressBar1.Maximum = 10000;

            //_backgroundWorker.ProgressChanged += _backgroundWorker_ProgressChanged;
            //_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {
            //this.Enabled = false;
        }

        private void label23_Click(object sender, EventArgs e)
        {
            //this.Text = "Progress";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            //
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {

        }

        //initializing delegate for ranadom numbers
        public delegate void FillRandomMatrix(double[,] randomnumbersmatrix, int starindex, int stopindex, int seed);
        //method performed inside delegate
        static FillRandomMatrix fillarray = delegate (double[,] randomnumbersmatrix, int starindex, int stopindex, int seed)
        {
            Random random1 = new Random(seed);
            for (int i = starindex; i < stopindex; i++)
            {
                for (int j = 0; j < randomnumbersmatrix.GetLength(1); j += 2)
                {

                    double x1, x2;
                    x1 = random1.NextDouble();
                    x2 = random1.NextDouble();

                    randomnumbersmatrix[i, j] = (Math.Sqrt(-2 * Math.Log(x1))) * Math.Cos(2 * Math.PI * x2);
                    randomnumbersmatrix[i, j + 1] = (Math.Sqrt(-2 * Math.Log(x1))) * Math.Sin(2 * Math.PI * x2);

                }
            }
        };
        //thread creation for random numbers
        static Thread createthread(double[,] randomnumbersmatrix, int starindex, int stopindex, int seed)
        {
            Thread th = new Thread(new ThreadStart(() => fillarray(randomnumbersmatrix, starindex, stopindex, seed)));
            return th;
        }

        public int selectedValue1;

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
        public delegate void ReportProgressDelegate(int percentage);
        string a1, a2, a3, a4, a5, a6, a7, a8;
        public class ComboboxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {


            //Thread t3 = new Thread(new ThreadStart(FormCalc));
            //t = t3;
            //t.Start();
            watch.Start();
            if (radioButton3.Checked == true || radioButton4.Checked == true || radioButton5.Checked == true || radioButton6.Checked == true || radioButton7.Checked == true || radioButton8.Checked == true)
            {
                if (radioButton3.Checked == true)
                {
                    O = "E";
                }
                if (radioButton4.Checked == true)
                {
                    O = "B";
                }
                if (radioButton5.Checked == true)
                {
                    O = "LB";
                }
                if (radioButton6.Checked == true)
                {
                    O = "R";
                }
                if (radioButton7.Checked == true)
                {
                    O = "D";
                }
                if (radioButton8.Checked == true)
                {
                    O = "A";
                }
            }
            else
            {
                MessageBox.Show("Please select either One of the Option Types", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
           
            try
            {
                for (int i = 0; i < idk; i++)
                {
                    if (d1[i].Text == Inst)
                    {
                        selectedValue1 = d1[i].Value;
                        Console.WriteLine("i:{0}, {1}", d1[i].Text, Inst);
                        break;
                    }

                }
                //selectedValue1;
            }
            catch
            {
                MessageBox.Show("Please select an underlying or add historical price before proceesding", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            U = GetUnderlying.GetClosingPrice(selectedValue1); //Convert.ToDouble(textBox1.Text);
            S = Convert.ToDouble(textBox2.Text);
            T = Convert.ToDouble(textBox3.Text);
            V = Convert.ToDouble(Program.Vuniversal) / 100;
            Console.WriteLine("U:{0}, S:{1}, T: {2},  V:{3}", U, S, T, V);

            Trials = Program.Trial_uni; //Convert.ToInt32(textBox6.Text);
            Steps = Program.Steps_uni;//Convert.ToInt32(textBox7.Text);
                                      //RP = Convert.ToDouble(textBox10);
            try
            {
               

            }
            catch
            {
                MessageBox.Show("Please check the input and enter the right type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                R = Program.GetInterestRate(T) /100;
            }
            catch
            {
                MessageBox.Show("Please add appropriate Interest Rates before Proceeding", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (radioButton1.Checked == true || radioButton2.Checked == true)
            {
                if (radioButton1.Checked == true)
                {
                    C = "C";
                }
                if (radioButton2.Checked == true)
                {
                    C = "P";
                }
            }
            //else
            //{
            //    MessageBox.Show("Please select either Call or Put Option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //if (checkBox1.Checked == true)
            //{
            //    A = "A";//anthithetic
            //}
            //else
            //{
            //    A = "Not A";
            //}
            //if (checkBox2.Checked == true)
            //{
            //    CV = "CV";
            //}
            //else
            //{
            //    CV = "Not CV";
            //}
            //if (checkBox3.Checked == true)
            //{

            //    M = "M";
            //}
            //else
            //{
            //    M = "Not M";
            //}
            M = Program.M_uni;
            CV = Program.CV_uni;
            A = Program.A_uni;
            matrix = getmatrix1(Trials, Steps);
            if (O =="E")
            {

                Program.European_Option first_option = new Program.European_Option
                {
                    U = U,
                    S = S,
                    T = T,
                    R = R,
                    D = R,
                    V = V,
                    Trials = Trials,
                    Steps = Steps,
                    matrix1 = matrix,
                    CorP = C,
                    A = A,
                    CV = CV ,
                    M = M
                };
                int[] incrementvalues = first_option.incrementmatrix;
                double[,] randomnumbers = new double[first_option.Trials, first_option.Steps];
                //double[][,] simulatedmatrix = Simulations.CreateSimulatedMatrix(first_option.CV, first_option.A, first_option.Trials, first_option.Steps);
                if (first_option.M == "M")
                {

                    //list of threads for random values
                    List<Thread> threadlist = new List<Thread>(first_option.z);
                    int seed1 = (int)DateTime.Now.Ticks;
                    // starting threadThread for random values
                    for (int i = 0; i < first_option.z; i++)
                    {
                        int seed = seed1 + i;
                        Thread t1 = createthread(randomnumbers, incrementvalues[i], incrementvalues[i + 1], seed);
                        threadlist.Add(t1);
                        t1.Start();
                    }
                    //waiting for all random numbers to be generated
                    for (int i = 0; i < first_option.z; i++)
                    {
                        threadlist[i].Join();
                    }
                    first_option.matrix1 = randomnumbers;

                }
                else
                {
                    //no multithreading
                    randomnumbers = Program.RandomNumberGenerator.NormalRandomNumbers(first_option.Trials, first_option.Steps);
                    first_option.matrix1 = randomnumbers;

                }
                a1 = Convert.ToString(first_option.Option_Price);
                a2 = Convert.ToString(first_option.Delta);
                a3 = Convert.ToString(first_option.Gamma);
                a4 = Convert.ToString(first_option.Theta);
                a5 = Convert.ToString(first_option.Vega);
                a6 = Convert.ToString(first_option.Rho);
                a7 = Convert.ToString(first_option.Standard_Deviation);
                a8 = Convert.ToString(first_option.z);
            }//European Option
            if (O == "A")
            {
                Program.Asian_Option first_option = new Program.Asian_Option
                {
                    U = U,
                    S = S,
                    T = T,
                    R = R,
                    D = R,
                    V = V,
                    Trials = Trials,
                    Steps = Steps,
                    matrix1 = matrix,
                    CorP = C,
                    A = A,
                    CV = CV,
                    M = M
                };
                int[] incrementvalues = first_option.incrementmatrix;
                double[,] randomnumbers = new double[first_option.Trials, first_option.Steps];
                //double[][,] simulatedmatrix = Simulations.CreateSimulatedMatrix(first_option.CV, first_option.A, first_option.Trials, first_option.Steps);
                if (first_option.M == "M")
                {

                    //list of threads for random values
                    List<Thread> threadlist = new List<Thread>(first_option.z);
                    int seed1 = (int)DateTime.Now.Ticks;
                    // starting threadThread for random values
                    for (int i = 0; i < first_option.z; i++)
                    {
                        int seed = seed1 + i;
                        Thread t1 = createthread(randomnumbers, incrementvalues[i], incrementvalues[i + 1], seed);
                        threadlist.Add(t1);
                        t1.Start();
                    }
                    //waiting for all random numbers to be generated
                    for (int i = 0; i < first_option.z; i++)
                    {
                        threadlist[i].Join();
                    }
                    first_option.matrix1 = randomnumbers;

                }
                else
                {
                    //no multithreading
                    randomnumbers = Program.RandomNumberGenerator.NormalRandomNumbers(first_option.Trials, first_option.Steps);
                    first_option.matrix1 = randomnumbers;

                }
                a1 = Convert.ToString(first_option.Option_Price);
                a2 = Convert.ToString(first_option.Delta);
                a3 = Convert.ToString(first_option.Gamma);
                a4 = Convert.ToString(first_option.Theta);
                a5 = Convert.ToString(first_option.Vega);
                a6 = Convert.ToString(first_option.Rho);
                a7 = Convert.ToString(first_option.Standard_Deviation);
                a8 = Convert.ToString(first_option.z);
            }//Asian Option
            if (O == "R")
            {
                Program.Range_Option first_option = new Program.Range_Option
                {
                    U = U,
                    S = S,
                    T = T,
                    R = R,
                    D = R,
                    V = V,
                    Trials = Trials,
                    Steps = Steps,
                    matrix1 = matrix,
                    CorP = C,
                    A = A,
                    CV = CV,
                    M = M
                };
                int[] incrementvalues = first_option.incrementmatrix;
                double[,] randomnumbers = new double[first_option.Trials, first_option.Steps];
                //double[][,] simulatedmatrix = Simulations.CreateSimulatedMatrix(first_option.CV, first_option.A, first_option.Trials, first_option.Steps);
                if (first_option.M == "M")
                {

                    //list of threads for random values
                    List<Thread> threadlist = new List<Thread>(first_option.z);
                    int seed1 = (int)DateTime.Now.Ticks;
                    // starting threadThread for random values
                    for (int i = 0; i < first_option.z; i++)
                    {
                        int seed = seed1 + i;
                        Thread t1 = createthread(randomnumbers, incrementvalues[i], incrementvalues[i + 1], seed);
                        threadlist.Add(t1);
                        t1.Start();
                    }
                    //waiting for all random numbers to be generated
                    for (int i = 0; i < first_option.z; i++)
                    {
                        threadlist[i].Join();
                    }
                    first_option.matrix1 = randomnumbers;

                }
                else
                {
                    //no multithreading
                    randomnumbers = Program.RandomNumberGenerator.NormalRandomNumbers(first_option.Trials, first_option.Steps);
                    first_option.matrix1 = randomnumbers;

                }
                a1 = Convert.ToString(first_option.Option_Price);
                a2 = Convert.ToString(first_option.Delta);
                a3 = Convert.ToString(first_option.Gamma);
                a4 = Convert.ToString(first_option.Theta);
                a5 = Convert.ToString(first_option.Vega);
                a6 = Convert.ToString(first_option.Rho);
                a7 = Convert.ToString(first_option.Standard_Deviation);
                a8 = Convert.ToString(first_option.z);
            }//Range Option
            if (O == "LB")
            {
                Program.FixedStrikeLookback_Option first_option = new Program.FixedStrikeLookback_Option
                {
                    U = U,
                    S = S,
                    T = T,
                    R = R,
                    D = R,
                    V = V,
                    Trials = Trials,
                    Steps = Steps,
                    matrix1 = matrix,
                    CorP = C,
                    A = A,
                    CV = CV,
                    M = M
                };
                int[] incrementvalues = first_option.incrementmatrix;
                double[,] randomnumbers = new double[first_option.Trials, first_option.Steps];
                //double[][,] simulatedmatrix = Simulations.CreateSimulatedMatrix(first_option.CV, first_option.A, first_option.Trials, first_option.Steps);
                if (first_option.M == "M")
                {

                    //list of threads for random values
                    List<Thread> threadlist = new List<Thread>(first_option.z);
                    int seed1 = (int)DateTime.Now.Ticks;
                    // starting threadThread for random values
                    for (int i = 0; i < first_option.z; i++)
                    {
                        int seed = seed1 + i;
                        Thread t1 = createthread(randomnumbers, incrementvalues[i], incrementvalues[i + 1], seed);
                        threadlist.Add(t1);
                        t1.Start();
                    }
                    //waiting for all random numbers to be generated
                    for (int i = 0; i < first_option.z; i++)
                    {
                        threadlist[i].Join();
                    }
                    first_option.matrix1 = randomnumbers;

                }
                else
                {
                    //no multithreading
                    randomnumbers = Program.RandomNumberGenerator.NormalRandomNumbers(first_option.Trials, first_option.Steps);
                    first_option.matrix1 = randomnumbers;

                }
                a1 = Convert.ToString(first_option.Option_Price);
                a2 = Convert.ToString(first_option.Delta);
                a3 = Convert.ToString(first_option.Gamma);
                a4 = Convert.ToString(first_option.Theta);
                a5 = Convert.ToString(first_option.Vega);
                a6 = Convert.ToString(first_option.Rho);
                a7 = Convert.ToString(first_option.Standard_Deviation);
                a8 = Convert.ToString(first_option.z);
            }//Lookback Option
            if (O == "B")
            {
                //groupBox7.Enabled = true;
                try
                {
                    B = Convert.ToDouble(textBox9.Text);
                }
                catch
                {
                    MessageBox.Show("Please check the input and enter the right type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (radioButton9.Checked == true || radioButton10.Checked == true || radioButton11.Checked == true || radioButton12.Checked == true)
                {
                    if (radioButton9.Checked == true)
                    {
                        BT = "UI";
                    }
                    if (radioButton12.Checked == true)
                    {
                        BT = "DI";
                    }
                    if (radioButton10.Checked == true)
                    {
                        BT = "UO";
                    }
                    if (radioButton11.Checked == true)
                    {
                        BT = "DO";
                    }
                }
                if (BT== "UI" && (U>B))
                {
                    MessageBox.Show("The underlying cannot be greater than the barrier price for this option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //MessageBox.Show("The underlying cannot be greater than the barrier price for this option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (BT == "UO" && (U > B))
                {
                    MessageBox.Show("The underlying cannot be greater than the barrier price for this option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (BT == "DO" && (U < B))
                {
                    MessageBox.Show("The underlying cannot be less than the bar rier price for this option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (BT == "DI" && (U < B))
                {
                    MessageBox.Show("The underlying cannot be less than the barrier price for this option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Program.Barrier first_option = new Program.Barrier
                {
                    U = U,
                    S = S,
                    T = T,
                    R = R,
                    D = R,
                    V = V,
                    Trials = Trials,
                    Steps = Steps,
                    matrix1 = matrix,
                    CorP = C,
                    A = A,
                    CV = CV,
                    M = M,
                    Type = BT,
                    BarrierLine= B

                };
                int[] incrementvalues = first_option.incrementmatrix;
                double[,] randomnumbers = new double[first_option.Trials, first_option.Steps];
                //double[][,] simulatedmatrix = Simulations.CreateSimulatedMatrix(first_option.CV, first_option.A, first_option.Trials, first_option.Steps);
                if (first_option.M == "M")
                {

                    //list of threads for random values
                    List<Thread> threadlist = new List<Thread>(first_option.z);
                    int seed1 = (int)DateTime.Now.Ticks;
                    // starting threadThread for random values
                    for (int i = 0; i < first_option.z; i++)
                    {
                        int seed = seed1 + i;
                        Thread t1 = createthread(randomnumbers, incrementvalues[i], incrementvalues[i + 1], seed);
                        threadlist.Add(t1);
                        t1.Start();
                    }
                    //waiting for all random numbers to be generated
                    for (int i = 0; i < first_option.z; i++)
                    {
                        threadlist[i].Join();
                    }
                    first_option.matrix1 = randomnumbers;

                }
                else
                {
                    //no multithreading
                    randomnumbers = Program.RandomNumberGenerator.NormalRandomNumbers(first_option.Trials, first_option.Steps);
                    first_option.matrix1 = randomnumbers;

                }
                a1 = Convert.ToString(first_option.Option_Price);
                a2 = Convert.ToString(first_option.Delta);
                a3 = Convert.ToString(first_option.Gamma);
                a4 = Convert.ToString(first_option.Theta);
                a5 = Convert.ToString(first_option.Vega);
                a6 = Convert.ToString(first_option.Rho);
                a7 = Convert.ToString(first_option.Standard_Deviation);
                a8 = Convert.ToString(first_option.z);
                //groupBox7.Enabled = false;
            }//Barrier Option
            if (O == "D")
            {
                try
                {
                    RP = Convert.ToDouble(textBox10.Text);
                }
                catch
                {
                    MessageBox.Show("Please check the input and enter the right type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Program.Digital_Option first_option = new Program.Digital_Option
                {
                    U = U,
                    S = S,
                    T = T,
                    R = R,
                    D = R,
                    V = V,
                    Trials = Trials,
                    Steps = Steps,
                    matrix1 = matrix,
                    CorP = C,
                    A = A,
                    CV = CV,
                    M = M,
                    RP=RP
                };
                int[] incrementvalues = first_option.incrementmatrix;
                double[,] randomnumbers = new double[first_option.Trials, first_option.Steps];
                //double[][,] simulatedmatrix = Simulations.CreateSimulatedMatrix(first_option.CV, first_option.A, first_option.Trials, first_option.Steps);
                if (first_option.M == "M")
                {

                    //list of threads for random values
                    List<Thread> threadlist = new List<Thread>(first_option.z);
                    int seed1 = (int)DateTime.Now.Ticks;
                    // starting threadThread for random values
                    for (int i = 0; i < first_option.z; i++)
                    {
                        int seed = seed1 + i;
                        Thread t1 = createthread(randomnumbers, incrementvalues[i], incrementvalues[i + 1], seed);
                        threadlist.Add(t1);
                        t1.Start();
                    }
                    //waiting for all random numbers to be generated
                    for (int i = 0; i < first_option.z; i++)
                    {
                        threadlist[i].Join();
                    }
                    first_option.matrix1 = randomnumbers;

                }
                else
                {
                    //no multithreading
                    randomnumbers = Program.RandomNumberGenerator.NormalRandomNumbers(first_option.Trials, first_option.Steps);
                    first_option.matrix1 = randomnumbers;

                }
                a1 = Convert.ToString(first_option.Option_Price);
                a2 = Convert.ToString(first_option.Delta);
                a3 = Convert.ToString(first_option.Gamma);
                a4 = Convert.ToString(first_option.Theta);
                a5 = Convert.ToString(first_option.Vega);
                a6 = Convert.ToString(first_option.Rho);
                a7 = Convert.ToString(first_option.Standard_Deviation);
                a8 = Convert.ToString(first_option.z);
            }//Digital Option
        }

        public static string Inst;
        private void button1_Click(object sender, EventArgs e)
        {
            Inst = comboBox1.SelectedItem.ToString();
            progressBar1.Value = 0;
            label27.Text = "Progress";
            this._backgroundWorker.RunWorkerAsync();
            button1.Enabled = false;
          
            while (this._backgroundWorker.IsBusy)
            {

                int[,] abc = new int[5000, 5000];
                for (long c = 0; c < 5000; c++)
                {
                    for (long a = 0; a < 5000; a++)
                    {
                        for (long b = 0; b < 50; b++) abc[a, b] = 50 * 50;

                    }
                    if (progressBar1.Value == 9999)
                    {
                       
                        progressBar1.Value = progressBar1.Maximum;
                        label27.Text = "Complete";
                        break;
                    }

                    if (progressBar1.Value<10000)
                    {
                        progressBar1.Value++;
                        double x = Convert.ToDouble(progressBar1.Value) / 10000 * 100;
                        x = Math.Round(x, 1);
                        label27.Text =  Convert.ToString(x)+ "%";
                        Application.DoEvents();
                    }
                    if(this._backgroundWorker.IsBusy == false)
                    {
                        break;
                    }
                    
                }
                if (this._backgroundWorker.IsBusy == false)
                {
                    break;
                }

            }
            
            label15.Text = a1;
            label16.Text = a2;
            label17.Text = a3;
            label18.Text = a4;
            label19.Text = a5;
            label20.Text = a6;
            label21.Text = a7;
            label26.Text = a8;
            progressBar1.Value = progressBar1.Maximum;
            label27.Text = "Complete";




            //t.Abort();
            //t.Suspend();


            watch.Stop();
            label22.Text = watch.Elapsed.Hours.ToString() +":" +watch.Elapsed.Minutes.ToString() + ":"+watch.Elapsed.Seconds.ToString() + ":" + watch.Elapsed.Milliseconds.ToString();
            watch.Reset();
            button1.Enabled = true;
            //Thread t2 = new Thread(new ThreadStart(Endtask));
            //t2.Start();
            


        }
    }
}
