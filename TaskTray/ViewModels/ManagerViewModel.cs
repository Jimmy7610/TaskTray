using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using TaskTray.Models;
using TaskTray.Services;
using Microsoft.Win32;

namespace TaskTray.ViewModels
{
    public class ManagerViewModel : BaseViewModel
    {
        private Category? _selectedCategory;
        private string _searchText = string.Empty;

        public ObservableCollection<Category> Categories { get; }
        public ObservableCollection<AppItem> FilteredApps { get; }

        public ManagerViewModel()
        {
            Categories = new ObservableCollection<Category>(ConfigService.Data.Categories);
            FilteredApps = new ObservableCollection<AppItem>();

            AddCategoryCommand = new RelayCommand(_ => AddCategory());
            AddProgramCommand = new RelayCommand(_ => AddProgram(), _ => SelectedCategory != null);
            RenameCategoryCommand = new RelayCommand(_ => RenameCategory(), _ => SelectedCategory != null);
            DeleteCategoryCommand = new RelayCommand(_ => DeleteCategory(), _ => SelectedCategory != null);
            
            LaunchAppCommand = new RelayCommand(app => LaunchApp(app as AppItem));
            DeleteAppCommand = new RelayCommand(app => DeleteApp(app as AppItem));
            
            ImportCommand = new RelayCommand(_ => ImportConfig());
            ExportCommand = new RelayCommand(_ => ExportConfig());

            if (Categories.Any())
                SelectedCategory = Categories.First();
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    UpdateAppList();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    UpdateAppList();
                }
            }
        }

        public bool HasApps => FilteredApps.Any();
        public bool NoApps => !HasApps && SelectedCategory != null;

        public ICommand AddCategoryCommand { get; }
        public ICommand AddProgramCommand { get; }
        public ICommand RenameCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand LaunchAppCommand { get; }
        public ICommand DeleteAppCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ExportCommand { get; }

        private void UpdateAppList()
        {
            FilteredApps.Clear();
            if (SelectedCategory == null)
            {
                OnPropertyChanged(nameof(HasApps));
                OnPropertyChanged(nameof(NoApps));
                return;
            }

            var items = SelectedCategory.Items.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                items = items.Where(i => i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var item in items)
            {
                FilteredApps.Add(item);
            }
            
            OnPropertyChanged(nameof(HasApps));
            OnPropertyChanged(nameof(NoApps));
        }

        private void AddCategory()
        {
            // Simple prompt for category name
            string name = InteractionUtils.ShowInputBox(
                LanguageService.GetString("EnterCategoryName"), 
                LanguageService.GetString("AddCategory"));

            if (!string.IsNullOrWhiteSpace(name))
            {
                var cat = new Category { Name = name };
                ConfigService.Data.Categories.Add(cat);
                Categories.Add(cat);
                ConfigService.Save();
                SelectedCategory = cat;
                TrayService.RefreshMenu();
            }
        }

        private void AddProgram()
        {
            if (SelectedCategory == null) return;

            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*",
                Title = LanguageService.GetString("AddProgram")
            };

            if (ofd.ShowDialog() == true)
            {
                AddAppToCategory(ofd.FileName);
            }
        }

        public void AddAppToCategory(string filePath)
        {
            if (SelectedCategory == null) return;

            string resolvedPath = ShellService.ResolveShortcut(filePath);
            var app = new AppItem
            {
                Name = ShellService.GetFileName(filePath),
                Path = resolvedPath,
                IconBase64 = ShellService.GetIconBase64(resolvedPath)
            };

            SelectedCategory.Items.Add(app);
            ConfigService.Save();
            UpdateAppList();
            TrayService.RefreshMenu();
        }

        private void RenameCategory()
        {
            if (SelectedCategory == null) return;

            string name = InteractionUtils.ShowInputBox(
                LanguageService.GetString("EnterCategoryName"), 
                LanguageService.GetString("RenameCategory"), 
                SelectedCategory.Name);

            if (!string.IsNullOrWhiteSpace(name))
            {
                SelectedCategory.Name = name;
                ConfigService.Save();
                // Refresh binding
                int idx = Categories.IndexOf(SelectedCategory);
                Categories[idx] = SelectedCategory;
                SelectedCategory = Categories[idx];
                TrayService.RefreshMenu();
            }
        }

        private void DeleteCategory()
        {
            if (SelectedCategory == null) return;

            if (MessageBox.Show(LanguageService.GetString("ConfirmDelete"), 
                LanguageService.GetString("DeleteCategory"), 
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ConfigService.Data.Categories.Remove(SelectedCategory);
                Categories.Remove(SelectedCategory);
                ConfigService.Save();
                SelectedCategory = Categories.FirstOrDefault();
                TrayService.RefreshMenu();
            }
        }

        private void LaunchApp(AppItem? app)
        {
            if (app == null) return;
            try
            {
                Process.Start(new ProcessStartInfo(app.Path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch: {ex.Message}");
            }
        }

        private void DeleteApp(AppItem? app)
        {
            if (app != null && SelectedCategory != null)
            {
                if (MessageBox.Show(LanguageService.GetString("ConfirmDelete"), 
                    LanguageService.GetString("Remove"), 
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SelectedCategory.Items.Remove(app);
                    ConfigService.Save();
                    UpdateAppList();
                    TrayService.RefreshMenu();
                }
            }
        }

        private void ImportConfig()
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "JSON Files (*.json)|*.json" };
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    ConfigService.Load(); // Refresh local data from file if needed, or replace directly
                    // For logic simplicity here, we assume standard load
                    // Full implementation depends on ConfigService.Load() behavior
                    Categories.Clear();
                    foreach (var cat in ConfigService.Data.Categories) Categories.Add(cat);
                    SelectedCategory = Categories.FirstOrDefault();
                    TrayService.RefreshMenu();
                }
                catch (Exception ex) { MessageBox.Show($"Import failed: {ex.Message}"); }
            }
        }

        private void ExportConfig()
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "JSON Files (*.json)|*.json", FileName = "tasktray_config.json" };
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                    string json = System.Text.Json.JsonSerializer.Serialize(ConfigService.Data, options);
                    System.IO.File.WriteAllText(sfd.FileName, json);
                }
                catch (Exception ex) { MessageBox.Show($"Export failed: {ex.Message}"); }
            }
        }
    }

    public static class InteractionUtils
    {
        public static string ShowInputBox(string prompt, string title, string defaultValue = "")
        {
            // Minimalist WPF Input Box
            Window win = new Window
            {
                Title = title, Width = 350, Height = 170, WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize, WindowStyle = WindowStyle.ToolWindow,
                Background = (Brush)Application.Current.FindResource("BackgroundBrush"),
                Foreground = (Brush)Application.Current.FindResource("TextBrush")
            };

            StackPanel sp = new StackPanel { Margin = new Thickness(15) };
            sp.Children.Add(new TextBlock { Text = prompt, Margin = new Thickness(0, 0, 0, 10), FontWeight = FontWeights.Bold });
            TextBox txt = new TextBox { Text = defaultValue, Margin = new Thickness(0, 0, 0, 15) };
            sp.Children.Add(txt);

            Button btn = new Button { Content = "OK", IsDefault = true, HorizontalAlignment = HorizontalAlignment.Right, Padding = new Thickness(20, 5, 20, 5) };
            btn.Click += (s, e) => { win.DialogResult = true; win.Close(); };
            sp.Children.Add(btn);

            win.Content = sp;
            txt.Focus();
            txt.SelectAll();

            return win.ShowDialog() == true ? txt.Text : string.Empty;
        }
    }
}
