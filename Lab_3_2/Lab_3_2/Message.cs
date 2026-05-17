namespace Lab_3_2;

public class Message
{
    public required string Text { get; init; }
    public required DateTime Date { get; init; }
    public required UserData Author { get; init; }
    public required string Id { get; init; }
}