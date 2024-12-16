namespace CpuEmulator.Entities;

public class Cpu
{
    private int _programCounter;
    
    private Memory _instructionMemory;
    private readonly Memory _dataMemory;
    
    private readonly Register[] _registers;
    
    public Cpu(int registerCount, Memory dataMemory, Memory instructionMemory)
    {
        _registers = new Register[registerCount];
        for (var i = 0; i < registerCount; i++)
            _registers[i] = new Register();
        
        _programCounter = 0;
        _dataMemory = dataMemory;
        _instructionMemory = instructionMemory;
    }
    
    public int GetRegisterValue(int registerIndex) => _registers[registerIndex].Read();
    public void LoadInstruction(Memory instructionMemory) => _instructionMemory = instructionMemory;

    public void Execute()
    {
        var instructionMemoryLenght = _instructionMemory.Get().Length;
        
        // _programCounter < instructionMemoryLenght
        while (_programCounter < instructionMemoryLenght)
        {
            Console.WriteLine($"_pCounter: {_programCounter}");
            if (_programCounter == -2) break;
            
            var instruction = _instructionMemory.Read(_programCounter++);
                
            // opCode, dstRegister, srcRegister
            int[] decodedInstructions = [ (instruction >> 16) & 0xFF, (instruction >> 8) & 0xFF, instruction & 0xFF ];

            Console.WriteLine($"<opCode: {decodedInstructions[0]}, " +
                              $"dst: {decodedInstructions[1]}, " +
                              $"src: {decodedInstructions[2]}>");

            try
            {
                switch (decodedInstructions[0])
                {
                    case 0x00: // LOAD dest <- memory[src2]
                        _registers[decodedInstructions[1]].Write(
                            _dataMemory.Read(_registers[decodedInstructions[2]].Read()));
                        break;
                    case 0x01: // MOV reg[opCode[1]] = reg[opCode[2]]
                        _registers[decodedInstructions[1]].Write(_registers[decodedInstructions[2]].Read());
                        break;
                    case 0x02: // SUB dest = dest - src2
                        var subValue= _registers[decodedInstructions[1]].Read() - 
                                      _registers[decodedInstructions[2]].Read();
                        _registers[decodedInstructions[1]].Write(subValue);
                        break;
                    case 0x03: // INCREMENT reg[opCode[1]]
                        var incrValue = _registers[decodedInstructions[1]].Read();
                        _registers[decodedInstructions[1]].Write(++incrValue);
                        break;
                    case 0x04: // JUMP TO opCode[1] if reg[opCode[2]] < 0
                        if (_registers[decodedInstructions[2]].Read() < 0)
                            _programCounter = decodedInstructions[1] - 1;
                        break;
                    case 0x05: // JUMP TO opCode[1]
                        _programCounter = decodedInstructions[1] - 1;
                        break;
                    case 0x06: // ABORT
                        _programCounter = -2;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(decodedInstructions), 
                            decodedInstructions[0], "Unknown opCode");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            
            
            WriteState(decodedInstructions);
            //_programCounter++;
        }
    }

    private void WriteState(int[] instructions)
    {
        Console.WriteLine($"Instructions: {string.Join(", ", instructions)}");
        
        Console.Write("Registers: ");
        foreach (var register in _registers)
            Console.Write($"{register.Read()} ");

        Console.WriteLine();
        Console.WriteLine("Memory: " + string.Join(", ", _dataMemory.Get()));
        Console.WriteLine("ProgramCounter: " + _programCounter);
        Console.WriteLine("-----------------");
    }
}