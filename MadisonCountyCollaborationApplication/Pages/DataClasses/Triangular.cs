namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class Triangular : Distribution
    {
        public Random rand;
        public double min;
        public double max;
        public double likely;
        public Triangular(Random rand, double low, double high, double most) : base(rand)
        {
            min = low;
            max = high;
            likely = most;
            this.rand = rand;
        }
        //inverse triangular CDF
        public override double GenerateRandom()
        {
            double probability = 1 - rand.NextDouble();
            if (probability < (likely - min) / (max - min))
            {
                return min + Math.Sqrt((likely - min) * (max - min) * probability);
            }
            else
            {
                return max - Math.Sqrt((max - min) * (max - likely) * (1 - probability));
            }
        }
        public override string PrintType()
        {
            return "Triangular";
        }
    }
}
