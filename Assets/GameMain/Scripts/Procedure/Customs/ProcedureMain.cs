using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Procedure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace FlappyBird
{
    /// <summary>
    /// 主流程
    /// </summary>
    public class ProcedureMain : ProcedureBase
    {
        /// <summary>
        /// 管道产生时间
        /// 管道产生时间间隔
        /// </summary>
        private float m_PipeSpawnTime = 0f;

        /// <summary>
        /// 管道产生计时器
        /// </summary>
        private float m_PipeSpawnTimer = 0f;

        /// <summary>
        /// 结束界面ID
        /// </summary>
        private int m_ScoreFormId = -1;

        /// <summary>
        /// 是否返回主菜单
        /// </summary>
        private bool m_IsReturnMenu = false;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            //1 打开UI计分窗口
            m_ScoreFormId = GameEntry.UI.OpenUIForm(UIFormId.ScoreForm).Value;
            //2 显示背景游戏物体
            GameEntry.Entity.ShowBg(new BgData(GameEntry.Entity.GenerateSerialId(), 1, 1f, 0));
            //3 显示小鸟
            GameEntry.Entity.ShowBird(new BirdData(GameEntry.Entity.GenerateSerialId(), 3, 5f));

            //4 设置初始管道产生时间
            m_PipeSpawnTime = Random.Range(3f, 5f);

            //5 订阅事件 记录是否返回主菜单的值 的 变化事件
            GameEntry.Event.Subscribe(ReturnMenuEventArgs.EventId, OnReturnMenu);
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            //1 产生管道职能
            //管道产生计时器    经过的秒
            m_PipeSpawnTimer += elapseSeconds;
            // 如果管道产生计时器 大于等于 管道产生时间间隔
            if (m_PipeSpawnTimer >= m_PipeSpawnTime)
            {
                m_PipeSpawnTimer = 0;//计时器清零
                //随机设置管道产生时间 在3到5之间
                m_PipeSpawnTime = Random.Range(3f, 5f);

                //产生管道
                GameEntry.Entity.ShowPipe(new PipeData(GameEntry.Entity.GenerateSerialId(), 2, 1f));

            }

            //2 切换场景职能
            if (m_IsReturnMenu)//如果返回主菜单的标志值为true
            {
                m_IsReturnMenu = false;
                procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, GameEntry.Config.GetInt("Scene.Menu"));
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            //1 关闭UI
            GameEntry.UI.CloseUIForm(m_ScoreFormId);

            //2 取消订阅事件
            GameEntry.Event.Unsubscribe(ReturnMenuEventArgs.EventId, OnReturnMenu);
        }

        private void OnReturnMenu(object sender, GameEventArgs e)
        {
            //1 设置是否返回主菜单标志值
            m_IsReturnMenu = true;
        }
    }

}
