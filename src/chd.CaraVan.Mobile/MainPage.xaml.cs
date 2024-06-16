namespace chd.CaraVan.Mobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.Disappearing += this.MainPage_Disappearing;
            this.Appearing += this.MainPage_Appearing;
        }

        private void MainPage_Appearing(object? sender, EventArgs e)
        {
        }

        private void MainPage_Disappearing(object? sender, EventArgs e)
        {
        }
    }
}
