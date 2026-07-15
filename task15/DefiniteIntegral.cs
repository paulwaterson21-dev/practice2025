using System;
using System.Threading;

namespace task15
{
    public class DefiniteIntegral
    {
        public static double InterlockedAddDouble(ref double location, double value)
        {
            double newCurrentValue =location;
            while (true)
            {
                double currentValue = newCurrentValue;
                double newValue = currentValue + value;
                newCurrentValue = Interlocked.CompareExchange(ref location, newValue, currentValue);
                if (newCurrentValue == currentValue)
                    return newValue;
            }
        }


        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
        {
            double totalResult = 0.0;
            double range = b - a;
            double rangePerThread = range / threadsNumber;

            using (Barrier barrier = new Barrier(threadsNumber + 1))
            {
                for (int i = 0; i < threadsNumber; i++)
                {
                    int threadIndex = i;
                    Thread thread = new Thread(() =>
                    {
                        double localSum = 0.0;
                        double start = a + threadIndex * rangePerThread;
                        double end = (threadIndex == threadsNumber - 1) ? b : start + rangePerThread;

                        int stepsCount = (int)Math.Ceiling((end - start) / step);
                        double localStep = (end - start) / stepsCount;

                        for (int j = 0; j < stepsCount; j++)
                        {
                            double x1 = start + j * localStep;
                            double x2 = start + (j + 1) * localStep;
                            localSum += 0.5 * localStep * (function(x1) + function(x2));
                        }

                        InterlockedAddDouble(ref totalResult, localSum);
                        barrier.SignalAndWait();
                    });
                    thread.Start();
                }
                barrier.SignalAndWait();
            }
            return totalResult;
        }


        public static double SolveSingleThread(double a, double b, Func<double, double> function, double step)
        {
            double sum=0.0;
            int stepsCount = (int)Math.Ceiling((b - a) / step);
            double localStep = (b - a) / stepsCount;

            for (int i = 0; i < stepsCount; i++)
            {
                double x1 =a + i* localStep;
                double x2 = a + (i + 1) * localStep;
                sum += 0.5 * localStep * (function(x1) + function(x2));
            }
            return sum;
        }
    }
}