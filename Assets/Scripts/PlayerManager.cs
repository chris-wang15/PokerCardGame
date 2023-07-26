using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    Button confirmButton;
    Button cancelButton;
    Button hintButton;
    Text hintText;
    List<CardDisplay> cards = new List<CardDisplay>();
    internal int id;
    internal bool verticalMode = false;
    GameManager gameManager;
    UsingCards usingCards;
    List<CardDisplay> selectedCards = new List<CardDisplay>();
    Rule rule;
    RuleValue targetRule = null;
    bool enableUnselectListen = false;
    PlayerInfo playerInfo;
    int score = 20000;
    AiController aiController = null;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        usingCards = GameObject.FindGameObjectWithTag("UsingCards").GetComponent<UsingCards>();
        rule = Rule.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        if (enableUnselectListen && Input.GetMouseButtonDown(1))
        {
            // Debug.Log("right click");
            UnSelectAll();
        }
    }

    public void LoadParams(int _id, bool vertical,
     Button _confirmButton, Button _cancelButton, Button _hintButton, Text _hintText)
    {
        id = _id;
        verticalMode = vertical;
        confirmButton = _confirmButton;
        cancelButton = _cancelButton;
        hintButton = _hintButton;
        hintText = _hintText;
        if (id == 0)
        {
            playerInfo = GameObject.FindGameObjectWithTag("LeftPlayerInfo").GetComponent<PlayerInfo>();
        }
        else if (id == 1)
        {
            playerInfo = GameObject.FindGameObjectWithTag("CenterPlayerInfo").GetComponent<PlayerInfo>();
        }
        else
        {
            playerInfo = GameObject.FindGameObjectWithTag("RightPlayerInfo").GetComponent<PlayerInfo>();
        }
        // Debug.Log("pinfo: " + playerInfo);
        playerInfo.SetInfo("Player " + id, score);

        if (DataManager.GetInstance().InitAiControl(id))
        {
            aiController = new AiController(this);
        }
    }

    void SortCards()
    {
        // StartCoroutine(SortCardsEnumerator());
        cards.Sort((card1, card2) =>
        {
            int valueDis = card2.cardValue - card1.cardValue;
            if (valueDis != 0) return valueDis;
            int c1TypeValue = (int)card1.cardType;
            int c2TypeValue = (int)card2.cardType;
            return c2TypeValue - c1TypeValue;
        });
        if (verticalMode)
        {
            foreach (var cardDisplay in cards)
            {
                cardDisplay.transform.SetAsFirstSibling();
            }
        }
        else
        {
            foreach (var cardDisplay in cards)
            {
                cardDisplay.transform.SetAsLastSibling();
            }
        }

        calculateCardsPosition();
    }

    public void AddCards(CardDisplay card)
    {
        cards.Add(card);
        card.transform.parent = transform;
        if (verticalMode)
        {
            card.transform.SetAsFirstSibling();
        }
        calculateCardsPosition();
        if (cards.Count == 17)
        {
            SortCards();
            gameManager.OnShuffleReady();
        }
    }

    void calculateCardsPosition()
    {
        int cardCount = cards.Count;
        if (cardCount == 0)
        {
            return;
        }
        if (cardCount == 1)
        {
            cards[0].transform.localPosition = new Vector3(
                0, 0, 0
            );
            return;
        }
        int centerId = cardCount % 2 == 0 ? cardCount / 2 - 1 : cardCount / 2;
        // float deltaDis = verticalMode ? 55.0f : 80.0f;
        float deltaDis;
        if (verticalMode)
        {
            if (gameManager.curGameState == GameState.Shuffle)
            {
                deltaDis = 55.0f;
            }
            else
            {
                deltaDis = 880.0f / (cardCount - 1);
                if (deltaDis > 200.0f) deltaDis = 200.0f;
                if (deltaDis < 44.0f) deltaDis = 44.0f;
            }
        }
        else
        {
            if (gameManager.curGameState == GameState.Shuffle)
            {
                deltaDis = 80.0f;
            }
            else
            {
                deltaDis = 1520.0f / (cardCount - 1);
                if (deltaDis > 200.0f) deltaDis = 200.0f;
                if (deltaDis < 80.0f) deltaDis = 80.0f;
            }
        }
        for (int i = 0; i < cardCount; i++)
        {
            float dis = (i - centerId) * deltaDis;
            var tmpObj = cards[i];
            if (verticalMode)
            {
                tmpObj.transform.localPosition = new Vector3(
                    0, dis, 0
                );
            }
            else
            {
                tmpObj.transform.localPosition = new Vector3(
                    dis, 0, 0
                );
            }
        }
    }

    public void DoLordAsking()
    {
        hintText.text = "Player " + id + " Run for landlord";
        playerInfo.OnUsing();

        if (aiController != null)
        {
            confirmButton.gameObject.SetActive(value: false);
            cancelButton.gameObject.SetActive(false);
            // Debug.Log("cards count: " + cards.Count);
            StartCoroutine(aiController.DoLordAsking(cards));
        }
        else
        {
            confirmButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirmClicked);
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(OnCancelClicked);
        }
    }

    public void OnConfirmClicked()
    {
        if (gameManager.curGameState == GameState.LordAsking)
        {
            hintText.text = "";
            confirmButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            hintButton.gameObject.SetActive(false);
            gameManager.OnLandlordConfirm(id);
        }
        else if (gameManager.curGameState == GameState.Playing)
        {
            hintText.text = "";
            confirmButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            hintButton.gameObject.SetActive(false);
            enableUnselectListen = false;
            foreach (var selectCard in selectedCards)
            {
                cards.Remove(selectCard);
            }
            if (CardRuleUtils.IsBomb(selectedCards)) gameManager.OnBombUsed();
            usingCards.AddCards(selectedCards, id);
            selectedCards.Clear();
            TurnOffSelectMode();
            if (cards.Count == 0)
            {
                // win battle
                gameManager.OnPlayerUsedAllCards();
            }
            else
            {
                gameManager.OnUsingCardFinished();
                calculateCardsPosition();
            }
        }
        playerInfo.OnUsingEnd();
        targetRule = null;
    }

    public void OnCancelClicked()
    {
        if (gameManager.curGameState == GameState.LordAsking)
        {
            hintText.text = "";
            confirmButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            hintButton.gameObject.SetActive(false);
            gameManager.OnLandlordCancel();
        }
        else if (gameManager.curGameState == GameState.Playing)
        {
            // pass cards
            hintText.text = "";
            confirmButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            hintButton.gameObject.SetActive(false);
            enableUnselectListen = false;
            gameManager.OnUsingCardFinished();
            playerInfo.OnPass();
            if (selectedCards.Count > 0) UnSelectAll();
            TurnOffSelectMode();
        }
        playerInfo.OnUsingEnd();
        targetRule = null;
    }

    public void AddBootomCards(List<CardDisplay> bottomCards)
    {
        foreach (var card in bottomCards)
        {
            cards.Add(card);
            card.transform.parent = transform;
            if (verticalMode)
            {
                card.transform.SetAsFirstSibling();
            }
            else
            {
                card.transform.SetAsLastSibling();
            }
        }
        calculateCardsPosition();
        StartCoroutine(DelaySortEnumerator());
    }

    IEnumerator DelaySortEnumerator()
    {
        yield return new WaitForSeconds(1.0f);
        SortCards();
        gameManager.OnBottomCardsReady();
    }

    public void DoUsingCard()
    {
        playerInfo.OnUsing();
        var preUsing = usingCards.GetShowing();
        if (preUsing.Count != 0 && usingCards.usingCardPlayerId != id)
        {
            targetRule = rule.GetRule(preUsing);
            hintText.text = GetCardDealingHintText(targetRule);
        }
        else
        {
            hintText.text = "Player " + id + " use card";
            targetRule = null;
        }

        if (aiController != null)
        {
            StartCoroutine(aiController.DoUsingCard());
            return;
        }

        confirmButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(true);
        hintButton.gameObject.SetActive(true);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(OnCancelClicked);
        hintButton.onClick.RemoveAllListeners();
        hintButton.onClick.AddListener(OnHintButtonClicked);

        if (targetRule != null)
        {
            if (targetRule.ruleType == RuleType.Joker_Bomb)
            {
                TurnOffSelectMode();
            }
            else
            {
                TurnOnSelectMode();
            }
        }
        else
        {
            cancelButton.gameObject.SetActive(value: false);
            TurnOnSelectMode();
        }

        enableUnselectListen = true;
    }

    private void TurnOnSelectMode()
    {
        foreach (var card in cards)
        {
            card.SetSelectMode(this);
        }
    }

    private void TurnOffSelectMode()
    {
        foreach (var card in cards)
        {
            card.SetUnSelectMode();
        }
    }

    private void UnSelectAll()
    {
        foreach (var selectedCard in selectedCards)
        {
            selectedCard.OnUnSelectAll(id);
        }
        selectedCards.Clear();
        UpdateUiAfterSelectChanged();
    }

    public void OnSelectCard(CardDisplay card)
    {
        selectedCards.Add(card);
        UpdateUiAfterSelectChanged();
    }

    public void OnUnSelectCard(CardDisplay card)
    {
        selectedCards.Remove(card);
        UpdateUiAfterSelectChanged();
    }

    private void UpdateUiAfterSelectChanged()
    {
        if (aiController != null) return;
        if (targetRule == null)
        {
            // can use anly type, but can not pass
            cancelButton.gameObject.SetActive(value: false);
            if (selectedCards.Count == 0)
            {
                hintText.text = "Please use any card";
                confirmButton.gameObject.SetActive(false);
            }
            else
            {
                RuleType ruleType = rule.GetRule(selectedCards).ruleType;
                if (ruleType == RuleType.Wrong)
                {
                    confirmButton.gameObject.SetActive(false);
                    hintText.text = "Not comply with the using card rules";
                }
                else
                {
                    hintText.text = "Please confirm";
                    confirmButton.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            cancelButton.gameObject.SetActive(value: true);
            if (selectedCards.Count == 0)
            {
                hintText.text = GetCardDealingHintText(targetRule);
                confirmButton.gameObject.SetActive(false);
            }
            else
            {
                RuleValue selectRule = rule.GetRule(selectedCards);
                if (targetRule.ruleType == RuleType.Joker_Bomb)
                {
                    Debug.Log("Internal error for joker bomb");
                }
                else if (targetRule.ruleType == RuleType.Bomb)
                {

                    if ((selectRule.ruleType == RuleType.Bomb && selectRule.value > targetRule.value)
                     || selectRule.ruleType == RuleType.Joker_Bomb)
                    {
                        hintText.text = "Please confirm";
                        confirmButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        hintText.text = "Select bomb or pass";
                        confirmButton.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (selectRule.ruleType == RuleType.Bomb || selectRule.ruleType == RuleType.Joker_Bomb)
                    {
                        hintText.text = "Please confirm";
                        confirmButton.gameObject.SetActive(true);
                    }
                    else if (selectRule.ruleType == targetRule.ruleType)
                    {
                        if (selectRule.value > targetRule.value)
                        {
                            hintText.text = "Please confirm";
                            confirmButton.gameObject.SetActive(true);
                        }
                        else
                        {
                            hintText.text = "Only larger cards can use";
                            confirmButton.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        hintText.text = "Not comply with the using card rules";
                        confirmButton.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private string GetCardDealingHintText(RuleValue ruleValue)
    {
        string playerHint = "Player " + id + " dealing, " + "Player " + usingCards.usingCardPlayerId;
        switch (ruleValue.ruleType)
        {
            case RuleType.Single:
                return playerHint + " used " + GetValueString(ruleValue.value);
            case RuleType.Joker_Bomb:
                return playerHint + " used Joker Bomb";
            case RuleType.Pair:
                return playerHint + " used Pair " + GetValueString(ruleValue.value);
            case RuleType.Bomb:
                return playerHint + " used Bomb " + GetValueString(ruleValue.value);
            case RuleType.Tri:
                return playerHint + " used Tri " + GetValueString(ruleValue.value);
            case RuleType.Flush:
                return playerHint + " used Flush " + GetValueString(ruleValue.value)
                + " to " + getLastFlush(ruleValue.value, usingCards.GetShowingCount());
            case RuleType.Tri_One:
                return playerHint + " used tri " + GetValueString(ruleValue.value) + " carry one";
            case RuleType.Four_Two:
                return playerHint + " used four " + GetValueString(ruleValue.value) + " carry two";
            case RuleType.Pair_Flush:
                return playerHint + " used pair flush " + GetValueString(ruleValue.value) + " to "
                + getLastPairFlush(ruleValue.value, usingCards.GetShowingCount());
            default:
                Debug.Log("Wrong type used: " + ruleValue);
                return "Please use cards";
        }
    }

    private string getLastFlush(int init, int count)
    {
        int lastValue = init + count - 1;
        return GetValueString(lastValue);
    }

    private string getLastPairFlush(int init, int count)
    {
        int lastValue = count / 2 + init - 1;
        return GetValueString(lastValue);
    }

    private string GetValueString(int valueId)
    {
        return CardRuleUtils.GetValueString(valueId);
    }

    public void OnLandlordSelected(bool landLord)
    {
        playerInfo.SetLandLord(landLord);
    }

    public void OnHintButtonClicked()
    {
        TryHintStartegy();
    }

    public bool TryHintStartegy () {
        UnSelectAll();
        if (targetRule == null)
        {
            // Debug.Log("usingCards.usingCardPlayerId: " + usingCards.usingCardPlayerId);
            // use any card
            AiUsingStrategy aiUsingStrategy = new AiUsingStrategy();
            aiUsingStrategy.LoadHandCards(new List<CardDisplay>(cards));
            var aiSelect = aiUsingStrategy.TryUsingAnyCard();
            foreach (var card in aiSelect)
            {
                card.OnSelectByHint(id);
                selectedCards.Add(card);
            }
            UpdateUiAfterSelectChanged();
            return true;
        } 
        else
        {
            var handCards = new List<CardDisplay>();
            handCards.AddRange(cards);
            var hintSelect = CardRuleUtils.GetLarger(handCards, targetRule);
            if (hintSelect.Count == 0)
            {
                OnCancelClicked();
                return false;
            }
            else
            {
                foreach (var card in hintSelect)
                {
                    card.OnSelectByHint(id);
                    selectedCards.Add(card);
                }
                UpdateUiAfterSelectChanged();
                return true;
            }
        }
    }

    public void ChangeScore(int scoreChange)
    {
        score += scoreChange;
        playerInfo.ChangeScore(score);
    }

    public List<CardDisplay> RecycleAllCards()
    {
        List<CardDisplay> recycleCards = new List<CardDisplay>();
        if (cards.Count > 0)
        {
            recycleCards.AddRange(cards);
        }
        cards.Clear();
        return recycleCards;
    }
}
