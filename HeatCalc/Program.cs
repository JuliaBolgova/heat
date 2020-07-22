using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeatCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            double eps = 0.1;

            double N = 100;
            double N1 = 50;

            double l1 = 1;
            double l2 = 1;
            double L = L
            double ro1 = 7800;
            double ro2 = 7800;
            double lyambda_1 = 38.5;
            double lyambda_2 = 38.5;
            double C_1 = 380;
            double C_2 = 380;
            
            K = N1;
            L = l1 + l2;
            h = L / N;
            H = 5;
            T_left = 100;
            T_right = 25;
            T_air = 25;

            double[] T_old = new double[N + 1];
            double[] T_new = new double[N + 1];
            double[] X = new double[N + 1];

            for (int i = 0; i <= N; i++)
            {
                X[i] = i * h;
            }

            T_old[0] = T_left;
            for (int i = 1; i <= N; i++)
            {
                if (i <= K)
                    T_old[i] = T_left;
                if (i > K)
                    T_old[i] = T_right;
            }

            r = 0.2;
            tau = 1;// h * h * r;

            time = 0;
            dT = 1;
            while (dT > eps)
            {
                time = time + tau;
                T_new[0] = T_old[0];
                for (int i = 1; i < K; i++)
                {
                    T_new[i] = T_old[i] + lyambda_1 / (C_1 * Ro_1) * tau * (T_old[i - 1] - 2 * T_old[i]
                        + T_old[i + 1]) / (h * h);
                }

                T_new[K] = (lyambda_2 * T_old[K + 1] + lyambda_1 * T_old[K - 1]) / (lyambda_2 + lyambda_1);

                for (int i = K + 1; i < N; i++)
                {
                    T_new[i] = T_old[i] + lyambda_2 / (C_2 * Ro_2) * tau *
                        (T_old[i - 1] - 2 * T_old[i] + T_old[i + 1]) / (h * h);
                }
                T_new[N] = T_new[N - 1];
                //H * h * (T_old[N] - T_air) / lyambda_2 + T_old[N - 1];//коэфф теплоотдачи

                dT = Math.Abs(T_old[N1 + 1] - T_new[N1 + 1]);

                T_old_Sum = T_old.Sum();
                T_new_Sum = T_new.Sum();
                //dT = Math.Abs(T_old_Sum - T_new_Sum);

                for (int i = 0; i <= N; i++)
                {
                    T_old[i] = T_new[i];

                    string X_T = X[i].ToString().Replace(",", ".") + "\t" + "    " + T_new[i].ToString().Replace(",", ".") + "\t";
                    File1.WriteLine(X_T);
                }
                string Time_eps = time.ToString().Replace(",", ".") + "\t" + "    " + dT.ToString().Replace(",", ".") + "\t";
                File2.WriteLine(Time_eps);
            }
            File1.Close();
            File2.Close();
            var Gran = new List<T_x>();
            for (int i = 0; i <= N; i++)
            {
                Gran.Add(new T_x(Math.Round(X[i], 4), Math.Round(T_new[i], 4)));
            }
            this.tablegrid_xy.ItemsSource = Gran;
        }
    }
    }
}
