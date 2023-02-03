using System;
using System.IO;
using System.Text;

namespace Minder.Service.Helpers {

    public static class EMailHelper {

        public static string GetOTPBody(string OTP) {
            if (!Directory.Exists("Template")) Directory.CreateDirectory("Template");
            string filename = $"Template/OTPTemplate.html";
            string body = File.ReadAllText(filename);
            body = body.Replace("#OTP#", OTP);
            return body;
        }

        public static string GenarateOTP() {
            string[] arr = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            var otp = new StringBuilder();
            string temp;
            var rand = new Random();
            for (int i = 0; i < 6; i++) {
                temp = arr[rand.Next(0, arr.Length)];
                otp.Append(temp);
            }
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