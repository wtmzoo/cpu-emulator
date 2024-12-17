using CpuEmulator.Entities;
using CpuEmulator.Utilities;

namespace CpuEmulator;

public static class Program
{
    public static void Main()
    {
        var dataMemory = new Memory([5, 1, 4, 2, 5, 5]);
        
        var instructionMemory = new Memory(
            [
                0x00 << 16 | 0x03 << 8 | 0x00, // LOAD R3, 0 (в регистр R3 размер массива)
                
                0x04 << 16 | 0x02 << 8 | 0x00, // INCREMENT R2 (инкремент регистра индекса)
                0x00 << 16 | 0x01 << 8 | 0x02, // LOAD R1, M[R2] (в регистр текущего значения значение из памяти по регистру индекса)
                0x02 << 16 | 0x00 << 8 | 0x01, // ADD R0, R1 (сумма регистра суммы и регистра текущего значения)
                
                0x07 << 16 | 0x04 << 8 | 0x02, // COPY R4, R2 (текущий индекс в регистр разницы размера массива и текущего индекса)
                0x03 << 16 | 0x04 << 8 | 0x03, // SUB R4, R3 (разница текущего индекса и размера массива в соответствующем регистре)
                0x06 << 16 | 0x02 << 8 | 0x04, // JUMP TO 2, R4 (переход на команду, если значение в R4 < 0)
                0x08 << 16 | 0x00 << 8 | 0x00  // HALT
            ]);
        
        var cpu = new Cpu(dataMemory, instructionMemory);
        cpu.Execute();
        
        // Результат исполнения находится в регистре R0
        Console.WriteLine($"ПР1. Сумма элементов массива: {cpu.GetRegister(0).Read()}");
        
        var convertCommands = AssemblerConverter.ConvertCommands(
            [
                "LOAD R3, 0",
                "INCR R2",
                "LOAD R1, R2",
                "ADD R0, R1",
                "COPY R4, R2",
                "SUB R4, R3",
                "JMP 2, R4",
                "HLT"
            ]);
        
        var secondCpu = new Cpu(dataMemory, instructionMemory);
        secondCpu.LoadInstructions(new Memory(convertCommands));
        secondCpu.Execute();
        
        Console.WriteLine($"ПР2. Сумма элементов массива: {cpu.GetRegister(0).Read()}");
    }
}