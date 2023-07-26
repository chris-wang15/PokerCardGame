using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingCards : MonoBehaviour
{
    public GameObject discardCards;
    List<CardDisplay> showingCards = new List<CardDisplay>();
    List<CardDisplay> usedCards = new List<CardDisplay>();
    int bottomMoveEndCount = 0;
    internal int usingCardPlayerId = -1;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void AddCards (List<CardDisplay> cards, int id) {
        usingCardPlayerId = id;
        AddCards(cards);
    }

    public void AddCards (List<CardDisplay> cards) {
        if (showingCards.Count != 0) {
            DiscardShowingCards();
        }
        foreach(var card in cards) {
            card.transform.parent = transform;
            showingCards.Add(card);
        }
        CalculateCardsPosition();
    }

    void DiscardShowingCards() {
        foreach(var card in showingCards) {
            usedCards.Add(card);
            card.MoveWithCallback(
                discardCards.transform.position,
                () => {
                    card.transform.parent = discardCards.transform;
                }
            );
        }
        showingCards.Clear();
    }

    void CalculateCardsPosition() {
        int cardCount = showingCards.Count;
        if (cardCount == 0) {
            Debug.Log("Empty cards used");
            return;
        }
        if (cardCount == 1) {
            showingCards[0].transform.localPosition = new Vector3(
                0, 0, 0
            );
            return;
        }
        int centerId = cardCount % 2 == 0 ? cardCount / 2 - 1 : cardCount / 2;
        float deltaDis = 1045.0f / (cardCount - 1);
        if (deltaDis > 400.0f) deltaDis = 400.0f;
        if (deltaDis < 80.0f) deltaDis = 80.0f;
        for(int i = 0; i < cardCount; i++) {
            float dis = (i - centerId) * deltaDis;
            var tmpObj = showingCards[i];
            tmpObj.transform.localPosition = new Vector3(
                dis, 0, 0
            );
        }
    }

    public void ShowAndDispatchBottomCards(PlayerManager landlord) {
        foreach(var card in showingCards) {
            card.Rotate();
        }
        StartCoroutine(DispatchBottomCardsEnumerator(landlord));
    }

    IEnumerator DispatchBottomCardsEnumerator(PlayerManager landlord) {
        yield return new WaitForSeconds(2.0f);
        foreach(var card in showingCards) {
            card.MoveWithCallback(
                landlord.transform.position,
                () => {
                    OnBottomMoveEnd(landlord);
                }
            );
        }
    }

    void OnBottomMoveEnd(PlayerManager landlord) {
        bottomMoveEndCount++;
        if (bottomMoveEndCount == 3) {
            bottomMoveEndCount = 0;
            landlord.AddBootomCards(showingCards);
            showingCards.Clear();
        }
    }

    public List<CardDisplay> GetShowing() {
        List<CardDisplay> curShowingCards = new List<CardDisplay>();
        curShowingCards.AddRange(showingCards);
        return curShowingCards;
    }

    public int GetShowingCount() {
        return showingCards.Count;
    }

    public List<CardDisplay> RecycleAllCards () {
        List<CardDisplay> recycleCards = new List<CardDisplay>();
        if (showingCards.Count > 0) {
            recycleCards.AddRange(showingCards);
        }
        recycleCards.AddRange(usedCards);
        showingCards.Clear();
        usedCards.Clear();
        return recycleCards;
    }
}
