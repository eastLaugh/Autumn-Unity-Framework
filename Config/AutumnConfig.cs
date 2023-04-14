using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutumnFramework;

[CreateAssetMenu(menuName ="Autumn/Config")]
[Config]
public class AutumnConfig : ScriptableObject
{
    public string HelloText;

    [Header("指南")]
    public bool FirstStart;
    public string FirstStartMessage;

    [Header("Autumn Core Exception")]
    public string 重复安装Bean;
}
