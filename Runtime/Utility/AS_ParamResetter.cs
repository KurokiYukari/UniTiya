using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya.Utility
{
    /// <summary>
    /// 适用于 Animator State 的参数重置器
    /// </summary>
    class AS_ParamResetter : StateMachineBehaviour
    {
        [System.Serializable]
        struct IntParamPair
        {
            [SerializeField] private string _name;
            public string Name => _name;

            [SerializeField] private int _defaultValue;
            public int DefaultValue => _defaultValue;
        }

        [System.Serializable]
        struct FloatParamPair
        {
            [SerializeField] private string _name;
            public string Name => _name;

            [SerializeField] private float _defaultValue;
            public float DefaultValue => _defaultValue;
        }

        [System.Serializable]
        struct BoolParamPair
        {
            [SerializeField] private string _name;
            public string Name => _name;

            [SerializeField] private bool _defaultValue;
            public bool DefaultValue => _defaultValue;
        }

        [Header("Exit Resetter")]
        [SerializeField] IntParamPair[] _intParams;
        [SerializeField] FloatParamPair[] _floatParams;
        [SerializeField] BoolParamPair[] _boolParams;

        // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
        //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}
        
        // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called before OnStateExit is called on any state inside this state machine
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var pair in _intParams)
            {
                animator.SetInteger(pair.Name, pair.DefaultValue);
            }
            foreach (var pair in _floatParams)
            {
                animator.SetFloat(pair.Name, pair.DefaultValue);
            }
            foreach (var pair in _boolParams)
            {
                animator.SetBool(pair.Name, pair.DefaultValue);
            }
        }

        // OnStateMove is called before OnStateMove is called on any state inside this state machine
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateIK is called before OnStateIK is called on any state inside this state machine
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMachineEnter is called when entering a state machine via its Entry Node
        //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        //{
        //    
        //}

        // OnStateMachineExit is called when exiting a state machine via its Exit Node
        //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        //{

        //}
    }
}
