using DataAccess.Advantage;
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
    /// Interaction logic for Advantage.xaml
    /// </summary>
    public partial class Advantage : UserControl, IDatabaseConnectionControl
    {
        public Advantage()
        {
            InitializeComponent();
        }

        public DataAccess.Core.Interfaces.IDataStore GetConnection()
        {
            return new AdvantageDatabase(DataPath.Text, TableType.Text, ServerType.Text);
        }
    }
}
