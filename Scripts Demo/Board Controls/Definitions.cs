using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Definitions : MonoBehaviour
{
    public static int Mode = 0;
    public static bool AIWhite = true;
    public static string selectedDifficulty = "Medium";
    public static Dictionary<string, int> search_depth;
    [SerializeField] private Camera cam;
    [SerializeField] private Camera cam2D;
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject board2D;
    

    // Start is called before the first frame update
    void Start()
    {
        search_depth = new Dictionary<string, int>();
        search_depth.Add("Easy", 2);
        search_depth.Add("Medium", 5);
        search_depth.Add("Hard", 6);
    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null) return;
        if (Mode == 0)
        {
            cam.gameObject.SetActive(true);
            board.SetActive(true);
        }
        else
        {
            cam2D.gameObject.SetActive(true);
            board2D.SetActive(true);
        }
    }
}
