using System;

namespace DashX360.Avatar.Core
{
    public enum AvatarBodyType : byte { Male = 0, Female = 1 }

    public sealed class AvatarDescription
    {
        public const int BufferSize = 1021;
        public byte[] Description { get; private set; }

        public AvatarBodyType BodyType => (AvatarBodyType)Description[0];
        public float Height => BitConverter.ToSingle(Description, 1);

        public AvatarDescription()
        {
            Description = new byte[BufferSize];
        }

        public static AvatarDescription CreateRandom()
        {
            var desc = new AvatarDescription();
            var rng = new Random();
            
            desc.Description[0] = (byte)rng.Next(0, 2); // Random body type
            Buffer.BlockCopy(BitConverter.GetBytes(1.0f + (float)rng.NextDouble() * 0.4f), 0, desc.Description, 1, 4);
            
            // In a full implementation, face morphs, hair colors, and clothing GUIDs 
            // would be written to their specific offsets here.
            return desc;
        }

        public static AvatarDescription CreateFromBuffer(byte[] buffer)
        {
            if (buffer.Length != BufferSize) 
                throw new ArgumentException($"Buffer must be exactly {BufferSize} bytes.");
            
            return new AvatarDescription { Description = buffer };
        }
    }
}
