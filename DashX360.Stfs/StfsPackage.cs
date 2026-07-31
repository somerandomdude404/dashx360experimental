using System;
using System.IO;

namespace DashX360.Stfs
{
    public enum StfsMagic { CON, LIVE, PIRS }
    public enum StfsContentType { AvatarItem = 0x0009000, Profile = 0x0010000 }

    public sealed class StfsPackage
    {
        public StfsMagic Magic;
        public StfsContentType ContentType;
        public int TitleId;
        public Guid ProfileId;

        public static StfsPackage Open(string path)
        {
            using var fs = File.OpenRead(path);
            Span<byte> hdr = stackalloc byte[0x379];
            fs.Read(hdr);

            var pkg = new StfsPackage();
            string magicStr = System.Text.Encoding.ASCII.GetString(hdr.Slice(0, 4)).Trim();
            pkg.Magic = Enum.Parse<StfsMagic>(magicStr);
            
            pkg.ContentType = (StfsContentType)BitConverter.ToInt32(hdr.Slice(0x344));
            pkg.TitleId = BitConverter.ToInt32(hdr.Slice(0x360));
            pkg.ProfileId = new Guid(hdr.Slice(0x371, 8).ToArray());

            return pkg;
        }
    }
}
