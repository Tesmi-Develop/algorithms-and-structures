namespace Record;

public struct FullName : IComparable<FullName>
{
    public string LastName;
    public string FirstName;
    public string MiddleName;
    
    public override string ToString()
    {
        return $"{LastName} {FirstName} {MiddleName}";
    }
    
    public int CompareTo(FullName other)
    {
        var lastNameCompare = string.Compare(LastName, other.LastName, StringComparison.OrdinalIgnoreCase);
        if (lastNameCompare != 0)
            return lastNameCompare;
        
        var firstNameCompare = string.Compare(FirstName, other.FirstName, StringComparison.OrdinalIgnoreCase);
        if (firstNameCompare != 0)
            return firstNameCompare;
        
        return string.Compare(MiddleName, other.MiddleName, StringComparison.OrdinalIgnoreCase);
    }
}