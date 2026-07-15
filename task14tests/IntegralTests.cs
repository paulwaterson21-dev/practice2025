using System;
using Xunit;
using task14;

namespace task14tests
{
    public class IntegralTests
    {
        [Fact]
        public void Test_Solve_LinearFunction()
        {
            Func<double, double> X = (double x) => x;
            Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2), 4);
        }

        [Fact]
        public void Test_Solve_SinFunction()
        {
            Func<double, double> SIN = (double x) => Math.Sin(x);
            Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8), 4);
        }

        [Fact]
        public void Test_Solve_LinearFunction_PositiveRange()
        {
            Func<double, double> X = (double x) => x;
            Assert.Equal(12.5, DefiniteIntegral.Solve(0, 5, X, 1e-6, 8), 4);
        }
    }
}