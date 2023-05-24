using System.Collections.ObjectModel;

namespace Deuces;

internal class CanadianStudLookupTable : StdLookupTable
{
    protected new static readonly int MAX_STRAIGHT_FLUSH = 10;
    protected new static readonly int MAX_FOUR_OF_A_KIND = 166;
    protected new static readonly int MAX_FULL_HOUSE = 322;
    protected new static readonly int MAX_FLUSH = 1599;
    protected new static readonly int MAX_STRAIGHT = 1609;
    protected new static readonly int MAX_THREE_OF_A_KIND = 2467;
    protected new static readonly int MAX_TWO_PAIR = 3325;
    protected static readonly int MAX_FOUR_FLUSH = 40505;
    protected static readonly int MAX_FOUR_STRAIGHT = 40616;
    protected new static readonly int MAX_PAIR = 43436;
    protected new static readonly int MAX_HIGH_CARD = 44642;
    public readonly IReadOnlyDictionary<int, int> FourFlushLUT;
    public override int MaxValue { get; } = MAX_HIGH_CARD;

    public CanadianStudLookupTable()
    {
        var fourFlushLut = new Dictionary<int, int>();
        PopulateFourFlushLUT(fourFlushLut, MAX_TWO_PAIR + 1);
        FourFlushLUT = fourFlushLut;
    }

    public override int GetRankClass(int hr)
    {
        if (hr < 0) throw new Exception("Invalid hand rank. Rank must be at least 0.");
        if (hr <= MAX_STRAIGHT_FLUSH) return 1;
        if (hr <= MAX_FOUR_OF_A_KIND) return 2;
        if (hr <= MAX_FULL_HOUSE) return 3;
        if (hr <= MAX_FLUSH) return 4;
        if (hr <= MAX_STRAIGHT) return 5;
        if (hr <= MAX_THREE_OF_A_KIND) return 6;
        if (hr <= MAX_TWO_PAIR) return 7;
        if (hr <= MAX_FOUR_FLUSH) return 8;
        if (hr <= MAX_FOUR_STRAIGHT) return 9;
        if (hr <= MAX_PAIR) return 10;
        if (hr <= MAX_HIGH_CARD) return 11;
        throw new Exception("Invalid hand rank, cannot return rank class");
    }

    public override string RankClassToString(int rank) => rank switch
    {
        1 => "Straight Flush",
        2 => "Four of a Kind",
        3 => "Full House",
        4 => "Flush",
        5 => "Straight",
        6 => "Three of a Kind",
        7 => "Two Pair",
        8 => "Four Flush",
        9 => "Four Straight",
        10 => "Pair",
        11 => "High Card",
        _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null)
    };

    protected override IReadOnlyDictionary<int, int> Multiples(IDictionary<int, int> lookUp, IReadOnlyList<int> flushes)
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

        // 6) Four straight ...already defined

        // 7) Four straight
        PopulateFourStraightLUT(lookUp, MAX_FOUR_FLUSH + 1);

        // 8) Pair
        PopulatePairLUT(lookUp, MAX_FOUR_STRAIGHT + 1);

        // 9) High Cards
        PopulateHighCardLUT(lookUp, flushes, MAX_PAIR + 1);

        return new ReadOnlyDictionary<int, int>(lookUp); // TODO Does this have an overhead ??
    }

    /// <summary>
    /// Populates the lookUp table with Four flush hands
    /// StdLookUp: Ts9s8s7s2h = High Card
    /// CanadianStud: Ts9s8s7s2h = Four flush
    /// </summary>
    /// <param name="lookUp"></param>
    /// <param name="rank"></param>
    private void PopulateFourFlushLUT(IDictionary<int, int> lookUp, int rank)
    {
        // for each flush
        for (var i1 = 0; i1 < _backWardRanks.Length - 3; i1++)
        {
            var kicker1 = _backWardRanks[i1];
            for (var i2 = i1 + 1; i2 < _backWardRanks.Length - 2; i2++)
            {
                var kicker2 = _backWardRanks[i2];
                for (var i3 = i2 + 1; i3 < _backWardRanks.Length - 1; i3++)
                {
                    var kicker3 = _backWardRanks[i3];
                    for (var i4 = i3 + 1; i4 < _backWardRanks.Length; i4++)
                    {
                        var kicker4 = _backWardRanks[i4];

                        // for each fifth card
                        foreach (var fifthCard in _backWardRanks)
                        {
                            // Prime product from flush ranks
                            var flushProduct = Card.PRIMES[kicker1] * Card.PRIMES[kicker2] * Card.PRIMES[kicker3] * Card.PRIMES[kicker4];
                            // Prime product for kicker because we need to differentiate the kicker rank somehow
                            var rankProduct = (int)Math.Pow(Card.PRIMES[fifthCard], 2);
                            // Multiply prime products
                            var product = flushProduct * rankProduct;
                            // HACK - only the largest possible combination overflows -> make it 0 because it's not otherwise possible
                            product = Math.Max(0, product);
                            lookUp.Add(product, rank);
                            //lookUp[product] = rank;
                            // Every four flush rank is then identified with suite order -> increase every suite with 4
                            rank += 4;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Populates the lookUp table with Four straight hands
    /// StdLookUp: Ts9s8s7s2h = High Card
    /// CanadianStud: Ts9s8s7s2h = Four flush
    /// </summary>
    /// <param name="lookUp"></param>
    /// <param name="rank"></param>
    private void PopulateFourStraightLUT(IDictionary<int, int> lookUp, int rank)
    {
        // for each straight
        for (var i = 0; i < _backWardRanks.Length - 3; i++)
        {
            // Create a straight
            var kicker1 = _backWardRanks[i];
            var kicker2 = _backWardRanks[i + 1];
            var kicker3 = _backWardRanks[i + 2];
            var kicker4 = _backWardRanks[i + 3];
            var straightProduct = Card.PRIMES[kicker1] * Card.PRIMES[kicker2] * Card.PRIMES[kicker3] * Card.PRIMES[kicker4];

            // for each fifth card
            foreach (var fifthCard in _backWardRanks)
            {
                // Skip indices where the fifth card is either one higher or one lower than the four straight
                // which would make it a Five card straight
                if (kicker1 + 1 == fifthCard || fifthCard == kicker4 - 1) continue;
                // Explicitly check A2345 straight
                if (kicker4 == 0 && fifthCard == 12) continue;

                var product = straightProduct * Card.PRIMES[fifthCard];
                lookUp.Add(product, rank++);
            }
        }
    }

    /// <summary>
    /// Populates the lookUp table with Pair hands
    /// </summary>
    /// <param name="lookUp"></param>
    /// <param name="rank"></param>
    protected override void PopulatePairLUT(IDictionary<int, int> lookUp, int rank)
    {
        // choose a pair
        foreach (var pairRank in _backWardRanks)
        {
            var pairProduct = (int)Math.Pow(Card.PRIMES[pairRank], 2);
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

                        var product = pairProduct * Card.PRIMES[k1] * Card.PRIMES[k2] * Card.PRIMES[k3];

                        // This combination is already defined in four cards straight
                        if (lookUp.ContainsKey(product))
                            continue;

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
    protected override void PopulateHighCardLUT(IDictionary<int, int> lookUp, IReadOnlyList<int> highCards, int rank)
    {
        foreach (var h in highCards)
        {
            var primeProduct = Card.PrimeProductFromRankBits(h);

            // This combination is already defined in four cards straight
            // TODO We should actually exclude also the four flushes ... so maybe just recreate this whole implementation
            if (lookUp.ContainsKey(primeProduct))
                continue;

            lookUp.Add(primeProduct, rank++);
        }
    }
}