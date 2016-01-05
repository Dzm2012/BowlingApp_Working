using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace WpfApplication7.Code
{
    class XMLReadWrite
    {
        public void XMLRead()
        {

        }
        public void XMLWrite(List<Player> scores, string sessionGUID, string gameGUID)
        {
            CheckFile();

            string fileName = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "scoreData.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            var gameCheck = doc.SelectSingleNode("//Game[@GUID='"+ gameGUID + "']");
            if (gameCheck != null)
                return;
            foreach (Player player in scores)
            {
                var playersNode = doc.SelectSingleNode("//Players");
                //check for player name in document
                var playerNode = doc.SelectSingleNode("//Player[@Name='"+ player.Name + "']");
                if (playerNode == null)
                {
                    playerNode = AddPlayer(playersNode, player.Name, doc);
                }
                var sessionNode = AddSession(sessionGUID, playerNode, doc);
                var gameNode = AddGame(sessionNode, doc, gameGUID);

                for (int i = 0; i < player.Throws.Count; i++)
                {
                    XmlNode frameNode = null;
                    if (i == 18)
                    {
                        frameNode = AddScore(gameNode, doc, player.Throws[i], player.Throws[i + 1], player.Throws[i+2]);
                        i += 3;
                    }
                    else
                    {
                        frameNode = AddScore(gameNode, doc, player.Throws[i], player.Throws[i + 1], null);
                        i++;
                    }

                    gameNode.AppendChild(frameNode);
                }
                sessionNode.AppendChild(gameNode);
                playerNode.AppendChild(sessionNode);
                playersNode.AppendChild(playerNode);
                doc.AppendChild(playersNode);
            }
            doc.Save(fileName);
        }

        public void CheckFile()
        {
            string fileName = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "scoreData.xml");
            if (!File.Exists(fileName))
            {
                var doc = new XmlDocument();
                var playersNode = doc.CreateElement("Players");
                doc.AppendChild(playersNode);
                doc.Save(fileName);
            }
        }

        private XmlNode AddPlayer(XmlNode playersNode, string name, XmlDocument doc)
        {
            var playerNode = doc.CreateElement("Player");
            var nameAttribute = doc.CreateAttribute("Name");
            nameAttribute.Value = name;
            playerNode.Attributes.Append(nameAttribute);
            return playerNode;
        }
        private XmlNode AddSession(string sessionGUID, XmlNode playerNode, XmlDocument doc)
        {
            var sessionNode = doc.CreateElement("Session");
            var idAttribute = doc.CreateAttribute("ID");
            idAttribute.Value = sessionGUID;
            sessionNode.Attributes.Append(idAttribute);
            return sessionNode;
        }
        private XmlNode AddGame(XmlNode sessionNode, XmlDocument doc, string gameGUID)
        {
            int totalGameNodes = sessionNode.SelectNodes("//Game").Count;

            var dateNode = doc.CreateElement("Date");
            dateNode.InnerText = DateTime.Now.ToShortDateString();

            var gameNode = doc.CreateElement("Game");

            var numberAttribute = doc.CreateAttribute("Number");
            numberAttribute.Value = (1+totalGameNodes).ToString();
            var guidAttribute = doc.CreateAttribute("GUID");
            guidAttribute.Value = gameGUID;

            gameNode.AppendChild(dateNode);
            gameNode.Attributes.Append(guidAttribute);
            gameNode.Attributes.Append(numberAttribute);

            return gameNode;
        }
        private XmlNode AddScore(XmlNode gameNode, XmlDocument doc, string throw1, string throw2, string throw3)
        {
            int totalFrameNodes = gameNode.SelectNodes("//Frame").Count;
            
            var frameNode = doc.CreateElement("Frame");
            var numberAttribute = doc.CreateAttribute("Number");
            numberAttribute.Value = (1 + totalFrameNodes).ToString();
            frameNode.Attributes.Append(numberAttribute);

            var throw1Node = doc.CreateElement("Throw1");
            throw1Node.InnerText = throw1;
            frameNode.AppendChild(throw1Node);
            var throw2Node = doc.CreateElement("Throw2");
            throw2Node.InnerText = throw2;
            frameNode.AppendChild(throw2Node);
            if (throw3!=null)
            {
                var throw3Node = doc.CreateElement("Throw3");
                throw3Node.InnerText = throw3;
                frameNode.AppendChild(throw3Node);
            }

            return frameNode;
        }

        public void Debug()
        {
            string fileName = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "scoreData.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            var playersNode = doc.SelectSingleNode("//Players");
            var player = doc.CreateElement("Player");
            var nameAttribute = doc.CreateAttribute("Name");
            nameAttribute.Value = "TemplateDebug";//Player Name
            player.Attributes.Append(nameAttribute);

            var sessionNode = doc.CreateElement("Session");
            var idAttribute = doc.CreateAttribute("ID");
            idAttribute.Value = Guid.NewGuid().ToString();//SessionGUID this should be passed so all are the same
            sessionNode.Attributes.Append(idAttribute);

            player.AppendChild(sessionNode);
            playersNode.AppendChild(player);
            doc.AppendChild(playersNode);
            doc.Save(fileName);
        }
    }
}
