using System.Windows;
using TaskTray.ViewModels;

namespace TaskTray.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ManagerViewModel();
            this.AllowDrop = true;
            this.DragEnter += OnDragEnter;
            this.Drop += OnDrop;
        }

        private void OnDragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                e.Effects = System.Windows.DragDropEffects.Copy;
        }

        private void OnDrop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (DataContext is ManagerViewModel vm)
                {
                    foreach (string file in files)
                    {
                        vm.AddAppToCategory(file);
                    }
                }
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Minimize to tray instead of exit
            e.Cancel = true;
            this.Hide();
        }
    }
}
