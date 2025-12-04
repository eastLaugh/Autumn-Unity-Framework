using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutumnFramework;

[CreateAssetMenu(menuName = "Autumn/Config")]
[Config]
public class AutumnConfig : ScriptableObject {
    public string HelloText;

    [Header("指南")]
    public bool FirstStart;
    public string FirstStartMessage;

    [Header("实验性")]
    public bool 切换场景时自动装配;
    [Header("Autumn Core Exception")]
    public string 重复安装Bean;
    public string 场景丢失Bean;
}
