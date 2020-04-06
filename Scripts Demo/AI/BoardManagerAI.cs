using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class BoardManagerAI : MonoBehaviour
{
    public bool debug = false;
    [Header("Time Settings")]

    public float StartTime = 25f;          // This is the initial starting timer
    [SerializeField] private float extraSec;
    [SerializeField] private Text startTimeUI;                         // Base Seconds Per Turn

    [SerializeField] [Greyout] private float RematchTime = 30f;

    public static BoardManagerAI Instance { set; get; }
    private bool[,] allowedMoves { set; get; }              // Move Variable


    [Header("The Piece Manager")]
    [SerializeField] private ChessTimer whiteTimer = null;
    [SerializeField] private ChessTimer blackTimer = null;
    private bool timed = false;
    private float Timing = 0;

    public PiecesManager selectedChessManager;

    [SerializeField] private GameObject endGamePanel;

    [Header("Board Setup")]
    //[WarnIfEmpty]
    private Material previousMat;
    //[WarnIfEmpty]
    public Material selectedMat;
    [SerializeField] private GameObject highlight2D;
    [SerializeField] private GameObject kingCheck2D;
    [SerializeField] private GameObject kingCheck3D;
    private GameObject highlightinstance = null;
    private GameObject kingcheckinstance = null;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    public int selectionX = -1;
    public int selectionY = -1;

    [Header("Game Objects")]
    public List<GameObject> activePrefabs;

    public bool selected = false;

    [Header("Current Turn")]
    public bool isWhiteturn = true;

    // Below belows are to make sure that both players have made their first turn befeore the clock starts
    [Header("Color Has Started")]
    [SerializeField] private bool m_WhiteStart;
    [SerializeField] private bool m_BlackStart;

    [Header("Network Client")]
    private NetworkClient _NC;

    [Header("Promotion")]
    [SerializeField] private GameObject promotionPanel;
    private bool promoting = false;
    //The position where the pawn will be after promotion
    private int promotionX = -1;
    private int promotionY = -1;
    //The pawn to remove after promotion
    private PiecesManager promotingPiece = null;
    private PiecesManager takenPiece = null;
    private bool gameOver = false;


    [Header("End Game Panel")]
    [SerializeField] private Text resultTitle;
    [SerializeField] private GameObject RematchPanel;

    [SerializeField] [Greyout] private bool RematchRequested;

    [SerializeField] private GameObject RequestedText;
    [SerializeField] private GameObject RematchTimer;
    [Greyout] public bool RematchAccepted;
    [HideInInspector]
    public BoardControl board;
    public int Mode = Definitions.Mode;


    [Header("User")]
    public string WhiteUser;
    public string BlackUser;
    public List<string> Spectators = new List<string>();

    [Header("Sound")]
    private bool inStale = false;
    private bool inCheck = false;
    private bool inCheckMate = false;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip StaleSound;
    [SerializeField] private AudioClip CheckmateSound;
    [SerializeField] private AudioClip InCheckSound;
    [SerializeField] private AudioClip StaleSound2D;
    [SerializeField] private AudioClip CheckmateSound2D;
    [SerializeField] private AudioClip InCheckSound2D;
    [SerializeField] private AudioClip TimeUpSound;

    #region AI Variables
    public bool isAIWhite;
    private ChessAI chessBrain = new ChessAI();
    ChessAIResult result = new ChessAIResult();
    private ChessAIResult temp_result = new ChessAIResult();
    private bool AIMoving = false;
    #endregion

    private void Awake()
    {
        Instance = this;
        chessBrain = new ChessAI();
        if (!debug)
        {
            _NC = GameObject.FindGameObjectWithTag("NetworkClient").GetComponent<NetworkClient>();
        }
    }

    private void Start()
    {
        if (!debug)
        {
            Mode = Definitions.Mode;
            if (Mode == 0)
                board = transform.Find("Board").GetComponent<BoardControl>();
            else
                board = transform.Find("Board2D").GetComponent<BoardControl>();
        }

        if (whiteTimer == null || blackTimer == null)
        {
            timed = false;
        }

        if (timed)
        {
            WhiteTurnTimer();
        }
        if (!debug)
        {
            SpawnAllPieces();
        }
    }

    private void Update()
    {
        if (board == null)
        {
            return;
        }
        Debug.Log("Update: " + board.PiecesManagers.Count);
        DrawBoard();
        //UpdateSelection();

        if (!gameOver && !AIMoving)
        {
            //checkmate
            if (InCheckmate(isWhiteturn))
            {
                Debug.Log("CHECKMATE");
                if (isWhiteturn)
                {
                    EndGame("White Wins!");
                }
                else
                {
                    EndGame("Black Wins!");
                }
            }
            //Stale
            if (InStale(isWhiteturn))
            {
                EndGame("Draw!");
            }
            //check
            if (InCheck(isWhiteturn))
            {
                Debug.Log("CHECK");
                // TODO: Implement End Game
            }
        }

        //if (isWhiteturn && WhiteUser == NetworkClient.ClientID || !isWhiteturn && BlackUser == NetworkClient.ClientID)			// Check if the player is playing
        //{
        if (Input.GetMouseButtonDown(0) && isWhiteturn != isAIWhite)
        {
            UpdateSelection();
            if (promoting)
            {
                PromotePawn();
            }
            else if (selectionX >= 0 && selectionY >= 0 && !selected)
            {
                //select the piecemanager
                SelectPiece(selectionX, selectionY);
                //Debug.Log("x"+selectionY+"y"+selectionX);

            }
            else if (selected == true && selectionX >= 0 && selectionY >= 0)
            {
                MoveToNewPositions();
            }
            else if (selectionX < 0 && selectionY < 0)
            {
                Debug.Log("out of bounds");
                selected = false;
                BoardHighlight.Instance.HideHighlights();
            }
            else
            {
                Debug.Log("bounds");
                selected = false;
                BoardHighlight.Instance.HideHighlights();
            }
        }
        else if (isWhiteturn == isAIWhite)
        {
            // AI Turn to Move
            if (!AIMoving)
                StartCoroutine(AIMove(board.PiecesManagers));
        }

        WaitForRematch();
    }

    #region Game Process Controls
    private void MoveToNewPositions()
    {
        //move piecemanager
        if (selectionX > 7 || selectionY > 7)
            return;

        if (selectedChessManager != board.PiecesManagers.AtLocation(selectionX, selectionY))
        {
            selected = false;
            BoardHighlight.Instance.HideHighlights();
            RevertSelectedPiece();
        }

        if (!MoveCausesCheck(selectedChessManager, selectionX, selectionY))
            MovePiece(selectionX, selectionY);
    }

    private void PromotePawn()
    {
        bool promoted = false;
        for (int i = 1; i < 5; i++)
        {
            RectTransform rect = promotionPanel.transform.GetChild(i).GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
            {
                promoted = true;
            }
        }

        if (!promoted)
        {
            promoting = false;
            promotionPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Wait for the rematch if anyone clicks
    /// </summary>
    private void WaitForRematch()
    {
        if (RematchRequested)
        {
            RematchTime -= 1 * Time.deltaTime;          // Reduce the time
            RematchTimer.GetComponent<Text>().text = RematchTime.ToString("F0");                // Set the Text
            if (RematchTime <= 0)
            {
                RematchRequested = false;
                // TODO: Load Main Menu
            }
        }
    }

    public void EndGame(string result)
    {
        Timing += Time.deltaTime;

        if (Timing >= 1.0f)
        {
            Debug.Log(result);
            if (isWhiteturn)
            {
                //white team win
                resultTitle.text = "Black team wins";
                Debug.Log("Black team wins");
            }
            else if (m_BlackStart || m_WhiteStart)
            {
                //Black team win
                resultTitle.text = "White team wins";
                Debug.Log("White team wins");
            }
            else
            {
                // No one wins and we have an unranked match
                resultTitle.text = "Match unranked";
                //could be a tie
            }

            //wipe the whole board
            board.Wipe();

            // Make the restart screen appear
            RematchPanel.SetActive(true);
        }
    }

    public void Rematch()
    {


        if (!RematchRequested)
        {
            RematchRequested = true;
            RequestedText.SetActive(true);
            RematchTimer.SetActive(true);
            _NC.RequestRematch();           // Send request
        }
        else
        {
            RematchPanel.SetActive(false);
            RequestedText.SetActive(false);
            RematchTimer.SetActive(false);
            RematchRequested = false;
            StartNewGame();
        }
    }
    #endregion

    #region AI Code
    IEnumerator AIMove(List<PiecesManager> pieces)
    {
        AIMoving = true;
        yield return StartCoroutine(GetMiniMaxMove(Definitions.search_depth[Definitions.selectedDifficulty], pieces, -10000, 10000, isAIWhite, result, (x) => result = x));
        selectionX = board.PiecesManagers[result.selected_ind].CurrentX;
        selectionY = board.PiecesManagers[result.selected_ind].CurrentY;
        SelectPiece(selectionX, selectionY);

        //move piecemanager
        selectionX = result.selected_move_x;
        selectionY = result.selected_move_y;

        yield return new WaitForSeconds(1);

        if (selectedChessManager != board.PiecesManagers.AtLocation(selectionX, selectionY))
        {
            selected = false;
            BoardHighlight.Instance.HideHighlights();
            RevertSelectedPiece();
        }

        if (!MoveCausesCheck(selectedChessManager, selectionX, selectionY))
            MovePiece(selectionX, selectionY);
        isWhiteturn = !isAIWhite;
        AIMoving = false;
    }

    public IEnumerator GetMiniMaxMove(int depth, List<PiecesManager> pieces, float alpha, float beta, bool isMaximisingplayer, ChessAIResult result, Action<ChessAIResult> actionResult)
    {
        int temp_best, prevX, prevY, pos, check_lost, count = 0;

        check_lost = CheckLost(pieces, isMaximisingplayer);

        if (check_lost != -1 && depth < Definitions.search_depth[Definitions.selectedDifficulty])
        {
            result.bestValue = GetBoardEvaluate(pieces) + (10000 - depth) * (Definitions.AIWhite ? 1 : -1) * (check_lost == 0 ? 1 : -1);
            actionResult(result);
            yield break;
        }

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
                                StartCoroutine(GetMiniMaxMove(depth - 1, pieces, alpha, beta, !isMaximisingplayer, temp_result, (x) => temp_result = x));
                                temp_result.bestValue += GetKingProtectionPenalty(pieces, true);
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
                    count++;
                    if (count % 1 == 0)
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

                                StartCoroutine(GetMiniMaxMove(depth - 1, pieces, alpha, beta, !isMaximisingplayer, temp_result, (x) => temp_result = x));
                                temp_result.bestValue += GetKingProtectionPenalty(pieces, false);

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
                    count++;
                    if (count % 1 == 0)
                        yield return null;
                    //yield return null;
                }
            yield break;
            //return bestMove;
        }
    }

    private int CheckLost(List<PiecesManager> pieces, bool isWhite)
    {
        // State 0 means ai wins, 1 means another side wins, -1 means no one wins yet
        if (InCheckAI(isWhite, pieces))
            return 0;
        else if (InCheckAI(!isWhite, pieces))
            return 1;
        else return -1;
    }

    public bool InCheckAI(bool isWhite, List<PiecesManager> pieces)
    {
        //get the player's king
        PiecesManager king = pieces.TeamAndType(isWhite, typeof(King));

        //check if any move of the other players is in the king position
        foreach (PiecesManager piece in pieces.AllTeam(!isWhite))
        {
            bool[,] moves = piece.PossibleMovesAI(board.PiecesManagers);
            if (moves[king.CurrentX, king.CurrentY])
                return true;
        }
        return false;

        // TODO: Implement Audio
    }

    private float GetKingProtectionPenalty(List<PiecesManager> pieces, bool iswhite)
    {
        int friend_count = 0, enemy_count = 0;
        for (int i = 0; i < pieces.Count; i++)
            if (pieces[i].IsWhite == iswhite)
                friend_count++;
            else
                enemy_count++;

        return (Math.Max(enemy_count - friend_count, 0) * 5);
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
            if (Definitions.search_depth[Definitions.selectedDifficulty] < 2) // Choose easy then dont use optimized table
                sum += pieces[i].GetValue();
            else
                sum += pieces[i].GetValueAdvanced(pieces[i].CurrentX, pieces[i].CurrentY, pieces[i].IsWhite);
        return sum;
    }
    #endregion

    #region Timer Code

    public void WhiteFirstTurn() => m_WhiteStart = true;
    public void BlackFirstTurn() => m_BlackStart = true;

    private void WhiteTurnTimer()
    {
        if (m_WhiteStart)
        {
            whiteTimer.UpdateTimer();
            if (whiteTimer.secTotal == 0 && whiteTimer.minTotal == 0)
            {
                _audio.clip = TimeUpSound;

                if (!_audio.isPlaying)
                    _audio.Play();
                Debug.Log("WHITE TIME RUNS OUT");
                EndGame("Black Wins!");
            }
        }
        else
        {
            StartTime -= 1 * Time.deltaTime;
            if (StartTime <= 0)
            {
                _audio.clip = TimeUpSound;

                if (!_audio.isPlaying)
                    _audio.Play();
                Debug.Log("WHITE FAILED TO START");
                EndGame("White Aborted");
                // TODO: Cancel game
            }
        }
    }

    // This method will be used on the first turn for each player
    private void FirstTurn(string turn)
    {
        StartTime = 25;
        if (turn == "White")
        {
            BlackTurnTimer();
        }
        else
        {
            m_WhiteStart = true;
            m_BlackStart = true;
            WhiteTurnTimer();
        }
    }

    private void BlackTurnTimer()
    {
        if (!isWhiteturn)
        {
            if (m_BlackStart)
            {
                blackTimer.UpdateTimer();
                if (blackTimer.secTotal == 0 && blackTimer.minTotal == 0)
                {
                    _audio.clip = TimeUpSound;

                    if (!_audio.isPlaying)
                        _audio.Play();
                    Debug.Log("BLACK TIME RUNS OUT");
                    EndGame("White Wins!");
                }
            }
            else
            {
                StartTime -= 1 * Time.deltaTime;
                if (StartTime <= 0)
                {
                    _audio.clip = TimeUpSound;

                    if (!_audio.isPlaying)
                        _audio.Play();
                    Debug.Log("BLACK FAILED TO START");
                    EndGame("Black Aborted");
                    // TODO: Cancel Game
                }
            }
        }
    }
    #endregion

    public int[] GetPlayerRatings()
    {
        // TODO: Add method code to get player ratings
        int[] values = { 1, 2 };
        return values;
    }

    #region Piece Controller
    public void SelectPiece(int x, int y)
    {
        PiecesManager selectedPiece = board.GetPiece(x, y);
        //selection happens here
        if (selectedPiece == null)
            return;

        if (selectedPiece.IsWhite != isWhiteturn)
            return;


        allowedMoves = selectedPiece.PossibleMovesAI(board.PiecesManagers);
        selectedChessManager = selectedPiece;
        //assign selected material
        ShowSelectedPiece();
        //show allowed possible movement if available
        BoardHighlight.Instance.HighlightAllowedMoves(allowedMoves);
        //Debug.Log(PiecesManagers[x,y]);
        selected = true;
    }

    private void ShowSelectedPiece()
    {
        if (Mode == 0)
        {
            //apply new selected material to pieces
            previousMat = selectedChessManager.GetComponent<MeshRenderer>().material;
            selectedMat.mainTexture = previousMat.mainTexture;
            selectedChessManager.GetComponent<MeshRenderer>().material = selectedMat;
        }
        else
        {
            Vector3 origin = Vector3.zero;
            origin.x = (130.0f * selectionX) - 460f + 7 - selectionX;
            origin.y = (130.0f * selectionY) - 465f - (selectionY - 3);
            highlightinstance = Instantiate(highlight2D, origin, Quaternion.Euler(0, 0, 0)) as GameObject;
            highlightinstance.transform.SetParent(transform);

        }
    }

    public void RevertSelectedPiece()
    {
        //sets the previous material after move
        if (!selected)
        {
            if (Mode == 0)
                selectedChessManager.GetComponent<MeshRenderer>().material = previousMat;
            else
            {
                Destroy(highlightinstance);
            }
        }

    }

    private void MovePiece(int x, int y)
    {
        var captured = false;
        if (!allowedMoves[x, y])
            return;

        if (allowedMoves[x, y])
        {
            PiecesManager selectedPiece = board.GetPiece(x, y);
            if (selectedChessManager != null && selectedChessManager.GetType() == typeof(Pawn) && !promoting)
            {
                Pawn p = (Pawn)selectedChessManager;
                if (p.PromotionRank() == y)
                {
                    StartPromote(p, x, y);
                    return;
                }
            }
            if (selectedPiece != null)
            {
                if (selectedPiece.IsWhite != isWhiteturn)
                {
                    //capture a piece

                    //if is king
                    if (selectedPiece.GetType() == typeof(King))
                    {
                        //end game and check win
                        EndGame(isWhiteturn ? "White" : "Black");
                    }
                    RemovePiece(selectedPiece);
                    captured = true;
                }
                //castling
                else if (selectedPiece.GetType() == typeof(Rook) && selectedChessManager.GetType() == typeof(King) && selectedChessManager.IsWhite == selectedPiece.IsWhite)
                {
                    //move the rook to the kings position
                    if (selectedPiece.CurrentX < selectedChessManager.CurrentX)
                        x = selectedChessManager.CurrentX - 2;
                    else
                        x = selectedChessManager.CurrentX + 2;

                    y = selectedChessManager.CurrentY;

                    if (selectedPiece.CurrentX < selectedChessManager.CurrentX)
                    {
                        selectedPiece.transform.position = ChessCenter(selectedChessManager.CurrentX - 1, selectedChessManager.CurrentY);
                        //update the rook position
                        selectedPiece.SetPosition(selectedChessManager.CurrentX - 1, selectedChessManager.CurrentY);
                    }
                    else
                    {
                        selectedPiece.transform.position = ChessCenter(selectedChessManager.CurrentX + 1, selectedChessManager.CurrentY);
                        //update the rook position
                        selectedPiece.SetPosition(selectedChessManager.CurrentX + 1, selectedChessManager.CurrentY);
                    }
                    selectedPiece.Moved = true;
                }
            }

            //move the selected pieces to the allowd position
            selectedChessManager.transform.position = ChessCenter(x, y);
            if (_NC != null)
            {
                _NC.SendMoveToServer(x, y, selectedChessManager.index);         // Send the movement to the server
                                                                                // If a piece is captured then send that piece to the server
                if (captured)
                    _NC.SendCapture(selectedPiece.index);
            }

            //update the the piece position
            selectedChessManager.SetPosition(x, y);
            selectedChessManager.Moved = true;
            Debug.Log(captured);
            selectedChessManager.PlayAudio(captured ? "Captured" : "Move");


            selectedPiece = selectedChessManager;

            // switches the turn after move has been made
            if (timed)
            {
                if (isWhiteturn) { FirstTurn("White"); }
                if (!isWhiteturn) { FirstTurn("Black"); }
            }

            isWhiteturn = !isWhiteturn;
        }

        selected = false;

        RevertSelectedPiece();
        //hides the hilight 
        BoardHighlight.Instance.HideHighlights();

        selectedChessManager = null;



    }


    private void UpdateSelection()
    {
        Vector2Int selection = board.GetSelection();
        selectionX = selection.x;
        selectionY = selection.y;
    }

    #endregion

    #region Board Controller

    private void DrawBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);
            }
        }

        //draw selection Lline
        if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(Vector3.right * (selectionX) + Vector3.forward * selectionY,
                Vector3.right * (selectionX + 1) + Vector3.forward * selectionY, Color.black);

            Debug.DrawLine(Vector3.right * (selectionX) + Vector3.forward * selectionY,
                Vector3.right * (selectionX) + Vector3.forward * (selectionY + 1), Color.black);

            Debug.DrawLine(Vector3.right * (selectionX) + Vector3.forward * (selectionY + 1),
                Vector3.right * (selectionX + 1) + Vector3.forward * (selectionY + 1), Color.black);

            Debug.DrawLine(Vector3.right * (selectionX + 1) + Vector3.forward * (selectionY),
                Vector3.right * (selectionX + 1) + Vector3.forward * (selectionY + 1), Color.black);

            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));

            Debug.DrawLine(Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }

    public Vector3 ChessCenter(int x, int y)
    {
        return board.ChessCenter(x, y);
    }

    #endregion

    #region Spawn Controller
    private void SpawnChesman(int index, int x, int y, int id)
    {
        board.SpawnChesman(index, x, y, id);
    }


    private void SpawnAllPieces()
    {
        board.ResetLists();

        #region Whites
        //spawn Queen
        SpawnChesman(1, 3, 0, 0);
        //spawn King
        SpawnChesman(0, 4, 0, 1);
        //spawn rock
        SpawnChesman(2, 0, 0, 2);
        SpawnChesman(2, 7, 0, 3);
        //spawn bishop
        SpawnChesman(3, 2, 0, 4);
        SpawnChesman(3, 5, 0, 5);
        //spawn knight
        SpawnChesman(4, 1, 0, 6);
        SpawnChesman(4, 6, 0, 7);
        //spanw pawn
        for (int i = 0; i < 8; i++)
        {
            var id = 8 + i;
            SpawnChesman(5, i, 1, id);
        }
        #endregion

        #region Blacks

        //spawn King
        SpawnChesman(7, 3, 7, 16);
        //spawn Queen
        SpawnChesman(6, 4, 7, 17);
        //spawn rock
        SpawnChesman(8, 0, 7, 18);
        SpawnChesman(8, 7, 7, 19);
        //spawn bishop
        SpawnChesman(9, 2, 7, 20);
        SpawnChesman(9, 5, 7, 21);
        //spawn knight
        SpawnChesman(10, 1, 7, 22);
        SpawnChesman(10, 6, 7, 23);
        //spanw pawn
        for (int i = 0; i < 8; i++)
        {
            var id = 24 + i;
            SpawnChesman(11, i, 6, id);
        }
        #endregion
    }

    #endregion


    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Start a new Game if players select rematch
    /// </summary>
    public void StartNewGame()
    {
        //start new game
        isWhiteturn = true;
        BoardHighlight.Instance.HideHighlights();
        m_WhiteStart = false;
        m_BlackStart = false;


        if (!debug)
        {
            SpawnAllPieces();
        }

        if (timed)
        {
            whiteTimer.ResetTimer();
            blackTimer.ResetTimer();
            WhiteTurnTimer();
            StartTime = 25f;
        }

    }

    #region Check Game Strategies and Moves
    /// <summary>
    /// Check if the current player king under check, if not player can move, else player has to protect king
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="destX"></param>
    /// <param name="destY"></param>
    /// <returns></returns>
    public bool MoveCausesCheck(PiecesManager piece, int destX, int destY)
    {
        //backup the original position
        int currentX = piece.CurrentX;
        int currentY = piece.CurrentY;

        //change to new position
        piece.CurrentX = destX;
        piece.CurrentY = destY;

        //get the player's king
        PiecesManager king = board.PiecesManagers.TeamAndType(piece.IsWhite, typeof(King));

        //check if the move put the player's king in a check
        foreach (PiecesManager p in board.PiecesManagers.AllTeam(!piece.IsWhite))
        {
            //discard the piece if it would be captured with the current move
            if (p.CurrentX == destX && p.CurrentY == destY)
            {
                continue;
            }

            bool[,] moves = p.PossibleMovesAI(board.PiecesManagers);
            if (moves[king.CurrentX, king.CurrentY])
            {
                //restaurate position
                piece.CurrentX = currentX;
                piece.CurrentY = currentY;
                return true;
            }
        }
        //restaurate position
        piece.CurrentX = currentX;
        piece.CurrentY = currentY;
        return false;
    }


    public bool InCheckmate(bool isWhite)
    {
        if (InCheck(isWhite))
        {
            foreach (PiecesManager piece in board.PiecesManagers.AllTeam(isWhite))
            {
                //Check if each valid move stills causing a check
                {
                    bool[,] moves = piece.PossibleMovesAI(board.PiecesManagers);
                    for (int x = 0; x < 7; x++)
                    {
                        for (int y = 0; y < 7; y++)
                        {
                            if (moves[x, y])
                            {
                                if (MoveCausesCheck(piece, x, y) == false) return false;
                            }
                        }
                    }
                }
            }
            if (!inCheckMate && !_audio.isPlaying)
            {
                _audio.clip = (Mode == 0) ? CheckmateSound : CheckmateSound2D;
                _audio.Play();
            }
            inCheckMate = true;
            return true;
        }
        inCheckMate = false;
        return false;
        // TODO: Implement Audio
    }

    public bool InStale(bool isWhite)
    {
        if (!InCheck(isWhite))
        {
            foreach (PiecesManager piece in board.PiecesManagers.AllTeam(isWhite))
            {
                //Check if each valid move stills causing a check
                {
                    bool[,] moves = piece.PossibleMovesAI(board.PiecesManagers);
                    for (int x = 0; x < 7; x++)
                    {
                        for (int y = 0; y < 7; y++)
                        {
                            if (moves[x, y])
                            {
                                Debug.Log(piece.gameObject.name + ": " + MoveCausesCheck(piece, x, y));
                                if (!MoveCausesCheck(piece, x, y)) { return false; };
                            }
                        }
                    }
                }
            }

            if (!inStale && !_audio.isPlaying)
            {
                _audio.clip = (Mode == 0) ? StaleSound : StaleSound2D;
                _audio.Play();
            }
            inStale = true;
            return true;
        }
        inStale = false;
        return false;

        // TODO: Implement Audio
    }

    public bool InCheck(bool isWhite)
    {
        //get the player's king
        PiecesManager king = board.PiecesManagers.TeamAndType(isWhite, typeof(King));

        //check if any move of the other players is in the king position
        foreach (PiecesManager piece in board.PiecesManagers.AllTeam(!isWhite))
        {
            bool[,] moves = piece.PossibleMovesAI(board.PiecesManagers);
            if (moves[king.CurrentX, king.CurrentY])
            {
                if (kingcheckinstance != null) return true;
                if (Mode == 0)
                {
                    kingcheckinstance = Instantiate(kingCheck3D);
                    kingcheckinstance.transform.position = new Vector3(king.CurrentX + 0.5f, 0.01f, king.CurrentY + 0.5f);
                }
                else
                {
                    kingcheckinstance = Instantiate(kingCheck2D);
                    kingcheckinstance.transform.position = new Vector2(king.CurrentX * 130f - 460f + 7 - king.CurrentX, king.CurrentY * 130f - 460f - (king.CurrentY - 3));
                }
                if (!inCheck && !_audio.isPlaying)
                {
                    _audio.clip = (Mode == 0) ? InCheckSound : InCheckSound2D;
                    _audio.Play();
                }
                inCheck = true;
                return true;
            }
        }
        inCheck = false;
        Destroy(kingcheckinstance);
        return false;

        // TODO: Implement Audio
    }
    #endregion



    #region Promotion
    public void StartPromote(Pawn p, int x, int y)
    {
        promoting = true;
        promotingPiece = selectedChessManager;
        promotionX = x;
        promotionY = y;
        promotionPanel.SetActive(true);
    }

    public void RemovePiece(PiecesManager piece)
    {
        board.RemovePiece(piece);
    }

    public void ChoosePromotion(string piece)
    {
        int promotionPiece = -1;
        if (isWhiteturn)
        {
            if (piece.Equals("Queen"))
            {
                promotionPiece = 1; //white queen
            }
            else if (piece.Equals("Knignt"))
            {
                promotionPiece = 4; //black knight
            }
            else if (piece.Equals("Rook"))
            {
                promotionPiece = 2; //black rook
            }
            else if (piece.Equals("Bishop"))
            {
                promotionPiece = 3; //black bishop
            }
        }
        else
        {
            if (piece.Equals("Queen"))
            {
                promotionPiece = 7; //black queen
            }
            else if (piece.Equals("Knight"))
            {
                promotionPiece = 10; //black knight
            }
            else if (piece.Equals("Rook"))
            {
                promotionPiece = 8; //black rook
            }
            else if (piece.Equals("Bishop"))
            {
                promotionPiece = 9; //black bishop
            }
        }
        promoting = false;
        promotionPanel.SetActive(false);
        EndPromote(promotionPiece, promotionX, promotionY);

    }

    public void EndPromote(int promotionPiece, int x, int y)
    {
        if (promotionPiece != -1)
        {
            PiecesManager selectedPiece = board.GetPiece(x, y);
            RemovePiece(promotingPiece);
            SpawnChesman(promotionPiece, x, y, 1);			// TODO: Change to have correct ID
            promotionPiece = -1;
            //The king is guaranteed to both be on the board and awake. If there's a better way to get the sound to work let me know
            PiecesManager king = board.PiecesManagers.TeamAndType(isWhiteturn, typeof(King));

            if (selectedPiece != null)
            {
                king.PlayAudio("Captured");
                RemovePiece(selectedPiece);
            }
            else
            {
                king.PlayAudio("Move");
            }

            //Copied end of MovePiece():

            // switches the turn after move has been made
            if (timed)
            {
                if (isWhiteturn) { FirstTurn("White"); }
                if (!isWhiteturn) { FirstTurn("Black"); }
            }

            isWhiteturn = !isWhiteturn;

            selected = false;

            RevertSelectedPiece();
            //hides the highlight 
            BoardHighlight.Instance.HideHighlights();

            selectedChessManager = null;
        }
    }
    #endregion
}


