using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gravitas
{
    /// <summary>
    /// <Author>Andrew Giannopoulos</Author>
    /// <Description>Static class for managing Level data IO</Description>
    /// </summary>
    public static class LevelIO
    {
        /// <summary>
        /// Saves level data from the playingGameState into the xml document specified by a_levelName
        /// </summary>
        /// <param name="a_levelName">The string that contains the name of the file the level data will be saved to.</param>
        /// <param name="a_state">The playingGameState from which the level data is retrieved and saved</param>
        public static void SaveLevel(String a_levelName, PlayingGameState a_state)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement rootElement = doc.CreateElement("LevelData");
            XmlElement platforms = doc.CreateElement("Platforms");
            XmlElement turrets = doc.CreateElement("Turrets");

            for (int i = 0; i < a_state.m_platformList.Count; i++)
            {
                XmlElement platform = doc.CreateElement("Platform");
                XmlAttribute x1 = doc.CreateAttribute("x1");
                XmlAttribute y1 = doc.CreateAttribute("y1");
                XmlAttribute x2 = doc.CreateAttribute("x2");
                XmlAttribute y2 = doc.CreateAttribute("y2");

                x1.Value = a_state.m_platformList[i].m_leftPoint.X.ToString();
                y1.Value = a_state.m_platformList[i].m_leftPoint.Y.ToString();
                x2.Value = a_state.m_platformList[i].m_rightPoint.X.ToString();
                y2.Value = a_state.m_platformList[i].m_rightPoint.Y.ToString();

                platform.Attributes.Append(x1);
                platform.Attributes.Append(y1);
                platform.Attributes.Append(x2);
                platform.Attributes.Append(y2);

                platforms.AppendChild(platform);
            }

            for (int i = 0; i < a_state.m_turretList.Count; i++)
            {
                XmlElement turret = doc.CreateElement("Turret");
                XmlAttribute xPos = doc.CreateAttribute("xPos");
                XmlAttribute yPos = doc.CreateAttribute("yPos");
                XmlAttribute rotation = doc.CreateAttribute("rotation");

                xPos.Value = a_state.m_turretList[i].m_base.Position.X.ToString();
                yPos.Value = a_state.m_turretList[i].m_base.Position.Y.ToString();
                rotation.Value = a_state.m_turretList[i].m_base.m_body.Rotation.ToString();

                turret.Attributes.Append(xPos);
                turret.Attributes.Append(yPos);
                turret.Attributes.Append(rotation);

                turrets.AppendChild(turret);
            }

            XmlElement playerSpawn = doc.CreateElement("PlayerSpawn");

            XmlAttribute playerLocationX = doc.CreateAttribute("px");
            XmlAttribute playerLocationY = doc.CreateAttribute("py");

            playerLocationX.Value = a_state.m_playerSpawnLocation.X.ToString();
            playerLocationY.Value = a_state.m_playerSpawnLocation.Y.ToString();

            playerSpawn.Attributes.Append(playerLocationX);
            playerSpawn.Attributes.Append(playerLocationY);

            XmlElement targetGoal = doc.CreateElement("Goal");

            XmlAttribute goalX = doc.CreateAttribute("gx");
            XmlAttribute goalY = doc.CreateAttribute("gy");

            goalX.Value = a_state.m_goalLocation.X.ToString();
            goalY.Value = a_state.m_goalLocation.Y.ToString();

            targetGoal.Attributes.Append(goalX);
            targetGoal.Attributes.Append(goalY);

            //goalX.Value = a_state.m_goal.Position.X.ToString();
            //goalY.Value = a_state.m_goal.Position.Y.ToString();

            rootElement.AppendChild(playerSpawn);
            rootElement.AppendChild(targetGoal);
            rootElement.AppendChild(platforms);
            rootElement.AppendChild(turrets);

            doc.AppendChild(rootElement);

            doc.Save(a_levelName);

        }

        /// <summary>
        /// Loads level data from the specified file and generates platform, turret, player location, and endpoint data in the playingGameState
        /// </summary>
        /// <param name="a_levelName">The file that the data is to be loaded from</param>
        /// <param name="a_state">The playingGameState that the data is to be loaded to</param>
        public static void LoadLevel(String a_levelName, PlayingGameState a_state)
        {

            for (int i = 0; i < a_state.m_platformList.Count; i++)
            {
                a_state.m_world.RemoveBody(a_state.m_platformList[i].m_sprite.m_body);
                a_state.m_world.RemoveBody(a_state.m_platformList[i].m_connection.m_body);
                a_state.m_world.RemoveBody(a_state.m_platformList[i].m_connection2.m_body);
                a_state.m_platformList.RemoveAt(i--);
            }

            for (int i = 0; i < a_state.m_turretList.Count; i++)
            {
                a_state.m_world.RemoveBody(a_state.m_turretList[i].m_base.m_body);
                a_state.m_world.RemoveBody(a_state.m_turretList[i].m_top.m_body);
                a_state.m_turretList.RemoveAt(i--);
            }

            a_state.m_goalLocation = new Vector2(0, 0);
            a_state.m_playerSpawnLocation = new Vector2(0, 0);

            XmlDocument xml = new XmlDocument();

#if PSM
            xml.Load(@"Application\Content\" + a_levelName);
#else
            xml.Load(a_levelName);
#endif

            XmlElement levelData = xml.DocumentElement;

            XmlElement playerSpawn = levelData.FirstChild as XmlElement;
            XmlElement goalLocation = playerSpawn.NextSibling as XmlElement;
            XmlElement platformList = goalLocation.NextSibling as XmlElement;
            XmlElement turretList = platformList.NextSibling as XmlElement;

            XmlElement platformElement = platformList.FirstChild as XmlElement;
            while (platformElement != null)
            {
                Vector2 leftPoint = new Vector2(0, 0);
                Vector2 rightPoint = new Vector2(0, 0);

                XmlAttribute x1 = platformElement.Attributes["x1"];
                XmlAttribute y1 = platformElement.Attributes["y1"];
                XmlAttribute x2 = platformElement.Attributes["x2"];
                XmlAttribute y2 = platformElement.Attributes["y2"];

                try
                {
                    float.TryParse(x1.Value, out leftPoint.X);
                    float.TryParse(y1.Value, out leftPoint.Y);
                    float.TryParse(x2.Value, out rightPoint.X);
                    float.TryParse(y2.Value, out rightPoint.Y);

                    a_state.m_platformList.Add(new Platform(a_state.m_world, leftPoint, rightPoint, ContentLibrary.platformTexture));
                }
                catch (Exception)
                {
                    // failed to load XML
                    Console.WriteLine("Failed to load platforms");
                }

                platformElement = platformElement.NextSibling as XmlElement;
            }

            XmlElement turretElement = turretList.FirstChild as XmlElement;
            while (turretElement != null)
            {
                Vector2 position = new Vector2(0, 0);
                float rotation = 0.0f;

                XmlAttribute xPos = turretElement.Attributes["xPos"];
                XmlAttribute yPos = turretElement.Attributes["yPos"];
                XmlAttribute rot = turretElement.Attributes["rotation"];

                try
                {
                    float.TryParse(xPos.Value, out position.X);
                    float.TryParse(yPos.Value, out position.Y);
                    float.TryParse(rot.Value, out rotation);

                    a_state.m_turretList.Add(
                        new Turret(
                            a_state, 
                            ContentLibrary.turretTopTexture, 
                            ContentLibrary.turretBaseTexture, 
                            ContentLibrary.circleTexture, 
                            position, 
                            a_state.m_player, 
                            rotation
                            )
                        );
                }
                catch (Exception)
                {
                    // failed to load XML
                    Console.WriteLine("Failed to load turrets");
                }

                turretElement = turretElement.NextSibling as XmlElement;
            }

            XmlAttribute playerX = playerSpawn.Attributes["px"];
            XmlAttribute playerY = playerSpawn.Attributes["py"];

            try
            {
                float.TryParse(playerX.Value, out a_state.m_playerSpawnLocation.X);
                float.TryParse(playerY.Value, out a_state.m_playerSpawnLocation.Y);
            }
            catch (Exception)
            {
                // failed to load XML
                Console.WriteLine("Failed to load Player spawn location");
            }

            XmlAttribute goalX = goalLocation.Attributes["gx"];
            XmlAttribute goalY = goalLocation.Attributes["gy"];

            try
            {

                float.TryParse(goalX.Value, out a_state.m_goalLocation.X);
                float.TryParse(goalY.Value, out a_state.m_goalLocation.Y);
                //Vector2 goalPos = new Vector2(0, 0);
                //float.TryParse(goalX.Value, out goalPos.X);
                //float.TryParse(goalY.Value, out goalPos.Y);
                //a_state.m_goal.Position = goalPos;
            }
            catch (Exception)
            {
                // failes to load XML
                Console.WriteLine("Failed to load goal location");
            }

        }

        /// <summary>
        /// Saves a list of scores to a specified binary file
        /// </summary>
        /// <param name="a_filename">Name of the file for the scores to be saved to</param>
        /// <param name="a_scores">List of scores to be saved to the file</param>
        public static void SaveScore(String a_filename, List<Score> a_scores)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream fStream = new FileStream(a_filename, FileMode.OpenOrCreate);

            foreach (Score s in a_scores)
            {
                formatter.Serialize(fStream, s);
            }

            fStream.Close();
            fStream.Dispose();
        }

        /// <summary>
        /// Loads a list of scores from the specified file
        /// </summary>
        /// <param name="a_filename">The name of the file that the scores are to be loaded from</param>
        /// <returns>The list of loaded scores.</returns>
        public static List<Score> LoadScore(String a_filename)
        {
            List<Score> output = new List<Score>();

            IFormatter formatter = new BinaryFormatter();

            try
            {
                Stream fStream = new FileStream(a_filename, FileMode.Open);
                while (fStream.Position < fStream.Length)
                {
                    output.Add((Score)formatter.Deserialize(fStream));
                }

                fStream.Close();
                fStream.Dispose();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Binary file not found");
            }

            return output;
        }
    }
}
