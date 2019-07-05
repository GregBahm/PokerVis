using System;
using UnityEngine;

public class ScoringDisplayManager : MonoBehaviour
{
    public ComputeShader Computer;
    public ComputeBuffer DrawIndirectArgs { get; private set; }
    public Material BaseMat;
    private const int ThreadsCount = 128;
    public int PlayerKernel { get; private set; }
    public int OpponentKernel { get; private set; }
    public Mesh BaseMesh;
    private int groupsCount;

    public static ScoringDisplayManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DrawIndirectArgs = GetDrawIndirectArgs();
        PlayerKernel = Computer.FindKernel("PlayerCompute");
        OpponentKernel = Computer.FindKernel("OpponentCompute");
        groupsCount = Mathf.CeilToInt((float)ScoreAnalysisTable.UniqueSevenCardHands / ThreadsCount);
    }

    private void OnDestroy()
    {
        DrawIndirectArgs.Dispose();
    }

    private ComputeBuffer GetDrawIndirectArgs()
    {
        uint[] args = new uint[]
        {
            BaseMesh.GetIndexCount(0),
            ScoreAnalysisTable.UniqueSevenCardHands,
            0,
            0,
            0
        };
        ComputeBuffer ret = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        ret.SetData(args);
        return ret;
    }

    internal void DoCompute(ComputeBuffer scoresBuffer, int playerHand, int opponentHand)
    {
        Computer.SetInt("_PlayerHandIndex", playerHand);
        Computer.SetInt("_OpponentHandIndex", opponentHand);

        Computer.SetBuffer(PlayerKernel, "_Scores", scoresBuffer);
        Computer.Dispatch(PlayerKernel, 1, 1, 1);

        Computer.SetBuffer(OpponentKernel, "_Scores", scoresBuffer);
        Computer.Dispatch(OpponentKernel, 1, 1, 1);
    }
}
