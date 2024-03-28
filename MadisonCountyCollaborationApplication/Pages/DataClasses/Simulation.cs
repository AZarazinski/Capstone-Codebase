namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class Simulation
    {
        public DataClasses.Distribution AssignDistribution(Parameters distribution)
        {
            if (distribution.dist.Equals("Uniform"))
            {
                if (Convert.ToDouble(distribution.param1) < Convert.ToDouble(distribution.param2))
                {
                    return new DataClasses.Uniform(new Random(), Convert.ToDouble(distribution.param1), Convert.ToDouble(distribution.param2));
                }
                else
                {
                    throw new InvalidOperationException("Min must be below Max");
                }
            }
            else if (distribution.dist.Equals("Triangular"))
            {
                if (Convert.ToDouble(distribution.param1) <= Convert.ToDouble(distribution.param2) &
                    Convert.ToDouble(distribution.param1) < Convert.ToDouble(distribution.param3))
                {
                    if (Convert.ToDouble(distribution.param3) <= Convert.ToDouble(distribution.param2))
                    {
                        return new DataClasses.Triangular(new Random(), Convert.ToDouble(distribution.param1), Convert.ToDouble(distribution.param3),
                            Convert.ToDouble(distribution.param2));
                    }
                    else
                    {
                        throw new InvalidOperationException("Likely must be below the max");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Min must be below max and Likely");
                }

            }
            else if (distribution.dist.Equals("Normal"))
            {
                return new DataClasses.Normal(new Random(), Convert.ToDouble(distribution.param1), Convert.ToDouble(distribution.param2));
            }
            else if (distribution.dist.Equals("Lognormal"))
            {
                return new DataClasses.Normal(new Random(), Convert.ToDouble(distribution.param1), Convert.ToDouble(distribution.param2));
            }
            else
            {
                return new DataClasses.Constant(new Random(), Convert.ToDouble(distribution.param3));
            }
        }
        public double GenerateResult(DataClasses.Distribution dist, double initial, string growth, double years)
        {
            if (growth.Equals("Constant"))
            {
                return initial * Math.Pow(1 + dist.GenerateRandom(), years);
            }
            else if (growth.Equals("Nonconstant"))
            {
                for (int i = 0; i < years; i++)
                {
                    initial *= 1 + dist.GenerateRandom();
                }
                return initial;
            }
            else
            {
                return dist.GenerateRandom();
            }
        }
        public double[] ConfidenceInterval(double[] Results, double significance)
        {
            double average = Results.Average();
            double sumOfSquaredDeviations = Results.Select(val => Math.Pow(val - average, 2)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaredDeviations / (Results.Length - 1));
            double[] CI = new double[2];
            if (significance == 95)
            {
                CI[0] = average - 1.959964 * standardDeviation / Math.Sqrt(Results.Length);
                CI[1] = average + 1.959964 * standardDeviation / Math.Sqrt(Results.Length);
            }
            else if (significance == 90)
            {
                
                CI[0] = average - 1.644854 * standardDeviation / Math.Sqrt(Results.Length);
                CI[1] = average + 1.644854 * standardDeviation / Math.Sqrt(Results.Length);
            }
            else
            {

                CI[0] = average - 2.575829 * standardDeviation / Math.Sqrt(Results.Length);
                CI[1] = average + 2.575829 * standardDeviation / Math.Sqrt(Results.Length);
            }
            return CI;
        }
    }

}
