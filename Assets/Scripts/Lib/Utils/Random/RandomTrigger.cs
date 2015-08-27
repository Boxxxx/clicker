using System;
using System.Collections;

namespace Utils
{
    public class RandomTrigger
    {
        protected Random random;
        protected float possibility;

        public RandomTrigger(float possibility)
            : this(possibility, RandomFactory.Create()) { }
        public RandomTrigger(float possibility, Random random)
        {
            this.random = random;
            this.possibility = Maths.Clamp(possibility, 0.0f, 1.0f);
        }

        public virtual bool TryTrigger()
        {
            return random.NextFloat() <= possibility;
        }
        public virtual void ResetPossibility(float possibility)
        {
            this.possibility = possibility;
        }
    }

    public class PseudoRandomTrigger : RandomTrigger
    {
        private static readonly float[,] PARAMS_C = {
            { 0.00f, 0.00000f },
            { 0.05f, 0.00380f },
            { 0.10f, 0.01475f },
            { 0.15f, 0.03221f },
            { 0.20f, 0.05570f },
            { 0.25f, 0.08475f },
            { 0.30f, 0.11895f },
            { 0.35f, 0.14628f },
            { 0.40f, 0.18128f },
            { 0.45f, 0.21867f },
            { 0.50f, 0.25701f },
            { 0.55f, 0.29509f },
            { 0.60f, 0.33324f },
            { 0.65f, 0.38109f },
            { 0.70f, 0.42448f },
            { 0.75f, 0.46134f },
            { 0.80f, 0.50276f },
            { 0.85f, 0.57910f },
            { 0.90f, 0.67068f },
            { 0.95f, 0.77041f },
            { 1.00f, 1.00000f }
        };
        private float _c;
        private float _noTriggerCnt;

        public PseudoRandomTrigger(float possibility)
            : this(possibility, RandomFactory.Create()) { }
        public PseudoRandomTrigger(float possibility, Random random)
            : base(possibility, random)
        {

            for (int i = 0; i < PARAMS_C.GetLength(0); i++)
            {
                if (Maths.GreaterOrEqual(PARAMS_C[i, 0], this.possibility))
                {
                    _c = PARAMS_C[i, 1];
                    break;
                }
            }
            _noTriggerCnt = 1;
        }

        public override void ResetPossibility(float possibility)
        {
            base.ResetPossibility(possibility);
            float c = PARAMS_C[0, 1];
            for (int i = 0; i < PARAMS_C.GetLength(0); i++)
            {
                if (Maths.GreaterOrEqual(PARAMS_C[i, 0], this.possibility))
                {
                    c = PARAMS_C[i, 1];
                    break;
                }
            }

            if (!Maths.Equals(c, _c))
            {
                // Set new noTriggerCnt, let the accumulated possibility is similar to previous one.
                _noTriggerCnt = Maths.Max(1.0f, Maths.IsZero(c) ? 1.0f : (int)Math.Round(_c * _noTriggerCnt / c));
                _c = c;
            }
        }

        public override bool TryTrigger()
        {
            if (random.NextFloat() <= _noTriggerCnt * _c)
            {
                _noTriggerCnt = 1;
                return true;
            }
            else
            {
                _noTriggerCnt++;
                return false;
            }
        }

        public bool TrySimpleTrigger()
        {
            return base.TryTrigger();
        }
    }

    public class AlwaysTrigger : RandomTrigger
    {
        private static AlwaysTrigger m_instance = null;
        public static AlwaysTrigger Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new AlwaysTrigger(1f);
                }
                return m_instance;
            }
        }

        private AlwaysTrigger(float possibility)
            : base(possibility, null) { }

        public override bool TryTrigger()
        {
            return true;
        }
    }
}