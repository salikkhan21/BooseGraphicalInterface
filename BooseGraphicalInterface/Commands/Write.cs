using System;
using System.Drawing;

/// <summary>
/// Represents a command for drawing a line in a graphics context.
/// </summary>
public class WriteCommand : ICommand
{
    /// <summary>
    /// Checks the syntax of the DRAW command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <returns>True if the syntax is correct; otherwise, false.</returns>
    /// <remarks>
    /// The DRAW command should have 1 or 2 arguments: X and Y (optional) coordinates.
    /// The X and Y coordinates should be integers.
    /// </remarks>
    /// <example>
    /// DRAW 50
    /// </example>
    /// <example>
    /// DRAW 50 100
    /// </example>
    public bool SyntaxCheck(string[] commandParts, bool showError = true)
    {
        if (commandParts.Length < 2)
        {
            string errorMessage = "Syntax error: WRITE command should have at least 1 argument.";
            if (showError)
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // if the size is 2, make sure the first argument is string (text)
        // make sure the second argument is a string starting with "
        if (commandParts.Length == 2 && !commandParts[1].StartsWith("\"") && !commandParts[1].EndsWith("\""))
        {
            string errorMessage = "Syntax error: WRITE command text argument should be a string starting and ending with \".";
            if (showError)
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // if the size is 3, make sure the first argument is a number (size)
        if (commandParts.Length == 3 && !int.TryParse(commandParts[1], out int size))
        {
            string errorMessage = "Syntax error: WRITE command size argument should be an integer.";
            if (showError)
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        

        // make sure its not more than 1 argument
        if (commandParts.Length > 3)
        {
            string errorMessage = "Syntax error: WRITE command should have at most 1 argument.";
            if (showError)
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Executes the DRAW command by drawing a line in the graphics context.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <param name="x">The current X coordinate.</param>
    /// <param name="y">The current Y coordinate.</param>
    /// <param name="penColor">The current pen color.</param>
    /// <param name="fillShapes">A boolean indicating whether shapes should be filled.</param>
    /// <param name="graphics">The graphics context to draw on.</param>
    public void Execute(string[] commandParts, ref int x, ref int y, ref Color penColor, ref bool fillShapes, Graphics graphics)
    {
        if (SyntaxCheck(commandParts))
        {
            // get the text size
            string text;
            int size = 12;

            // if the size is 3, make sure the first argument is a number (size)
            if (commandParts.Length == 3)
            {
                size = int.Parse(commandParts[1]);
                text = commandParts[2];
            }
            else
            {
                text = commandParts[1];
            }

            // create a font
            Font font = new Font("Arial", size);

            // draw the text at the current x and y coordinates
            graphics.DrawString(text, font, Brushes.Black, x, y);

            // dispose of the font
            font.Dispose();
        }
    }
}
