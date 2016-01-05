using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApplication7.Code
{
    public class MainHelper
    {
        Grid grid = new Grid();
        string GUID = Guid.NewGuid().ToString();
        string gameGUID = Guid.NewGuid().ToString();
        Dictionary<string, int> challenge = new Dictionary<string, int>();
        public MainHelper(Grid grid, Dictionary<string, int> challenge)
        {
            this.challenge = challenge;
            this.grid = grid;
        }

        public void ClearGrid()
        {
            int count = grid.Children.Count;
            for (int i = 0; i < count; i++)
                grid.Children.RemoveAt(0);
        }

        public void Populate(int NumberOfPlayers)
        {
            ClearGrid();
            for (int o = 0; o < NumberOfPlayers; o++)
            {
                for (int i = 0; i < 12; i++)
                {

                    var rectangle = GetRectangle(NumberOfPlayers);
                    if (o % 2 == 0)
                    {
                        rectangle.Background = new SolidColorBrush(Colors.Gray);
                    }
                    
                    rectangle.Child = new Label();

                    rectangle.Margin = new Thickness(0, 0, grid.Width - ((grid.Width / 12) * (i * 2)), ((grid.Height + rectangle.Height) - ((rectangle.Height * 2) * (o + 1))));
                    grid.Children.Add(rectangle);
                }
            }
        }

        public Border GetRectangle(int NumberOfPlayers)
        {
            Border rec = new Border();
            rec.Width = (grid.Width / 12);
            rec.Height = grid.Height / NumberOfPlayers;
            rec.BorderThickness = new Thickness(2, 2, 2, 2);

            rec.Background = new SolidColorBrush(Colors.AliceBlue);
            rec.MouseLeftButtonDown += Rec_MouseLeftButtonDown;
            return rec;
        }

        public void NextPlayer()
        {
            for (int i = 0; i < grid.Children.Count; i++)
            {
                if (((Border)grid.Children[i]).Background.ToString() == "#FFADD8E6")
                {
                    ResetColors();
                    if (i + 12 < grid.Children.Count)
                        ((Border)grid.Children[i + 12]).Background = new SolidColorBrush(Colors.LightBlue);
                    else
                    {
                        while (i > 12)
                            i = i - 12;
                        ((Border)grid.Children[i + 1]).Background = new SolidColorBrush(Colors.LightBlue);
                    }
                    break;
                }
            }
        }

        public void ResetColors()
        {
            //DC 12/16/2015 Counts number of white and gray to see what color has less to recolor last selected correcly
            int white = 0;
            int gray = 0;
            foreach (Border rec in grid.Children)
            {
                if (rec.Background.ToString() == "#FFF0F8FF")
                {
                    white++;
                }
                if (rec.Background.ToString() == "#FF808080")
                {
                    gray++;
                }
            }

            if (gray > white + 1)
                gray = gray / 2;

            foreach (Border rec in grid.Children)
            {
                if (rec.Background.ToString() == "#FFADD8E6")
                {
                    if (white < gray)
                        rec.Background = new SolidColorBrush(Colors.AliceBlue);
                    else
                        rec.Background = new SolidColorBrush(Colors.Gray);
                    break;
                }
            }
        }

        public int FindActiveFrame()
        {
            for (int i = 0; i < grid.Children.Count; i++)
            {
                if (((Border)grid.Children[i]).Background.ToString() == "#FFADD8E6")
                {
                    return i;
                }
            }
            return 0;
        }

        public void SetScore(Label lab, string number)
        {
            lab.FontSize = 40;
            lab.Content += string.Format(" {0} ", number);
            if (lab.Content.ToString().Length > 5)
            {
                CalculateScore(lab);
                NextPlayer();
            }
        }

        public void CalculateScore(Label lab)
        {
            int activeFrame = FindActiveFrame();
            int activeRow = 0;
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (activeFrame > i * 12 && activeFrame < (i + 1) * 12)
                {
                    activeRow = i;
                    break;
                }
            }

            char[] throws = new char[30];
            int throwCount = 0;
            for (int i = (activeRow * 12); i < (activeRow * 12) + 11; i++)
            {
                if (i == (activeRow * 12)||i-1 == (activeRow * 12))
                    continue;
                string content = null;
                if (((Label)((Border)grid.Children[i]).Child).Content != null)
                    content = ((Label)((Border)grid.Children[i]).Child).Content.ToString();
                if (content != null)
                {
                    if (content.Length > 4)
                    {
                        foreach (char character in content.Substring(0, 5))
                        {
                            if (character != ' ')
                            {
                                throws[throwCount] = character;
                                throwCount++;
                                if (character == 'X')
                                    throwCount++;
                                if (i != (activeRow * 12) + 11)
                                    if (throwCount % 2 == 0)
                                        break;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < 29; i++)
            {
                if(throws[i]=='\0')
                {
                    for (int o = 0; o < 29 - i; o++)
                        throws[i + o] = throws[i + o + 1];
                }
            }
                    
            int total = 0;
            int throwCounter = 0;
            for (int i = 2; i < 12; i++)
            {
                if (Convert.ToInt32(throws[throwCounter]) == 0)
                    break;
                #region Strike Condition
                if (throws[throwCounter].ToString().Equals("X"))
                {
                    int xthrow1 = 0;
                    int xthrow2 = 0;
                    //throwCounter++;
                    if (Convert.ToInt32(throws[throwCounter + 1]) != 0 && Convert.ToInt32(throws[throwCounter + 2]) != 0)
                    {
                        switch(throws[throwCounter + 1].ToString())
                        {
                            case "X":
                                xthrow1 = 10;
                                break;
                            case "-":
                                xthrow1 = 0;
                                break;
                            default:
                                xthrow1 = Convert.ToInt32(throws[throwCounter + 1].ToString());
                                break;
                        }

                        switch (throws[throwCounter + 2].ToString())
                        {
                            case "X":
                                xthrow2 = 10;
                                break;
                            case "-":
                                xthrow2 = 0;
                                break;
                            case "/":
                                xthrow2 = 10 - xthrow1;
                                break;
                            default:
                                xthrow2 = Convert.ToInt32(throws[throwCounter + 2].ToString());
                                break;
                        }
                        total += xthrow1 + xthrow2 + 10;
                        ((Label)((Border)grid.Children[i + (activeRow * 12)]).Child).Content = string.Format(" {0} \n{1}", "X", total);
                        throwCounter++;
                        continue;
                    }
                    else
                        break;
                }
                #endregion
                #region Spare Condition
                if (throws[throwCounter+1].ToString().Equals("/"))
                {
                    int xthrow1 = 0;
                    int xthrow2 = 0;
                    if (Convert.ToInt32(throws[throwCounter + 1]) != 0 && Convert.ToInt32(throws[throwCounter + 2]) != 0)
                    {
                        xthrow1 = Convert.ToInt32(throws[throwCounter].ToString());
                        switch (throws[throwCounter + 2].ToString())
                        {
                            case "X":
                                xthrow2 = 10;
                                break;
                            case "-":
                                xthrow2 = 0;
                                break;
                            default:
                                xthrow2 = Convert.ToInt32(throws[throwCounter + 2].ToString());
                                break;
                        }
                        
                        total += xthrow2 + 10;
                        ((Label)((Border)grid.Children[i + (activeRow * 12)]).Child).Content = string.Format(" {0} {1} \n{2}", xthrow1, "/", total);
                        throwCounter+=2;
                        continue;
                    }
                    else
                        break;
                }
                #endregion
                int throw1 = Convert.ToInt32(throws[throwCounter].ToString());
                throwCounter++;
                int throw2 = Convert.ToInt32(throws[throwCounter].ToString());
                throwCounter++;
                total += throw1 + throw2;

                ((Label)((Border)grid.Children[i + (activeRow * 12)]).Child).Content = string.Format(" {0} {1} \n{2}", throw1, throw2, total);
                
            }
            
        }

        public void AddPlayer(string name, string challengeScore)
        {
            int cellCount = grid.Children.Count;
            int numberOfRows = cellCount / 12;
            List<string> names = new List<string>();
            for (int i = 1; i < cellCount; i += 12)
            {
                if (((Label)((Border)grid.Children[i]).Child).Content != null)
                    names.Add(((Label)((Border)grid.Children[i]).Child).Content.ToString());
            }
            names.Add(name);
            Populate(numberOfRows + 1);
            cellCount = grid.Children.Count;
            int nameCounter = 0;
            for (int i = 1; i < cellCount; i += 12)
            {
                if (i / 12 > names.Count)
                    break;
                //DC: this is where the challenge message is getting duplicated 
                ((Label)((Border)grid.Children[i]).Child).Content = string.Format("{0}\nCS:{1}",names[nameCounter],challengeScore);
                nameCounter++;
            }
        }

        public void AutoCalculateScore()
        {
            int activeFrame = FindActiveFrame();

            for (int activeRow = 0; activeRow < grid.Children.Count / 12; activeRow++)
            {


                char[] throws = new char[30];
                int throwCount = 0;
                for (int i = (activeRow * 12); i < (activeRow * 12) + 11; i++)
                {
                    if (i == (activeRow * 12) || i - 1 == (activeRow * 12))
                        continue;
                    string content = null;
                    if (((Label)((Border)grid.Children[i]).Child).Content != null)
                        content = ((Label)((Border)grid.Children[i]).Child).Content.ToString();
                    if (content != null)
                    {
                        if (content.Length > 4)
                        {
                            foreach (char character in content.Substring(0, 5))
                            {
                                if (character != ' ')
                                {
                                    throws[throwCount] = character;
                                    throwCount++;
                                    if (character == 'X')
                                        throwCount++;
                                    if (i != (activeRow * 12) + 12)
                                        if (throwCount % 2 == 0)
                                            break;
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < 29; i++)
                {
                    if (throws[i] == '\0')
                    {
                        for (int o = 0; o < 29 - i; o++)
                            throws[i + o] = throws[i + o + 1];
                    }
                }

                int total = 0;
                int throwCounter = 0;
                for (int i = 2; i < 12; i++)
                {
                    if (Convert.ToInt32(throws[throwCounter]) == 0)
                        break;
                    #region Strike Condition
                    if (throws[throwCounter].ToString().Equals("X"))
                    {
                        int xthrow1 = 0;
                        int xthrow2 = 0;
                        //throwCounter++;
                        if (Convert.ToInt32(throws[throwCounter + 1]) != 0 && Convert.ToInt32(throws[throwCounter + 2]) != 0)
                        {
                            switch (throws[throwCounter + 1].ToString())
                            {
                                case "X":
                                    xthrow1 = 10;
                                    break;
                                case "-":
                                    xthrow1 = 0;
                                    break;
                                default:
                                    xthrow1 = Convert.ToInt32(throws[throwCounter + 1].ToString());
                                    break;
                            }

                            switch (throws[throwCounter + 2].ToString())
                            {
                                case "X":
                                    xthrow2 = 10;
                                    break;
                                case "-":
                                    xthrow2 = 0;
                                    break;
                                case "/":
                                    xthrow2 = 10 - xthrow1;
                                    break;
                                default:
                                    xthrow2 = Convert.ToInt32(throws[throwCounter + 2].ToString());
                                    break;
                            }
                            total += xthrow1 + xthrow2 + 10;
                            ((Label)((Border)grid.Children[i + (activeRow * 12)]).Child).Content = string.Format(" {0} \n{1}", "X", total);
                            throwCounter++;
                            continue;
                        }
                        else
                            break;
                    }
                    #endregion
                    #region Spare Condition
                    if (throws[throwCounter + 1].ToString().Equals("/"))
                    {
                        int xthrow1 = 0;
                        int xthrow2 = 0;
                        if (Convert.ToInt32(throws[throwCounter + 1]) != 0 && Convert.ToInt32(throws[throwCounter + 2]) != 0)
                        {
                            xthrow1 = Convert.ToInt32(throws[throwCounter].ToString());
                            switch (throws[throwCounter + 2].ToString())
                            {
                                case "X":
                                    xthrow2 = 10;
                                    break;
                                case "-":
                                    xthrow2 = 0;
                                    break;
                                default:
                                    xthrow2 = Convert.ToInt32(throws[throwCounter + 2].ToString());
                                    break;
                            }

                            total += xthrow2 + 10;
                            ((Label)((Border)grid.Children[i + (activeRow * 12)]).Child).Content = string.Format(" {0} {1} \n{2}", xthrow1, "/", total);
                            throwCounter += 2;
                            continue;
                        }
                        else
                            break;
                    }
                    #endregion
                    int throw1 = Convert.ToInt32(throws[throwCounter].ToString());
                    throwCounter++;
                    int throw2 = 0;
                    if (Convert.ToInt32(throws[throwCounter]) == 0)
                        throw2 = 0;
                    else
                        throw2 = Convert.ToInt32(throws[throwCounter].ToString());
                    throwCounter++;
                    total += throw1 + throw2;
                    //DC: This might be the cause of the flashing
                    ((Label)((Border)grid.Children[i + (activeRow * 12)]).Child).Content = string.Format(" {0} {1} \n{2}", throw1, throw2, total);

                }
            }
        }

        public void AutoPopulation(List<Player> Scores)
        {
            if (Scores.Count == 0)
                return;
            int place = 0;
            foreach (var player in Scores)
            {
                int challegeScore = 0;
                for(int i =0;i<Scores[place].Throws.Count;i++)
                {
                    foreach (string key in challenge.Keys)
                    {
                        if (Scores[place].Throws[i].Contains(key) ||
                            key != "X" &&
                            ((Scores[place].Throws[i].Contains("/") && Scores[place].Throws[i-1].Contains((10- Convert.ToInt32(key)).ToString())))
                            )
                        {
                            challegeScore += challenge[key];
                        }
                    }
                }

                AddPlayer(player.Name, challegeScore.ToString());
                place++;
            }
            int playerCounter = 2;
            foreach (var player in Scores)
            {
                int throwCounter = 0;
                for (int i = playerCounter; i < (playerCounter+10); i++)
                {
                    if (i + 1 >= (playerCounter + 10))
                    {
                        ((Label)((Border)grid.Children[i]).Child).FontSize = 20;
                        ((Label)((Border)grid.Children[i]).Child).Content = string.Format(" {0}  {1}  {2}", player.Throws[throwCounter], player.Throws[throwCounter + 1], player.Throws[throwCounter + 2]);
                    }
                    else
                    {
                        ((Label)((Border)grid.Children[i]).Child).FontSize = 40;
                        ((Label)((Border)grid.Children[i]).Child).Content = string.Format(" {0}  {1} ", player.Throws[throwCounter], player.Throws[throwCounter + 1]);
                        throwCounter += 2;
                    }
                }
                playerCounter += 12;
            }
            List<int> activeFinder = new List<int>();
            int counter = 0;
            foreach (var player in Scores)
            {
                activeFinder.Add(0);
                for (int i=0;i<player.Throws.Count;i++)
                {
                    if(player.Throws[i] == "" && player.Throws[i-1]!="X")
                    {
                        activeFinder[counter]++;
                    }
                }
                counter++;
            }
            int last = activeFinder[0];
            bool changed = false;
            for(int i =0;i< activeFinder.Count;i++)
            {
                if(activeFinder[i]> last)
                {
                    changed = true;
                    last = i;
                    break;
                }
            }
            if (!changed)
                last = 0;
            for (int i =2;i<12;i++)
            {
                if (((Label)((Border)grid.Children[(last * 12) + i]).Child).Content == null || ((Label)((Border)grid.Children[(last * 12) + i]).Child).Content.ToString().Trim() == "")
                {
                    ((Border)grid.Children[(last * 12) + i]).Background = new SolidColorBrush(Colors.LightBlue);
                    break;
                }
            }

            string contentText = ((Label)((Border)grid.Children[(Scores.Count * 12) - 1]).Child).Content.ToString().Replace(" ", "");
            if ((contentText.Contains("X") && contentText.Length == 3) || (contentText.Contains("/") && contentText.Length == 3)
                    || ((!contentText.Contains("X") && !contentText.Contains("/")) && contentText.Length == 2))
            {
                XMLReadWrite XRW = new XMLReadWrite();
                XRW.XMLWrite(Scores, GUID, gameGUID);
            }
            if(Scores[0].Throws.Count<2)
            {
                gameGUID = Guid.NewGuid().ToString();
            }
            
            

            AutoCalculateScore();
        }

        public void Rec_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResetColors();

            Border rectangle = (Border)sender;
            ((Label)(rectangle.Child)).Content = null;
            rectangle.Background = new SolidColorBrush(Colors.LightBlue);
        }
    }
}
