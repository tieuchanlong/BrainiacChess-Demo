using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BoardControl : MonoBehaviour
{
    public List<PiecesManager> PiecesManagers { set; get; }
    public List<GameObject> gamePrefabs;
    protected List<GameObject> activePrefabs;
    protected const float TILE_SIZE = 1.0f;
    protected const float TILE_OFFSET = 0.5f;
    protected const float TILE_SIZE2D = 128.0f;
    protected const float TILE_OFFSET2D = -448f;
    public Quaternion orientation = Quaternion.Euler(0, 0, 0);
    public int Mode = Definitions.Mode;


    public void SpawnChesman(int index, int x, int y, int id)
    {
        GameObject go;
        // TODO: Optimize
        if (index != 10)
        {
            go = Instantiate(gamePrefabs[index], ChessCenter(x, y), Quaternion.Euler(-90, 0, 0)) as GameObject;
        }
        else
        {
            go = Instantiate(gamePrefabs[index], ChessCenter(x, y), Quaternion.Euler(-90, 0, 0)) as GameObject;
        }

        if (Mode == 1)
            go.transform.localEulerAngles = new Vector3(0, 0, 0);

        go.transform.SetParent(transform);

        PiecesManager newPiece = go.GetComponent<PiecesManager>();
        newPiece.SetPosition(x, y);

        PiecesManagers.Add(newPiece);
        newPiece.index = id.ToString();

        activePrefabs.Add(go);
    }

    public Vector3 ChessCenter(int x, int y)
    {
        Vector3 orign = Vector3.zero;

        if (Mode == 0)
        {
            orign.x += (TILE_SIZE * x) + TILE_OFFSET;
            orign.z += (TILE_SIZE * y) + TILE_OFFSET;
            //orign.y += 0.2f;
        }
        else
        {
            orign.x += (TILE_SIZE2D * x) + TILE_OFFSET2D;
            orign.y += (TILE_SIZE2D * y) + TILE_OFFSET2D;
        }

        return orign;
    }

    public void RemovePiece(PiecesManager piece)
    {
        PiecesManagers.Remove(piece);
        activePrefabs.Remove(piece.gameObject);
        Destroy(piece.gameObject);
    }

    public void Wipe()
    {
        Debug.Log("Wiped");
        foreach (GameObject go in activePrefabs)
            Destroy(go);
    }

    public void ResetLists()
    {
        Debug.Log("Reset");
        activePrefabs = new List<GameObject>();
        PiecesManagers = new List<PiecesManager>();
    }

    public PiecesManager GetPiece(int x, int y)
    {
        return PiecesManagers.AtLocation(x, y);
    }

    public Vector2Int GetSelection()
    {
        if (!Camera.main)
            return new Vector2Int();
        int selectionX = -1;
        int selectionY = -1;

        if (Mode == 0)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 250.0f, LayerMask.GetMask("ChessPlane")))
            {
                selectionX = (int)Mathf.Floor(hit.point.x);
                selectionY = (int)Mathf.Floor(hit.point.z);
                if (selectionX == 8 || selectionY == 8)
                {
                    selectionX = -1;
                    selectionY = -1;
                }
            }
            else
            {
                selectionX = -1;
                selectionY = -1;
            }
        }
        else
        {
            selectionX = ((int)Input.mousePosition.x - 176) / 83;
            selectionY = ((int)Input.mousePosition.y - 34) / 83;
        }

        return new Vector2Int(selectionX, selectionY);
    }

    public string[] Load(string file)
    {
        string[] output = new string[2];
        output[0] = "";
        Wipe();
        ResetLists();
        Directory.CreateDirectory(Application.dataPath + "/SavedBoards");
        string path = Application.dataPath + "/SavedBoards/" + file + ".txt";
        if (!File.Exists(path))
        {

            output[0] = "Path not found";
            output[1] = "";
            return output;
        }
        string content = File.ReadAllText(path);
        output[1] = content[0].ToString();

        int id = 0;
        for (int i = 2; i < content.Length; i += 5)
        {
            int j;
            if (content[i] == 'W')
            {
                j = 0;
            }
            else
            {
                j = 6;
            }
            SpawnChesman(PieceToWhiteIndex(content[i + 1].ToString()) + j, (int)char.GetNumericValue(content[i + 2]), (int)char.GetNumericValue(content[i + 3]), id++);
        }
        return output;
    }

    public int PieceToWhiteIndex(string type)
    {
        if (type.Equals("King") || type.Equals("K"))
        {
            return 0;
        }
        else if (type.Equals("Queen") || type.Equals("Q"))
        {
            return 1;
        }
        else if (type.Equals("Rook") || type.Equals("R"))
        {
            return 2;
        }
        else if (type.Equals("Bishop") || type.Equals("B"))
        {
            return 3;
        }
        else if (type.Equals("Knight") || type.Equals("N"))
        {
            return 4;
        }
        else if (type.Equals("Pawn") || type.Equals("P"))
        {
            return 5;
        }
        else if (type.Equals("Delete"))
        {
            return -2;
        }
        return -1;
    }
}

public static class Extensions
{
    public static PiecesManager AtLocation(this List<PiecesManager> pieces, int x, int y)
    {
        return pieces.Where(p => p.CurrentX == x && p.CurrentY == y).FirstOrDefault();
    }

    public static PiecesManager TeamAndType(this List<PiecesManager> pieces, bool white, System.Type type)
    {
        return pieces.Where(p => p.IsWhite == white && p.GetType() == type).FirstOrDefault();
    }

    public static List<PiecesManager> AllTeam(this List<PiecesManager> pieces, bool white)
    {
        return pieces.Where(p => p.IsWhite == white).ToList();
    }

}
