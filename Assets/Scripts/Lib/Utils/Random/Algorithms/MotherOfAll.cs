/*
 * Copyright (C) Rei HOBARA 2007
 * 
 * Name:
 *     MotherOfAll.cs
 * Class:
 *     Rei.Random.MotherOfAll
 * Purpose:
 *     A random number generator using Mother-of-All.
 * Remark:
 *     This code is C# implementation of Mother-of-All.
 *     Mother-of-All was introduced by George Marsaglia.
 *     See ftp://ftp.taygeta.com/pub/c/mother.c for detail of Mother-of-All.
 *     Parameters are derived from http://www.stat.fsu.edu/pub/diehard/ and http://www.agner.org/
 * History:
 *     2007/10/6 initial release.
 * 
 */

using System;

namespace Rei.Random {

    /// <summary>
    /// Mother-of-All�̋[�������W�F�l���[�^�[�N���X�B
    /// </summary>
    public class MotherOfAll : RandomBase {

        /// <summary>
        /// ������ԃx�N�g���B
        /// </summary>
        protected UInt32 x, y, z, w, v;

        /// <summary>
        /// ���ݎ�������Ƃ����AMother-Of-All�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public MotherOfAll() : this(Environment.TickCount) { }

        /// <summary>
        /// seed����Ƃ����AMother-Of-All�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public MotherOfAll( int seed ) {
            UInt32 s = (UInt32)seed;
            x = s = 29943829 * s - 1;
            y = s = 29943829 * s - 1;
            z = s = 29943829 * s - 1;
            w = s = 29943829 * s - 1;
            v = s = 29943829 * s - 1;
            for (int i = 0; i < 19; i++) NextUInt32();
        }

        /// <summary>
        /// �����Ȃ�32bit�̋[���������擾���܂��B
        /// </summary>
        public override uint NextUInt32() {
            UInt64 s = 2111111111UL * w + 1492UL * z + 1776UL * y + 5115UL * x + v;
            w = z; z = y; y = x; x = (UInt32)s;
            v = (UInt32)(s >> 32);
            return x;
        }
    }

}
