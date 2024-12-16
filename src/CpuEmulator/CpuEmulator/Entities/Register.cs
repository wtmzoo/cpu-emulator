namespace CpuEmulator.Entities;

public class Register(int data = 0)
{
    private int _data = data;
    public int Read() => _data;
    public void Write(int value) => _data = value;
}