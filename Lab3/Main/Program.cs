using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text;
using Record;
using Searchers;

namespace Lab2;

public static class Program
{
    private const string InputFile = "input.txt";
    private delegate List<int> SearchSignature(UserRecord[] array, int n, int key, Func<UserRecord, int, int> compare, out int steps);

    private struct SearchResult
    {
        public required string Name;
        public required int Steps;
        public required TimeSpan Time;
        public required List<int> Result;

        public override string ToString()
        {
            var sb = new StringBuilder();
            return sb
                .AppendLine($"Название поиска: {Name}")
                .AppendLine($"Результат: {string.Join(", ", Result)}")
                .AppendLine($"Количество шагов: {Steps}")
                .AppendLine($"Время: {Time.TotalMicroseconds:F3} µs")
                .ToString();
        }
    }
    
    private static List<UserRecord> ReadRecords(int countRecords, string fileName)
    {
        var records = new List<UserRecord>();
        using var reader = new StreamReader(fileName);
        var random = new Random();
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
            }, Index = random.Next(countRecords) });
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
    
    private static string ToBlocks(SearchResult[] arr)
    {
        var lines = new StringBuilder();

        foreach (var r in arr)
            lines.AppendLine(r.ToString());

        return lines.ToString();
    }

    private static (int countSteps, Stopwatch stopwatch, List<int> result) RunSearch(UserRecord[] array, int length, int key, SearchSignature search)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = search(array, length, key, Compare, out var countSteps);
        stopwatch.Stop();
        return (countSteps, stopwatch, result);
        
        int Compare(UserRecord record, int k) => record.Index - k;
    }
    
    private static (int countSteps, Stopwatch stopwatch, List<int> result) RunBinarySearchWithTable(UserRecord[] array, int length, int key)
    {
        var stopwatch = new Stopwatch();
        var table = Searcher.BuildStepTable(length);
        stopwatch.Start();
        var result = Searcher.UniformBinary(array, length, key, Compare, table, out var countSteps);
        stopwatch.Stop();
        return (countSteps, stopwatch, result);
        
        int Compare(UserRecord record, int k) => record.Index - k;
    }

    private static void GeneratePivotTable(double coefficient)
    {
        const int maxIterations = 19;
        var count = (int)Math.Pow(2, maxIterations);
        var data = ReadRecords(count, InputFile).ToArray();
        Sorting.Sorting.NaturalMergeSort(data, count);
        
        var result = new StringBuilder();
        var findKey = (int)Lerp(0, data[^1].Index, 0.75f);
        result.AppendLine("N\tЛинейный поиск\tОднородный бинарный поиск\tОднородный бинарный поиск с готовой таблицей");

        for (var i = 1; i <= maxIterations; i++)
        {
            var n = (int)Math.Pow(2, i);
            var result1 = RunSearch(data, n, findKey, Searcher.Linear);
            var result2 = RunSearch(data, n, findKey, Searcher.UniformBinary);
            var result3 = RunBinarySearchWithTable(data, n, findKey);
            
            result.AppendLine($"{n}\t{result1.stopwatch.Elapsed.TotalMicroseconds:F3}\t{result2.stopwatch.Elapsed.TotalMicroseconds:F3}\t{result3.stopwatch.Elapsed.TotalMicroseconds:F3}");
            Console.WriteLine($"Завершено для i={i}");
        }
        
        File.WriteAllText(Path.Combine(GetProjectRoot(), $"output_percent_{(int)(coefficient * 100)}.txt"), result.ToString());
    }

    private static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
    
    public static void Main()
    {
        Console.Write("Введите количество строк n: ");
        var n = int.Parse(Console.ReadLine() ?? string.Empty);
        
        var records = ReadRecords(n, InputFile).ToArray();
        Sorting.Sorting.NaturalMergeSort(records, n);
        
        var findKey = (int)Lerp(0, records.Length, 0.66f);
        
        Console.WriteLine("Старт поиска: Линейный");
        var (linearSteps, linearTime, linearResult) = RunSearch(records, n, records[findKey].Index, Searcher.Linear);
        
        Console.WriteLine("Старт поиска: однородный бинарный поиск");
        var (uniformBinarySteps, uniformBinaryTime, uniformBinaryResult) = RunSearch(records, n, records[findKey].Index, Searcher.UniformBinary);
        
        Console.WriteLine("Поиски завершены");

        var result = ToBlocks([
            new SearchResult()
            {
                Name = "Линейный поиск",
                Result = linearResult,
                Steps = linearSteps,
                Time = linearTime.Elapsed,
            },
            new SearchResult()
            {
                Name = "Однородный бинарный поиск с использованием таблицы шагов",
                Result = uniformBinaryResult,
                Steps = uniformBinarySteps,
                Time = uniformBinaryTime.Elapsed,
            },
        ]);
        
        File.WriteAllText(Path.Combine(GetProjectRoot(), "output.txt"), result);

        for (double i = 0; i <= 1; i += 0.2)
            GeneratePivotTable(i);
    }
}