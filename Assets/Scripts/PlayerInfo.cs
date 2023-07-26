using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    Text playerName;
    Text playerRole;
    Text passText;
    Text playerScore;
    Color originColor = Color.white;
    Color onTurnColor = Color.green;

    string playerNameText;
    string playerInitScore;
    bool shouldInitUi = false;

    void Start()
    {
        playerName = transform.Find("PlayerName").gameObject.GetComponent<Text>();
        playerRole = transform.Find("Role").gameObject.GetComponent<Text>();
        passText = transform.Find(n: "Pass").gameObject.GetComponent<Text>();
        playerScore = transform.Find(n: "Score").gameObject.GetComponent<Text>();
        // Debug.Log(playerName + " /st/ " + playerScore);
    }

    void Update()
    {
        if (shouldInitUi) {
            shouldInitUi = false;
            playerName.text = playerNameText;
            playerScore.text = playerInitScore;
        }
    }

    public void SetInfo(string _playerName, int score)
    {
        // update text here sometimes throw playerName not found
        playerNameText = _playerName;
        playerInitScore = score.ToString();
        shouldInitUi = true;

    }

    public void SetLandLord(bool landLord)
    {
        playerRole.text = landLord ? "Lord" : "Free Man";
    }

    public void OnUsing()
    {
        playerName.color = onTurnColor;
        playerRole.color = onTurnColor;
        playerScore.color = onTurnColor;
        passText.color = onTurnColor;
    }

    public void OnUsingEnd()
    {
        playerName.color = originColor;
        playerRole.color = originColor;
        playerScore.color = originColor;
        passText.color = originColor;
    }

    public void OnPass () {
        passText.text = "Pass";
        StartCoroutine(DismissPassEnumerator());
    }

    IEnumerator DismissPassEnumerator()
    {
        yield return new WaitForSeconds(1.0f);
        passText.text = "";
    }

    public void ChangeScore (int value) {
        playerScore.text = value.ToString();
    }
}
