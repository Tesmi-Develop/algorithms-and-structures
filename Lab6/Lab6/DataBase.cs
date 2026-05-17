using System.Text;

namespace Lab6;

public class Database<TKey, TData> 
    where TKey : IComparable<TKey>
{
    private const int MaxCapacity = 1000;
    
    private readonly TData[] _storage = new TData[MaxCapacity];
    public int Count { get; private set; }
    
    private readonly RedBlackTree<TKey, int> _index = new();
    private readonly Func<TData, TKey> _keySelector;

    public Database(Func<TData, TKey> keySelector)
    {
        _keySelector = keySelector;
    }

    public void Add(TData record)
    {
        if (Count >= MaxCapacity)
            throw new OverflowException("Overflow array.");
        
        var indexInArray = Count;
        _storage[indexInArray] = record;
        Count++;
        
        var key = _keySelector(record);
        _index.Insert(key, indexInArray);
    }

    public TData GetByIndex(int index)
    {
        return _storage[index];
    }

    public void Delete(TKey key, int indexInArray)
    {
        _index.Delete(key, indexInArray);
        var lastIndex = Count - 1;
        
        if (indexInArray != lastIndex)
        {
            var lastRecord = _storage[lastIndex];
            var lastKey = _keySelector(lastRecord);
            
            _index.Delete(lastKey, lastIndex);
            _storage[indexInArray] = lastRecord;
            _index.Insert(lastKey, indexInArray);
        }
        
        _storage[lastIndex] = default;
        Count--;
    }
    
    public void Delete(TKey key)
    {
        var node = _index.Search(key);
        var result = new int[node.Values.Count];
        var current = node.Values.Head;
        var index = 0;
        
        if (current != null)
        {
            var temp = current;
            do {
                result[index] = temp.Data;
                index++;
                temp = temp.Next;
            } while (temp != null && temp != current); 
        }
        
        _index.Delete(key);

        foreach (var indexInArray in result)
        {
            var lastIndex = Count - 1;
        
            if (indexInArray != lastIndex)
                _storage[indexInArray] = _storage[lastIndex];
        
            _storage[lastIndex] = default;
            Count--;
        }
    }

    public TData[] Find(TKey key)
    {
        var node = _index.Search(key);
        var result = new TData[node.Values.Count];
        var index = 0;
        
        if (node.Values.Empty) 
            return result;
        
        var current = node.Values.Head;

        if (current != null)
        {
            var temp = current;
            do {
                result[index] = _storage[temp.Data];
                index++;
                temp = temp.Next;
            } while (temp != null && temp != current); 
        }

        return result;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        
        for (var i = 0; i < Count; i++)
            sb.AppendLine($"[{i}] {_storage[i]}");
        
        return sb.ToString();
    }
}