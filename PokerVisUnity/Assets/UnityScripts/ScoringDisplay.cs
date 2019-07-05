using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ScoringDisplay : MonoBehaviour
{
    public float BoxMargin;
    private BoxCollider boundingBox;
    private ComputeBuffer scoreDataBuffer;
    private const int DisplayItemStride = sizeof(uint) * 2;
    private Material scoreVisualMat;
    
    private struct ScoreDisplayData
    {
        public uint PlayerWins;
        public uint OpponentWins;
    }

    private void Start()
    {
        GetComponent<MeshRenderer>();
        boundingBox = GetComponent<BoxCollider>();
        scoreDataBuffer = GetScoreDataBuffer();
        scoreVisualMat = new Material(ScoringDisplayManager.Instance.BaseMat);
    }

    private ComputeBuffer GetScoreDataBuffer()
    {
        return new ComputeBuffer(ScoreAnalysisTable.UniqueSevenCardHands, DisplayItemStride);
    }

    private void Update()
    {
        DrawBoxes();
    }

    void DrawBoxes()
    {
        scoreVisualMat.SetInt("_BoxCount", ScoreAnalysisTable.UniqueSevenCardHands);
        scoreVisualMat.SetBuffer("_ScoreData", scoreDataBuffer);
        scoreVisualMat.SetFloat("_BoxMargin", BoxMargin);
        scoreVisualMat.SetVector("_PositionFixer", transform.position);
        scoreVisualMat.SetMatrix("_Transform", transform.localToWorldMatrix);
        Graphics.DrawMeshInstancedIndirect(
            ScoringDisplayManager.Instance.BaseMesh,
            0,
            scoreVisualMat,
            boundingBox.bounds,
            ScoringDisplayManager.Instance.DrawIndirectArgs);
    }

    private void OnDestroy()
    {
        scoreDataBuffer.Dispose();
    }
}
