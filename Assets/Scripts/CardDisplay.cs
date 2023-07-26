using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
{
    Image cardFront;
    Image cardBack;
    Transform favePoint;

    float rotateLimit = 180.0f;
    float rotateSpeed = 180.0f;
    float curRotateCount = 0.0f;

    bool shouldMove = false;
    Vector3 targetPos;
    float speed = 2000.0f;
    Action moveEndCallback = null;
    internal string cardName;
    internal int cardValue;
    internal CardType cardType;
    // bool showingBack = false;
    PlayerManager playerManager = null;
    float selectDis = 50.0f;
    bool selected = false;

    void Start()
    {
        Transform cardFrontTrans = transform.Find("Canvas/Panel/CardFace");
        Transform cardBackTrans = transform.Find("Canvas/Panel/CardBack");
        cardFront = cardFrontTrans.GetComponent<Image>();
        cardBack = cardBackTrans.GetComponent<Image>();
        favePoint = transform.Find("FacePoint");

        // Note there should be only one Resources file, and ignore .png
        var path = "Pics/PlayingCards/" + cardName;
        var sp  = Resources.Load<Sprite>(path);
        if (sp == null) Debug.Log("sp: " + cardName);
        // Debug.Log("cardFront: " + cardFront);
        cardFront.sprite = sp;

        // use back as init state; this will trigger bug
        // RotateImmediately();
    }

    public void LoadParams (string name, int value, CardType type) {
        cardName = name;
        cardValue = value;
        cardType = type;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(transform.localEulerAngles);
        var angleY = transform.localEulerAngles.y;
        if (angleY > 90.0f && angleY < 270.0f) {
            // should show back
            cardBack.gameObject.SetActive(true);
            cardFront.gameObject.SetActive(value: false);
        } else {
            // should show face
            cardBack.gameObject.SetActive(false);
            cardFront.gameObject.SetActive(true);
        }

        // var facePointZ = favePoint.position.z;
        // if (facePointZ > 0) {
        //     // should show back
        //     // if (!showingBack) {
        //     //     cardBack.gameObject.SetActive(true);
        //     //     cardFront.gameObject.SetActive(value: false);
        //     //     showingBack = true;
        //     // }
        //     cardBack.gameObject.SetActive(true);
        //     cardFront.gameObject.SetActive(value: false);
        // } else {
        //     // should show face
        //     // if (showingBack) {
        //     //     cardBack.gameObject.SetActive(false);
        //     //     cardFront.gameObject.SetActive(true);
        //     //     showingBack = false;
        //     // }
        //     cardBack.gameObject.SetActive(false);
        //     cardFront.gameObject.SetActive(true);
        // }

        if (shouldMove && curRotateCount > 0) {
            Debug.Log("Card rotate not work while moving");
        }

        if (curRotateCount > 0) {
            var rotateCount = rotateSpeed * Time.deltaTime;
            if (curRotateCount >= rotateCount) {
                curRotateCount -= rotateCount;
            } else {
                rotateCount = curRotateCount;
                curRotateCount = 0;
            }
            

            transform.Rotate(
                0, rotateCount, 0, Space.Self
            );
        }

        if (shouldMove) {
            float dis = Vector3.Distance(targetPos, transform.position);
            if (dis < 0.1) {
                shouldMove = false;
                if (moveEndCallback != null) {
                    moveEndCallback();
                    moveEndCallback = null;
                }
            } else {
                transform.position = Vector3.MoveTowards(
                    current: transform.position, 
                    targetPos,
                    speed * Time.deltaTime
                );
            }
        }
    }

    public void RotateImmediately() {
        transform.Rotate(
            0, 180f, 0, Space.Self
        );
    }

    public void Rotate() {
        if (curRotateCount > 0) {
            Debug.Log("previous rotation not finished");
            return;
        }
        curRotateCount = rotateLimit;
    }

    public void MoveWithCallback (Vector3 _targetPos, Action callback) {
        targetPos = _targetPos;
        moveEndCallback = callback;
        shouldMove = true;
    }

    public void SetSelectMode (PlayerManager pm) {
        if (playerManager != null) Debug.Log(playerManager.id + " pm not clear after using");
        playerManager = pm;
    }

    public void SetUnSelectMode () {
        playerManager = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log(cardName + " OnPointerClick " + playerManager);
        if (playerManager == null 
        || eventData.button != PointerEventData.InputButton.Left
        ) {
            return;
        }
        if (selected) {
            OnUnSelected();
        } else {
            OnSelected();
        }
    }

    void OnSelected () {
        OnSelectByHint(playerManager.id);
        // Debug.Log(cardName + " OnSelected");
        playerManager.OnSelectCard(this);
    }

    void OnUnSelected () {
        OnUnSelectAll(playerManager.id);
        playerManager.OnUnSelectCard(this);
    }

    public void OnUnSelectAll (int id) {
        if (!selected) return;
        selected = false;
        if (id == 0) {
            transform.localPosition = new Vector3(
                transform.localPosition.x - selectDis,
                transform.localPosition.y,
                transform.localPosition.z
            );
        } else if (id == 1) {
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y - selectDis,
                transform.localPosition.z
            );
        } else {
            transform.localPosition = new Vector3(
                transform.localPosition.x + selectDis,
                transform.localPosition.y,
                transform.localPosition.z
            );
        }
        // Debug.Log(cardName + " OnUnSelectedAll");
    }

    public void OnSelectByHint (int id) {
        if (selected) return;
        selected = true;
        transform.localScale = new Vector3(1.0f,1.0f,0);
        if (id == 0) {
            transform.localPosition = new Vector3(
                transform.localPosition.x + selectDis,
                transform.localPosition.y,
                transform.localPosition.z
            );
        } else if (id == 1) {
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y + selectDis,
                transform.localPosition.z
            );
        } else {
            transform.localPosition = new Vector3(
                transform.localPosition.x - selectDis,
                transform.localPosition.y,
                transform.localPosition.z
            );
        }
        // Debug.Log("pmid: " + playerManager.id);
        // Debug.Log("transform.localEulerAngles: " + transform.localEulerAngles);
        // Debug.Log("transform.localPosition: " + transform.localPosition);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (playerManager == null) return;
        // Debug.Log("OnPointerExit");
        transform.localScale = new Vector3(1.0f,1.0f,0);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (playerManager == null) return;
        // Debug.Log("OnPointerEnter");
        transform.localScale = new Vector3(1.1f,1.1f,0);
    }

    public void RotateToBackImediately () {
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        // transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    public void ResetState () {
        selected = false;
        playerManager = null;
    }
}

public enum CardType {
    Spade = 0,
    Heart = 1,
    Club = 2,
    Diamond = 3,
    Joker_Monochrome = 4,
    Joker_Color = 5
}
