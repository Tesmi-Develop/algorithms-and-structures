using Searchers;

namespace Tests;

[TestFixture]
public class Tests
{
    [Test]
    public void AhoCorasickTest()
    {
        var cases = new[]
        {
            new { 
                Patterns = new[] { "he", "she", "his", "hers" }, 
                Text = "ushers", 
                Expected = new Dictionary<string, int> {
                    { "he", 1 },
                    { "she", 1 },
                    { "his", 0 },
                    { "hers", 1 }
                }
            },
            new { 
                Patterns = new[] { "aba", "a" }, 
                Text = "abababa", 
                Expected = new Dictionary<string, int> {
                    { "aba", 3 },
                    { "a", 4 }
                }
            },
            
            new { 
                Patterns = new[] { "a", "aa", "aaa" }, 
                Text = "aaaa", 
                Expected = new Dictionary<string, int> {
                    { "a", 4 },
                    { "aa", 3 },
                    { "aaa", 2 }
                }
            },
            
            new { 
                Patterns = new[] { "hello", "world" }, 
                Text = "goodbye moon", 
                Expected = new Dictionary<string, int> {
                    { "hello", 0 },
                    { "world", 0 }
                }
            },
            
            new { 
                Patterns = new[] { "ab", "ba", "aba", "bab" }, 
                Text = "ababab", 
                Expected = new Dictionary<string, int> {
                    { "ab", 3 },
                    { "ba", 2 },
                    { "aba", 2 },
                    { "bab", 2 }
                }
            }
        };

        foreach (var c in cases)
        {
            var actual = AhoCorasick.Search(c.Text, c.Patterns.ToList());

            foreach (var (pattern, count) in c.Expected)
            {
                    Assert.Multiple(() =>
                    {
                        if (count == 0)
                        {
                            Assert.That(actual.ContainsKey(pattern), Is.False);
                            return;
                        }
                        
                        Assert.That(actual.ContainsKey(pattern), Is.True);
                        Assert.That(actual[pattern], Is.EqualTo(count));
                    });
            }
        }
    }
    
    [Test]
    public void TurboBoyerMooreTest()
    {
        var cases = new[]
        {
            new { 
                Pattern = "she", 
                Text = "ushers", 
                Expected = 1
            },
            new { 
                Pattern = "a", 
                Text = "abababa", 
                Expected = 4
            },
            
            new { 
                Pattern = "aa", 
                Text = "aaaa", 
                Expected = 3
            },
            
            new { 
                Pattern = "hello", 
                Text = "goodbye moon", 
                Expected = 0
            },
            
            new { 
                Pattern = "bab", 
                Text = "ababab", 
                Expected = 2
            }
        };

        foreach (var c in cases)
        {
            var actual = TurboBoyerMoore.Search(c.Text, c.Pattern);
            Assert.That(actual, Is.EqualTo(c.Expected));
        }
    }
}