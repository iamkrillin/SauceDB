using DataAccess.Core.Interfaces;
using DataAccess.PostgreSQL;
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
    /// Interaction logic for Postgre.xaml
    /// </summary>
    public partial class Postgre : UserControl, IDatabaseConnectionControl
    {
        public Postgre()
        {
            InitializeComponent();
        }

        public IDataStore GetConnection()
        {
            return PostgreSQLServerConnection.GetDataStore(ServerAddress.Text, ServerSchema.Text, ServerUser.Text, ServerPassword.Password);
        }
    }
}
