using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PK2SDK;

namespace PK2ScoreEditor
{
    public partial class Form1 : Form
    {
        PK2EpisodeScores scores = new PK2EpisodeScores();
        String file;

        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            scores.SetEpisodeTopPlayer(textBox1.Text);
            scores.EpisodeTopScore = (int) numericUpDown1.Value;

            scores.TopPlayer[0][0] = (char) 0x20;
            scores.FastestPlayer[0][0] = (char) 0x20;

            for (int i = 1; i < 20; i++)
            {
                scores.TopPlayer[0][i] = (char) 0x0;
                scores.FastestPlayer[0][i] = (char) 0x0;
            }

            for (int i = 1; i < dataGridView1.Rows.Count; i++)
            {
                scores.SetTopPlayer(i, dataGridView1.Rows[i - 1].Cells[0].Value.ToString());
                scores.BestScores[i] = Convert.ToInt32(dataGridView1.Rows[i - 1].Cells[1].Value);

                scores.SetFastestPlayer(i, dataGridView2.Rows[i - 1].Cells[0].Value.ToString());
                scores.BestTimes[i] = Convert.ToInt32(dataGridView2.Rows[i - 1].Cells[1].Value);
            }

            Console.WriteLine(scores.TopPlayer.Count);

            bool k = scores.save(file);

            if (k)
            {
                Console.WriteLine("OK");
            } else
            {
                Console.WriteLine("NAH!");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripComboBox1.SelectedIndex = 0;
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox1.SelectedIndex == 0)
            {
                scores.setLevelLimit(50);
            } else
            {
                scores.setLevelLimit(100);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DialogResult r = openFileDialog1.ShowDialog();

            if (r == DialogResult.OK)
            {
                bool ok = false;

                try
                {
                    file = openFileDialog1.FileName;

                    ok = scores.load(openFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(System.IO.EndOfStreamException))
                    {
                        MessageBox.Show("Couldn't read file.\nTry to change the version.", "Wrong version");
                    }
                }

                if (ok)
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Refresh();

                    dataGridView2.Rows.Clear();
                    dataGridView2.Refresh();

                    for (int i = 1; i < scores.getLevelLimit(); i++)
                    {
                        dataGridView1.Rows.Add(scores.GetTopPlayer(i));
                        dataGridView1.Rows[i - 1].Cells[1].Value = scores.BestScores[i].ToString();

                        dataGridView2.Rows.Add(scores.GetFastestPlayer(i));
                        dataGridView2.Rows[i - 1].Cells[1].Value = scores.BestTimes[i].ToString();
                    }

                    textBox1.Text = new String(scores.EpisodeTopPlayer);
                    numericUpDown1.Value = (int) scores.EpisodeTopScore;
                }
            }
        }
    }
}
