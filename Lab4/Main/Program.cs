using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text;
using Record;
using Searchers;

namespace Lab4;

public static class Program
{
    private enum FieldType
    {
        Text,
        Name,
    }
    
    private const string InputFile = "input.txt";

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
            }, Index = int.Parse(parts[2]), Text = parts[3]});
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
    
    private static StringBuilder ToLines(List<UserRecord> arr, Dictionary<string, int> expectedNames, Dictionary<string, int> expectedTexts, TimeSpan time)
    {
        var lines = new StringBuilder();

        lines.AppendLine("Условия поиска для ФИО");

        foreach (var (key, value) in expectedNames)
            lines.AppendLine($"{key} - {value}");
        
        lines.AppendLine("Условия поиска для Описания");

        foreach (var (key, value) in expectedTexts)
            lines.AppendLine($"{key} - {value}");
        
        lines.AppendLine("Найденные записи:");
        
        foreach (var r in arr)
            lines.AppendLine(r.ToString());
        
        lines.Append($"Суммарное время поиска: {time.TotalMicroseconds:F3} µs");
        return lines;
    }

    private static bool AreEqualDictionaries(Dictionary<string, int> result, Dictionary<string, int> expected)
    {
        var matched = result.Count != 0;

        foreach (var (pattern, count) in expected)
        {
            if (result.TryGetValue(pattern, out var value) && value == count) 
                continue;
                
            matched = false;
            break;
        }

        return matched;
    }
    
    private static (TimeSpan, List<UserRecord>) RunAhoCorasick(UserRecord[] array, int length, Dictionary<string, int> expectedNames, Dictionary<string, int> expectedTexts)
    {
        List<UserRecord> foundRecords = [];
        var namePatterns = new List<string>();
        var textPatterns = new List<string>();

        foreach (var (pattern, _) in expectedNames)
            namePatterns.Add(pattern);
        
        foreach (var (pattern, _) in expectedTexts)
            textPatterns.Add(pattern);
        
        var time = TimeSpan.Zero;

        for (var i = 0; i < length; i++)
        {
            var element = array[i];
            
            var sw = Stopwatch.StartNew();
            var resultNames = AhoCorasick.Search(element.FullName.ToString().ToLower(), namePatterns);
            var resultTexts = AhoCorasick.Search(element.Text.ToLower(), textPatterns);
            sw.Stop();
            
            time += sw.Elapsed;
            var namesFound = AreEqualDictionaries(resultNames, expectedNames);
            var textFound = AreEqualDictionaries(resultTexts, expectedTexts);

            if (namesFound && textFound)
                foundRecords.Add(element);
        }
       
        
        return (time, foundRecords);
    }
    
    private static (TimeSpan, List<UserRecord>) RunBoyerMoore(UserRecord[] array, int length, string patternName, int expectedName, string patternText, int expectedText)
    {
        List<UserRecord> foundRecords = [];
        var time = TimeSpan.Zero;

        for (var i = 0; i < length; i++)
        {
            var element = array[i];
            var sw = Stopwatch.StartNew();
            var result1 = TurboBoyerMoore.Search(element.FullName.ToString().ToLower(), patternName);
            var result2 = TurboBoyerMoore.Search(element.Text.ToLower(), patternText);
            sw.Stop();
            time += sw.Elapsed;
            
            if (result1 != expectedName || result2 != expectedText) 
                continue;
            
            foundRecords.Add(element);
        }
       
        
        return (time, foundRecords);
    }

    private static void GeneratePivotTable()
    {
        const int maxIterations = 19;
        var data = ReadRecords((int)Math.Pow(2, maxIterations), InputFile).ToArray();
        var result = new StringBuilder();
        var templatesForNames = new Dictionary<string, int>{{"АBCD", 1}};
        var templatesForTexts = new Dictionary<string, int>{{"АBCD", 1}};
        var firstTemplateName = templatesForNames.First();
        var firstTemplateText = templatesForTexts.First();
        result.AppendLine("N\tАхо-Карасик\tТурбо-бойер-мура");

        for (var i = 1; i <= maxIterations; i++)
        {
            var n = (int)Math.Pow(2, i);
            var axoResult = RunAhoCorasick(data, n, templatesForNames, templatesForTexts);
            var boyerMoore = RunBoyerMoore(data, n, firstTemplateName.Key, firstTemplateName.Value, firstTemplateText.Key, firstTemplateText.Value);
            
            result.AppendLine($"{n}\t{axoResult.Item1.TotalMicroseconds:F3}\t{boyerMoore.Item1.TotalMicroseconds:F3}");
            Console.WriteLine($"Завершено для i={i}");
        }
        
        File.WriteAllText(Path.Combine(GetProjectRoot(), "output_pivot_table1.txt"), result.ToString());

        var fixedN = data.Length / 2;
        result = new StringBuilder();
        result.AppendLine("N\tАхо-Карасик\tТурбо-бойер-мура");
        
        /*for (var i = 1; i <= maxIterations; i++)
        {
            templatesForNames = new Dictionary<string, int>{{new string('А', i), 1}};
            templatesForTexts = new Dictionary<string, int>{{new string('А', i), 1}};
            var axoResult = RunAhoCorasick(data, fixedN, templatesForNames, templatesForTexts);
            var boyerMoore = RunBoyerMoore(data, fixedN, firstTemplateName.Key, firstTemplateName.Value, firstTemplateText.Key, firstTemplateText.Value);
            
            result.AppendLine($"{i}\t{axoResult.Item1.TotalMicroseconds:F3}\t{boyerMoore.Item1.TotalMicroseconds:F3}");
            Console.WriteLine($"Завершено для i={i}");
        }

        File.WriteAllText(Path.Combine(GetProjectRoot(), "output_pivot_table2.txt"), result.ToString());*/
        
        result = new StringBuilder();
        result.AppendLine("N\tАхо-Карасик\tТурбо-бойер-мура");
        var alphabet = "abcdefghijklmnopqrstuvwxyz";
        
        for (var i = 1; i <= maxIterations; i++)
        {
            var str = alphabet.Substring(1, i);
            templatesForNames = new Dictionary<string, int>{{str, 1}};
            templatesForTexts = new Dictionary<string, int>{{str, 1}};
            var axoResult = RunAhoCorasick(data, fixedN, templatesForNames, templatesForTexts);
            var boyerMoore = RunBoyerMoore(data, fixedN, firstTemplateName.Key, firstTemplateName.Value, firstTemplateText.Key, firstTemplateText.Value);
            
            result.AppendLine($"{i}\t{axoResult.Item1.TotalMicroseconds:F3}\t{boyerMoore.Item1.TotalMicroseconds:F3}");
            Console.WriteLine($"Завершено для i={i}");
        }

        File.WriteAllText(Path.Combine(GetProjectRoot(), "output_pivot_table3.txt"), result.ToString());
    }
    
    public static void Main()
    {
        Console.Write("Введите количество строк n: ");
        var n = int.Parse(Console.ReadLine() ?? string.Empty);
        
        var records = ReadRecords(n, InputFile);
        var data = records.ToArray();

        var templatesForNames = new Dictionary<string, int>{{"Кириллов", 1}};
        var templatesForTexts = new Dictionary<string, int>{{"напоминает", 1}};
        var firstTemplateName = templatesForNames.First();
        var firstTemplateText = templatesForTexts.First();
        
        Console.WriteLine("Старт поиска: Ахо-карасик");
        var axoResult = RunAhoCorasick(data, n, templatesForNames, templatesForTexts);
        
        Console.WriteLine("Старт поиска: Турбо-алгоритм Бойера – Мура");
        var boyerMoore = RunBoyerMoore(data, n, firstTemplateName.Key, firstTemplateName.Value, firstTemplateText.Key, firstTemplateText.Value);
        
        Console.WriteLine("Поиски завершены завершены");
        
        File.WriteAllText(Path.Combine(GetProjectRoot(), "output_axo_corasick.txt"), ToLines(
            axoResult.Item2, templatesForNames, templatesForTexts, axoResult.Item1
            ).ToString());
        File.WriteAllText(Path.Combine(GetProjectRoot(), "output_boyer_moore.txt"), ToLines(
            boyerMoore.Item2, new Dictionary<string, int>{ {firstTemplateName.Key, firstTemplateName.Value} }, 
            new Dictionary<string, int>{ {firstTemplateText.Key, firstTemplateText.Value} }, boyerMoore.Item1
        ).ToString());
        GeneratePivotTable();
        //GenerateBestAndBadResults();
    }
}