namespace Sorting;

public static class Sorting
{
    public static void InsertionSort<T>(T[] array, int length) where T : IComparable<T>
    {
        for (var i = 1; i < array.Length; i++)
        {
            var key = array[i];
            var j = i - 1;
            
            for (;j >= 0 && array[j].CompareTo(key) > 0; j--)
                array[j + 1] = array[j];
            
            array[j + 1] = key;
        }
    }
    public static void NaturalMergeSort<T>(T[] array, int length) where T : IComparable<T>
    {
        if (length <= 1)
            return;
        
        var temp = new T[length];
        bool merged;

        do
        {
            merged = false;
            var read = 0;

            while (read < length)
            {
                var start1 = read;
                var end1 = FindRunEnd(array, length, start1);
                read = end1 + 1;

                if (read >= length)
                {
                    CopyRange(array, temp, start1, end1);
                    break;
                }
                
                var end2 = FindRunEnd(array, length, read);
                read = end2 + 1;

                Merge(array, temp, start1, end1, end2);
                merged = true;
            }
            
            if (merged)
                Array.Copy(temp, array, length);
            
        } while(merged);
    }

    private static int FindRunEnd<T>(T[] array, int length, int start) where T : IComparable<T>
    {
        var i = start;
        while (i + 1 < length && array[i].CompareTo(array[i + 1]) <= 0)
            i++;
        
        return i;
    }
    
    private static void Merge<T>(T[] source, T[] destination, int left, int mid, int right) where T : IComparable<T>
    {
        var i = left;
        var j = mid + 1;
        var k = left;

        while (i <= mid && j <= right)
        {
            if (source[i].CompareTo(source[j]) <= 0) {
                destination[k++] = source[i++];
                continue;
            }
            
            destination[k++] = source[j++];
        }

        while (i <= mid)
            destination[k++] = source[i++];

        while (j <= right)
            destination[k++] = source[j++];
    }
    
    private static void CopyRange<T>(T[] source, T[] destination, int start, int end)
    {
        for (var i = start; i <= end; i++)
            destination[i] = source[i];
    }
}