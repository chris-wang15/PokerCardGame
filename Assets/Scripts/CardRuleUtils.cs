using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRuleUtils
{
    
    public static List<CardDisplay> GetLarger(List<CardDisplay> handCards, RuleValue target)
    {
        handCards.Sort((card1, card2) => {
            return card1.cardValue - card2.cardValue;
        });
        List<CardDisplay> selected;
        switch (target.ruleType)
        {
            case RuleType.Single:
                selected = GetLargerSingle(handCards, target.value);
                if (selected.Count > 0) return selected;
                break;
            case RuleType.Joker_Bomb:
                return new List<CardDisplay>();
            case RuleType.Pair:
                selected = GetLargerPair(handCards, target.value);
                if (selected.Count > 0) return selected;
                break;
            case RuleType.Bomb:
                selected = GetLargerBomb(handCards, target.value);
                if (selected.Count > 0) return selected;
                return GetJokerBomb(handCards);
            case RuleType.Tri:
                selected = GetLargerTri(handCards, target.value);
                if (selected.Count > 0) return selected;
                break;
            case RuleType.Flush:
                selected = GetLargerFlush(handCards, target.value, target.length);
                if (selected.Count > 0) return selected;
                break;
            case RuleType.Tri_One:
                selected = GetLargerTriOne(handCards, target.value);
                if (selected.Count > 0) return selected;
                break;
            case RuleType.Four_Two:
                break;
            case RuleType.Pair_Flush:
                selected = GetLargerPairFlush(handCards, target.value, target.length);
                if (selected.Count > 0) return selected;
                break;
            default:
                Debug.Log("Wrong type used: " + target.ruleType);
                break;
        }
        selected = GetLargerBomb(handCards, 0);
        if (selected.Count > 0) return selected;
        return GetJokerBomb(handCards);
    }

    private static List<CardDisplay> GetLargerSingle(
        List<CardDisplay> handCards, int value
    ) 
    {
        List<CardDisplay> selected = new List<CardDisplay>();
        for(int i = 0; i < handCards.Count; i++) {
            if (handCards[i].cardValue > value) {
                selected.Add(handCards[i]);
                break;
            }
        }
        return selected;
    }

    public static List<CardDisplay> GetLargerPair(
        List<CardDisplay> handCards, int value
    ) 
    {
        List<CardDisplay> selected = new List<CardDisplay>();
        int count = 1;
        for(int i = 1; i < handCards.Count; i++) {
            if (handCards[i].cardValue > value 
            && handCards[i].cardValue == handCards[i - 1].cardValue) {
                count++;
                if (count == 2) {
                    selected.Add(handCards[i]);
                    selected.Add(handCards[i-1]);
                    break;
                }
            } else {
                count = 1;
            }
        }
        return selected;
    }

    public static List<CardDisplay> GetLargerTri(
        List<CardDisplay> handCards, int value
    ) 
    {
        List<CardDisplay> selected = new List<CardDisplay>();
        if (handCards.Count < 3)
        {
            return selected;
        }
        int count = 1;
        for(int i = 1; i < handCards.Count; i++) {
            if (handCards[i].cardValue > value 
            && handCards[i].cardValue == handCards[i - 1].cardValue) {
                count++;
                if (count == 3) {
                    selected.Add(handCards[i]);
                    selected.Add(handCards[i-1]);
                    selected.Add(handCards[i-2]);
                    break;
                }
            } else {
                count = 1;
            }
        }
        return selected;
    }

    public static List<CardDisplay> GetLargerBomb(
        List<CardDisplay> handCards, int value
    ) 
    {
        List<CardDisplay> selected = new List<CardDisplay>();
        if (handCards.Count < 4)
        {
            return selected;
        }
        int count = 1;
        for(int i = 1; i < handCards.Count; i++) {
            if (handCards[i].cardValue > value 
            && handCards[i].cardValue == handCards[i - 1].cardValue) {
                count++;
                if (count == 4) {
                    selected.Add(handCards[i]);
                    selected.Add(handCards[i-1]);
                    selected.Add(handCards[i-2]);
                    selected.Add(handCards[i-3]);
                    break;
                }
            } else {
                count = 1;
            }
        }
        return selected;
    }

    public static List<CardDisplay> GetJokerBomb(
        List<CardDisplay> handCards
    ) 
    {
        List<CardDisplay> selected = new List<CardDisplay>();
        if (handCards[handCards.Count - 1].cardType == CardType.Joker_Color 
        && handCards[handCards.Count - 2].cardType == CardType.Joker_Monochrome )
        {
            selected.Add(handCards[handCards.Count - 1]);
            selected.Add(handCards[handCards.Count - 2]);
        }
        return selected;
    }

    private static List<CardDisplay> GetLargerFlush(
        List<CardDisplay> handCards, int value, int length
    ) 
    {
        if (handCards.Count < length) return new List<CardDisplay>();
        List<CardDisplay> selected = new List<CardDisplay>();
        int count = 0;
        int previousValue = -1;
        
        for(int i = 0; i < handCards.Count; i++) {
            if (handCards[i].cardValue >= 15) {
                // 2 -> 15, jocker -> 16,17
                break;
            }
            if (previousValue == -1 && handCards[i].cardValue > value)
            {
                selected.Add(handCards[i]);
                count = 1;
                previousValue = handCards[i].cardValue;
            } else if (previousValue != -1
            && handCards[i].cardValue == 1 + previousValue 
            ) {
                count++;
                selected.Add(handCards[i]);
                previousValue = handCards[i].cardValue;
                if (count == length)
                {
                    return selected;
                }
            } else if (previousValue != -1 
            && handCards[i].cardValue > 1 + previousValue ) {
                count = 1;
                selected.Clear();
                selected.Add(handCards[i]);
                previousValue = handCards[i].cardValue;
            }
        }
        return new List<CardDisplay>();
    }

    private static List<CardDisplay> GetLargerPairFlush(
        List<CardDisplay> handCards, int value, int length
    ) 
    {
        if (handCards.Count < length) return new List<CardDisplay>();
        List<CardDisplay> selected = new List<CardDisplay>();
        int count = 0;
        int tmpCount = 0;
        int previousValue = -1;
        
        for(int i = 0; i < handCards.Count; i++) {
            if (handCards[i].cardValue >= 15) {
                // 2 -> 15, jocker -> 16,17
                break;
            }
            if (previousValue == -1 && handCards[i].cardValue > value)
            {
                selected.Add(handCards[i]);
                count = 1;
                previousValue = handCards[i].cardValue;
            } else if (previousValue != -1 && tmpCount == 0
            && handCards[i].cardValue == previousValue 
            ) {
                count++;
                tmpCount++;
                selected.Add(handCards[i]);
            } else if (previousValue != -1 && tmpCount == 1 
            && handCards[i].cardValue == 1 + previousValue) {
                count++;
                tmpCount = 0;
                selected.Add(handCards[i]);
                if (count == length)
                {
                    return selected;
                }
                previousValue = handCards[i].cardValue;
            } else if (previousValue != -1 && tmpCount == 0
            && handCards[i].cardValue > previousValue) {
                count = 1;
                tmpCount = 0;
                selected.Clear();
                selected.Add(handCards[i]);
                previousValue = handCards[i].cardValue;
            } else if (previousValue != -1 && tmpCount == 1
            && handCards[i].cardValue > 1 + previousValue) {
                count = 1;
                tmpCount = 0;
                selected.Clear();
                selected.Add(handCards[i]);
                previousValue = handCards[i].cardValue;
            }
        }
        return new List<CardDisplay>();
    }

    private static List<CardDisplay> GetLargerTriOne(
        List<CardDisplay> handCards, int value
    ) {
        if (handCards.Count < 4) return new List<CardDisplay>();
        var selected = GetLargerTri(handCards, value);
        if (selected.Count == 0) return selected;
        int curValue = selected[0].cardValue;
        for(int i = 0; i < handCards.Count; i++) {
            if (handCards[i].cardValue != curValue) {
                selected.Add(handCards[i]);
                return selected;
            }
        }
        return new List<CardDisplay>();
    }

    private static List<CardDisplay> GetLargerFourTwo(
        List<CardDisplay> handCards, int value
    ) {
        if (handCards.Count < 6) return new List<CardDisplay>();
        var selected = GetLargerBomb(handCards, value);
        if (selected.Count == 0) return selected;
        int curValue = selected[0].cardValue;
        int count = 0;
        for(int i = 0; i < handCards.Count; i++) {
            if (handCards[i].cardValue != curValue) {
                count++;
                selected.Add(handCards[i]);
                if (count == 2) {
                    return selected;
                }
            }
        }
        return new List<CardDisplay>();
    }

    public static List<CardDisplay> GetAnyFlush(
        List<CardDisplay> handCards
    ) 
    {
        if (handCards.Count < 5) return new List<CardDisplay>();
        List<CardDisplay> selected = new List<CardDisplay>();
        int previousValue = -1;
        
        for(int i = 0; i < handCards.Count; i++) {
            if (handCards[i].cardValue >= 15) {
                // 2 -> 15, jocker -> 16,17
                break;
            }
            if (previousValue == -1)
            {
                selected.Add(handCards[i]);
                previousValue = handCards[i].cardValue;
            } else if (previousValue != -1
            && handCards[i].cardValue == 1 + previousValue 
            ) {
                selected.Add(handCards[i]);
                previousValue = handCards[i].cardValue;
            } else if (previousValue != -1 
            && handCards[i].cardValue > 1 + previousValue ) {
                selected.Clear();
                selected.Add(handCards[i]);
                previousValue = handCards[i].cardValue;
            }
        }
        if (selected.Count >= 5) return selected;
        return new List<CardDisplay>();
    }

    public static void ListRandom<T>(List<T> sources)
    {
        System.Random rd = new System.Random();
        int index = 0;
        T temp;
        for (int i = 0; i < sources.Count; i++)
        {
            index = rd.Next(0, sources.Count - 1);
            if (index != i)
            {
                temp = sources[i];
                sources[i] = sources[index];
                sources[index] = temp;
            }
        }
    }

    public static string GetValueString(int valueId) {
        if (valueId <= 10) return valueId.ToString();
        switch (valueId)
        {
            case 11:
                return "J";
            case 12:
                return "Q";
            case 13:
                return "K";
            case 14:
                return "A";
            case 15:
                return "2";
            case 16:
                return "Balck Joker";
            case 17:
                return "Red Joker";
            default:
                Debug.Log("Wrong value count " + valueId);
                return "UnKnow";
        }
    }

    public static bool IsBomb (List<CardDisplay> cards) {
        if (cards.Count == 2) {
            return cards[0].cardValue >= 16 && cards[1].cardValue >= 16;
        } else if (cards.Count == 4) {
            return cards[0].cardValue == cards[1].cardValue 
            && cards[0].cardValue == cards[2].cardValue 
            && cards[0].cardValue == cards[3].cardValue;
        }
        return false;
    }
}
