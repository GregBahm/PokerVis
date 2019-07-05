using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

public class Mainscript : MonoBehaviour
{
    public int OpponentsCount;
    public TextAsset HandFrequencyTable;
    public TextMeshPro WinProbabilityText;

    private Thread thread;

    private ScoreAnalysisTable scoreAnalysis;
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
        scoreAnalysis = new ScoreAnalysisTable(HandFrequencyTable);
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

    public Hand GetRandomPlayerHand()
    {
        return playerHandGenerator.GetRandomHand();
    }
    
    public Hand GetRandomOpponentHand()
    {
        return opponentHandGenerator.GetRandomHand();
    }

    private void PerpetuallyScoreHands()
    {
        while (true)
        {
            Hand playerHand = GetRandomPlayerHand();
            Hand[] opponentHands = new Hand[OpponentsCount];
            for (int i = 0; i < OpponentsCount; i++)
            {
                opponentHands[i] = GetRandomOpponentHand();
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
        string text = "Win Probability: " + (int)(Scoring.WinProbability * 100) + "%";
        WinProbabilityText.text = text;
    }

    private void RefreshGenerators()
    {
        if (!HandState.PlayerGeneratorUpToDate || !HandState.OpponentGeneratorUpToDate)
        {
            playerHandGenerator = HandState.GetPlayerHandGenerator();
            opponentHandGenerator = HandState.GetOpponentHandGenerator();
            Scoring = new StateScoring();
        }
    }
}
