namespace Searchers;

public static class AhoCorasick
{
    public class Node
    {
        public required bool IsTerminal;
        public required char Symbol;
        public string Template = string.Empty;
        public required Node Parent;
        public Node SuffixLink = null!;
        public readonly Dictionary<char, Node> Children = [];
    }

    private static Node BuildBor(List<string> templates)
    {
        var root = new Node() { IsTerminal = false, Symbol = ' ', Parent = null! };
        
        foreach (var template in templates)
        {
            var nodePointer = root;
            var currentTemplate = string.Empty;
            
            foreach (var symbol in template)
            {
                currentTemplate += symbol;
                if (nodePointer.Children.TryGetValue(symbol, out var value))
                {
                    nodePointer = value;
                    continue;
                }
                
                var newNode = new Node() { IsTerminal = false, Symbol = symbol, SuffixLink = null!, Parent = nodePointer };
                nodePointer.Children.Add(symbol, newNode);
                nodePointer = newNode;
                nodePointer.Template = currentTemplate;
            }
            
            nodePointer.IsTerminal = true;
            nodePointer.Template = template;
        }
        
        return root;
    }

    private static Node FindSuffixLink(Node node)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (node.Parent.Parent is null)
            return node.Parent;
        
        var nodePointer = node.Parent.SuffixLink;
        Node? result = null;
        
        if (nodePointer is null)
            throw new Exception("Cannot find suffix link");

        while (result is null)
        {
            if (nodePointer.Children.TryGetValue(node.Symbol, out var value))
            {
                result = value;
                continue;
            }

            if (nodePointer.Parent is null)
            {
                result = nodePointer;
                continue;
            }
            
            nodePointer = nodePointer.SuffixLink;
        }

        return result;
    }
    
    private static void BuildSuffixes(Node bor)
    {
        var queue = new Queue<Node>();

        foreach (var (key, value) in bor.Children)
            queue.Enqueue(value);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            foreach (var child in node.Children)
                queue.Enqueue(child.Value);
            
            node.SuffixLink = FindSuffixLink(node);
        }
    }

    private static void VisitAllSuffixes(Node node, Dictionary<string, int> results)
    {
        var nodePointer = node;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        while (nodePointer is not null)
        {
            if (nodePointer.IsTerminal)
            {
                if (results.TryAdd(nodePointer.Template, 0))
                    continue;
                
                results[nodePointer.Template] += 1;
            }

            nodePointer = nodePointer.SuffixLink;
        }
    }
    
    private static Node FindNextNodeFromSymbol(Node node, char symbol)
    {
        var nodePointer = node;
        Node? result = null; 
        
        while (result is null)
        {
            if (nodePointer.Children.TryGetValue(symbol, out var value))
            {
                result = value;
                continue;
            }

            if (nodePointer.SuffixLink is null)
            {
                result = nodePointer;
                continue;
            }
            
            nodePointer = nodePointer.SuffixLink;
        }
        
        return result;
    }

    public static Node BuildFullBor(List<string> templates)
    {
        var bor = BuildBor(templates);
        BuildSuffixes(bor);
        return bor;
    }
    
    public static Dictionary<string, int> Search(string str, List<string> templates)
    {
        var result = new Dictionary<string, int>();
        var bor = BuildBor(templates);
        var nodePointer = bor;
        BuildSuffixes(bor);

        foreach (var ch in str)
        {
            if (nodePointer.Children.TryGetValue(ch, out var value))
            {
                nodePointer = value;
                VisitAllSuffixes(nodePointer, result);
                continue;
            }
            
            if (nodePointer.SuffixLink is null)
                continue;
            
            nodePointer = FindNextNodeFromSymbol(nodePointer, ch);
            VisitAllSuffixes(nodePointer, result);
        }
        
        return result;
    }
    
    public static Dictionary<string, int> Search(string str, Node bor)
    {
        var result = new Dictionary<string, int>();
        var nodePointer = bor;

        foreach (var ch in str)
        {
            if (nodePointer.Children.TryGetValue(ch, out var value))
            {
                nodePointer = value;
                VisitAllSuffixes(nodePointer, result);
                continue;
            }
            
            if (nodePointer.SuffixLink is null)
                continue;
            
            nodePointer = FindNextNodeFromSymbol(nodePointer, ch);
            VisitAllSuffixes(nodePointer, result);
        }
        
        return result;
    }
}