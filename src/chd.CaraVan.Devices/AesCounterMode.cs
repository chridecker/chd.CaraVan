using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices
{
    public class AesCounterMode : SymmetricAlgorithm
{
    private readonly ulong _nonce;
    private readonly ulong _counter;
    private readonly AesManaged _aes;

    public AesCounterMode(byte[] nonce, ulong counter)
      : this(ConvertNonce(nonce), counter)
    {
    }

    public AesCounterMode(ulong nonce, ulong counter)
    {
        _aes = new AesManaged
        {
            Mode = CipherMode.ECB,
            Padding = PaddingMode.None
        };

        _nonce = nonce;
        _counter = counter;
    }

    private static ulong ConvertNonce(byte[] nonce)
    {
        if (nonce == null) throw new ArgumentNullException(nameof(nonce));
        if (nonce.Length < sizeof(ulong)) throw new ArgumentException($"{nameof(nonce)} must have at least {sizeof(ulong)} bytes");

        return BitConverter.ToUInt64(nonce);
    }

    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] ignoredParameter)
    {
        return new CounterModeCryptoTransform(_aes, rgbKey, _nonce, _counter);
    }

    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] ignoredParameter)
    {
        return new CounterModeCryptoTransform(_aes, rgbKey, _nonce, _counter);
    }

    public override void GenerateKey()
    {
        _aes.GenerateKey();
    }

    public override void GenerateIV()
    {
        // IV not needed in Counter Mode
    }
}

}
