using System.Collections.ObjectModel;

namespace Deuces;

internal abstract class LookupTableBase
{
    public abstract int MaxValue { get; }
}

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
internal class StdLookupTable : LookupTableBase, ILookupTable
{
    protected static readonly int MAX_STRAIGHT_FLUSH = 10;
    protected static readonly int MAX_FOUR_OF_A_KIND = 166; // 156
    protected static readonly int MAX_FULL_HOUSE = 322; // 156
    protected static readonly int MAX_FLUSH = 1599;
    protected static readonly int MAX_STRAIGHT = 1609; // 10
    protected static readonly int MAX_THREE_OF_A_KIND = 2467; // 858
    protected static readonly int MAX_TWO_PAIR = 3325; // 858
    protected static readonly int MAX_PAIR = 6185; // 2860
    protected static readonly int MAX_HIGH_CARD = 7462; // 1277

    public override int MaxValue { get; } = MAX_HIGH_CARD;
    public IReadOnlyDictionary<int, int> FlushLookup { get; init; }
    public IReadOnlyDictionary<int, int> UnSuitedLookup { get; }
    protected readonly int[] _backWardRanks = { 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
    private readonly int[] _straightFlushes =
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
    /// Create a standard lookup table for the most common poker hands:
    /// 1) Straight flush
    /// 2) Four of a kind
    /// 3) Full House
    /// 4) Flush
    /// 5) Straight
    /// 6) Three of a kind
    /// 7) Two pairs
    /// 8) Pair
    /// 9) High card
    /// </summary>
    public StdLookupTable()
    {
        var flushesList = GetFlushesList();
        FlushLookup = Flushes(flushesList);

        // we can reuse these bit sequences for straights
        // and high cards since they are inherently related
        // and differ only by context 
        var lookUp = new Dictionary<int, int>(4898);
        UnSuitedLookup = Multiples(lookUp, flushesList);
    }

    public virtual int GetRankClass(int hr)
    {
        if (hr < 0) throw new Exception("Invalid hand rank. Rank must be at least 0.");
        if (hr <= MAX_STRAIGHT_FLUSH) return 1;
        if (hr <= MAX_FOUR_OF_A_KIND) return 2;
        if (hr <= MAX_FULL_HOUSE) return 3;
        if (hr <= MAX_FLUSH) return 4;
        if (hr <= MAX_STRAIGHT) return 5;
        if (hr <= MAX_THREE_OF_A_KIND) return 6;
        if (hr <= MAX_TWO_PAIR) return 7;
        if (hr <= MAX_PAIR) return 8;
        if (hr <= MAX_HIGH_CARD) return 9;
        throw new Exception("Invalid hand rank, cannot return rank class");
    }

    public virtual string RankClassToString(int rank) => rank switch
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

    /// <summary>
    /// Populates the lookUp table with Four of a kind, Full house, Straights, Three of a kind, Two pairs, Pairs and High cards
    /// </summary>
    /// <param name="lookUp"></param>
    /// <param name="flushes"></param>
    /// <returns></returns>
    protected virtual IReadOnlyDictionary<int, int> Multiples(IDictionary<int, int> lookUp, IReadOnlyList<int> flushes)
    {
        // 1) Four of a Kind
        PopulateFourOfAKindLUT(lookUp, MAX_STRAIGHT_FLUSH + 1);

        // 2) Full House
        PopulateFullHouseLUT(lookUp, MAX_FOUR_OF_A_KIND + 1);

        // ...Flushes are in separate lookUp table

        // 3) Straights
        PopulateStraightsLUT(lookUp, MAX_FLUSH + 1);

        // 4) Three of a Kind
        PopulateThreeOfAKindLUT(lookUp, MAX_STRAIGHT + 1);

        // 5) Two Pair
        PopulateTwoPairsLUT(lookUp, MAX_THREE_OF_A_KIND + 1);

        // 6) Pair
        PopulatePairLUT(lookUp, MAX_TWO_PAIR + 1);

        // 7) High Cards
        PopulateHighCardLUT(lookUp, flushes, MAX_PAIR + 1);

        return new ReadOnlyDictionary<int, int>(lookUp); // TODO Does this have an overhead ??
    }

    /// <summary>
    /// Populates the lookUp table with Four a kind hands
    /// </summary>
    /// <param name="lookUp"></param>
    /// <param name="rank"></param>
    protected void PopulateFourOfAKindLUT(IDictionary<int, int> lookUp, int rank)
    {
        // for each choice of a set of four rank
        foreach (var i in _backWardRanks)
        {
            // and for each possible kicker rank
            foreach (var kicker in _backWardRanks)
            {
                if (kicker == i) continue;

                var product = (int)Math.Pow(Card.PRIMES[i], 4) * Card.PRIMES[kicker];
                lookUp.Add(product, rank++);
            }
        }
    }

    /// <summary>
    /// Populates the lookUp table with Full house hands
    /// </summary>
    /// <param name="lookUp"></param>
    /// <param name="rank"></param>
    protected void PopulateFullHouseLUT(IDictionary<int, int> lookUp, int rank)
    {
        // for each three of a kind
        foreach (var i in _backWardRanks)
        {
            // and for each choice of pair rank
            foreach (var pr in _backWardRanks)
            {
                if (pr == i) continue;

                var product = (int)(Math.Pow(Card.PRIMES[i], 3) * Math.Pow(Card.PRIMES[pr], 2));
                lookUp.Add(product, rank++);
            }
        }
    }

    /// <summary>
    /// Populates the lookUp table with Straight hands
    /// </summary>
    /// <param name="lookUp"></param>
    /// <param name="rank"></param>
    protected void PopulateStraightsLUT(IDictionary<int, int> lookUp, int rank)
    {
        // "Straights"
        foreach (var sf in _straightFlushes)
        {
            var primeProduct = Card.PrimeProductFromRankBits(sf);
            lookUp.Add(primeProduct, rank++);
        }
    }

    /// <summary>
    /// Populates the lookUp table with Three of a kind hands
    /// </summary>
    /// <param name="lookUp"></param>
    /// <param name="rank"></param>
    protected void PopulateThreeOfAKindLUT(IDictionary<int, int> lookUp, int rank)
    {
        // pick three of one rank
        foreach (var r in _backWardRanks)
        {
            // Generate Combinations
            for (var comb1 = 0; comb1 < _backWardRanks.Length - 1; comb1++)
            {
                var c1 = _backWardRanks[comb1];
                if (c1 == r) continue;

                for (var comb2 = comb1 + 1; comb2 < _backWardRanks.Length; comb2++)
                {
                    var c2 = _backWardRanks[comb2];
                    if (c2 == r) continue;

                    var product = (int)Math.Pow(Card.PRIMES[r], 3) * Card.PRIMES[c1] * Card.PRIMES[c2];
                    lookUp.Add(product, rank++);
                }
            }
        }
    }

    /// <summary>
    /// Populates the lookUp table with Two pair hands
    /// </summary>
    /// <param name="lookUp"></param>
    /// <param name="rank"></param>
    protected void PopulateTwoPairsLUT(IDictionary<int, int> lookUp, int rank)
    {
        // Generate Combinations
        for (var comb1 = 0; comb1 < _backWardRanks.Length - 1; comb1++)
        {
            for (var comb2 = comb1 + 1; comb2 < _backWardRanks.Length; comb2++)
            {
                var pair1 = _backWardRanks[comb1];
                var pair2 = _backWardRanks[comb2];

                foreach (var kicker in _backWardRanks)
                {
                    if (kicker == pair1 || kicker == pair2) continue;

                    var product = (int)(Math.Pow(Card.PRIMES[pair1], 2) * Math.Pow(Card.PRIMES[pair2], 2)) * Card.PRIMES[kicker];
                    lookUp.Add(product, rank++);
                }
            }
        }
    }

    /// <summary>
    /// Populates the lookUp table with Pair hands
    /// </summary>
    /// <param name="lookUp"></param>
    /// <param name="rank"></param>
    protected virtual void PopulatePairLUT(IDictionary<int, int> lookUp, int rank)
    {
        // choose a pair
        foreach (var pairRank in _backWardRanks)
        {
            for (var comb1 = 0; comb1 < _backWardRanks.Length - 2; comb1++)
            {
                var k1 = _backWardRanks[comb1];
                if (k1 == pairRank) continue;
                for (var comb2 = comb1 + 1; comb2 < _backWardRanks.Length - 1; comb2++)
                {
                    var k2 = _backWardRanks[comb2];
                    if (k2 == pairRank) continue;
                    for (var comb3 = comb2 + 1; comb3 < _backWardRanks.Length; comb3++)
                    {
                        var k3 = _backWardRanks[comb3];
                        if (k3 == pairRank) continue;

                        var product = (int)Math.Pow(Card.PRIMES[pairRank], 2) * Card.PRIMES[k1] * Card.PRIMES[k2] * Card.PRIMES[k3];
                        lookUp.Add(product, rank++);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Populates the lookUp table with High card hands
    /// </summary>
    /// <param name="lookUp"></param>
    /// <param name="highCards"></param>
    /// <param name="rank"></param>
    protected virtual void PopulateHighCardLUT(IDictionary<int, int> lookUp, IReadOnlyList<int> highCards, int rank)
    {
        foreach (var h in highCards)
        {
            var primeProduct = Card.PrimeProductFromRankBits(h);
            lookUp.Add(primeProduct, rank++);
        }
    }

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
            flushes.Add(primeProduct, rank++);
            //flushes[primeProduct] = rank++;
        }

        // we start the counting for flushes on max full house, which
        // is the worst rank that a full house can have (2,2,2,3,3)
        rank = MAX_FULL_HOUSE + 1;
        foreach (var f in flushesArr)
        {
            var primeProduct = Card.PrimeProductFromRankBits(f);
            flushes.Add(primeProduct, rank++);
            //flushes[primeProduct] = rank++;
        }

        return new ReadOnlyDictionary<int, int>(flushes); // TODO Does this have an overhead ??
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
