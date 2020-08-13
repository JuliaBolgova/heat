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

            System.IO.StreamWriter FileTx = new StreamWriter("T(x)" + ".txt");
            string head = "x[i]" + "             " + "T_new[i]";
            FileTx.WriteLine(head);
            //System.IO.StreamWriter FileTimeEps = new StreamWriter("time(eps)" + ".txt");
            //string head1 = "time" + "     " + "eps";
            //FileTimeEps.WriteLine(head1);
            System.IO.StreamWriter FileTxEnd = new StreamWriter("T(x)End" + ".txt");
            string head2 = "X[i]" + "     " + "T(x)";
            FileTxEnd.WriteLine(head2);

            double r = 0.3; // число Куранта
            double time = 0; // старт времени

            int N = 6; // Количество разбиений
            int K = 3; // Место соединения двух материалов

            double l1 = 0.01; // Длина 1 материала
            double l2 = 0.01; // Длина 2 материала
            double L = l1 + l2; // Общая длина материалов
            double h = L / N; // Шаг по пространству
            double Q; // Тепловой поток

            double ro1 = 8933; // Плотность 1 материала
            double ro2 = 8933; // Плотность 2 материала
            double lyamb1 = 401; // Теплопроводность 1 материала
            double lyamb2 = 401; // Теплопроволность 2 материала
            double C1 = 259; // Теплоемкость 1 материала
            double C2 = 259; // Теплоемкость 2 материала
            double a1 = lyamb1 / (C1 * ro1); // Коэффициент температуропроводности 1 материала
            double a2 = lyamb2 / (C2 * ro2); // Коэффициент температуропроводности 2 материала
            double tau1 = r * (h * h) / a1;
            double tau2 = r * (h * h) / a2;
            double tau = Math.Min(tau1, tau2); // Шаг по времени
            double alphaAir = 5; // Коэффициент теплоотдачи

            double Tl1 = 100; // Начальная температура 1 материала
            double Tl2 = 25; // Начальная температура 2 материала
            double T_air = 25;

            double[] TOld = new double[N + 1]; // Массив данных старого слоя температуры
            double[] TNew = new double[N + 1]; // Массив данных нового слоя температуры
            double[] X = new double[N + 1]; // Данные по оси Х

            for (int i = 0; i <= N; i++)
            {
                X[i] = i * h; //  Разбиение оси Х на шаги
            }

            for (int i = 0; i <= N; i++)
            {
                if (i <= K)
                    TOld[i] = Tl1; // Распределение начальной температуры на 1 материале
                if (i > K)
                    TOld[i] = Tl2; // Распределение начальной температуры на 2 материале
            }

            FileTx.WriteLine("   ");
            FileTx.WriteLine("   " + "Пройденное время: " + time + "   ");


            for (int i = 0; i <= N; i++)
            {
                string X_T = Math.Round(X[i], 4).ToString().Replace(",", ".") + "\t" + "           " + TOld[i].ToString().Replace(",", ".") + "\t";
                FileTx.WriteLine(X_T);
            }

            while (time <= 100)
            {
                if (TOld[0] >= 97 && TOld[0] <= 103)
                {
                    Q = 0;
                    time += tau; // увеличиваем время на tau
                    FileTx.WriteLine("   ");
                    FileTx.WriteLine("   " + "Пройденное время: " + time + "   ");

                    TNew[0] = TOld[1] + Q * h / lyamb1; // 2 ГУ ( задание теплового потока Q )

                    for (int i = 1; i < K; i++)
                    {
                        TNew[i] = TOld[i] + a1 * tau / (h * h) * (TOld[i + 1] - 2 * TOld[i] + TOld[i - 1]); // Температура на 1 материале
                    }

                    TNew[K] = (lyamb2 * TOld[K + 1] + lyamb1 * TOld[K - 1]) / (lyamb2 + lyamb1); // Температура на соединении


                    for (int i = K + 1; i < N; i++)
                    {
                        TNew[i] = TOld[i] + a2 * tau / (h * h) * (TOld[i + 1] - 2 * TOld[i] + TOld[i - 1]); // Температура на 2 материале
                    }

                    TNew[N] = TNew[N - 1] - (alphaAir * h / lyamb2) * T_air; // 3 граничное условие (остывание 1 материала)

                }

                if (TOld[0] < 97)
                {
                    while (TOld[0] < 103 && time <= 100)
                    {
                        Q = 25000;
                        time += tau; // увеличиваем время на tau
                        FileTx.WriteLine("   ");
                        FileTx.WriteLine("   " + "Пройденное время: " + time + "   ");



                        TNew[0] = TOld[0] + Q * h / lyamb1; // 2 ГУ ( задание теплового потока Q )

                        for (int i = 1; i < K; i++)
                        {
                            TNew[i] = TOld[i] + a1 * tau / (h * h) * (TOld[i + 1] - 2 * TOld[i] + TOld[i - 1]); // Температура на 1 материале
                        }

                        TNew[K] = (lyamb2 * TOld[K + 1] + lyamb1 * TOld[K - 1]) / (lyamb2 + lyamb1); // Температура на соединении


                        for (int i = K + 1; i < N; i++)
                        {
                            TNew[i] = TOld[i] + a2 * tau / (h * h) * (TOld[i + 1] - 2 * TOld[i] + TOld[i - 1]); // Температура на 2 материале
                        }

                        TNew[N] = TNew[N - 1] - (alphaAir * h / lyamb2) * T_air; // 3 граничное условие (остывание 1 материала)

                        for (int i = 0; i <= N; i++)
                        {
                            TOld[i] = TNew[i];
                            string X_T = Math.Round(X[i], 4).ToString().Replace(",", ".") + "\t" + "           " + TOld[i].ToString().Replace(",", ".") + "\t";
                            FileTx.WriteLine(X_T);
                        }
                    }
                }
                if (TOld[0] >= 103)
                {
                    Q = 0;
                    time += tau; // увеличиваем время на tau
                    FileTx.WriteLine("   ");
                    FileTx.WriteLine("   " + "Пройденное время: " + time + "   ");

                    TNew[0] = TOld[1] + Q * h / lyamb1; // 2 ГУ ( задание теплового потока Q )

                    for (int i = 1; i < K; i++)
                    {
                        TNew[i] = TOld[i] + a1 * tau / (h * h) * (TOld[i + 1] - 2 * TOld[i] + TOld[i - 1]); // Температура на 1 материале
                    }

                    TNew[K] = (lyamb2 * TOld[K + 1] + lyamb1 * TOld[K - 1]) / (lyamb2 + lyamb1); // Температура на соединении


                    for (int i = K + 1; i < N; i++)
                    {
                        TNew[i] = TOld[i] + a2 * tau / (h * h) * (TOld[i + 1] - 2 * TOld[i] + TOld[i - 1]); // Температура на 2 материале
                    }

                    TNew[N] = TNew[N - 1] - (alphaAir * h / lyamb2) * T_air; // 3 граничное условие (остывание 1 материала)
                }

                for (int i = 0; i <= N; i++)
                {
                    TOld[i] = TNew[i];
                    string X_T = Math.Round(X[i], 4).ToString().Replace(",", ".") + "\t" + "           " + TOld[i].ToString().Replace(",", ".") + "\t";
                    FileTx.WriteLine(X_T);
                }

            }
            for (int i = 0; i <= N; i++)
            {
                string X_TEnd = X[i].ToString().Replace(",", ".") + "\t" + "    " + TNew[i].ToString().Replace(",", ".") + "\t";
                FileTxEnd.WriteLine(X_TEnd);
                Console.WriteLine(time + "    " + X[i].ToString().Replace(",", ".") + "\t" + "    " + TNew[i].ToString().Replace(",", ".") + "\t");
            }

            FileTx.Close();
            FileTxEnd.Close();
            //FileTimeEps.Close();
            Console.ReadKey();
        }
    }
}
