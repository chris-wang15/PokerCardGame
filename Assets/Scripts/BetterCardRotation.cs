using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class BetterCardRotation : MonoBehaviour
{
    public RectTransform CardFront;
    public RectTransform CardBack;
    public Transform targetFacePoint;
    public Collider col;

    bool showingBack = false;

    void Start()
    {
        
    }
    
    void Update()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(
            Camera.main.transform.position,
            (-Camera.main.transform.position + targetFacePoint.position).normalized,
            (-Camera.main.transform.position + targetFacePoint.position).magnitude
        );
        bool passedThroughColliderOnCard = false;
        foreach(RaycastHit h in hits)
        {
            if(h.collider == col) passedThroughColliderOnCard = true;
        }
        if (passedThroughColliderOnCard != showingBack)
        {
            showingBack = passedThroughColliderOnCard;
            if (showingBack)
            {
                CardFront.gameObject.SetActive(false);
                CardBack.gameObject.SetActive(true);
            } else
            {
                CardFront.gameObject.SetActive(true);
                CardBack.gameObject.SetActive(false);
            }
        }
    }
}
