namespace _488Labs.Pages.DataClasses
{
    public class Uniform : Distribution
    {
        private Random rand;
        private double min;
        private double max;
        public Uniform(Random rand, double low, double high) : base(rand)
        {
            min = low;
            max = high;
            this.rand = rand;
        }
        //inverse uniform CDF
        public double GenerateRandom()
        {
            return min + (max - min) * (1 - rand.NextDouble());
        }
    }
}
