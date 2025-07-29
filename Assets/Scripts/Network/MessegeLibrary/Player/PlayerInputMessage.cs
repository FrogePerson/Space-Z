using Mirror;
using UnityEngine;

namespace MessageLibrary.Player
{
    public class PlayerInputMessage : NetworkMessage
    {
        public uint PlayerID;
        public byte inputFlags;
        public ushort camRot;

        public static byte Pack(bool w, bool a, bool s, bool d, bool jump)
        {
            byte flags = 0;

            if (w) flags    |= 1 << 0;
            if (a) flags    |= 1 << 0;
            if (s) flags    |= 1 << 0;
            if (d) flags    |= 1 << 0;
            if (jump) flags |= 1 << 0;

            return flags;
        }
    }
}
