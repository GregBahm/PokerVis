using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class Mainscript : MonoBehaviour
{
    public static Mainscript Instance;

    public float CardOptionMargin;
    public const float CardHeight = 1.62f;

    public GameObject CardOptionPrefab;

    private void Awake()
    {
        Instance = this;
    }
}
