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
    public Transform Aimove;
    // Start is called before the first frame update
    void Start()
    {
        for (int a = 1; a < rows.Length; a++) {
            rows[a].GetComponent<CardSlots>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextTurn() {
        StartCoroutine("PlayAi");
    }
    public IEnumerator PlayAi()
    {
        //Move One Card
        Aimove = Ai.transform.GetChild(Random.Range(0, Ai.transform.childCount));
        Aimove.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0,-64), .3f, true);
        yield return new WaitForSeconds(.3f);
        Aimove.GetComponent<RectTransform>().SetParent(rows[5].transform);
        rows[5].GetComponent<CardSlots>().cardType = Aimove.GetComponent<CardValue>().card.Cardname;
        rows[5].tag = "Ai Cards";


        //Move to next row
        StartCoroutine("MoveAhead");
    }

    public IEnumerator MoveAhead()
    {
        //Move Player Cards
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
                            if (AiCount >= PlayerCount) {
                                for (int b = 0; b < rows[i].transform.childCount; b++) {
                                    Destroy(rows[i].transform.GetChild(b).gameObject);
                                }
                            }
                            for (int b = 0; b < AiCount; b++) {
                                Destroy(rows[i].transform.GetChild(b).gameObject);
                            }
                        }
                    }
                    foreach (string str in ai.weakness) {
                        if (str.Equals(player.Cardname)) {
                            Debug.Log("Ai Dies");
                            if (PlayerCount >= AiCount){
                                for (int b = 0; b < rows[(i+1)].transform.childCount; b++){
                                    Destroy(rows[(i + 1)].transform.GetChild(b).gameObject);
                                }
                            }
                            for (int b = 0; b < AiCount; b++) {
                                Destroy(rows[(i + 1)].transform.GetChild(b).gameObject);
                            }
                        }
                    }


                }
            }
        }
        yield return new WaitForSeconds(.3f);
    }
}
