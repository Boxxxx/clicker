using System;
using System.Collections;
using Rei.Random;

namespace Utils {
    public static class RandomFactory {

        public enum AlgorithmType {
            Default,
            LCG,
            MersenneTwister,
            MotherOfAll,
            RanrotB,
            SFMT,
            Well,
            Xorshift
        };


        public static Random Create(int seed, AlgorithmType algorithm = AlgorithmType.Default) {
            switch(algorithm) {
                case AlgorithmType.LCG:
                    return new CompatilizedRandom(new LCG(seed));
                case AlgorithmType.MersenneTwister:
                    return new CompatilizedRandom(new MersenneTwister(seed));
                case AlgorithmType.MotherOfAll:
                    return new CompatilizedRandom(new MotherOfAll(seed));
                case AlgorithmType.RanrotB:
                    return new CompatilizedRandom(new RanrotB(seed));
                case AlgorithmType.SFMT:
                    return new CompatilizedRandom(new SFMT(seed));
                case AlgorithmType.Well:
                    return new CompatilizedRandom(new Well(seed));
                case AlgorithmType.Xorshift:
                    return new CompatilizedRandom(new Xorshift(seed));
                default:
                    return new Random(seed);
            }
        }

        public static Random Create(AlgorithmType algorithm = AlgorithmType.Default) {
            switch(algorithm) {
                case AlgorithmType.LCG:
                    return new CompatilizedRandom(new LCG());
                case AlgorithmType.MersenneTwister:
                    return new CompatilizedRandom(new MersenneTwister());
                case AlgorithmType.MotherOfAll:
                    return new CompatilizedRandom(new MotherOfAll());
                case AlgorithmType.RanrotB:
                    return new CompatilizedRandom(new RanrotB());
                case AlgorithmType.SFMT:
                    return new CompatilizedRandom(new SFMT());
                case AlgorithmType.Well:
                    return new CompatilizedRandom(new Well());
                case AlgorithmType.Xorshift:
                    return new CompatilizedRandom(new Xorshift());
                default:
                    return new Random();
            }
        }

    }
}
