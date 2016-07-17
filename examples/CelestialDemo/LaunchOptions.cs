using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace CelestialEngine.TechDemo
{
    public partial class LaunchOptions : Form
    {
        public LaunchOptions()
        {
            InitializeComponent();
        }

        struct Res : IComparable<Res>, IComparable
        {
            public int X;
            public int Y;

            public int CompareTo(Res other)
            {
                return other.X - X;
            }

            public override string ToString()
            {
                return String.Format("{0} x {1}", X, Y);
            }

            public int CompareTo(object obj)
            {
                if (obj.GetType().Equals(this.GetType()))
                    return ((Res)obj).X - X;

                return 0;
            }
        }

        private void LaunchOptions_Load(object sender, EventArgs e)
        {
            DEVMODE vDevMode = new DEVMODE();
            int i = 0;
            List<object> items = new List<object>();
            while (EnumDisplaySettings(null, i, ref vDevMode))
            {
                Res dick = new Res()
                {
                    X = vDevMode.dmPelsWidth,
                    Y = vDevMode.dmPelsHeight
                };

                if (!items.Contains(dick))
                    items.Add(dick); // Needs more dick

                i++;
            }

            items.Sort();

            comboBox1.Items.AddRange(items.ToArray());
            comboBox1.SelectedIndex = 0;
        }

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(
              string deviceName, int modeNum, ref DEVMODE devMode);
        const int ENUM_CURRENT_SETTINGS = -1;

        const int ENUM_REGISTRY_SETTINGS = -2;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {

            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread execThread = new Thread(() =>
                {
                    using (TechDemoGame game = new TechDemoGame())
                    {
                        game.GraphicsDeviceManager.PreferredBackBufferWidth = ((Res)comboBox1.SelectedItem).X;
                        game.GraphicsDeviceManager.PreferredBackBufferHeight = ((Res)comboBox1.SelectedItem).Y;
                        game.IsFixedTimeStep = checkBox2.Checked;
                        game.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
                        game.GraphicsDeviceManager.IsFullScreen = checkBox1.Checked;
                        game.GraphicsDeviceManager.ApplyChanges();
                        game.IsDebug = checkBox3.Checked;
                        game.Run();
                    }
                });

            execThread.Start();

            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
