using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using task05;

namespace task05tests
{
    public class TestClass
    {
        public int PublicField;
        private string? _privateField;
        public int Property { get; set; }

        public void Method() { }
        public void MethodWithParams(int id, string name) { }
    }
    [Serializable] 
    public class AttributedClass { }

    public class ClassAnalyzerTests
    {
        [Fact]
        public void GetPublicMethods_ReturnsCorrectMethods()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methods = analyzer.GetPublicMethods();

            Assert.Contains("Method", methods);
            Assert.Contains("MethodWithParams", methods);
        }

        [Fact]
        public void GetMethodParams_ReturnsCorrectParameters()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var parameters = analyzer.GetMethodParams("MethodWithParams").ToList();

            Assert.Equal(2, parameters.Count);
            Assert.Contains("Int32 id", parameters);
            Assert.Contains("String name", parameters);
        }

        [Fact]
        public void GetAllFields_IncludesPrivateFields()
        {
            var analyzer =new ClassAnalyzer(typeof(TestClass));
            var fields = analyzer.GetAllFields();

            Assert.Contains("_privateField", fields);
            Assert.Contains("PublicField", fields);
        }

        [Fact]
        public void GetProperties_ReturnsCorrectProperties()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var properties = analyzer.GetProperties();

            Assert.Contains("Property", properties);
        }

        [Fact]
        public void HasAttribute_ReturnsTrue_WhenAttributeExists()
        {
            var analyzer =new ClassAnalyzer(typeof(AttributedClass));
            var hasAttr = analyzer.HasAttribute<SerializableAttribute>();

            Assert.True(hasAttr);
        }

        [Fact]
        public void HasAttribute_ReturnsFalse_WhenAttributeDoesNotExist()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var hasAttr = analyzer.HasAttribute<SerializableAttribute>();

            Assert.False(hasAttr);
        }
    }
}