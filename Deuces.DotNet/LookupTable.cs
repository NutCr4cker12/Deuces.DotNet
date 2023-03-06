using System.Collections.ObjectModel;

namespace Deuces;
/// <summary>
/// Number of Distinct Hand Values:
/// Straight Flush   10 
/// Four of a Kind   156      [(13 choose 2) * (2 choose 1)]
/// Full Houses      156      [(13 choose 2) * (2 choose 1)]
/// Flush            1277     [(13 choose 5) - 10 straight flushes]
/// Straight         10 
/// Three of a Kind  858      [(13 choose 3) * (3 choose 1)]
/// Two Pair         858      [(13 choose 3) * (3 choose 2)]
/// One Pair         2860     [(13 choose 4) * (4 choose 1)]
/// High Card      + 1277     [(13 choose 5) - 10 straights]
/// -------------------------
/// TOTAL            7462
/// Here we create a lookup table which maps:
/// 5 card hand's unique prime product =&gt; rank in range [1, 7462]
/// Examples:
/// * Royal flush (best hand possible)          =&gt; 1
/// * 7-5-4-3-2 unsuited (worst hand possible)  =&gt; 7462
/// </summary>
public class LookupTable
{
    internal const int MAX_STRAIGHT_FLUSH = 10;
    internal const int MAX_FOUR_OF_A_KIND = 166;
    internal const int MAX_FULL_HOUSE = 322;
    internal const int MAX_FLUSH = 1599;
    internal const int MAX_STRAIGHT = 1609;
    internal const int MAX_THREE_OF_A_KIND = 2467;
    internal const int MAX_TWO_PAIR = 3325;
    internal const int MAX_PAIR = 6185;
    internal const int MAX_HIGH_CARD = 7462;

    internal readonly IReadOnlyDictionary<int, int> FlushLookup;
    internal readonly IReadOnlyDictionary<int, int> UnSuitedLookup;
    /// <summary>
    /// Calculates lookup tables
    /// </summary>
    public LookupTable()
    {
        var flushesList = GetFlushesList();
        FlushLookup = Flushes(flushesList);

        // we can reuse these bit sequences for straights
        // and high cards since they are inherently related
        // and differ only by context 
        UnSuitedLookup = Multiples(flushesList);
    }

    internal static int MaxToRankClass(int rank) => rank switch
    {
        MAX_STRAIGHT_FLUSH => 1,
        MAX_FOUR_OF_A_KIND => 2,
        MAX_FULL_HOUSE => 3,
        MAX_FLUSH => 4,
        MAX_STRAIGHT => 5,
        MAX_THREE_OF_A_KIND => 6,
        MAX_TWO_PAIR => 7,
        MAX_PAIR => 8,
        MAX_HIGH_CARD => 9,
        _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null)
    };

    internal static string RankClassToString(int rank) => rank switch
    {
        1 => "Straight Flush",
        2 => "Four of a Kind",
        3 => "Full House",
        4 => "Flush",
        5 => "Straight",
        6 => "Three of a Kind",
        7 => "Two Pair",
        8 => "Pair",
        9 => "High Card",
        _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null)
    };

    private readonly int[] _straightFlushes = new int[]
    {
        7936, // int('0b1111100000000', 2), # royal flush
        3968, // int('0b111110000000', 2),
        1984, // int('0b11111000000', 2),
        992, // int('0b1111100000', 2),
        496, // int('0b111110000', 2),
        248, // int('0b11111000', 2),
        124, // int('0b1111100', 2),
        62, // int('0b111110', 2),
        31, // int('0b11111', 2),
        4111, //nt('0b1000000001111', 2) # 5 high
    };

    /// <summary>
    /// Straight flushes and flushes. 
    /// Lookup is done on 13 bit integer (2^13 &gt; 7462):
    /// xxxbbbbb bbbbbbbb =&gt; integer hand index
    /// </summary>
    /// <returns></returns>
    private IReadOnlyDictionary<int, int> Flushes(IReadOnlyList<int> flushesArr)
    {
        var flushes = new Dictionary<int, int>(1287);

        // now add to the lookup map:
        // start with straight flushes and the rank of 1
        // since it is the best hand in poker
        // rank 1 = Royal Flush!
        var rank = 1;
        foreach (var sf in _straightFlushes)
        {
            var primeProduct = Card.PrimeProductFromRankBits(sf);
            flushes[primeProduct] = rank++;
        }

        // we start the counting for flushes on max full house, which
        // is the worst rank that a full house can have (2,2,2,3,3)
        rank = MAX_FULL_HOUSE + 1;
        foreach (var f in flushesArr)
        {
            var primeProduct = Card.PrimeProductFromRankBits(f);
            flushes[primeProduct] = rank++;
        }

        return new ReadOnlyDictionary<int, int>(flushes); // TODO Does this have an overhead ??
    }

    /// <summary>
    /// Pair, Two Pair, Three of a Kind, Full House, and 4 of a Kind.
    /// </summary>
    /// <param name="readOnlyList"></param>
    /// <returns></returns>
    private IReadOnlyDictionary<int, int> Multiples(IReadOnlyList<int> readOnlyList)
    {
        var lookUp = StraightAndHighCards(readOnlyList);

        var backwards_ranks = new[] { 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };

        // 1) Four of a Kind
        var rank = MAX_STRAIGHT_FLUSH + 1;

        // for each choice of a set of four rank
        foreach (var i in backwards_ranks)
        {
            // and for each possible kicker rank
            foreach (var kicker in backwards_ranks)
            {
                if (kicker == i) continue;

                var product = (int)Math.Pow(Card.PRIMES[i], 4) * Card.PRIMES[kicker];
                lookUp[product] = rank++;
            }
        }


        // 2) Full House
        rank = MAX_FOUR_OF_A_KIND + 1;

        // for each three of a kind
        foreach (var i in backwards_ranks)
        {
            // and for each choice of pair rank
            foreach (var pr in backwards_ranks)
            {
                if (pr == i) continue;

                var product = (int)(Math.Pow(Card.PRIMES[i], 3) * Math.Pow(Card.PRIMES[pr], 2));
                lookUp[product] = rank++;
            }
        }


        // 3) Three of a Kind
        rank = MAX_STRAIGHT + 1;

        // pick three of one rank
        foreach (var r in backwards_ranks)
        {
            // Generate Combinations
            for (var comb1 = 0; comb1 < backwards_ranks.Length - 1; comb1++)
            {
                var c1 = backwards_ranks[comb1];
                if (c1 == r) continue;

                for (var comb2 = comb1 + 1; comb2 < backwards_ranks.Length; comb2++)
                {
                    var c2 = backwards_ranks[comb2];
                    if (c2 == r) continue;

                    var product = (int)Math.Pow(Card.PRIMES[r], 3) * Card.PRIMES[c1] * Card.PRIMES[c2];
                    lookUp[product] = rank++;
                }
            }
        }

        // 4) Two Pair
        rank = MAX_THREE_OF_A_KIND + 1;

        // Generate Combinations
        for (var comb1 = 0; comb1 < backwards_ranks.Length - 1; comb1++)
        {
            for (var comb2 = comb1 + 1; comb2 < backwards_ranks.Length; comb2++)
            {
                var pair1 = backwards_ranks[comb1];
                var pair2 = backwards_ranks[comb2];

                foreach (var kicker in backwards_ranks)
                {
                    if (kicker == pair1 || kicker == pair2) continue;

                    var product = (int)(Math.Pow(Card.PRIMES[pair1], 2) * Math.Pow(Card.PRIMES[pair2], 2)) * Card.PRIMES[kicker];
                    lookUp[product] = rank++;
                }
            }
        }

        // 5) Pair
        rank = MAX_TWO_PAIR + 1;

        // choose a pair
        foreach (var pairrank in backwards_ranks)
        {
            for (var comb1 = 0; comb1 < backwards_ranks.Length - 2; comb1++)
            {
                var k1 = backwards_ranks[comb1];
                if (k1 == pairrank) continue;
                for (var comb2 = comb1 + 1; comb2 < backwards_ranks.Length - 1; comb2++)
                {
                    var k2 = backwards_ranks[comb2];
                    if (k2 == pairrank) continue;
                    for (var comb3 = comb2 + 1; comb3 < backwards_ranks.Length; comb3++)
                    {
                        var k3 = backwards_ranks[comb3];
                        if (k3 == pairrank) continue;

                        var product = (int)Math.Pow(Card.PRIMES[pairrank], 2) * Card.PRIMES[k1] * Card.PRIMES[k2] * Card.PRIMES[k3];
                        lookUp[product] = rank++;
                    }
                }
            }
        }

        return new ReadOnlyDictionary<int, int>(lookUp); // TODO Does this have an overhead ??
    }

    /// <summary>
    /// Unique five card sets. Straights and highcards. 
    /// Reuses bit sequences from flush calculations.
    /// </summary>
    /// <param name="straightFlushes"></param>
    /// <param name="highCards"></param>
    /// <returns></returns>
    private Dictionary<int, int> StraightAndHighCards(IReadOnlyList<int> highCards)
    {
        var res = new Dictionary<int, int>(4898);
        var rank = MAX_FLUSH + 1;

        // "Straights"
        foreach (var sf in _straightFlushes)
        {
            var primeProduct = Card.PrimeProductFromRankBits(sf);
            res[primeProduct] = rank++;
        }

        rank = MAX_PAIR + 1;
        foreach (var h in highCards)
        {
            var primeProduct = Card.PrimeProductFromRankBits(h);
            res[primeProduct] = rank++;
        }

        return res;
    }

    private IReadOnlyList<int> GetFlushesList()
    {
        // now we'll dynamically generate all the other
        // flushes (including straight flushes)
        var flushesArr = new List<int>(1287);
        using var gen = GetLexoGraphicallyNextBitSequence(0b11111);

        // 1277 = number of high cards
        // 1277 + len(str_flushes) is number of hands with all cards unique rank

        // we also iterate over SFs
        for (var i = 0; i < 1277 + _straightFlushes.Length - 1; i++)
        {
            // pull the next flush pattern from our generator
            gen.MoveNext();
            var f = gen.Current;

            //if this flush matches perfectly any
            // straight flush, do not add it
            var notSF = true;
            foreach (var sf in _straightFlushes)
            {
                //if f XOR sf == 0, then bit pattern 
                // is same, and we should not add
                if ((f ^ sf) == 0)
                {
                    notSF = false;
                    break;
                }
            }

            if (notSF)
            {
                flushesArr.Add(f);
            }
        }

        // we started from the lowest straight pattern, now we want to start ranking from
        // the most powerful hands, so we reverse
        flushesArr.Reverse();
        return flushesArr;
    }

    /// <summary>
    /// Bit hack from here:
    /// http://www-graphics.stanford.edu/~seander/bithacks.html#NextBitPermutation
    /// Generator even does this in poker order rank 
    /// so no need to sort when done! Perfect.
    /// </summary>
    /// <param name="bits"></param>
    /// <returns></returns>
    private IEnumerator<int> GetLexoGraphicallyNextBitSequence(int bits)
    {
        var t = (bits | (bits - 1)) + 1;
        var next = t | ((((t & -t) / (bits & -bits)) >> 1) - 1);
        yield return next;
        while (true)
        {
            t = (next | (next - 1)) + 1;
            next = t | ((((t & -t) / (next & -next)) >> 1) - 1);
            yield return next;
        }
    }
}
