using System;

namespace Elzik.Mecon.Service.Domain
{
    public sealed class EntryKey : IEquatable<EntryKey>
    {
        public EntryKey(string filename, long byteCount)
        {
            Filename = filename;
            ByteCount = byteCount;
        }

        public string Filename { get; }
        public long ByteCount { get; }
        
        public bool Equals(EntryKey other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Filename == other.Filename && ByteCount == other.ByteCount;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EntryKey) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Filename, ByteCount);
        }

        public static bool operator ==(EntryKey left, EntryKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntryKey left, EntryKey right)
        {
            return !Equals(left, right);
        }
    }
}
