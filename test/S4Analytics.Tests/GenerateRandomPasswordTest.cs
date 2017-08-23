using System;
using Xunit;
using System.Security.Cryptography;
using Xunit.Abstractions;


namespace S4Analytics.Tests.Properties
{
    public class GenerateRandomPasswordTest
    {
        private readonly ITestOutputHelper output;

        public GenerateRandomPasswordTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void GenerateRandomPassword()
        {
            var len = 0;
            var repeat = 10000;

            var str = string.Empty;
            for (int i = 0; i < repeat; i++)
            {
                var pwd = GenerateRandomPassword(8, 0);
                str += pwd;
                len += pwd.Length;
            }

            output.WriteLine(str);
            Assert.True(str.Length == 8*repeat);
        }

        private string GenerateRandomPassword(int length, int numberOfNonAlphanumericCharacters)
        {
            int nonANcount = 0;
            byte[] buffer1 = new byte[length];

            //chPassword contains the password's characters as it's built up
            char[] chPassword = new char[length];

            //chPunctionations contains the list of legal non-alphanumeric characters
            char[] chPunctuations = "!@#$%^*()_-+=[{]};:|./?".ToCharArray();

            //Get a cryptographically strong series of bytes
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer1);

            for (int i = 0; i < length; i++)
            {
                //Convert each byte into its representative character
                int rndChr = buffer1[i] % 85;
                if (rndChr < 10)
                {
                    chPassword[i] = Convert.ToChar(Convert.ToUInt16(48 + rndChr));
                }
                else
                if (rndChr < 36)
                {

                    chPassword[i] = Convert.ToChar(Convert.ToUInt16((65 + rndChr) - 10));
                }
                else
                if (rndChr < 62)
                {

                    chPassword[i] = Convert.ToChar(Convert.ToUInt16((97 + rndChr) - 36));
                }
                else
                {
                    chPassword[i] = chPunctuations[rndChr - 62];
                    nonANcount += 1;
                }
            }

            if (nonANcount < numberOfNonAlphanumericCharacters)
            {
                Random rndNumber = new Random();
                for (int i = 0; i < (numberOfNonAlphanumericCharacters - nonANcount); i++)
                {
                    int passwordPos;
                    do
                    {
                        passwordPos = rndNumber.Next(0, length);
                    }
                    while (!char.IsLetterOrDigit(chPassword[passwordPos]));
                    chPassword[passwordPos] = chPunctuations[rndNumber.Next(0, chPunctuations.Length)];
                }
            }

            return new String(chPassword);
        }

    }
}
