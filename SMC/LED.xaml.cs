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

namespace SMC
{
    /// <summary>
    /// LED.xaml 的互動邏輯
    /// </summary>
    public partial class LED : UserControl
    {
        public LED()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsOnProperty =
        DependencyProperty.Register("DIO_LED", typeof(int), typeof(LED), new FrameworkPropertyMetadata(2, OnIsOnChanged));

        public int DIO_LED
        {
            get { return (int)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }

        private static void OnIsOnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as LED;
            control.OnIsOnChanged();
        }

        private void OnIsOnChanged()
        {
            switch (DIO_LED)
            {
                case 0:
                    led.Fill = Brushes.Green;
                    break;
                case 1:
                    led.Fill = Brushes.Orange;
                    break;
                case 2:
                    led.Fill = Brushes.Red;
                    break;
            }
              
        }


    }
}
