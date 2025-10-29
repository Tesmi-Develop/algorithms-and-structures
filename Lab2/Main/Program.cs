using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text;
using Record;

namespace Lab2;

public static class Program
{
    private const string InputFile = "input.txt";
    private const string Input2File = "input2.txt";

    private static List<UserRecord> ReadRecords(int countRecords, string fileName)
    {
        var records = new List<UserRecord>();
        using var reader = new StreamReader(fileName);
        var index = 1;

        while (index <= countRecords && reader.ReadLine() is { } line)
        {
            var parts = line.Split('\t',  StringSplitOptions.RemoveEmptyEntries);
            var date = new CustomMyDate(parts[0]);

            var names = parts[1].Split(' ');
            
            records.Add(new UserRecord { Date = date, FullName = new FullName()
            {
                LastName = names[0],
                FirstName = names[1],
                MiddleName = names[2],
            }, Index = index });
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

    private static void GeneratePivotTable()
    {
        const int maxIterations = 16;
        var data = ReadRecords((int)Math.Pow(2, maxIterations), InputFile);
        var result = new StringBuilder();
        result.AppendLine("N\tПрямые вставки\tЕстественное слияние");

        for (var i = 1; i <= maxIterations; i++)
        {
            var n = (int)Math.Pow(2, i);
            var time1 = RunSort(data.ToArray(), n, Sorting.Sorting.InsertionSort);
            var time2 = RunSort(data.ToArray(), n, Sorting.Sorting.NaturalMergeSort);
            
            result.AppendLine($"{n}\t{time1.TotalMilliseconds:F3}\t{time2.TotalMilliseconds:F3}");
            Console.WriteLine($"Завершено для i={i}");
        }
        
        File.WriteAllText(Path.Combine(GetProjectRoot(), "output_pivot_table.txt"), result.ToString());
    }

    private static void GenerateBestAndBadResults()
    {
        const int n = 50000;
        var data = ReadRecords(n, InputFile);
        var result = new StringBuilder();
        result.AppendLine("Случаи\tПрямые вставки\tЕстественное слияние");
        
        var goodArray = data.ToArray();
        Sorting.Sorting.NaturalMergeSort(goodArray, n);
        
        var badArray = data.ToArray();
        Sorting.Sorting.NaturalMergeSort(badArray, n);
        Array.Reverse(badArray);
        
        var time11 = RunSort(goodArray.ToArray(), n, Sorting.Sorting.InsertionSort);
        var time12 = RunSort(goodArray.ToArray(), n, Sorting.Sorting.NaturalMergeSort);
        
        result.AppendLine($"Наилучший случай\t{time11.TotalMilliseconds:F3}\t{time12.TotalMilliseconds:F3}");
        
        var time21 = RunSort(badArray.ToArray(), n, Sorting.Sorting.InsertionSort);
        var time22 = RunSort(badArray.ToArray(), n, Sorting.Sorting.NaturalMergeSort);
        
        result.AppendLine($"Наихудший случай\t{time21.TotalMilliseconds:F3}\t{time22.TotalMilliseconds:F3}");
        
        File.WriteAllText(Path.Combine(GetProjectRoot(), "output_good_and_bad_results.txt"), result.ToString());
    }
    
    public static void Main()
    {
        Console.Write("Введите количество строк n: ");
        var n = int.Parse(Console.ReadLine() ?? string.Empty);
        
        var records = ReadRecords(n, InputFile);
        var dataMerge = records.ToArray();
        var dataInsert = records.ToArray();
        
        Console.WriteLine("Старт сортировки: Слияние (естественное, итеративная реализация)");
        var mergeTime = RunSort(dataMerge, n, Sorting.Sorting.NaturalMergeSort);
        
        Console.WriteLine("Старт сортировки: Простые вставки");
        var insertTime = RunSort(dataInsert, n, Sorting.Sorting.InsertionSort);
        
        Console.WriteLine("Сортировки завершены");
        
        File.WriteAllText(Path.Combine(GetProjectRoot(), "output_merge.txt"), ToLines(dataMerge, mergeTime).ToString());
        File.WriteAllText(Path.Combine(GetProjectRoot(), "output_insertion.txt"), ToLines(dataInsert, insertTime).ToString());
        //GeneratePivotTable();
        //GenerateBestAndBadResults();
    }
}