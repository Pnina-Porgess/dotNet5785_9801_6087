namespace stage0
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            welcome9801();
            welcome6087();
            Console.ReadKey();

        }
        private static void welcome9801() { }
        private static void welcome6087()
        {
            Console.WriteLine("Enter your name: pnina");
            string name = Console.ReadLine();
            Console.WriteLine("{0} welcome to my first console application", name);
        }
    }
}