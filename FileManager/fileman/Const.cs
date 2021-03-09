using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileman
{
    public static class Const
    {
        public static double scale = 0.6;
        public static int fieldWidth;
        public static int fieldHeight;
        public static int promptPosition;
        public static int messPosition;
        public static int top = 1;
        public static int left = 1;
        public static int maxFileNameLen;
        public static int timeStringLen = 22;
        public static int sizeStringLen = 11;
        public static int attrStringLen = 10;

        public static int GetMaxFileNameLen(int mode)
        {
            int len = Const.fieldWidth - 5;
            if (mode > 0) len -= Const.sizeStringLen;
            if (mode > 1) len -= Const.attrStringLen;
            if (mode > 2) len -= Const.timeStringLen;
            if (mode > 3) len -= Const.timeStringLen;
            if (mode > 4) len -= Const.timeStringLen;
            return len;
        }

    }
}
