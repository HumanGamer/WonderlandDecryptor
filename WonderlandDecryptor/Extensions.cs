using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderlandDecryptor
{
    public static class Extensions
    {
        public static bool Matches(this byte[] self, byte[] other, bool semi = false)
        {
            if (self.Length != other.Length && !semi)
                return false;

            int len = Math.Min(self.Length, other.Length);

            for (int i = 0; i < len; i++)
            {
                if (self[i] != other[i])
                    return false;
            }

            return true;
        }
    }
}
