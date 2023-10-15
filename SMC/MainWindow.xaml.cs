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
        public string Pluse_val { get; set; }
        public string Rotational_Speed_val { get; set; }
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
                            Connect_Led.DIO_LED = 2;
                            Rotate_Led.DIO_LED = 2;
                        });

                    }
                }
            });
        }

        #region Config
        private void LoadConfig()
        {
            List<Parameter> Parameter_info = Config.Load();
            Pluse.Text = Parameter_info[0].Pluse_val;
            Rotational_Speed.Text = Parameter_info[0].Rotational_Speed_val;
        }

        private void SaveConfig()
        {
            List<Parameter> Parameter_config = new List<Parameter>()
                        {
                            new Parameter() {Pluse_val=Pluse.Text,
                                             Rotational_Speed_val=Rotational_Speed.Text
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
                        sp.PortName = "COM5";
                        sp.BaudRate = 115200;
                        try
                        {
                            sp.Open();
                            Connect_Led.DIO_LED = 0;
                            Button_Init(true);
                            Logger.WriteLog("連線成功!", 1, richTextBoxGeneral);
                            CheckConnect();
                        }
                        catch
                        {
                            MessageBox.Show("連線失敗，請檢查線路或Com port正確性!", "確認", MessageBoxButton.OK, MessageBoxImage.Warning);
                            Logger.WriteLog("連線失敗!", 1, richTextBoxGeneral);
                        }
                        break;
                    }
                case nameof(Rotate):
                    {
                        sp.WriteLine("1");
                        sp.WriteLine(Pluse.Text);
                        sp.Write(Rotational_Speed.Text);
                        sp.Write(Rotational_Speed.Text);
                        Rotate_Led.DIO_LED = 0;
                        Logger.WriteLog("開始轉動!", 1, richTextBoxGeneral);
                        break;
                    }
                case nameof(Stop):
                    {
                        sp.WriteLine("0");
                        Rotate_Led.DIO_LED = 2;
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
