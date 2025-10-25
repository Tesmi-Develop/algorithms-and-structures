namespace Record;

public struct UserRecord : IComparable<UserRecord>
{
    public CustomMyDate Date;
    public FullName FullName;
    public int Index;

    public override string ToString()
    {
        return $"{Date}\t{FullName}\t{Index}";
    }
    
    public string ToStringWithoutIndex()
    {
        return $"{Date}\t{FullName}";
    }

    public int CompareTo(UserRecord other)
    {
        var compare = Date.CompareTo(other.Date);
        return compare != 0 ? compare : other.FullName.CompareTo(FullName);
    }
}