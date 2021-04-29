
using Sarachan.UniTiya.BehaviourTree;
using Sarachan.UniTiya.Commands;

namespace Sarachan.UniTiya.TiyaActor.AI
{
    /// <summary>
    /// Actor 的武器行为（攻击）节点
    /// </summary>
    public class ActorAttackNode : ActorActionNodeBase
    {
        /// <summary>
        /// 攻击 Perform 到 Cancel 的时间间隔
        /// </summary>
        public float AttackSustainTimeSpan
        {
            get => _sustainedNode.TimeSpan;
            set => _sustainedNode.TimeSpan = value;
        }

        AttackType _attackType;

        /// <summary>
        /// 攻击的类型
        /// </summary>
        public AttackType AttackType
        {
            get => _attackType;
            set
            {
                if (IsAttacking)
                {
                    AttackCancelListener();
                }
                _attackType = value;
            }
        }

        /// <summary>
        /// 当前是否处于攻击状态
        /// </summary>
        public bool IsAttacking { get; private set; }

        readonly SequenceNode _actualRootNode;
        readonly TimerNode _sustainedNode;

        public ActorAttackNode(AttackType attackType, float attackSustainTimeSpan)
        {
            AttackType = attackType;

            _actualRootNode = new SequenceNode();

            _actualRootNode.AddChildNode(new SimpleActionNode(AttackPerformListener));
            _actualRootNode.AddChildNode(_sustainedNode = new TimerNode(attackSustainTimeSpan));
            _actualRootNode.AddChildNode(new SimpleActionNode(AttackCancelListener));
        }

        void AttackPerformListener()
        {
            IsAttacking = true;
            if ((AttackType & AttackType.NormalAttack) != 0)
            {
                Actor.CommandProcessor.AddCommand(ActorCommands.NormalAttack(SkillCmdType.PerformSkill));
            }
            else if ((AttackType & AttackType.SpecialAttack) != 0)
            {
                Actor.CommandProcessor.AddCommand(ActorCommands.SpecialAttack(SkillCmdType.PerformSkill));
            }
            else
            {
                var extraSkillIndex = AttackType.ToExtraSkillIndex();
                Actor.CommandProcessor.AddCommand(ActorCommands.WeaponExtraSkill(extraSkillIndex, SkillCmdType.PerformSkill));
            }
        }
        void AttackCancelListener()
        {
            IsAttacking = false;
            if ((AttackType & AttackType.NormalAttack) != 0)
            {
                Actor.CommandProcessor.AddCommand(ActorCommands.NormalAttack(SkillCmdType.CancelSkill));
            }
            else if ((AttackType & AttackType.SpecialAttack) != 0)
            {
                Actor.CommandProcessor.AddCommand(ActorCommands.SpecialAttack(SkillCmdType.CancelSkill));
            }
            else
            {
                var extraSkillIndex = AttackType.ToExtraSkillIndex();
                Actor.CommandProcessor.AddCommand(ActorCommands.WeaponExtraSkill(extraSkillIndex, SkillCmdType.CancelSkill));
            }
        }

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            return _actualRootNode.Update();
        }

        protected internal override void Init(TiyaBehaviourTree behaviourTree)
        {
            base.Init(behaviourTree);

            foreach (var node in _actualRootNode.ChildNodes)
            {
                node.Init(behaviourTree);
            }
        }
    }
}
