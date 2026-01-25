namespace Searchers;

public static class TurboBoyerMoore
{
    public static int Search(string text, string pattern)
    {
        var result = 0;
        var patternLength = pattern.Length;
        var textLength = text.Length;

        if (patternLength == 0 || patternLength > textLength) 
            return result;
        
        var badCharacterTable = BuildBadCharacterTable(pattern);
        var goodSuffixTable = BuildGoodSuffixTable(pattern);

        var patternOffset = 0;
        var matchedFromPrevStep = 0;
        var shiftFromPrevStep = 0;

        while (patternOffset <= textLength - patternLength)
        {
            var patternIdx = patternLength - 1;

            while (patternIdx >= 0 && pattern[patternIdx] == text[patternOffset + patternIdx])
            {
                patternIdx--;
                
                if (matchedFromPrevStep != 0 && patternIdx == patternLength - 1 - shiftFromPrevStep)
                    patternIdx -= matchedFromPrevStep;
            }

            if (patternIdx < 0)
            {
                result++;
                
                var suffixShift = goodSuffixTable[0];
                patternOffset += suffixShift;
                
                matchedFromPrevStep = patternLength - suffixShift;
                shiftFromPrevStep = suffixShift;
                continue;
            }

            var mismatchCharInText = text[patternOffset + patternIdx];
            var lastOccurrenceInPattern = badCharacterTable.GetValueOrDefault(mismatchCharInText, -1);
            var badCharShift = patternIdx - lastOccurrenceInPattern;
            var goodSuffixShift = goodSuffixTable[patternIdx + 1];
            var actualShift = Math.Max(badCharShift, goodSuffixShift);
                
            if (matchedFromPrevStep != 0 && patternIdx < patternLength - 1 - shiftFromPrevStep)
            {
                var turboShift = matchedFromPrevStep - (patternLength - 1 - patternIdx);
                actualShift = Math.Max(actualShift, turboShift);
            }
                
            matchedFromPrevStep = 
                actualShift >= goodSuffixShift ? 
                    Math.Min(patternLength - actualShift, patternLength - 1 - patternIdx) : 
                    actualShift < matchedFromPrevStep ? 
                        0 : 
                        Math.Min(matchedFromPrevStep, patternLength - actualShift);

            shiftFromPrevStep = actualShift;
            patternOffset += actualShift;
        }

        return result;
    }

    private static Dictionary<char, int> BuildBadCharacterTable(string pattern)
    {
        var table = new Dictionary<char, int>();
        for (var i = 0; i < pattern.Length; i++)
            table[pattern[i]] = i;
        
        return table;
    }

    private static int[] BuildGoodSuffixTable(string pattern)
    {
        var patternLength = pattern.Length;
        var suffixLengths = ComputeSuffixHelper(pattern);
        var table = new int[patternLength + 1];

        for (var i = 0; i < patternLength; i++) 
            table[i] = patternLength;

        var j = 0;
        for (var i = patternLength - 1; i >= -1; i--)
        {
            if (i != -1 && suffixLengths[i] != i + 1) 
                continue;
            
            while (j < patternLength - 1 - i)
            {
                if (table[j] == patternLength) table[j] = patternLength - 1 - i;
                j++;
            }
        }

        for (var i = 0; i <= patternLength - 2; i++)
            table[patternLength - 1 - suffixLengths[i]] = patternLength - 1 - i;

        return table;
    }

    private static int[] ComputeSuffixHelper(string pattern)
    {
        var patternLength = pattern.Length;
        var suffixes = new int[patternLength];
        suffixes[patternLength - 1] = patternLength;
        
        var rightBoundary = patternLength - 1;
        var lastMatchIdx = 0;

        for (var i = patternLength - 2; i >= 0; i--)
        {
            if (i > rightBoundary && suffixes[i + patternLength - 1 - lastMatchIdx] < i - rightBoundary)
            {
                suffixes[i] = suffixes[i + patternLength - 1 - lastMatchIdx];
                continue;
            }

            if (i < rightBoundary) 
                rightBoundary = i;
            
            lastMatchIdx = i;
            
            while (rightBoundary >= 0 && pattern[rightBoundary] == pattern[rightBoundary + patternLength - 1 - lastMatchIdx])
                rightBoundary--;
            
            suffixes[i] = lastMatchIdx - rightBoundary;
        }
        return suffixes;
    }
}