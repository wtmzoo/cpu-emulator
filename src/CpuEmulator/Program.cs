using CpuEmulator.Entities;
using CpuEmulator.Utilities;

namespace CpuEmulator;

public static class Program
{
    public static void Main()
    {
        var dataMemory = new Memory([2, 1, 2, 2, 5, 5]);
        
        var instructionMemory = new Memory(
            [
                0x00 << 24 | 0x03 << 16 | 0x00 << 8 | 0x0, // LOAD R3, 0, 0 (в регистр R3 размер массива)
                
                0x04 << 24 | 0x02 << 16 | 0x00 << 8 | 0x00, // INCREMENT R2 (инкремент регистра значения индекса)
                0x00 << 24 | 0x01 << 16 | 0x02 << 8 | 0x00, // LOAD R1, R2 (в регистр текущего значения значение из памяти по регистру индекса)
                0x02 << 24 | 0x00 << 16 | 0x00 << 8 | 0x01, // ADD R0, R0, R1 (сумма регистра суммы и регистра текущего значения в регистр суммы)
                
                0x03 << 24 | 0x04 << 16 | 0x02 << 8 | 0x03, // SUB R4, R2, R3 (разница текущего индекса и размера массива в регистр R4)
                0x06 << 24 | 0x02 << 16 | 0x04 << 8 | 0x00, // JUMP TO 2, R4 (переход на команду, если значение в R4 < 0)
                0x08 << 24 | 0x00 << 16 | 0x00 << 8 | 0x00 // HALT
            ]);
        
        var cpu = new Cpu(dataMemory, instructionMemory);
        cpu.Execute();
        
        // Результат исполнения находится в регистре R0
        Console.WriteLine($"\nPW1. Sum of array elements: {cpu.GetRegister(0).Read()}\n");
        
        var convertCommands = AssemblerConverter.ConvertCommands(
            [
                "LOAD R3, 0, 0",
                "INCR R2, 0, 0",
                "LOAD R1, R2, 0",
                "ADD R0, R0, R1",
                "SUB R4, R2, R3",
                "JMP 2, R4, 0",
                "HLT"
            ]);
        
        var secondCpu = new Cpu(dataMemory, instructionMemory);
        secondCpu.LoadInstructions(new Memory(convertCommands));
        secondCpu.Execute();
        
        Console.WriteLine($"\nPW2. Sum of array elements: {cpu.GetRegister(0).Read()}\n");
    }
}