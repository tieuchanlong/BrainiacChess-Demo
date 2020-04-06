using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessAI: MonoBehaviour
{
    private ChessAIResult temp_result = new ChessAIResult();
    // This class holds the brain of the Chess AI system
    public ChessAI()
    {

    }

    public IEnumerator GetMiniMaxMove(int depth, List<PiecesManager> pieces, float alpha, float beta, bool isMaximisingplayer, ChessAIResult result, Action<ChessAIResult> actionResult)
    {
        int temp_best, prevX, prevY, pos;

        if (depth == 0)
        {
            result.bestValue = GetBoardEvaluate(pieces);
            actionResult(result);
            yield break; // Replace with value for each piece
        }

        if (isMaximisingplayer)
        {
            float bestMove = -9999;
            for (int i = 0; i < pieces.Count; i++)
                if (pieces[i].IsWhite == true)
                {
                    bool[,] moves = pieces[i].PossibleMovesAI(pieces);

                    for (int k = 0; k < 8; k++)
                        for (int t = 0; t < 8; t++)
                            if (moves[k, t])
                            {
                                PiecesManager c = AtLocation(pieces, k, t);
                                pos = GetPiecePosition(pieces, k, t);

                                prevX = pieces[i].CurrentX;
                                prevY = pieces[i].CurrentY;
                                pieces[i].CurrentX = k;
                                pieces[i].CurrentY = t;
                                if (c != null)
                                    pieces.Remove(c);
                                yield return StartCoroutine(GetMiniMaxMove(depth - 1, pieces, alpha, beta, !isMaximisingplayer, temp_result, (x)=>temp_result=x));
                                
                                if (bestMove < temp_result.bestValue)
                                {
                                    bestMove = temp_result.bestValue;
                                    result.bestValue = temp_result.bestValue;
                                    result.selected_ind = i;
                                    result.selected_move_x = k;
                                    result.selected_move_y = t;
                                    actionResult(result);
                                }

                                if (c != null)
                                    pieces.Insert(pos, c);
                                pieces[i].CurrentX = prevX;
                                pieces[i].CurrentY = prevY;
                                alpha = Mathf.Max(alpha, bestMove);

                                if (beta <= alpha)
                                {
                                    yield break;
                                    //return bestMove;
                                }
                            }
                    yield return null;
                }
            yield break;
            //return bestMove;
        }
        else
        {
            float bestMove = 9999;
            for (int i = 0; i < pieces.Count; i++)
                if (pieces[i].IsWhite == false)
                {
                    bool[,] moves = pieces[i].PossibleMovesAI(pieces);

                    for (int k = 0; k < 8; k++)
                        for (int t = 0; t < 8; t++)
                            if (moves[k, t])
                            {
                                PiecesManager c = AtLocation(pieces, k, t);
                                pos = GetPiecePosition(pieces, k, t);

                                prevX = pieces[i].CurrentX;
                                prevY = pieces[i].CurrentY;
                                pieces[i].CurrentX = k;
                                pieces[i].CurrentY = t;
                                if (c != null)
                                    pieces.Remove(c);

                                yield return StartCoroutine(GetMiniMaxMove(depth - 1, pieces, alpha, beta, !isMaximisingplayer, temp_result, (x) => temp_result = x));

                                if (bestMove > temp_result.bestValue)
                                {
                                    bestMove = temp_result.bestValue;
                                    result.bestValue = temp_result.bestValue;
                                    result.selected_ind = i;
                                    result.selected_move_x = k;
                                    result.selected_move_y = t;
                                    actionResult(result);
                                }

                                if (c != null)
                                    pieces.Insert(pos, c);
                                pieces[i].CurrentX = prevX;
                                pieces[i].CurrentY = prevY;
                                beta = Mathf.Min(beta, bestMove);

                                if (beta <= alpha)
                                {
                                    yield break;
                                    //return bestMove;
                                }
                            }
                    yield return null;
                }
            yield break;
            //return bestMove;
        }
    }

    private int GetPiecePosition(List<PiecesManager> pieces, int k, int t)
    {
        for (int j = 0; j < pieces.Count; j++)
            if (pieces[j].CurrentX == k && pieces[j].CurrentY == t)
                return j;

        return -1;
    }

    public PiecesManager AtLocation(List<PiecesManager> pieces, int x, int y)
    {
        return pieces.Where(p => p.CurrentX == x && p.CurrentY == y).FirstOrDefault();
    }

    public int Location(List<PiecesManager> pieces, int x, int y)
    {
        return int.Parse(pieces.Where(p => p.CurrentX == x && p.CurrentY == y).FirstOrDefault().index);
    }

    private float GetBoardEvaluate(List<PiecesManager> pieces)
    {
        float sum = 0;
        for (int i = 0; i < pieces.Count; i++)
            if (Definitions.search_depth[Definitions.selectedDifficulty] <= 2) // Choose easy then dont use optimized table
                sum += pieces[i].GetValue();
            else
                sum += pieces[i].GetValueAdvanced(pieces[i].CurrentX, pieces[i].CurrentY, pieces[i].IsWhite);
        return sum;
    }

}
