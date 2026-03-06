using Main;

namespace Tests;

[TestFixture]
public class GraphTests
{
    private Graph _graph;

    [SetUp]
    public void Setup()
    {
        _graph = new Graph();
    }

    [Test]
    public void LoadFromEdges_DuplicateEdges_ThrowsException()
    {
        var edges = new List<Edge>
        {
            new() { From = 1, To = 2, Weight = 10 },
            new() { From = 2, To = 1, Weight = 5 } 
        };

        Assert.Throws<Exception>(() => _graph.LoadFromEdges(edges));
    }
    
    [Test]
    public void DFS_ValidGraph_ReturnsCorrectOrder()
    {
        var edges = new List<Edge>
        {
            new() { From = 1, To = 2, Weight = 1 },
            new() { From = 2, To = 3, Weight = 1 }
        };
        _graph.LoadFromEdges(edges);
        var result = _graph.DFS(1);

        Assert.That(result, Is.EqualTo(new List<int> { 1, 2, 3 }));
    }
    
    [Test]
    public void DFS_NonExistentNode_ThrowsException()
    {
        var edges = new List<Edge> { new() { From = 1, To = 2, Weight = 1 } };
        _graph.LoadFromEdges(edges);

        Assert.Throws<Exception>(() => _graph.DFS(99));
    }
    
    [Test]
    public void Kruskal_SimpleGraph_ReturnsMinimumSpanningTree()
    {
        var edges = new List<Edge>
        {
            new() { From = 1, To = 2, Weight = 10 },
            new() { From = 2, To = 3, Weight = 1 },
            new() { From = 1, To = 3, Weight = 20 }
        };
        _graph.LoadFromEdges(edges);

        var mst = _graph.Kruskal();
        Assert.That(mst.TotalWeight, Is.EqualTo(11));
    }
    
    [Test]
    public void Kruskal_ComplexGraph_TotalWeightIsCorrect()
    {
        var edges = new List<Edge>
        {
            new() { From = 1, To = 2, Weight = 4 },
            new() { From = 1, To = 3, Weight = 2 },
            new() { From = 3, To = 2, Weight = 1 },
            new() { From = 2, To = 4, Weight = 5 },
            new() { From = 3, To = 4, Weight = 8 }
        };
        _graph.LoadFromEdges(edges);

        var mst = _graph.Kruskal();
        Assert.That(mst.TotalWeight, Is.EqualTo(8));
    }
}