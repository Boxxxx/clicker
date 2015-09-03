/*
 * Copyright (C) Rei HOBARA 2007
 * 
 * Name:
 *     MersenneTwister.cs
 * Class:
 *     Rei.Random.MersenneTwister
 * Purpose:
 *     A random number generator using Mersenne Twister.
 * Remark:
 *     This code is C# implementation of Mersenne Twister.
 *     Mersenne Twister was introduced by Takuji Nishimura and Makoto Matsumoto.
 *     See http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/mt.html for detail of Mersenne Twister.
 * History:
 *     2007/10/6 initial release.
 * 
 */

using System;

namespace Rei.Random {

    /// <summary>
    /// MersenneTwister�̋[�������W�F�l���[�^�[�N���X�B
    /// </summary>
    public class MersenneTwister : RandomBase {

        #region Field

        /// <summary>
        /// ������ԃx�N�g������
        /// </summary>
        protected const int N = 624;
        /// <summary>
        /// MT�����肷��p�����[�^�[�̈�B
        /// </summary>
        protected const int M = 397;
        /// <summary>
        /// MT�����肷��p�����[�^�[�̈�B
        /// </summary>
        protected const uint MATRIX_A = 0x9908b0dfU;
        /// <summary>
        /// MT�����肷��p�����[�^�[�̈�B
        /// </summary>
        protected const uint UPPER_MASK = 0x80000000U;
        /// <summary>
        /// MT�����肷��p�����[�^�[�̈�B
        /// </summary>
        protected const uint LOWER_MASK = 0x7fffffffU;
        /// <summary>
        /// MT�����肷��p�����[�^�[�̈�B
        /// </summary>
        protected const uint TEMPER1 = 0x9d2c5680U;
        /// <summary>
        /// MT�����肷��p�����[�^�[�̈�B
        /// </summary>
        protected const uint TEMPER2 = 0xefc60000U;
        /// <summary>
        /// MT�����肷��p�����[�^�[�̈�B
        /// </summary>
        protected const int TEMPER3 = 11;
        /// <summary>
        /// MT�����肷��p�����[�^�[�̈�B
        /// </summary>
        protected const int TEMPER4 = 7;
        /// <summary>
        /// MT�����肷��p�����[�^�[�̈�B
        /// </summary>
        protected const int TEMPER5 = 15;
        /// <summary>
        /// MT�����肷��p�����[�^�[�̈�B
        /// </summary>
        protected const int TEMPER6 = 18;

        /// <summary>
        /// ������ԃx�N�g���B
        /// </summary>
        protected UInt32[] mt;
        /// <summary>
        /// ������ԃx�N�g���̂����A���ɗ����Ƃ��Ďg�p����C���f�b�N�X�B
        /// </summary>
        protected int mti;
        private UInt32[] mag01;

        #endregion

        /// <summary>
        /// ���ݎ�������Ƃ����AMersenneTwister�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public MersenneTwister() : this(Environment.TickCount) { }

        /// <summary>
        /// seed����Ƃ����AMersenneTwister�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public MersenneTwister( int seed ) {
            mt = new UInt32[N];
            mti = N + 1;
            mag01 = new UInt32[] { 0x0U, MATRIX_A };
            //������Ԕz�񏉊���
            mt[0] = (UInt32)seed;
            for (int i = 1; i < N; i++)
                mt[i] = (UInt32)(1812433253 * (mt[i - 1] ^ (mt[i - 1] >> 30)) + i);
        }

        /// <summary>
        /// �����Ȃ�32bit�̋[���������擾���܂��B
        /// </summary>
        public override uint NextUInt32() {
            UInt32 y;
            if (mti >= N) { gen_rand_all(); mti = 0; }
            y = mt[mti++];
            y ^= (y >> TEMPER3);
            y ^= (y << TEMPER4) & TEMPER1;
            y ^= (y << TEMPER5) & TEMPER2;
            y ^= (y >> TEMPER6);
            return y;
        }

        /// <summary>
        /// ������ԃx�N�g�����X�V���܂��B
        /// </summary>
        protected void gen_rand_all() {
            int kk = 1;
            UInt32 y;
            UInt32 p;
            y = mt[0] & UPPER_MASK;
            do {
                p = mt[kk];
                mt[kk - 1] = mt[kk + (M - 1)] ^ ((y | (p & LOWER_MASK)) >> 1) ^ mag01[p & 1];
                y = p & UPPER_MASK;
            } while (++kk < N - M + 1);
            do {
                p = mt[kk];
                mt[kk - 1] = mt[kk + (M - N - 1)] ^ ((y | (p & LOWER_MASK)) >> 1) ^ mag01[p & 1];
                y = p & UPPER_MASK;
            } while (++kk < N);
            p = mt[0];
            mt[N - 1] = mt[M - 1] ^ ((y | (p & LOWER_MASK)) >> 1) ^ mag01[p & 1];
        }

    }
}
