using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using ConsoleApp21;

namespace WindowsFormsApp3
{
    
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        public static void Start()
        {

            Form2 f = new Form2();
            Application.Run(f);
        }
        public static double Vuniversal;
        public static string M_uni="Not M", CV_uni="Not CV", A_uni="Not A";
        public static int Trial_uni = 1000, Steps_uni = 300;

        static void Main()
        {

            //Thread a1 = new Thread(new ThreadStart(RunGUI));
            //Form form1 = new Form1();
            //form1.Show();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Start();

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
            Thread t = new Thread(new ThreadStart(() => fillarray(randomnumbersmatrix, starindex, stopindex, seed)));
            return t;
        }
        public delegate void FillSimulateMatrix(double[][,] simulatedmatrix, string A, string CV, string C, double V, double D, double T, int Trials, int Steps, double U, double K, double[,] matrix1, int starindex, int stopindex);
        public static FillSimulateMatrix fillsim = delegate (double[][,] simulatedmatrix, string A, string CV, string C, double V, double D, double T, int Trials, int Steps, double U, double K, double[,] matrix1, int starindex, int stopindex)
        {
            if (A == "A" && CV == "CV")
            {

                double dt = T / Convert.ToDouble(Steps);
                double nudt = (D - ((V * V) / 2)) * dt;
                double sigsdt = V * Math.Sqrt(dt);
                double erddt = Math.Exp(D * dt);
                double t, delta, eps, Stn, delta1, Stn1;
                for (int i = starindex; i < stopindex; i++)
                {
                    simulatedmatrix[0][i, 0] = U;
                    simulatedmatrix[1][i, 0] = U;
                    //Console.WriteLine("{0},{1}",i,matrix2[i, 0]);

                }
                double deltaT = (T / Convert.ToDouble(Steps));
                //Console.WriteLine("DeltaT {0}", deltaT);
                for (int i = starindex; i < stopindex; i++)
                {
                    double St = U;
                    double cv = 0;
                    double St1 = U;
                    double cv1 = 0;
                    for (int j = 0; j < Steps - 1; j++)
                    {
                        //Console.WriteLine("{0},{1}:{2},RandomNumber:{3} ", i, j, matrix2[i, j + 1], matrix1[i, j + 1]);
                        t = (j + 1) * dt;
                        eps = matrix1[i, j + 1];
                        delta = Simulations.BSDelta(C, St, K, D, V, T - t);
                        Stn = St * Math.Exp(nudt + sigsdt * eps);
                        simulatedmatrix[0][i, j + 1] = Stn;
                        cv = cv + delta * (Stn - St * erddt);
                        St = Stn;
                        //Antithetic part and second delta and covariate
                        delta1 = Simulations.BSDelta(C, St1, K, D, V, T - t);
                        Stn1 = St1 * Math.Exp(nudt + sigsdt * (-1 * eps));
                        simulatedmatrix[1][i, j + 1] = Stn1;
                        cv1 = cv1 + delta1 * (Stn1 - St1 * erddt);
                        St1 = Stn1;
                    }
                    simulatedmatrix[2][i, 0] = cv;// control varitate 1
                    simulatedmatrix[2][i, 1] = cv1; //control variate 2

                }
                //Console.ReadLine();
                //simulatedmatrix[0] = matrix; code this is main
                //simulatedmatrix[1] = matrix2;
                //simulatedmatrix[2] = matrix3;
                //matrix3 = matrix2;


            }
            if (CV == "CV")
            {
                //double[,] matrix = new double[Trials, Steps];
                //double[,] matrix3 = new double[Trials, 2];
                //double[][,] boss = new double[2][,];

                double dt = T / Convert.ToDouble(Steps);
                double nudt = (D - ((V * V) / 2)) * dt;
                double sigsdt = V * Math.Sqrt(dt);
                double erddt = Math.Exp(D * dt);
                double t, delta, eps, Stn;

                for (int j = starindex; j < stopindex; j++)
                {
                    double St = U;
                    double cv = 0;
                    for (int i = 0; i < Steps - 1; i++)
                    {
                        t = (i + 1) * dt;
                        delta = Simulations.BSDelta(C, St, K, D, V, T - t);
                        eps = matrix1[j, i + 1];
                        Stn = St * Math.Exp(nudt + sigsdt * eps);
                        simulatedmatrix[0][j, i + 1] = Stn;
                        cv = cv + delta * (Stn - St * erddt);
                        St = Stn;
                    }
                    simulatedmatrix[1][j, 0] = cv;
                    simulatedmatrix[1][j, 1] = St;
                    //Console.WriteLine("Control Variate..Payoff::, {0}, payoff 2:, {1}, cv:, {2}", St, matrix[j, Steps-1], cv);

                }
                //boss[0] = matrix;
                //boss[1] = matrix3;
            }
            if (A == "A")
            {
                //double[,] matrix = new double[Trials, Steps];
                //double[,] matrix2 = new double[Trials, Steps];
                //double[][,] boss = new double[2][,];
                for (int i = starindex; i < stopindex; i++)
                {
                    simulatedmatrix[0][i, 0] = U;
                    simulatedmatrix[1][i, 0] = U;
                    //Console.WriteLine("{0},{1}",i,matrix2[i, 0]);

                }
                double deltaT = (T / Convert.ToDouble(Steps));
                //Console.WriteLine("DeltaT {0}", deltaT);
                for (int i = starindex; i < stopindex; i++)
                    for (int j = 0; j < Steps - 1; j++)
                    {
                        simulatedmatrix[0][i, j + 1] = simulatedmatrix[0][i, j] * Math.Exp(((D - ((V * V) / 2)) * deltaT) + (V * Math.Sqrt(deltaT) * matrix1[i, j + 1]));
                        simulatedmatrix[1][i, j + 1] = simulatedmatrix[1][i, j] * Math.Exp(((D - ((V * V) / 2)) * deltaT) + (V * Math.Sqrt(deltaT) * (-1 * matrix1[i, j + 1])));//antitheitc values on the epsilon is negative here
                                                                                                                                                                                //Console.WriteLine("{0},{1}:{2},RandomNumber:{3} ", i, j, matrix2[i, j + 1], matrix1[i, j + 1]);
                    }
                //Console.ReadLine();
                //boss[0] = matrix;
                //boss[1] = matrix2;
                //matrix3 = matrix2;
                //return boss;

            }
            else
            {
                //double[,] matrix5 = new double[0, 0];
                //double[,] matrix = new double[Trials, Steps];
                for (int i = 0; i < Trials; i++)
                {
                    simulatedmatrix[0][i, 0] = U;
                    //Console.WriteLine("{0},{1}",i,matrix2[i, 0]);

                }
                double deltaT = (T / Convert.ToDouble(Steps));
                //Console.WriteLine("DeltaT {0}", deltaT);
                for (int i = starindex; i < stopindex; i++)
                    for (int j = 0; j < Steps - 1; j++)
                    {
                        simulatedmatrix[0][i, j + 1] = simulatedmatrix[0][i, j] * Math.Exp(((D - ((V * V) / 2)) * deltaT) + (V * Math.Sqrt(deltaT) * matrix1[i, j + 1]));
                        //Console.WriteLine("{0},{1}:{2},RandomNumber:{3} ", i, j, matrix2[i, j + 1], matrix1[i, j + 1]);
                    }

            }


        };
        //thread creation for simulated matrix
        public static Thread Createthread1(double[][,] simulatedmatrix, string A, string CV, string C, double V, double D, double T, int Trials, int Steps, double U, double K, double[,] matrix1, int starindex, int stopindex)
        {
            Thread t = new Thread(new ThreadStart(() => fillsim(simulatedmatrix, A, CV, C, V, D, T, Trials, Steps, U, K, matrix1, starindex, stopindex)));
            return t;
        }
        public abstract class Option
        {
            public double U { get; set; }
            public double T { get; set; }
            public double V { get; set; }
            public double D { get; set; }
            public double R { get; set; }
            public double S { get; set; }
            public int Trials { get; set; }
            public int Steps { get; set; }
            public String CorP { get; set; }
            public double[,] matrix1 { get; set; }//random numbers matrix
        }
        public class European_Option : Option
        {
            public double Option_Price { get => GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Option_Price = value; }
            public double Delta { get => GetDelta(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Delta = value; }
            public double Gamma { get => GetGamma(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Gamma = value; }
            public double Vega { get => GetVega(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Vega = value; }
            public double Theta { get => GetTheta(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Theta = value; }
            public double Rho { get => GetRho(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Rho = value; }
            public double Standard_Deviation { get => GetStandard_deviation(CorP, V, D, T, Steps, U, Option_Price, matrix1, Trials); set => Standard_Deviation = value; }

            public int z = System.Environment.ProcessorCount;//number of processors
            public string A { get; set; }
            public string CV { get; set; }
            public string M { get; set; }
            public double[][,] matrix3 { get; set; }//simulation matrix
            public int[] incrementmatrix { get => createincrementmatrix(); set => incrementmatrix = value; }//matrix required for multithreading
            private int[] createincrementmatrix()
            {
                int increment = Trials / z;
                int[] incrementmatrix1 = new int[z + 1];
                incrementmatrix1[0] = 0;
                int temp = 0;
                for (int x = 1; x < z + 1; x++)
                {
                    temp = increment + temp;
                    incrementmatrix1[x] = temp;
                    Console.WriteLine("i:{0}, value:{1}", x, incrementmatrix1[x]);
                }
                return incrementmatrix1;
            }


            private double GetOptionPrice(String C, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {

                matrix3 = Simulations.CreateSimulatedMatrix(CV, A, Trials, Steps);

                List<Thread> threadlist2 = new List<Thread>(z);
                if (CV == "CV" && A == "A")
                {
                    if (M == "M")
                    {

                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticDeltaVariate(C, S, V, D, T, Trials, Steps, U, matrix1);
                    }
                    double sum_CT = 0;
                    double sum_CT2 = 0;
                    double beta1 = -1;
                    double CT;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {

                            CT = (Math.Max(matrix3[0][z, Steps - 1] - S, 0) + Math.Max(matrix3[1][z, Steps - 1] - S, 0) + (beta1 * matrix3[2][z, 0]) + (beta1 * matrix3[2][z, 1])) * 0.5;
                            sum_CT = sum_CT + CT;
                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        //Console.ReadLine();
                    }

                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            CT = (Math.Max(S - matrix3[0][z, Steps - 1], 0) + Math.Max(S - matrix3[1][z, Steps - 1], 0) + (beta1 * matrix3[2][z, 0]) + (beta1 * matrix3[2][z, 1])) * 0.5;
                            sum_CT = sum_CT + CT;
                            //m_CT2 = sum_CT2 + (CT * CT);

                        }

                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return (sum_CT / Convert.ToDouble(Trials)) * Math.Exp(-D * T);
                }
                if (CV == "CV")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.deltacontrolvariate(C, V, D, R, Trials, Steps, U, S, matrix1);
                    }
                    double[][,] matrix2 = matrix3;
                    double sum_CT = 0;
                    double sum_CT2 = 0;
                    double beta1 = -1;
                    double CT;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            CT = Math.Max(matrix2[0][z, Steps - 1] - S, 0) + (beta1 * matrix2[1][z, 0]);
                            sum_CT = sum_CT + CT;
                            sum_CT2 = sum_CT2 + (CT * CT);

                            //Console.WriteLine("payoff:,{0}, average:, {1},  CV:, {2}", Math.Max(matrix2[0][z, Steps - 1] - S, 0), CT, matrix2[1][z, 0]);
                            //Console.WriteLine(temp);

                        }
                        Console.ReadLine();
                    }

                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            CT = Math.Max(S - matrix2[0][z, Steps - 1], 0) + (beta1 * matrix2[1][z, 0]);
                            sum_CT = sum_CT + CT;
                            sum_CT2 = sum_CT2 + (CT * CT);

                        }

                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return (sum_CT / Convert.ToDouble(Trials)) * Math.Exp(-D * T);
                }

                if (A == "A")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double temp = 0;
                    double temp1 = 0;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            matrix3[0][z, Steps - 1] = Math.Max(matrix3[0][z, Steps - 1] - S, 0);
                            matrix3[1][z, Steps - 1] = Math.Max(matrix3[1][z, Steps - 1] - S, 0);
                            temp = matrix3[0][z, Steps - 1] + temp;
                            temp1 = matrix3[1][z, Steps - 1] + temp1;
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        Console.ReadLine();
                    }
                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            matrix3[0][z, Steps - 1] = Math.Max(S - matrix3[0][z, Steps - 1], 0);
                            matrix3[1][z, Steps - 1] = Math.Max(matrix3[1][z, Steps - 1] - S, 0);
                            temp = matrix3[0][z, Steps - 1] + temp;
                            temp1 = matrix3[1][z, Steps - 1] + temp1;
                        }
                    }
                    return 0.5 * ((temp / Trials) + (temp1 / Trials)) * Math.Exp(-R * T);
                }
                else
                {
                    if (M == "M")
                    {
                        //matrix3 = Simulations.GenerateSimulations2(V, D, T, Trials, Steps, U, matrix1);
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.GenerateSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[,] matrix2 = matrix3[0];
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            matrix2[z, Steps - 1] = Math.Max(matrix2[z, Steps - 1] - S, 0);
                            temp = matrix2[z, Steps - 1] + temp;
                            //Console.WriteLine(temp);
                        }
                    }
                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            matrix2[z, Steps - 1] = Math.Max(S - matrix2[z, Steps - 1], 0);
                            temp = matrix2[z, Steps - 1] + temp;
                        }
                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return temp / Trials * Math.Exp(-R * T);
                }
            }
            private double GetDelta(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaS = U * 0.001;

                return (GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U + deltaS, R, matrix1) - GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U - deltaS, R, matrix1)) / (2 * deltaS);
            }
            private double GetGamma(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaS = U * 0.001;
                return (GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U + deltaS, R, matrix1) - 2 * GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1) + GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U - deltaS, R, matrix1)) / (deltaS * deltaS);
            }
            private double GetVega(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaV = V * 0.001;
                return (GetOptionPrice(CorP, S, V + deltaV, D, T, Trials, Steps, U, R, matrix1) - GetOptionPrice(CorP, S, V - deltaV, D, T, Trials, Steps, U, R, matrix1)) / (2 * deltaV);
            }
            private double GetTheta(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaT = T * 0.001;
                return -(GetOptionPrice(CorP, S, V, D, T + deltaT, Trials, Steps, U, R, matrix1) - GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1)) / (deltaT);
            }
            private double GetRho(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaR = R * 0.001;
                return (GetOptionPrice(CorP, S, V, D + deltaR, T, Trials, Steps, U, R + deltaR, matrix1) - GetOptionPrice(CorP, S, V, D - deltaR, T, Trials, Steps, U, R - deltaR, matrix1)) / (deltaR * 2);
            }

            private double GetStandard_deviation(String CorP, double V, double D, double T, int Steps, double U, double option, double[,] matrix2, int trials)
            {
                List<Thread> threadlist2 = new List<Thread>(z);
                //Standard deviation for anithetic + delta variate
                if (CV == "CV" && A == "A")
                {
                    if (M == "M")
                    {

                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticDeltaVariate(CorP, S, V, D, T, Trials, Steps, U, matrix1);
                    }

                    double[] intrinsic = new double[trials];
                    double[] intrinsic1 = new double[trials];
                    double temp = 0;
                    double beta1 = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = (Math.Max(matrix3[0][i, Steps - 1] - S, 0) + Math.Max(matrix3[0][i, Steps - 1] - S, 0) + (beta1 * matrix3[2][i, 0]) + (beta1 * matrix3[2][i, 1])) * 0.5;
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {

                            intrinsic[i] = (Math.Max(S - matrix3[0][i, Steps - 1], 0) + Math.Max(S - matrix3[0][i, Steps - 1], 0) + (beta1 * matrix3[2][i, 0]) + (beta1 * matrix3[2][i, 1])) * 0.5;
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));
                    //Console.ReadLine();
                    return x;
                }
                if (CV == "CV")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.deltacontrolvariate(CorP, V, D, R, Trials, Steps, U, S, matrix1);
                    }
                    double[] intrinsic = new double[trials];
                    double temp = 0;
                    double beta1 = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = Math.Max(matrix3[0][i, Steps - 1] - S, 0) + (beta1 * matrix3[1][i, 0]);
                            temp += Math.Pow((intrinsic[i] - option), 2);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {

                            intrinsic[i] = Math.Max(matrix3[0][i, Steps - 1] - S, 0) + (beta1 * matrix3[1][i, 0]);
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));

                    return x;
                }
                if (A == "A")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[] intrinsic = new double[trials];
                    double[] intrinsic1 = new double[trials];
                    double beta1 = -1;
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = Math.Max(matrix3[0][i, Steps - 1] - S, 0);
                            intrinsic1[i] = Math.Max(matrix3[1][i, Steps - 1] - S, 0);
                            temp += Math.Pow((intrinsic[i] - option), 2) + Math.Pow((intrinsic1[i] - option), 2);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {

                            intrinsic[i] = Math.Max(S - matrix3[0][i, Steps - 1], 0);
                            intrinsic1[i] = Math.Max(S - matrix3[1][i, Steps - 1], 0);
                            temp += Math.Pow((intrinsic[i] - option), 2) + Math.Pow((intrinsic1[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));

                    return x;
                }
                else
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.GenerateSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[,] matrix = matrix3[0];
                    double[] intrinsic = new double[trials];
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = Math.Max(matrix[i, Steps - 1] - S, 0);
                            temp += Math.Pow((intrinsic[i] - option), 2);
                            //Console.WriteLine("Yes");


                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = Math.Max(S - matrix[i, Steps - 1], 0);
                            temp += Math.Pow((intrinsic[i] - option), 2);


                        }
                    }

                    double x = ((Math.Sqrt(temp / (trials - 1))) / Math.Sqrt(trials));

                    return x;
                }
            }
        }
        public static class Statistics
        {
            public static double Phi(double x)
            {
                // constants
                double a1 = 0.254829592;
                double a2 = -0.284496736;
                double a3 = 1.421413741;
                double a4 = -1.453152027;
                double a5 = 1.061405429;
                double p = 0.3275911;
                double x1 = x;
                // Save the sign of x
                int sign = 1;
                if (x < 0)
                    sign = -1;
                x = Math.Abs(x) / Math.Sqrt(2.0);

                // A&S formula 7.1.26
                double t = 1.0 / (1.0 + p * x);
                double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

                double g = 0.5 * (1.0 + sign * y);
                //Console.WriteLine("d1:{0},cdf:{1}", x1, g);
                //Console.ReadLine();
                return g;

            }

        }
        public static class RandomNumberGenerator
        {

            //Polar rejection formula
            public static double[,] NormalRandomNumbers(int Trials, int Steps)
            {
                double[,] matrix = new double[Trials, Steps];
                Random random1 = new Random();
                for (int i = 0; i < Trials; i++)
                    for (int j = 0; j < Steps - 1; j += 2)
                    {
                        double x1, x2;
                        x1 = random1.NextDouble();
                        x2 = random1.NextDouble();
                        //Console.WriteLine("Uniform Random Number {0} |{1}   ", x1, x2);
                        matrix[i, j] = (Math.Sqrt(-2 * Math.Log(x1))) * Math.Cos(2 * Math.PI * x2);
                        matrix[i, j + 1] = (Math.Sqrt(-2 * Math.Log(x1))) * Math.Sin(2 * Math.PI * x2);

                    }

                return matrix;
            }
        }
        public static class Simulations
        {
            public static double[][,] CreateSimulatedMatrix(string CV, string A, int Trials, int Steps)
            {
                if (CV == "CV" && A == "A")
                {
                    double[,] matrix = new double[Trials, Steps];
                    double[,] matrix2 = new double[Trials, Steps];
                    double[,] matrix3 = new double[Trials, 2];
                    double[][,] boss = new double[3][,];
                    boss[0] = matrix;
                    boss[1] = matrix2;
                    boss[2] = matrix3;
                    return boss;
                }
                if (CV == "CV")
                {
                    double[,] matrix = new double[Trials, Steps];
                    double[,] matrix2 = new double[Trials, Steps];
                    double[,] matrix3 = new double[Trials, 2];
                    double[][,] boss = new double[3][,];
                    boss[0] = matrix;
                    boss[1] = matrix2;
                    boss[2] = matrix3;
                    return boss;
                }
                if (A == "A")
                {
                    double[,] matrix = new double[Trials, Steps];
                    double[,] matrix2 = new double[Trials, Steps];
                    double[][,] boss = new double[2][,];
                    boss[0] = matrix;
                    boss[1] = matrix2;
                    return boss;
                }
                else
                {
                    double[,] matrix5 = new double[0, 0];
                    double[,] matrix = new double[Trials, Steps];
                    double[][,] boss = new double[2][,];
                    boss[0] = matrix;
                    boss[1] = matrix5;
                    return boss;

                }
            }

            public static double[][,] GenerateSimulations(double V, double D, double T, int Trials, int Steps, double U, double[,] matrix1)
            {
                //Console.WriteLine("V{0}, D{1}, T{2}, Trials{3}, Steps{4}, U{5}", V, D, T, Trials, Steps, U);
                //Console.ReadLine();

                double[,] matrix5 = new double[0, 0];
                double[,] matrix = new double[Trials, Steps];
                for (int i = 0; i < Trials; i++)
                {
                    matrix[i, 0] = U;
                    //Console.WriteLine("{0},{1}",i,matrix2[i, 0]);

                }
                double deltaT = (T / Convert.ToDouble(Steps));
                //Console.WriteLine("DeltaT {0}", deltaT);
                for (int i = 0; i < Trials; i++)
                    for (int j = 0; j < Steps - 1; j++)
                    {
                        matrix[i, j + 1] = matrix[i, j] * Math.Exp(((D - ((V * V) / 2)) * deltaT) + (V * Math.Sqrt(deltaT) * matrix1[i, j + 1]));
                        //Console.WriteLine("{0},{1}:{2},RandomNumber:{3} ", i, j, matrix2[i, j + 1], matrix1[i, j + 1]);
                    }
                //Console.ReadLine();
                //matrix3 = matrix2;
                double[][,] boss = new double[2][,];
                boss[0] = matrix;
                boss[1] = matrix5;
                return boss;

            }


            public static double[][,] AntitheticSimulations(double V, double D, double T, int Trials, int Steps, double U, double[,] matrix1)
            {
                double[,] matrix = new double[Trials, Steps];
                double[,] matrix2 = new double[Trials, Steps];
                double[][,] boss = new double[2][,];
                for (int i = 0; i < Trials; i++)
                {
                    matrix[i, 0] = U;
                    matrix2[i, 0] = U;
                    //Console.WriteLine("{0},{1}",i,matrix2[i, 0]);

                }
                double deltaT = (T / Convert.ToDouble(Steps));
                //Console.WriteLine("DeltaT {0}", deltaT);
                for (int i = 0; i < Trials; i++)
                    for (int j = 0; j < Steps - 1; j++)
                    {
                        matrix[i, j + 1] = matrix[i, j] * Math.Exp(((D - ((V * V) / 2)) * deltaT) + (V * Math.Sqrt(deltaT) * matrix1[i, j + 1]));
                        matrix2[i, j + 1] = matrix2[i, j] * Math.Exp(((D - ((V * V) / 2)) * deltaT) + (V * Math.Sqrt(deltaT) * (-1 * matrix1[i, j + 1])));//antitheitc values on the epsilon is negative here
                                                                                                                                                          //Console.WriteLine("{0},{1}:{2},RandomNumber:{3} ", i, j, matrix2[i, j + 1], matrix1[i, j + 1]);
                    }
                //Console.ReadLine();
                boss[0] = matrix;
                boss[1] = matrix2;
                //matrix3 = matrix2;
                return boss;
            }
            public static double[][,] AntitheticDeltaVariate(string C, double K, double V, double D, double T, int Trials, int Steps, double U, double[,] matrix1)
            {
                double[,] matrix = new double[Trials, Steps];
                double[,] matrix2 = new double[Trials, Steps];
                double[,] matrix3 = new double[Trials, 2];
                double[][,] boss = new double[3][,];
                double dt = T / Convert.ToDouble(Steps);
                double nudt = (D - ((V * V) / 2)) * dt;
                double sigsdt = V * Math.Sqrt(dt);
                double erddt = Math.Exp(D * dt);
                double t, delta, eps, Stn, delta1, Stn1;
                for (int i = 0; i < Trials; i++)
                {
                    matrix[i, 0] = U;
                    matrix2[i, 0] = U;
                    //Console.WriteLine("{0},{1}",i,matrix2[i, 0]);

                }
                double deltaT = (T / Convert.ToDouble(Steps));
                //Console.WriteLine("DeltaT {0}", deltaT);
                for (int i = 0; i < Trials; i++)
                {
                    double St = U;
                    double cv = 0;
                    double St1 = U;
                    double cv1 = 0;
                    for (int j = 0; j < Steps - 1; j++)
                    {
                        //Console.WriteLine("{0},{1}:{2},RandomNumber:{3} ", i, j, matrix2[i, j + 1], matrix1[i, j + 1]);
                        t = (j + 1) * dt;
                        eps = matrix1[i, j + 1];
                        delta = BSDelta(C, St, K, D, V, T - t);
                        Stn = St * Math.Exp(nudt + sigsdt * eps);
                        matrix[i, j + 1] = Stn;
                        cv = cv + delta * (Stn - St * erddt);
                        St = Stn;
                        //Antithetic part and second delta and covariate
                        delta1 = BSDelta(C, St1, K, D, V, T - t);
                        Stn1 = St1 * Math.Exp(nudt + sigsdt * (-1 * eps));
                        matrix2[i, j + 1] = Stn1;
                        cv1 = cv1 + delta1 * (Stn1 - St1 * erddt);
                        St1 = Stn1;
                    }
                    matrix3[i, 0] = cv;// control varitate 1
                    matrix3[i, 1] = cv1; //control variate 2

                }
                //Console.ReadLine();
                boss[0] = matrix;
                boss[1] = matrix2;
                boss[2] = matrix3;
                //matrix3 = matrix2;
                return boss;
            }
            public static double BSDelta(String c, double S, double k, double r, double sigma, double T)
            {
                double d1;
                d1 = (Math.Log(S / k) + ((r + ((sigma * sigma) / 2.0)) * T)) / (sigma * Math.Sqrt(T));
                if (c == "C")
                {

                    return Statistics.Phi(d1);
                }
                else
                {
                    return Statistics.Phi(d1) - 1;
                }
            }
            public static double[][,] deltacontrolvariate(string C, double V, double D, double T, int Trials, int Steps, double U, double K, double[,] matrix1)
            {
                double[,] matrix = new double[Trials, Steps];
                double[,] matrix3 = new double[Trials, 2];
                double[][,] boss = new double[2][,];

                double dt = T / Convert.ToDouble(Steps);
                double nudt = (D - ((V * V) / 2)) * dt;
                double sigsdt = V * Math.Sqrt(dt);
                double erddt = Math.Exp(D * dt);
                double t, delta, eps, Stn;

                for (int j = 0; j < Trials; j++)
                {
                    double St = U;
                    double cv = 0;
                    for (int i = 0; i < Steps - 1; i++)
                    {
                        t = (i + 1) * dt;
                        delta = BSDelta(C, St, K, D, V, T - t);
                        eps = matrix1[j, i + 1];
                        Stn = St * Math.Exp(nudt + sigsdt * eps);
                        matrix[j, i + 1] = Stn;
                        cv = cv + delta * (Stn - St * erddt);
                        St = Stn;
                    }
                    matrix3[j, 0] = cv;
                    matrix3[j, 1] = St;
                    //Console.WriteLine("Control Variate..Payoff::, {0}, payoff 2:, {1}, cv:, {2}", St, matrix[j, Steps-1], cv);

                }
                boss[0] = matrix;
                boss[1] = matrix3;
                //matrix3 = matrix2;
                //Console.ReadLine();
                return boss;
            }
        }
        public class Asian_Option : Option
        {
            public double Option_Price { get => GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Option_Price = value; }
            public double Delta { get => GetDelta(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Delta = value; }
            public double Gamma { get => GetGamma(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Gamma = value; }
            public double Vega { get => GetVega(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Vega = value; }
            public double Theta { get => GetTheta(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Theta = value; }
            public double Rho { get => GetRho(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Rho = value; }
            public double Standard_Deviation { get => GetStandard_deviation(CorP, V, D, T, Steps, U, Option_Price, matrix1, Trials); set => Standard_Deviation = value; }

            public int z = System.Environment.ProcessorCount;//number of processors
            public string A { get; set; }
            public string CV { get; set; }
            public string M { get; set; }
            public double[][,] matrix3 { get; set; }//simulation matrix
            public int[] incrementmatrix { get => createincrementmatrix(); set => incrementmatrix = value; }//matrix required for multithreading
            private int[] createincrementmatrix()
            {
                int increment = Trials / z;
                int[] incrementmatrix1 = new int[z + 1];
                incrementmatrix1[0] = 0;
                int temp = 0;
                for (int x = 1; x < z + 1; x++)
                {
                    temp = increment + temp;
                    incrementmatrix1[x] = temp;
                    Console.WriteLine("i:{0}, value:{1}", x, incrementmatrix1[x]);
                }
                return incrementmatrix1;
            }


            private double GetOptionPrice(String C, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {

                matrix3 = Simulations.CreateSimulatedMatrix(CV, A, Trials, Steps);

                List<Thread> threadlist2 = new List<Thread>(z);
                if (CV == "CV" && A == "A")
                {
                    if (M == "M")
                    {

                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticDeltaVariate(C, S, V, D, T, Trials, Steps, U, matrix1);
                    }


                    double sum_CT = 0;
                    double sum_CT2 = 0;
                    double beta1 = -1;
                    double CT;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            //averaging the stockprices for path dependent Asian options
                            double Sumsp = 0;
                            double Sumsp1 = 0;
                            double averagesp = 0;
                            double averagesp1 = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][z, g];
                                Sumsp1 += matrix3[1][z, g];
                            }
                            averagesp = Sumsp / Steps;
                            averagesp1 = Sumsp1 / Steps;
                            CT = (Math.Max(averagesp - S, 0) + Math.Max(averagesp1 - S, 0) + (beta1 * matrix3[2][z, 0]) + (beta1 * matrix3[2][z, 1])) * 0.5;
                            sum_CT = sum_CT + CT;
                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        //Console.ReadLine();
                    }

                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            //averaging the stockprices for path dependent Asian options
                            double Sumsp = 0;
                            double Sumsp1 = 0;
                            double averagesp = 0;
                            double averagesp1 = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][z, g];
                                Sumsp1 += matrix3[1][z, g];
                            }
                            averagesp = Sumsp / Steps;
                            averagesp1 = Sumsp1 / Steps;
                            CT = (Math.Max(S - averagesp, 0) + Math.Max(S - averagesp1, 0) + (beta1 * matrix3[2][z, 0]) + (beta1 * matrix3[2][z, 1])) * 0.5;
                            sum_CT = sum_CT + CT;
                            //m_CT2 = sum_CT2 + (CT * CT);

                        }

                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return (sum_CT / Convert.ToDouble(Trials)) * Math.Exp(-D * T);
                }
                if (CV == "CV")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.deltacontrolvariate(C, V, D, R, Trials, Steps, U, S, matrix1);
                    }
                    double[][,] matrix2 = matrix3;
                    double sum_CT = 0;
                    double sum_CT2 = 0;
                    double beta1 = -1;
                    double CT;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double Sumsp = 0;
                            double averagesp = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix2[0][z, g];

                            }
                            averagesp = Sumsp / Steps;
                            CT = Math.Max(averagesp - S, 0) + (beta1 * matrix2[1][z, 0]);
                            sum_CT = sum_CT + CT;
                            sum_CT2 = sum_CT2 + (CT * CT);

                            //Console.WriteLine("payoff:,{0}, average:, {1},  CV:, {2}", Math.Max(matrix2[0][z, Steps - 1] - S, 0), CT, matrix2[1][z, 0]);
                            //Console.WriteLine(temp);

                        }
                        Console.ReadLine();
                    }

                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {

                            double Sumsp = 0;
                            double averagesp = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix2[0][z, g];

                            }
                            averagesp = Sumsp / Steps;
                            CT = Math.Max(S - averagesp, 0) + (beta1 * matrix2[1][z, 0]);
                            sum_CT = sum_CT + CT;
                            sum_CT2 = sum_CT2 + (CT * CT);

                        }

                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return (sum_CT / Convert.ToDouble(Trials)) * Math.Exp(-D * T);
                }

                if (A == "A")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double temp = 0;
                    double temp1 = 0;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            //averaging the stockprices for path dependent Asian options
                            double Sumsp = 0;
                            double Sumsp1 = 0;
                            double averagesp = 0;
                            double averagesp1 = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][z, g];
                                Sumsp1 += matrix3[1][z, g];
                            }
                            averagesp = Sumsp / Steps;
                            averagesp1 = Sumsp1 / Steps;
                            averagesp = Math.Max(averagesp - S, 0);
                            averagesp1 = Math.Max(averagesp1 - S, 0);
                            temp = averagesp + temp;
                            temp1 = averagesp1 + temp1;
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        Console.ReadLine();
                    }
                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double Sumsp = 0;
                            double Sumsp1 = 0;
                            double averagesp = 0;
                            double averagesp1 = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][z, g];
                                Sumsp1 += matrix3[1][z, g];
                            }
                            averagesp = Sumsp / Steps;
                            averagesp1 = Sumsp1 / Steps;
                            averagesp = Math.Max(S- averagesp , 0);
                            averagesp1 = Math.Max(S- averagesp1 , 0);
                            temp = averagesp + temp;
                            temp1 = averagesp1 + temp1;
                        }
                    }
                    return 0.5 * ((temp / Trials) + (temp1 / Trials)) * Math.Exp(-R * T);
                }
                else
                {
                    if (M == "M")
                    {
                        //matrix3 = Simulations.GenerateSimulations2(V, D, T, Trials, Steps, U, matrix1);
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.GenerateSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[,] matrix2 = matrix3[0];
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {

                            double Sumsp = 0;
                            double averagesp = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix2[z, g];

                            }
                            averagesp = Sumsp / Steps;
                            averagesp = Math.Max(averagesp - S, 0);
                            temp = averagesp + temp;
                            //Console.WriteLine(temp);
                        }
                    }
                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {

                            double Sumsp = 0;
                            double averagesp = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix2[z, g];

                            }
                            averagesp = Sumsp / Steps;
                            averagesp = Math.Max(S-averagesp , 0);
                            temp = averagesp + temp;
                        }
                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return temp / Trials * Math.Exp(-R * T);
                }
            }
            private double GetDelta(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaS = U * 0.001;

                return (GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U + deltaS, R, matrix1) - GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U - deltaS, R, matrix1)) / (2 * deltaS);
            }
            private double GetGamma(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaS = U * 0.001;
                return (GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U + deltaS, R, matrix1) - 2 * GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1) + GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U - deltaS, R, matrix1)) / (deltaS * deltaS);
            }
            private double GetVega(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaV = V * 0.001;
                return (GetOptionPrice(CorP, S, V + deltaV, D, T, Trials, Steps, U, R, matrix1) - GetOptionPrice(CorP, S, V - deltaV, D, T, Trials, Steps, U, R, matrix1)) / (2 * deltaV);
            }
            private double GetTheta(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaT = T * 0.001;
                return -(GetOptionPrice(CorP, S, V, D, T + deltaT, Trials, Steps, U, R, matrix1) - GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1)) / (deltaT);
            }
            private double GetRho(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaR = R * 0.001;
                return (GetOptionPrice(CorP, S, V, D + deltaR, T, Trials, Steps, U, R + deltaR, matrix1) - GetOptionPrice(CorP, S, V, D - deltaR, T, Trials, Steps, U, R - deltaR, matrix1)) / (deltaR * 2);
            }

            private double GetStandard_deviation(String CorP, double V, double D, double T, int Steps, double U, double option, double[,] matrix2, int trials)
            {
                List<Thread> threadlist2 = new List<Thread>(z);
                //Standard deviation for anithetic + delta variate
                if (CV == "CV" && A == "A")
                {
                    if (M == "M")
                    {

                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticDeltaVariate(CorP, S, V, D, T, Trials, Steps, U, matrix1);
                    }

                    double[] intrinsic = new double[trials];
                    double[] intrinsic1 = new double[trials];
                    double temp = 0;
                    double beta1 = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double Sumsp1 = 0;
                            double averagesp = 0;
                            double averagesp1 = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][i, g];
                                Sumsp1 += matrix3[1][i, g];
                            }
                            averagesp = Sumsp / Steps;
                            averagesp1 = Sumsp1 / Steps;
                            intrinsic[i] = (Math.Max(averagesp - S, 0) + Math.Max(averagesp1 - S, 0) + (beta1 * matrix3[2][i, 0]) + (beta1 * matrix3[2][i, 1])) * 0.5;
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double Sumsp1 = 0;
                            double averagesp = 0;
                            double averagesp1 = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][z, g];
                                Sumsp1 += matrix3[1][z, g];
                            }
                            averagesp = Sumsp / Steps;
                            averagesp1 = Sumsp1 / Steps;
                            intrinsic[i] = (Math.Max(S - averagesp, 0) + Math.Max(S - averagesp1, 0) + (beta1 * matrix3[2][i, 0]) + (beta1 * matrix3[2][i, 1])) * 0.5;
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));
                    //Console.ReadLine();
                    return x;
                }
                if (CV == "CV")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.deltacontrolvariate(CorP, V, D, R, Trials, Steps, U, S, matrix1);
                    }
                    double[] intrinsic = new double[trials];
                    double temp = 0;
                    double beta1 = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double averagesp = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][i, g];

                            }
                            averagesp = Sumsp / Steps;
                            intrinsic[i] = Math.Max(averagesp - S, 0) + (beta1 * matrix3[1][i, 0]);
                            temp += Math.Pow((intrinsic[i] - option), 2);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double averagesp = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][i, g];

                            }
                            averagesp = Sumsp / Steps;
                            intrinsic[i] = Math.Max(averagesp - S, 0) + (beta1 * matrix3[1][i, 0]);
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));

                    return x;
                }
                if (A == "A")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[] intrinsic = new double[trials];
                    double[] intrinsic1 = new double[trials];
                    double beta1 = -1;
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double Sumsp1 = 0;
                            double averagesp = 0;
                            double averagesp1 = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][z, g];
                                Sumsp1 += matrix3[1][z, g];
                            }
                            averagesp = Sumsp / Steps;
                            averagesp1 = Sumsp1 / Steps;
                            averagesp = Math.Max(averagesp - S, 0);
                            averagesp1 = Math.Max(averagesp1 - S, 0);
                            intrinsic[i] = averagesp;
                            intrinsic1[i] =averagesp1;
                            temp += Math.Pow((intrinsic[i] - option), 2) + Math.Pow((intrinsic1[i] - option), 2);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double Sumsp1 = 0;
                            double averagesp = 0;
                            double averagesp1 = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][z, g];
                                Sumsp1 += matrix3[1][z, g];
                            }
                            averagesp = Sumsp / Steps;
                            averagesp1 = Sumsp1 / Steps;
                            averagesp = Math.Max(S- averagesp , 0);
                            averagesp1 = Math.Max(S- averagesp1, 0);

                            intrinsic[i] = averagesp;
                            intrinsic1[i] = averagesp1;
                            temp += Math.Pow((intrinsic[i] - option), 2) + Math.Pow((intrinsic1[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));

                    return x;
                }
                else
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.GenerateSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[,] matrix = matrix3[0];
                    double[] intrinsic = new double[trials];
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double averagesp = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix2[z, g];

                            }
                            averagesp = Sumsp / Steps;
                            averagesp = Math.Max(averagesp - S, 0);
                            intrinsic[i] = averagesp;
                            temp += Math.Pow((intrinsic[i] - option), 2);
                            //Console.WriteLine("Yes");


                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double averagesp = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix2[z, g];

                            }
                            averagesp = Sumsp / Steps;
                            averagesp = Math.Max(S- averagesp, 0);
                            intrinsic[i] = averagesp;
                            temp += Math.Pow((intrinsic[i] - option), 2);


                        }
                    }

                    double x = ((Math.Sqrt(temp / (trials - 1))) / Math.Sqrt(trials));

                    return x;
                }
            }
        }
        public class Digital_Option : Option
        {
            public double Option_Price { get => GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Option_Price = value; }
            public double Delta { get => GetDelta(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Delta = value; }
            public double Gamma { get => GetGamma(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Gamma = value; }
            public double Vega { get => GetVega(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Vega = value; }
            public double Theta { get => GetTheta(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Theta = value; }
            public double Rho { get => GetRho(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Rho = value; }
            public double Standard_Deviation { get => GetStandard_deviation(CorP, V, D, T, Steps, U, Option_Price, matrix1, Trials); set => Standard_Deviation = value; }

            public int z = System.Environment.ProcessorCount;//number of processors
            public string A { get; set; }
            public string CV { get; set; }
            public string M { get; set; }
            public double RP { get; set; }
            public double[][,] matrix3 { get; set; }//simulation matrix
            public int[] incrementmatrix { get => createincrementmatrix(); set => incrementmatrix = value; }//matrix required for multithreading
            private int[] createincrementmatrix()
            {
                int increment = Trials / z;
                int[] incrementmatrix1 = new int[z + 1];
                incrementmatrix1[0] = 0;
                int temp = 0;
                for (int x = 1; x < z + 1; x++)
                {
                    temp = increment + temp;
                    incrementmatrix1[x] = temp;
                    Console.WriteLine("i:{0}, value:{1}", x, incrementmatrix1[x]);
                }
                return incrementmatrix1;
            }


            private double GetOptionPrice(String C, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {

                matrix3 = Simulations.CreateSimulatedMatrix(CV, A, Trials, Steps);

                List<Thread> threadlist2 = new List<Thread>(z);
                if (CV == "CV" && A == "A")
                {
                    if (M == "M")
                    {

                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticDeltaVariate(C, S, V, D, T, Trials, Steps, U, matrix1);
                    }
                    double sum_CT = 0;
                    double sum_CT2 = 0;
                    double beta1 = -1;
                    double CT;
                    double CT1;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double rebate = RP;
                            if (matrix3[0][z, Steps - 1] - S>0)
                            {
                                CT = rebate + (beta1 * matrix3[2][z, 0]);
                            }
                            else
                            {
                                CT = (beta1 * matrix3[2][z, 0]);
                            }
                            if (matrix3[1][z, Steps - 1] - S > 0)
                            {
                                CT1 = rebate + (beta1 * matrix3[2][z, 1]);
                            }
                            else
                            {
                                CT1 = beta1 * matrix3[2][z, 1];
                            }
                            sum_CT = sum_CT + ((CT+CT1)/ 2);
                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        //Console.ReadLine();
                    }

                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double rebate = RP;
                            if (S-matrix3[0][z, Steps - 1]  > 0)
                            {
                                CT = rebate + (beta1 * matrix3[2][z, 0]);
                            }
                            else
                            {
                                CT = (beta1 * matrix3[2][z, 0]);
                            }
                            if (S-matrix3[1][z, Steps - 1]  > 0)
                            {
                                CT1 = rebate + (beta1 * matrix3[2][z, 1]);
                            }
                            else
                            {
                                CT1 = beta1 * matrix3[2][z, 1];
                            }
                            sum_CT = sum_CT + ((CT + CT1) / 2);

                            //m_CT2 = sum_CT2 + (CT * CT);

                        }

                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return (sum_CT / Convert.ToDouble(Trials)) * Math.Exp(-D * T);
                }
                if (CV == "CV")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.deltacontrolvariate(C, V, D, R, Trials, Steps, U, S, matrix1);
                    }
                    double[][,] matrix2 = matrix3;
                    double sum_CT = 0;
                    double sum_CT2 = 0;
                    double beta1 = -1;
                    double CT;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            //for the digital payoff
                            double rebate = RP;
                            if (matrix2[0][z, Steps - 1] - S <0)
                            {
                                CT = rebate + (beta1 * matrix2[1][z, 0]);
                            }
                            else
                            {
                                CT = (beta1 * matrix2[1][z, 0]);
                            }
                            
                            sum_CT = sum_CT + CT;

                            sum_CT2 = sum_CT2 + (CT * CT);

                            //Console.WriteLine("payoff:,{0}, average:, {1},  CV:, {2}", Math.Max(matrix2[0][z, Steps - 1] - S, 0), CT, matrix2[1][z, 0]);
                            //Console.WriteLine(temp);

                        }
                        Console.ReadLine();
                    }

                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            CT = Math.Max(S - matrix2[0][z, Steps - 1], 0) + (beta1 * matrix2[1][z, 0]);
                            sum_CT = sum_CT + CT;
                            sum_CT2 = sum_CT2 + (CT * CT);

                        }

                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return (sum_CT / Convert.ToDouble(Trials)) * Math.Exp(-D * T);
                }

                if (A == "A")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double temp = 0;
                    double temp1 = 0;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            matrix3[0][z, Steps - 1] = Math.Max(matrix3[0][z, Steps - 1] - S, 0);
                            matrix3[1][z, Steps - 1] = Math.Max(matrix3[1][z, Steps - 1] - S, 0);
                            double rebate = RP;
                            if (matrix3[0][z, Steps - 1] > 0)
                            {
                                temp += rebate;
                            }
                            else
                            {
                                temp += 0;
                            }
                            if (matrix3[1][z, Steps - 1] > 0)
                            {
                                temp1 += rebate;
                            }
                            else
                            {
                                temp1 += 0;
                            }
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        Console.ReadLine();
                    }
                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double rebate = RP;
                            matrix3[0][z, Steps - 1] = Math.Max(S - matrix3[0][z, Steps - 1], 0);
                            matrix3[1][z, Steps - 1] = Math.Max(matrix3[1][z, Steps - 1] - S, 0);
                            if (matrix3[0][z, Steps - 1] > 0)
                            {
                                temp += rebate;
                            }
                            else
                            {
                                temp += 0;
                            }
                            if (matrix3[1][z, Steps - 1] > 0)
                            {
                                temp1 += rebate;
                            }
                            else
                            {
                                temp1 += 0;
                            }
                            //Co
                        }
                    }
                    return 0.5 * ((temp / Trials) + (temp1 / Trials)) * Math.Exp(-R * T);
                }
                else
                {
                    if (M == "M")
                    {
                        //matrix3 = Simulations.GenerateSimulations2(V, D, T, Trials, Steps, U, matrix1);
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.GenerateSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[,] matrix2 = matrix3[0];
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            matrix2[z, Steps - 1] = Math.Max(matrix2[z, Steps - 1] - S, 0);
                            double rebate = RP;
                            if (matrix2[z, Steps - 1] > 0)
                            {
                                temp += rebate;
                            }
                            else
                            {
                                temp += 0;
                            }

                            //Console.WriteLine(temp);
                        }
                    }
                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            matrix2[z, Steps - 1] = Math.Max(S - matrix2[z, Steps - 1], 0);
                            double rebate = RP;
                            if (matrix2[z, Steps - 1] > 0)
                            {
                                temp += rebate;
                            }
                            else
                            {
                                temp += 0;
                            }
                        }
                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return temp / Trials * Math.Exp(-R * T);
                }
            }
            private double GetDelta(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaS = U * 0.001;

                return (GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U + deltaS, R, matrix1) - GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U - deltaS, R, matrix1)) / (2 * deltaS);
            }
            private double GetGamma(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaS = U * 0.001;
                return (GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U + deltaS, R, matrix1) - 2 * GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1) + GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U - deltaS, R, matrix1)) / (deltaS * deltaS);
            }
            private double GetVega(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaV = V * 0.001;
                return (GetOptionPrice(CorP, S, V + deltaV, D, T, Trials, Steps, U, R, matrix1) - GetOptionPrice(CorP, S, V - deltaV, D, T, Trials, Steps, U, R, matrix1)) / (2 * deltaV);
            }
            private double GetTheta(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaT = T * 0.001;
                return -(GetOptionPrice(CorP, S, V, D, T + deltaT, Trials, Steps, U, R, matrix1) - GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1)) / (deltaT);
            }
            private double GetRho(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaR = R * 0.001;
                return (GetOptionPrice(CorP, S, V, D + deltaR, T, Trials, Steps, U, R + deltaR, matrix1) - GetOptionPrice(CorP, S, V, D - deltaR, T, Trials, Steps, U, R - deltaR, matrix1)) / (deltaR * 2);
            }

            private double GetStandard_deviation(String CorP, double V, double D, double T, int Steps, double U, double option, double[,] matrix2, int trials)
            {
                List<Thread> threadlist2 = new List<Thread>(z);
                //Standard deviation for anithetic + delta variate
                if (CV == "CV" && A == "A")
                {
                    if (M == "M")
                    {

                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticDeltaVariate(CorP, S, V, D, T, Trials, Steps, U, matrix1);
                    }

                    double[] intrinsic = new double[trials];
                    double[] intrinsic1 = new double[trials];
                    double temp = 0;
                    double beta1 = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double CT;
                            double CT1;
                            double rebate = RP;
                            if (matrix3[0][z, Steps - 1] - S > 0)
                            {
                                CT = rebate + (beta1 * matrix3[2][z, 0]);
                            }
                            else
                            {
                                CT = (beta1 * matrix3[2][z, 0]);
                            }
                            if (matrix3[1][z, Steps - 1] - S > 0)
                            {
                                CT1 = rebate + (beta1 * matrix3[2][z, 1]);
                            }
                            else
                            {
                                CT1 = beta1 * matrix3[2][z, 1];
                            }
                            intrinsic[i] = (CT1+ CT) * 0.5;
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {

                            double CT;
                            double CT1;
                            double rebate = RP;
                            if (S-matrix3[0][z, Steps - 1] > 0)
                            {
                                CT = rebate + (beta1 * matrix3[2][z, 0]);
                            }
                            else
                            {
                                CT = (beta1 * matrix3[2][z, 0]);
                            }
                            if (S-matrix3[1][z, Steps - 1] > 0)
                            {
                                CT1 = rebate + (beta1 * matrix3[2][z, 1]);
                            }
                            else
                            {
                                CT1 = beta1 * matrix3[2][z, 1];
                            }
                            intrinsic[i] = (CT1 + CT) * 0.5;
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));
                    //Console.ReadLine();
                    return x;
                }
                if (CV == "CV")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.deltacontrolvariate(CorP, V, D, R, Trials, Steps, U, S, matrix1);
                    }
                    double[] intrinsic = new double[trials];
                    double temp = 0;
                    double beta1 = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        { 
                            double CT;
                            double rebate = RP;
                            if (matrix3[0][z, Steps - 1] - S < 0)
                            {
                                CT = rebate + (beta1 * matrix3[1][z, 0]);
                            }
                            else
                            {
                                CT = (beta1 * matrix3[1][z, 0]);
                            }
                            intrinsic[i] = CT;
                            temp += Math.Pow((intrinsic[i] - option), 2);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double CT;
                            double rebate = RP;
                            if (S-matrix3[0][z, Steps - 1] < 0)
                            {
                                CT = rebate + (beta1 * matrix3[1][z, 0]);
                            }
                            else
                            {
                                CT = (beta1 * matrix3[1][z, 0]);
                            }
                            intrinsic[i] = CT;
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));

                    return x;
                }
                if (A == "A")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[] intrinsic = new double[trials];
                    double[] intrinsic1 = new double[trials];
                    double beta1 = -1;
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = Math.Max(matrix3[0][i, Steps - 1] - S, 0);
                            intrinsic1[i] = Math.Max(matrix3[1][i, Steps - 1] - S, 0);
                            temp += Math.Pow((intrinsic[i] - option), 2) + Math.Pow((intrinsic1[i] - option), 2);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            matrix3[0][z, Steps - 1] = Math.Max(S- matrix3[0][z, Steps - 1] , 0);
                            matrix3[1][z, Steps - 1] = Math.Max(S- matrix3[1][z, Steps - 1] , 0);
                            double rebate = RP;
                            if (matrix3[0][z, Steps - 1] > 0)
                            {
                                intrinsic[i] += rebate;
                            }
                            else
                            {
                                intrinsic[i] += 0;
                            }
                            if (matrix3[1][z, Steps - 1] > 0)
                            {
                                intrinsic1[i] += rebate;
                            }
                            else
                            {
                                intrinsic1[i] += 0;
                            }
                            temp += Math.Pow((intrinsic[i] - option), 2) + Math.Pow((intrinsic1[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));

                    return x;
                }
                else
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.GenerateSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[,] matrix = matrix3[0];
                    double[] intrinsic = new double[trials];
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = Math.Max(matrix[i, Steps - 1] - S, 0);
                            double rebate = RP;
                            if (intrinsic[i]> 0)
                            {
                                intrinsic[i] += rebate;
                            }
                            else
                            {
                                intrinsic[i] += 0;
                            }
                            temp += Math.Pow((intrinsic[i] - option), 2);
                            //Console.WriteLine("Yes");


                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = Math.Max(S - matrix[i, Steps - 1], 0);
                            double rebate = RP;
                            if (intrinsic[i] > 0)
                            {
                                intrinsic[i] += rebate;
                            }
                            else
                            {
                                intrinsic[i] += 0;
                            }
                            temp += Math.Pow((intrinsic[i] - option), 2);
                        }
                    }

                    double x = ((Math.Sqrt(temp / (trials - 1))) / Math.Sqrt(trials));

                    return x;
                }
            }
        }
        public class Range_Option : Option
        {
            public double Option_Price { get => GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Option_Price = value; }
            public double Delta { get => GetDelta(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Delta = value; }
            public double Gamma { get => GetGamma(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Gamma = value; }
            public double Vega { get => GetVega(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Vega = value; }
            public double Theta { get => GetTheta(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Theta = value; }
            public double Rho { get => GetRho(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Rho = value; }
            public double Standard_Deviation { get => GetStandard_deviation(CorP, V, D, T, Steps, U, Option_Price, matrix1, Trials); set => Standard_Deviation = value; }

            public int z = System.Environment.ProcessorCount;//number of processors
            public string A { get; set; }
            public string CV { get; set; }
            public string M { get; set; }
            public double[][,] matrix3 { get; set; }//simulation matrix
            public int[] incrementmatrix { get => createincrementmatrix(); set => incrementmatrix = value; }//matrix required for multithreading
            private int[] createincrementmatrix()
            {
                int increment = Trials / z;
                int[] incrementmatrix1 = new int[z + 1];
                incrementmatrix1[0] = 0;
                int temp = 0;
                for (int x = 1; x < z + 1; x++)
                {
                    temp = increment + temp;
                    incrementmatrix1[x] = temp;
                    Console.WriteLine("i:{0}, value:{1}", x, incrementmatrix1[x]);
                }
                return incrementmatrix1;
            }


            private double GetOptionPrice(String C, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {

                matrix3 = Simulations.CreateSimulatedMatrix(CV, A, Trials, Steps);

                List<Thread> threadlist2 = new List<Thread>(z);
                if (CV == "CV" && A == "A")
                {
                    if (M == "M")
                    {

                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticDeltaVariate(C, S, V, D, T, Trials, Steps, U, matrix1);
                    }


                    double sum_CT = 0;
                    double sum_CT2 = 0;
                    double beta1 = -1;
                    double CT;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            //finding maximum and minimum values for range option
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            double Maximum1 = matrix3[1][z, 0];
                            double minimum1 = matrix3[1][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g]>Maximum  )
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] > Maximum)
                                {
                                    Maximum1 = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] < minimum)
                                {
                                    minimum1 = matrix3[0][z, g];
                                }

                            }

                            CT = (Math.Max(Maximum - minimum, 0) + Math.Max(Maximum1 - minimum1, 0) + (beta1 * matrix3[2][z, 0]) + (beta1 * matrix3[2][z, 1])) * 0.5;
                            sum_CT = sum_CT + CT;
                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        //Console.ReadLine();
                    }

                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            double Maximum1 = matrix3[1][z, 0];
                            double minimum1 = matrix3[1][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] > Maximum)
                                {
                                    Maximum1 = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] < minimum)
                                {
                                    minimum1 = matrix3[0][z, g];
                                }

                            }

                            CT = (Math.Max(Maximum - minimum, 0) + Math.Max(Maximum1 - minimum1, 0) + (beta1 * matrix3[2][z, 0]) + (beta1 * matrix3[2][z, 1])) * 0.5;
                            sum_CT = sum_CT + CT;
                            //m_CT2 = sum_CT2 + (CT * CT);

                        }

                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return (sum_CT / Convert.ToDouble(Trials)) * Math.Exp(-D * T);
                }
                if (CV == "CV")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.deltacontrolvariate(C, V, D, R, Trials, Steps, U, S, matrix1);
                    }
                    double[][,] matrix2 = matrix3;
                    double sum_CT = 0;
                    double sum_CT2 = 0;
                    double beta1 = -1;
                    double CT;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }

                            }
                            CT = Math.Max(Maximum - minimum, 0) + (beta1 * matrix2[1][z, 0]);
                            sum_CT = sum_CT + CT;
                            sum_CT2 = sum_CT2 + (CT * CT);

                            //Console.WriteLine("payoff:,{0}, average:, {1},  CV:, {2}", Math.Max(matrix2[0][z, Steps - 1] - S, 0), CT, matrix2[1][z, 0]);
                            //Console.WriteLine(temp);

                        }
                        Console.ReadLine();
                    }

                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }

                            }
                            CT = Math.Max(Maximum - minimum, 0) + (beta1 * matrix2[1][z, 0]);
                            sum_CT = sum_CT + CT;
                            sum_CT2 = sum_CT2 + (CT * CT);
                        }

                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return (sum_CT / Convert.ToDouble(Trials)) * Math.Exp(-D * T);
                }
                if (A == "A")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double temp = 0;
                    double temp1 = 0;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            //averaging the stockprices for path dependent Range options
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            double Maximum1 = matrix3[1][z, 0];
                            double minimum1 = matrix3[1][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] > Maximum)
                                {
                                    Maximum1 = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] < minimum)
                                {
                                    minimum1 = matrix3[0][z, g];
                                }
                            }
                            temp = (Maximum-minimum) + temp;
                            temp1 =  (Maximum1- minimum1) + temp1;
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        Console.ReadLine();
                    }
                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            double Maximum1 = matrix3[1][z, 0];
                            double minimum1 = matrix3[1][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] > Maximum)
                                {
                                    Maximum1 = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] < minimum)
                                {
                                    minimum1 = matrix3[0][z, g];
                                }
                            }
                            temp = (Maximum - minimum) + temp;
                            temp1 = (Maximum1 - minimum1) + temp1;
                        }
                    }
                    return 0.5 * ((temp / Trials) + (temp1 / Trials)) * Math.Exp(-R * T);
                }
                else
                {
                    if (M == "M")
                    {
                        //matrix3 = Simulations.GenerateSimulations2(V, D, T, Trials, Steps, U, matrix1);
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.GenerateSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[,] matrix2 = matrix3[0];
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {

                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }

                            }
                            temp = Maximum-minimum + temp;
                            //Console.WriteLine(temp);
                        }
                    }
                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {

                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }

                            }
                            temp = Maximum - minimum + temp;
                        }
                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return temp / Trials * Math.Exp(-R * T);
                }
            }
            private double GetDelta(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaS = U * 0.001;

                return (GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U + deltaS, R, matrix1) - GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U - deltaS, R, matrix1)) / (2 * deltaS);
            }
            private double GetGamma(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaS = U * 0.001;
                return (GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U + deltaS, R, matrix1) - 2 * GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1) + GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U - deltaS, R, matrix1)) / (deltaS * deltaS);
            }
            private double GetVega(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaV = V * 0.001;
                return (GetOptionPrice(CorP, S, V + deltaV, D, T, Trials, Steps, U, R, matrix1) - GetOptionPrice(CorP, S, V - deltaV, D, T, Trials, Steps, U, R, matrix1)) / (2 * deltaV);
            }
            private double GetTheta(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaT = T * 0.001;
                return -(GetOptionPrice(CorP, S, V, D, T + deltaT, Trials, Steps, U, R, matrix1) - GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1)) / (deltaT);
            }
            private double GetRho(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaR = R * 0.001;
                return (GetOptionPrice(CorP, S, V, D + deltaR, T, Trials, Steps, U, R + deltaR, matrix1) - GetOptionPrice(CorP, S, V, D - deltaR, T, Trials, Steps, U, R - deltaR, matrix1)) / (deltaR * 2);
            }

            private double GetStandard_deviation(String CorP, double V, double D, double T, int Steps, double U, double option, double[,] matrix2, int trials)
            {
                List<Thread> threadlist2 = new List<Thread>(z);
                //Standard deviation for anithetic + delta variate
                if (CV == "CV" && A == "A")
                {
                    if (M == "M")
                    {

                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticDeltaVariate(CorP, S, V, D, T, Trials, Steps, U, matrix1);
                    }

                    double[] intrinsic = new double[trials];
                    double[] intrinsic1 = new double[trials];
                    double temp = 0;
                    double beta1 = 0;
                    double CT;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            //finding maximum and minimum values for range option
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            double Maximum1 = matrix3[1][z, 0];
                            double minimum1 = matrix3[1][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] > Maximum)
                                {
                                    Maximum1 = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] < minimum)
                                {
                                    minimum1 = matrix3[0][z, g];
                                }

                            }

                            CT = (Math.Max(Maximum - minimum, 0) + Math.Max(Maximum1 - minimum1, 0) + (beta1 * matrix3[2][z, 0]) + (beta1 * matrix3[2][z, 1])) * 0.5;
                            
                            temp += Math.Pow((CT- option), 2);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            //finding maximum and minimum values for range option
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            double Maximum1 = matrix3[1][z, 0];
                            double minimum1 = matrix3[1][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] > Maximum)
                                {
                                    Maximum1 = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] < minimum)
                                {
                                    minimum1 = matrix3[0][z, g];
                                }

                            }

                            CT = (Math.Max(Maximum - minimum, 0) + Math.Max(Maximum1 - minimum1, 0) + (beta1 * matrix3[2][z, 0]) + (beta1 * matrix3[2][z, 1])) * 0.5;

                            temp += Math.Pow((CT - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));
                    //Console.ReadLine();
                    return x;
                }
                if (CV == "CV")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.deltacontrolvariate(CorP, V, D, R, Trials, Steps, U, S, matrix1);
                    }
                    double[] intrinsic = new double[trials];
                    double temp = 0;
                    double beta1 = 0;
                    double CT;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }

                            }
                            CT = Math.Max(Maximum - minimum, 0) + (beta1 * matrix3[1][z, 0]);

                            temp += Math.Pow((CT - option), 2);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }

                            }
                            CT = Math.Max(Maximum - minimum, 0) + (beta1 * matrix3[1][z, 0]);

                            temp += Math.Pow((CT - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));

                    return x;
                }
                if (A == "A")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[] intrinsic = new double[trials];
                    double[] intrinsic1 = new double[trials];
                    double beta1 = -1;
                    double temp = 0;
                    double CT1, CT;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            //averaging the stockprices for path dependent Asian options
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            double Maximum1 = matrix3[1][z, 0];
                            double minimum1 = matrix3[1][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] > Maximum)
                                {
                                    Maximum1 = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] < minimum)
                                {
                                    minimum1 = matrix3[0][z, g];
                                }
                            }
                            CT = (Maximum - minimum) ;
                            CT1 = (Maximum1 - minimum1) ;
                            temp += Math.Pow((CT- option), 2) + Math.Pow((CT1 - option), 2);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            //averaging the stockprices for path dependent Asian options
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            double Maximum1 = matrix3[1][z, 0];
                            double minimum1 = matrix3[1][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] > Maximum)
                                {
                                    Maximum1 = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] < minimum)
                                {
                                    minimum1 = matrix3[0][z, g];
                                }
                            }
                            CT = (Maximum - minimum);
                            CT1 = (Maximum1 - minimum1);
                            temp += Math.Pow((CT - option), 2) + Math.Pow((CT1 - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));

                    return x;
                }
                else
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.GenerateSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[,] matrix = matrix3[0];
                    double[] intrinsic = new double[trials];
                    double temp = 0;
                    double CT;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }

                            }
                            CT = Maximum - minimum ;
                            temp += Math.Pow((CT - option), 2);
                            //Console.WriteLine("Yes");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Maximum = matrix3[0][z, 0];
                            double minimum = matrix3[0][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[0][z, g] < minimum)
                                {
                                    minimum = matrix3[0][z, g];
                                }

                            }
                            CT = Maximum - minimum;
                            temp += Math.Pow((CT - option), 2);
                            //Console.WriteLine("Yes");

                        }
                    }

                    double x = ((Math.Sqrt(temp / (trials - 1))) / Math.Sqrt(trials));

                    return x;
                }
            }
        }
        public class Barrier : Option
        {
            public double Option_Price { get => GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Option_Price = value; }
            public double Delta { get => GetDelta(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Delta = value; }
            public double Gamma { get => GetGamma(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Gamma = value; }
            public double Vega { get => GetVega(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Vega = value; }
            public double Theta { get => GetTheta(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Theta = value; }
            public double Rho { get => GetRho(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Rho = value; }
            public double Standard_Deviation { get => GetStandard_deviation(CorP, V, D, T, Steps, U, Option_Price, matrix1, Trials); set => Standard_Deviation = value; }

            public int z = System.Environment.ProcessorCount;//number of processors
            public string A { get; set; }
            public double BarrierLine { get; set; }
            public string Type { get; set; }
            public string CV { get; set; }
            public string M { get; set; }
            public double[][,] matrix3 { get; set; }//simulation matrix
            public int[] incrementmatrix { get => createincrementmatrix(); set => incrementmatrix = value; }//matrix required for multithreading
            private int[] createincrementmatrix()
            {
                int increment = Trials / z;
                int[] incrementmatrix1 = new int[z + 1];
                incrementmatrix1[0] = 0;
                int temp = 0;
                for (int x = 1; x < z + 1; x++)
                {
                    temp = increment + temp;
                    incrementmatrix1[x] = temp;
                    Console.WriteLine("i:{0}, value:{1}", x, incrementmatrix1[x]);
                }
                return incrementmatrix1;
            }


            private double GetOptionPrice(String C, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {

                matrix3 = Simulations.CreateSimulatedMatrix(CV, A, Trials, Steps);

                List<Thread> threadlist2 = new List<Thread>(z);
                if (CV == "CV" && A == "A")
                {
                    if (M == "M")
                    {

                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticDeltaVariate(C, S, V, D, T, Trials, Steps, U, matrix1);
                    }
                    double sum_CT = 0;
                    double sum_CT2 = 0;
                    double beta1 = -1;
                    double CT;
                    double CT1;

                    if (CorP == "C")
                    {
                        if (Type == "DO")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                            
                                for (int g = 0; g < Steps; g++)
                                {
                                    if(matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] < BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x== false)
                                {
                                    CT = Math.Max(matrix3[0][z, Steps - 1] - S, 0) + (beta1 * matrix3[2][z, 0]);
                                }
                                else
                                {
                                    CT = (beta1 * matrix3[2][z, 0]);
                                }
                                if (x1== false)
                                {
                                    CT1 = Math.Max(matrix3[1][z, Steps - 1] - S, 0) + (beta1 * matrix3[2][z, 1]);
                                }
                                else
                                {
                                    CT1 = (beta1 * matrix3[2][z, 1]);
                                }
                                sum_CT = sum_CT + (CT + CT1) / 2;
                            }

                            
                            
                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        if (Type == "DI")
                        {
                            bool x = false;
                            bool x1 = false;

                            for (int z = 0; z < Trials; z++)
                            {

                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] < BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    CT =  (beta1 * matrix3[2][z, 0]);
                                }
                                else
                                {
                                    CT = Math.Max(matrix3[0][z, Steps - 1] - S, 0) + (beta1 * matrix3[2][z, 0]);
                                }
                                if (x1 == false)
                                {
                                    CT1 = (beta1 * matrix3[2][z, 1]);
                                }
                                else
                                {
                                    CT1 = Math.Max(matrix3[1][z, Steps - 1] - S, 0) + (beta1 * matrix3[2][z, 1]);
                                }
                                sum_CT = sum_CT + (CT + CT1) / 2;
                            }



                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        if (Type=="UO")
                        {
                            bool x = false;
                            bool x1 = false;

                            for (int z = 0; z < Trials; z++)
                            {

                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)//barrier crossed
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] > BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    CT = Math.Max(matrix3[0][z, Steps - 1] - S, 0) + (beta1 * matrix3[2][z, 0]);
                                }
                                else
                                {
                                    CT =  (beta1 * matrix3[2][z, 0]);
                                }
                                if (x1 == false)
                                {
                                    CT1 = Math.Max(matrix3[1][z, Steps - 1] - S, 0) + (beta1 * matrix3[2][z, 1]);
                                }
                                else
                                {
                                    CT1 =  (beta1 * matrix3[2][z, 1]);
                                }
                                sum_CT = sum_CT + (CT + CT1) / 2;
                            }



                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        if (Type == "UI")
                        {
                            bool x = false;
                            bool x1 = false;

                            for (int z = 0; z < Trials; z++)
                            {

                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] > BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    CT = (beta1 * matrix3[2][z, 0]);
                                }
                                else
                                {
                                    CT = Math.Max(matrix3[0][z, Steps - 1] - S, 0) + (beta1 * matrix3[2][z, 0]);
                                }
                                if (x1 == false)
                                {
                                    CT1 = (beta1 * matrix3[2][z, 1]);
                                }
                                else
                                {
                                    CT1 = Math.Max(matrix3[1][z, Steps - 1] - S, 0) + (beta1 * matrix3[2][z, 1]);
                                }
                                sum_CT = sum_CT + (CT + CT1) / 2;
                            }



                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }

                        //Console.ReadLine();
                    }

                    else
                    {
                        if (Type == "DO")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;

                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] < BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    CT = Math.Max(S-matrix3[0][z, Steps - 1] , 0) + (beta1 * matrix3[2][z, 0]);
                                }
                                else
                                {
                                    CT = (beta1 * matrix3[2][z, 0]);
                                }
                                if (x1 == false)
                                {
                                    CT1 = Math.Max(S-matrix3[1][z, Steps - 1] , 0) + (beta1 * matrix3[2][z, 1]);
                                }
                                else
                                {
                                    CT1 = (beta1 * matrix3[2][z, 1]);
                                }
                                sum_CT = sum_CT + (CT + CT1) / 2;
                            }



                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        if (Type == "DI")
                        {
                            
                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;

                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] < BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    CT = (beta1 * matrix3[2][z, 0]);
                                }
                                else
                                {
                                    CT = Math.Max(S-matrix3[0][z, Steps - 1] , 0) + (beta1 * matrix3[2][z, 0]);
                                }
                                if (x1 == false)
                                {
                                    CT1 = (beta1 * matrix3[2][z, 1]);
                                }
                                else
                                {
                                    CT1 = Math.Max(S-matrix3[1][z, Steps - 1] , 0) + (beta1 * matrix3[2][z, 1]);
                                }
                                sum_CT = sum_CT + (CT + CT1) / 2;
                            }



                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        if (Type == "UO")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;

                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)//barrier crossed
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] > BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    CT = Math.Max(S-matrix3[0][z, Steps - 1] , 0) + (beta1 * matrix3[2][z, 0]);
                                }
                                else
                                {
                                    CT = (beta1 * matrix3[2][z, 0]);
                                }
                                if (x1 == false)
                                {
                                    CT1 = Math.Max(S-matrix3[1][z, Steps - 1] , 0) + (beta1 * matrix3[2][z, 1]);
                                }
                                else
                                {
                                    CT1 = (beta1 * matrix3[2][z, 1]);
                                }
                                sum_CT = sum_CT + (CT + CT1) / 2;
                            }



                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        if (Type == "UI")
                        {
                            bool x = false;
                            bool x1 = false;

                            for (int z = 0; z < Trials; z++)
                            {

                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] > BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    CT = (beta1 * matrix3[2][z, 0]);
                                }
                                else
                                {
                                    CT = Math.Max(S-matrix3[0][z, Steps - 1], 0) + (beta1 * matrix3[2][z, 0]);
                                }
                                if (x1 == false)
                                {
                                    CT1 = (beta1 * matrix3[2][z, 1]);
                                }
                                else
                                {
                                    CT1 = Math.Max(S-matrix3[1][z, Steps - 1], 0) + (beta1 * matrix3[2][z, 1]);
                                }
                                sum_CT = sum_CT + (CT + CT1) / 2;
                            }



                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }

                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return (sum_CT / Convert.ToDouble(Trials)) * Math.Exp(-D * T);
                }
                if (CV == "CV")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.deltacontrolvariate(C, V, D, R, Trials, Steps, U, S, matrix1);
                    }
                    double[][,] matrix2 = matrix3;
                    double sum_CT = 0;
                    double sum_CT2 = 0;
                    double beta1 = -1;
                    double CT;

                    if (CorP == "C")
                    {
                        if (Type == "DO")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == false)
                                {
                                    CT = Math.Max(matrix2[0][z, Steps - 1] - S, 0) + (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);

                                }
                                else
                                {
                                    CT = (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);
                                }


                            }

                        }
                        if (Type == "DI")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == true)
                                {
                                    CT = Math.Max(matrix2[0][z, Steps - 1] - S, 0) + (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);

                                }
                                else
                                {
                                    CT = (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);
                                }


                            }


                        }
                        if (Type == "UO")
                        {
                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == false)
                                {
                                    CT = Math.Max(matrix2[0][z, Steps - 1] - S, 0) + (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);

                                }
                                else
                                {
                                    CT = (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);
                                }


                            }

                        }
                        if (Type == "UI")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == true)
                                {
                                    CT = Math.Max(matrix2[0][z, Steps - 1] - S, 0) + (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);

                                }
                                else
                                {
                                    CT = (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);
                                }


                            }
                        }

                    }

                    else
                    {
                        if (Type == "DO")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == false)
                                {
                                    CT = Math.Max(S-matrix2[0][z, Steps - 1] , 0) + (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);

                                }
                                else
                                {
                                    CT = (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);
                                }


                            }

                        }
                        if (Type == "DI")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == true)
                                {
                                    CT = Math.Max(S-matrix2[0][z, Steps - 1], 0) + (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);

                                }
                                else
                                {
                                    CT = (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);
                                }


                            }


                        }
                        if (Type == "UO")
                        {
                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == false)
                                {
                                    CT = Math.Max(S-matrix2[0][z, Steps - 1] , 0) + (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);

                                }
                                else
                                {
                                    CT = (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);
                                }


                            }

                        }
                        if (Type == "UI")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == true)
                                {
                                    CT = Math.Max(S-matrix2[0][z, Steps - 1] , 0) + (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);

                                }
                                else
                                {
                                    CT = (beta1 * matrix2[1][z, 0]);
                                    sum_CT = sum_CT + CT;
                                    sum_CT2 = sum_CT2 + (CT * CT);
                                }


                            }
                        }

                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return (sum_CT / Convert.ToDouble(Trials)) * Math.Exp(-D * T);
                }

                if (A == "A")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double temp = 0;
                    double temp1 = 0;
                    if (CorP == "C")
                    {
                        if (Type == "DO")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] < BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    matrix3[0][z, Steps - 1] = Math.Max(matrix3[0][z, Steps - 1] - S, 0);

                                    temp = matrix3[0][z, Steps - 1] + temp;

                                }
                                else
                                {
                                    temp += 0;
                                }
                                if (x1 == false)
                                {
                                    matrix3[1][z, Steps - 1] = Math.Max(matrix3[1][z, Steps - 1] - S, 0);
                                    temp1 = matrix3[1][z, Steps - 1] + temp1;
                                }
                                else
                                {
                                    temp1 += 0;
                                }

                            }

                        }
                        if (Type == "DI")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] < BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    temp += 0;

                                }
                                else
                                {

                                    matrix3[0][z, Steps - 1] = Math.Max(matrix3[0][z, Steps - 1] - S, 0);

                                    temp = matrix3[0][z, Steps - 1] + temp;
                                }
                                if (x1 == false)
                                {
                                    temp1 += 0;
                                }
                                else
                                {
                                    matrix3[1][z, Steps - 1] = Math.Max(matrix3[1][z, Steps - 1] - S, 0);
                                    temp1 = matrix3[1][z, Steps - 1] + temp1;

                                }

                            }

                        }
                        if (Type == "UO")
                        {
                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] > BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    matrix3[0][z, Steps - 1] = Math.Max(matrix3[0][z, Steps - 1] - S, 0);

                                    temp = matrix3[0][z, Steps - 1] + temp;

                                }
                                else
                                {
                                    temp += 0;

                                }
                                if (x1 == false)
                                {
                                    matrix3[1][z, Steps - 1] = Math.Max(matrix3[1][z, Steps - 1] - S, 0);
                                    temp1 = matrix3[1][z, Steps - 1] + temp1;

                                }
                                else
                                {

                                    temp1 += 0;
                                }

                            }
                        }
                        if (Type == "UI")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] > BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    temp += 0;

                                }
                                else
                                {
                                    matrix3[0][z, Steps - 1] = Math.Max(matrix3[0][z, Steps - 1] - S, 0);

                                    temp = matrix3[0][z, Steps - 1] + temp;


                                }
                                if (x1 == false)
                                {

                                    temp1 += 0;

                                }
                                else
                                {

                                    matrix3[1][z, Steps - 1] = Math.Max(matrix3[1][z, Steps - 1] - S, 0);
                                    temp1 = matrix3[1][z, Steps - 1] + temp1;
                                }

                            }
                        }
                    }
                    else//put_option
                    {
                        if(Type == "DO")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] < BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    matrix3[0][z, Steps - 1] = Math.Max(S-matrix3[0][z, Steps - 1] , 0);

                                    temp = matrix3[0][z, Steps - 1] + temp;

                                }
                                else
                                {
                                    temp += 0;
                                }
                                if (x1 == false)
                                {
                                    matrix3[1][z, Steps - 1] = Math.Max(S-matrix3[1][z, Steps - 1] , 0);
                                    temp1 = matrix3[1][z, Steps - 1] + temp1;
                                }
                                else
                                {
                                    temp1 += 0;
                                }

                            }

                        }
                        if (Type == "DI")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] < BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    temp += 0;

                                }
                                else
                                {
                                    
                                    matrix3[0][z, Steps - 1] = Math.Max(S-matrix3[0][z, Steps - 1], 0);

                                    temp = matrix3[0][z, Steps - 1] + temp;
                                }
                                if (x1 == false)
                                {
                                    temp1 += 0;
                                }
                                else
                                {
                                    matrix3[1][z, Steps - 1] = Math.Max(S-matrix3[1][z, Steps - 1], 0);
                                    temp1 = matrix3[1][z, Steps - 1] + temp1;

                                }

                            }

                        }
                        if (Type == "UO")
                        {
                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] > BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    matrix3[0][z, Steps - 1] = Math.Max(S-matrix3[0][z, Steps - 1] , 0);

                                    temp = matrix3[0][z, Steps - 1] + temp;

                                }
                                else
                                {
                                    temp += 0;

                                }
                                if (x1 == false)
                                {
                                    matrix3[1][z, Steps - 1] = Math.Max(S-matrix3[1][z, Steps - 1] , 0);
                                    temp1 = matrix3[1][z, Steps - 1] + temp1;

                                }
                                else
                                {

                                    temp1 += 0;
                                }

                            }
                        }
                        if (Type == "UI")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[1][z, g] > BarrierLine)
                                    {
                                        x1 = true;
                                        break;
                                    }

                                }
                                if (x == false)
                                {
                                    temp += 0;

                                }
                                else
                                {
                                    matrix3[0][z, Steps - 1] = Math.Max(S-matrix3[0][z, Steps - 1] , 0);

                                    temp = matrix3[0][z, Steps - 1] + temp;


                                }
                                if (x1 == false)
                                {

                                    temp1 += 0;

                                }
                                else
                                {

                                    matrix3[1][z, Steps - 1] = Math.Max(S-matrix3[1][z, Steps - 1], 0);
                                    temp1 = matrix3[1][z, Steps - 1] + temp1;
                                }

                            }
                        }
                    }
                    return 0.5 * ((temp / Trials) + (temp1 / Trials)) * Math.Exp(-R * T);
                }
                else
                {
                    if (M == "M")
                    {
                        //matrix3 = Simulations.GenerateSimulations2(V, D, T, Trials, Steps, U, matrix1);
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.GenerateSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[,] matrix2 = matrix3[0];
                    double temp = 0;
                    if (CorP == "C")
                    {
                        if (Type == "DO")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == false)
                                {
                                    matrix2[z, Steps - 1] = Math.Max(matrix2[z, Steps - 1] - S, 0);
                                    temp = matrix2[z, Steps - 1] + temp;

                                }
                                else
                                {
                                    temp += 0;
                                }


                            }

                        }
                        if (Type == "DI")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == true)
                                {
                                    matrix2[z, Steps - 1] = Math.Max(matrix2[z, Steps - 1] - S, 0);
                                    temp = matrix2[z, Steps - 1] + temp;

                                }
                                else
                                {
                                    temp += 0;
                                }


                            }


                        }
                        if (Type == "UO")
                        {
                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix2[z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == false)
                                {
                                    matrix2[z, Steps - 1] = Math.Max(matrix2[z, Steps - 1] - S, 0);
                                    temp = matrix2[z, Steps - 1] + temp;

                                }
                                else
                                {
                                    temp += 0;
                                }


                            }

                        }
                        if (Type == "UI")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix2[z, g] > BarrierLine)
                                    {
                                        Console.WriteLine("{0}, {1}", matrix2[z, g], BarrierLine);
                                        x = true;
                                        break;
                                    }

                                }
                                Console.ReadLine();
                                if (x == true)
                                {
                                    matrix2[z, Steps - 1] = Math.Max(matrix2[z, Steps - 1] - S, 0);
                                    temp = matrix2[z, Steps - 1] + temp;

                                }
                                else
                                {
                                    temp += 0;
                                }


                            }
                        }

                    }
                    else
                    {
                        if (Type == "DO")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix2[z,g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == false)
                                {
                                    matrix2[z, Steps - 1] = Math.Max(S-matrix2[z, Steps - 1] , 0);
                                    temp = matrix2[z, Steps - 1] + temp;

                                }
                                else
                                {
                                    temp += 0;
                                }


                            }

                        }
                        if (Type == "DI")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix2[z, g] < BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == true)
                                {
                                    matrix2[z, Steps - 1] = Math.Max(S-matrix2[z, Steps - 1] , 0);
                                    temp = matrix2[z, Steps - 1] + temp;

                                }
                                else
                                {
                                    temp += 0;
                                }


                            }


                        }
                        if (Type == "UO")
                        {
                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == false)
                                {
                                    matrix2[z, Steps - 1] = Math.Max(S-matrix2[z, Steps - 1] , 0);
                                    temp = matrix2[z, Steps - 1] + temp;

                                }
                                else
                                {
                                    temp += 0;
                                }


                            }

                        }
                        if (Type == "UI")
                        {

                            for (int z = 0; z < Trials; z++)
                            {
                                bool x = false;
                                bool x1 = false;
                                for (int g = 0; g < Steps; g++)
                                {
                                    if (matrix3[0][z, g] > BarrierLine)
                                    {
                                        x = true;
                                        break;
                                    }

                                }

                                if (x == true)
                                {
                                    matrix2[z, Steps - 1] = Math.Max(S-matrix2[z, Steps - 1] , 0);
                                    temp = matrix2[z, Steps - 1] + temp;

                                }
                                else
                                {
                                    temp += 0;
                                }


                            }
                        }
                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return temp / Trials * Math.Exp(-R * T);
                }
            }
            private double GetDelta(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaS = U * 0.001;

                return (GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U + deltaS, R, matrix1) - GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U - deltaS, R, matrix1)) / (2 * deltaS);
            }
            private double GetGamma(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaS = U * 0.001;
                return (GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U + deltaS, R, matrix1) - 2 * GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1) + GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U - deltaS, R, matrix1)) / (deltaS * deltaS);
            }
            private double GetVega(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaV = V * 0.001;
                return (GetOptionPrice(CorP, S, V + deltaV, D, T, Trials, Steps, U, R, matrix1) - GetOptionPrice(CorP, S, V - deltaV, D, T, Trials, Steps, U, R, matrix1)) / (2 * deltaV);
            }
            private double GetTheta(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaT = T * 0.001;
                return -(GetOptionPrice(CorP, S, V, D, T + deltaT, Trials, Steps, U, R, matrix1) - GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1)) / (deltaT);
            }
            private double GetRho(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaR = R * 0.001;
                return (GetOptionPrice(CorP, S, V, D + deltaR, T, Trials, Steps, U, R + deltaR, matrix1) - GetOptionPrice(CorP, S, V, D - deltaR, T, Trials, Steps, U, R - deltaR, matrix1)) / (deltaR * 2);
            }

            private double GetStandard_deviation(String CorP, double V, double D, double T, int Steps, double U, double option, double[,] matrix2, int trials)
            {
                List<Thread> threadlist2 = new List<Thread>(z);
                //Standard deviation for anithetic + delta variate
                if (CV == "CV" && A == "A")
                {
                    if (M == "M")
                    {

                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticDeltaVariate(CorP, S, V, D, T, Trials, Steps, U, matrix1);
                    }

                    double[] intrinsic = new double[trials];
                    double[] intrinsic1 = new double[trials];
                    double temp = 0;
                    double beta1 = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = (Math.Max(matrix3[0][i, Steps - 1] - S, 0) + Math.Max(matrix3[0][i, Steps - 1] - S, 0) + (beta1 * matrix3[2][i, 0]) + (beta1 * matrix3[2][i, 1])) * 0.5;
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {

                            intrinsic[i] = (Math.Max(S - matrix3[0][i, Steps - 1], 0) + Math.Max(S - matrix3[0][i, Steps - 1], 0) + (beta1 * matrix3[2][i, 0]) + (beta1 * matrix3[2][i, 1])) * 0.5;
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));
                    //Console.ReadLine();
                    return x;
                }
                if (CV == "CV")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.deltacontrolvariate(CorP, V, D, R, Trials, Steps, U, S, matrix1);
                    }
                    double[] intrinsic = new double[trials];
                    double temp = 0;
                    double beta1 = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = Math.Max(matrix3[0][i, Steps - 1] - S, 0) + (beta1 * matrix3[1][i, 0]);
                            temp += Math.Pow((intrinsic[i] - option), 2);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {

                            intrinsic[i] = Math.Max(matrix3[0][i, Steps - 1] - S, 0) + (beta1 * matrix3[1][i, 0]);
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));

                    return x;
                }
                if (A == "A")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[] intrinsic = new double[trials];
                    double[] intrinsic1 = new double[trials];
                    double beta1 = -1;
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = Math.Max(matrix3[0][i, Steps - 1] - S, 0);
                            intrinsic1[i] = Math.Max(matrix3[1][i, Steps - 1] - S, 0);
                            temp += Math.Pow((intrinsic[i] - option), 2) + Math.Pow((intrinsic1[i] - option), 2);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {

                            intrinsic[i] = Math.Max(S - matrix3[0][i, Steps - 1], 0);
                            intrinsic1[i] = Math.Max(S - matrix3[1][i, Steps - 1], 0);
                            temp += Math.Pow((intrinsic[i] - option), 2) + Math.Pow((intrinsic1[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));

                    return x;
                }
                else
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.GenerateSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[,] matrix = matrix3[0];
                    double[] intrinsic = new double[trials];
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = Math.Max(matrix[i, Steps - 1] - S, 0);
                            temp += Math.Pow((intrinsic[i] - option), 2);
                            //Console.WriteLine("Yes");


                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            intrinsic[i] = Math.Max(S - matrix[i, Steps - 1], 0);
                            temp += Math.Pow((intrinsic[i] - option), 2);


                        }
                    }

                    double x = ((Math.Sqrt(temp / (trials - 1))) / Math.Sqrt(trials));

                    return x;
                }
            }
        }//calculate standard deviation
        public class FixedStrikeLookback_Option : Option
        {
            public double Option_Price { get => GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Option_Price = value; }
            public double Delta { get => GetDelta(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Delta = value; }
            public double Gamma { get => GetGamma(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Gamma = value; }
            public double Vega { get => GetVega(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Vega = value; }
            public double Theta { get => GetTheta(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Theta = value; }
            public double Rho { get => GetRho(CorP, S, V, D, T, Trials, Steps, U, R, matrix1); set => Rho = value; }
            public double Standard_Deviation { get => GetStandard_deviation(CorP, V, D, T, Steps, U, Option_Price, matrix1, Trials); set => Standard_Deviation = value; }

            public int z = System.Environment.ProcessorCount;//number of processors
            public string A { get; set; }
            public string CV { get; set; }
            public string M { get; set; }
            public double[][,] matrix3 { get; set; }//simulation matrix
            public int[] incrementmatrix { get => createincrementmatrix(); set => incrementmatrix = value; }//matrix required for multithreading
            private int[] createincrementmatrix()
            {
                int increment = Trials / z;
                int[] incrementmatrix1 = new int[z + 1];
                incrementmatrix1[0] = 0;
                int temp = 0;
                for (int x = 1; x < z + 1; x++)
                {
                    temp = increment + temp;
                    incrementmatrix1[x] = temp;
                    Console.WriteLine("i:{0}, value:{1}", x, incrementmatrix1[x]);
                }
                return incrementmatrix1;
            }


            private double GetOptionPrice(String C, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {

                matrix3 = Simulations.CreateSimulatedMatrix(CV, A, Trials, Steps);

                List<Thread> threadlist2 = new List<Thread>(z);
                if (CV == "CV" && A == "A")
                {
                    if (M == "M")
                    {

                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticDeltaVariate(C, S, V, D, T, Trials, Steps, U, matrix1);
                    }


                    double sum_CT = 0;
                    double sum_CT2 = 0;
                    double beta1 = -1;
                    double CT;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            //finding maximum and minimum values for range option
                            double Maximum = matrix3[0][z, 0];
                            double Maximum1 = matrix3[1][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }

                                if (matrix3[1][z, g] > Maximum)
                                {
                                    Maximum1 = matrix3[0][z, g];
                                }


                            }

                            CT = (Math.Max(Maximum - S, 0) + Math.Max(Maximum1 - S, 0) + (beta1 * matrix3[2][z, 0]) + (beta1 * matrix3[2][z, 1])) * 0.5;
                            sum_CT = sum_CT + CT;
                            //m_CT2 = sum_CT2 + (CT * CT);
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        //Console.ReadLine();
                    }

                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double Maximum = matrix3[0][z, 0];
                            double Maximum1 = matrix3[1][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }

                                if (matrix3[1][z, g] > Maximum)
                                {
                                    Maximum1 = matrix3[0][z, g];
                                }


                            }

                            CT = (Math.Max(Maximum - S, 0) + Math.Max(Maximum1 - S, 0) + (beta1 * matrix3[2][z, 0]) + (beta1 * matrix3[2][z, 1])) * 0.5;
                            sum_CT = sum_CT + CT;
                            //m_CT2 = sum_CT2 + (CT * CT);

                        }

                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return (sum_CT / Convert.ToDouble(Trials)) * Math.Exp(-D * T);
                }
                if (CV == "CV")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.deltacontrolvariate(C, V, D, R, Trials, Steps, U, S, matrix1);
                    }
                    double[][,] matrix2 = matrix3;
                    double sum_CT = 0;
                    double sum_CT2 = 0;
                    double beta1 = -1;
                    double CT;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double Maximum = matrix3[0][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }

                            }
                            CT = Math.Max(Maximum - S, 0) + (beta1 * matrix2[1][z, 0]);
                            sum_CT = sum_CT + CT;
                            sum_CT2 = sum_CT2 + (CT * CT);

                            //Console.WriteLine("payoff:,{0}, average:, {1},  CV:, {2}", Math.Max(matrix2[0][z, Steps - 1] - S, 0), CT, matrix2[1][z, 0]);
                            //Console.WriteLine(temp);

                        }
                        Console.ReadLine();
                    }

                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double Maximum = matrix3[0][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }

                            }
                            CT = Math.Max(Maximum - S, 0) + (beta1 * matrix2[1][z, 0]);
                            sum_CT = sum_CT + CT;
                            sum_CT2 = sum_CT2 + (CT * CT);
                        }

                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return (sum_CT / Convert.ToDouble(Trials)) * Math.Exp(-D * T);
                }

                if (A == "A")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double temp = 0;
                    double temp1 = 0;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            //averaging the stockprices for path dependent Asian options
                            double Maximum = matrix3[0][z, 0];
                            double Maximum1 = matrix3[1][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] > Maximum)
                                {
                                    Maximum1 = matrix3[0][z, g];
                                }

                            }   
                            temp = Math.Max((Maximum - S),0) + temp;
                            temp1 = Math.Max((Maximum1 - S),0) + temp1;
                            //Console.WriteLine("AntiThetic..Payoff::, {0}", matrix3[0][z, Steps - 1] );
                        }
                        Console.ReadLine();
                    }
                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {
                            double Maximum = matrix3[0][z, 0];
                            double Maximum1 = matrix3[1][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }
                                if (matrix3[1][z, g] > Maximum)
                                {
                                    Maximum1 = matrix3[0][z, g];
                                }
                            }
                            temp = Math.Max((Maximum - S),0) + temp;
                            temp1 = Math.Max((Maximum1 - S),0) + temp1;
                        }
                    }
                    return 0.5 * ((temp / Trials) + (temp1 / Trials)) * Math.Exp(-R * T);
                }
                else
                {
                    if (M == "M")
                    {
                        //matrix3 = Simulations.GenerateSimulations2(V, D, T, Trials, Steps, U, matrix1);
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.GenerateSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[,] matrix2 = matrix3[0];
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int z = 0; z < Trials; z++)
                        {

                            double Maximum = matrix3[0][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }

                            }
                            temp =Math.Max(( Maximum - S),0) + temp;
                            //Console.WriteLine(temp);
                        }
                    }
                    else
                    {
                        for (int z = 0; z < Trials; z++)
                        {

                            double Maximum = matrix3[0][z, 0];
                            for (int g = 0; g < Steps; g++)
                            {
                                if (matrix3[0][z, g] > Maximum)
                                {
                                    Maximum = matrix3[0][z, g];
                                }

                            }
                            temp = Math.Max((Maximum-S),0 )+ temp;
                        }
                    }
                    //Console.WriteLine("temp{0} -R{1} T{2}", temp, -R, T);
                    //Console.ReadLine();

                    return temp / Trials * Math.Exp(-R * T);
                }
            }
            private double GetDelta(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaS = U * 0.001;

                return (GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U + deltaS, R, matrix1) - GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U - deltaS, R, matrix1)) / (2 * deltaS);
            }
            private double GetGamma(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaS = U * 0.001;
                return (GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U + deltaS, R, matrix1) - 2 * GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1) + GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U - deltaS, R, matrix1)) / (deltaS * deltaS);
            }
            private double GetVega(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaV = V * 0.001;
                return (GetOptionPrice(CorP, S, V + deltaV, D, T, Trials, Steps, U, R, matrix1) - GetOptionPrice(CorP, S, V - deltaV, D, T, Trials, Steps, U, R, matrix1)) / (2 * deltaV);
            }
            private double GetTheta(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaT = T * 0.001;
                return -(GetOptionPrice(CorP, S, V, D, T + deltaT, Trials, Steps, U, R, matrix1) - GetOptionPrice(CorP, S, V, D, T, Trials, Steps, U, R, matrix1)) / (deltaT);
            }
            private double GetRho(string CorP, double S, double V, double D, double T, int Trials, int Steps, double U, double R, double[,] matrix1)
            {
                double deltaR = R * 0.001;
                return (GetOptionPrice(CorP, S, V, D + deltaR, T, Trials, Steps, U, R + deltaR, matrix1) - GetOptionPrice(CorP, S, V, D - deltaR, T, Trials, Steps, U, R - deltaR, matrix1)) / (deltaR * 2);
            }

            private double GetStandard_deviation(String CorP, double V, double D, double T, int Steps, double U, double option, double[,] matrix2, int trials)
            {
                List<Thread> threadlist2 = new List<Thread>(z);
                //Standard deviation for anithetic + delta variate
                if (CV == "CV" && A == "A")
                {
                    if (M == "M")
                    {

                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticDeltaVariate(CorP, S, V, D, T, Trials, Steps, U, matrix1);
                    }

                    double[] intrinsic = new double[trials];
                    double[] intrinsic1 = new double[trials];
                    double temp = 0;
                    double beta1 = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double Sumsp1 = 0;
                            double averagesp = 0;
                            double averagesp1 = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][i, g];
                                Sumsp1 += matrix3[1][i, g];
                            }
                            averagesp = Sumsp / Steps;
                            averagesp1 = Sumsp1 / Steps;
                            intrinsic[i] = (Math.Max(averagesp - S, 0) + Math.Max(averagesp1 - S, 0) + (beta1 * matrix3[2][i, 0]) + (beta1 * matrix3[2][i, 1])) * 0.5;
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double Sumsp1 = 0;
                            double averagesp = 0;
                            double averagesp1 = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][z, g];
                                Sumsp1 += matrix3[1][z, g];
                            }
                            averagesp = Sumsp / Steps;
                            averagesp1 = Sumsp1 / Steps;
                            intrinsic[i] = (Math.Max(S - averagesp, 0) + Math.Max(S - averagesp1, 0) + (beta1 * matrix3[2][i, 0]) + (beta1 * matrix3[2][i, 1])) * 0.5;
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));
                    //Console.ReadLine();
                    return x;
                }
                if (CV == "CV")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.deltacontrolvariate(CorP, V, D, R, Trials, Steps, U, S, matrix1);
                    }
                    double[] intrinsic = new double[trials];
                    double temp = 0;
                    double beta1 = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double averagesp = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][i, g];

                            }
                            averagesp = Sumsp / Steps;
                            intrinsic[i] = Math.Max(averagesp - S, 0) + (beta1 * matrix3[1][i, 0]);
                            temp += Math.Pow((intrinsic[i] - option), 2);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double averagesp = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][i, g];

                            }
                            averagesp = Sumsp / Steps;
                            intrinsic[i] = Math.Max(averagesp - S, 0) + (beta1 * matrix3[1][i, 0]);
                            temp += Math.Pow((intrinsic[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));

                    return x;
                }
                if (A == "A")
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.AntitheticSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[] intrinsic = new double[trials];
                    double[] intrinsic1 = new double[trials];
                    double beta1 = -1;
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double Sumsp1 = 0;
                            double averagesp = 0;
                            double averagesp1 = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][z, g];
                                Sumsp1 += matrix3[1][z, g];
                            }
                            averagesp = Sumsp / Steps;
                            averagesp1 = Sumsp1 / Steps;
                            averagesp = Math.Max(averagesp - S, 0);
                            averagesp1 = Math.Max(averagesp1 - S, 0);
                            intrinsic[i] = averagesp;
                            intrinsic1[i] = averagesp1;
                            temp += Math.Pow((intrinsic[i] - option), 2) + Math.Pow((intrinsic1[i] - option), 2);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double Sumsp1 = 0;
                            double averagesp = 0;
                            double averagesp1 = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix3[0][z, g];
                                Sumsp1 += matrix3[1][z, g];
                            }
                            averagesp = Sumsp / Steps;
                            averagesp1 = Sumsp1 / Steps;
                            averagesp = Math.Max(S - averagesp, 0);
                            averagesp1 = Math.Max(S - averagesp1, 0);

                            intrinsic[i] = averagesp;
                            intrinsic1[i] = averagesp1;
                            temp += Math.Pow((intrinsic[i] - option), 2) + Math.Pow((intrinsic1[i] - option), 2);

                        }
                    }

                    double x = ((Math.Sqrt(temp / (2 * trials - 1))) / Math.Sqrt(2 * trials));

                    return x;
                }
                else
                {
                    if (M == "M")
                    {
                        for (int i = 0; i < z; i++)
                        {
                            Thread t1 = Program.Createthread1(matrix3, A, CV, CorP, V, D, T, Trials, Steps, U, S, matrix1, incrementmatrix[i], incrementmatrix[i + 1]);
                            threadlist2.Add(t1);
                            t1.Start();
                        }
                        for (int i = 0; i < z; i++)
                        {
                            threadlist2[i].Join();
                        }
                    }
                    else
                    {
                        matrix3 = Simulations.GenerateSimulations(V, D, T, Trials, Steps, U, matrix1);
                    }
                    double[,] matrix = matrix3[0];
                    double[] intrinsic = new double[trials];
                    double temp = 0;
                    if (CorP == "C")
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double averagesp = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix2[z, g];

                            }
                            averagesp = Sumsp / Steps;
                            averagesp = Math.Max(averagesp - S, 0);
                            intrinsic[i] = averagesp;
                            temp += Math.Pow((intrinsic[i] - option), 2);
                            //Console.WriteLine("Yes");


                        }
                    }
                    else
                    {
                        for (int i = 0; i < trials; i++)
                        {
                            double Sumsp = 0;
                            double averagesp = 0;
                            for (int g = 0; g < Steps; g++)
                            {
                                Sumsp += matrix2[z, g];

                            }
                            averagesp = Sumsp / Steps;
                            averagesp = Math.Max(S - averagesp, 0);
                            intrinsic[i] = averagesp;
                            temp += Math.Pow((intrinsic[i] - option), 2);


                        }
                    }

                    double x = ((Math.Sqrt(temp / (trials - 1))) / Math.Sqrt(trials));

                    return x;
                }
            }
        }//calculate standard deviation
        public static double GetInterestRate(double T)
        {
            double R;
            InterestRate1[] interest;
            Model1Container db = new Model1Container();
            interest = db.InterestRate1.ToArray();
            double UT = 0, UR = 0, LR = 0, LT = 0;
            for (int i = 0; i < interest.Length; i++)
            {
                Console.WriteLine("Tenor: {1}, Rate: {0}", interest[i].Rate, interest[i].Tenor);
                if (interest[i].Tenor > T)
                {
                    UT = interest[i].Tenor;
                    UR = interest[i].Rate;
                }
                else
                {
                    LT = interest[i].Tenor;
                    LR = interest[i].Rate;
                    break;
                }
            }
            R = ((UR - LR) / (UT - LT) * (T - LT)) + LR;
            return R;

        }



    }





}
    


    



 