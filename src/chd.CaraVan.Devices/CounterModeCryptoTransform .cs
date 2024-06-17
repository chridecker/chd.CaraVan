using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class CounterModeCryptoTransform : ICryptoTransform
{
    private readonly byte[] _nonceAndCounter;
    private readonly ICryptoTransform _counterEncryptor;
    private readonly Queue<byte> _xorMask = new Queue<byte>();
    private readonly SymmetricAlgorithm _symmetricAlgorithm;

    private ulong _counter;

    public CounterModeCryptoTransform(SymmetricAlgorithm symmetricAlgorithm, byte[] key, ulong nonce, ulong counter)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        _symmetricAlgorithm = symmetricAlgorithm ?? throw new ArgumentNullException(nameof(symmetricAlgorithm));
        _counter = counter;
        _nonceAndCounter = new byte[16];
        BitConverter.TryWriteBytes(_nonceAndCounter, nonce);
        BitConverter.TryWriteBytes(new Span<byte>(_nonceAndCounter, sizeof(ulong), sizeof(ulong)), counter);

        var zeroIv = new byte[_symmetricAlgorithm.BlockSize / 8];
        _counterEncryptor = symmetricAlgorithm.CreateEncryptor(key, zeroIv);
    }

    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
    {
        var output = new byte[inputCount];
        TransformBlock(inputBuffer, inputOffset, inputCount, output, 0);
        return output;
    }

    public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer,
        int outputOffset)
    {
        for (var i = 0; i < inputCount; i++)
        {
            if (NeedMoreXorMaskBytes())
            {
                EncryptCounterThenIncrement();
            }

            var mask = _xorMask.Dequeue();
            outputBuffer[outputOffset + i] = (byte) (inputBuffer[inputOffset + i] ^ mask);
        }

        return inputCount;
    }

    private bool NeedMoreXorMaskBytes()
    {
        return _xorMask.Count == 0;
    }

    private byte[] _counterModeBlock;
    private void EncryptCounterThenIncrement()
    {
        _counterModeBlock ??= new byte[_symmetricAlgorithm.BlockSize / 8];

        _counterEncryptor.TransformBlock(_nonceAndCounter, 0, _nonceAndCounter.Length, _counterModeBlock, 0);
        IncrementCounter();

        foreach (var b in _counterModeBlock)
        {
            _xorMask.Enqueue(b);
        }
    }

    private void IncrementCounter()
    {
        _counter++;
        var span = new Span<byte>(_nonceAndCounter, sizeof(ulong), sizeof(ulong));
        BitConverter.TryWriteBytes(span, _counter);
    }

    public int InputBlockSize => _symmetricAlgorithm.BlockSize / 8;
    public int OutputBlockSize => _symmetricAlgorithm.BlockSize / 8;
    public bool CanTransformMultipleBlocks => true;
    public bool CanReuseTransform => false;

    public void Dispose()
    {
        _counterEncryptor.Dispose();
    }
}
}
