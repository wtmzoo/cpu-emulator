namespace CpuEmulator.Entities;

public class Memory(int[] memoryData)
{
    public int[] Get() => memoryData;
    public int Read(int index) => memoryData[index];
    public void Write(int index, int value) => memoryData[index] = value;
}