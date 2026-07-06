using System;
using System.IO;
using System.Reflection;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace task11
{
    public class RuntimeCompiler
    {

        public static ICalculator CreateCalculator(string sourceCode)
        {
            return CompileCalculatorSource(sourceCode);
        }


        public static ICalculator CompileCalculatorSource(string sourceCode)
        {
            if (string.IsNullOrEmpty(sourceCode))
                throw new ArgumentNullException(nameof(sourceCode));

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            string assemblyName = Path.GetRandomFileName();

            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location)
            };

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);
                    string errors = string.Join(Environment.NewLine, failures.Select(f => $"{f.Id}: {f.GetMessage()}"));
                    throw new InvalidOperationException($"Ошибка компиляции кода калькулятора:{Environment.NewLine}{errors}");
                }

                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());

                Type type = assembly.GetType("task11.Calculator");
                if (type == null)
                    throw new InvalidOperationException("В скомпилированном коде не найден класс task11.Calculator!");

                object instance = Activator.CreateInstance(type);
                return instance as ICalculator;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string calculatorSource = @"
            using System;

            namespace task11
            {
                public class Calculator : ICalculator
                {
                    public double Add(double a, double b) => a + b;
                    public double Minus(double a, double b) => a - b;
                    public double Mul(double a, double b) => a * b;
                    public double Div(double a, double b) 
                    {
                        if (b == 0) throw new DivideByZeroException(""Деление на ноль невозможно."");
                        return a / b;
                    }
                }
            }";

            try
            {

                ICalculator calc = RuntimeCompiler.CreateCalculator(calculatorSource);

                Console.WriteLine("Калькулятор успешно скомпилирован в рантайме!");
                Console.WriteLine($"Тест Add(5, 3): {calc.Add(5, 3)}");
                Console.WriteLine($"Тест Minus(5, 3): {calc.Minus(5, 3)}");
                Console.WriteLine($"Тест Mul(5, 3): {calc.Mul(5, 3)}");
                Console.WriteLine($"Тест Div(6, 2): {calc.Div(6, 2)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}