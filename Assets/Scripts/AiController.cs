using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController
{
    readonly PlayerManager playerManager;
    public AiController(PlayerManager _playerManager) {
        playerManager = _playerManager;
    }

    public IEnumerator DoLordAsking(List<CardDisplay> cards) {
        yield return new WaitForSeconds(1.0f);
        List<CardDisplay> handCards = new List<CardDisplay>();
        handCards.AddRange(cards);
        handCards.Sort(
            (card1, card2) => {return card1.cardValue - card2.cardValue; }
        );
        bool canRunForLandlord = handCards[handCards.Count - 1].cardValue >= 16;
        if (canRunForLandlord)
        {
            playerManager.OnConfirmClicked();
        } else
        {
            playerManager.OnCancelClicked();
        }
    }

    public IEnumerator DoUsingCard() {
        yield return new WaitForSeconds(1.0f);
        bool hasSelected = playerManager.TryHintStartegy();
        if (hasSelected) {
            yield return new WaitForSeconds(1.0f);
            playerManager.OnConfirmClicked();
        }
    }
}

public class AiUsingStrategy
{
    // c# dont have HashMap, what a suprise
    // IDictionary<int, int> numberNames = new Dictionary<int, int>();

    List<CardDisplay> jockerCards = new List<CardDisplay>();
    List<CardDisplay> flushCards = new List<CardDisplay>();
    List<int> flushCountList = new List<int>();
    List<CardDisplay> triCards = new List<CardDisplay>();
    List<CardDisplay> bombCards = new List<CardDisplay>();
    List<CardDisplay> singleCards = new List<CardDisplay>();
    List<CardDisplay> pairCards = new List<CardDisplay>();


    public void LoadHandCards (List<CardDisplay> handCards) {
        jockerCards.Clear();
        flushCards.Clear();
        triCards.Clear();
        bombCards.Clear();
        singleCards.Clear();
        pairCards.Clear();

        if (handCards.Count == 1) {
            singleCards.Add(handCards[0]);
            return;
        }
        handCards.Sort((card1, card2) => {
            return card1.cardValue - card2.cardValue;
        });
        // jocker bomb
        var lastCard = handCards[handCards.Count - 1];
        var lastTwoCard = handCards[handCards.Count - 2];
        if (lastCard.cardValue >= 16)
        {
            jockerCards.Add(lastCard);
            handCards.Remove(lastCard);
        }
        if (lastTwoCard.cardValue >= 16)
        {
            jockerCards.Add(lastTwoCard);
            handCards.Remove(lastTwoCard);
        }

        // bomb cards
        while(true) {
            var tmpBombCards = CardRuleUtils.GetLargerBomb(handCards, 0);
            if (tmpBombCards.Count > 0){
                foreach(var card in tmpBombCards) {
                    handCards.Remove(card);
                }
                bombCards.AddRange(tmpBombCards);
            } else {
                break;
            }
        }

        // Tri cards
        while(true) {
            var tmpCards = CardRuleUtils.GetLargerTri(handCards, 0);
            if (tmpCards.Count > 0){
                foreach(var card in tmpCards) {
                    handCards.Remove(card);
                }
                triCards.AddRange(tmpCards);
            } else {
                break;
            }
        }

        // flush cards
        while(true) {
            var tmpCards = CardRuleUtils.GetAnyFlush(handCards);
            if (tmpCards.Count > 0){
                foreach(var card in tmpCards) {
                    handCards.Remove(card);
                }
                flushCards.AddRange(tmpCards);
                flushCountList.Add(tmpCards.Count);
            } else {
                break;
            }
        }

        // pair cards
        while(true) {
            var tmpCards = CardRuleUtils.GetLargerPair(handCards, 0);
            if (tmpCards.Count > 0){
                foreach(var card in tmpCards) {
                    handCards.Remove(card);
                }
                pairCards.AddRange(tmpCards);
            } else {
                break;
            }
        }

        // single cards
        singleCards.AddRange(handCards);
    }

    public List<CardDisplay> TryUsingAnyCard () {
        List<CardDisplay> selected = new List<CardDisplay>();
        if (flushCards.Count > 0) {
            var flushLength = flushCountList[0];
            flushCountList.RemoveAt(0);
            // TODO Should remove and search together
            for(int i = 0; i < flushLength; i++) {
                selected.Add(flushCards[i]);
            }
            foreach(var card in selected) {
                flushCards.Remove(card);
            }
        } else if (triCards.Count > 0) {
            for(int i = 0; i < 3; i++) {
                selected.Add(triCards[i]);
            }
            foreach(var card in selected) {
                triCards.Remove(card);
            }
            if (singleCards.Count > 0) {
                selected.Add(singleCards[0]);
                singleCards.RemoveAt(0);
            }
        } else if (pairCards.Count > 0) {
            for(int i = 0; i < 2; i++) {
                selected.Add(pairCards[i]);
            }
            foreach(var card in selected) {
                pairCards.Remove(card);
            }
        } else if (singleCards.Count > 0) {
            selected.Add(singleCards[0]);
            singleCards.RemoveAt(0);
        } else if (bombCards.Count > 0) {
            for(int i = 0; i < 4; i++) {
                selected.Add(bombCards[i]);
            }
            foreach(var card in selected) {
                bombCards.Remove(card);
            }
        } else if (jockerCards.Count > 0) {
            selected.AddRange(jockerCards);
            jockerCards.Clear();
        }

        return selected;
    }
}
