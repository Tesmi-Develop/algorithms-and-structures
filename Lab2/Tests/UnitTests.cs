namespace Tests;

[TestFixture]
public class Tests
{
    [Test]
    public void InsertionSortTest()
    {
        var cases = new[]
        {
            new[] { 2, 1 },
            new[] { 1, 2, 1 },
            new[] { 3, 2, 1 },
            new[] { 9, 8, 7, 6, 9, 8, 7, 6, 3, 4, 1 },
        };

        foreach (var array in cases)
        {
           Sorting.Sorting.InsertionSort(array, array.Length);
           
           for (var i = 0; i < array.Length - 1; i++)
            Assert.That(array[i + 1], Is.GreaterThanOrEqualTo(array[i]));
        }
    }
    
    [Test]
    public void NaturalMergeSortTest()
    {
        var cases = new[]
        {
            new[] { 2, 1 },
            new[] { 1, 2, 1 },
            new[] { 3, 2, 1 },
            new[] { 9, 8, 7, 6, 9, 8, 7, 6, 3, 4, 1 },
            new[] { 1, 2 },
            new[] { 1, 4, 5, 2, 3, 8, 9 }
        };

        foreach (var array in cases)
        {
            Sorting.Sorting.NaturalMergeSort(array, array.Length);
           
            for (var i = 0; i < array.Length - 1; i++)
                Assert.That(array[i + 1], Is.GreaterThanOrEqualTo(array[i]));
        }
    }
}