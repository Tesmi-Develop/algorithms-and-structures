using Hypercube.Utilities.Dependencies;

namespace Lab_3_2;

public class UserClient : IDisposable
{
    [Dependency] private readonly ChatService _chatService = null!;
    private Action<Message, Group> _messageReceived;
    public UserData UserData { get; }

    public static UserClient Create(string userName, Action<Message, Group> messageReceived)
    {
        var userData = new UserData { Name = userName, Id = Guid.NewGuid().ToString() };
        return new UserClient(userData, messageReceived);
    }
    
    private UserClient(UserData userData, Action<Message, Group> messageReceived)
    {
        UserData = userData;
        _messageReceived = messageReceived;
    }

    public void Start()
    {
        _chatService.MessageReceived += _messageReceived;
    }

    public void Dispose()
    {
        _chatService.MessageReceived -= _messageReceived;
    }
}