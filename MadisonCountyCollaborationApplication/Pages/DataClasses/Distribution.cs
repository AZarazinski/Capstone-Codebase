using System.Reflection.Metadata;

namespace _488Labs.Pages.DataClasses
{
    public class Distribution
    {
        private Random rand;
        public Distribution(Random random)
        {
            rand = random;
        }
        public double GenerateRandom()
        {
            return rand.NextDouble();
        }
    }
}
