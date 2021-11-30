using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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


namespace Сache_Сleaner
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
            TreeViewItem treeViewItem = new TreeViewItem();
            cElement element = new cElement("");
            //<TreeView x:Name="treeViewBases"  Margin="10,0,10,10" />
            string fileName = Environment.GetEnvironmentVariable("AppData")+ "\\1C\\1CEStart\\ibases.v8i";
            foreach (string line in System.IO.File.ReadLines(fileName))
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    element = new cElement(line.Substring(1, line.Length - 2));
                    treeViewItem = new TreeViewItem();
                    treeViewItem.Header = element.ToString();
                    treeViewItem.Tag = element;
                }

                if (line.StartsWith("ID="))                   
                    element.guid = new Guid(line.Substring(3));

                if (line.StartsWith("Connect="))
                    element.Connect = line.Substring(8);
                    
                if (line.StartsWith("Folder="))
                {

                    element.Folder = line.Substring(7);                    
                    
                    if (element.Folder == @"/")
                        treeViewBases.Items.Add(treeViewItem);
                    else
                    {
                        ItemCollection ic = treeViewBases.Items;
                        foreach (string path in element.Folder.Split('/'))
                        {
                            foreach (TreeViewItem item in ic)
                                if (((cElement)item.Tag).Name.Equals(path))
                                {
                                    ic = item.Items;
                                    break;
                                }
                        }
                        ic.Add(treeViewItem);
                    }
                    
                }

            }

            
               
        }

        private void ShowInfo()
        {
            cElement element = ((cElement)((TreeViewItem)treeViewBases.SelectedItem).Tag);
            LabelPath.Content = element.Connect;
            TextGuid.Text = "GUID = " + element.guid.ToString() + "; Size = " + (element.CacheSize/1024/1024).ToString("# ##0") + " mb";
        }

        private void treeViewBases_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ShowInfo();

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ((cElement)((TreeViewItem)treeViewBases.SelectedItem).Tag).ClearCache();
            ShowInfo();


        }
    }
}
