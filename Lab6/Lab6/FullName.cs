namespace Lab6;

public struct FullName : IComparable<FullName>
{
    public string LastName;
    public string FirstName;
    public string MiddleName;

    public FullName()
    {
        LastName = string.Empty;
        FirstName = string.Empty;
        MiddleName = string.Empty;
    }
    
    public FullName(string lastName, string firstName, string middleName)
    {
        LastName = lastName;
        FirstName = firstName;
        MiddleName = middleName;
    }
    
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