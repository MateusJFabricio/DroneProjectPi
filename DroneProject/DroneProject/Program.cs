using View;

namespace DroneProject
{
    public class Program
    {
        static ConsoleView ConsoleView = new();

        static void Main(string[] args)
        {
            bool iteract = true;

            ConsoleView.Start(iteract);
        }
    }
}