using Hypercube.Utilities.Dependencies;
using Lab_3_2.Commands;

namespace Lab_3_2;

public static class Program
{
    private static readonly DependenciesContainer Container = new();
    
    public static void Main()
    {
        Container.RegisterSingleton<DependenciesContainer>(Container);
        Container.Register<ChatService>();
        Container.ResolveAll();
        
        Start();
    }

    private static void Start()
    {
        var user1 = UserClient.Create("John Doe", (message, group) => Console.WriteLine($"I read message from {message.Author.Name} in group with id: {group.Id}"));
        var user2 = UserClient.Create("Tesmi",  (message, group) => Console.WriteLine($"I read message: {message.Text} in group with id: {group.Id}"));

        var service = Container.Resolve<ChatService>();
        var groupCommand = CreateGroup.Create();
        
        service.SendCommand(groupCommand);
        
        service.AddUserInGroup(user1, groupCommand.Group.Id);
        service.AddUserInGroup(user2, groupCommand.Group.Id);

        var createMessageCommand = SendMessage.Create("Hello World!", user1.UserData, groupCommand.Group.Id);
        service.SendCommand(createMessageCommand);
        service.SendCommand(new RemoveMessage(createMessageCommand.Message.Id, groupCommand.Group.Id));

        foreach (var command in service.CommandHistory)
        {
            Console.WriteLine($"Command name: {command.GetType().Name}, Date: {command.Date}");
        }
    }
}