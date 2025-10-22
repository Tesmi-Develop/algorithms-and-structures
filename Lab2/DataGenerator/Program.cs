using System.Text;
using Record;

namespace DataGenerator;

public static class Program
{
    private const string DataFileName = "data.txt";

    private struct Data
    {
        public required List<string> FirstNames;
        public required List<string> LastNames;
    }
    
    private static string GetProjectRoot()
    {
        var dirInfo = new DirectoryInfo(AppContext.BaseDirectory);
        
        while (dirInfo is not null)
        {
            if (dirInfo.GetFiles("*.csproj").Length != 0)
                return dirInfo.FullName;

            dirInfo = dirInfo.Parent;
        }
        
        return Directory.GetCurrentDirectory();
    }
    
    private static Data ReadData()
    {
        var firstNames = new List<string>();
        var lastNames = new List<string>();
        using var reader = new StreamReader(DataFileName);

        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split('\t',  StringSplitOptions.RemoveEmptyEntries);
            firstNames.Add(parts[0]);
            lastNames.Add(parts[1]);
        }
        
        return new Data()
        {
            FirstNames = firstNames,
            LastNames = lastNames
        };
    }
    
    private static StringBuilder ToLines(UserRecord[] array)
    {
        var lines = new StringBuilder();
        
        foreach (var r in array)
            lines.AppendLine(r.ToStringWithoutIndex());
        
        return lines;
    }
    
    private static DateTime GetRandomDate(Random random, DateTime start, DateTime end)
    {
        var range = (int)(end - start).TotalSeconds;
        return start.AddSeconds(random.Next(range));
    }

    private static string GetRandomFullName(Random random, Data data)
    {
        return $"{data.FirstNames[random.Next(data.FirstNames.Count)]} {data.LastNames[random.Next(data.LastNames.Count)]}";
    }
     
    public static void Main()
    {
        Console.WriteLine("Введите количество записей для генерации");
        var count = int.Parse(Console.ReadLine() ?? string.Empty);
        var data = ReadData();
        var random = new Random();
        var startData = new DateTime(2020, 1, 1);
        var endData = new DateTime(2025, 10, 22);
        var records = new List<UserRecord>();

        for (var i = 0; i < count; i++)
        {
            records.Add(new UserRecord()
            {
                Name = GetRandomFullName(random, data),
                Date = GetRandomDate(random, startData, endData),
                Index = i,
            });
        }
        
        File.WriteAllText(Path.Combine(GetProjectRoot(), "output.txt"), ToLines(records.ToArray()).ToString());
    }
}