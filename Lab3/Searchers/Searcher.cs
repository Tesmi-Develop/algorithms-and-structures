namespace Searchers;

public static class Searcher
{
    public static List<int> Linear<T, TK>(T[] array, int n, TK key, Func<T, TK, int> comparer, out int steps)
    {
        steps = 0;
        List<int> result = [];

        for (var i = 0; i < n; i++)
        {
            steps++;
            if (comparer(array[i], key) == 0)
                result.Add(i);
        }

        return result;
    }

    public static int[] BuildStepTable(int n)
    {
        var steps = new List<int>();
        var step = (n + 1) / 2;
        var power = 2;

        while (step > 0)
        {
            steps.Add(step);
            var nextPower = power * 2;
            
            step = (n + power) / nextPower;
            power = nextPower;
        }

        return steps.ToArray();
    }
    
    public static List<int> UniformBinary<T, TK>(T[] array, int n, TK key, Func<T, TK, int> comparer, int[] steps, out int countSteps)
    {
        countSteps = 0;
        var index = 0;
        var found = -1;
        var result = new List<int>();
        
        foreach (var step in steps)
        {
            countSteps++;
            var target = index + step;
            
            if (target >= n)
                target = n - 1;
            
            var comparison = comparer(array[target], key);

            if (comparison == 0)
            {
                found = target;
                break;
            }
            
            if (comparison < 0) 
                index = target;
        }

        if (found == -1 && comparer(array[index], key) == 0)
            found = index;

        if (found == -1)
            return result;
        
        var startIndex = found;
        for (var i = found - 1; i >= 0 && comparer(array[i], key) == 0; i--)
        {
            countSteps++;
            startIndex = i;
        }

        for (var i = startIndex; i < n && comparer(array[i], key) == 0; i++)
        {
            countSteps++;
            result.Add(i);
        }

        return result;
    }

    public static List<int> UniformBinary<T, TK>(T[] array, int n, TK key, Func<T, TK, int> comparer, out int countSteps)
    {
        return UniformBinary(array, n, key, comparer, BuildStepTable(n), out countSteps);
    }
}