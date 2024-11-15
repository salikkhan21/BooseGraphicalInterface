using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using BOOSE;

namespace BooseGraphicalInterface
{
    /// <summary>
    /// The main form of the Graphical Programming Language application.
    /// </summary>
    public partial class MainForm : Form
    {
       
        
        private TextBox programTextBox = new TextBox();
        private TextBox commandTextBox = new TextBox();
        private Button runButton = new Button();
        private Button syntaxCheckButton = new Button();
        private PictureBox resultBox = new PictureBox();
        private Graphics resultBoxGraphics;
        private KeyboardControl keyboardControl = new KeyboardControl();
        private Label statusLabel = new Label();
        private Button saveButton = new Button();
        private Button loadButton = new Button();
        private Button openNewThread = new Button();
        private List<Form> additionalForms = new List<Form>();
        private CommandOneParameter parser;


        public MainForm()
        {
            InitializeComponent();
            this.resultBoxGraphics = resultBox.CreateGraphics();
           


        }

        /// <summary>
        /// Initializes the GUI components.
        /// </summary>
        private void InitializeComponent()
        {
            // Create and configure GUI components
            programTextBox.Multiline = true;
            programTextBox.Size = new System.Drawing.Size(400, 200);
            programTextBox.Location = new System.Drawing.Point(10, 10);
            programTextBox.ScrollBars = ScrollBars.Vertical;

            // Set up the keyboard control
            programTextBox.Enter += (sender, e) =>
            {
                keyboardControl.SetActiveTextBox(programTextBox);
            };

            commandTextBox.Size = new System.Drawing.Size(400, 30);
            commandTextBox.Location = new System.Drawing.Point(10, 220);

            commandTextBox.Enter += (sender, e) =>
            {
                keyboardControl.SetActiveTextBox(commandTextBox);
            };

            // add different buttons and their properties
            runButton.Text = "Run";
            runButton.Size = new System.Drawing.Size(195, 30);
            runButton.Location = new System.Drawing.Point(10, 260);
           

            syntaxCheckButton.Text = "Syntax Check";
            syntaxCheckButton.Size = new System.Drawing.Size(195, 30);
            syntaxCheckButton.Location = new System.Drawing.Point(215, 260);
           

            saveButton.Text = "Save";
            saveButton.Size = new System.Drawing.Size(195, 30);
            saveButton.Location = new System.Drawing.Point(10, 300);
           

            loadButton.Text = "Load";
            loadButton.Size = new System.Drawing.Size(195, 30);
            loadButton.Location = new System.Drawing.Point(215, 300);
           
            openNewThread.Text = "Open New Thread";
            openNewThread.Size = new System.Drawing.Size(195, 30);
            openNewThread.Location = new System.Drawing.Point(215, 340);

           

            resultBox.Size = new System.Drawing.Size(400, 400);
            resultBox.Location = new System.Drawing.Point(430, 10);
            resultBox.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(resultBox);

            // info label
            statusLabel.Text = "Position: {X=0, Y=0}\nFill: ON\nColor: Red";
            statusLabel.Size = new System.Drawing.Size(400, 50);
            statusLabel.Location = new System.Drawing.Point(10, 340);

            // Set up the form
            this.Text = "BOOSE GUI";
            this.Size = new System.Drawing.Size(855, 700);
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Controls.Add(programTextBox);
            this.Controls.Add(commandTextBox);
            this.Controls.Add(runButton);
            this.Controls.Add(syntaxCheckButton);
            this.Controls.Add(saveButton);
            this.Controls.Add(loadButton);
            this.Controls.Add(openNewThread);
            this.Controls.Add(resultBox);
            this.Controls.Add(statusLabel);

            keyboardControl = new KeyboardControl();
            this.Controls.Add(keyboardControl);
            keyboardControl.BringToFront();
            keyboardControl.Location = new System.Drawing.Point(10, 420);
            keyboardControl.Visible = true;
        }

      

     
      

     

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
