using DataAccess.Core.Interfaces;
using DataAccess.MySql;
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
    /// Interaction logic for MySql.xaml
    /// </summary>
    public partial class MySql : UserControl, IDatabaseConnectionControl
    {
        public MySql()
        {
            InitializeComponent();
        }

        public IDataStore GetConnection()
        {
            return MySqlServerConnection.GetDataStore(ServerAddress.Text, ServerSchema.Text, ServerUser.Text, ServerPassword.Password);
        }
    }
}
