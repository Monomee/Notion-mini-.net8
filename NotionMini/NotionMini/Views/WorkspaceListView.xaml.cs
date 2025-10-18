using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NotionMini.ViewModels;
using NotionMini.ViewModels.Models;

namespace NotionMini.Views
{
    /// <summary>
    /// Interaction logic for WorkspaceListView.xaml
    /// </summary>
    public partial class WorkspaceListView : UserControl
    {
        public WorkspaceListView()
        {
            InitializeComponent();
            
            // Test: Add some debug info when control loads
            Loaded += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("WorkspaceListView loaded");
                var vm = DataContext as WorkspaceListViewModel;
                if (vm != null)
                {
                    System.Diagnostics.Debug.WriteLine($"ViewModel found, Workspaces count: {vm.Workspaces.Count}");
                    System.Diagnostics.Debug.WriteLine($"DeleteWorkspaceCommand CanExecute: {vm.DeleteWorkspaceCommand.CanExecute(null)}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ViewModel is null!");
                }
            };
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ListBox double click detected!");
            
            if (sender is ListBox listBox)
            {
                var item = listBox.SelectedItem as WorkspaceItemViewModel;
                if (item != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Selected workspace: {item.Name}");
                    
                    var viewModel = DataContext as WorkspaceListViewModel;
                    if (viewModel != null)
                    {
                        System.Diagnostics.Debug.WriteLine("ViewModel found, executing rename command");
                        viewModel.RenameWorkspaceCommand.Execute(item);
                        
                        // Use Dispatcher to ensure UI is updated
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            var listBoxItem = listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                            if (listBoxItem != null)
                            {
                                var textBox = FindTextBoxInItem(listBoxItem);
                                if (textBox != null)
                                {
                                    System.Diagnostics.Debug.WriteLine("TextBox found, focusing");
                                    textBox.Focus();
                                    textBox.SelectAll();
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("TextBox not found!");
                                }
                            }
                        }), System.Windows.Threading.DispatcherPriority.Input);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ViewModel is null!");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No workspace selected!");
                }
            }
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Double click detected!");
            
            if (sender is ListBoxItem listBoxItem && listBoxItem.DataContext is WorkspaceItemViewModel workspace)
            {
                System.Diagnostics.Debug.WriteLine($"Workspace found: {workspace.Name}");
                
                var viewModel = DataContext as WorkspaceListViewModel;
                if (viewModel != null)
                {
                    System.Diagnostics.Debug.WriteLine("ViewModel found, executing rename command");
                    viewModel.RenameWorkspaceCommand.Execute(workspace);
                    
                    // Focus the TextBox for editing
                    var textBox = FindTextBoxInItem(listBoxItem);
                    if (textBox != null)
                    {
                        System.Diagnostics.Debug.WriteLine("TextBox found, focusing");
                        textBox.Focus();
                        textBox.SelectAll();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("TextBox not found!");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ViewModel is null!");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ListBoxItem or DataContext is not correct type");
            }
        }

        private TextBox? FindTextBoxInItem(ListBoxItem item)
        {
            return FindVisualChild<TextBox>(item);
        }

        private T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;
                
                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        private async void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is WorkspaceItemViewModel workspace)
            {
                var viewModel = DataContext as WorkspaceListViewModel;
                if (viewModel != null)
                {
                    await viewModel.SaveRenameAsync(workspace, textBox.Text);
                }
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (e.Key == Key.Enter)
                {
                    textBox.LostFocus += TextBox_LostFocus;
                    textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    textBox.LostFocus -= TextBox_LostFocus;
                }
                else if (e.Key == Key.Escape)
                {
                    if (textBox.DataContext is WorkspaceItemViewModel workspace)
                    {
                        workspace.IsEditing = false;
                    }
                }
            }
        }
    }
}
