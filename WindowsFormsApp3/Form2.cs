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
using System.Threading;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;


namespace WindowsFormsApp3
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            Model1Container db = new Model1Container();

            dataGridView2.DataSource = db.Trade1.ToList();

            dataGridView2.Columns["IsBuy"].Visible = false;
            dataGridView2.Columns["Instrument"].Visible = false;
            DataGridViewColumn MarkPrice = new DataGridViewColumn();
            MarkPrice.Name = "MarkPrice";
            MarkPrice.ValueType = Type.GetType("double");
            //MarkPrice.CellType = Type.GetType("double");
            MarkPrice.CellTemplate = new DataGridViewTextBoxCell();
            dataGridView2.Columns.Insert(8, MarkPrice);
            DataGridViewColumn PL = new DataGridViewColumn();
            PL.Name = "P&L";
            PL.CellTemplate = new DataGridViewTextBoxCell();
            PL.ValueType = Type.GetType("double");
            dataGridView2.Columns.Insert(9, PL);
            DataGridViewColumn Delta = new DataGridViewColumn();
            Delta.Name = "Delta";
            Delta.CellTemplate = new DataGridViewTextBoxCell();
            Delta.ValueType = Type.GetType("double");
            dataGridView2.Columns.Insert(10, Delta);
            DataGridViewColumn Gamma = new DataGridViewColumn();
            Gamma.Name = "Gamma";
            Gamma.CellTemplate = new DataGridViewTextBoxCell();
            Gamma.ValueType = Type.GetType("double");
            dataGridView2.Columns.Insert(11, Gamma);
            DataGridViewColumn Vega = new DataGridViewColumn();
            Vega.Name = "Vega";
            Vega.CellTemplate = new DataGridViewTextBoxCell();
            Vega.ValueType = Type.GetType("double");
            dataGridView2.Columns.Insert(12, Vega);
            DataGridViewColumn Theta = new DataGridViewColumn();
            Theta.Name = "Theta";
            Theta.CellTemplate = new DataGridViewTextBoxCell();
            Theta.ValueType = Type.GetType("double");
            dataGridView2.Columns.Insert(13, Theta);
            DataGridViewColumn Rho = new DataGridViewColumn();
            Rho.Name = "Rho";
            Rho.CellTemplate = new DataGridViewTextBoxCell();
            Rho.ValueType = Type.GetType("double");
            dataGridView2.Columns.Insert(14, Rho);
            DataGridViewColumn DT = new DataGridViewColumn();
            DT.Name = "InstrumentType";
            DT.CellTemplate = new DataGridViewTextBoxCell();
            DT.ValueType = Type.GetType("string");
            dataGridView2.Columns.Insert(15, DT);
            //Output
            dataGridView1.ColumnCount = 6;
            dataGridView1.Columns[0].Name = "P&L";
            dataGridView1.Columns[1].Name = "Delta";
            dataGridView1.Columns[2].Name = "Gamma";
            dataGridView1.Columns[3].Name = "Vega";
            dataGridView1.Columns[4].Name = "Theta";
            dataGridView1.Columns[5].Name = "Rho"; 
            //dataGridViewColumn x = 

        }
        private void dataGridView1_CellFormatting(object sender,
    DataGridViewCellFormattingEventArgs e)
        {
            String value = e.Value as string;
            if ((value != null) && value.Equals(e.CellStyle.DataSourceNullValue))
            {
                e.Value = e.CellStyle.NullValue;
                e.FormattingApplied = true;
            }
        }
        public void GenerateMarkPrice()
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            

        }
        private void dataGridView1_CellValueChanged( object sender, DataGridViewCellEventArgs e)
        {
            //UpdateBalance();
        }
        private void DataGridView1_RowsRemoved(
    object sender, DataGridViewRowsRemovedEventArgs e)
        {
            // Update the balance column whenever rows are deleted.
            //UpdateBalance();
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
        static Thread createthread(double[,] randomnumbersmatrix, int starindex, int stopindex, int seed)
        {
            Thread th = new Thread(new ThreadStart(() => fillarray(randomnumbersmatrix, starindex, stopindex, seed)));
            return th;
        }

        private void UpdateBalance()
        {
            int counter;

            //int withdrawal;
            int InstID;
            double Tenor = 0, Strike = 0, Underlying = 0;
            string y;
            char x;
            Model1Container db = new Model1Container();
            // Iterate through the rows, skipping the Starting Balance row.
            for (counter = 0; counter < (dataGridView2.Rows.Count); 
                counter++)
            {
                //deposit = 0;
                //withdrawal = 0;
                //balance = int.Parse(dataGridView1.Rows[counter - 1].Cells["Balance"].Value.ToString());


                //find out instrument ID
                InstID = Convert.ToInt32(dataGridView2.Rows[counter ].Cells["InstrumentsID"].Value);
                //find out instrument type
                y = Convert.ToString(db.Instruments1.Find(InstID).GetType());
                x = y[34];
                // if instrument is a stock markprice=underlying, delta =1
                if (Convert.ToString(x) == "S")
                {
                    Stock1 s = new Stock1();
                    s = db.Instruments1.OfType<Stock1>().Where(m => m.Id == InstID).FirstOrDefault();
                    Underlying = s.Underlying;
                    dataGridView2.Rows[counter].Cells["MarkPrice"].Value = Underlying;
                    dataGridView2.Rows[counter].Cells["Delta"].Value = 1;
                    dataGridView2.Rows[counter].Cells["Gamma"].Value = 0;
                    dataGridView2.Rows[counter].Cells["Vega"].Value = 0;
                    dataGridView2.Rows[counter].Cells["Theta"].Value = 0;
                    dataGridView2.Rows[counter].Cells["Rho"].Value = 0;
                }
                if (Convert.ToString(x)=="E")
                {
                    European1 z = new European1();
                    z = db.Instruments1.OfType<European1>().Where(m => m.Id == InstID).FirstOrDefault();
                    string c;
                    if (z.IsCall == "Call")
                    {
                        c = "C";
                    }
                    else { c = "NorC"; }
                    Program.European_Option first_option = new Program.European_Option
                    {

                        U = GetUnderlying.GetClosingPrice(Convert.ToInt32(z.Underlying)),
                        S = z.Strike,
                        T = z.ExpirationDate,
                        R = Program.GetInterestRate(z.ExpirationDate)/100,
                        D = Program.GetInterestRate(z.ExpirationDate)/100,
                        V = Program.Vuniversal/100,
                        Trials = Program.Trial_uni,
                        Steps = Program.Steps_uni,
                        matrix1 = Program.RandomNumberGenerator.NormalRandomNumbers(Program.Trial_uni, Program.Steps_uni),
                        CorP = c,
                        A = Program.A_uni,
                        CV = Program.CV_uni,
                        M = Program.M_uni


                    };
                    Console.WriteLine("U:{0}, s:{1}, T:{2}, R:{3}, D:{4}, v:{5}, TRials:{6}, Steps:{7}, CorP:{8}", first_option.U, first_option.S, first_option.T, first_option.R, 0, first_option.V, first_option.Trials, first_option.Steps, first_option.CorP);
                    dataGridView2.Rows[counter].Cells["MarkPrice"].Value = first_option.Option_Price;
                    Console.WriteLine("Mark Price :{0}", first_option.Option_Price);
                    dataGridView2.Rows[counter].Cells["Delta"].Value = first_option.Delta;
                    dataGridView2.Rows[counter].Cells["Gamma"].Value = first_option.Gamma;
                    dataGridView2.Rows[counter].Cells["Vega"].Value = first_option.Vega;
                    dataGridView2.Rows[counter].Cells["Theta"].Value = first_option.Theta;
                    dataGridView2.Rows[counter].Cells["Rho"].Value = first_option.Rho;
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
                    dataGridView2.Rows[counter].Cells["MarkPrice"].Value = first_option.Option_Price;
                    Console.WriteLine("Mark Price :{0}", first_option.Option_Price);
                    dataGridView2.Rows[counter].Cells["Delta"].Value = first_option.Delta;
                    dataGridView2.Rows[counter].Cells["Gamma"].Value = first_option.Gamma;
                    dataGridView2.Rows[counter].Cells["Vega"].Value = first_option.Vega;
                    dataGridView2.Rows[counter].Cells["Theta"].Value = first_option.Theta;
                    dataGridView2.Rows[counter].Cells["Rho"].Value = first_option.Rho;

                }

                if (Convert.ToString(x) == "A")
                {
                    Asian1 z = new Asian1();
                    z = db.Instruments1.OfType<Asian1>().Where(m => m.Id == InstID).FirstOrDefault();
                    string c;
                    if (z.IsCall == "Call")
                    {
                        c = "C";
                    }
                    else { c = "NorC"; }
                    Program.European_Option first_option = new Program.European_Option
                    {

                        U = GetUnderlying.GetClosingPrice(Convert.ToInt32(z.Underlying)),
                        S = z.Strike,
                        T = z.ExpirationDate,
                        R = Program.GetInterestRate(z.ExpirationDate) / 100,
                        D = Program.GetInterestRate(z.ExpirationDate) / 100,
                        //V = Program.Vuniversal/100,
                        Trials = Program.Trial_uni,
                        Steps = Program.Steps_uni,
                        matrix1 = Program.RandomNumberGenerator.NormalRandomNumbers(Program.Trial_uni, Program.Steps_uni),
                        CorP = c,
                        A = Program.A_uni,
                        CV = Program.CV_uni,
                        M = Program.M_uni


                    };
                    Console.WriteLine("U:{0}, s:{1}, T:{2}, R:{3}, D:{4}, v:{5}, TRials:{6}, Steps:{7}, CorP:{8}", first_option.U, first_option.S, first_option.T, first_option.R, 0, first_option.V, first_option.Trials, first_option.Steps, first_option.CorP);

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
                    dataGridView2.Rows[counter].Cells["MarkPrice"].Value = first_option.Option_Price;
                    Console.WriteLine("Mark Price :{0}", first_option.Option_Price);
                    dataGridView2.Rows[counter].Cells["Delta"].Value = first_option.Delta;
                    dataGridView2.Rows[counter].Cells["Gamma"].Value = first_option.Gamma;
                    dataGridView2.Rows[counter].Cells["Vega"].Value = first_option.Vega;
                    dataGridView2.Rows[counter].Cells["Theta"].Value = first_option.Theta;
                    dataGridView2.Rows[counter].Cells["Rho"].Value = first_option.Rho;

                }
                if (Convert.ToString(x) == "R")
                {
                    Range1 z = new Range1();
                    z = db.Instruments1.OfType<Range1>().Where(m => m.Id == InstID).FirstOrDefault();
                    string c;
                    /*if (z.IsCall == "Call")
                    {
                        c = "C";
                    }
                    else { c = "NorC"; }*/
                    Program.Range_Option first_option = new Program.Range_Option
                    {

                        U = GetUnderlying.GetClosingPrice(Convert.ToInt32(z.Underlying)),
                        S = z.Strike,
                        T = z.ExpirationDate,
                        R = Program.GetInterestRate(z.ExpirationDate) / 100,
                        D = Program.GetInterestRate(z.ExpirationDate) / 100,
                        V = Program.Vuniversal/100,
                        Trials = Program.Trial_uni,
                        Steps = Program.Steps_uni,
                        matrix1 = Program.RandomNumberGenerator.NormalRandomNumbers(Program.Trial_uni, Program.Steps_uni),
                        //CorP = c,
                        A = Program.A_uni,
                        CV = Program.CV_uni,
                        M = Program.M_uni


                    };
                    Console.WriteLine("U:{0}, s:{1}, T:{2}, R:{3}, D:{4}, v:{5}, TRials:{6}, Steps:{7}, CorP:{8}", first_option.U, first_option.S, first_option.T, first_option.R, 0, first_option.V, first_option.Trials, first_option.Steps, first_option.CorP);

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
                    dataGridView2.Rows[counter].Cells["MarkPrice"].Value = first_option.Option_Price;
                    Console.WriteLine("Mark Price :{0}", first_option.Option_Price);
                    dataGridView2.Rows[counter].Cells["Delta"].Value = first_option.Delta;
                    dataGridView2.Rows[counter].Cells["Gamma"].Value = first_option.Gamma;
                    dataGridView2.Rows[counter].Cells["Vega"].Value = first_option.Vega;
                    dataGridView2.Rows[counter].Cells["Theta"].Value = first_option.Theta;
                    dataGridView2.Rows[counter].Cells["Rho"].Value = first_option.Rho;

                }
                if (Convert.ToString(x) == "L")
                {
                    LookBack1 z = new LookBack1();
                    z = db.Instruments1.OfType<LookBack1>().Where(m => m.Id == InstID).FirstOrDefault();
                    string c;
                    if (z.IsCall == "Call")
                    {
                        c = "C";
                    }
                    else { c = "NorC"; }
                    Program.FixedStrikeLookback_Option  first_option = new Program.FixedStrikeLookback_Option
                    {

                        U = GetUnderlying.GetClosingPrice(Convert.ToInt32(z.Underlying)),
                        S = z.Strike,
                        T = z.ExpirationDate,
                        R = Program.GetInterestRate(z.ExpirationDate) / 100,
                        D = Program.GetInterestRate(z.ExpirationDate) / 100,
                        V = Program.Vuniversal/100,
                        Trials = Program.Trial_uni,
                        Steps = Program.Steps_uni,
                        matrix1 = Program.RandomNumberGenerator.NormalRandomNumbers(Program.Trial_uni, Program.Steps_uni),
                        CorP = c,
                        A = Program.A_uni,
                        CV = Program.CV_uni,
                        M = Program.M_uni


                    };
                    Console.WriteLine("U:{0}, s:{1}, T:{2}, R:{3}, D:{4}, v:{5}, TRials:{6}, Steps:{7}, CorP:{8}", first_option.U, first_option.S, first_option.T, first_option.R, 0, first_option.V, first_option.Trials, first_option.Steps, first_option.CorP);

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
                    dataGridView2.Rows[counter].Cells["MarkPrice"].Value = first_option.Option_Price;
                    Console.WriteLine("Mark Price :{0}", first_option.Option_Price);
                    dataGridView2.Rows[counter].Cells["Delta"].Value = first_option.Delta;
                    dataGridView2.Rows[counter].Cells["Gamma"].Value = first_option.Gamma;
                    dataGridView2.Rows[counter].Cells["Vega"].Value = first_option.Vega;
                    dataGridView2.Rows[counter].Cells["Theta"].Value = first_option.Theta;
                    dataGridView2.Rows[counter].Cells["Rho"].Value = first_option.Rho;

                }
                if (Convert.ToString(x) == "D")
                {
                    Digital1 z = new Digital1();
                    z = db.Instruments1.OfType<Digital1>().Where(m => m.Id == InstID).FirstOrDefault();
                    string c;
                    if (z.IsCall == "Call")
                    {
                        c = "C";
                    }
                    else { c = "NorC"; }
                    Program.Digital_Option first_option = new Program.Digital_Option
                    {

                        U = GetUnderlying.GetClosingPrice(Convert.ToInt32(z.Underlying)),
                        S = z.Strike,
                        T = z.ExpirationDate,
                        R = Program.GetInterestRate(z.ExpirationDate) / 100,
                        D = Program.GetInterestRate(z.ExpirationDate) / 100,
                        V = Program.Vuniversal/100,
                        Trials = Program.Trial_uni,
                        Steps = Program.Steps_uni,
                        matrix1 = Program.RandomNumberGenerator.NormalRandomNumbers(Program.Trial_uni, Program.Steps_uni),
                        CorP = c,
                        A = Program.A_uni,
                        CV = Program.CV_uni,
                        M = Program.M_uni,
                        RP= z.Rebate


                    };
                    Console.WriteLine("U:{0}, s:{1}, T:{2}, R:{3}, D:{4}, v:{5}, TRials:{6}, Steps:{7}, CorP:{8}", first_option.U, first_option.S, first_option.T, first_option.R, 0, first_option.V, first_option.Trials, first_option.Steps, first_option.CorP);

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
                    dataGridView2.Rows[counter].Cells["MarkPrice"].Value = first_option.Option_Price;
                    Console.WriteLine("Mark Price :{0}", first_option.Option_Price);
                    dataGridView2.Rows[counter].Cells["Delta"].Value = first_option.Delta;
                    dataGridView2.Rows[counter].Cells["Gamma"].Value = first_option.Gamma;
                    dataGridView2.Rows[counter].Cells["Vega"].Value = first_option.Vega;
                    dataGridView2.Rows[counter].Cells["Theta"].Value = first_option.Theta;
                    dataGridView2.Rows[counter].Cells["Rho"].Value = first_option.Rho;


                }
                if(Convert.ToString(x) == "B")
                {
                    Barrier1 z = new Barrier1();
                    z = db.Instruments1.OfType<Barrier1>().Where(m => m.Id == InstID).FirstOrDefault();
                    string c;
                    if (z.IsCall == "Call")
                    {
                        c = "C";
                    }
                    else { c = "NorC"; }
                    string BT;
                    if (z.BarrierType == "Up and In")
                    {
                        BT = "UI";
                    }
                    if (z.BarrierType == "Up and Out")
                    {
                        BT = "UO";
                    }
                    if (z.BarrierType == "Down and In")
                    {
                        BT = "DI";
                    }
                    else
                    {
                        BT = "DO";
                    }
                    Program.Barrier first_option = new Program.Barrier
                    {

                        U = GetUnderlying.GetClosingPrice(Convert.ToInt32(z.Underlying)),
                        S = z.Strike,
                        T = z.ExpirationDate,
                        R = Program.GetInterestRate(z.ExpirationDate) / 100,
                        D = Program.GetInterestRate(z.ExpirationDate) / 100,
                        V = Program.Vuniversal/100,
                        Trials = Program.Trial_uni,
                        Steps = Program.Steps_uni,
                        matrix1 = Program.RandomNumberGenerator.NormalRandomNumbers(Program.Trial_uni, Program.Steps_uni),
                        CorP = c,
                        A = Program.A_uni,
                        CV = Program.CV_uni,
                        M = Program.M_uni,
                        BarrierLine = z.BarrierPrice,
                        Type =z.BarrierType


                    };
                    Console.WriteLine("U:{0}, s:{1}, T:{2}, R:{3}, D:{4}, v:{5}, TRials:{6}, Steps:{7}, CorP:{8}", first_option.U, first_option.S, first_option.T, first_option.R, 0, first_option.V, first_option.Trials, first_option.Steps, first_option.CorP);

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
                    dataGridView2.Rows[counter].Cells["MarkPrice"].Value = first_option.Option_Price;
                    Console.WriteLine("Mark Price :{0}", first_option.Option_Price);
                    dataGridView2.Rows[counter].Cells["Delta"].Value = first_option.Delta;
                    dataGridView2.Rows[counter].Cells["Gamma"].Value = first_option.Gamma;
                    dataGridView2.Rows[counter].Cells["Vega"].Value = first_option.Vega;
                    dataGridView2.Rows[counter].Cells["Theta"].Value = first_option.Theta;
                    dataGridView2.Rows[counter].Cells["Rho"].Value = first_option.Rho;

                   

                }
                //dataGridView1.Update();
                dataGridView2.Update();
                //dataGridView1.Refresh();
                dataGridView2.Refresh();
                if (Convert.ToString(dataGridView2.Rows[counter].Cells["Direction"].Value) == "Buy")
                {
                    dataGridView2.Rows[counter].Cells["P&L"].Value = ((Convert.ToDouble(dataGridView2.Rows[counter].Cells["MarkPrice"].Value) - Convert.ToDouble(dataGridView2.Rows[counter].Cells["Price"].Value))*Convert.ToDouble(dataGridView2.Rows[counter].Cells["Quantity"].Value));
                }
                else
                {
                    dataGridView2.Rows[counter].Cells["P&L"].Value = ((Convert.ToDouble(dataGridView2.Rows[counter].Cells["Price"].Value) - Convert.ToDouble(dataGridView2.Rows[counter].Cells["MarkPrice"].Value))*Convert.ToDouble(dataGridView2.Rows[counter].Cells["Quantity"].Value));
                }


            }
        }

               

        private void priceBookUsingSimulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Program.Vuniversal = Convert.ToDouble(textBox1.Text);//place holder
                UpdateBalance();
                dataGridView1.Update();
                dataGridView2.Update();
                dataGridView1.Refresh();
                dataGridView2.Refresh();
                string x, y;
                Model1Container db = new Model1Container();
                foreach (DataGridViewRow r in dataGridView2.Rows )
                {
                    int InstID = Convert.ToInt32(r.Cells["InstrumentsID"].Value);
                    //find out instrument type
                    y = Convert.ToString(db.Instruments1.Find(InstID).GetType());
                    x = Convert.ToString(y[34]);
                    Console.WriteLine("x:{0}", x);
                    if (x == "S")
                    {
                        r.Cells["InstrumentType"].Value = "Stock";
                    }
                    if (x == "E")
                    {
                        r.Cells["InstrumentType"].Value = "European Option";
                    }
                    if (x == "A")
                    {
                        r.Cells["InstrumentType"].Value = "Asian Option";
                    }
                    if (x == "B")
                    {
                        r.Cells["InstrumentType"].Value = "Barrier Option";
                    }
                    if (x == "D")
                    {
                        r.Cells["InstrumentType"].Value = "Digital Option";
                    }
                    if (x == "R")
                    {
                        r.Cells["InstrumentType"].Value = "Range Option";
                    }
                    if (x == "L")
                    {
                        r.Cells["InstrumentType"].Value = "LookBack Option";
                    }
                }

            }
            catch
            {
                MessageBox.Show("Please check the input for Volatility before proceeding", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }

        private void tradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 whatever = new Form4();
            whatever.ShowDialog();
            dataGridView1.Update();
            dataGridView2.Update();
            dataGridView1.Refresh();
            dataGridView2.Refresh();
        }

        private void refreshTradesFromDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = null;
            Model1Container db = new Model1Container();
            dataGridView2.DataSource = db.Trade1.ToList();





        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void instrumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 z = new Form3();
            z.ShowDialog();
        }

        private void historicalPriceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form6 z = new Form6();
            z.ShowDialog();
        }

        private void analysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form7 h = new Form7();
            h.ShowDialog();
        }

        private void simulateOptionPriceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 n = new Form1();
            n.ShowDialog();
             
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Model1Container db = new Model1Container();
            foreach (DataGridViewRow r in dataGridView2.SelectedRows)
            {

                int id = Convert.ToInt32(r.Cells["Id"].Value);
                Console.WriteLine("r:{0}, id:{1}", r, id);
                var g = db.Trade1.SingleOrDefault(h => h.Id == id);
                db.Entry(g).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();


            }
            dataGridView2.DataSource = null;
            dataGridView2.DataSource = db.Trade1.ToList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double totalPL = 0;
            double tdelta=0, tgamma=0, ttheta=0, tvega=0, trho = 0;
            Model1Container db = new Model1Container();
            string y,x;
            foreach( DataGridViewRow r in dataGridView2.SelectedRows)
            {
                
                totalPL = totalPL + Convert.ToDouble(r.Cells["P&L"].Value);
                tdelta = tdelta + (Convert.ToDouble(r.Cells["Delta"].Value) * Convert.ToDouble(r.Cells["Quantity"].Value));
                tgamma = tgamma + (Convert.ToDouble(r.Cells["Gamma"].Value) * Convert.ToDouble(r.Cells["Quantity"].Value));
                ttheta = ttheta + (Convert.ToDouble(r.Cells["Theta"].Value) * Convert.ToDouble(r.Cells["Quantity"].Value));
                tvega = tvega + (Convert.ToDouble(r.Cells["Vega"].Value) * Convert.ToDouble(r.Cells["Quantity"].Value));
                trho = trho + (Convert.ToDouble(r.Cells["Rho"].Value) * Convert.ToDouble(r.Cells["Quantity"].Value));
            }
            dataGridView1.Rows[0].Cells["P&L"].Value = totalPL;
            dataGridView1.Rows[0].Cells["Delta"].Value = tdelta;
            dataGridView1.Rows[0].Cells["Gamma"].Value = tgamma;
            dataGridView1.Rows[0].Cells["Vega"].Value = tvega;
            dataGridView1.Rows[0].Cells["Theta"].Value = ttheta;
            dataGridView1.Rows[0].Cells["Rho"].Value = trho;


        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void changeSimulationSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form8 eight = new Form8();
            eight.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Stock 1 Microsoft
            try
            {
                DateTime y = DateTime.Today;
                Stock1 x = new Stock1
                {
                    Underlying = 50,
                    //Strike = 90,
                    //IsCall = "call",
                    Ticker = "MSFT_Stock",
                    Exchange = "NASDAQ",
                    CompanyName = "Microsoft"
                    //ExpirationDate = y
                };
                using (Model1Container db1 = new Model1Container())
                {
                    //Console.WriteLine("yes1");
                    //db.SaveChanges();
                    db1.Instruments1.Add(x);
                    db1.SaveChanges();
                }
                //Stock 2 Amazon
                Stock1 x1 = new Stock1
                {
                    Underlying = 60,
                    //Strike = 90,
                    //IsCall = "call",
                    Ticker = "AMZN_Stock",
                    Exchange = "NASDAQ",
                    CompanyName = "AMAZON"
                    //ExpirationDate = y
                };
                using (Model1Container db1 = new Model1Container())
                {
                    //Console.WriteLine("yes1");
                    //db.SaveChanges();
                    db1.Instruments1.Add(x1);
                    db1.SaveChanges();
                }
                //Microsoft historical Price
                Model1Container dbx = new Model1Container();
                Price1 price1 = new Price1
                {
                    InstrumentsId = dbx.Instruments1.OfType<Stock1>().First().Id,
                    Date = DateTime.Today,
                    ClosingPrice = 50,
                };
                using (Model1Container db1 = new Model1Container())
                {
                    db1.Price1.Add(price1);
                    db1.SaveChanges();

                }
                Price1 price2 = new Price1
                {
                    InstrumentsId = dbx.Instruments1.OfType<Stock1>().First().Id,
                    Date = DateTime.Today,
                    ClosingPrice = 55,
                };
                using (Model1Container db1 = new Model1Container())
                {
                    db1.Price1.Add(price2);
                    db1.SaveChanges();

                }
                Price1 price3 = new Price1
                {
                    InstrumentsId = dbx.Instruments1.OfType<Stock1>().First().Id + 1,
                    Date = DateTime.Today,
                    ClosingPrice = 60,
                };
                Price1 price4 = new Price1
                {
                    InstrumentsId = dbx.Instruments1.OfType<Stock1>().First().Id + 1,
                    Date = DateTime.Today,
                    ClosingPrice = 61,
                };
                using (Model1Container db1 = new Model1Container())
                {
                    db1.Price1.Add(price3);
                    db1.Price1.Add(price4);
                    db1.SaveChanges();

                }
                //European Microsoft Option
                Model1Container db = new Model1Container();
                European1 y2 = new European1
                {
                    Underlying = db.Instruments1.OfType<Stock1>().First().Id,
                    Strike = 50,
                    IsCall = "C",
                    Ticker = "MSFT_EURP",
                    Exchange = "NASDAQ",
                    //CompanyName = co
                    ExpirationDate = 1
                };
                using (Model1Container db1 = new Model1Container())
                {

                    db1.Instruments1.Add(y2);
                    db1.SaveChanges();
                }
                //Range Option Amazon
                Range1 x2 = new Range1
                {
                    Underlying = db.Instruments1.OfType<Stock1>().First().Id + 1,
                    Strike = 60,
                    //IsCall = C,
                    Ticker = "AMZN_ASIA",
                    Exchange = "NYFTY",
                    //CompanyName = com
                    ExpirationDate = 2
                };
                using (Model1Container db1 = new Model1Container())
                {

                    db1.Instruments1.Add(x2);
                    db1.SaveChanges();
                    //Console.ReadLine();

                }
                //Barrier  Up and I Microsoft
                Barrier1 x3 = new Barrier1
                {
                    Underlying =db.Instruments1.OfType<Stock1>().First().Id,
                    Strike = 60,
                    IsCall = "C",
                    Ticker = "MSFT_BARR",
                    Exchange = "NASDAQ",
                    BarrierPrice = 70,
                    BarrierType = "UI",
                    //CompanyName = com
                    ExpirationDate = 1.5
                };
                using (Model1Container db2 = new Model1Container())
                {
                    //Console.WriteLine("yes1");
                    //db.SaveChanges();
                    db2.Instruments1.Add(x3);
                    db2.SaveChanges();
                    //Console.ReadLine();

                }
                //Add historical Prices


                //Add interest Rate
                InterestRate1 I1 = new InterestRate1
                {
                    Rate = 0.75,
                    Tenor = 0.5
                };
                InterestRate1 I2 = new InterestRate1
                {
                    Rate = 1.25,
                    Tenor = 1
                };
                InterestRate1 I3 = new InterestRate1
                {
                    Rate = 1.75,
                    Tenor = 1.5
                };
                InterestRate1 I4 = new InterestRate1
                {
                    Rate = 2.25,
                    Tenor = 2
                };
                using (Model1Container db4 = new Model1Container())
                {
                    db4.InterestRate1.Add(I1);
                    db4.InterestRate1.Add(I2);
                    db4.InterestRate1.Add(I3);
                    db4.InterestRate1.Add(I4);
                    db4.SaveChanges();
                }
                MessageBox.Show("Test Data Added Successfully", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Something Went wrong while adding the data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
