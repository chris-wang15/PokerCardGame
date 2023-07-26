using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule
{
    private static Rule uniqueInstance;
    private static readonly object locker = new object();

    private Rule()
    {
        // loadData();
    }

    public static Rule GetInstance()
    {
        lock (locker)
        {
            if (uniqueInstance == null)
            {
                uniqueInstance = new Rule();
            }
        }

        return uniqueInstance;
    }

    public RuleValue GetRule (List<CardDisplay> cards) {
        if (cards.Count == 0){
            Debug.Log("Empty cards");
            return new RuleValue(RuleType.Wrong);
        }
        if (cards.Count == 1) {
            return new RuleValue(RuleType.Single, cards[0].cardValue);
        }
        cards.Sort((card1, card2) => {
            return card1.cardValue - card2.cardValue;
        });
        if (cards[1].cardValue < cards[0].cardValue) {
            Debug.Log("Sort order error");
        }
        
        if (cards.Count == 2) {
            if (cards[0].cardValue == 16 && cards[1].cardValue == 17) {
                return new RuleValue(RuleType.Joker_Bomb);
            } else if (cards[0].cardValue == cards[1].cardValue) {
                return new RuleValue(RuleType.Pair, cards[0].cardValue);
            } else {
                return new RuleValue(RuleType.Wrong);
            }
        } else if (cards.Count == 3) {
            if (cards[0].cardValue == cards[1].cardValue 
            && cards[0].cardValue == cards[2].cardValue) {
                return new RuleValue(RuleType.Tri, cards[0].cardValue);
            } else {
                return new RuleValue(RuleType.Wrong);
            }
        } else if (cards.Count == 4) {
            if (cards[0].cardValue == cards[1].cardValue
            && cards[0].cardValue == cards[2].cardValue
            && cards[0].cardValue == cards[3].cardValue) {
                return new RuleValue(RuleType.Bomb, cards[0].cardValue);
            } else if (cards[1].cardValue == cards[2].cardValue 
            && (
                cards[0].cardValue == cards[1].cardValue 
                || cards[3].cardValue == cards[1].cardValue
                )
            ) {
                return new RuleValue(RuleType.Tri_One, cards[1].cardValue);
            } else {
                return new RuleValue(RuleType.Wrong);
            }
        } else {
            if (isFlush(cards)) {
                return new RuleValue(RuleType.Flush, cards[0].cardValue, cards.Count);
            } else if (isFourTwo(cards)) {
                return new RuleValue(RuleType.Four_Two, cards[2].cardValue);
            } else if (isPairFlush(cards)) {
                return new RuleValue(RuleType.Pair_Flush, cards[0].cardValue, cards.Count);
            } else {
                return new RuleValue(RuleType.Wrong);
            }
        }
    }

    private bool isFlush(List<CardDisplay> cards) {
        int firstValue = cards[0].cardValue;
        for(int i = 1; i < cards.Count; i++) {
            if (cards[i].cardValue >= 15) {
                return false;
            }
            if (cards[i].cardValue - firstValue != i) {
                return false;
            }
        }
        return true;
    }

    private bool isPairFlush(List<CardDisplay> cards) {
        if (cards.Count % 2 != 0) {
            return false;
        }
        int firstValue = cards[0].cardValue;
        for(int i = 1; i < cards.Count; i++) {
            if (cards[i].cardValue >= 15) {
                return false;
            }
            if (cards[i].cardValue - firstValue != i/2) {
                return false;
            }
        }
        return true;
    }

    private bool isFourTwo(List<CardDisplay> cards) {
        if (cards.Count != 6) {
            return false;
        }
        int cnt = 0;
        for(int i = 0; i < cards.Count; i++) {
            if (i==0) {
                cnt++;
                continue;
            }
            if (cards[i].cardValue == cards[i-1].cardValue) {
                cnt++;
                
            }else{
                cnt=1;
            }
            if(cnt==4) {
                return true;
            }
        }
        return false;
    }
}

public class RuleValue {
    public readonly int value;
    public readonly RuleType ruleType;
    public readonly int length;

    public RuleValue(RuleType _ruleType) {
        value = 0;
        ruleType = _ruleType;
        length = 0;
    }

    public RuleValue(RuleType _ruleType, int _value) {
        value = _value;
        ruleType = _ruleType;
        length = 0;
    }

    public RuleValue(RuleType _ruleType, int _value, int _length) {
        ruleType = _ruleType;
        value = _value;
        length = _length;
    }
}

public enum RuleType {
    Single, // a single card
    Joker_Bomb, // a pair of joker cards
    Pair, // two of a kind
    Bomb, // four of a kind
    Tri, // three of a kind
    Flush,
    Tri_One,
    Four_Two,
    Pair_Flush,
    Wrong
}
