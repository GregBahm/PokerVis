using UnityEngine;

public class ScoringDisplayManager : MonoBehaviour
{
    public ComputeShader Computer;
    public ComputeBuffer DrawIndirectArgs { get; private set; }
    public Material BaseMat;
    public int ComputeKernel { get; private set; }
    public Mesh BaseMesh;

    public static ScoringDisplayManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DrawIndirectArgs = GetDrawIndirectArgs();
        ComputeKernel = Computer.FindKernel("ScoringDisplayCompute");
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
}
