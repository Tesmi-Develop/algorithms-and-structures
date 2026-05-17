namespace Lab_3_2;

public class Group
{
    public event Action<Message>? MessageReceived;
    public string Id { get; } = Guid.NewGuid().ToString();
    
    private readonly List<Message> _messages = [];
    private readonly List<UserClient> _users = [];

    public void AddUser(UserClient user)
    {
        _users.Add(user);
        user.Start();
    }

    public void RemoveUser(string userId)
    {
        var user = _users.Find(x => x.UserData.Id == userId);
        if (user == null)
            throw new ArgumentException($"User with id: {userId} not found");
        
        _users.Remove(user);
        user.Dispose();
    }
    
    public void SendMessage(Message message)
    {
        _messages.Add(message);
        MessageReceived?.Invoke(message);
    }

    public void RemoveMessage(string messageId)
    {
        var message = _messages.Find(x => x.Id == messageId);
        
        if (message is null)
            throw new ArgumentException($"Message with ID {messageId} not found");
        
        _messages.Remove(message);
    }
}