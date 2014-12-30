using System;
using System.Security.Cryptography;

namespace Assets.Scripts
{
	public class CryptoRandom
    {
        private const int BufferSize = 1024;
        private readonly byte[] _randomBuffer;
        private int _bufferOffset;
        private readonly RNGCryptoServiceProvider _rngCryptoServiceProvider;

        public CryptoRandom()
        {
            _randomBuffer = new byte[BufferSize];
            _rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            _bufferOffset = _randomBuffer.Length;
        }

        private void FillBuffer()
        {
            _rngCryptoServiceProvider.GetBytes(_randomBuffer);
            _bufferOffset = 0;
        }

        public int Next()
        {
            if (_bufferOffset >= _randomBuffer.Length)
            {
                FillBuffer();
            }

            var val = BitConverter.ToInt32(_randomBuffer, _bufferOffset) & 0x7fffffff;

            _bufferOffset += sizeof(int);

            return val;
        }

        public int Next(int maxValue)
        {
            return Next() % maxValue;
        }

        public int Next(int minValue, int maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            var range = maxValue - minValue;

            return minValue + Next(range);
        }

	    public float NextFloat()
        {
            var val = Next();

            return (float) val / int.MaxValue;
        }

        public void GetBytes(byte[] buff)
        {
            _rngCryptoServiceProvider.GetBytes(buff);
        }
    }
}
