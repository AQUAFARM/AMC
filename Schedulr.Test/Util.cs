using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Schedulr.Test
{
    internal static class Util
    {
        public static string GetDummyFileName()
        {
            var dummy = string.Format(CultureInfo.CurrentCulture, @"Z:\{0}.dummy", Guid.NewGuid().ToString());
            if (File.Exists(dummy))
            {
                // This should obviously never happen.
                Debug.Fail("This is quite unexpected, a file with the following randomly generated GUID exists: " + dummy);
            }
            return dummy;
        }
    }
}