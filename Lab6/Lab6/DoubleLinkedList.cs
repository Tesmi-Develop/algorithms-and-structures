namespace Lab6;

public class DoubleLinkedList<T> where T : IComparable<T>
{
    public bool Empty => Head is null;
    public int Count { get; private set; }
    
    public Node? Head { get; private set; }
    public Node? Tail { get; private set; }

    public class Node
    {
        public Node(T data, Node? next, Node? previous)
        {
            Data = data;
            Next = next;
            Previous = previous;
        }

        public T Data { get; }
        public Node? Next { get; internal set; }
        public Node? Previous { get; internal set; }
    }
    
    public DoubleLinkedList() { }

    public DoubleLinkedList(T[] data)
    {
        foreach (var value in data)
            Add(value);
    }
    
    public void Add(T data)
    {
        if (Empty)
        {
            AddFirst(data);
            return;
        }
        
        if (data.CompareTo(Tail!.Data) <= 0)
        {
            AddAfter(Tail, data);
            return;
        }
        
        if (data.CompareTo(Tail!.Data) >= 0)
        {
            AddFirst(data);
            return;
        }
        
        var current = Head;
        while (current!.Next != null && current.Next.Data.CompareTo(data) > 0)
            current = current.Next;

        AddAfter(current, data);
    }

    private void AddFirst(T data)
    {
        var newNode = new Node(data, Head, null);
        Head?.Previous = newNode;

        Head = newNode;
        Tail ??= newNode;
        Count++;
    }

    private void AddAfter(Node previous, T data)
    {
        var newNode = new Node(data, previous.Next, previous);
        
        if (previous.Next != null)
            previous.Next.Previous = newNode;
        else
            Tail = newNode;

        previous.Next = newNode;
        Count++;
    }

    public bool Remove(T data)
    {
        var current = Head;
        while (current != null)
        {
            var next = current.Next;
            if (current.Data.CompareTo(data) == 0)
            {
                RemoveNode(current);
                return true;
            }

            current = next;
        }

        return false;
    }

    public void RemoveAllBefore(T data)
    {
        var current = Head;
        while (current != null)
        {
            if (current.Data.CompareTo(data) == 0 && current.Previous != null)
                RemoveNode(current.Previous);
            
            current = current.Next;
        }
    }

    private void RemoveNode(Node node)
    {
        if (node.Previous != null)
            node.Previous.Next = node.Next;
        else
            Head = node.Next;

        if (node.Next != null)
            node.Next.Previous = node.Previous;
        else
            Tail = node.Previous;

        node.Next = null;
        node.Previous = null;
        Count--;
    }

    public bool TryFind(T data, out Node? foundNode)
    {
        var current = Head;
        while (current != null)
        {
            if (current.Data.CompareTo(data) == 0)
            {
                foundNode = current;
                return true;
            }
            current = current.Next;
        }
        foundNode = null;
        return false;
    }

    public static DoubleLinkedList<T> Union(DoubleLinkedList<T> first, DoubleLinkedList<T> second)
    {
        var result = new DoubleLinkedList<T>();
        var node1 = first.Head;
        var node2 = second.Head;

        while (node1 != null && node2 != null)
        {
            if (node1.Data.CompareTo(node2.Data) >= 0)
            {
                result.Add(node1.Data);
                node1 = node1.Next;
            }
            else
            {
                result.Add(node2.Data);
                node2 = node2.Next;
            }
        }

        while (node1 != null)
        {
            result.Add(node1.Data);
            node1 = node1.Next;
        }

        while (node2 != null)
        {
            result.Add(node2.Data);
            node2 = node2.Next;
        }

        return result;
    }

    public void Clear()
    {
        var current = Head;
        while (current != null)
        {
            var next = current.Next;
            current.Next = null;
            current.Previous = null;
            current = next;
        }
        Head = null;
        Tail = null;
    }
}