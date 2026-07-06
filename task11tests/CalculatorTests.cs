using Xunit;
using task11;

namespace task11tests
{
    public class CalculatorTests
    {
        private const string CalculatorSourceCode = @"
        using task11;

        namespace task11
        {
            public class RuntimeCalculator : ICalculator
            {
                public double Add(double a, double b) => a + b;
                public double Minus(double a, double b) => a - b;
                public double Mul(double a, double b) => a * b;
                public double Div(double a, double b) => a / b;
            }
        }";

        [Fact]
        public void DynamicCalculator_ShouldPerformArithmeticOperationsWithoutReflection()
        {
            ICalculator calculator = RuntimeCompiler.CreateCalculator(CalculatorSourceCode);

            Assert.NotNull(calculator);
            Assert.Equal(10, calculator.Add(7, 3));
            Assert.Equal(4, calculator.Minus(7, 3));
            Assert.Equal(21, calculator.Mul(7, 3));
            Assert.Equal(2, calculator.Div(6, 3));
        }
    }
}