namespace CpuEmulator.Utilities;

public static class AssemblerConverter
{
    private static readonly Dictionary<string, int> InstructionSet  = new()
    {
        { "LOAD",  0x00 },
        { "MOV",   0x01 },
        { "ADD",   0x02 },
        { "SUB",   0x03 },
        { "INCR",  0x04 },
        { "ABORT", 0x05 },
        { "JMP",   0x06 },
        { "COPY",  0x07 },
        { "HLT",   0x08 }
    };
    
    public static int[] ConvertCommands(string[] assemblerCommands)
    {
        var machineCodes = new List<int>();

        foreach (var command in assemblerCommands)
        {
            var parts = command.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0) continue;

            var opCode = parts[0];
            if (InstructionSet.TryGetValue(opCode, out var opCodeValue) == false)
                throw new Exception($"Unknown opCode: {opCode}");
            
            int arg0 = 0, arg1 = 0;
            
            if (parts.Length > 1)
                arg0 = ParseRegisterOrValue(parts[1]);
            if (parts.Length > 2)
                arg1 = ParseRegisterOrValue(parts[2]);
            
            var machineCode = (opCodeValue << 16) | (arg0 << 8) | arg1;
            machineCodes.Add(machineCode);
        }
        
        return machineCodes.ToArray();
    }

    private static int ParseRegisterOrValue(string token) => int.Parse(token.StartsWith('R') ? token[1..] : token);
}