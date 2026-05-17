namespace Lab_3_2;

public interface ICommand
{
    DateTime Date { get; }
    void Execute();
}