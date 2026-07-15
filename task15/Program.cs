using System;
using System.Diagnostics;
using System.IO;
using task15;

namespace task15
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<double, double> SIN = Math.Sin;
            double a = -100;
            double b = 100;
            double targetAccuracy = 1e-4;

            Console.WriteLine("1. Поиск минимального шага...");
            double[] stepsToTest = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
            double optimalStep = 1e-1;
            foreach (var step in stepsToTest)
            {
                double result = DefiniteIntegral.SolveSingleThread(a, b, SIN, step);
                if (Math.Abs(result -0) <= targetAccuracy)
                {
                    optimalStep = step;
                    Console.WriteLine($"Оптимальный шаг найден: {optimalStep}");
                    break;
                }
            }

            Console.WriteLine("\n2. Тестирование потоков...");
            int[] threadsList = { 1, 2, 4, 6, 8, 12, 16 };
            double[] times = new double[threadsList.Length];
            double[] threadsArray = new double[threadsList.Length];
            double minMultiTime = double.MaxValue;
            int optimalThreads = 1;

            for (int i = 0; i < threadsList.Length; i++)
            {
                long totalTime = 0;
                for (int j = 0; j < 5; j++)
                {
                    var sw = Stopwatch.StartNew();
                    DefiniteIntegral.Solve(a, b, SIN, optimalStep, threadsList[i]);
                    sw.Stop();
                    totalTime += sw.ElapsedMilliseconds;
                }

                double avgTime = totalTime / 5.0;
                times[i] = avgTime;
                threadsArray[i] = threadsList[i];

                if (avgTime < minMultiTime)
                {
                    minMultiTime = avgTime;
                    optimalThreads = threadsList[i];
                }
            }

            long singleTotal =0;
            for (int i = 0; i < 5; i++)
            {
                var sw = Stopwatch.StartNew();
                DefiniteIntegral.SolveSingleThread(a, b, SIN, optimalStep);
                sw.Stop();
                singleTotal += sw.ElapsedMilliseconds;
            }
            double singleAvg = singleTotal / 5.0;
            double diffPercent = ((singleAvg - minMultiTime) / singleAvg) * 100;

            var plt = new ScottPlot.Plot();
            plt.Add.Scatter(threadsArray, times);
            plt.Title("Зависимость времени от числа потоков");
            plt.XLabel("Количество потоков");
            plt.YLabel("Время (мс)");
            plt.SavePng("plot.png", 600, 400);

            string report = $"Оптимальный шаг: {optimalStep}\nОптимальные потоки: {optimalThreads}\nВремя однопоток: {singleAvg}мс\nВремя многопоток: {minMultiTime}мс\nУскорение: {diffPercent:F2}%";
            File.WriteAllText("Report.txt", report);
            Console.WriteLine("\nГотово! plot.png и Report.txt созданы.");
        }
    }
}