using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PiecesManager
{
    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8];

        //upleft
        KnightMove(CurrentX - 1, CurrentY + 2, ref r);

        //upright
        KnightMove(CurrentX + 1, CurrentY + 2, ref r);

        //rightup
        KnightMove(CurrentX + 2, CurrentY + 1, ref r);

        //right down
        KnightMove(CurrentX + 2, CurrentY - 1, ref r);

        //downleft
        KnightMove(CurrentX - 1, CurrentY - 2, ref r);

        //downright
        KnightMove(CurrentX + 1, CurrentY - 2, ref r);

        //leftup
        KnightMove(CurrentX - 2, CurrentY + 1, ref r);

        //left down
        KnightMove(CurrentX - 2, CurrentY - 1, ref r);

        return r;
    }

    public override bool[,] PossibleMovesAI(List<PiecesManager> pieces)
    {
        bool[,] r = new bool[8, 8];

        //upleft
        KnightMoveAI(pieces, CurrentX - 1, CurrentY + 2, ref r);

        //upright
        KnightMoveAI(pieces, CurrentX + 1, CurrentY + 2, ref r);

        //rightup
        KnightMoveAI(pieces, CurrentX + 2, CurrentY + 1, ref r);

        //right down
        KnightMoveAI(pieces, CurrentX + 2, CurrentY - 1, ref r);

        //downleft
        KnightMoveAI(pieces, CurrentX - 1, CurrentY - 2, ref r);

        //downright
        KnightMoveAI(pieces, CurrentX + 1, CurrentY - 2, ref r);

        //leftup
        KnightMoveAI(pieces, CurrentX - 2, CurrentY + 1, ref r);

        //left down
        KnightMoveAI(pieces, CurrentX - 2, CurrentY - 1, ref r);

        return r;
    }

    public void KnightMoveAI(List<PiecesManager> pieces, int x, int y, ref bool[,] r)
    {
        PiecesManager c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = AtLocation(pieces, x, y);
            if (c == null)
            {
                r[x, y] = true;
            }
            else if (IsWhite != c.IsWhite)
            {
                r[x, y] = true;
            }
        }
    }

    public void KnightMove(int x, int y, ref bool[,] r)
    {
        PiecesManager c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = BoardManager.Instance.board.GetPiece(x,y);
            if (c == null)
            {
                r[x, y] = true;
            }
            else if (IsWhite != c.IsWhite)
            {
                r[x, y] = true;
            }
        }
    }

    public override int GetValue()
    {
        return ((IsWhite) ? 30 : -30);
    }

    public override float GetValueAdvanced(int x, int y, bool isWhite)
    {
        float[,] val = {
            {-5.0f, -4.0f, -3.0f, -3.0f, -3.0f, -3.0f, -4.0f, -5.0f},
            {-4.0f, -2.0f,  0.0f,  0.0f,  0.0f,  0.0f, -2.0f, -4.0f},
            {-3.0f,  0.0f,  1.0f,  1.5f,  1.5f,  1.0f,  0.0f, -3.0f},
            {-3.0f,  0.5f,  1.5f,  2.0f,  2.0f,  1.5f,  0.5f, -3.0f},
            {-3.0f,  0.0f,  1.5f,  2.0f,  2.0f,  1.5f,  0.0f, -3.0f},
            {-3.0f,  0.5f,  1.0f,  1.5f,  1.5f,  1.0f,  0.5f, -3.0f},
            {-4.0f, -2.0f,  0.0f,  0.5f,  0.5f,  0.0f, -2.0f, -4.0f},
            {-5.0f, -4.0f, -3.0f, -3.0f, -3.0f, -3.0f, -4.0f, -5.0f}
        };

        if (isWhite)
            return val[x, y];
        else
            return val[y, x];
    }
}
