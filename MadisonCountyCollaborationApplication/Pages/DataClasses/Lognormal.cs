using System.Reflection.Metadata;

namespace _488Labs.Pages.DataClasses
{
    public class Lognormal : Distribution
    {
        private Random rand;
        private double mean;
        private double variance;
        public Lognormal(Random rand, double mu, double sigma) : base(rand)
        {
            mean = mu;
            variance = sigma;
            this.rand = rand;
        }
        //log tranformation of Box-Muller transform
        public double GenerateRandom()
        {
            double u1 = 1 - rand.NextDouble();
            double u2 = 1 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);
            return Math.Pow(Math.E, mean + Math.Sqrt(variance) * randStdNormal);
        }
    }
}
