using System.Diagnostics;

namespace Deuces;

public class Evaluator
{
    protected readonly ILookupTable _table;

    // Static instances of possible Evaluators
    private static Evaluator? _instance;
    private static Evaluator? _canadianStudInstance;

    // ShortHands to access same instance from everywhere
    public static Evaluator Instance => _instance ??= new();
    public static Evaluator CanadianStud => _canadianStudInstance ??= new Evaluator(new CanadianStudLookupTable());


    public Evaluator() : this(new StdLookupTable())
    {
    }

    protected Evaluator(ILookupTable table)
    {
        _table = table;
    }

    /// <summary>
    /// This is the function that the user calls to get a hand rank. 
    /// Supports empty board, etc very flexible. No input validation 
    /// because that's cycles!
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="board"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public int Evaluate(int[] cards, int[] board)
    {
        Debug.Assert(cards.Length == 2);
        var cardsCount = cards.Length + board.Length;
        return cardsCount switch
        {
            5 => Five(new List<int> { cards[0], cards[1], board[0], board[1], board[2] }),
            6 => Six(new List<int> { cards[0], cards[1], board[0], board[1], board[2], board[3] }),
            7 => Seven(new List<int> { cards[0], cards[1], board[0], board[1], board[2], board[3], board[4] }),
            _ => throw new ArgumentOutOfRangeException(nameof(cardsCount), cardsCount, null)
        };
    }

    /// <summary>
    /// Returns the class of hand given the hand hand_rank
    /// returned from evaluate. 
    /// </summary>
    /// <param name="handRank"></param>
    /// <returns></returns>
    public int GetRankClass(int hr)
    {
        return _table.GetRankClass(hr);
    }

    /// <summary>
    /// Converts the integer class hand score into a human-readable string.
    /// </summary>
    /// <param name="classInt"></param>
    /// <returns></returns>
    public string ClassToString(int classInt)
    {
        return _table.RankClassToString(classInt);
    }

    /// <summary>
    /// Scales the hand rank score to the [0.0, 1.0] range.
    /// </summary>
    /// <param name="handRank"></param>
    /// <returns></returns>
    public double GetFiveCardRankPercentage(int handRank)
    {
        return handRank / (double)_table.MaxValue;
    }

    /// <summary>
    /// Performs an evaluation given cards in integer form, mapping them to
    /// a rank in the range [1, 7462], with lower ranks being more powerful.
    /// Variant of Cactus Kev's 5 card evaluator, though I saved a lot of memory
    /// space using a hash table and condensing some of the calculations. 
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    protected virtual int Five(List<int> cards)
    {
        int prime;
        // if flush
        if ((cards[0] & cards[1] & cards[2] & cards[3] & cards[4] & 0xF000) != 0)
        {
            var handOR = (cards[0] | cards[1] | cards[2] | cards[3] | cards[4]) >> 16;
            prime = Card.PrimeProductFromRankBits(handOR);
            return _table.FlushLookup[prime];
        }

        // otherwise
        prime = Card.PrimeProductFromHand(cards);
        return _table.UnSuitedLookup[prime];
    }

    /// <summary>
    /// Performs five_card_eval() on all (6 choose 5) = 6 subsets
    /// of 5 cards in the set of 6 to determine the best ranking, 
    /// and returns this ranking.
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private int Six(List<int> cards)
    {
        var minimum = _table.MaxValue;

        var temp = cards.ToList();
        for (var _ = 0; _ < 6; _++)
        {
            var item = temp[0];
            temp.RemoveAt(0);
            minimum = Math.Min(minimum, Five(temp));
            temp.Add(item);
        }

        return minimum;
    }

    /// <summary>
    /// Performs five_card_eval() on all (7 choose 5) = 21 subsets
    /// of 5 cards in the set of 7 to determine the best ranking, 
    /// and returns this ranking.
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private int Seven(List<int> cards)
    {
        var minimum = _table.MaxValue;

        var temp = cards.ToList();
        for (var i = 0; i < 6; i++)
        {
            var item1 = temp[i];
            temp.RemoveAt(i);
            for (var j = i + 1; j < 7; j++)
            {
                var item2 = temp[j - 1];
                temp.RemoveAt(j - 1);
                minimum = Math.Min(minimum, Five(temp));
                temp.Insert(j - 1, item2);
            }
            temp.Insert(i, item1);
        }

        return minimum;
    }
}
