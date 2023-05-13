namespace Deuces;

public interface ILookupTable
{
    IReadOnlyDictionary<int, int> FlushLookup { get; }
    IReadOnlyDictionary<int, int> UnSuitedLookup { get; }
    int GetRankClass(int handRank);
    string RankClassToString(int classInt);
    int MaxHighCard { get; }
}