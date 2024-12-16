using CpuEmulator.Entities;

namespace CpuEmulator;

public static class Program
{
    public static void Main(string[] args)
    {
        // Исходные данные: массив для суммирования
        var dataMemory = new Memory([1, 2, 3, 4, 5]);

        // Программа для CPU (в инструкционной памяти)
        // Инструкция: (byte3 << 16) | (byte2 << 8) | byte1
        var instructionMemory = new Memory(
            [
                (0 << 16) | (1 << 8) | 0, // LOAD R1 <- memory[R0]
                (2 << 16) | (2 << 8) | 1, // SUB R2 = R2 - R1
                (3 << 16) | (0 << 8) | 0, // INCREMENT R0
                (4 << 16) | (0 << 8) | 9, // JUMP TO 0 if R9 < 0
                (6 << 16) | (0 << 8) | 0  // ABORT
            ]);

        // Создаем CPU и выполняем программу
        var cpu = new Cpu(8, dataMemory, instructionMemory);
        cpu.Execute();

        // Результат находится в R2
        Console.WriteLine($"Сумма элементов массива: {cpu.GetRegisterValue(2)}");
    }
    
    private static int Encode(int command, int arg0 = 0, int arg1 = 0)
    {
        return (command << 16) | (arg0 << 8) | arg1;
    }
}