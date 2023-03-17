using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutumnFramework;

[CreateAssetMenu(menuName ="Autumn/Config")]
[Config]
public class AutumnConfig : ScriptableObject
{
    public string HelloText;

    public bool firstStart;
}
