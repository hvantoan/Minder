using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Minder.Service.Helpers {

    public static class EMailHelper {

        public static string GetOTPBody(string OTP) {
            if (!Directory.Exists("Resources")) Directory.CreateDirectory("Resources");
            string filename = $"Resources/OTPTemplate.html";
            string body = File.ReadAllText(filename);
            body = body.Replace("#OTP#", OTP);
            return body;
        }

        public static string GenarateOTP(List<string> otps) {
            string[] arr = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            var otp = new StringBuilder();
            string temp;
            var rand = new Random();
            do {
                otp.Clear();
                for (int i = 0; i < 6; i++) {
                    temp = arr[rand.Next(0, arr.Length)];
                    otp.Append(temp);
                }
            } while (otps.Contains(otp.ToString()));

            return otp.ToString();
        }

        public static string CreatePassword(int length) {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var res = new StringBuilder();
            var rnd = new Random();
            while (0 < length--) {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}