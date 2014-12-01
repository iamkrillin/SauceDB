using DataAccess.Core.Interfaces;
using DataAccess.SQLite;
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

namespace TheCodeHaven.SauceClassGeneration.DatabaseInterfaces.Controls
{
    /// <summary>
    /// Interaction logic for SQLite.xaml
    /// </summary>
    public partial class SQLite : UserControl, IDatabaseConnectionControl
    {
        public SQLite()
        {
            InitializeComponent();
        }

        public IDataStore GetConnection()
        {
            return SqlLiteDataConnection.GetDataStore(File.Text);
        }
    }
}
