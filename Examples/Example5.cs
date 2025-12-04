//使用[Config]配置资源库

using AutumnFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Bean]
public class 主系统 {
}

public class 资源库 {
    public static 音乐 场景1音乐库;
    public static 音乐 场景2音乐库;
    public static 美术 略;
}

[System.Serializable]
[Config]
public class 音乐 : ScriptableObject {
    public List<AudioClip> BackgrooundMusic;

    public void 播放选集(String name) {
        BackgrooundMusic.Where(clip => clip.name == name);
    }
    public void 播放选集(int index) {
    }
}

[Config]
public class 美术 : ScriptableObject {
    // .... 省略
}


[Bean]
public class AudioPlayer : MonoBehaviour {

    private void Start() {

    }
}