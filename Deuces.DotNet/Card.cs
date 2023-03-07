using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Deuces;

public static class Card
{
    // Static class that handles cards. We represent cards as 32-bit integers, so 
    // there is no object instantiation - they are just ints. Most of the bits are 
    // used, and have a specific meaning. See below: 
    //                                 Card:
    //                       bitrank     suit rank   prime
    //                 +--------+--------+--------+--------+
    //                 |xxxbbbbb|bbbbbbbb|cdhsrrrr|xxpppppp|
    //                 +--------+--------+--------+--------+
    //     1) p = prime number of rank (deuce=2,trey=3,four=5,...,ace=41)
    //     2) r = rank of card (deuce=0,trey=1,four=2,five=3,...,ace=12)
    //     3) cdhs = suit of card (bit turned on based on suit of card)
    //     4) b = bit turned on depending on rank of card
    //     5) x = unused
    // This representation will allow us to do very important things like:
    // - Make a unique prime prodcut for each hand
    // - Detect flushes
    // - Detect straights
    // and is also quite performant.
    //private static readonly int[] _ranks = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
    public const int INT_RANK_COUNT = 13;
    public static readonly IReadOnlyList<int> PRIMES = new ReadOnlyCollection<int>(new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 });
    public static readonly IReadOnlyList<char> STR_RANKS = new ReadOnlyCollection<char>("23456789TJQKA".ToCharArray());
    public static readonly IReadOnlyList<char> STR_SUITES = new ReadOnlyCollection<char>("shdc".ToCharArray());

    /// <summary>
    /// Converts Card string to binary integer representation of card, inspired by:
    /// http://www.suffecool.net/poker/evaluator.html
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    public static int New(string hand)
    {
        Debug.Assert(hand.Length == 2, "Card length must be exactly 2");
        return New(hand[0], hand[1]);
    }

    /// <summary>
    /// Converts Card string to binary integer representation of card, inspired by:
    /// http://www.suffecool.net/poker/evaluator.html
    /// </summary>
    /// <param name="rank"></param>
    /// <param name="suite"></param>
    /// <returns></returns>
    public static int New(char rank, char suite)
    {
        var rankInt = CharRankToIntRank(rank);
        var suiteInt = CharSuiteToIntRank(suite);
        var rankPrime = PRIMES[rankInt];

        var bitRank = 1 << rankInt << 16;
        var suiteBit = suiteInt << 12;
        var rankBit = rankInt << 8;

        return bitRank | suiteBit | rankBit | rankPrime;
    }

    private static int CharRankToIntRank(char rank) => rank switch
    {
        '2' => 0,
        '3' => 1,
        '4' => 2,
        '5' => 3,
        '6' => 4,
        '7' => 5,
        '8' => 6,
        '9' => 7,
        'T' => 8,
        'J' => 9,
        'Q' => 10,
        'K' => 11,
        'A' => 12,
        _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null)
    };

    private static int CharSuiteToIntRank(char suite) => suite switch
    {
        's' => 1,
        'h' => 2,
        'd' => 4,
        'c' => 8,
        _ => throw new ArgumentOutOfRangeException(nameof(suite), suite, null)
    };

    /// <summary>
    /// Returns the prime product using the bitrank (b)
    /// bits of the hand. Each 1 in the sequence is converted
    /// to the correct prime and multiplied in.
    /// Params:
    /// rankbits = a single 32-bit (only 13-bits set) integer representing 
    /// the ranks of 5 _different_ ranked cards 
    /// (5 of 13 bits are set)
    /// Primarily used for evaulating flushes and straights, 
    /// two occasions where we know the ranks are *ALL* different.
    /// Assumes that the input is in form (set bits):
    /// rankbits     
    /// +--------+--------+
    /// |xxxbbbbb|bbbbbbbb|
    /// +--------+--------+
    /// 
    /// </summary>
    /// <param name="rankBits"></param>
    /// <returns></returns>
    internal static int PrimeProductFromRankBits(int rankBits)
    {
        var product = 1;
        for (var i = 0; i < INT_RANK_COUNT; i++)
        {
            // if the ith bit is set
            if ((rankBits & (1 << i)) != 0)
            {
                product *= PRIMES[i];
            }
        }

        return product;
    }

    /// <summary>
    /// Expects a list of cards in integer form. 
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    internal static int PrimeProductFromHand(List<int> cardInts)
    {
        var product = 1;
        foreach (var c in cardInts)
        {
            product *= c & 0xFF;
        }

        return product;
    }
}
