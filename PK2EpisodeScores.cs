using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PK2SDK
{
    class PK2EpisodeScores
    {
        private int EPISODE_MAX_LEVELS = 50;
        public const int EPISODE_PLAYER_NAME_LENGTH = 20;

        public int[] BestScores;
        public int[] BestTimes;

        public List<char[]> TopPlayer = new List<char[]>();
        public List<char[]> FastestPlayer = new List<char[]>();

        public int EpisodeTopScore;
        public char[] EpisodeTopPlayer = new char[EPISODE_PLAYER_NAME_LENGTH];

        public bool load(String file)
        {
            bool ok = true;

            if (!File.Exists(file))
            {
                return false;
            }

            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open)))
                {
                    byte[] version = reader.ReadBytes(4);

                    if (version[0] != (char)'1' || version[1] != (char)'.' || version[2] != (char)'0' || version[3] != 0x0)
                    {
                        return false;
                    }

                    BestScores = new int[EPISODE_MAX_LEVELS];
                    BestTimes = new int[EPISODE_MAX_LEVELS];
                    EpisodeTopPlayer = new char[EPISODE_PLAYER_NAME_LENGTH];

                    TopPlayer.Clear();
                    FastestPlayer.Clear();

                    for (int i = 0; i < EPISODE_MAX_LEVELS; i++)
                    {
                        BestScores[i] = reader.ReadInt32();
                    }

                    for (int i = 0; i < EPISODE_MAX_LEVELS; i++)
                    {
                        char[] tmp = new char[EPISODE_PLAYER_NAME_LENGTH];

                        for (int j = 0; j < EPISODE_PLAYER_NAME_LENGTH; j++)
                        {
                            tmp[j] = (char)reader.ReadByte();
                        }

                        TopPlayer.Add(tmp);
                    }

                    for (int i = 0; i < EPISODE_MAX_LEVELS; i++)
                    {
                        BestTimes[i] = reader.ReadInt32();
                    }

                    for (int i = 0; i < EPISODE_MAX_LEVELS; i++)
                    {
                        char[] tmp = new char[EPISODE_PLAYER_NAME_LENGTH];

                        for (int j = 0; j < EPISODE_PLAYER_NAME_LENGTH; j++)
                        {
                            tmp[j] = (char)reader.ReadByte();
                        }

                        FastestPlayer.Add(tmp);
                    }

                    EpisodeTopScore = reader.ReadInt32();

                    for (int i = 0; i < EPISODE_PLAYER_NAME_LENGTH; i++)
                    {
                        EpisodeTopPlayer[i] = (char)reader.ReadByte();
                    }
                }
            } catch (Exception e)
            {
                ok = false;

                throw;
            }

            return ok;
        }

        public bool save(String file)
        {
            bool ok = false;

            /*
            try
            {*/
                using (BinaryWriter w = new BinaryWriter(File.Open(file, FileMode.Create)))
                {
                    char[] version = { '1', '.', '0', (char) 0 };

                    w.Write(version);

                    for (int i = 0; i < EPISODE_MAX_LEVELS; i++)
                    {
                        w.Write((int) BestScores[i]);
                    }

                    for (int i = 0; i < TopPlayer.Count; i++)
                    {
                        for (int j = 0; j < 20; j++)
                        {
                            w.Write((byte) TopPlayer[i][j]);
                        }
                    }

                    for (int i = 0; i < EPISODE_MAX_LEVELS; i++)
                    {
                        w.Write((int) BestTimes[i]);
                    }

                    for (int i = 0; i < EPISODE_MAX_LEVELS; i++)
                    {
                        for (int j = 0; j < EPISODE_PLAYER_NAME_LENGTH; j++)
                        {
                            w.Write((byte) FastestPlayer[i][j]);
                        }
                    }

                    w.Write((int) EpisodeTopScore);

                    for (int i = 0; i < EPISODE_PLAYER_NAME_LENGTH; i++)
                    {
                        w.Write((byte) EpisodeTopPlayer[i]);
                    }

                    w.Flush();

                    ok = true;
                }
            /*} catch (Exception e)
            {
                ok = false;

                throw;
            }*/

            return ok;
        }

        public String GetEpisodeTopPlayer()
        {
            return clean(EpisodeTopPlayer);
        }

        public String GetFastestPlayer(int index)
        {
            return clean(FastestPlayer[index]);
        }

        public String GetTopPlayer(int index)
        {
            return (index >= 0 && index < TopPlayer.Count) ? clean(TopPlayer[index]) : "ERROR";
        }

        public void setLevelLimit(int limit)
        {
            EPISODE_MAX_LEVELS = limit;

            BestScores = new int[EPISODE_MAX_LEVELS];
            BestTimes = new int[EPISODE_MAX_LEVELS];
        }

        public void SetTopPlayer(int index, String player)
        {
            TopPlayer[index] = new char[EPISODE_PLAYER_NAME_LENGTH];

            for (int i = 0; i < player.ToCharArray().Length; i++)
            {
                TopPlayer[index][i] = player.ToCharArray()[i];
            }

            if (player.Length < EPISODE_PLAYER_NAME_LENGTH)
            {
                for (int i = player.Length; i < EPISODE_PLAYER_NAME_LENGTH; i++)
                {
                    TopPlayer[index][i] = (char) 0x0;
                }
            }
        }

        public void SetFastestPlayer(int index, String player)
        {
            FastestPlayer[index] = new char[EPISODE_PLAYER_NAME_LENGTH];

            for (int i = 0; i < player.ToCharArray().Length; i++)
            {
                FastestPlayer[index][i] = player.ToCharArray()[i];
            }

            if (player.Length < EPISODE_PLAYER_NAME_LENGTH)
            {
                for (int i = player.Length; i < EPISODE_PLAYER_NAME_LENGTH; i++)
                {
                    FastestPlayer[index][i] = (char) 0x0;
                }
            }
        }

        public void SetEpisodeTopPlayer(String player)
        {
            EpisodeTopPlayer = new char[EPISODE_PLAYER_NAME_LENGTH];

            for (int i = 0; i < player.ToCharArray().Length; i++)
            {
                EpisodeTopPlayer[i] = player.ToCharArray()[i];
            }

            if (player.Length < EPISODE_PLAYER_NAME_LENGTH)
            {
                for (int i = player.Length; i < EPISODE_PLAYER_NAME_LENGTH; i++)
                {
                    EpisodeTopPlayer[i] = (char) 0x0;
                }
            }
        }

        public int getLevelLimit()
        {
            return EPISODE_MAX_LEVELS;
        }

        public String clean(char[] c)
        {
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] != 0x0)
                {
                    s.Append(c[i]);
                } else
                {
                    break;
                }
            }

            return s.ToString();
        }
    }
}
