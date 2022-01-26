using System;

namespace fileman2
{
    internal static class FMConstants
    {
        public const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

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
        public static byte numOfViewMode = 5;

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
