using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Represents a command to draw a rectangle.
/// </summary>
public class RectangleCommand : ICommand
{
    /// <summary>
    /// Checks the syntax of the RECTANGLE command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <returns>True if the syntax is correct, otherwise false.</returns>
    /// <remarks>
    /// The RECTANGLE command should have 2 arguments: width and height.
    /// The width and height should be positive integers.
    /// </remarks>
    /// <example>
    /// RECTANGLE 50 100
    /// </example>
    public bool SyntaxCheck(string[] commandParts, bool showError = true)
    {
        // The RECTANGLE command should have 3 parts: RECTANGLE, width, and height
        if (commandParts.Length != 3)
        {
            string errorMessage = "Syntax error: RECTANGLE command should have 2 arguments.";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if (!int.TryParse(commandParts[1], out int width) || width <= 0)
        {
            string errorMessage = "Syntax error: Invalid width for RECTANGLE command.";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if (!int.TryParse(commandParts[2], out int height) || height <= 0)
        {
            string errorMessage = "Syntax error: Invalid height for RECTANGLE command.";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Executes the RECTANGLE command to draw a rectangle.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <param name="x">The current x-coordinate.</param>
    /// <param name="y">The current y-coordinate.</param>
    /// <param name="penColor">The color of the drawing pen.</param>
    /// <param name="fillShapes">A boolean indicating whether shapes should be filled.</param>
    /// <param name="graphics">The Graphics object for drawing.</param>
    public void Execute(string[] commandParts, ref int x, ref int y, ref Color penColor, ref bool fillShapes, Graphics graphics)
    {
        if (SyntaxCheck(commandParts))
        {
            if (int.TryParse(commandParts[1], out int width) && width > 0 &&
                int.TryParse(commandParts[2], out int height) && height > 0)
            {
                using (Pen pen = new Pen(penColor))
                {
                    if (fillShapes)
                    {
                        using (SolidBrush brush = new SolidBrush(penColor))
                        {
                            graphics.FillRectangle(brush, x, y, width, height);
                        }
                    }
                    else
                    {
                        graphics.DrawRectangle(pen, x, y, width, height);
                    }
                }
            }
        }
    }
}
