using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlight : MonoBehaviour
{
    public static BoardHighlight Instance { get; set; }
    public GameObject highlightprifab;
    public GameObject highlightprefab2D;
    public int Mode;
    private List<GameObject> highlights;
    public List<GameObject> highlights2D;

    public void Start()
    {
        Instance = this;
        highlights = new List<GameObject>();
        Mode = Definitions.Mode;
    }

    private GameObject GetHighlightObject(int Mode)
    {
        GameObject go;
        if (Mode == 0)
        {
            go = highlights.Find(g => !g.activeSelf);

            if (go == null)
            {
                go = Instantiate(highlightprifab);
                highlights.Add(go);
            }
        }
        else
        {
            go = highlights2D.Find(g => !g.activeSelf);

            if (go == null)
            {
                go = Instantiate(highlightprefab2D);
                highlights2D.Add(go);
            }
        }

        return go;
    }

    public void HighlightAllowedMoves(bool[,] moves)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (moves[i, j])
                {
                    if (Mode == 0)
                    {
                        GameObject go = GetHighlightObject(0);
                        go.SetActive(true);
                        go.transform.position = new Vector3(i + 0.5f, 0.01f, j + 0.5f);
                    }
                    else
                    {
                        GameObject go1 = GetHighlightObject(1);
                        go1.SetActive(true);
                        go1.transform.position = new Vector2(i * 130f - 460f + 7 - i, j * 130f - 465f - (j - 3));
                    }
                }
            }
        }
    }

    public void HideHighlights()
    {
        if (Mode == 0)
        {
            foreach (GameObject go in highlights)
                go.SetActive(false);
        }
        else
        {
            foreach (GameObject go in highlights2D)
                go.SetActive(false);
        }
    }
}