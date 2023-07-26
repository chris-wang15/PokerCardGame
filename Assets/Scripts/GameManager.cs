using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    internal  CardPool cardPool;

    public Button gameButtton;

    public GameObject leftPlayer;
    public GameObject player;
    public GameObject rightPlayer;
    public UsingCards usingCards;

    public GameObject bombEffect;
    public Text vitoryHint;

    int curPlayerId = 0;
    List<PlayerManager> playerList = new List<PlayerManager>();
    int readyCount = 0;
    internal GameState curGameState = GameState.End;
    bool inTurn = false;
    int landlordId = -1;
    Button confirmButton;
    Button cancelButton;
    Button hintButton;
    Text hintText;
    int curBattleScore;

    void Start()
    {
        confirmButton = GameObject.FindGameObjectWithTag(tag: "ConfirmButton").GetComponent<Button>();
        cancelButton = GameObject.FindGameObjectWithTag(tag: "CancelButton").GetComponent<Button>();
        hintButton = GameObject.FindGameObjectWithTag(tag: "HintButton").GetComponent<Button>();
        hintText = GameObject.FindGameObjectWithTag(tag: "HintText").GetComponent<Text>();
        hintText.text = "";
        confirmButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        hintButton.gameObject.SetActive(false);

        cardPool = GameObject.FindGameObjectWithTag("CardPool").GetComponent<CardPool>();
        // Debug.Log("leftPlayer: " + leftPlayer.GetComponent<PlayerManager>());
        leftPlayer.GetComponent<PlayerManager>().LoadParams(
            0, true,
            confirmButton,
            cancelButton,
            hintButton, 
            hintText
            );
        player.GetComponent<PlayerManager>().LoadParams(1, false,
            confirmButton,
            cancelButton, 
            hintButton,
            hintText);
        rightPlayer.GetComponent<PlayerManager>().LoadParams(2, true,
            confirmButton,
            cancelButton, 
            hintButton,
            hintText);
        playerList.Add(leftPlayer.GetComponent<PlayerManager>());
        playerList.Add(player.GetComponent<PlayerManager>());
        playerList.Add(rightPlayer.GetComponent<PlayerManager>());
    }

    // Update is called once per frame
    void Update()
    {
        if (!inTurn) {
            return;
        }
        switch (curGameState) {
            case GameState.End:
                // gameButtton.gameObject.SetActive(true);
                inTurn = false;
                break;
            case GameState.Shuffle:
                // gameButtton.gameObject.SetActive(false);
                inTurn = false;
                break;
            case GameState.LordAsking:
                playerList[curPlayerId].DoLordAsking();
                inTurn = false;
                break;
            case GameState.Playing:
                // Debug.Log("Start playing");
                playerList[curPlayerId].DoUsingCard();
                inTurn = false;
                break;
            default :
                
                break;
        }
    }

    public void OnClickButton () {
        // TestCardMove();
        if (curGameState == GameState.End) {
            // cardPool recycle cards
            vitoryHint.gameObject.SetActive(false);
            cardPool.RecycleAllCards(usingCards, playerList);
            readyCount = 0;
            curBattleScore = 100;
            curGameState = GameState.Shuffle;
            gameButtton.gameObject.SetActive(false);
            Shuffle();
        }
    }
    
    void  Shuffle() {
        cardPool.Shuffle(playerList, curPlayerId);
    }

    public void OnShuffleReady () {
        if (curGameState == GameState.LordAsking) {
            Debug.Log("Coroutin error");
            return;
        }
        readyCount++;
        if (readyCount == 3) {
            readyCount = 0;
            MovebottomCards();
            curGameState = GameState.LordAsking;
            inTurn = true;
        }
        // Debug.Log("OnShuffleReady " + readyCount);
    }

    public void OnLandlordCancel () {
        readyCount++;
        // Debug.Log(curPlayerId + " OnLandlordCancel " + readyCount);
        if (readyCount == 3) {
            OnLandlordConfirm(curPlayerId);
            return;
        }
        curPlayerId++;
        if (curPlayerId == 3) curPlayerId = 0;
        inTurn = true;
    }

    public void OnLandlordConfirm (int id) {
        readyCount = 0;
        hintText.text = "Landlord is Player " + id;
        
        landlordId = id;
        for(int i = 0; i < 3; i++) {
            if (i == landlordId) {
                playerList[i].OnLandlordSelected(true);
            }
            else
            {
                playerList[i].OnLandlordSelected(false);
            }
        }
        DispatBottomCards();
        // Debug.Log("OnLandlordConfirm " + readyCount);
        // Time.timeScale = 0.1f;
    }

    void MovebottomCards() {
        cardPool.MovebottomCards(usingCards);
    }

    void DispatBottomCards() {
        usingCards.ShowAndDispatchBottomCards(playerList[landlordId]);
    }

    public void OnBottomCardsReady () {
        curGameState = GameState.Playing;
        inTurn = true;
    }

    public void OnUsingCardFinished () {
        curPlayerId++;
        if (curPlayerId == 3) curPlayerId = 0;
        inTurn = true;
    }

    public void OnBombUsed () {
        // Debug.Log("OnBombUsed");
        bombEffect.SetActive(true);
        StartCoroutine(DelayDeactiveBomb());
        curBattleScore += 100;
    }

    IEnumerator DelayDeactiveBomb() {
        yield return new WaitForSeconds(1.0f);
        bombEffect.SetActive(false);
    }

    public void OnPlayerUsedAllCards () {
        // curPlayerId win
        hintText.text = "Player " + curPlayerId + " win";
        if (curPlayerId == landlordId) {
            vitoryHint.gameObject.SetActive(true);
            vitoryHint.text = "Landlord Win";
            playerList[curPlayerId].ChangeScore(2 * curBattleScore);
            for(int i = 0; i <= 2; i++) {
                if (i == landlordId) continue;
                playerList[index: i].ChangeScore(-1 * curBattleScore);
            }
        } else {
            vitoryHint.gameObject.SetActive(true);
            vitoryHint.text = "Free Man Win";
            playerList[landlordId].ChangeScore(-2 * curBattleScore);
            for(int i = 0; i <= 2; i++) {
                if (i == landlordId) continue;
                playerList[index: i].ChangeScore(curBattleScore);
            }
        }
        curBattleScore = 100;

        gameButtton.gameObject.SetActive(true);
        curGameState = GameState.End;
        inTurn = true;
    }
}

public enum GameState {
    End,
    Shuffle,
    LordAsking,
    Playing
}