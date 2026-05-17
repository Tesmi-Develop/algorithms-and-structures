using Hypercube.Utilities.Dependencies;

namespace Lab_3_2.Commands;

public class RemoveMessage : ICommand
{
    public DateTime Date => DateTime.Now;
    public string GroupId { get; }
    
    [Dependency] private readonly ChatService _chatService = null!;
    private readonly string _messageId;
    
    public RemoveMessage(string messageId, string groupId)
    {
        _messageId = messageId;
        GroupId = groupId;
    }
    
    public void Execute()
    {
        _chatService.RemoveMessage(_messageId, GroupId);
    }
}