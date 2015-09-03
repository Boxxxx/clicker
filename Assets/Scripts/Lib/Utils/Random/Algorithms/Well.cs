/*
 * Copyright (C) Rei HOBARA 2007
 * 
 * Name:
 *     Well.cs
 * Class:
 *     Rei.Random.Well
 * Purpose:
 *     A random number generator using Well.
 * Remark:
 *     This code is C# implementation of Well.
 *     Well was introduced by Francois panneton, Pierre L'ecuyer and Makoto Matsumoto.
 *     See http://www.iro.umontreal.ca/~lecuyer/myftp/papers/wellrng.pdf for detail of Well.
 * History:
 *     2007/10/6 initial release.
 * 
 */

using System;

namespace Rei.Random {

    /// <summary>
    /// Well�̋[�������W�F�l���[�^�[�N���X�B
    /// </summary>
    public class Well : RandomBase {

        #region Field

        /// <summary>
        /// Well�̃p�����[�^�[�̈�B
        /// </summary>
        protected const int W = 32;
        /// <summary>
        /// ������ԃx�N�g���̌��B
        /// </summary>
        protected const int R = 624;
        /// <summary>
        /// Well�̃p�����[�^�[�̈�B
        /// </summary>
        protected const int P = 31;
        /// <summary>
        /// Well�̃p�����[�^�[�̈�B
        /// </summary>
        protected const uint MASKU = (0xffffffffU >> (W - P));
        /// <summary>
        /// Well�̃p�����[�^�[�̈�B
        /// </summary>
        protected const uint MASKL = (~MASKU);
        /// <summary>
        /// Well�̃p�����[�^�[�̈�B
        /// </summary>
        protected const int M1 = 70;
        /// <summary>
        /// Well�̃p�����[�^�[�̈�B
        /// </summary>
        protected const int M2 = 179;
        /// <summary>
        /// Well�̃p�����[�^�[�̈�B
        /// </summary>
        protected const int M3 = 449;
        /// <summary>
        /// Well�̃p�����[�^�[�̈�B
        /// </summary>
        protected const uint TEMPERB = 0x12345678U;
        /// <summary>
        /// Well�̃p�����[�^�[�̈�B
        /// </summary>
        protected const uint TEMPERC = 0x87654321U;

        /// <summary>
        /// ������ԃx�N�g���B
        /// </summary>
        protected UInt32[] state;
        /// <summary>
        /// ������ԃx�N�g���̂����A���ɗ����Ƃ��Ďg�p����C���f�b�N�X�B
        /// </summary>
        protected int state_i;

        #endregion

        /// <summary>
        /// ���ݎ�������Ƃ����AWell�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public Well() : this(Environment.TickCount) { }

        /// <summary>
        /// seed����Ƃ����AWell�[�������W�F�l���[�^�[�����������܂��B
        /// </summary>
        public Well( int seed ) {
            state = new UInt32[R];
            state_i = 0;
            //������Ԕz�񏉊���
            //�X�^���_�[�h�ȏ������@��������Ȃ��̂łƂ肠����MT�Ɠ����������ɂ��Ă����B
            state[0] = (UInt32)seed;
            for (int i = 1; i < R; i++)
                state[i] = (UInt32)(1812433253 * (state[i - 1] ^ (state[i - 1] >> 30)) + i);
        }

        /// <summary>
        /// �����Ȃ�32bit�̋[���������擾���܂��B
        /// </summary>
        public override uint NextUInt32() {
            uint z1, z2;
            int i1, i2, i3, i4, i5;
            if (state_i < 2) {
                if (state_i < 1) i2 = state_i - 1 + R; else i2 = state_i - 1;
                i1 = state_i - 2 + R;
            } else {
                i1 = state_i - 2;
                i2 = state_i - 1;
            }
            if (state_i + M3 >= R) {
                if (state_i + M2 >= R) {
                    if (state_i + M1 >= R) i3 = state_i + M1 - R; else i3 = state_i + M1;
                    i4 = state_i + M2 - R;
                } else {
                    i3 = state_i + M1;
                    i4 = state_i + M2;
                }
                i5 = state_i + M3 - R;
            } else {
                i3 = state_i + M1;
                i4 = state_i + M2;
                i5 = state_i + M3;
            }
            z1 = (state[state_i] ^ (state[state_i] << 25)) ^ (state[i3] ^ (state[i3] >> 27));
            z2 = (state[i4] >> 9) ^ (state[i5] ^ (state[i5] >> 1));
            state[state_i] = z1 ^ z2;
            z1 = (state[i2] & MASKL) ^ (state[i1] & MASKU) ^ (z1 << 9) ^ (z2 << 21) ^ (state[state_i] >> 21);
            state_i = i2;
            state[i2] = z1;
            z1 = z1 ^ ((z1 << 7) & TEMPERB);
            z1 = z1 ^ ((z1 << 15) & TEMPERC);
            return z1;
            //�ȉ��A����B
            //uint z0, z1, z2;
            //uint y;
            //if (state_i == 0) {
            //    z0 = (state[state_i + R - 1] & MASKL) | (state[state_i + R - 2] & MASKU);
            //    z1 = (state[state_i] ^ (state[state_i] << (25))) ^ (state[state_i + M1] ^ (state[state_i + M1] >> 27));
            //    z2 = (state[state_i + M2] >> 9) ^ (state[state_i + M3] ^ (state[state_i + M3] >> 1));
            //    state[state_i] = z1 ^ z2;
            //    state[state_i - 1 + R] = (z0) ^ (z1 ^ (z1 << (9))) ^ (z2 ^ (z2 << (21))) ^ (state[state_i] ^ (state[state_i] >> 21));
            //} else if (state_i == 1) {
            //    z0 = (state[state_i - 1] & MASKL) | (state[state_i + R - 2] & MASKU);
            //    z1 = (state[state_i] ^ (state[state_i] << (25))) ^ (state[state_i + M1] ^ (state[state_i + M1] >> 27));
            //    z2 = (state[state_i + M2] >> 9) ^ (state[state_i + M3] ^ (state[state_i + M3] >> 1));
            //    state[state_i] = z1 ^ z2;
            //    state[state_i - 1] = (z0) ^ (z1 ^ (z1 << (9))) ^ (z2 ^ (z2 << (21))) ^ (state[state_i] ^ (state[state_i] >> 21));
            //} else if (state_i + M1 >= R) {
            //    z0 = (state[state_i - 1] & MASKL) | (state[state_i - 2] & MASKU);
            //    z1 = (state[state_i] ^ (state[state_i] << (25))) ^ (state[state_i + M1 - R] ^ (state[state_i + M1 - R] >> 27));
            //    z2 = (state[state_i + M2 - R] >> 9) ^ (state[state_i + M3 - R] ^ (state[state_i + M3 - R] >> 1));
            //    state[state_i] = z1 ^ z2;
            //    state[state_i - 1] = (z0) ^ (z1 ^ (z1 << (9))) ^ (z2 ^ (z2 << (21))) ^ (state[state_i] ^ (state[state_i] >> 21));
            //} else if (state_i + M2 >= R) {
            //    z0 = (state[state_i - 1] & MASKL) | (state[state_i - 2] & MASKU);
            //    z1 = (state[state_i] ^ (state[state_i] << (25))) ^ (state[state_i + M1] ^ (state[state_i + M1] >> 27));
            //    z2 = (state[state_i + M2 - R] >> 9) ^ (state[state_i + M3 - R] ^ (state[state_i + M3 - R] >> 1));
            //    state[state_i] = z1 ^ z2;
            //    state[state_i - 1] = (z0) ^ (z1 ^ (z1 << (9))) ^ (z2 ^ (z2 << (21))) ^ (state[state_i] ^ (state[state_i] >> 21));
            //} else if (state_i + M3 >= R) {
            //    z0 = (state[state_i - 1] & MASKL) | (state[state_i - 2] & MASKU);
            //    z1 = (state[state_i] ^ (state[state_i] << (25))) ^ (state[state_i + M1] ^ (state[state_i + M1] >> 27));
            //    z2 = (state[state_i + M2] >> 9) ^ (state[state_i + M3 - R] ^ (state[state_i + M3 - R] >> 1));
            //    state[state_i] = z1 ^ z2;
            //    state[state_i - 1] = (z0) ^ (z1 ^ (z1 << (9))) ^ (z2 ^ (z2 << (21))) ^ (state[state_i] ^ (state[state_i] >> 21));
            //} else {
            //    z0 = (state[state_i - 1] & MASKL) | (state[state_i - 2] & MASKU);
            //    z1 = (state[state_i] ^ (state[state_i] << (25))) ^ (state[state_i + M1] ^ (state[state_i + M1] >> 27));
            //    z2 = (state[state_i + M2] >> 9) ^ (state[state_i + M3] ^ (state[state_i + M3] >> 1));
            //    state[state_i] = z1 ^ z2;
            //    state[state_i - 1] = (z0) ^ (z1 ^ (z1 << (9))) ^ (z2 ^ (z2 << (21))) ^ (state[state_i] ^ (state[state_i] >> 21));
            //}
            //state_i--;
            //if (state_i == -1) state_i = R - 1;
            //y = state[state_i] ^ ((state[state_i] << 7) & TEMPERB);
            //y = y ^ ((y << 15) & TEMPERC);
            //return y;
        }

    }

}
