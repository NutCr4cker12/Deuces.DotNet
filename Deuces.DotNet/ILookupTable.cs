namespace Deuces;

public interface ILookupTable
{
    IReadOnlyDictionary<int, int> FlushLookup { get; }
    IReadOnlyDictionary<int, int> UnSuitedLookup { get; }
    /// <summary>
    /// Returns the class of hand given the hand hand_rank
    /// returned from evaluate. 
    /// </summary>
    /// <param name="handRank"></param>
    /// <returns></returns>
    int GetRankClass(int handRank);
    /// <summary>
    /// Converts the integer class hand score into a human-readable string.
    /// </summary>
    /// <param name="classInt"></param>
    /// <returns></returns>
    string RankClassToString(int classInt);
    /// <summary>
    /// Maximum value that the implementation of this ILookupTable can have
    /// </summary>
    int MaxValue { get; }
}