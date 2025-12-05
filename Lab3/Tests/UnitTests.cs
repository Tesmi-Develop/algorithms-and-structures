using Searchers;

namespace Tests;

[TestFixture]
public class Tests
{
    [Test]
    public void LinearSearchTest()
    {
        var array = new[] { 1, 2, 3, 4, 5 };
        var res = Searcher.Linear(array, array.Length, 3, (i, i1) => i - i1, out var steps);
        Assert.That(res[0], Is.EqualTo(2));
    }
    
    [Test]
    public void UniformBinaryTest()
    {
        var array = new[] { 1, 2, 3, 3, 4, 5, 6, 8, 15, 20 };
        var testCases = new[]
        {
            new
            {
                Key = 1,
                Expected = new[] { 0 },
            },
            new {
                Key = 3,
                Expected = new[] { 2, 3 },
            },
            new {
                Key = 5,
                Expected = new[] { 5 },
            },
            new {
                Key = 2,
                Expected = new[] { 1 },
            },
            new {
                Key = 4,
                Expected = new[] { 4 },
            },
        };

        foreach (var testCase in testCases)
        {
            var result = Searcher.UniformBinary(array, array.Length, testCase.Key, (i, i1) => i - i1,  out var steps);
            Assert.That(result, Has.Count.EqualTo(testCase.Expected.Length));
                
            for (var i = 0; i < testCase.Expected.Length; i++)
                Assert.That(result[i], Is.EqualTo(testCase.Expected[i]));
        }
    }
}