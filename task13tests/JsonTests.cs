using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using task13;

namespace task13tests
{
    public class JsonTests
    {
        [Fact]
        public void SerializeAndDeserialize_ShouldWorkCorrectly()
        {
            var student =new Student
            {
                FirstName = "Иван",
                LastName = "Иванов",
                BirthDate = new DateTime(2000, 1, 1),
                Grades = new List<Subject> { new Subject { Name = "Математика", Grade = 5 } }
            };

            string json = JsonManager.Serialize(student);
            var deserialized = JsonManager.Deserialize(json);

            Assert.Equal(student.FirstName, deserialized.FirstName);
            Assert.Equal(student.BirthDate, deserialized.BirthDate);
        }

        [Fact]
        public void FileOperations_ShouldSaveAndLoad()
        {
            string path="test_student.json";
            var student =new Student { FirstName = "Анна", LastName = "Смирнова", BirthDate = new DateTime(2002, 5, 10) };

            JsonManager.SaveToFile(student, path);
            var loaded = JsonManager.LoadFromFile(path);

            Assert.Equal("Анна", loaded.FirstName);
            if (File.Exists(path)) File.Delete(path);
        }
    }
}