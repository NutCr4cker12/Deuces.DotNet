namespace Deuces;

public class CanadianStudEvaluator : Evaluator
{
    public CanadianStudEvaluator() : base(new CanadianStudLookupTable()) { }
    protected override int Five(List<int> cards)
    {
        int prime;
        // if flush
        if ((cards[0] & cards[1] & cards[2] & cards[3] & cards[4] & 0xF000) != 0)
        {
            var handOR = (cards[0] | cards[1] | cards[2] | cards[3] | cards[4]) >> 16;
            prime = Card.PrimeProductFromRankBits(handOR);
            return _table.FlushLookup[prime];
        }

        // four flush
        if (IsFourFlush(cards, out prime, out var suiteOrder))
            return ((CanadianStudLookupTable)_table).FourFlushLUT[prime] + suiteOrder;

        // otherwise
        prime = Card.PrimeProductFromHand(cards);
        return _table.UnSuitedLookup[prime];
    }

    private bool IsFourFlush(List<int> cards, out int prime, out int suiteOrder)
    {
        prime = -1;
        suiteOrder = -1;
        if ((cards[0] & cards[1] & cards[2] & cards[3] & 0xF000) != 0)
        {
            suiteOrder = GetSuiteOrder(cards[0]);
            var handOR = (cards[0] | cards[1] | cards[2] | cards[3]) >> 16;
            var rankPrime = Card.PrimeProductFromRankBits(handOR);
            prime = rankPrime * (int)Math.Pow(Card.PrimeProductFromHand(new List<int> { cards[4] }), 2);
            prime = Math.Max(0, prime);
            return true;
        }

        if ((cards[0] & cards[1] & cards[2] & cards[4] & 0xF000) != 0)
        {
            suiteOrder = GetSuiteOrder(cards[0]);
            var handOR = (cards[0] | cards[1] | cards[2] | cards[4]) >> 16;
            var rankPrime = Card.PrimeProductFromRankBits(handOR);
            prime = rankPrime * (int)Math.Pow(Card.PrimeProductFromHand(new List<int> { cards[3] }), 2);
            prime = Math.Max(0, prime);
            return true;
        }

        if ((cards[0] & cards[1] & cards[3] & cards[4] & 0xF000) != 0)
        {
            suiteOrder = GetSuiteOrder(cards[0]);
            var handOR = (cards[0] | cards[1] | cards[3] | cards[4]) >> 16;
            var rankPrime = Card.PrimeProductFromRankBits(handOR);
            prime = rankPrime * (int)Math.Pow(Card.PrimeProductFromHand(new List<int> { cards[2] }), 2);
            prime = Math.Max(0, prime);
            return true;
        }

        if ((cards[0] & cards[2] & cards[3] & cards[4] & 0xF000) != 0)
        {
            suiteOrder = GetSuiteOrder(cards[0]);
            var handOR = (cards[0] | cards[2] | cards[3] | cards[4]) >> 16;
            var rankPrime = Card.PrimeProductFromRankBits(handOR);
            prime = rankPrime * (int)Math.Pow(Card.PrimeProductFromHand(new List<int> { cards[1] }), 2);
            prime = Math.Max(0, prime);
            return true;
        }

        if ((cards[1] & cards[2] & cards[3] & cards[4] & 0xF000) != 0)
        {
            suiteOrder = GetSuiteOrder(cards[1]);
            var handOR = (cards[1] | cards[2] | cards[3] | cards[4]) >> 16;
            var rankPrime = Card.PrimeProductFromRankBits(handOR);
            prime = rankPrime * (int)Math.Pow(Card.PrimeProductFromHand(new List<int> { cards[0] }), 2);
            prime = Math.Max(0, prime);
            return true;
        }

        return false;
    }

    private int GetSuiteOrder(int card) => (card & 0xF000) switch
    {
        4096 => 1,
        8192 => 2,
        16384 => 3,
        32768 => 4,
        _ => throw new ArgumentOutOfRangeException(nameof(card), card, "Out of range")
    };
}