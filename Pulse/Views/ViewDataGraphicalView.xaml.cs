using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Caliburn.Micro;

namespace RTI
{
    /// <summary>
    /// Interaction logic for ViewDataGraphicalView.xaml
    /// </summary>
    public partial class ViewDataGraphicalView : UserControl
    {
        /// <summary>
        /// Setup the eventhandling.
        /// </summary>
        public ViewDataGraphicalView()
        {
            InitializeComponent();

            // Subscribe to receive events
            //IoC.Get<IEventAggregator>().Subscribe(this);
        }
    }
}
