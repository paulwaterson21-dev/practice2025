using System;

namespace task04
{
    public class Cruiser : ISpaceship
    {
        public int Speed { get; } = 50;
        public int FirePower { get; } = 100;

        public void MoveForward()
        {
            Console.WriteLine("Крейсер движется вперед.");
        }

        public void Rotate(int angle)
        {
            // Логика поворота
            Console.WriteLine($"крейсер поворачивается на {angle} градусов.");
        }

        public void Fire()
        {
            Console.WriteLine($"крейсер стреляет,урон: {FirePower}");
        }
    }
}