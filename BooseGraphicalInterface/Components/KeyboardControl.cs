using System;
using System.Windows.Forms;
using System.Linq;

namespace BooseGraphicalInterface
{
    /// <summary>
    /// A custom user control for a virtual keyboard.
    /// </summary>
    public partial class KeyboardControl : UserControl
    {
        private TextBox? activeTextBox; // The currently active TextBox

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardControl"/> class.
        /// </summary>
        public KeyboardControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the active TextBox that will receive keyboard input.
        /// </summary>
        /// <param name="textBox">The TextBox to set as active.</param>
        public void SetActiveTextBox(TextBox textBox)
        {
            activeTextBox = textBox;
        }

        private void InitializeComponent()
        {
            int buttonWidth = 75;
            int buttonHeight = 40;
            int buttonSpacing = 5;

            // Create and configure the FlowLayoutPanels for button arrangement
            var buttonLayouts = new FlowLayoutPanel[5];
            string[] rowContents = { "QWERTYUIOP[]", "ASDFGHJKL;'", "ZXCVBNM,./()", "1234567890", "-+=*/" };

            for (int i = 0; i < 5; i++)
            {
                buttonLayouts[i] = CreateButtonLayout(rowContents[i], buttonWidth, buttonHeight);
                buttonLayouts[i].Location = new System.Drawing.Point(0, 10 + i * (buttonHeight + buttonSpacing));
                this.Controls.Add(buttonLayouts[i]);
            }

            // Set the size of the control
            this.Size = new System.Drawing.Size(830, 270);

            // Add the space, enter, and backspace buttons on the last row
            var spaceButton = CreateButton("Space", 200, buttonHeight);
            var enterButton = CreateButton("Enter", 100, buttonHeight);
            var backspaceButton = CreateButton("Backspace", 100, buttonHeight);

            var lastButtonLayout = buttonLayouts.Last();
            lastButtonLayout.Controls.Add(spaceButton);
            lastButtonLayout.Controls.Add(enterButton);
            lastButtonLayout.Controls.Add(backspaceButton);

            // Expand the last row with the space, enter, and backspace buttons
            lastButtonLayout.Size = new System.Drawing.Size(lastButtonLayout.Size.Width + 400, lastButtonLayout.Size.Height);
        }

        private FlowLayoutPanel CreateButtonLayout(string rowContent, int buttonWidth, int buttonHeight)
        {
            var buttonLayout = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Size = new System.Drawing.Size(buttonWidth * rowContent.Length, buttonHeight + 5)
            };

            foreach (char key in rowContent)
            {
                var button = CreateButton(key.ToString(), 62, buttonHeight);
                buttonLayout.Controls.Add(button);
            }

            return buttonLayout;
        }

        /// <summary>
        /// Creates a button with the specified text and size.
        /// </summary>
        /// <param name="text">The text to display on the button.</param>
        /// <param name="width">The width of the button.</param>
        /// <param name="height">The height of the button.</param>
        /// <returns>The created button.</returns>
        private Button CreateButton(string text, int width, int height)
        {
            var button = new Button
            {
                Size = new System.Drawing.Size(width, height),
                Text = text,
            };

            button.Click += (sender, e) =>
            {
                if (activeTextBox != null)
                {
                    string inputText = button.Text;

                    if (inputText == "Space")
                    {
                        inputText = " ";
                    }
                    else if (inputText == "Enter")
                    {
                        inputText = Environment.NewLine;
                    }
                    else if (inputText == "Backspace")
                    {
                        if (activeTextBox.Text.Length > 0)
                        {
                            activeTextBox.Text = activeTextBox.Text.Substring(0, activeTextBox.Text.Length - 1);
                        }
                        return;
                    }

                    activeTextBox.Focus();
                    SendKeys.SendWait(inputText);
                }
            };

            return button;
        }
    }
}
