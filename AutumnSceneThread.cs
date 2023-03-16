using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
namespace AutumnFramework
{
    [Bean]
    public class AutumnSceneThread : MonoBehaviour
    {


        [SerializeField]
        [Autowired]
        private List<数据层> 多个数据层多个BEANS;
        // private 数据层_让数据层变成BEAN抽离出来[] hh;
        private void Update()
        {
            Autumn.Call("Update");
        }
    }
}