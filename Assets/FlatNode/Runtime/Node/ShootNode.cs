//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using RR.Game.Test;
//using RR.Game;

//namespace RR.SkillRuntime
//{
//    [SkillNode("Shoot", "Function/Shoot", "开枪")]
//    public class ShootNode : NodeBase
//    {
//        //射击方式
//        public enum ShootType
//        {
//            None = 0,
//            Single,     //单射
//            Burst,      //连射
//            Scatter,    //散射
//        }

//        [NodeInputPort(0, "冷却时间")] private NodeInputVariable<float> coolDownInputVariable;
//        //[NodeInputPort(1, "射击方式")] private NodeInputVariable<ShootType> shootTypeInputVariable;
//        [NodeInputPort(2, "射程")] private NodeInputVariable<int> fieldInputVariable;
//        [NodeInputPort(3, "射击速度")] private NodeInputVariable<float> speedInputVariable;
//        [NodeInputPort(4, "加速度")] private NodeInputVariable<float> accelerateInputVariable;
//        [NodeInputPort(5, "子弹半径")] private NodeInputVariable<float> radiusInputVariable;
//        [NodeInputPort(6, "伤害值")] private NodeInputVariable<float> hurtInputVariable;
//        [NodeInputPort(7, "是否可以穿透")] private NodeInputVariable<bool> canPassInputVariable;
//        [NodeInputPort(8, "弹道特效")] private NodeInputVariable<string> passEffectInputVariable;
//        [NodeInputPort(9, "拖尾特效")] private NodeInputVariable<string> trailEffectInputVariable;
//        [NodeInputPort(10, "枪口火花特效")] private NodeInputVariable<string> appearEffectInputVariable;
//        //[NodeInputPort(11, "连发间隔时间", "如果是连射的情况需要填写这个")] private NodeInputVariable<float> fireIntervalInputVariable;
//        //[NodeInputPort(12, "连发次数", "如果是连射的情况需要填写这个")] private NodeInputVariable<int> fireCntInputVariable;
//        //[NodeInputPort(13, "散射角度范围", "如果是散射的情况需要填写这个")] private NodeInputVariable<int> angleInputVariable;
//        //[NodeInputPort(14, "散射子弹个数", "如果是散射的情况需要填写这个")] private NodeInputVariable<int> bulletCntInputVariable;

//        private float coolDown;
//        private ShootType shootType;
//        private int field;
//        private float speed;
//        private float accelerate;
//        private float radius;
//        private float hurt;
//        private bool canPass;
//        private string passEffect;
//        private string trailEffect;
//        private string appearEffect;
//        private float fireInterval;
//        private int fireCnt;
//        private int angle;
//        private int bulletCnt;

//        private Vector3 mStartPos;     //记录子弹开始飞行的坐标
//        private Quaternion mStartRot;  //记录子弹开始飞行的旋转记录
//        private Vector3 mDirection;    //记录子弹飞行的方向
//        private Vector3 mTargetPos;    //记录子弹飞行的目标地点

//        CharacterEntity mCaster;       //玩家实体
//        Transform mBulletEffect;

//        private bool mIsMakeBullet = false;
//        private bool mIsMoveFinish = false;
//        float mSumTime;
//        Vector3 mMovePos;

//        public override void OnEnter()
//        {
//            base.OnEnter();

//            coolDown = GetInputValue(coolDownInputVariable);
//            //shootType = GetInputValue(shootTypeInputVariable);
//            field = GetInputValue(fieldInputVariable);
//            speed = GetInputValue(speedInputVariable);
//            accelerate = GetInputValue(accelerateInputVariable);
//            radius = GetInputValue(radiusInputVariable);
//            hurt = GetInputValue(hurtInputVariable);
//            canPass = GetInputValue(canPassInputVariable);
//            passEffect = GetInputValue(passEffectInputVariable);
//            trailEffect = GetInputValue(trailEffectInputVariable);
//            appearEffect = GetInputValue(appearEffectInputVariable);
//            //fireInterval = GetInputValue(fireIntervalInputVariable);
//            //fireCnt = GetInputValue(fireCntInputVariable);
//            //angle = GetInputValue(angleInputVariable);
//            //bulletCnt = GetInputValue(bulletCntInputVariable);

//            mCaster = TestGameScene.Current.World.GetOrCreateData<WorldAllHeroData>().CurPlayer;
//            mDirection = mCaster.enTransform.Rotation * Vector3.forward;
//            mStartPos = mCaster.enTransform.Position;
//            mStartRot = mCaster.enTransform.Rotation;
//            mTargetPos = mStartPos + mDirection * field;

//            MakeBullet();
//        }

//        private void MakeBullet()
//        {
//            if (passEffect != string.Empty && !passEffect.Equals(""))       // 子弹射击特效
//            {
//                PLD.ResLoader loader = PLD.ResManager.Instance.Load(passEffect);
//                if (loader != null)
//                {
//                    Object obj = loader.mMainAsset;
//                    GameObject currentObj = GameObject.Instantiate(obj) as GameObject;
//                    mBulletEffect = currentObj.transform;
//                    mBulletEffect.SetParent(null);
//                    mBulletEffect.position = mStartPos;
//                    mBulletEffect.rotation = mStartRot;
//                    mBulletEffect.gameObject.SetActive(true);
//                }
//            }
//            mIsMakeBullet = true;
//        }

//        public override void OnUpdate(float deltaTime)
//        {
//            base.OnUpdate(deltaTime);

//            if (mIsMakeBullet)
//            {
//                if (!mIsMoveFinish)
//                {
//                    CalculateMovePos(deltaTime);//计算当前帧移动的目标点
//                    BulletMove();//子弹移动
//                }
//                else
//                {
//                    MoveFinished();
//                }
//            }
//        }

//        /// <summary>
//        /// 还需要移动
//        /// </summary>
//        void CalculateMovePos(float deltaTime)
//        {
//            mSumTime += deltaTime;
//            mMovePos = Vector3.zero;
//            float completeness = speed * mSumTime
//                + 0.5f * accelerate * Mathf.Pow(mSumTime, 2);
//            completeness = completeness / field;
//            if (completeness >= 1)
//            {
//                mMovePos = mTargetPos;
//                MoveFinished();
//            }
//            mMovePos = mStartPos * (1 - completeness) + mTargetPos * completeness;
//        }

//        private void BulletMove()
//        {
//            if (null != mBulletEffect)
//            {
//                mBulletEffect.position = mMovePos;
//                mBulletEffect.rotation = mStartRot;
//            }
//        }

//        private void MoveFinished()
//        {
//            mIsMoveFinish = true;
//            if (null != mBulletEffect)
//                GameObject.Destroy(mBulletEffect.gameObject);
//            Finish();
//        }
//    }
//}