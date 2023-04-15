using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace AutumnFramework
{
    [Bean]
    public class AutumnSceneGuard : MonoBehaviour
    {
        [Autowired]
        private static AutumnConfig autumnConfig;

        [SerializeField]
        [Autowired]
        private List<数据层> 多个数据层多个BEANS;
        private void Update()
        {
            Autumn.Call("Update");
        }
        private void Awake()
        {

        }
        private void Start()
        {
            StartCoroutine(WaitForNextFrame(() =>
            {
                Autumn.Call("AfterStart");
            }));
        }

        public static IEnumerator WaitForNextFrame(Action action)
        {
            yield return null;
            action?.Invoke();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (autumnConfig.切换场景时自动装配)
            {
                Autumn.Autowired();
            }
        }
    }
}