using Lab1;

namespace Tests;

[TestFixture]
public class Tests
{
    private static bool IsMatch(CircleList source, int[] nums)
    {
        if (source.Empty)
            return nums.Length == 0;
        
        var index = 0;
        var current = source.Head!;

        do
        {
            if (index >= nums.Length || current.Data != nums[index]) 
                return false;
            
            current = current.Next;
            index++;
        } while (source.Head != current);

        return index == nums.Length;
    }
    
    [Test]
    public void InitializeTest()
    {
        Assert.DoesNotThrow(() => { _ = new CircleList(); });
    }

    [Test]
    public void InitializeWithDataTest()
    {
        var list = new CircleList([5, 6]);
        Assert.That(IsMatch(list, [6, 5]), Is.True);
    }

    [Test]
    public void IsEmptyTest()
    {
        var list = new CircleList();
        Assert.That(list.Empty, Is.True);
        list.Add(1);
        Assert.That(list.Empty, Is.False);
    }

    [Test]
    public void DisposeTest()
    {
        var list = new CircleList([5, 6]);
        list.Dispose();
        Assert.That(list.Empty, Is.True);

        list = new CircleList();
        list.Clear();
        Assert.That(list.Empty, Is.True);
    }

    [Test]
    public void AddTest()
    {
        var list = new CircleList();
        const int maxValue = 50;

        for (var i = 0; i <= maxValue; i++)
        {
            list.Add(i);
            list.Add(maxValue - i);
        }

        var current = list.Head!;

        do
        {
            Assert.That(current.Data, Is.GreaterThanOrEqualTo(current.Next.Data));
            current = current.Next;
        } while (current.Next != list.Head);
    }

    [Test]
    public void SearchTest()
    {
        var list1 = new CircleList();
        for (var i = 0; i <= 50; i++)
            list1.Add(i);

        Assert.Multiple(() =>
        {
            Assert.That(list1.Search(50, out var foundNode), Is.True);
            Assert.That(foundNode!.Data, Is.EqualTo(50));
            
            Assert.That(list1.Search(25, out foundNode), Is.True);
            Assert.That(foundNode!.Data, Is.EqualTo(25));
            
            Assert.That(list1.Search(51, out foundNode), Is.False);
            Assert.That(foundNode, Is.EqualTo(null));
        });
        
        var list2 = new CircleList([0, 0, 1]);
        Assert.Multiple(() =>
        {
            Assert.That(list2.Search(0, out var foundNode), Is.True);
            Assert.That(foundNode.Previous.Data, Is.EqualTo(1));
        });

        var emptyList = new CircleList();
        Assert.Multiple(() =>
        {
            Assert.That(emptyList.Search(0, out var foundNode), Is.False);
            Assert.That(foundNode, Is.EqualTo(null));
        });
    }

    [Test]
    public void PrintTest()
    {
        var list = new CircleList([1, 2]);
        var originalOut = Console.Out;

        try
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);

            list.Print();
            var output = sw.ToString().Trim().Split(Environment.NewLine);

            Assert.Multiple(() =>
            {
                Assert.That(output[0], Is.EqualTo("2"));
                Assert.That(output[1], Is.EqualTo("1"));
            });

            list.Clear();
            sw.GetStringBuilder().Clear();
            sw.Flush();
            list.Print();
            Assert.That(sw.ToString().Trim(), Is.EqualTo("List is empty"));
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Test]
    public void RemoveAllTest()
    {
        var list = new CircleList([1, 1, 4, 4]);
        list.RemoveAll(1);
        Assert.That(IsMatch(list, [4, 4]), Is.True);

        list.RemoveAll(4);
        Assert.That(list.Empty, Is.True);

        list = new CircleList([1, 1, 3, 3, 4, 4]);
        list.RemoveAll(3);
        Assert.That(IsMatch(list, [4, 4, 1, 1]));
    }

    [Test]
    public void RemoveAllBeforeTest()
    {
        var list = new CircleList([1, 2, 3]);
        list.RemoveAllBefore(1);
        Assert.That(IsMatch(list, [3, 1]), Is.True);

        list = new CircleList([1, 2, 3]);
        list.RemoveAllBefore(2);
        Assert.That(IsMatch(list, [2, 1]), Is.True);

        list = new CircleList([1, 2, 3]);
        list.RemoveAllBefore(3);
        Assert.That(IsMatch(list, [3, 2]), Is.True);

        list = new CircleList([1, 2, 2, 3]);
        list.RemoveAllBefore(2);
        Assert.That(IsMatch(list, [2, 1]), Is.True);
    }
    
    [Test]
    public void UnionTest()
    {
        var testCases = new[]
        {
            new
            {
                List1 = new[] {15, 10, 5},
                List2 = new[] {20, 15, 5},
                Expected = new[] {20, 15, 15, 10, 5, 5}
            },
            new
            {
                List1 = Array.Empty<int>(),
                List2 = Array.Empty<int>(),
                Expected = Array.Empty<int>()
            },
            new
            {
                List1 = new[] {1, 2, 3},
                List2 = Array.Empty<int>(),
                Expected = new[] {3, 2, 1}
            },
            new
            {
                List1 = new[] {10, 8, 5},
                List2 = new[] {20, 15, 12},
                Expected = new[] {20, 15, 12, 10, 8, 5}
            },
            new
            {
                List1 = new[] {10, 8, 8, 5},
                List2 = new[] {8, 8, 5},
                Expected = new[] {10, 8, 8, 8, 8, 5, 5}
            },
            new {
                List1 = new[] {10, 8, 8, 8, 5},
                List2 = new[] {8, 8, 5},
                Expected = new[] {10, 8, 8, 8, 8, 8, 5, 5}
            }
        };

        foreach (var testCase in testCases)
        {
            var list1 = new CircleList(testCase.List1);
            var list2 = new CircleList(testCase.List2);
            var unionList = list1 | list2;

            Assert.That(IsMatch(unionList, testCase.Expected), Is.True);
        }
    }
}