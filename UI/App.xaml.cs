namespace UI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
            window.Height = 1080;
            window.Width = 1920;
            window.X = 0;
            window.Y = 0;
            return window;
        }
    }
}