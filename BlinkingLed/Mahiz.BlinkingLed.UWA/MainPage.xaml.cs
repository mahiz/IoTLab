using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Mahiz.BlinkingLed.UWA
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool gpioAvailable;
        GpioController gpio;
        GpioPin pin;

        private readonly int GPIO12 = 12;

        DispatcherTimer timer = new DispatcherTimer();

        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        GpioPinValue ledStatus;
        GpioPinValue LedStatus
        {
            get { return ledStatus; }
            set
            {
                ledStatus = value;

                if (pin != null)
                {
                    pin.Write(ledStatus);
                }

                LedImage.Fill = (LedStatus == GpioPinValue.Low) ? grayBrush : redBrush;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            MyInit();
        }

        private async void MyInit()
        {
            await InitGpio();
            BlinkingSwitch.IsEnabled = gpioAvailable;
            timer.Tick += Timer_Tick;
        }

        private async Task InitGpio()
        {
            gpioAvailable = false;

            if (ApiInformation.IsTypePresent(typeof(GpioController).ToString()))
            {
                gpio = GpioController.GetDefault();
                if (gpio != null)
                {
                    pin = gpio.OpenPin(GPIO12);
                    if (pin != null)
                    {
                        gpioAvailable = true;
                        LedStatus = GpioPinValue.Low;
                        pin.SetDriveMode(GpioPinDriveMode.Output);
                    }
                }
            }
            else
            {
                await new MessageDialog("GPIO is unavailable on this device").ShowAsync();
            }
        }

        private void BlinkingPeriod_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            timer.Interval = TimeSpan.FromMilliseconds(e.NewValue);
        }

        private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            
            if (BlinkingSwitch.IsOn)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
                LedStatus = GpioPinValue.Low;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            if (LedStatus == GpioPinValue.Low)
            {
                LedStatus = GpioPinValue.High;
            }
            else
            {
                LedStatus = GpioPinValue.Low;
            }
        }

    }
}
