/*
 * Copyright (C) Rei HOBARA 2007
 * 
 * Name:
 *     Well.cs
 * Class:
 *     Rei.Random.RanrotB
 * Purpose:
 *     A random number generator using RanrotB.
 * Remark:
 *     This code is C# implementation of RanrotB.
 *     Well was introduced by Agner Fog.
 *     See http://www.agner.org/random/theory/chaosran.pdf for detail of RanrotB.
 * History:
 *     2007/10/6 initial release.
 * 
 */

using System;

namespace Rei.Random {

    /// <summary>
    /// Ranrot�̋[�������W�F�l���[�^�[�N���X�B
    /// </summary>
    public class RanrotB : RandomBase {

        #region Field

        /// <summary>
        /// ������ԃx�N�g���̌��B
        /// </summary>
        protected const int KK = 17;
        /// <summary>
        /// RanrotB�̃p�����[�^�[�̈�B
        /// </summary>
        protected const int JJ = 10;
        /// <summary>
        /// RanrotB�̃p�����[�^�[�̈�B
        /// </summary>
        protected const int R1 = 13;
        /// <summary>
        /// RanrotB�̃p�����[�^�[�̈�B
        /// </summary>
        protected const int R2 = 9;
        /// <summary>
        /// ������ԃx�N�g���B
        /// </summary>
        protected UInt32[] randbuffer;
        /// <summary>
        /// �����O�o�b�t�@�̃C���f�b�N�X�B
        /// </summary>
        protected int p1, p2;

        #endregion

        /// <summary>
        /// ���ݎ�������Ƃ����AWell�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public RanrotB() : this(Environment.TickCount) { }

        /// <summary>
        /// seed����Ƃ����AWell�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public RanrotB( int seed ) {
            UInt32 s = (UInt32)seed;
            randbuffer = new UInt32[KK];
            for (int i = 0; i < KK; i++)
                randbuffer[i] = s = s * 2891336453 + 1;
            p1 = 0; p2 = JJ;
            for (int i = 0; i < 9; i++) NextUInt32();
        }

        /// <summary>
        /// �����Ȃ�32bit�̋[���������擾���܂��B
        /// </summary>
        public override uint NextUInt32() {
            UInt32 x;
            x = randbuffer[p1] = ((randbuffer[p2] << R1) | (randbuffer[p2] >> (32 - R1))) + ((randbuffer[p1] << R2) | (randbuffer[p1] >> (32 - R2)));
            if (--p1 < 0) p1 = KK - 1;
            if (--p2 < 0) p2 = KK - 1;
            return x;
        }

    }

}
