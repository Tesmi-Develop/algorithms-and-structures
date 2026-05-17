using Hypercube.Utilities.Dependencies;

namespace Lab_3_2.Commands;

public class SendMessage : ICommand
{
    public DateTime Date => DateTime.Now;
    
    public Message Message { get; }
    public string GroupId { get; }
    [Dependency] private ChatService _chatService = null!;

    public static SendMessage Create(string text, UserData author, string groupId)
    {
        return new SendMessage(new Message()
        {
            Text = text,
            Date = DateTime.Now,
            Author = author,
            Id = Guid.NewGuid().ToString(),
        }, groupId);
    }
    
    public SendMessage(Message message,  string groupId)
    {
        Message = message;
        GroupId = groupId;
    }


    public void Execute()
    {
        _chatService.SendMessage(Message, GroupId);
    }
}