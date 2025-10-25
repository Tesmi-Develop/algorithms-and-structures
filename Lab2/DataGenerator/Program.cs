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
        public required List<string> MiddleNames;
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
        var middleNames = new List<string>();
        using var reader = new StreamReader(DataFileName);

        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split(' ',  StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 2)
                throw new Exception("Invalid data format");
            
            lastNames.Add(parts[0]);
            firstNames.Add(parts[1]);
            middleNames.Add(parts[2]);
        }
        
        return new Data()
        {
            FirstNames = firstNames,
            LastNames = lastNames,
            MiddleNames = middleNames,
        };
    }
    
    private static StringBuilder ToLines(UserRecord[] array)
    {
        var lines = new StringBuilder();
        
        foreach (var r in array)
            lines.AppendLine(r.ToStringWithoutIndex());
        
        return lines;
    }
    
    private static CustomMyDate GetRandomDate(Random random, DateTime start, DateTime end)
    {
        var range = (int)(end - start).TotalDays;
        return new CustomMyDate(start.Year, start.Month, start.Day).AddDays(random.Next(range));
    }

    private static FullName GetRandomFullName(Random random, Data data)
    {
        var index = random.Next(data.LastNames.Count);
        return new FullName()
        {
            LastName = data.LastNames[index],
            FirstName = data.FirstNames[index],
            MiddleName = data.MiddleNames[index],
        };
    }
    
    private static int ReadYear(string message, int defaultYear)
    {
        while (true)
        {
            Console.Write($"{message} (по умолчанию: {defaultYear}): ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                return defaultYear;

            if (int.TryParse(input, out var year) && year is >= 1 and <= 9999)
                return year;

            Console.WriteLine("Неверный формат. Введите корректный год (например: 2022).");
        }
    }
    
    private static int ReadCount(string message)
    {
        while (true)
        {
            Console.Write($"{message}: ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out var count) && count > 0)
                return count;

            Console.WriteLine("Некорректное количество. Введите корректное число > 0.");
        }
    }
     
    public static void Main()
    {
        try
        {
            var random = new Random();
            var data = ReadData();

            if (data.FirstNames.Count == 0 || data.LastNames.Count == 0 || data.MiddleNames.Count == 0)
                throw new Exception("Empty data");

            var count = ReadCount("Введите количество записей для генерации"); 

            int minYear, maxYear;
            while (true)
            {
                minYear = ReadYear("Введите (или пропустите этот шаг) минимальный год для записи", 2020);
                maxYear = ReadYear("Введите (или пропустите этот шаг) максимальный год для записи", 2025);

                if (minYear > maxYear)
                {
                    Console.WriteLine(
                        $"⚠️ Минимальный год ({minYear}) не может быть больше максимального ({maxYear}). Попробуйте снова.");
                    continue;
                }

                break;
            }

            var startData = new DateTime(minYear, 1, 1);
            var endData = new DateTime(maxYear, 12, 31);
            var records = new List<UserRecord>();

            for (var i = 0; i < count; i++)
            {
                records.Add(new UserRecord()
                {
                    FullName = GetRandomFullName(random, data),
                    Date = GetRandomDate(random, startData, endData),
                    Index = i,
                });
            }

            File.WriteAllText(Path.Combine(GetProjectRoot(), "output.txt"), ToLines(records.ToArray()).ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        Console.WriteLine("Для завершение программы нажмите любую клавишу");
        Console.ReadKey();
    }
}