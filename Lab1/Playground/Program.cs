using Lab1;

namespace Playground;

public static class Program
{
    public static void Main()
    {
        var list = new CircleList();
        var exit = false;

        while (!exit)
        {
            Console.WriteLine("\n===== Меню =====");
            Console.WriteLine("1. Добавить элемент");
            Console.WriteLine("2. Удалить все вхождений заданного по значению элемента");
            Console.WriteLine("3. Удалить элемент перед каждым вхождением заданного по значению элемента");
            Console.WriteLine("4. Поиск элемента");
            Console.WriteLine("5. Печать списка");
            Console.WriteLine("6. Очистить список");
            Console.WriteLine("7. Объединить с другим списком");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите пункт: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Введите число: ");
                    
                    if (!int.TryParse(Console.ReadLine(), out var addValue))
                    {
                        Console.WriteLine("Ошибка: нужно число!");
                        break;
                    }

                    list.Add(addValue);
                    break;

                case "2":
                    Console.Write("Введите число для удаления: ");
                    
                    if (!int.TryParse(Console.ReadLine(), out var removeValue)) {
                        Console.WriteLine("Ошибка: нужно число!");
                        break;
                    }
                    
                    list.RemoveAll(removeValue);
                    break;

                case "3":
                    Console.Write("Введите число: ");
                    
                    if (!int.TryParse(Console.ReadLine(), out var data))
                    {
                        Console.WriteLine("Ошибка: нужно число!");
                        break;
                    }
                    
                    list.RemoveAllBefore(data);
                    break;

                case "4":
                    Console.Write("Введите число для поиска: ");
                    
                    if (!int.TryParse(Console.ReadLine(), out var searchValue))
                    {
                        Console.WriteLine("Ошибка: нужно число!");
                        break;
                    }

                    Console.WriteLine(list.Search(searchValue, out var foundIndex)
                        ? $"Элемент {searchValue} найден на позиции {foundIndex}."
                        : $"Элемент {searchValue} не найден.");

                    break;

                case "5":
                    list.Print();
                    break;

                case "6":
                    list.Clear();
                    break;

                case "7":
                    Console.WriteLine("Введите числа для второго списка через пробел:");
                    
                    var input = Console.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (input != null)
                    {
                        var nums = Array.ConvertAll(input, int.Parse);
                        var second = new CircleList(nums);
                        var intersected = CircleList.Union(list, second);
                        
                        Console.WriteLine("Результат пересечения:");
                        intersected.Print();
                    }
                    break;

                case "0":
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Неверный выбор, попробуйте снова.");
                    break;
            }
        }
    }
}