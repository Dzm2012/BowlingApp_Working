using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Windows.Automation;
using System.Text.RegularExpressions;
using OpenQA;
using OpenQA.Selenium.Chrome;
using Selenium.Webdriver.Domify;
using OpenQA.Selenium;

namespace WpfApplication7.Code
{
    public class XBowlingDataGrabber
    {
        public List<Player> Scores = new List<Player>();
        ChromeDriver driver = null;
        int laneNumber = 0;
        public XBowlingDataGrabber(int laneNumber)
        {
            this.laneNumber = laneNumber;
            GetData();
        }
        public void Updater()
        {
            
            
            while (true)
            {
                System.Threading.Thread.Sleep(500);
                GetData();
            }
            
        }
        public void GetData()
        {
            Scores = new List<Player>();
            string Url = Constiants.DebugLanes + laneNumber;
            if (driver == null)
            {
                driver = new ChromeDriver();
                driver.Navigate().GoToUrl(Url);
                System.Threading.Thread.Sleep(3000);

                var doc = driver.Divs(By.ClassName("container-fluid"))[0];
                string tempHTML = doc.InnerHtml;
                HTMLShredder(tempHTML);
            }
            else
            {
                var doc = driver.Divs(By.ClassName("container-fluid"))[0];
                string tempHTML = doc.InnerHtml;
                HTMLShredder(tempHTML);
            }
        }
        public void HTMLShredder(string html)
        {
            var byName = html.Split(new string[] { "lane-name" },StringSplitOptions.RemoveEmptyEntries);
            for(int i =1;i<byName.Length;i++)
            {
                Scores.Add(new Player(byName[i].Split('>')[1].Split('<')[0]));
                var temp = byName[i].Split(new string[] { "squareScore" },
                    StringSplitOptions.RemoveEmptyEntries);
                foreach(string item in temp)
                {
                    if (item.Contains(Scores[i - 1].Name))
                        continue;
                    Scores[i-1].Throws.Add(item.Split('>')[1].Split('<')[0]);
                }
            }
            
        }
        
    }
}
