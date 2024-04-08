namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class Uniform : Distribution
    {
        public Random rand;
        public double min;
        public double max;
        public Uniform(Random rand, double low, double high) : base(rand)
        {
            min = low;
            max = high;
            this.rand = rand;
        }
        //inverse uniform CDF
        public override double GenerateRandom()
        {
            return min + (max - min) * (1 - rand.NextDouble());
        }
        public override string PrintType()
        {
            return "Uniform";
        }
    }
}
