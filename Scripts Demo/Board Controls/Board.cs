using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class Board : BoardControl
{
    private void Start()
    {
        if(PiecesManagers == null)
        {
            ResetLists();
        }
        Mode = Definitions.Mode;

        if (Mode == 1)
            this.gameObject.SetActive(false);
    }
}