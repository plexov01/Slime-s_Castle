using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SpiderBossPhase1;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpiderBossPhase1
{
    public class StateMachine
    {
        public State currentState;
        public GameObject bossObject;
        
        public Animator animator;
        public scr_BossSpiderPhase1 spiderPhase1Script;

        public void Init(State state, Animator animator, scr_BossSpiderPhase1 spiderPhase1Script, GameObject bossObject)
        {
            currentState = state;
            this.animator = animator;
            this.spiderPhase1Script = spiderPhase1Script;
            this.bossObject = bossObject;
            
            state.Enter();
        }
        
        public void ChangeState(State newState)
        {
            currentState.Exit();
            currentState = newState;
            newState.Enter();
        }

        // public void ChangeState(string stateName)
        // {
        //     switch (stateName)
        //     {
        //         case "WaitingForStart":
        //             ChangeState(new WaitingForStart(this));
        //             break;
        //         case "StartingFight":
        //             ChangeState(new StartingFight(this));
        //             break;
        //         case "Attacking":
        //             ChangeState(new Attacking(this));
        //             break;
        //         case "Stunned":
        //             ChangeState(new Stunned(this));
        //             break;
        //         case "SuperAttacking":
        //             ChangeState(new SuperAttacking(this));
        //             break;
        //     }
        // }
    }

    public abstract class State
    {
        protected StateMachine stateMachine;

        public State(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }
        
        public virtual void Enter()
        {
                
        }
            
        public virtual void UpdateLogic()
        {
                
        }
        
        public virtual void Hit()
        {
                
        }
            
        public virtual void Exit()
        {
                
        }
    }
    
    public class WaitingForStart : State
    {
        public WaitingForStart(StateMachine stateMachine) : base(stateMachine){ }
        
    }
    
    public class StartingFight : State
    {
        public StartingFight(StateMachine stateMachine) : base(stateMachine) { }
        
        private bool isStartingFight = false;

        public override void UpdateLogic()
        {
            if (!isStartingFight)
            {
                isStartingFight = true;
                stateMachine.animator.Play("StartingFight");
                return;
            }

            var isAnimating = stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName("StartingFight");
            if (isAnimating)
                return;
            
            stateMachine.ChangeState(new Waiting(stateMachine));
        }
    }
    
    public class Waiting : State
    {
        public Waiting(StateMachine stateMachine) : base(stateMachine){ }

        private float idleTimeUpperLimit = 5f;
        private float idleTimeLowerLimit = 2f;
        private float idleTimeLeft = 0f;

        private bool isWaiting = false;

        public override void Enter()
        {
            isWaiting = false;
            idleTimeLeft = Random.Range(idleTimeLowerLimit, idleTimeUpperLimit);
        }

        public override void UpdateLogic()
        {
            if (isWaiting)
            {
                idleTimeLeft -= Time.deltaTime;
            }
            else
            {
                isWaiting = true;
                stateMachine.animator.Play("Waiting");
                return;
            }
            
            if (idleTimeLeft > 0)
                return;
            
            
            stateMachine.ChangeState(new Attacking(stateMachine));
        }

        public override void Exit()
        {
            isWaiting = false;
        }
    }
    
    public class Moving : State
    {
        public Moving(StateMachine stateMachine) : base(stateMachine){ }

        public override void Enter()
        {

        }

        public override void UpdateLogic()
        {
            if (stateMachine.spiderPhase1Script.MovingToWaitingPoint())
                return;
            
            stateMachine.ChangeState(new Attacking(stateMachine));
        }

        public override void Exit()
        {
            
        }
    }

    public class Attacking : State
    {
        private MethodInfo attackMethod;
        private bool isChasing = false;
        private bool isChasingFromTheLeft = false;
        private bool isAttacking = false;
        
        public Attacking(StateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            isChasing = false;
            isAttacking = false;
            var chosenAttack = ChooseAttack();
            attackMethod = GetType().GetMethod(chosenAttack.ToString());
        }
        
        public override void UpdateLogic()
        {
            attackMethod?.Invoke(this, null);
        }

        public override void Exit()
        {
            attackMethod = null;
            isChasing = false;
            isAttacking = false;
        }

        private BossAttack ChooseAttack()
        {
            var values = Enum.GetValues(typeof(BossAttack));
            var chosenAttack = (BossAttack)Random.Range(0, values.Length);

            return chosenAttack;
        }

        #region AttackMethods

            public void PawStrike()
            {
                if (!isChasing && !isAttacking)
                {
                    isChasingFromTheLeft = Random.value < 0.5f;
                    
                    isChasing = true;
                    stateMachine.animator.Play("Chasing");
                    stateMachine.spiderPhase1Script.Appear(isChasingFromTheLeft);
                    return;
                }
                
                // if (isChasing)
                    //stateMachine.bossObject.transform.position = scr_Player.instance.transform.position + new Vector3(2.5f * Mathf.Pow(-1,Convert.ToInt32(isChasingFromTheLeft)) ,1.5f,0);
                
                var isAnimatingChasing = stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName("Chasing");
                if (isAnimatingChasing)
                    return;
                
                
                if (!isAttacking)
                {
                    isAttacking = true;
                    isChasing = false;
                    
                    if (isChasingFromTheLeft)
                        stateMachine.animator.Play("RightPawStrike");
                    else
                        stateMachine.animator.Play("LeftPawStrike");
                    
                    return;
                }


                var isAnimatingAttack = true;
                if (isChasingFromTheLeft)
                    isAnimatingAttack = stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName("RightPawStrike");
                else
                    isAnimatingAttack = stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName("LeftPawStrike");
                if (isAnimatingAttack)
                    return;
                
                    
                stateMachine.ChangeState(new Waiting(stateMachine));
            }
        
            public void BatteringRam()
            {
                if (!isChasing && !isAttacking)
                {
                    isChasingFromTheLeft = Random.value < 0.5f;
                    isChasingFromTheLeft = true;
                    
                    isChasing = true;
                    stateMachine.animator.Play("Chasing");
                    stateMachine.spiderPhase1Script.Appear(isChasingFromTheLeft);
                    return;
                }
                
                // if (isChasing)
                //     stateMachine.bossObject.transform.position = scr_Player.instance.transform.position + new Vector3(2.5f * Mathf.Pow(-1,Convert.ToInt32(isChasingFromTheLeft)) ,1.5f,0);
                
                var isAnimatingChasing = stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName("Chasing");
                if (isAnimatingChasing)
                    return;
                
                
                if (!isAttacking)
                {
                    isAttacking = true;
                    isChasing = false;
                    
                    if (isChasingFromTheLeft)
                        stateMachine.animator.Play("RightBatteringRamStrike");
                    else
                        stateMachine.animator.Play("LeftBatteringRamStrike");
                    
                    return;
                }


                var isAnimatingAttack = true;
                if (isChasingFromTheLeft)
                    isAnimatingAttack = stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName("RightBatteringRamStrike");
                else
                    isAnimatingAttack = stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName("LeftBatteringRamStrike");
                if (isAnimatingAttack)
                    return;
                
                    
                stateMachine.ChangeState(new Moving(stateMachine));
            }
            
            public void UnderGroundPawStrike()
            {

                
                if (!isAttacking)
                {
                    isAttacking = true;
                    stateMachine.animator.Play("Bite");
                    return;
                }

                var isAnimating = stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName("Bite");
                if (isAnimating)
                    return;

                stateMachine.ChangeState(new Waiting(stateMachine));
            }

            public void Bite()
            {

                
                if (!isAttacking)
                {
                    isAttacking = true;
                    stateMachine.animator.Play("Bite");
                    return;
                }

                var isAnimating = stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName("Bite");
                if (isAnimating)
                    return;

                stateMachine.ChangeState(new Waiting(stateMachine));
            }
            
            // private void WebShot()
            // {
            //     stateMachine.animator.Play("WebShot");
            //     //var playerPosition = scr_Player.instance.gameObject.transform.position;
            //
            //
            //     //получаем позицию игрока
            //     //смотрим на кд, если можем, стреляем
            //     //когда стреляем, вызываем анимацию отталкивания отдачей и спавним снаряд
            //     
            //     
            //     //в коцне атаки
            //     stateMachine.ChangeState(new Attacking(stateMachine));
            // }
        

        #endregion

        
    }
    
    public class Stunned : State
    {
        private bool isStunned = false;
        
        public Stunned(StateMachine stateMachine) : base(stateMachine) { }

        public override void UpdateLogic()
        {
            if (!isStunned)
            {
                isStunned = true;
                stateMachine.animator.Play("Stunned");
                return;
            }

            var isAnimating = stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName("Stunned");
            if (isAnimating)
                return;
            
            stateMachine.ChangeState(new Attacking(stateMachine));
        }

        public override void Hit()
        {
            stateMachine.spiderPhase1Script.TakeDamage();
            //TakeDamage animation? or state?
            stateMachine.ChangeState(new SuperAttacking(stateMachine));
        }
    }

    public class SuperAttacking : State
    {
        private MethodInfo superAttackMethod;
        private bool isSuperAttacking = false;
        
        public SuperAttacking(StateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            isSuperAttacking = false;
            var chosenSuperAttack = ChooseSuperAttack();
            superAttackMethod = GetType().GetMethod(chosenSuperAttack.ToString());
            
            //start prepare animation?
            
            // var chosenAttack = ChooseAttack();
            // attackMethod = GetType().GetMethod(chosenAttack.ToString());
            
        }
        
        public override void UpdateLogic()
        {
            //prepare animation is still playing? -> return

            superAttackMethod?.Invoke(this, null);
        }

        public override void Exit()
        {
            superAttackMethod = null;
            isSuperAttacking = false;
        }

        // private BossAttack ChooseAttack()
        // {
        //     var values = Enum.GetValues(typeof(BossAttack));
        //     return (BossAttack)Random.Range(0, values.Length);
        // }

        private BossAttack ChooseSuperAttack()
        {
            //var values = Enum.GetValues(typeof(BossAttack));
            //var chosenAttack = (BossAttack)Random.Range(0, values.Length);

            //return chosenAttack;
            return BossAttack.PawStrike; // temp!
        }

        #region SuperAttackMethods

            private void SuperShot()
            {
                //var playerPosition = scr_Player.instance.gameObject.transform.position;


                //получаем позицию игрока
                //смотрим на кд, если можем, стреляем
                //когда стреляем, вызываем анимацию отталкивания отдачей и спавним снаряд
            
            
                if (!isSuperAttacking)
                {
                    isSuperAttacking = true;
                    stateMachine.animator.Play("SuperAttacking");
                    return;
                }
                
                var isAnimating = stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName("SuperAttacking");
                if (isAnimating)
                    return;
                
                //в коцне атаки
                stateMachine.ChangeState(new Attacking(stateMachine));
            }

        #endregion
    }
    
    public enum BossAttack
    {
        PawStrike,
        BatteringRam,
        //UndergroundPawStrike,
        //Bite,
        //WebShot,
        
        //SuperShot,
    }
}

