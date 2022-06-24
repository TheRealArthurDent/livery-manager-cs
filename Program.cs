namespace LiveryManager
{
    class Runner
    {
        private const int DAYS = 60;

        static void Main(string[] args)
        {
            ICollection<int> exceptedDriverIds = new HashSet<int>();
            foreach (string arg in args)
            {
                try
                {
                    exceptedDriverIds.Add(Int32.Parse(arg));
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Unable to parse '{arg}' as a DriverID. It will be ignored.");
                }
            }
            new FileWalker(DAYS, exceptedDriverIds).Walk();
        }
    }
}