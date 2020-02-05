using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Psafe3.Cli.Model;
using WinAuth;

namespace Psafe3.Cli
{
    internal class Program
    {
        private static void Main(
            string[] args)
        {
            using var stream = File.Open(args[0], FileMode.Open, FileAccess.Read);

            using var reader = new PasswordSafeReader(stream);

            reader.SetPassphrase(args[1]);

            var list = new List<KeyValuePair<string, string>>();

            while (reader.Read())
            {
                switch (reader.CurrentPartType)
                {
                    case PasswordSafePartType.Header:
                        var header = reader.Header;

                        break;

                    case PasswordSafePartType.Record:
                        var record = reader.Record;

                        var group = record.Group;

                        const string strongboxLabel = "Strongbox TOTP Auth URL: [";

                        if (record.Notes?.Contains(strongboxLabel) == true)
                        {
                            var beginIndex =
                                record.Notes.IndexOf(
                                    strongboxLabel,
                                    StringComparison.Ordinal) + strongboxLabel.Length;

                            var endIndex =
                                record.Notes.IndexOf(']', beginIndex);

                            list.Add(
                                new KeyValuePair<string, string>(
                                    record.Title,
                                    record.Notes.Substring(
                                        beginIndex,
                                        endIndex - beginIndex)));
                        }

                        break;

                    case PasswordSafePartType.End:
                        break;

                    case PasswordSafePartType.None:
                        break;

                    case PasswordSafePartType.Keys:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var (key, value) in list)
            {
                var authenticator = new GoogleAuthenticator();

                authenticator.HMACType = Authenticator.HMACTypes.SHA256;

                var qs = ParseQueryString(value.Substring(value.IndexOf("?", StringComparison.Ordinal) + 1));

                var privatekey = qs["secret"];

                authenticator.Enroll(privatekey);

                if (int.TryParse(qs["digits"], out var querydigits) && querydigits != 0)
                {
                    authenticator.CodeDigits = querydigits;
                }

                var issuer = qs["issuer"];

                var beginIndex = value.IndexOf("otpauth://totp/", StringComparison.OrdinalIgnoreCase) +
                                 "otpauth://totp/".Length;

                var label =
                    value.Substring(
                        beginIndex,
                        value.IndexOf('?', StringComparison.OrdinalIgnoreCase) - beginIndex);

                var periods = qs["period"];

                if (string.IsNullOrEmpty(periods) == false)
                {
                    if (int.TryParse(periods, out var period))
                    {
                        authenticator.Period = period;
                    }
                }

                var code = authenticator.CurrentCode;

                Console.WriteLine($"{label}: {code}");
            }
        }

        public static NameValueCollection ParseQueryString(
            string qs)
        {
            var pairs = new NameValueCollection();

            // ignore blanks and remove initial "?"
            if (string.IsNullOrEmpty(qs))
            {
                return pairs;
            }

            if (qs.StartsWith("?"))
            {
                qs = qs.Substring(1);
            }

            // get each a=b&... key-value pair
            foreach (var p in qs.Split('&'))
            {
                var keypair = p.Split('=');
                var key = keypair[0];
                var v = keypair.Length >= 2 ? keypair[1] : null;
                if (string.IsNullOrEmpty(v) == false)
                {
                    // decode (without using System.Web)
                    string newv;
                    while ((newv = Uri.UnescapeDataString(v)) != v)
                    {
                        v = newv;
                    }
                }

                pairs.Add(key, v);
            }

            return pairs;
        }
    }
}