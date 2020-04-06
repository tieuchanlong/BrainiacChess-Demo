using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board2D : BoardControl
{
    private void Start()
    {
        if (PiecesManagers == null)
        {
            ResetLists();
        }
        Mode = Definitions.Mode;

        if (Mode == 0)
            this.gameObject.SetActive(false);
    }
}
