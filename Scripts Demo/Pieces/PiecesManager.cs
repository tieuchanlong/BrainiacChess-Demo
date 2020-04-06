using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class PiecesManager : MonoBehaviour
{
   public int CurrentX{set; get;}
   public int CurrentY { set; get; }
   public bool IsWhite;
	public bool Moved; //used fors castling
  
   private AudioSource _Audio;
   [Header("Audio")]
   public AudioClip MoveSound;
   public AudioClip CaptureSound;
    public AudioClip MoveSound2D;
    public AudioClip CaptureSound2D;

    public string index;
    private int Mode;

   protected virtual void Awake()
   {
       _Audio = GetComponent<AudioSource>();
       _Audio.playOnAwake = false;
       _Audio.loop = false;
        Mode = Definitions.Mode;

        if (Mode == 0)
            _Audio.clip = MoveSound;
        else
            _Audio.clip = CaptureSound2D;
   }

    
    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
        
    }

    public virtual bool[,] PossibleMoves()
    {
        return new bool[8, 8];
    }

    /// <summary>
    /// Possible moves for Chess AI
    /// </summary>
    /// <param name="pieces"></param>
    /// <returns></returns>
    public virtual bool[,] PossibleMovesAI(List<PiecesManager> pieces)
    {
        return new bool[8, 8];
    }

    public virtual void PlayAudio(string action)
    {
        if (action == "Captured")
        {
            _Audio.clip = (Mode == 0) ? CaptureSound : CaptureSound2D;
        } else
        {
            _Audio.clip = (Mode == 0) ? MoveSound : MoveSound2D;
        }
        
        _Audio.Play();
    }

    public PiecesManager AtLocation(List<PiecesManager> pieces, int x, int y)
    {
        return pieces.Where(p => p.CurrentX == x && p.CurrentY == y).FirstOrDefault();
    }

    public virtual int GetValue()
    {
        return 0;
    }

    public virtual float GetValueAdvanced(int x, int y, bool isWhite)
    {
        return 0;
    }
}
