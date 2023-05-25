using System.Diagnostics;

namespace Deuces;
public class Evaluator
{
    private readonly LookupTable _table;

    public Evaluator()
    {
        _table = new LookupTable();
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
            5 => Five(stackalloc int[] { cards[0], cards[1], board[0], board[1], board[2] }),
            6 => Six(stackalloc int[] { cards[0], cards[1], board[0], board[1], board[2], board[3] }),
            7 => Seven(stackalloc int[] { cards[0], cards[1], board[0], board[1], board[2], board[3], board[4] }),
            _ => throw new ArgumentOutOfRangeException(nameof(cardsCount), cardsCount, null)
        };
    }

    /// <summary>
    /// Returns the class of hand given the hand hand_rank
    /// returned from evaluate. 
    /// </summary>
    /// <param name="handRank"></param>
    /// <param name="hr"></param>
    /// <returns></returns>
    public int GetRankClass(int hr)
    {
        if (hr is >= 0 and <= LookupTable.MAX_STRAIGHT_FLUSH)
            return LookupTable.MaxToRankClass(LookupTable.MAX_STRAIGHT_FLUSH);
        if (hr <= LookupTable.MAX_FOUR_OF_A_KIND)
            return LookupTable.MaxToRankClass(LookupTable.MAX_FOUR_OF_A_KIND);
        if (hr <= LookupTable.MAX_FULL_HOUSE)
            return LookupTable.MaxToRankClass(LookupTable.MAX_FULL_HOUSE);
        if (hr <= LookupTable.MAX_FLUSH)
            return LookupTable.MaxToRankClass(LookupTable.MAX_FLUSH);
        if (hr <= LookupTable.MAX_STRAIGHT)
            return LookupTable.MaxToRankClass(LookupTable.MAX_STRAIGHT);
        if (hr <= LookupTable.MAX_THREE_OF_A_KIND)
            return LookupTable.MaxToRankClass(LookupTable.MAX_THREE_OF_A_KIND);
        if (hr <= LookupTable.MAX_TWO_PAIR)
            return LookupTable.MaxToRankClass(LookupTable.MAX_TWO_PAIR);
        if (hr <= LookupTable.MAX_PAIR)
            return LookupTable.MaxToRankClass(LookupTable.MAX_PAIR);
        if (hr <= LookupTable.MAX_HIGH_CARD)
            return LookupTable.MaxToRankClass(LookupTable.MAX_HIGH_CARD);

        throw new Exception("Invalid hand rank, cannot return rank class");
    }

    /// <summary>
    /// Converts the integer class hand score into a human-readable string.
    /// </summary>
    /// <param name="classInt"></param>
    /// <returns></returns>
    public string ClassToString(int classInt)
    {
        return LookupTable.RankClassToString(classInt);
    }

    /// <summary>
    /// Scales the hand rank score to the [0.0, 1.0] range.
    /// </summary>
    /// <param name="handRank"></param>
    /// <returns></returns>
    public double GetFiveCardRankPercentage(int handRank)
    {
        return handRank / (double)LookupTable.MAX_HIGH_CARD;
    }

    /// <summary>
    /// Performs an evaluation given cards in integer form, mapping them to
    /// a rank in the range [1, 7462], with lower ranks being more powerful.
    /// Variant of Cactus Kev's 5 card evaluator, though I saved a lot of memory
    /// space using a hash table and condensing some of the calculations. 
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private int Five(ReadOnlySpan<int> cards)
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
    private int Six(ReadOnlySpan<int> cards)
    {
        var minimum = LookupTable.MAX_HIGH_CARD;

        Span<int> temp = stackalloc int[5];
        for (var i = 0; i < 6; i++)
        {
            // From 0 to i
            if (i != 0)
            {
                var first = cards[..i];
                first.CopyTo(temp[..i]);
            }

            // From i + 1 to end
            if (i != 5)
            {
                var last = cards[(i + 1)..];
                last.CopyTo(temp[i..]);
            }

            minimum = Math.Min(minimum, Five(temp));
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
    private int Seven(ReadOnlySpan<int> cards)
    {
        var minimum = LookupTable.MAX_HIGH_CARD;

        Span<int> temp = stackalloc int[6];
        for (var i = 0; i < 7; i++)
        {
            // From 0 to i
            if (i != 0)
            {
                var first = cards[..i];
                first.CopyTo(temp[..i]);
            }

            // From i + 1 to end
            if (i != 5)
            {
                var last = cards[(i + 1)..];
                last.CopyTo(temp[i..]);
            }

            minimum = Math.Min(minimum, Six(temp));
        }

        return minimum;
    }
}
