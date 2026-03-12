using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Diagnostics;
using TaskTray.Models;
using TaskTray.Services;

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

            var ofd = new Microsoft.Win32.OpenFileDialog
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
                int idx = Categories.IndexOf(SelectedCategory);
                Categories[idx] = SelectedCategory;
                SelectedCategory = Categories[idx];
                TrayService.RefreshMenu();
            }
        }

        private void DeleteCategory()
        {
            if (SelectedCategory == null) return;

            if (System.Windows.MessageBox.Show(LanguageService.GetString("ConfirmDelete"), 
                LanguageService.GetString("DeleteCategory"), 
                System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
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
                System.Windows.MessageBox.Show($"Failed to launch: {ex.Message}");
            }
        }

        private void DeleteApp(AppItem? app)
        {
            if (app != null && SelectedCategory != null)
            {
                if (System.Windows.MessageBox.Show(LanguageService.GetString("ConfirmDelete"), 
                    LanguageService.GetString("Remove"), 
                    System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
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
            var ofd = new Microsoft.Win32.OpenFileDialog { Filter = "JSON Files (*.json)|*.json" };
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    ConfigService.Load();
                    Categories.Clear();
                    foreach (var cat in ConfigService.Data.Categories) Categories.Add(cat);
                    SelectedCategory = Categories.FirstOrDefault();
                    TrayService.RefreshMenu();
                }
                catch (Exception ex) { System.Windows.MessageBox.Show($"Import failed: {ex.Message}"); }
            }
        }

        private void ExportConfig()
        {
            var sfd = new Microsoft.Win32.SaveFileDialog { Filter = "JSON Files (*.json)|*.json", FileName = "tasktray_config.json" };
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                    string json = System.Text.Json.JsonSerializer.Serialize(ConfigService.Data, options);
                    System.IO.File.WriteAllText(sfd.FileName, json);
                }
                catch (Exception ex) { System.Windows.MessageBox.Show($"Export failed: {ex.Message}"); }
            }
        }
    }

    public static class InteractionUtils
    {
        public static string ShowInputBox(string prompt, string title, string defaultValue = "")
        {
            var win = new System.Windows.Window
            {
                Title = title, Width = 350, Height = 170, WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                ResizeMode = System.Windows.ResizeMode.NoResize, WindowStyle = System.Windows.WindowStyle.ToolWindow,
                Background = (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource("BackgroundBrush"),
                Foreground = (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource("TextBrush")
            };

            var sp = new System.Windows.Controls.StackPanel { Margin = new System.Windows.Thickness(15) };
            sp.Children.Add(new System.Windows.Controls.TextBlock { Text = prompt, Margin = new System.Windows.Thickness(0, 0, 0, 10), FontWeight = System.Windows.FontWeights.Bold });
            var txt = new System.Windows.Controls.TextBox { Text = defaultValue, Margin = new System.Windows.Thickness(0, 0, 0, 15) };
            sp.Children.Add(txt);

            var btn = new System.Windows.Controls.Button { Content = "OK", IsDefault = true, HorizontalAlignment = System.Windows.HorizontalAlignment.Right, Padding = new System.Windows.Thickness(20, 5, 20, 5) };
            btn.Click += (s, e) => { win.DialogResult = true; win.Close(); };
            sp.Children.Add(btn);

            win.Content = sp;
            txt.Focus();
            txt.SelectAll();

            return win.ShowDialog() == true ? txt.Text : string.Empty;
        }
    }
}
