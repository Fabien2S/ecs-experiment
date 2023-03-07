using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace EcsExp;

public readonly struct Identifier
{
    private const int GuidByteCount = 16;

    private readonly Guid _guid;

    private Identifier(Guid guid)
    {
        _guid = guid;
    }

    public bool IsValid()
    {
        return _guid != default;
    }

    public override bool Equals(object? obj)
    {
        return obj is Identifier other && other._guid.Equals(_guid);
    }

    public override int GetHashCode()
    {
        return _guid.GetHashCode();
    }

    public override string ToString()
    {
        return _guid.ToString("D", CultureInfo.InvariantCulture);
    }

    public static bool IsPathAllowed(ReadOnlySpan<char> path)
    {
        var newBlock = true;

        foreach (var c in path)
        {
            // allowed characters
            if (char.IsAsciiLetterOrDigit(c) || c == '_')
            {
                newBlock = false;
                continue;
            }

            // allowed path when not preceded by a path
            if (!newBlock && c == '/')
            {
                newBlock = true;
                continue;
            }

            // invalid character
            return false;
        }

        return newBlock == false;
    }

    public static bool TryCreate(ReadOnlySpan<char> path, out Identifier identifier)
    {
        if (!IsPathAllowed(path))
        {
            identifier = default;
            return false;
        }

        var arrayPool = ArrayPool<byte>.Shared;
        var encoding = Encoding.UTF8;

        // array = hash(bytes) + path(utf8)
        var pathByteCount = encoding.GetByteCount(path);
        var arrayLength = SHA1.HashSizeInBytes + pathByteCount;
        var arrayBuffer = arrayPool.Rent(arrayLength);
        try
        {
            var buffer = arrayBuffer.AsSpan(0, arrayLength);

            const int hashOffset = 0;
            const int pathOffset = hashOffset + SHA1.HashSizeInBytes;

            // encode path as utf8 bytes
            {
                var pathDestination = buffer[pathOffset..];
                var encodedByteCount = encoding.GetBytes(path, pathDestination);
                Debug.Assert(pathByteCount == encodedByteCount, "pathByteCount == encodedByteCount");
            }

            // hash encoded path using SHA1
            {
                var hashSpanSource = buffer[pathOffset..];
                var hashSpanDestination = buffer[hashOffset..pathOffset];
                if (!SHA1.TryHashData(hashSpanSource, hashSpanDestination, out var writtenBytes))
                {
                    identifier = default;
                    return false;
                }

                Debug.Assert(SHA1.HashSizeInBytes == writtenBytes, "SHA1.HashSizeInBytes == writtenBytes");
            }

            // fix guid version
            // TODO Check endian-ness
            {
                //set high-nibble to 5 to indicate type 5
                buffer[6] &= 0x0F;
                buffer[6] |= 0x50;

                //set upper two bits to "10"
                buffer[8] &= 0x3F;
                buffer[8] |= 0x80;
            }

            // create identifier with guid
            var guid = new Guid(buffer[..GuidByteCount]);
            identifier = new Identifier(guid);
            return true;
        }
        finally
        {
            arrayPool.Return(arrayBuffer);
        }
    }

    public static Identifier Create(ReadOnlySpan<char> path)
    {
        return TryCreate(path, out var i) ? i : throw new ArgumentException("Invalid path", nameof(path));
    }

    public static Identifier Random()
    {
        var guid = Guid.NewGuid();
        return new Identifier(guid);
    }

    public static bool operator ==(Identifier x, Identifier y) => x.Equals(y);

    public static bool operator !=(Identifier x, Identifier y) => !x.Equals(y);
}