using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
    [SkillNode("OnClick","EntranceNodes/OnClick","点击技能",isEntranceNode = true)]
    [NodeFlowOutPortDefine(0,"点击技能时")]
    public class OnClickNode : EntranceNodeBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            
            EventTiming(0);
            Finish();
        }
    }
}