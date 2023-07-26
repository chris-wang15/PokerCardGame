using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardPool : MonoBehaviour
{
    public GameObject cardPrefab;
    List<CardDisplay> cards = new List<CardDisplay>();

    void Start()
    {
        GenerateCards();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateCards()
    {
        int id = 0;
        float dis = 1.0f;
        for (int i = 1; i <= 13; i++)
        {
            int cardValue = i >= 3 ? i : 13 + i;
            var tmpHeartCard = GameObject.Instantiate(
                cardPrefab,
                transform
            );
            var cardName = "Heart" + (i < 10 ? "0" + i : i);
            var cardDisplay = tmpHeartCard.GetComponent<CardDisplay>();
            cardDisplay.LoadParams(
                cardName,
                cardValue,
                CardType.Heart
            );
            tmpHeartCard.transform.localPosition = new Vector3(
                id * dis,
                id * dis,
                0
            );

            cards.Add(cardDisplay);
            id++;
        }

        for (int i = 1; i <= 13; i++)
        {
            int cardValue = i >= 3 ? i : 13 + i;
            var tmpSpadeCard = GameObject.Instantiate(
                cardPrefab,
                transform
            );
            var cardName = "Spade" + (i < 10 ? "0" + i : i);
            var cardDisplay = tmpSpadeCard.GetComponent<CardDisplay>();
            cardDisplay.LoadParams(
                cardName,
                cardValue,
                CardType.Spade
            );
            tmpSpadeCard.transform.localPosition = new Vector3(
                id * dis,
                id * dis,
                0
            );

            cards.Add(cardDisplay);
            id++;
        }

        for (int i = 1; i <= 13; i++)
        {
            int cardValue = i >= 3 ? i : 13 + i;
            var tmpClubCard = GameObject.Instantiate(
                cardPrefab,
                transform
            );
            var cardName = "Club" + (i < 10 ? "0" + i : i);
            var cardDisplay = tmpClubCard.GetComponent<CardDisplay>();
            cardDisplay.LoadParams(
                cardName,
                cardValue,
                CardType.Club
            );
            tmpClubCard.transform.localPosition = new Vector3(
                id * dis,
                id * dis,
                0
            );

            cards.Add(cardDisplay);
            id++;
        }

        for (int i = 1; i <= 13; i++)
        {
            int cardValue = i >= 3 ? i : 13 + i;
            var tmpDiamondCard = GameObject.Instantiate(
                cardPrefab,
                transform
            );
            var cardName = "Diamond" + (i < 10 ? "0" + i : i);
            var cardDisplay = tmpDiamondCard.GetComponent<CardDisplay>();
            cardDisplay.LoadParams(
                cardName,
                cardValue,
                CardType.Diamond
            );
            tmpDiamondCard.transform.localPosition = new Vector3(
                id * dis,
                id * dis,
                0
            );

            cards.Add(cardDisplay);
            id++;
        }

        // Joker_Monochrome
        // Joker_Color

        var jokerMonochromeCard = GameObject.Instantiate(
            cardPrefab,
            transform
        );
        var jmCardName = "Joker_Monochrome";
        var jmCardDisplay = jokerMonochromeCard.GetComponent<CardDisplay>();
        jmCardDisplay.LoadParams(
            jmCardName,
            16,
            CardType.Joker_Monochrome
        );
        jokerMonochromeCard.transform.localPosition = new Vector3(
            id * dis,
            id * dis,
            0
        );

        cards.Add(jmCardDisplay);
        id++;

        var jokerColorCard = GameObject.Instantiate(
                cardPrefab,
                transform
            );
        var jcCardName = "Joker_Color";
        var jcCardDisplay = jokerColorCard.GetComponent<CardDisplay>();
        jcCardDisplay.LoadParams(
            jcCardName,
            17,
            CardType.Joker_Color
        );
        jokerColorCard.transform.localPosition = new Vector3(
            id * dis,
            id * dis,
            0
        );

        cards.Add(jcCardDisplay);
        id++;
    }

    public void Shuffle(List<PlayerManager> playerList, int startId)
    {
        CardRuleUtils.ListRandom(cards);
        float dis = 1.0f;
        for(int i = 0; i < cards.Count; i++) {
            var tmpCard = cards[i];
            tmpCard.transform.SetAsLastSibling();
            tmpCard.transform.localPosition = new Vector3(
                i * dis,
                i * dis,
                0
            );
        }
        StartCoroutine(ShuffleEnumerator(playerList, startId));
    }

    IEnumerator ShuffleEnumerator(List<PlayerManager> playerList, int startId)
    {
        int curId = startId;
        while (cards.Count > 3)
        {
            yield return new WaitForSeconds(0.2f);
            PlayerManager player = playerList[curId];
            var tmpCard = cards[0];
            cards.Remove(tmpCard);
            var cardDisplay = tmpCard.GetComponent<CardDisplay>();
            cardDisplay.MoveWithCallback(
                player.transform.position,
                () => {
                    player.AddCards(cardDisplay);
                    cardDisplay.RotateImmediately();
                }
            );
            curId++;
            if (curId == 3) curId = 0;
        }
    }

    public void MovebottomCards (UsingCards usingCards) {
        if (cards.Count != 3) {
            Debug.Log("Internal error, wrong bottom cards count: " + cards.Count);
        }
        List<CardDisplay> bottomCards = new List<CardDisplay>();
        foreach(var card in cards) {
            bottomCards.Add(card);
        }
        cards.Clear();
        usingCards.AddCards(bottomCards);
        // Debug.Log("After deal cards count: " + cards.Count);
    }

    public void RecycleAllCards (UsingCards usingCards, List<PlayerManager> players) {
        var usedCards = usingCards.RecycleAllCards();
        if (usedCards.Count > 0 && cards.Count != 0)
        {
            Debug.Log("cards not empty after end: " + cards.Count);
        }
        cards.AddRange(usedCards);
        foreach(var player in players) {
            cards.AddRange(player.RecycleAllCards());
        }
        if (cards.Count != 54) Debug.Log("cards count error " + cards.Count);
        float dis = 1.0f;
        for(int i = 0; i < cards.Count; i++) {
            var tmpCard = cards[i];
            tmpCard.ResetState();
            tmpCard.RotateToBackImediately();
            tmpCard.transform.parent = transform;
            tmpCard.transform.localPosition = new Vector3(
                i * dis,
                i * dis,
                0
            );
        }
    }
}
