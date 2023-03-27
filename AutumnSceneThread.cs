using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
namespace AutumnFramework
{
    [Bean]
    public class AutumnSceneThread : MonoBehaviour
    {
        [Autowired]
        private static AutumnConfig autumnConfig;

        // [SerializeField]
        // [Autowired]
        // private List<数据层> 多个数据层多个BEANS;
        private void Update()
        {
            Autumn.Call("Update");
        }

        private void Start() {
            Debug.Log(autumnConfig.HelloText);

            AutumnUtil.WaitForNextFrame(()=>{
                Autumn.Call("AfterStart");
            });
        }
    }
}