using Lab6;

var db = new Database<TreeKey, Record>(r => new TreeKey(r.FullName, r.OrderId));
var inputFile = "input.txt";

var content = File.ReadAllLines(inputFile);

foreach (var line in content)
{
    var parts = line.Split(' ');
    if (parts.Length < 4)
        continue;

    var fullName = new FullName()
    {
        LastName = parts[0],
        FirstName = parts[1],
        MiddleName = parts[2]
    };
    
    if (!int.TryParse(parts[3], out var orderId))
        continue;

    var record = new Record(fullName, orderId);
    db.Add(record);
}

var key = new TreeKey(new FullName("Смирнов", "Александр", "Михайлович"), 40);
var records = db.Find(key);

Console.WriteLine($"Found record by key {key}");

foreach (var record in records)
    Console.WriteLine(record);

var prevCount = db.Count;
Console.WriteLine($"Current count: {prevCount}");
Console.WriteLine($"Delete key {key}");
db.Delete(key);

Console.WriteLine($"Deleted {prevCount - db.Count} records");
Console.WriteLine($"New count {db.Count}");
Console.WriteLine("Remove all records");
/*
void GenerateInputFile(string filePath, int count)
{
    string[] lastNames = { "Иванов", "Петров", "Сидоров", "Кузнецов", "Смирнов", "Михайлов", "Васильев", "Павлов", "Соколов", "Новиков" };
    string[] firstNames = { "Александр", "Дмитрий", "Сергей", "Андрей", "Алексей", "Максим", "Евгений", "Иван", "Михаил", "Артем" };
    string[] middleNames = { "Александрович", "Дмитриевич", "Сергеевич", "Андреевич", "Алексеевич", "Максимович", "Евгеньевич", "Иванович", "Михайлович", "Артемович" };

    var rnd = new Random();

    using var sw = new StreamWriter(filePath);
    for (var i = 0; i < count; i++)
    {
        var lastName = lastNames[rnd.Next(lastNames.Length)];
        var firstName = firstNames[rnd.Next(firstNames.Length)];
        var middleName = middleNames[rnd.Next(middleNames.Length)];
        var number = rnd.Next(0, 101);

        sw.WriteLine($"{lastName} {firstName} {middleName} {number}");
    }
}

GenerateInputFile(inputFile, 999);
*/