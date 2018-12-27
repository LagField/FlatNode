using System.Collections;
using System.Collections.Generic;
using FlatBuffers;
using UnityEngine;
using FlatNode.Runtime;

public class TestBase : MonoBehaviour {
    private GraphBehaviour graphBehaviour;

    private void Start()
    {
        //合适的时机调用初始化节点反序列化功能的类
        FlatNodeUtility.Init();
        
        TextAsset textAsset = Resources.Load<TextAsset>("GraphRuntime/0");
        if (textAsset == null)
        {
            return;
        }

        byte[] bytes = textAsset.bytes;
        ByteBuffer byteBuffer = new ByteBuffer(bytes);
        
        graphBehaviour = new GraphBehaviour(byteBuffer);
        graphBehaviour.Init();
        //行为图的入口方法，可自行拓展
        graphBehaviour.OnClick();
    }

    private void Update()
    {
        //需要一个地方来驱动行为图的Update
        if (graphBehaviour != null)
        {
            graphBehaviour.Update(Time.deltaTime);
        }
    }
}
