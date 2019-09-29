using System;
namespace csharpworker.TestClasses.Utils
{
    public class TestUtils
    {
        public TestUtils()
        {
        }

        public static string[] GenerateAsciiStrings(int count, int length)
        {
            string[] strings = new string[count];
            for (int i = 0; i < count; i++)
            {
                strings[i] = GenerateAsciiString(length);
            }

            return strings;
        }

        public static string GenerateAsciiString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
    }
}
