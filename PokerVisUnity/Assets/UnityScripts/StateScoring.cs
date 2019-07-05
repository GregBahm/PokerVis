using System.Collections.Generic;
using System.Linq;

public class StateScoring
{
    public int RoundsPlayed { get; private set; }
    public int Wins { get; private set; }
    public int Losses { get; private set; }
    public float WinProbability { get { return RoundsPlayed == 0 ? .5f : (float)Wins / RoundsPlayed; } }
    public float LossProbability { get { return RoundsPlayed == 0 ? .5f : (float)Losses / RoundsPlayed; } }

    public void RegisterHandScore(Hand playerhand, IEnumerable<Hand> opponentHands)
    {
        RoundsPlayed++;
        foreach (Hand opponent in opponentHands)
        {
            if(playerhand.Rank > opponent.Rank)
            {
                Losses++;
            }
            if(playerhand.Rank >= opponent.Rank)
            {
                return;
            }
            Wins++;
        }
    }
}
