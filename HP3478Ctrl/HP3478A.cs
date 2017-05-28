using System;
using Ivi.Visa;
using System.Text.RegularExpressions;

namespace HP3478Ctrl
{
    class HP3478A {
        public static bool CalibrationsEqual(string a, string b) {
            if (a == null || b == null)
                return false;
            string aTrimmed = Regex.Replace(a, @"\s+", "");
            string bTrimmed = Regex.Replace(b, @"\s+", "");
            // Ignore the first byte (usually). It toggles back and forth
            if ((aTrimmed[0] == '@' || aTrimmed[0] == 'O') &&
                (bTrimmed[0] == '@' || bTrimmed[0] == 'O')) {
                    aTrimmed = aTrimmed[0] + bTrimmed.Substring(1);
            }
            if (aTrimmed.Length != 256 || bTrimmed.Length != 256)
                return false;
            return a == b;
        }
        public static bool IsValidCalString(string s) {
            if (s == null)
                return false;
            // Remove Whitespace
            s = Regex.Replace(s, @"\s+", "");
            if (s.Length != 256)
                return false;
            if (s[0] != '@' && s[0] != 'O')
                return false;
            foreach (char c in s) {
                int cint = (int)c;
                cint = cint - (int)'@';
                if (cint < 0 || cint >= 16)
                    return false;
            }
            return true;
        }
        public static string ReadCalibration(string addr) {
            string calStr = "";
            using (IGpibSession dev = GlobalResourceManager.Open(addr) as IGpibSession) {

                dev.Clear();
                //dev.RawIO.Write("H0");
                //dev.ReadStatusByte();
                //List<byte> vals = new List<byte>();
                for (int i = 0; i < 256; i++) {
                    dev.RawIO.Write(new byte[] { 0x57, (byte)i }); // 0x57 is 'W' in hex
                    calStr = calStr + dev.RawIO.ReadString();
                    if (i % 32 == 31)
                        calStr = calStr + System.Environment.NewLine;
                    /*byte[] x = dev.RawIO.Read();
                    x[0] = (byte)((int)x[0] - 64);
                    vals.Add(x[0]);*/
                }
            }
            return calStr;
        }
        public static void WriteCalibration(string addr, string calString) {
            calString = Regex.Replace(calString, @"\s+", "");
            if (!IsValidCalString(calString))
                throw new FormatException("Calibration string is not valid.");

            using (IGpibSession dev = GlobalResourceManager.Open(addr) as IGpibSession) {

                dev.Clear();
                //dev.RawIO.Write("H0");
                //dev.ReadStatusByte();
                //List<byte> vals = new List<byte>();
                for (int i = 0; i < 256; i++) {
                    dev.RawIO.Write(new byte[] { (byte)'X', (byte)i, (byte)calString[i]});
                }
            }
        }
    }
}
