using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

public class StateScoring
{
    public int RoundsPlayed { get; private set; }
    public int Wins { get; private set; }
    public float WinProbability { get { return RoundsPlayed == 0 ? .5f : (float)Wins / RoundsPlayed; } }
    public void RegisterHandScore(Hand playerhand, IEnumerable<Hand> opponentHands)
    {
        RoundsPlayed++;
        bool win = opponentHands.All(opponent => playerhand.Probabilities.Rank < opponent.Probabilities.Rank);
        if(win)
        {
            Wins++;
        }
    }
}

public class Mainscript : MonoBehaviour
{
    public int OpponentsCount;
    public TextMeshPro WinProbabilityText;

    private Thread thread;
    private object locker = new object();

    public HandState HandState;
    public StateScoring Scoring;
    private RandomHandGenerator playerHandGenerator;
    private RandomHandGenerator opponentHandGenerator;

    public static Mainscript Instance;

    public float CardOptionMargin;
    public const float CardHeight = 1.62f;

    public GameObject CardOptionPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HandState = new HandState();
        RefreshGenerators();
        thread = new Thread(() => PerpetuallyScoreHands());
        thread.IsBackground = true;
        thread.Start();
    }

    private void OnDestroy()
    {
        thread.Abort();
    }

    private void PerpetuallyScoreHands()
    {
        while (true)
        {
            Hand playerHand = playerHandGenerator.GetRandomHand();
            Hand[] opponentHands = new Hand[OpponentsCount];
            for (int i = 0; i < OpponentsCount; i++)
            {
                opponentHands[i] = opponentHandGenerator.GetRandomHand();
            }
            Scoring.RegisterHandScore(playerHand, opponentHands);
        }
    }

    private void Update()
    {
        RefreshGenerators();
        UpdateWinProbabilityLabel();
    }

    private void UpdateWinProbabilityLabel()
    {
        string text = "Wind Probability: " + (int)(Scoring.WinProbability * 100) + "%";
        WinProbabilityText.text = text;
    }

    private void RefreshGenerators()
    {
        if (!HandState.PlayerGeneratorUpToDate || !HandState.OpponentGeneratorUpToDate)
        {
            lock (locker)
            {
                playerHandGenerator = HandState.GetPlayerHandGenerator();
                opponentHandGenerator = HandState.GetOpponentHandGenerator();
                Scoring = new StateScoring();
            }
        }
    }
}
