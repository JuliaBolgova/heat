using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeatCalc
{
    class Program
    {
        static void Main(string[] args)
        {

            System.IO.StreamWriter File1 = new StreamWriter("T(x)" + ".txt");
            string head = "x[i]" + "     " + "T_new[i]";
            File1.WriteLine(head);
            System.IO.StreamWriter File2 = new StreamWriter("time(eps)" + ".txt");
            string head1 = "time" + "     " + "eps";
            File2.WriteLine(head1);


            double eps = 0.00001; // погрешность
            double r = 0.3; // число Куранта
            double time = 0; // старт времени
            double dT = 1; // разница температуры между старым и новым слоем по времени
                           // (нужно только вначале, чтобы не было по умолчанию 0)

            int N = 20; // Количество разбиений
            int K = 10; // Место соединения двух материалов

            double l1 = 0.5; // Длина 1 материала
            double l2 = 0.5; // Длина 2 материала
            double L = l1 + l2; // Общая длина материалов
            double h = L / N; // Шаг по пространству
            
            

            double ro1 = 7800; // Плотность 1 материала
            double ro2 = 7800; // Плотность 2 материала
            double lyamb1 = 13; // Теплопроводность 1 материала
            double lyamb2 = 13; // Теплопроволность 2 материала
            double C1 = 380; // Теплоемкость 1 материала
            double C2 = 380; // Теплоемкость 2 материала
            double a1 = 1;// lyamb1 / (C1 * ro1); // Коэффициент температуропроводности 1 материала
            double a2 = 1;// lyamb2 / (C2 * ro2); // Коэффициент температуропроводности 2 материала
            double tau1 = r * (h * h) / a1;
            double tau2 = r * (h * h) / a2;
            double tau = Math.Min(tau1, tau2); // Шаг по времени

            double TLeft = 100; // Температура на левой границе 1 материала - для 1 ГУ
            double Tl1 = 100; // Начальная температура 1 материала
            double Tl2 = 25; // Начальная температура 2 материала

            double[] TOld = new double[N + 1]; // Массив данных старого слоя температуры
            double[] TNew = new double[N + 1]; // Массив данных нового слоя температуры
            double[] X = new double[N + 1]; // Данные по оси Х

            for (int i = 0; i <= N; i++)
            {
                X[i] = i * h; //  Разбиение оси Х на шаги
            }

            TOld[0] = TLeft; // Первое граничное условие (постоянство температуры)

            for (int i = 1; i <= N; i++)
            {
                if (i <= K)
                    TOld[i] = Tl1; // Распределение начальной температуры на 1 материале
                if (i > K)
                    TOld[i] = Tl2; // Распределение начальной температуры на 2 материале
            }
            for (int i = 0; i <= N; i++)
            {
               // string X_T = X[i].ToString().Replace(",", ".") + "\t" + "    " + TOld[i].ToString().Replace(",", ".") + "\t";
               // File1.WriteLine(X_T);
                //Console.WriteLine(X[i].ToString().Replace(",", ".") + "\t" + "    " + TOld[i].ToString().Replace(",", ".") + "\t");
            }

            while (dT > eps)
            {
                time += tau; // увеличиваем время на tau
                TNew[0] = TOld[0]; // 1 граничное условие (постоянство температуры слева)

                for (int i = 1; i <= K; i++)
                {
                    TNew[i] = TOld[i] + a1 * tau / (h * h) * (TOld[i - 1] - 2 * TOld[i] + TOld[i + 1]); // Температура на 1 материале
                }

               // TNew[K] = (lyamb2 * TOld[K + 1] + lyamb1 * TOld[K - 1]) / (lyamb2 + lyamb1); // Температура на соединении

                for (int i = K + 1; i < N; i++)
                {
                    TNew[i] = TOld[i] + a2 * tau / (h * h) * (TOld[i - 1] - 2 * TOld[i] + TOld[i + 1]); // Температура на 2 материале
                }

                TNew[N] = TNew[N - 1]; // Условие симметрии на конце материала 2 

                dT = 0;
                for(int i = 0; i <= N; i++)
                {
                    dT = dT + (TNew[i] - TOld[i]) * (TNew[i] - TOld[i]);
                }
                dT = Math.Sqrt(dT) / N;
                    //dT = Math.Abs(TOld[K] - TNew[K]);

                for (int i = 0; i <= N; i++)
                {
                    TOld[i] = TNew[i];

                    //string X_T = X[i].ToString().Replace(",", ".") + "\t" + "    " + TNew[i].ToString().Replace(",", ".") + "\t";
                    //File1.WriteLine(X_T);
                    //Console.WriteLine(X[i].ToString().Replace(",", ".") + "\t" + "    " + TNew[i].ToString().Replace(",", ".") + "\t");

                }
                Console.WriteLine(time.ToString().Replace(",", ".") + "\t" + "    " + dT.ToString().Replace(",", ".") + "\t");
                string Time_eps = time.ToString().Replace(",", ".") + "\t" + "    " + dT.ToString().Replace(",", ".") + "\t";
                File2.WriteLine(Time_eps);
            }
            for (int i = 0; i <= N; i++)
            {
                string X_T = X[i].ToString().Replace(",", ".") + "\t" + "    " + TNew[i].ToString().Replace(",", ".") + "\t";
                File1.WriteLine(X_T);
            }
            File1.Close();
            File2.Close();
            Console.ReadKey();
        }
    }
}
