namespace Lab6;

public struct TreeKey : IComparable<TreeKey>
{
    public FullName FullName;
    public int OrderId;

    public TreeKey(FullName fullName, int orderId)
    {
        FullName = fullName;
        OrderId = orderId;
    }

    public int CompareTo(TreeKey other)
    {
        var fullNameComparison = FullName.CompareTo(other.FullName);
        return fullNameComparison != 0 ? fullNameComparison : OrderId.CompareTo(other.OrderId);
    }
    
    public override string ToString()
    {
        return $"{FullName}, {OrderId}";
    }
}