using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GamePlay : MonoBehaviour
{
    public GameSetup gs;
    public GameObject Deck;
    public GameObject Player;
    public GameObject Ai;
    public GameObject[] rows;
    public GameObject playerDiscardDeck;
    public GameObject AiDiscardDeck;
    public Transform Aimove;
    // Start is called before the first frame update
    void Start()
    {
        //when the game starts the Player can only place card in the first row the below for loop disables rest of the rows
        for (int a = 1; a < rows.Length; a++) {
            rows[a].GetComponent<CardSlots>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextTurn() {
        //in each turn first the Ai plays the card, then a nested Coroutine initiates where each card move one step ahead
        //finally the last coroutine executes to resolve the conflict if any 
        StartCoroutine("PlayAi");
    }
    public IEnumerator PlayAi()
    {
        //the below section of code selects one card at random from Ai's hand and moves it to the first row next to Ai's Hand
        if (rows[5].tag.Equals("Untagged"))
        {
            Aimove = Ai.transform.GetChild(Random.Range(0, Ai.transform.childCount));
            Aimove.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -64), .3f, true);
            yield return new WaitForSeconds(.3f);
            Aimove.GetComponent<RectTransform>().SetParent(rows[5].transform);
            rows[5].GetComponent<CardSlots>().cardType = Aimove.GetComponent<CardValue>().card.Cardname;
            rows[5].tag = "Ai Cards";
        }

        //Move to next row
        StartCoroutine("MoveAhead");
    }

    public IEnumerator MoveAhead()
    {
        //Move Player Cards
        //in each iteration the card moves one row above in doing so it will rename the tag for that row so it becomes players row
        //it will assign cardtype string a value in doing so card of only that type can be added to the row if needed in future
        for (int i = (rows.Length-1); i > 0 ; i--) {
            if (rows[i].tag.Equals("Untagged")) {
                if (rows[(i - 1)].tag.Equals("Untagged")) {
                    
                }
                else{
                    if (rows[(i-1)].tag.Equals("Player Cards")) {
                        rows[i].GetComponent<CardSlots>().enabled = true;
                        while (rows[(i - 1)].transform.childCount > 0) { 
                            
                            string cardType = rows[(i - 1)].transform.GetChild(0).GetComponent<CardValue>().card.Cardname;
                            rows[(i - 1)].transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(rows[(i - 1)].transform.GetChild(0).transform.localPosition.x, 64), .3f, true);
                            yield return new WaitForSeconds(.3f);
                            rows[(i - 1)].transform.GetChild(0).GetComponent<RectTransform>().SetParent(rows[i].transform);
                            rows[(i - 1)].tag = "Untagged";
                            rows[i].tag = "Player Cards";
                            rows[i].GetComponent<CardSlots>().cardType = cardType;
                            rows[(i-1)].GetComponent<CardSlots>().cardType = "";
                        }

                    }
                }
            }
        }
        yield return new WaitForSeconds(.1f);

        //Move Ai cards
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].tag.Equals("Untagged") && i<(rows.Length-1))
            {
                if (rows[(i + 1)].tag.Equals("Untagged"))
                {
                    
                }
                else
                {
                    if (rows[(i + 1)].tag.Equals("Ai Cards"))
                    {

                        while (rows[(i + 1)].transform.childCount > 0)
                        {
                            string cardType = rows[(i + 1)].transform.GetChild(0).GetComponent<CardValue>().card.Cardname;
                            rows[(i + 1)].transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(rows[(i + 1)].transform.GetChild(0).transform.localPosition.x, -64), .3f, true);
                            yield return new WaitForSeconds(.3f);
                            rows[(i + 1)].transform.GetChild(0).GetComponent<RectTransform>().SetParent(rows[i].transform);
                            rows[(i + 1)].tag = "Untagged";
                            rows[i].tag = "Ai Cards";
                            rows[i].GetComponent<CardSlots>().cardType = cardType;
                            rows[(i + 1)].GetComponent<CardSlots>().cardType = "";
                        }

                    }
                }
            }
        }
        //conflict Resolve
        StartCoroutine("conflictResolve");
    }
    public IEnumerator conflictResolve()      
    {
        //each iteration will first check if two adjacent rows are conflicting rows for ex players hand and Ai's hand based on the tag.
        //if so it will take into account the number of cards in those rows
        //then will run a loop to determing if the opositions card is weaker if so the player/Ai dies

        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].tag.Equals("Player Cards") && i < (rows.Length - 1))
            {
                if (rows[(i + 1)].tag.Equals("Ai Cards"))
                {
                    int PlayerCount = rows[i].transform.childCount;
                    int AiCount = rows[(i+1)].transform.childCount;
                    Cards player = rows[i].transform.GetChild(0).GetComponent<CardValue>().card;
                    Cards ai = rows[(i+1)].transform.GetChild(0).GetComponent<CardValue>().card;

                    foreach (string str in player.weakness) {
                        if (str.Equals(ai.Cardname)) {
                            Debug.Log("Player Dies");
                            if (AiCount >= PlayerCount)
                            {
                                for (int b = 0; b < rows[i].transform.childCount; b++)
                                {
                                    //Destroy(rows[i].transform.GetChild(b).gameObject);
                                    //move card to discard pile
                                    rows[i].transform.GetChild(b).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-350, -60), .5f, true);
                                    yield return new WaitForSeconds(.55f);
                                    rows[i].transform.GetChild(b).GetComponent<RectTransform>().SetParent(playerDiscardDeck.transform);
                                }
                            }
                            else
                            {
                                for (int b = 0; b < AiCount; b++)
                                {
                                    //Destroy(rows[i].transform.GetChild(b).gameObject);
                                    //move card to discard pile
                                    rows[i].transform.GetChild(b).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-350, -60), .5f, true);
                                    yield return new WaitForSeconds(.55f);
                                    rows[i].transform.GetChild(b).GetComponent<RectTransform>().SetParent(playerDiscardDeck.transform);
                                }
                            }  
                        }
                    }
                    

                    foreach (string str in ai.weakness) {
                        if (str.Equals(player.Cardname)) {
                            Debug.Log("Ai Dies");
                            if (PlayerCount >= AiCount)
                            {
                                for (int b = 0; b < rows[(i + 1)].transform.childCount; b++)
                                {
                                    //Destroy(rows[(i + 1)].transform.GetChild(b).gameObject);
                                    rows[(i + 1)].transform.GetChild(b).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-350, 150), .5f, true);
                                    yield return new WaitForSeconds(.55f);
                                    rows[(i + 1)].transform.GetChild(b).GetComponent<RectTransform>().SetParent(AiDiscardDeck.transform);

                                }
                            }
                            else
                            {
                                for (int b = 0; b < AiCount; b++)
                                {
                                    //Destroy(rows[(i + 1)].transform.GetChild(b).gameObject);
                                    rows[(i + 1)].transform.GetChild(b).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-350, 150), .5f, true);
                                    yield return new WaitForSeconds(.55f);
                                    rows[(i + 1)].transform.GetChild(b).GetComponent<RectTransform>().SetParent(AiDiscardDeck.transform);
                                }
                            }
                        }
                    }
                    //in case of both cards are same
                    if (ai.Cardname.Equals(player.Cardname)) {
                        int loopCount = Mathf.Min(rows[i].transform.childCount, rows[(i + 1)].transform.childCount);
                        for (int c = 0; c < loopCount; c++) {

                            rows[i].transform.GetChild(c).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-350, -60), .5f, true);
                            yield return new WaitForSeconds(.55f);
                            rows[i].transform.GetChild(c).GetComponent<RectTransform>().SetParent(playerDiscardDeck.transform);
                            rows[i].tag = "Untagged";
                            rows[(i + 1)].transform.GetChild(c).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-350, 150), .5f, true);
                            yield return new WaitForSeconds(.55f);
                            rows[(i + 1)].transform.GetChild(c).GetComponent<RectTransform>().SetParent(AiDiscardDeck.transform);
                            rows[(i + 1)].tag = "Untagged";
                            StartCoroutine("PlayerMove");
                            StartCoroutine("AiMove");


                        }
                    }
                    yield return new WaitForSeconds(1f);
                    //Move Ahead
                    if (rows[i].transform.childCount == 0) {
                        Debug.Log("Ai Should Move");
                        rows[i].tag = "Untagged";
                        StartCoroutine("AiMove");
                    }
                    else if (rows[(i + 1)].transform.childCount == 0) {
                        Debug.Log("Player Should Move");
                        rows[(i+1)].tag = "Untagged";
                        StartCoroutine("PlayerMove");

                    }
                    else {
                        Debug.Log("Nobody moves");
                    }
                }
            }
        }
        yield return new WaitForSeconds(.3f);
    }
    public IEnumerator AiMove() {
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].tag.Equals("Untagged") && i < (rows.Length - 1))
            {
                if (rows[(i + 1)].tag.Equals("Untagged"))
                {

                }
                else
                {
                    if (rows[(i + 1)].tag.Equals("Ai Cards"))
                    {

                        while (rows[(i + 1)].transform.childCount > 0)
                        {
                            string cardType = rows[(i + 1)].transform.GetChild(0).GetComponent<CardValue>().card.Cardname;
                            rows[(i + 1)].transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(rows[(i + 1)].transform.GetChild(0).transform.localPosition.x, -64), .3f, true);
                            yield return new WaitForSeconds(.3f);
                            rows[(i + 1)].transform.GetChild(0).GetComponent<RectTransform>().SetParent(rows[i].transform);
                            rows[(i + 1)].tag = "Untagged";
                            rows[i].tag = "Ai Cards";
                            rows[i].GetComponent<CardSlots>().cardType = cardType;
                            rows[(i + 1)].GetComponent<CardSlots>().cardType = "";
                        }

                    }
                }
            }
        }
    }

    public IEnumerator PlayerMove() {
        for (int i = (rows.Length - 1); i > 0; i--)
        {
            if (rows[i].tag.Equals("Untagged"))
            {
                if (rows[(i - 1)].tag.Equals("Untagged"))
                {

                }
                else
                {
                    if (rows[(i - 1)].tag.Equals("Player Cards"))
                    {
                        rows[i].GetComponent<CardSlots>().enabled = true;
                        while (rows[(i - 1)].transform.childCount > 0)
                        {

                            string cardType = rows[(i - 1)].transform.GetChild(0).GetComponent<CardValue>().card.Cardname;
                            rows[(i - 1)].transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(rows[(i - 1)].transform.GetChild(0).transform.localPosition.x, 64), .3f, true);
                            yield return new WaitForSeconds(.3f);
                            rows[(i - 1)].transform.GetChild(0).GetComponent<RectTransform>().SetParent(rows[i].transform);
                            rows[(i - 1)].tag = "Untagged";
                            rows[i].tag = "Player Cards";
                            rows[i].GetComponent<CardSlots>().cardType = cardType;
                            rows[(i - 1)].GetComponent<CardSlots>().cardType = "";
                        }

                    }
                }
            }
        }
    }

}
