using System.Collections.Generic;
using FlatBuffers;
using FlatNode.Runtime.Flat;
using UnityEngine;

namespace FlatNode.Runtime
{
    /// <summary>
    /// 通过入口节点来运行图
    /// </summary>
    public class LogicGraphBehaviour : GraphBehaviourBase
    {
        private OnClickNode onClickNode;
        private OnJoyStickDragNode onJoyStickDragNode;
        private OnJoyStickDownNode onJoyStickDownNode;
        private OnJoyStickUpNode onJoyStickUpNode;
        
        private SkillSequence onClickSequence;
        private SkillSequence onJoyStickDragSequence;
        private SkillSequence onJoyStickDownSequence;
        private SkillSequence onJoyStickUpSequence;

        private List<SkillSequence> allEntranceSequenceList;

#if UNITY_EDITOR
        public float runningTimer;
#endif

        public LogicGraphBehaviour(ByteBuffer byteBuffer) : base(byteBuffer)
        {
            allEntranceSequenceList = new List<SkillSequence>();
        }

        public override void Init()
        {
            //初始化入口节点
            GraphInfo graphInfo = GraphInfo.GetRootAsGraphInfo(byteBuffer);
            int entranceNodeCount = graphInfo.EntranceNodeIdsLength;
            for (int i = 0; i < entranceNodeCount; i++)
            {
                int nodeId = graphInfo.EntranceNodeIds(i);
                EntranceNodeBase entranceNode = DeserializeNode(nodeId) as EntranceNodeBase;
                if (entranceNode == null)
                {
                    Debug.LogError(string.Format("节点 {0} 不是入口节点",nodeId));
                    continue;
                }
                
                if (onClickNode == null)
                {
                    onClickNode = entranceNode as OnClickNode;
                }

                if (onJoyStickDragNode == null)
                {
                    onJoyStickDragNode = entranceNode as OnJoyStickDragNode;
                }

                if (onJoyStickDownNode == null)
                {
                    onJoyStickDownNode = entranceNode as OnJoyStickDownNode;
                }

                if (onJoyStickUpNode == null)
                {
                    onJoyStickUpNode = entranceNode as OnJoyStickUpNode;
                }
            }
        }

        public void OnClick()
        {
            if (onClickNode == null)
            {
                return;
            }

            if (onClickSequence == null)
            {
                onClickSequence = new SkillSequence(onClickNode, onClickNode.rightSideNodeIds, this, null);
                allEntranceSequenceList.Add(onClickSequence);
            }

            if (onClickSequence.IsFinish)
            {
                runningSequenceList.Add(onClickSequence);
                onClickSequence.Start();
            }
        }

        public void OnJoyStickDown()
        {
            if (onJoyStickDownNode == null)
            {
                return;
            }

            if (onJoyStickDownSequence == null)
            {
                onJoyStickDownSequence = new SkillSequence(onJoyStickDownNode, onJoyStickDownNode.rightSideNodeIds, this, null);
                allEntranceSequenceList.Add(onJoyStickDownSequence);
            }

            if (onJoyStickDownSequence.IsFinish)
            {
                runningSequenceList.Add(onJoyStickDownSequence);

                onJoyStickDownSequence.Start();
            }
        }

        public void OnJoyStickDrag(Vector2 joyStickInput)
        {
            if (onJoyStickDragNode == null)
            {
                return;
            }

            if (onJoyStickDragSequence == null)
            {
                onJoyStickDragSequence = new SkillSequence(onJoyStickDragNode, onJoyStickDragNode.rightSideNodeIds, this, null);
                allEntranceSequenceList.Add(onJoyStickDragSequence);
            }

            if (onJoyStickDragSequence.IsFinish)
            {
                runningSequenceList.Add(onJoyStickDragSequence);

                onJoyStickDragNode.joyStickInput = joyStickInput;
                onJoyStickDragSequence.Start();
            }
            else
            {
                //只更新
                onJoyStickDragNode.joyStickInput = joyStickInput;
            }
        }

        public void OnJoyStickUp(Vector2 joyStickInput)
        {
            if (onJoyStickUpNode == null)
            {
                return;
            }

            if (onJoyStickUpSequence == null)
            {
                onJoyStickUpSequence = new SkillSequence(onJoyStickUpNode, onJoyStickUpNode.rightSideNodeIds, this, null);
                allEntranceSequenceList.Add(onJoyStickUpSequence);
            }

            if (onJoyStickUpSequence.IsFinish)
            {
                runningSequenceList.Add(onJoyStickUpSequence);

                onJoyStickUpNode.joyStickInput = joyStickInput;
                onJoyStickUpSequence.Start();
            }
        }

        public override void Reset()
        {
            base.Reset();

            for (int i = 0; i < allEntranceSequenceList.Count; i++)
            {
                allEntranceSequenceList[i].Reset();
            }
//            allEntranceSequence.Clear();//不要clear
        }

        public override void Dispose()
        {
            base.Dispose();

            for (int i = 0; i < allEntranceSequenceList.Count; i++)
            {
                allEntranceSequenceList[i].Dispose();
            }
        }
    }
}