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
    public Transform Pmove;
    // Start is called before the first frame update
    void Start()
    {
        //when the game starts the Player can only place card in the first row the below for loop disables rest of the rows
        for (int a = 1; a < rows.Length; a++) {
            rows[a].GetComponent<CardSlots>().enabled = false;
        }
    }

    public void NextTurn() {
        //in each turn first the Ai plays the card, then a nested Coroutine initiates where each card move one step ahead
        //finally the last coroutine executes to resolve the conflict if any 
        StartCoroutine("PlayAi");
    }

    public IEnumerator PlayAi()
    {
        //the below section of code selects one card at random from Ai's hand and moves it to the first row next to Ai's Hand
        if (rows[5].tag.Equals("Untagged") && Ai.transform.childCount>0)
        {
            Aimove = Ai.transform.GetChild(Random.Range(0, Ai.transform.childCount-1));
            Aimove.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(Aimove.transform.localPosition.x, -64), .3f, true);
            yield return new WaitForSeconds(.3f);
            Aimove.GetComponent<RectTransform>().SetParent(rows[5].transform);
            rows[5].GetComponent<CardSlots>().cardType = Aimove.GetComponent<CardValue>().card.Cardname;
            rows[5].tag = "Ai Cards";
            gs.AiMove(Aimove.transform.localPosition.x);
        }
        StartCoroutine("MoveAllAiCards");

        //resolving conflict if any. The move will happen after the conflict
        StartCoroutine("conflictResolve");
    }
    public IEnumerator MoveAllAiCards()
    {
        for (int a = 0; a < Ai.transform.childCount; a++)
        {
            if (Ai.transform.GetChild(a).GetComponent<CardValue>().card.Cardname.Equals(rows[5].GetComponent<CardSlots>().cardType))
            {
                Aimove = Ai.transform.GetChild(a);
                Aimove.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(Aimove.transform.localPosition.x, -64), .3f, true);
                yield return new WaitForSeconds(.3f);
                Aimove.GetComponent<RectTransform>().SetParent(rows[5].transform);
                a--;
                gs.AiMove(Aimove.transform.localPosition.x);
            }
        }
        yield return new WaitForSeconds(.3f);
    }

    public void MoveAllplayerCards() {
        StartCoroutine("MoveAllPlayerCards");
    }
    public IEnumerator MoveAllPlayerCards()
    {
        for (int a = 0; a < Player.transform.childCount; a++) {
            if (Player.transform.GetChild(a).GetComponent<CardValue>().card.Cardname.Equals(rows[0].GetComponent<CardSlots>().cardType)) {
                Pmove = Player.transform.GetChild(a);
                Pmove.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(Pmove.transform.localPosition.x, 64), .3f, true);
                yield return new WaitForSeconds(.31f);
                Pmove.GetComponent<RectTransform>().SetParent(rows[0].transform);
                a--;
                gs.Pmove(Pmove.transform.localPosition.x);
            }
        }
        yield return new WaitForSeconds(.3f);
    }

        public IEnumerator conflictResolve()
    {
        //each iteration will first check if two adjacent rows are conflicting rows for ex players hand and Ai's hand based on the tag.
        //if so it will take into account the number of cards in those rows
        //then will run a loop to determing if the opositions card is weaker if so the player/Ai dies

        // Looking for the row ID where conflict happens
        int conflictRowId = -1;
        // Removing the huge loop, looking for the conflict row id (player row) where a conflict is happening
        // Loop ends if conflictRowId is found, or if no conflict is found until the last enemy row)
        for (int i = 0; i < rows.Length - 1 && conflictRowId == -1; i++)
        {
            if (rows[i].tag.Equals("Player Cards") && (rows[(i + 1)].tag.Equals("Ai Cards")))
            {
                conflictRowId = i;
            }
        }
        // Nothing to do if we didn't find any conflict
        if (conflictRowId == -1)
        {
            Debug.Log("No conflict yet");
            yield return new WaitForSeconds(.3f);
            StartCoroutine("MoveAhead");
        }
        else
        {
            Debug.Log("Conflict happening");

            int i = conflictRowId;

            int PlayerCount = rows[i].transform.childCount;
            int AiCount = rows[(i + 1)].transform.childCount;
            Cards player = rows[i].transform.GetChild(0).GetComponent<CardValue>().card;
            Cards ai = rows[(i + 1)].transform.GetChild(0).GetComponent<CardValue>().card;
            foreach (Transform t in rows[(i + 1)].transform) {
                t.GetComponent<Image>().sprite = t.GetComponent<CardValue>().card.Artwork;
                yield return new WaitForSeconds(.2f);
            }
            yield return new WaitForSeconds(.5f);
            // Saving current weight of counts for the normal case where there is no strenght/weakness involved
            int PlayerCountWeighted = PlayerCount;
            int AiCountWeighted = AiCount;

            // Checking if player is strong
            if (player.Strength[0].Equals(ai.Cardname))
            {
                // We will loop over AI cards for twice the number of player cards
                PlayerCountWeighted *= 2;
                // We will loop over Player cards for half the number of player cards
                AiCountWeighted /= 2;
            }

            // Checking if Ai is strong
            else if (player.weakness[0].Equals(ai.Cardname))
            {
                // We will loop over AI cards for twice the number of player cards
                AiCountWeighted *= 2;
                // We will loop over Player cards for half the number of player cards
                PlayerCountWeighted /= 2;
            }

            /** The PlayerCount and AiCount variable are now weighted with strength if needed */

            // Removing all the player cards that we need to
            // Going in reverse order since we remove elements of the table as we iterate in it
            for (int j = AiCount - 1; j >= 0 && PlayerCountWeighted > 0; j--)
            {
                PlayerCountWeighted--;
                rows[(i + 1)].transform.GetChild(j).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-350, 150), .5f, true);
                rows[(i + 1)].transform.GetChild(j).tag = "Untagged";
                yield return new WaitForSeconds(.55f);
                rows[(i + 1)].transform.GetChild(j).GetComponent<RectTransform>().SetParent(AiDiscardDeck.transform);
            }

            // Removing all the Ai cards that we need to
            for (int j = PlayerCount - 1; j >= 0 && AiCountWeighted > 0; j--)
            {
                AiCountWeighted--;
                rows[i].transform.GetChild(j).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-350, -60), .5f, true);
                rows[i].transform.GetChild(j).tag = "Untagged";
                yield return new WaitForSeconds(.55f);
                rows[i].transform.GetChild(j).GetComponent<RectTransform>().SetParent(playerDiscardDeck.transform);
            }
            yield return new WaitForSeconds(.2f);

            // Moving
            if ((rows[i].transform.childCount == 0) && (rows[(i + 1)].transform.childCount == 0))
            {
                Debug.Log("Both Should Move !");
                rows[i].tag = "Untagged";
                rows[(i + 1)].tag = "Untagged";
                StartCoroutine("MoveAhead");
            }
            else if (rows[i].transform.childCount == 0)
            {
                Debug.Log("Ai Should Move");
                rows[i].tag = "Untagged";
                StartCoroutine("AiMove");
            }
            else if (rows[(i + 1)].transform.childCount == 0)
            {
                Debug.Log("Player Should Move");
                rows[(i + 1)].tag = "Untagged";
                StartCoroutine("PlayerMove");
            }
            else
            {
                Debug.Log("Nobody moves (should never happen !!)");
            }
        }
        WinLoseCheck();

    }

    private static void WinLoseCheck()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            Debug.Log("Ai Won");
        }
        else if (GameObject.FindGameObjectsWithTag("Ai").Length == 0)
        {
            Debug.Log("Player Won");
        }
        else
        {
            Debug.Log("Game Continues");
            Debug.Log("Player " + GameObject.FindGameObjectsWithTag("Player").Length);
            Debug.Log("Ai " + GameObject.FindGameObjectsWithTag("Ai").Length);
        }
    }

    public IEnumerator MoveAhead()
    {
        //Move Player Cards        
        StartCoroutine("PlayerMove");

        //Move Ai cards
        yield return new WaitForSeconds(.3f);
        StartCoroutine("AiMove");
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

        //in each iteration the card moves one row above in doing so it will rename the tag for that row so it becomes players row
        //it will assign cardtype string a value in doing so card of only that type can be added to the row if needed in future
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
