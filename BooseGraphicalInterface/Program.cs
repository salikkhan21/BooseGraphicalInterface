using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using BooseGraphicalInterface;

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
        private CommandParser parser;
        private KeyboardControl keyboardControl = new KeyboardControl();
        private Label statusLabel = new Label();
        private Button saveButton = new Button();
        private Button loadButton = new Button();
        private Button openNewThread = new Button();
        private List<Form> additionalForms = new List<Form>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            this.resultBoxGraphics = resultBox.CreateGraphics();
            parser = new CommandParser(resultBoxGraphics);
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

            
            runButton.Text = "Run";
            runButton.Size = new System.Drawing.Size(195, 30);
            runButton.Location = new System.Drawing.Point(10, 260);
            runButton.Click += RunButton_Click;

            syntaxCheckButton.Text = "Syntax Check";
            syntaxCheckButton.Size = new System.Drawing.Size(195, 30);
            syntaxCheckButton.Location = new System.Drawing.Point(215, 260);
            syntaxCheckButton.Click += SyntaxCheckButton_Click;

            saveButton.Text = "Save";
            saveButton.Size = new System.Drawing.Size(195, 30);
            saveButton.Location = new System.Drawing.Point(10, 300);
            saveButton.Click += (sender, e) =>
            {
                SaveButton_Click(sender, e, programTextBox);
            };

            loadButton.Text = "Load";
            loadButton.Size = new System.Drawing.Size(195, 30);
            loadButton.Location = new System.Drawing.Point(215, 300);
            loadButton.Click += (sender, e) =>
            {
                LoadButton_Click(sender, e, programTextBox);
            };
            openNewThread.Text = "Open New Thread";
            openNewThread.Size = new System.Drawing.Size(195, 30);
            openNewThread.Location = new System.Drawing.Point(215, 340);

            openNewThread.Click += OpenNewThread_Click;

            void OpenNewThread_Click(object? sender, EventArgs e)
            {
                // Create a new form on the UI thread
                this.Invoke((MethodInvoker)delegate
                {
                    Form newForm = new Form();

                    TextBox programTextBox = new TextBox();
                    programTextBox.Multiline = true;
                    programTextBox.Size = new System.Drawing.Size(815, 200);
                    programTextBox.Location = new System.Drawing.Point(10, 10);
                    programTextBox.ScrollBars = ScrollBars.Vertical;

                    Button saveButton = new Button();
                    saveButton.Text = "Save";
                    saveButton.Size = new System.Drawing.Size(195, 30);
                    saveButton.Location = new System.Drawing.Point(10, 220);
                    saveButton.Click += (sender, e) =>
                    {
                        SaveButton_Click(sender, e, programTextBox);
                    };

                    Button loadButton = new Button();
                    loadButton.Text = "Load";
                    loadButton.Size = new System.Drawing.Size(195, 30);
                    loadButton.Location = new System.Drawing.Point(215, 220);
                    loadButton.Click += (sender, e) =>
                    {
                        LoadButton_Click(sender, e, programTextBox);
                    };

                    KeyboardControl keyboardControl = new KeyboardControl();
                    keyboardControl.Location = new System.Drawing.Point(10, 260);

                    newForm.Size = new System.Drawing.Size(855, 550);
                    newForm.StartPosition = FormStartPosition.CenterScreen;
                    newForm.FormBorderStyle = FormBorderStyle.FixedSingle;

                    newForm.Controls.Add(programTextBox);
                    newForm.Controls.Add(keyboardControl);
                    newForm.Controls.Add(saveButton);
                    newForm.Controls.Add(loadButton);

                    keyboardControl.SetActiveTextBox(programTextBox);

                    newForm.Show();

                    additionalForms.Add(newForm);
                });
            }

            resultBox.Size = new System.Drawing.Size(400, 400);
            resultBox.Location = new System.Drawing.Point(430, 10);
            resultBox.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(resultBox);

            // info label
            statusLabel.Text = "Example:\n FILL ON\nCOLOR BLUE\nMOVE 100 100\nCIRCLE 40";
            //statusLabel.Text = "Position: {X=0, Y=0}\nFill: ON\nColor: Red";
            statusLabel.Size = new System.Drawing.Size(400, 100);
            statusLabel.Location = new System.Drawing.Point(10, 340);

            // Set up the form
            this.Text = "BOOSE GUI";
            this.Size = new System.Drawing.Size(855, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            BackColor = System.Drawing.Color.AliceBlue;
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

        /// <summary>
        /// Executes the command or program entered in the text box.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void RunButton_Click(object? sender, EventArgs e)
        {
            string command = commandTextBox.Text.Trim();

            if (command == "")
            {
                if (programTextBox.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter a command.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    commandTextBox.Text = "RUN";
                    command = "RUN";
                }
            }

            if (command == "RUN")
            {
                parser.ResetProgram();

                string mainFormProgram = programTextBox.Text;

                // Create tasks for the main form program and additional forms
                var mainFormTask = Task.Run(() => ExecuteProgramOnUIThread(mainFormProgram));

                var additionalFormTasks = additionalForms
                    .Select(form => Task.Run(() =>
                    {
                        TextBox additionalFormProgramTextBox = form.Controls.OfType<TextBox>().FirstOrDefault()!;
                        string additionalFormProgram = additionalFormProgramTextBox.Text;
                        if (!string.IsNullOrWhiteSpace(additionalFormProgram))
                        {
                            ExecuteProgramOnUIThread(additionalFormProgram);
                        }
                    }));

                await Task.WhenAll(mainFormTask, Task.WhenAll(additionalFormTasks));

                Point currentPosition = parser.GetCurrentPosition();
                bool isFillOn = parser.IsFillOn();
                string currentColor = parser.GetCurrentColor();

                statusLabel.Text = $"Position: {currentPosition}\nFill: {(isFillOn ? "On" : "Off")}\nColor: {currentColor}";
            }
            else
            {
                if (!parser.SyntaxCheckLine(command))
                {
                    return;
                }

                int i = 0;
                parser.ExecuteCommand(command, ref i);
            }
        }

        /// <summary>
        /// Executes a program on the UI thread using the provided program string.
        /// </summary>
        /// <param name="program">The program string to execute.</param>
        private void ExecuteProgramOnUIThread(string program)
        {
            this.Invoke((MethodInvoker)delegate
            {
                // create a new parser for each program
                CommandParser parser = new CommandParser(resultBoxGraphics);

                if (!parser.SyntaxCheckProgram(program))
                {
                    return;
                }

                parser.ExecuteProgram(program);
            });
        }

        /// <summary>
        /// Checks the syntax of the command or program entered in the text box.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void SyntaxCheckButton_Click(object? sender, EventArgs e)
        {
            string programText = programTextBox.Text;

            if (programText.Trim() == "")
            {
                // try to syntax check the command
                string command = commandTextBox.Text.Trim();

                if (command == "")
                {
                    MessageBox.Show("Please enter a command.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (parser.SyntaxCheckLine(command))
                {
                    MessageBox.Show("Syntax check successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Syntax was not valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return;
            }

            if (parser.SyntaxCheckProgram(programText))
            {
                MessageBox.Show("Syntax check successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Syntax was not valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves the program content in the specified text box to a file.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        /// <param name="programTextBox">The text box containing the program to save.</param>
        private void SaveButton_Click(object? sender, EventArgs e, TextBox programTextBox)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        File.WriteAllText(filePath, programTextBox.Text);
                        MessageBox.Show("Program saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Loads a program from a file into the specified text box.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        /// <param name="programTextBox">The text box to load the program into.</param>
        private void LoadButton_Click(object? sender, EventArgs e, TextBox programTextBox)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    try
                    {
                        string program = File.ReadAllText(filePath);
                        programTextBox.Text = program;
                        MessageBox.Show("Program loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        /// <summary>
        /// Entry point of the application. Initializes and runs the main form.
        /// </summary>

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
