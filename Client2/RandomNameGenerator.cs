using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client2
{
    internal class RandomNameGenerator
    {
        private static readonly Random random = new Random();
        public static string GenerateRandomName()
        {
            string[] prefixes = { "Awesome", "Wonderful", "Brilliant", "Fantastic", "Magnificent" };
            string[] suffixes = { "Coder", "Developer", "Programmer", "Hacker", "Engineer" };
            string randomPrefix = prefixes[random.Next(prefixes.Length)];
            string randomSuffix = suffixes[random.Next(suffixes.Length)];
            return $"{randomPrefix} {randomSuffix}";
        }
    }
}
