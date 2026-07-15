//fix
using System;
using System.Collections.Generic;
using System.Linq;

namespace task02
{
    public class StudentService
    {
        private readonly List<Student> _students;

        public StudentService(List<Student> students) => _students= students;

        // Возвращает студентов указанного факультета
        public IEnumerable<Student> GetStudentsByFaculty(string faculty)
            => _students.Where(s => s.Faculty == faculty);

        // Возвращает студентов со средним баллом >= заданного
        public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade)
            => _students.Where(s => s.Grades != null && s.Grades.Count >0 && s.Grades.Average() >= minAverageGrade);

        //Возвращает студентов, отсортированных по имени (А-Я)
        public IEnumerable<Student> GetStudentsOrderedByName()
            => _students.OrderBy(s => s.Name);

        //Группировка по факультету
        public ILookup<string, Student> GroupStudentsByFaculty()
            => _students.ToLookup(s => s.Faculty);

        //Находит факультет с максимальным средним баллом
        public string GetFacultyWithHighestAverageGrade()
        {
            if (_students == null || _students.Count ==0)
                return null;

            return _students
                .GroupBy(s => s.Faculty)
                .Select(g => new
                {
                    FacultyName = g.Key,
                    AverageGrade = g.SelectMany(s => s.Grades).Average()
                })
                .OrderByDescending(f => f.AverageGrade)
                .Select(f => f.FacultyName)
                .FirstOrDefault();
        }
    }
}