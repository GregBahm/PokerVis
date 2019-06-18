using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class Mainscript : MonoBehaviour
{
    private void Start()
    {
        HandScoresTable elTable = new HandScoresTable();
        Debug.Log(elTable.All.Count);
    }
}
