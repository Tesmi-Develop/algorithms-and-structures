namespace Lab6;

public class Record : IComparable<TreeKey>
{
    public FullName FullName { get; }
    public int OrderId { get; }

    public Record(FullName fullName, int orderId)
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