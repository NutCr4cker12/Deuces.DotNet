using System.Collections.ObjectModel;

namespace Deuces;

/// <summary>
/// Class representing a deck. The first time we create, we seed the static 
/// deck with the list of unique card integers. Each object instantiated simply
/// makes a copy of this object and shuffles it. 
/// </summary>
public class Deck
{
    private static readonly IReadOnlyList<int> _fullDeck;
    private static readonly Random _rng = new Random();
    private readonly List<int> _cards;
    static Deck()
    {
        _fullDeck = new ReadOnlyCollection<int>(CreateFullDeck());
    }

    public Deck()
    {
        _cards = _fullDeck.ToList();
        Shuffle();
    }

    public static List<int> GetFullDeck()
    {
        return _fullDeck.ToList();
    }

    public int[] Draw(int n = 1)
    {
        if (n < 1)
            throw new ArgumentException($"Invalid argument n={n}. N must be at least 1");
        if (n > _cards.Count)
            throw new ArgumentException($"Can't draw more than what's left in the deck. Requested {n} but Deck contains only {_cards.Count}");

        var res = new int[n];
        for (var i = 0; i < n; i++)
        {
            var card = _cards[0];
            res[i] = card;
            _cards.RemoveAt(0);
        }

        return res;
    }

    public void Shuffle()
    {
        var n = _cards.Count;
        while (n > 1)
        {
            n--;
            var k = _rng.Next(n + 1);
            (_cards[k], _cards[n]) = (_cards[n], _cards[k]);
        }
    }

    private static List<int> CreateFullDeck()
    {
        var list = new List<int>(52);
        foreach (var rank in Card.STR_RANKS)
        {
            foreach (var suite in Card.STR_SUITES)
            {
                list.Add(Card.New($"{rank}{suite}"));
            }
        }
        return list;
    }
}