using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileman
{
    public static class FMConstants
    {
        public static ConsoleColor backColor;
        public static double scale = 0.7;
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
        public static int numOfDrivesParameters = 7;

        public static string reportConfigFileName = "reportconfig.txt";

        public static int nName = 0;
        public static int nType = 1;
        public static int nFormat = 2;
        public static int nLabel = 3;
        public static int nTotal = 4;
        public static int nFree = 5;
        public static int nAvailable = 6;

        public static int GetMaxFileNameLen(int mode)
        {
            int len = FMConstants.fieldWidth - 5;
            if (mode > 0) len -= FMConstants.sizeStringLen;
            if (mode > 1) len -= FMConstants.attrStringLen;
            if (mode > 2) len -= FMConstants.timeStringLen;
            if (mode > 3) len -= FMConstants.timeStringLen;
            if (mode > 4) len -= FMConstants.timeStringLen;
            return len;
        }

    }
}
