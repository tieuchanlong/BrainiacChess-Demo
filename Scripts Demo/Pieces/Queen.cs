using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : PiecesManager
{
    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8];
        PiecesManager c;
        int i, j;

        //right
        i = CurrentX;
        while (true)
        {
            i++;
            if (i >= 8)
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

		//

		//top left
		i = CurrentX;
		j = CurrentY;
		while (true)
		{
			i--;
			j++;
			if (i < 0 || j >= 8)
				break;

			c = BoardManager.Instance.board.GetPiece(i, j);
			if (c == null)
				r[i, j] = true;
			else
			{
				if (IsWhite != c.IsWhite)
					r[i, j] = true;

				break;
			}
		}


		//top right
		i = CurrentX;
		j = CurrentY;
		while (true)
		{
			i++;
			j++;
			if (i >= 8 || j >= 8)
				break;

			c = BoardManager.Instance.board.GetPiece(i, j);
			if (c == null)
				r[i, j] = true;
			else
			{
				if (IsWhite != c.IsWhite)
					r[i, j] = true;

				break;
			}
		}

		//down left
		i = CurrentX;
		j = CurrentY;
		while (true)
		{
			i--;
			j--;
			if (i < 0 || j < 0)
				break;

			c = BoardManager.Instance.board.GetPiece(i, j);
			if (c == null)
				r[i, j] = true;
			else
			{
				if (IsWhite != c.IsWhite)
					r[i, j] = true;

				break;
			}
		}


		//down right
		i = CurrentX;
		j = CurrentY;
		while (true)
		{
			i++;
			j--;
			if (i >= 8 || j < 0)
				break;

			c = BoardManager.Instance.board.GetPiece(i, j);
			if (c == null)
				r[i, j] = true;
			else
			{
				if (IsWhite != c.IsWhite)
					r[i, j] = true;

				break;
			}
		}

		return r;
    }

    public override bool[,] PossibleMovesAI(List<PiecesManager> pieces)
    {
        bool[,] r = new bool[8, 8];
        PiecesManager c;
        int i, j;

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

        //

        //top left
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i--;
            j++;
            if (i < 0 || j >= 8)
                break;

            c = AtLocation(pieces, i, j);
            if (c == null)
                r[i, j] = true;
            else
            {
                if (IsWhite != c.IsWhite)
                    r[i, j] = true;

                break;
            }
        }


        //top right
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i++;
            j++;
            if (i >= 8 || j >= 8)
                break;

            c = AtLocation(pieces, i, j);
            if (c == null)
                r[i, j] = true;
            else
            {
                if (IsWhite != c.IsWhite)
                    r[i, j] = true;

                break;
            }
        }

        //down left
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i--;
            j--;
            if (i < 0 || j < 0)
                break;

            c = AtLocation(pieces, i, j);
            if (c == null)
                r[i, j] = true;
            else
            {
                if (IsWhite != c.IsWhite)
                    r[i, j] = true;

                break;
            }
        }


        //down right
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i++;
            j--;
            if (i >= 8 || j < 0)
                break;

            c = AtLocation(pieces, i, j);
            if (c == null)
                r[i, j] = true;
            else
            {
                if (IsWhite != c.IsWhite)
                    r[i, j] = true;

                break;
            }
        }

        return r;
    }

    public override int GetValue()
    {
        return ((IsWhite) ? 900 : -900);
    }

    public override float GetValueAdvanced(int x, int y, bool isWhite)
    {
        float[,] val = {
            { -2.0f, -1.0f, -1.0f, -0.5f, -0.5f, -1.0f, -1.0f, -2.0f},
            { -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -1.0f},
            { -1.0f,  0.0f,  0.5f,  0.5f,  0.5f,  0.5f,  0.0f, -1.0f},
            { -0.5f,  0.0f,  0.5f,  0.5f,  0.5f,  0.5f,  0.0f, -0.5f},
            {  0.0f,  0.0f,  0.5f,  0.5f,  0.5f,  0.5f,  0.0f, -0.5f},
            { -1.0f,  0.5f,  0.5f,  0.5f,  0.5f,  0.5f,  0.0f, -1.0f},
            { -1.0f,  0.0f,  0.5f,  0.0f,  0.0f,  0.0f,  0.0f, -1.0f},
            { -2.0f, -1.0f, -1.0f, -0.5f, -0.5f, -1.0f, -1.0f, -2.0f}
        };

        if (isWhite)
            return val[x, y];
        else
            return val[y, x];
    }
}
