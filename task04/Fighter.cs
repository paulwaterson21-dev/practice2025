using System;

namespace task04
{
    public class Fighter : ISpaceship
    {
        public int Speed { get; } = 100;
        public int FirePower { get; } = 20;

        public void MoveForward()
        {
            Console.WriteLine("истребитель движется вперед.");
        }

        public void Rotate(int angle)
        {
            Console.WriteLine($"истребитель поворачивается на {angle} градусов.");
        }

        public void Fire()
        {
            Console.WriteLine($"истребитель стреляет,урон: {FirePower}");
        }
    }
}