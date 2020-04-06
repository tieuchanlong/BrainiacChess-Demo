using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessTimer : MonoBehaviour
{
    public int minTotal = 3;
    public float secTotal = 0;
    private int resetMin;
    private float resetSec;
    // Start is called before the first frame update
    void Start()
    {
        resetMin = minTotal;
        resetSec = secTotal;
    }
    
    public void UpdateTimer()
    {
        secTotal -= Time.deltaTime;
        if (secTotal <= 0)
        {
            secTotal = 60.0f + secTotal;
            minTotal -= 1;
            if(minTotal < 0)
            {
                minTotal = 0;
                secTotal = 0;
            }
        }
        GetComponent<Text>().text = minTotal.ToString() + ":" + (((int)Mathf.Floor(secTotal) < 10) ? "0" : "") + ((int)Mathf.Floor(secTotal)).ToString();
    }

    public void ResetTimer()
    {
        minTotal = resetMin;
        secTotal = resetSec;
        GetComponent<Text>().text = minTotal.ToString() + ":" + (((int)Mathf.Floor(secTotal) < 10) ? "0" : "") + ((int)Mathf.Floor(secTotal)).ToString();
    }

    /// <summary>
    /// This method used to add extra seconds for each move made
    /// </summary>
    /// <param name="sec"></param>
    private void AddExtraTime(float sec)
    {
        secTotal += sec;
        minTotal += ((int)secTotal / 60);
        secTotal = ((int)secTotal % 60);
        GetComponent<Text>().text = minTotal.ToString() + ":" + (((int)Mathf.Floor(secTotal) < 10) ? "0" : "") + ((int)Mathf.Floor(secTotal)).ToString();
    }
}
