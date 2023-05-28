using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using Newtonsoft.Json;

namespace Tune_Player
{
    public partial class Form1 : Form
    {
        public MusicData musicData;

        private bool bool_editButton = false;
        private bool bool_deleteButton = false;

        private Player playerOne;
        private Player playerTwo;



        public Form1()
        {
            //  Initialize the component
            InitializeComponent();

            //  Initialize player
            initPlayers();

            //  Initialize the Form
            this.action_initializeForm1();


        }

        private void initPlayers()
        {
            playerOne = new Player(
                button_playerOneToggle,
                button_playerOneStop,
                button_playerOneAutoplay,
                label_playerOneName, 
                label_playerOneStatus, 
                label_playerOneCountUp, 
                label_playerOneCountDown
                );

            playerTwo = new Player(
                button_playerTwoToggle,
                button_playerTwoStop,
                button_playerTwoAutoplay,
                label_playerTwoName, 
                label_playerTwoStatus, 
                label_playerTwoCountUp, 
                label_playerTwoCountDown
                );
        }

        public string GenerateRandomString(int length = 10)
        {
            Random random = new Random();
            String CharacterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be a positive integer.");

            if (string.IsNullOrEmpty(CharacterSet))
                throw new ArgumentException("Character set cannot be null or empty.", nameof(CharacterSet));

            char[] randomString = new char[length];
            for (int i = 0; i < length; i++)
            {
                randomString[i] = CharacterSet[random.Next(CharacterSet.Length)];
            }

            return new string(randomString);
        }

        
        

        private void action_initializeForm1()
        {
            //  set bounds
            var workingArea = Screen.FromHandle(Handle).WorkingArea;
            MaximizedBounds = new Rectangle(0, 0, workingArea.Width, workingArea.Height);
            WindowState = FormWindowState.Maximized;

            musicData = new MusicData();
            musicData.readData();

            //  Set tabs
            this.action_setPlayerLists();
        }

        private void action_setPlayerLists()
        {
            Console.WriteLine(this.musicData.data().Count);
            if (this.musicData.data().Count > 0)
            {
                action_setPlayerList(flowLayoutPanel_playerListOne, playerOne, this.musicData.data()[0]);
            }

            if (this.musicData.data().Count > 1)
            {
                
            }
           
            
        }

        private void action_setPlayerList(FlowLayoutPanel host, Player player,  PlayerList playerList)
        {
            foreach (MusicGroup musicGroup in playerList.musicGroups)
            {
                Label label = new Label();
                label.Text = musicGroup.title;
                label.Tag = musicGroup.ID;
                host.Controls.Add(label);
                foreach (MusicObject musicObject in musicGroup.musicObjects)
                {
                    ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

                    ToolStripMenuItem menuItemDelete = new ToolStripMenuItem("Delete");
                    ToolStripMenuItem menuItemDeleteConfirm = new ToolStripMenuItem("Confirm");
                    menuItemDeleteConfirm.Click += listener_toolStripMenuItem_Delete_Click;
                    menuItemDeleteConfirm.Tag = musicObject.ID;
                    menuItemDelete.DropDownItems.Add(menuItemDeleteConfirm);
                    contextMenuStrip.Items.Add(menuItemDelete);

                    ToolStripMenuItem menuItemRename = new ToolStripMenuItem("Rename");
                    menuItemRename.Click += listener_toolStripMenuItem_Rename_Click;
                    menuItemRename.Tag = musicObject.ID;
                    contextMenuStrip.Items.Add(menuItemRename);

                    ToolStripMenuItem menuItemPlace = new ToolStripMenuItem("Pick Player");
                    menuItemPlace.Click += listener_toolStripMenuItem_Place_Click;
                    menuItemPlace.Tag = musicObject.ID;
                    contextMenuStrip.Items.Add(menuItemPlace);

                    Button button = new Button();
                    button.Height = 50;
                    button.Width = host.Width - 10;
                    button.Text = musicObject.title;
                    button.Tag = musicObject.file;
                    button.TextAlign = ContentAlignment.MiddleLeft;
                    button.ContextMenuStrip = contextMenuStrip;
                    button.Click += (sender, EventArgs) => { listener_musicButtonClick(sender, EventArgs, player); };
                    
                    host.Controls.Add(button);
                }
            }
        }
        

        private void listener_musicButtonClick(object sender, EventArgs e, Player player)
        {
            Button btn = sender as Button;
            string file = btn?.Tag as string;

            if (file != null)
            {
                player?.Play(btn?.Text, file);
            }
        }

        private void action_setMusicButton(MusicButton button)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Filter = "Media files (*.mp4)|*.mp4|Music files (*.mp3)|*.mp3",
                FilterIndex = 0,
                RestoreDirectory = true
            };

            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Choose button name",
                StartPosition = FormStartPosition.CenterScreen
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String selectedFile = openFileDialog1.FileName;
                Label textLabel = new Label() { Left = 50, Top = 20, Text = "Enter the text that will be shown on the button", Width=300 };
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
                Button confirmation = new Button() { Text = "Save", Left = 200, Width = 100, Top = 90, DialogResult = DialogResult.OK };
                Button cancel = new Button() { Text = "Cancel", Left = 350, Width = 100, Top = 90, DialogResult = DialogResult.Cancel };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(cancel);
                prompt.AcceptButton = confirmation;

                if(prompt.ShowDialog() == DialogResult.OK)
                {
                    String text = textBox.Text == "" ? "no text" : textBox.Text;
                    button.file = selectedFile;
                    button.Text = text;
                    button.set = true;
                    
                    musicData.setButton(button, selectedFile, text);
                    this.action_setPlayerLists();
                }

            }

            
        }
        
        private void listener_toolStripMenuItem_Delete_Click(object sender, EventArgs e)
        {
            return;
        }

        private void listener_toolStripMenuItem_Rename_Click(object sender, EventArgs e)
        {
            return;
        }

        private void listener_toolStripMenuItem_Place_Click(object sender, EventArgs e)
        {
            return;
        }

        
        
    }

    public class MusicButton : Button
    {
        public Button btn { get; set; }
        public int buttonIndex;
        public int tabIndex;
        public bool set;
        public String parameter;
        public String file;
        public bool active1 = false;
        public bool active2 = false;
    }

    public class MusicData
    {
        private List<PlayerList> dataArray { get; set; }
        public String file { get; set; }

        public MusicData()
        {
            this.initFile();
        }

        private void initFile()
        {
            this.file = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\tune_player.json";
            Console.WriteLine(file);
            if(!File.Exists(file))
            {
                dataArray = new List<PlayerList>();
                File.WriteAllText(file, JsonConvert.SerializeObject(dataArray));
            }
        }

        public void readData()
        {
            if(file != null)
            {
                dataArray = new List<PlayerList>();
                String plain_json = File.ReadAllText(file);
                this.dataArray = JsonConvert.DeserializeObject<List<PlayerList>>(plain_json);
                
            }
        }

        public List<PlayerList> data()
        {
            return this.dataArray;
        }

        public void storeData()
        {
            if(file != null && dataArray != null)
            {
                File.Delete(file);
                File.WriteAllText(file, JsonConvert.SerializeObject(dataArray));
            }
        }

        public void setButton(MusicButton button, String file, String title)
        {
            /*PlayerList playerList = dataArray[button.tabIndex];
            MusicObject musicObject = playerList.musicObjects[button.buttonIndex];
            musicObject.title = title;
            musicObject.file = file;
            playerList.musicObjects[button.buttonIndex] = musicObject;
            dataArray[button.tabIndex] = playerList;
            this.storeData();*/
        }

        public void setButtonTitle(MusicButton button, String title)
        {
            /*PlayerList playerList = dataArray[button.tabIndex];
            MusicObject musicObject = playerList.musicObjects[button.buttonIndex];
            musicObject.title = title;
            playerList.musicObjects[button.buttonIndex] = musicObject;
            dataArray[button.tabIndex] = playerList;
            this.storeData();*/
        }

        public void clearButton(MusicButton button)
        {
            /*PlayerList playerList = dataArray[button.tabIndex];
            MusicObject musicObject = playerList.musicObjects[button.buttonIndex];
            musicObject.title = null;
            musicObject.file = null;
            playerList.musicObjects[button.buttonIndex] = musicObject;
            dataArray[button.tabIndex] = playerList;
            this.storeData();*/
        }
    }

    public struct MusicObject
    {
        public String ID { get; set; }
        public String file { get; set; }
        public String title { get; set; }
    }

    public struct PlayerList{
        public List<MusicGroup> musicGroups { get; set; }
    };

    public struct MusicGroup
    {
        public String ID { get; set; }
        public String title { get; set; }
        public List<MusicObject> musicObjects
        {
            get;
            set; 
        }
    }


    public class Player
    {
        private Button _buttonPlayPause;
        private Button _buttonStop;
        private Button _buttonAutoplay;

        private Label _labelName;
        private Label _labelStatus;
        private Label _labelCountUp;
        private Label _labelCountDown;

        public Player(
            Button buttonPlayPause,
            Button buttonStop,
            Button buttonAutoplay,
            Label label_name, 
            Label label_status, 
            Label label_countUp, 
            Label label_countDown)
        {
            _buttonPlayPause = buttonPlayPause;
            _buttonStop = buttonStop;
            _buttonAutoplay = buttonAutoplay;

            _labelName = label_name;
            _labelStatus = label_status;
            _labelCountUp = label_countUp;
            _labelCountDown = label_countDown;


            buttonAutoplay.Click += delegate (object sender, EventArgs e) { ToggleAutoplay(); };
            buttonStop.Click += delegate (object sender, EventArgs e) { Stop(); };
            buttonPlayPause.Click += delegate (object sender, EventArgs e) {Toggle();};


        }
        private WaveOutEvent _outputDevice { get; set; }
        private AudioFileReader _audioFile { get; set; }

        private bool _autoplay = false;

        public void Init()
        {

        }

        public void Play()
        {
            _outputDevice?.Play();
            _labelStatus.Text = "Playing";
        }

        public void Play(String Name, String file)
        {
            Load(Name, file);
        }

        public void Pause()
        {
            _outputDevice?.Pause();
            _labelStatus.Text = "Paused";
        }

        public void Load(String Name, String file)
        {
            Stop();
            _outputDevice = new WaveOutEvent();
            //outputDevice.PlaybackStopped += OnPlaybackStopped;
            _audioFile = new AudioFileReader(@"" + file);
            _outputDevice?.Init(_audioFile);

            _labelCountDown.Text = _audioFile.TotalTime.Minutes.ToString() + ":" + _audioFile.TotalTime.Seconds.ToString();
            _labelCountUp.Text = "00:00";
            _labelName.Text = Name;
            _labelStatus.Text = "Not Playing";

            if (_autoplay)
            {
                Play();
            }

            
        }

        
        

        public void Stop()
        {
            _outputDevice?.Stop();
            if (_audioFile != null)
            {
                _audioFile.Position = 0;
            }

            _labelCountDown.Text = string.Empty;
            _labelCountUp.Text = string.Empty;
            _labelName.Text = string.Empty;
            _labelStatus.Text = string.Empty;
        }
        

        public void Toggle()
        {
            if (_outputDevice?.PlaybackState == PlaybackState.Playing)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }
        

        public void ToggleAutoplay()
        {
            _autoplay = !_autoplay;
            if (_autoplay)
            {
                _buttonAutoplay.Text = "Autoplay = on";
            }
            else
            {
                _buttonAutoplay.Text = "Autoplay = off";
            }
        }
        


    }
}
