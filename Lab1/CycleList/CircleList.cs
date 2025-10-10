using System.Runtime.CompilerServices;

namespace Lab1;

public class Node
{
    public Node(int data, Node next, Node previous)
    {
        Data = data;
        Next = next;
        Previous = previous;
    }

    public int Data { get; } 
    public Node Next { get; internal set; }
    public Node Previous { get; internal set; }
}

public class CircleList : IDisposable
{
    public bool Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Head is null;
    }
    
    public Node? Head { get; private set; }

    public static CircleList operator |(CircleList first, CircleList second)
    {
        return Union(first, second);
    }
    
    public static CircleList Union(CircleList first, CircleList second)
    {
        var result = new CircleList();

        if (first.Empty && second.Empty)
            return result;
        
        if (first.Empty)
        {
            var node = second.Head!;
            do
            {
                result.Add(node.Data);
                node = node.Next;
            } while (node != second.Head);

            return result;
        }
        
        if (second.Empty)
        {
            var node = first.Head!;
            do
            {
                result.Add(node.Data);
                node = node.Next;
            } while (node != first.Head);
            
            return result;
        }

        var firstFinished = false;
        var secondFinished = false;
        var node1 = first.Head;
        var node2 = second.Head;
        
        while (!firstFinished && !secondFinished)
        {
            if (node1!.Data > node2!.Data)
            {
                result.Add(node1.Data);
                node1 = node1.Next;
                if (node1 == first.Head) firstFinished = true;
                continue;
            }
            if (node1.Data < node2.Data)
            {
                result.Add(node2.Data);
                node2 = node2.Next;
                if (node2 == second.Head) secondFinished = true;
                continue;
            }
            
            result.Add(node1.Data);
            result.Add(node2.Data);
            node1 = node1.Next;
            node2 = node2.Next;
            if (node1 == first.Head) firstFinished = true;
            if (node2 == second.Head) secondFinished = true;
        }
        
        while (!firstFinished)
        {
            result.Add(node1!.Data);
            node1 = node1.Next;
            if (node1 == first.Head) firstFinished = true;
        }
        
        while (!secondFinished)
        {
            result.Add(node2!.Data);
            node2 = node2.Next;
            if (node2 == second.Head) secondFinished = true;
        }

        return result;
    }
    
    public CircleList() {}
    
    public CircleList(int[] data)
    {
        foreach (var value in data)
            Add(value);
    }

    public void Add(int data)
    {
        if (Empty)
        {
            AddNode(data, null);
            return;
        }
        
        if (data <= Head!.Previous.Data)
        {
            AddNode(data, Head.Previous);
            return;
        }

        if (data >= Head!.Data)
        {
            AddNode(data, Head.Previous);
            return;
        }
        
        var current = Head!;
        while (current.Next != Head && current.Next.Data > data)
            current = current.Next;
        
        AddNode(data, current);
    }

    public void RemoveAll(int data)
    {
        if (Empty) return;

        var current = Head!;
        do
        {
            var next = current.Next;
            if (current.Data == data)
                RemoveNode(current);
            
            current = next;
        } while (current != Head);
        
        if (Head.Data == data)
            RemoveNode(Head);
    }

    public void RemoveAllBefore(int data)
    {
        if (Empty) return;

        var current = Head!;
        do
        {
            if (current.Data == data)
                RemoveNode(current.Previous);
            
            current = current.Next;
        } while (current != Head);
    }

    public bool Search(int data, out Node? foundNode)
    {
        foundNode = null;
        if (Empty)
            return false;

        var current = Head;
        do
        {
            if (current!.Data == data) {
                foundNode = current;
                return true;
            }

            current = current.Next;

        } while (current != Head);
        
        foundNode = null;
        return false;
    }

    public void Print()
    {
        if (Empty)
        {
            Console.WriteLine("List is empty");
            return;
        }

        var current = Head!;

        do
        {
            Console.WriteLine(current.Data);
            current = current.Next;
        } while (current != Head);
    }
    
    public void Clear()
    {
        if (Empty)
            return;

        var current = Head!;
        do
        {
            var next = current.Next;
            current.Next = null!;
            current.Previous = null!;
            current = next;
        } while (current != Head);

        Head = null;
    }
    
    public void Dispose()
    {
        Clear();
    }
    
    private void AddNode(int data, Node? previous)
    {
        Node node;
        
        if (previous is null)
        {
            node = new Node(data, null!, null!);

            node.Next = node;
            node.Previous = node;
            Head = node;
            return;
        }

        node = new Node(data, previous.Next, previous);
        
        if (data >= Head!.Data && previous.Next == Head)
            Head = node;

        previous.Next.Previous = node;
        previous.Next = node;
    }
    
    private void RemoveNode(Node node)
    {
        if (node.Next == node)
        {
            Head = null;
            return;
        }

        if (node == Head)
            Head = Head.Next;

        node.Previous.Next = node.Next;
        node.Next.Previous = node.Previous;

        node.Next = null!;
        node.Previous = null!;
    }
}