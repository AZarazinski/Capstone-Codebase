namespace _488Labs.Pages.DataClasses
{
    public class Normal : Distribution
    {
        private Random rand;
        private double mean;
        private double variance;
        public Normal(Random rand, double mu, double sigma) : base(rand)
        {
            mean = mu;
            variance = sigma;
            this.rand = rand;
        }
        //Box-Muller transform
        public double GenerateRandom()
        {
            double u1 = 1 - rand.NextDouble();
            double u2 = 1 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);
            return mean + Math.Sqrt(variance) * randStdNormal;

        }
    }
}
