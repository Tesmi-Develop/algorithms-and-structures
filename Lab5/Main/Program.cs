using Main;

var graph = new Graph();
graph.LoadFromFile("Input.txt");

Console.WriteLine(PrintNodes(graph.DFS(1)));
var minGraph = graph.Kruskal();

Console.WriteLine($"Суммарный минимальный вес остовного графа {minGraph.TotalWeight}");
return;

string PrintNodes(List<int> nodes)
{
    var output = string.Empty;

    for (var i = 0; i < nodes.Count; i++)
        output += i < nodes.Count - 1 ? $"{nodes[i]} -> " : $"{nodes[i]}";
    
    return output;
}