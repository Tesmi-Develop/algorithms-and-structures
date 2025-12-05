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

    public int CompareTo(UserRecord other)
    {
        return Index.CompareTo(other.Index);
    }
}