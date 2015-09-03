/*
 * Copyright (C) Rei HOBARA 2007
 * 
 * Name:
 *     LCG.cs
 * Class:
 *     Rei.Random.LCG
 * Purpose:
 *     A random number generator using Linear Congruential Generator(LCG).
 * Remark:
 * History:
 *     2007/10/6 initial release.
 * 
 */

using System;

namespace Rei.Random {

    /// <summary>
    /// LCG�̋[�������W�F�l���[�^�[�N���X�B
    /// </summary>
    public class LCG : RandomBase {

        #region Field

        /// <summary>
        /// LCG�̃p�����[�^�[�̈�B
        /// </summary>
        protected UInt32 A;
        /// <summary>
        /// LCG�̃p�����[�^�[�̈�B
        /// </summary>
        protected UInt32 C;
        /// <summary>
        /// ������ԁB
        /// </summary>
        protected UInt32 x;

        #endregion

        /// <summary>
        /// ���ݎ�������Ƃ����ALCG�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public LCG() : this(Environment.TickCount) { }

        /// <summary>
        /// seed����Ƃ����ALCG�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public LCG( int seed ) : this(seed, 1664525, 1013904223) { }

        /// <summary>
        /// seed����Ƃ��AparamA��paramC�ŕ\�����LCG�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public LCG( int seed, UInt32 paramA, UInt32 paramC ) {
            A = paramA;
            C = paramC;
            x = (UInt32)seed;
        }

        /// <summary>
        /// �����Ȃ�32bit�̋[���������擾���܂��B
        /// </summary>
        public override uint NextUInt32() {
            x = (UInt32)((UInt64)x * A + C);
            return x;
        }

    }

}
