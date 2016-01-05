using System;
using System.Collections.Generic;
using System.Data;
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
using WpfApplication7.Code;

namespace WpfApplication7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainHelper MH = null;
        List<Player> scores = new List<Player>();
        Dictionary<string, int> challenge = new Dictionary<string, int>();
        public MainWindow()
        {
            InitializeComponent();

            comboBox.ItemsSource = new List<string>() { "/","X","1","2","3","4","5","6","7","8","9","0" };
            comboBox.SelectedItem = "1";
            comboBox.FontSize = 40;
        }


        #region Event Handlers
        
        private void btnChallenge_Click(object sender, RoutedEventArgs e)
        {
            string selection = comboBox.SelectedItem.ToString();
            challenge.Add(selection, 1);
            comboBox.IsEnabled = false;
            btnChallenge.IsEnabled = false;
        }
        private void btnSetLane_Click(object sender, RoutedEventArgs e)
        {
            string number = tbLaneNumber.Text.Trim();
            int laneNumber = 0;
            if(!Int32.TryParse(number,out laneNumber))
            {
                return;
            }
            XBowlingDataGrabber DG = new XBowlingDataGrabber(laneNumber);
            Thread scoreUpdater = new Thread(DG.Updater);
            scoreUpdater.Start();
            MH = new MainHelper(grid,challenge);
            Thread screenUpdater = new Thread(() => {
                while (true)
                {
                    if (DG.Scores != scores && DG.Scores.Count >= scores.Count)
                    {
                        scores = DG.Scores;
                        Dispatcher.Invoke(new Action(delegate ()
                        {
                            int children = grid.Children.Count;
                            for (int i = 0; i < children; i++)
                            {
                                grid.Children.Remove(grid.Children[0]);
                            }
                            MH.AutoPopulation(DG.Scores);
                        }));
                    }
                    else
                    {
                        //nothing
                    }
                    System.Threading.Thread.Sleep(700);
                }
            });
            
            screenUpdater.Start();
            this.Topmost = true;

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ((Border)grid.Children[2]).Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        

        private void btnGutter_Click(object sender, RoutedEventArgs e)
        {
            int activeFrame = MH.FindActiveFrame();

            Label lab = (Label)((Border)grid.Children[activeFrame]).Child;
            MH.SetScore(lab, "-");
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            int activeFrame = MH.FindActiveFrame();

            Label lab = (Label)((Border)grid.Children[activeFrame]).Child;
            MH.SetScore(lab, "1");
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            int activeFrame = MH.FindActiveFrame();

            Label lab = (Label)((Border)grid.Children[activeFrame]).Child;
            MH.SetScore(lab, "2");
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            int activeFrame = MH.FindActiveFrame();

            Label lab = (Label)((Border)grid.Children[activeFrame]).Child;
            MH.SetScore(lab, "3");
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            int activeFrame = MH.FindActiveFrame();

            Label lab = (Label)((Border)grid.Children[activeFrame]).Child;
            MH.SetScore(lab, "4");
        }

        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            int activeFrame = MH.FindActiveFrame();

            Label lab = (Label)((Border)grid.Children[activeFrame]).Child;
            MH.SetScore(lab, "5");
        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {
            int activeFrame = MH.FindActiveFrame();

            Label lab = (Label)((Border)grid.Children[activeFrame]).Child;
            MH.SetScore(lab, "6");
        }

        private void btn7_Click(object sender, RoutedEventArgs e)
        {
            int activeFrame = MH.FindActiveFrame();

            Label lab = (Label)((Border)grid.Children[activeFrame]).Child;
            MH.SetScore(lab, "7");
        }

        private void btn8_Click(object sender, RoutedEventArgs e)
        {
            int activeFrame = MH.FindActiveFrame();

            Label lab = (Label)((Border)grid.Children[activeFrame]).Child;
            MH.SetScore(lab, "8");
        }

        private void btn9_Click(object sender, RoutedEventArgs e)
        {
            int activeFrame = MH.FindActiveFrame();

            Label lab = (Label)((Border)grid.Children[activeFrame]).Child;
            MH.SetScore(lab, "9");
        }

        private void btnSpare_Click(object sender, RoutedEventArgs e)
        {
            int activeFrame = MH.FindActiveFrame();

            Label lab = (Label)((Border)grid.Children[activeFrame]).Child;
            if(lab.Content!=null&&lab.Content.ToString().Length>2)
                MH.SetScore(lab, "/");
            else
            { /*Do nothing because spares are not valid on first throw*/ }

            }

        private void btnStrike_Click(object sender, RoutedEventArgs e)
        {
            int activeFrame = MH.FindActiveFrame();

            Label lab = (Label)((Border)grid.Children[activeFrame]).Child;
            if (lab.Content == null||lab.Content.ToString().Length < 2)
                MH.SetScore(lab, "X    ");
            else
            { /*Do nothing because strikes are not valid on second throw*/ }
            
        }

        private void btnAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int r = random.Next(255);
            int g = random.Next(255);
            int b = random.Next(255);
            MainWindow1.Background = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
        }


        #endregion

        private void btnAddPlayer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        private void btnAddPlayer_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow1.Background = new ImageBrush(new BitmapImage(new Uri(@"http://i.imgur.com/FJYL74J.png")));
        }
    }
}
