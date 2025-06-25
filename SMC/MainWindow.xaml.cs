using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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
using Brushes = System.Windows.Media.Brushes;

namespace SMC
{
    public class Parameter
    {
        public string Port_val { get; set; }
        public string Pulses_Per_Revolution_val { get; set; }
        public string Microseconds_Per_2_Pulses_val { get; set; }
    }
    
    public partial class MainWindow : Window
    {
        // StepperMotor Control
        public MainWindow()
        {
            InitializeComponent();
        }
        
        #region Function
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("請問是否要關閉？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        public void Button_Init(bool state)
        {
            if (state == false)
            {
                Rotate.IsEnabled = false;
                Rotate.Background = Brushes.Gray;
                Stop.IsEnabled = false;
                Stop.Background = Brushes.Gray;
            }
            else
            {
                Rotate.IsEnabled = true;
                Rotate.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb( 109, 180, 239)); 
                Stop.IsEnabled = true;
                Stop.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(109, 180, 239));

            }
            
        }

        public void CheckConnect()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (!sp.IsOpen)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            Connect_Led.LightColor = System.Windows.Media.Color.FromRgb(0, 255, 0);
                            Rotate_Led.LightColor = System.Windows.Media.Color.FromRgb(0, 255, 0);
                        });

                    }
                }
            });
        }

        #region Config
        private void LoadConfig()
        {
            List<Parameter> Parameter_info = Config.Load();
            Pulses_Per_Revolution.Text = Parameter_info[0].Pulses_Per_Revolution_val;
            Microseconds_Per_2_Pulses.Text = Parameter_info[0].Microseconds_Per_2_Pulses_val;
            Port.Text = Parameter_info[0].Port_val;
        }

        private void SaveConfig()
        {
            List<Parameter> Parameter_config = new List<Parameter>()
                        {
                            new Parameter() {Pulses_Per_Revolution_val=Pulses_Per_Revolution.Text,
                                             Microseconds_Per_2_Pulses_val=Microseconds_Per_2_Pulses.Text,
                                             Port_val = Port.Text
                                             }
                        };
            Config.Save(Parameter_config);
            Logger.WriteLog("儲存參數!", 1, richTextBoxGeneral);
        }
        #endregion
        #endregion

        #region Parameter and Init
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            Button_Init(false);
        }
        BaseLogRecord Logger = new BaseLogRecord();
        BaseConfig<Parameter> Config = new BaseConfig<Parameter>();
        SerialPort sp = new SerialPort();
        #endregion

        #region Main Screen
        private void Main_Btn_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case nameof(Connect):
                    {
                        if (!string.IsNullOrEmpty(Port.Text))
                        {
                            sp.PortName = Port.Text;
                            sp.BaudRate = 115200;
                            try
                            {
                                sp.Open();
                                Connect_Led.LightColor = System.Windows.Media.Color.FromRgb(0, 255, 0);
                                Button_Init(true);
                                Logger.WriteLog("連線成功!", 1, richTextBoxGeneral);
                                CheckConnect();
                            }
                            catch
                            {
                                MessageBox.Show("連線失敗，請檢查線路、Com port或BaudRate=115200正確性!", "確認", MessageBoxButton.OK, MessageBoxImage.Warning);
                                Logger.WriteLog("連線失敗!", 1, richTextBoxGeneral);
                            }
                        }
                        else
                        {
                            MessageBox.Show("請輸入Com port!", "確認", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        break;
                    }
                case nameof(Rotate):
                    {
                        sp.WriteLine("1");
                        sp.WriteLine(Pulses_Per_Revolution.Text);
                        sp.Write(Microseconds_Per_2_Pulses.Text);
                        sp.Write(Microseconds_Per_2_Pulses.Text);
                        Rotate_Led.LightColor = System.Windows.Media.Color.FromRgb(0, 255, 0);
                        Logger.WriteLog("開始轉動!", 1, richTextBoxGeneral);
                        break;
                    }
                case nameof(Stop):
                    {
                        sp.WriteLine("0");
                        Rotate_Led.LightColor = System.Windows.Media.Color.FromRgb(255, 0, 0);
                        Logger.WriteLog("停止轉動!", 1, richTextBoxGeneral);
                        break;
                    }
                case nameof(Save_Config):
                    {
                        SaveConfig();
                        break;
                    }
            }
        }
        #endregion



    }
}
