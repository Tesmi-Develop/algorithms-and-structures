namespace Record;

public struct UserRecord : IComparable<UserRecord>
{
    public DateTime Date;
    public string Name;
    public int Index;

    public override string ToString()
    {
        return $"{Date:yyyy-MM-dd}\t{Name}\t{Index}";
    }
    
    public string ToStringWithoutIndex()
    {
        return $"{Date:yyyy-MM-dd}\t{Name}";
    }

    public int CompareTo(UserRecord other)
    {
        var compare = Date.CompareTo(other.Date);
        return compare != 0 ? compare : string.Compare(other.Name, Name, StringComparison.Ordinal);
    }
}