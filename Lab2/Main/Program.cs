using System.Diagnostics;
using System.Globalization;
using System.Text;
using Record;

namespace Lab2;

public static class Program
{
    private const string InputFile = "input.txt";

    private static List<UserRecord> ReadRecords(int countRecords)
    {
        var records = new List<UserRecord>();
        using var reader = new StreamReader(InputFile);
        var index = 1;

        while (index <= countRecords && reader.ReadLine() is { } line)
        {
            var parts = line.Split('\t',  StringSplitOptions.RemoveEmptyEntries);
            var date = DateTime.Parse(parts[0], CultureInfo.InvariantCulture);
            
            records.Add(new UserRecord { Date = date, Name = parts[1], Index = index });
            index++;
        }
        
        return records;
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
    
    private static StringBuilder ToLines(UserRecord[] arr, TimeSpan time)
    {
        var lines = new StringBuilder();
        
        foreach (var r in arr)
            lines.AppendLine(r.ToString());
        
        lines.Append($"Время сортировки: {time.TotalSeconds:F3} сек");
        return lines;
    }

    private static TimeSpan RunSort(UserRecord[] array, int length, Action<UserRecord[], int> sort)
    {
        var sw = Stopwatch.StartNew();
        sort(array, length);
        sw.Stop();
        
        return sw.Elapsed;
    }
     
    public static void Main()
    {
        Console.WriteLine("Введите количество строк n: ");
        var n = int.Parse(Console.ReadLine() ?? string.Empty);
        
        var records = ReadRecords(n);
        var dataMerge = records.ToArray();
        var dataInsert = records.ToArray();
        
        Console.WriteLine("Старт сортировки: Слияние (естественное, итеративная реализация)");
        var mergeTime = RunSort(dataMerge, n, Sorting.Sorting.NaturalMergeSort);
        
        Console.WriteLine("Старт сортировки: Простые вставки");
        var insertTime = RunSort(dataInsert, n, Sorting.Sorting.InsertionSort);
        
        Console.WriteLine("Сортировки завершены");
        
        File.WriteAllText(Path.Combine(GetProjectRoot(), "output_merge.txt"), ToLines(dataMerge, mergeTime).ToString());
        File.WriteAllText(Path.Combine(GetProjectRoot(), "output_insertion.txt"), ToLines(dataInsert, insertTime).ToString());
    }
}