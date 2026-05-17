using Hypercube.Utilities.Dependencies;

namespace Lab_3_2.Commands;

public class CreateGroup : ICommand
{
    public DateTime Date { get; } = DateTime.Now;
    [Dependency] private ChatService _chatService = null!;
    public Group Group { get; }

    public static CreateGroup Create()
    {
        return new CreateGroup(new Group());
    }
    
    public CreateGroup(Group group)
    {
        Group = group;
    }
    
    public void Execute()
    {
        _chatService.CreateGroup(Group);
    }
}