namespace Main;

public class Graph
{
    private List<Edge> _edges = [];
    private Dictionary<int, List<int>> _neighbors = [];
    public int TotalWeight { get; private set; }

    public void LoadFromFile(string fileName)
    {
        var edges = new List<Edge>();
        var lines = File.ReadAllLines(fileName);

        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            var split = line.Split(' ');
            
            if (split.Length != 3 || 
                !int.TryParse(split[0], out var from) ||
                !int.TryParse(split[1], out var to) ||
                !int.TryParse(split[2], out var weight)
               )
                throw new Exception($"Invalid number of edges: {i}");
            
            
            edges.Add(new Edge { From = from, To = to, Weight = weight });
        }
        
        LoadFromEdges(edges);
    }

    public List<int> DFS(int nodeIndex)
    {
        var output = new List<int>();
        var visited = new HashSet<int>();
        DFSInternal(nodeIndex, visited, output);
        
        return output;
    }

    private void DFSInternal(int nodeIndex, HashSet<int> visited, List<int> output)
    {
        if (!_neighbors.TryGetValue(nodeIndex, out var value))
            throw new Exception($"Node {nodeIndex} does not exist");
        
        visited.Add(nodeIndex);
        output.Add(nodeIndex);

        foreach (var node in value)
        {
            if (!visited.Contains(node))
                DFSInternal(node, visited, output);
        }
    }

    private static int Find(int i, int[] parent)
    {
        if (parent[i] == i)
            return i;
        
        return parent[i] = Find(parent[i], parent);
    }
    
    public Graph Kruskal()
    {
        var graph = new Graph();
        var newEdges = new List<Edge>();
        var sortedEdges = _edges.OrderBy(e => e.Weight).ToList();
        var maxNode = _edges.Max(e => Math.Max(e.From, e.To));
        var parent = Enumerable.Range(0, maxNode + 1).ToArray();

        foreach (var edge in sortedEdges)
        {
            var rootFrom = Find(edge.From, parent);
            var rootTo = Find(edge.To, parent);

            if (rootFrom == rootTo) 
                continue;
            
            newEdges.Add(edge);
            parent[rootFrom] = rootTo;
        }
        
        graph.LoadFromEdges(newEdges);
        return graph;
    }

    public void LoadFromEdges(List<Edge> edges)
    {
        var neighbors = new Dictionary<int, List<int>>();
        var newEdges = new List<Edge>();
        var seenPairs = new HashSet<(int, int)>();
        var totalWeight = 0;
    
        foreach (var edge in edges)
        {
            var min = Math.Min(edge.From, edge.To);
            var max = Math.Max(edge.From, edge.To);
            var pair = (min, max);
            
            if (!seenPairs.Add(pair))
                throw new Exception("Found duplicate edges");

            newEdges.Add(edge);
            
            if (!neighbors.TryGetValue(edge.From, out var fromValue))
            {
                fromValue = [];
                neighbors.Add(edge.From, fromValue);
            }
            
            fromValue.Add(edge.To);
        
            if (!neighbors.TryGetValue(edge.To, out var toValue))
            {
                toValue = [];
                neighbors.Add(edge.To, toValue);
            }
            
            toValue.Add(edge.From);
            totalWeight += edge.Weight;
        }
    
        _edges = newEdges;
        _neighbors = neighbors;
        TotalWeight = totalWeight;
    }
}