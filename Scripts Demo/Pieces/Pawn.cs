using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : PiecesManager
{


    public override bool[,] PossibleMoves()
    {

        bool[,] r = new bool[8, 8];
        PiecesManager c, c2;
			
        //white team move
        if (IsWhite)
        {
            //diagonal left
            if (CurrentX != 0 && CurrentY != 7)
            {
                c = BoardManager.Instance.board.GetPiece(CurrentX - 1, CurrentY + 1);
                if (c != null && !c.IsWhite)
                {
                    r[CurrentX - 1, CurrentY + 1] = true;
                }
            }
            //diagonal right
            if (CurrentX != 7 && CurrentY != 7)
            {
                c = BoardManager.Instance.board.GetPiece(CurrentX + 1, CurrentY + 1);
                if (c != null && !c.IsWhite)
                {
                    r[CurrentX + 1, CurrentY + 1] = true;
                }
            }


            //middle
            if (CurrentY != 7)
            {
                c = BoardManager.Instance.board.GetPiece(CurrentX, CurrentY + 1);
                if (c == null) { r[CurrentX, CurrentY + 1] = true; }
            }
            //middle on first move
            if (CurrentY == 1)
            {
				c = BoardManager.Instance.board.GetPiece(CurrentX, CurrentY + 1);
                c2 = BoardManager.Instance.board.GetPiece(CurrentX, CurrentY + 2);
                if (c == null && c2 == null)
                {
                    r[CurrentX, CurrentY + 2] = true;
                }
            }

        }//black team move
        else
        {
            //diagonal left
            if (CurrentX != 0 && CurrentY != 0)
            {
                c = BoardManager.Instance.board.GetPiece(CurrentX - 1, CurrentY - 1);
                if (c != null && c.IsWhite)
                {
                    r[CurrentX - 1, CurrentY - 1] = true;
                }
            }
            //diagonal right
            if (CurrentX != 7 && CurrentY != 0)
            {
                c = BoardManager.Instance.board.GetPiece(CurrentX + 1, CurrentY - 1);
                if (c != null && c.IsWhite)
                {
                    r[CurrentX + 1, CurrentY - 1] = true;
                }
            }


            //middle
            if (CurrentY != 0)
            {
                c = BoardManager.Instance.board.GetPiece(CurrentX, CurrentY - 1);
                if (c == null) { r[CurrentX, CurrentY - 1] = true; }
            }
            //middle on first move
            if (CurrentY == 6)
            {
                c = BoardManager.Instance.board.GetPiece(CurrentX, CurrentY - 1);
                c2 = BoardManager.Instance.board.GetPiece(CurrentX, CurrentY - 2);
                if (c == null && c2 == null)
                {
                    r[CurrentX, CurrentY - 2] = true;
                }
            }
        }



        return r;

    }

    public override bool[,] PossibleMovesAI(List<PiecesManager> pieces)
    {

        bool[,] r = new bool[8, 8];
        PiecesManager c, c2;

        //white team move
        if (IsWhite)
        {
            //diagonal left
            if (CurrentX != 0 && CurrentY != 7)
            {
                c = AtLocation(pieces, CurrentX - 1, CurrentY + 1);
                if (c != null && !c.IsWhite)
                {
                    r[CurrentX - 1, CurrentY + 1] = true;
                }
            }
            //diagonal right
            if (CurrentX != 7 && CurrentY != 7)
            {
                c = AtLocation(pieces, CurrentX + 1, CurrentY + 1);
                if (c != null && !c.IsWhite)
                {
                    r[CurrentX + 1, CurrentY + 1] = true;
                }
            }


            //middle
            if (CurrentY != 7)
            {
                c = AtLocation(pieces, CurrentX, CurrentY + 1);
                if (c == null) { r[CurrentX, CurrentY + 1] = true; }
            }
            //middle on first move
            if (CurrentY == 1)
            {
                c = AtLocation(pieces, CurrentX, CurrentY + 1);
                c2 = AtLocation(pieces, CurrentX, CurrentY + 2);
                if (c == null && c2 == null)
                {
                    r[CurrentX, CurrentY + 2] = true;
                }
            }

        }//black team move
        else
        {
            //diagonal left
            if (CurrentX != 0 && CurrentY != 0)
            {
                c = AtLocation(pieces, CurrentX - 1, CurrentY - 1);
                if (c != null && c.IsWhite)
                {
                    r[CurrentX - 1, CurrentY - 1] = true;
                }
            }
            //diagonal right
            if (CurrentX != 7 && CurrentY != 0)
            {
                c = AtLocation(pieces, CurrentX + 1, CurrentY - 1);
                if (c != null && c.IsWhite)
                {
                    r[CurrentX + 1, CurrentY - 1] = true;
                }
            }


            //middle
            if (CurrentY != 0)
            {
                c = AtLocation(pieces, CurrentX, CurrentY - 1);
                if (c == null) { r[CurrentX, CurrentY - 1] = true; }
            }
            //middle on first move
            if (CurrentY == 6)
            {
                c = AtLocation(pieces, CurrentX, CurrentY - 1);
                c2 = AtLocation(pieces, CurrentX, CurrentY - 2);
                if (c == null && c2 == null)
                {
                    r[CurrentX, CurrentY - 2] = true;
                }
            }
        }



        return r;

    }

    public int PromotionRank()
    {
        if (IsWhite)
        {
            return 7;
        }
        else
        {
            return 0;
        }
    }

    public override int GetValue()
    {
        return ((IsWhite) ? 10 : -10);
    }

    public override float GetValueAdvanced(int x, int y, bool isWhite)
    {
        float[,] val = {
            { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
            { 5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f },
            { 1.0f,  1.0f,  2.0f,  3.0f,  3.0f,  2.0f,  1.0f,  1.0f },
            { 0.5f,  0.5f,  1.0f,  2.5f,  2.5f,  1.0f,  0.5f,  0.5f },
            { 0.0f,  0.0f,  0.0f,  2.0f,  2.0f,  0.0f,  0.0f,  0.0f },
            { 0.5f, -0.5f, -1.0f,  0.0f,  0.0f, -1.0f, -0.5f,  0.5f },
            { 0.5f,  1.0f, 1.0f,  -2.0f, -2.0f,  1.0f,  1.0f,  0.5f },
            { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f }
        };

        if (isWhite)
            return val[x, y];
        else
            return val[y, x];
    }

}
