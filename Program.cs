namespace snippet_manager
{
    internal static class Program
    {
        static readonly Mutex mutex = new(initiallyOwned: true, name: "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE");

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(timeout: TimeSpan.Zero, exitContext: true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
            else
            {
                MessageBox.Show($"You can only run one instance of {Application.ProductName} at a time.",
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}