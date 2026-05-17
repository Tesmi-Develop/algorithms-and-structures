using Hypercube.Utilities.Dependencies;

namespace Lab_3_2;

public class ChatService
{
    public event Action<Message, Group>? MessageReceived;
    public IReadOnlyList<ICommand> CommandHistory => _commandHistory;
    
    [Dependency] private readonly DependenciesContainer _container = null!;
    private List<ICommand> _commandHistory = [];
    private readonly List<Group> _groups = [];
    
    public void SendCommand(ICommand command)
    {
        _container.Inject(command);
        _commandHistory.Add(command);
        command.Execute();
    }

    public void SendMessage(Message message, string groupId)
    {
        if (!TryGetGroup(groupId, out var group))
            throw new ArgumentException($"Group with id: {groupId} not found");
        
        group!.SendMessage(message);
        MessageReceived?.Invoke(message, group!);
    }
    
    public void RemoveMessage(string messageId, string groupId)
    {
        if (!TryGetGroup(groupId, out var group))
            throw new ArgumentException($"Group with id: {groupId} not found");
        
        group!.RemoveMessage(messageId);
    }

    public void CreateGroup(Group group)
    {
        _container.Inject(group);
        _groups.Add(group);
    }

    private bool TryGetGroup(string id, out Group? group)
    {
        group = _groups.Find(group => group.Id == id);
        return group is not null;
    }
    
    public void AddUserInGroup(UserClient userClient, string groupId)
    {
        if (!TryGetGroup(groupId, out var group))
            throw new ArgumentException($"Group with id: {groupId} not found");
        
        _container.Inject(userClient);
        group!.AddUser(userClient);
    }

    public void RemoveUserFromGroup(string userId, string groupId)
    {
        if (!TryGetGroup(groupId, out var group))
            throw new ArgumentException($"Group with id: {groupId} not found");
        
        group!.RemoveUser(userId);
    }
}