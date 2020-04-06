using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : PiecesManager
{
    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8];
        PiecesManager c;
        int i, j;

        //top side
        j = CurrentY + 1;
        if (CurrentY != 7)
        {
            for (int k = 0; k < 3; k++)
            {
                i = CurrentX + k - 1;

                if (i >= 0 && i < 8)
                {
                    c = BoardManager.Instance.board.GetPiece(i, j);
                    if (c == null || (IsWhite != c.IsWhite))
                        r[i, j] = true;
                }
                i++;
            }
        }


        //down side
        j = CurrentY - 1;
        if (CurrentY != 0)
        {
            for (int k = 0; k < 3; k++)
            {
                i = CurrentX + k - 1;
                if (i >= 0 && i < 8)
                {
                    c = BoardManager.Instance.board.GetPiece(i, j);
                    if (c == null || (IsWhite != c.IsWhite))
                        r[i, j] = true;
                }
                i++;
            }
        }

        //middle left
        if (CurrentX != 0)
        {
            c = BoardManager.Instance.board.GetPiece(CurrentX - 1, CurrentY);
            if (c == null || (IsWhite != c.IsWhite))
                r[CurrentX - 1, CurrentY] = true;
        }

        //middle right
        if (CurrentX != 7)
        {
            c = BoardManager.Instance.board.GetPiece(CurrentX + 1, CurrentY);
            if (c == null || (IsWhite != c.IsWhite))
                r[CurrentX + 1, CurrentY] = true;
        }

        //castling

        if (!Moved)
        {
            //left
            bool Empty = true;
            c = BoardManager.Instance.board.GetPiece(0, CurrentY);
            if (c != null && !c.Moved && c.GetType() == typeof(Rook))
            {
                //empty spaces
                for (int k = 1; k <= CurrentX - 1; k++)
                {
                    c = BoardManager.Instance.board.GetPiece(k, CurrentY);
                    if (c != null)
                    {
                        Empty = false;
                        break;
                    }
                }
                if (Empty) r[0, CurrentY] = true;
            }

            //right
            Empty = true;
            c = BoardManager.Instance.board.GetPiece(7, CurrentY);
            if (c != null && !c.Moved && c.GetType() == typeof(Rook))
            {
                //empty spaces
                for (int k = CurrentX + 1; k <= 6; k++)
                {
                    c = BoardManager.Instance.board.GetPiece(k, CurrentY);
                    if (c != null)
                    {
                        Empty = false;
                        break;
                    }
                }
                if (Empty) r[7, CurrentY] = true;
            }
        }

        return r;
    }

    /// <summary>
    /// Possible moves for Chess AI
    /// </summary>
    /// <param name="pieces"></param>
    /// <returns></returns>
    public override bool[,] PossibleMovesAI(List<PiecesManager> pieces)
    {
        bool[,] r = new bool[8, 8];
        PiecesManager c;
        int i, j;

        //top side
        j = CurrentY + 1;
        if (CurrentY != 7)
        {
            for (int k = 0; k < 3; k++)
            {
                i = CurrentX + k - 1;

                if (i >= 0 && i < 8)
                {
                    c = AtLocation(pieces, i, j);
                    if (c == null || (IsWhite != c.IsWhite))
                        r[i, j] = true;
                }
                i++;
            }
        }


        //down side
        j = CurrentY - 1;
        if (CurrentY != 0)
        {
            for (int k = 0; k < 3; k++)
            {
                i = CurrentX + k - 1;
                if (i >= 0 && i < 8)
                {
                    c = AtLocation(pieces, i, j);
                    if (c == null || (IsWhite != c.IsWhite))
                        r[i, j] = true;
                }
                i++;
            }
        }

        //middle left
        if (CurrentX != 0)
        {
            c = AtLocation(pieces, CurrentX - 1, CurrentY);
            if (c == null || (IsWhite != c.IsWhite))
                r[CurrentX - 1, CurrentY] = true;
        }

        //middle right
        if (CurrentX != 7)
        {
            c = AtLocation(pieces, CurrentX + 1, CurrentY);
            if (c == null || (IsWhite != c.IsWhite))
                r[CurrentX + 1, CurrentY] = true;
        }

        //castling

        if (!Moved)
        {
            //left
            bool Empty = true;
            c = AtLocation(pieces, 0, CurrentY);
            if (c != null && !c.Moved && c.GetType() == typeof(Rook))
            {
                //empty spaces
                for (int k = 1; k <= CurrentX - 1; k++)
                {
                    c = AtLocation(pieces, k, CurrentY);
                    if (c != null)
                    {
                        Empty = false;
                        break;
                    }
                }
                if (Empty) r[0, CurrentY] = true;
            }

            //right
            Empty = true;
            c = AtLocation(pieces, 7, CurrentY);
            if (c != null && !c.Moved && c.GetType() == typeof(Rook))
            {
                //empty spaces
                for (int k = CurrentX + 1; k <= 6; k++)
                {
                    c = AtLocation(pieces, k, CurrentY);
                    if (c != null)
                    {
                        Empty = false;
                        break;
                    }
                }
                if (Empty) r[7, CurrentY] = true;
            }
        }

        return r;
    }

    public override int GetValue()
    {
        return ((IsWhite) ? 90 : -90);
    }

    public override float GetValueAdvanced(int x, int y, bool isWhite)
    {
        float[,] val = {
        {-3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f},
        {-3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f},
        {-3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f},
        {-3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f},
        {-2.0f, -3.0f, -3.0f, -4.0f, -4.0f, -3.0f, -3.0f, -2.0f},
        {-1.0f, -2.0f, -2.0f, -2.0f, -2.0f, -2.0f, -2.0f, -1.0f},
        {2.0f, 2.0f, 0.0f, 0.0f, 0.0f, 0.0f, 2.0f, 2.0f},
        {2.0f, 3.0f, 1.0f, 0.0f, 0.0f, 1.0f, 3.0f, 2.0f}
        };

        if (isWhite)
            return val[x, y];
        else
            return val[y, x];
    }
}
