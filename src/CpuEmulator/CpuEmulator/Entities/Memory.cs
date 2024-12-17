namespace CpuEmulator.Entities;

public class Memory(int[] memoryData)
{
    public int[] Get() => memoryData;
    public int Read(int index) => memoryData.Length > index ? memoryData[index] : 0;
    public void Write(int index, int value) => memoryData[index] = value;
}