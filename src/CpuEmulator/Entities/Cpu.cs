namespace CpuEmulator.Entities;

public class Cpu
{
    private int _programCounter;
    
    private Memory _instructionMemory;
    private readonly Memory _dataMemory;
    
    private readonly Register[] _registers;
    
    public Cpu(Memory dataMemory, Memory instructionMemory)
    {
        // 0 - сумма, 1 - текущее значение, 2 - индекс,
        // 3 - размер массива, 4 - разница размера массива и текущего индекса
        _registers = new Register[5];
        for (var i = 0; i < _registers.Length; i++)
            _registers[i] = new Register();
        
        _programCounter = 0;
        _dataMemory = dataMemory;
        _instructionMemory = instructionMemory;
    }
    
    public Register GetRegister(int registerIndex) => _registers[registerIndex];
    public void LoadInstructions(Memory instructionMemory) => _instructionMemory = instructionMemory;

    public void Execute()
    {
        while (_programCounter != -1)
        {
            var instruction = _instructionMemory.Read(_programCounter++);
            
            // opCode, dstRegister, srcRegister1, srcRegister2
            int[] decodedInstructions = 
                [
                    (instruction >> 24) & 0xFF,
                    (instruction >> 16) & 0xFF,
                    (instruction >> 8) & 0xFF,
                    instruction & 0xFF
                ];
            
            switch (decodedInstructions[0])
            {
                case 0x00: // LOAD reg[dst] <- memory[reg[src].read]
                    _registers[decodedInstructions[1]].Write(
                        _dataMemory.Read(_registers[decodedInstructions[2]].Read()));
                    break;
                case 0x01: // MOV reg[dst] = reg[src]
                    _registers[decodedInstructions[1]].Write(
                        _registers[decodedInstructions[2]].Read());
                    break;
                case 0x02: // ADD reg[dst] = reg[src1] + reg[src2]
                    _registers[decodedInstructions[1]].Write(
                        _registers[decodedInstructions[2]].Read() + _registers[decodedInstructions[3]].Read());
                    break;
                case 0x03: // SUB reg[dst] = reg[src1] - reg[src2]
                    _registers[decodedInstructions[1]].Write(
                        _registers[decodedInstructions[2]].Read() - _registers[decodedInstructions[3]].Read());
                    break;
                case 0x04: // INCREMENT reg[dst]
                    _registers[decodedInstructions[1]].Write(_registers[decodedInstructions[1]].Read() + 1);
                    break;
                case 0x05: // ABORT
                    _programCounter = -2;
                    break;
                case 0x06: // JUMP TO dst if reg[src] < 0
                    if (_registers[decodedInstructions[2]].Read() < 0)
                        _programCounter = decodedInstructions[1] - 1;
                    break;
                case 0x07: // COPY reg[dst] = reg[src]
                    _registers[decodedInstructions[1]].Write(_registers[decodedInstructions[2]].Read());
                    break;
                case 0x08: // HALT
                    _programCounter = -1;
                    break;
                case 0x09: // JUMP TO dst
                    _programCounter = decodedInstructions[1] - 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(decodedInstructions), 
                        decodedInstructions[0], "Unknown opCode");
            }
            
            WriteState(decodedInstructions);
        }
    }
    
    private void WriteState(int[] instructions)
    {
        Console.WriteLine($"<pCounter: {_programCounter}>");
        
        Console.WriteLine($"<opCode: {instructions[0]}, " +
                          $"dst: {instructions[1]}, " +
                          $"src1: {instructions[2]}, " +
                          $"src2: {instructions[3]}>");
        
        Console.WriteLine($"<r0: {_registers[0].Read()}, " +
                          $"r1: {_registers[1].Read()}, " +
                          $"r2: {_registers[2].Read()}, " +
                          $"r3: {_registers[3].Read()}, " +
                          $"r4: {_registers[4].Read()}>");

        var memoryLog = string.Empty;
        var memoryData = _dataMemory.Get();
        for (var i = 0; i < memoryData.Length; i++)
        {
            if (memoryData.Length - 1 == i)
            {
                memoryLog += $"m{i}: {memoryData[i]}";
            }
            else
            {
                memoryLog += $"m{i}: {memoryData[i]}, ";
            }
        }
            
        Console.WriteLine($"<{memoryLog}>");
        
        Console.WriteLine("-----------------");
    }
}