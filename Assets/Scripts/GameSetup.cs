using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{
    public Cards[] card;
    public List<Cards> Deck = new List<Cards>();
    public List<Cards> Player = new List<Cards>();
    public List<Cards> Ai = new List<Cards>();
    public GameObject Card;
    public Canvas canvas;
    public GameObject playerHand, AiHand;

    // Start is called before the first frame update
    void Start()
    {
        //initialize the Deck the for loop defines the number of times each card is repeated in the deck eg :- 6
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < card.Length; j++)
            {
                Deck.Add(card[j]);
            }
        }
        StartCoroutine("Setup");
        

    }


    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Setup()
    {
        int x = -224;
        //here the for loop defines how many cards player and Ai gets in the beginning for eg :- 7
        for (int i = 0; i < 7; i++)
        {
            int a = Random.Range(0, (Deck.Count - 1));
            Player.Add(Deck[a]);
            GameObject temp = Instantiate(Card);
            temp.GetComponent<RectTransform>().SetParent(canvas.transform, false);
            temp.GetComponent<CardValue>().card = Deck[a];
            temp.GetComponent<Image>().sprite = Deck[a].Artwork;
            //move animation used by implementing DoTween package 
            temp.GetComponent<RectTransform>().DOAnchorPos(new Vector2(x, -224), .5f, true);
            Deck.RemoveAt(a);
            temp.tag = "Player";
            yield return new WaitForSeconds(.25f);
            int b = Random.Range(0, (Deck.Count - 1));
            Ai.Add(Deck[b]);
            GameObject temp1 = Instantiate(Card);
            temp1.GetComponent<RectTransform>().SetParent(canvas.transform, false);
            temp1.GetComponent<Image>().sprite = Deck[a].Artwork;
            temp1.GetComponent<CardValue>().card = Deck[a];
            temp1.GetComponent<DragAndDrop>().enabled = false;
            //move animation used by implementing DoTween package 
            temp1.GetComponent<RectTransform>().DOAnchorPos(new Vector2(x, 224), .5f, true);
            Deck.RemoveAt(b);
            temp1.tag = "Ai";
            x += (72);
            yield return new WaitForSeconds(.25f);
        }
        yield return new WaitForSeconds(.26f);
        //once all the cards are dealt the below code transfers the card to their respective Hand ie Player hand and Ai's hand
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject g in player) {
            g.GetComponent<RectTransform>().SetParent(playerHand.transform);
        }
        GameObject[] Aiplayer = GameObject.FindGameObjectsWithTag("Ai");
        foreach (GameObject g in Aiplayer)
        {
            g.GetComponent<RectTransform>().SetParent(AiHand.transform);
        }

    }
    public void Pmove(float num) {
        StartCoroutine("PlayerDraw",num);
    }
    public void AiMove(float num)
    {
        StartCoroutine("AiDraw", num);
    }
    IEnumerator PlayerDraw(float basex) {
        yield return new WaitForSeconds(.5f);
        int a = Random.Range(0, (Deck.Count - 1));
        Player.Add(Deck[a]);
        GameObject temp = Instantiate(Card);
        temp.GetComponent<RectTransform>().SetParent(canvas.transform, false);
        temp.GetComponent<CardValue>().card = Deck[a];
        temp.GetComponent<Image>().sprite = Deck[a].Artwork;
        //move animation used by implementing DoTween package 
        temp.GetComponent<RectTransform>().DOAnchorPos(new Vector2(basex, -224), .5f, true);
        Deck.RemoveAt(a);
        temp.tag = "Player";
        yield return new WaitForSeconds(.51f);
        temp.GetComponent<RectTransform>().SetParent(playerHand.transform);
        
    }

    IEnumerator AiDraw(float basex) {
        yield return new WaitForSeconds(.5f);
        int b = Random.Range(0, (Deck.Count - 1));
        Ai.Add(Deck[b]);
        GameObject temp1 = Instantiate(Card);
        temp1.GetComponent<RectTransform>().SetParent(canvas.transform, false);
        temp1.GetComponent<Image>().sprite = Deck[b].Artwork;
        temp1.GetComponent<CardValue>().card = Deck[b];
        temp1.GetComponent<DragAndDrop>().enabled = false;
        //move animation used by implementing DoTween package 
        temp1.GetComponent<RectTransform>().DOAnchorPos(new Vector2(basex, 224), .5f, true);
        Deck.RemoveAt(b);
        temp1.tag = "Ai";
        yield return new WaitForSeconds(.51f);
        temp1.GetComponent<RectTransform>().SetParent(AiHand.transform);
    }

}
