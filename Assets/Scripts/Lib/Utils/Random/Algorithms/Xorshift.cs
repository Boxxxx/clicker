/*
 * Copyright (C) Rei HOBARA 2007
 * 
 * Name:
 *     Xorshift.cs
 * Class:
 *     Rei.Random.Xorshift
 * Purpose:
 *     A random number generator using Xorshift.
 * Remark:
 *     This code is C# implementation of Xorshift.
 *     Xorshift was introduced by George Marsaglia.
 *     See http://www.jstatsoft.org/v08/i14/paper for detail of Xorshift.
 * History:
 *     2007/10/6 initial release.
 * 
 */

using System;

namespace Rei.Random {

    /// <summary>
    /// Xorshift�̋[�������W�F�l���[�^�[�N���X�B
    /// </summary>
    public class Xorshift : RandomBase {

        /// <summary>
        /// ������ԃx�N�g���B
        /// </summary>
        protected UInt32 x, y, z, w;

        /// <summary>
        /// ���ݎ�������Ƃ����AXorshift�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public Xorshift() : this(Environment.TickCount) { }

        //static unsigned long x=123456789,y=362436069,z=521288629,w=88675123;

        /// <summary>
        /// seed����Ƃ����AXorshift�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public Xorshift( int seed ) : this((UInt32)seed, 362436069, 521288629, 88675123) { }

        /// <summary>
        /// seed����Ƃ����AXorshift�[�������W�F�l���[�^�[�����������܂��B
        /// George Marsaglia�ɂ��I���W�i����seed1=123456789,seed2=362436069,seed3=521288629,seed4=88675123��p���Ă��܂��B
        /// </summary>
        /// <param name="seed1"></param>
        /// <param name="seed2"></param>
        /// <param name="seed3"></param>
        /// <param name="seed4"></param>
        public Xorshift( UInt32 seed1, UInt32 seed2, UInt32 seed3, UInt32 seed4 ) {
            x = seed1; y = seed2; z = seed3; w = seed4;
        }

        /// <summary>
        /// �����Ȃ�32bit�̋[���������擾���܂��B
        /// </summary>
        public override uint NextUInt32() {
            UInt32 t;
            t = (x ^ (x << 11));
            x = y; y = z; z = w;
            return (w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)));
        }
    }
}
