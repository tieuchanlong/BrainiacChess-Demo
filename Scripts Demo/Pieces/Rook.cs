using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : PiecesManager
{
    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8];
        PiecesManager c;
        int i;

        //right
        i = CurrentX;
        while (true)
        {
            i++;
            if (i >= 8)
                break;

            c = BoardManager.Instance.board.GetPiece(i,CurrentY);
            if (c == null)
                r[i, CurrentY] = true;
            else
            {
                if (c.IsWhite != IsWhite)
                    r[i, CurrentY] = true;

                break;
            }
        }



        //left
        i = CurrentX;
        while (true)
        {
            i--;
            if (i < 0)
                break;

            c = BoardManager.Instance.board.GetPiece(i, CurrentY);
			if (c == null)
                r[i, CurrentY] = true;
            else
            {
                if (c.IsWhite != IsWhite)
                    r[i, CurrentY] = true;

                break;
            }
        }

        //up
        i = CurrentY;
        while (true)
        {
            i++;
            if (i >= 8)
                break;

            c = BoardManager.Instance.board.GetPiece(CurrentX, i);
			if (c == null)
                r[CurrentX, i] = true;
            else
            {
                if (c.IsWhite != IsWhite)
                    r[CurrentX, i] = true;

                break;
            }
        }

        //down
        i = CurrentY;
        while (true)
        {
            i--;
            if (i < 0)
                break;

            c = BoardManager.Instance.board.GetPiece(CurrentX, i);
			if (c == null)
                r[CurrentX, i] = true;
            else
            {
                if (c.IsWhite != IsWhite)
                    r[CurrentX, i] = true;

                break;
            }
        }
        return r;
    }

    public override bool[,] PossibleMovesAI(List<PiecesManager> pieces)
    {
        bool[,] r = new bool[8, 8];
        PiecesManager c;
        int i;

        //right
        i = CurrentX;
        while (true)
        {
            i++;
            if (i >= 8)
                break;

            c = AtLocation(pieces, i, CurrentY);
            if (c == null)
                r[i, CurrentY] = true;
            else
            {
                if (c.IsWhite != IsWhite)
                    r[i, CurrentY] = true;

                break;
            }
        }



        //left
        i = CurrentX;
        while (true)
        {
            i--;
            if (i < 0)
                break;

            c = AtLocation(pieces, i, CurrentY);
            if (c == null)
                r[i, CurrentY] = true;
            else
            {
                if (c.IsWhite != IsWhite)
                    r[i, CurrentY] = true;

                break;
            }
        }

        //up
        i = CurrentY;
        while (true)
        {
            i++;
            if (i >= 8)
                break;

            c = AtLocation(pieces, CurrentX, i);
            if (c == null)
                r[CurrentX, i] = true;
            else
            {
                if (c.IsWhite != IsWhite)
                    r[CurrentX, i] = true;

                break;
            }
        }

        //down
        i = CurrentY;
        while (true)
        {
            i--;
            if (i < 0)
                break;

            c = AtLocation(pieces, CurrentX, i);
            if (c == null)
                r[CurrentX, i] = true;
            else
            {
                if (c.IsWhite != IsWhite)
                    r[CurrentX, i] = true;

                break;
            }
        }
        return r;
    }

    public override int GetValue()
    {
        return ((IsWhite) ? 50 : -50);
    }

    public override float GetValueAdvanced(int x, int y, bool isWhite)
    {
        float[,] val = {
            {  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f},
            {  0.5f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  0.5f},
            { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f},
            { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f},
            { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f},
            { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f},
            { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f},
            {  0.0f,   0.0f, 0.0f,  0.5f,  0.5f,  0.0f,  0.0f,  0.0f}
        };

        if (isWhite)
            return val[x, y];
        else
            return val[y, x];
    }
}
