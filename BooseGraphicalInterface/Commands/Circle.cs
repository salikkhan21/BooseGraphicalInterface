using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Represents a command to draw a circle.
/// </summary>
public class CircleCommand : ICommand
{
    /// <summary>
    /// Checks the syntax of the CIRCLE command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <returns>True if the syntax is correct, otherwise false.</returns>
    /// <remarks>
    /// The CIRCLE command should have 1 argument: radius.
    /// The radius should be a positive integer.
    /// </remarks>
    /// <example>
    /// CIRCLE 50
    /// </example>
    public bool SyntaxCheck(string[] commandParts, bool showError = true)
    {
        // The CIRCLE command should have 2 parts: CIRCLE and the radius
        if (commandParts.Length != 2)
        {
            string errorMessage = "Syntax error: CIRCLE command should have 1 argument. Radius (e.g. CIRCLE 50)";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if (!int.TryParse(commandParts[1], out int radius) || radius <= 0)
        {
            string errorMessage = "Syntax error: CIRCLE command radius argument should be a positive integer.";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Executes the CIRCLE command to draw a circle.
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
            if (int.TryParse(commandParts[1], out int radius) && radius > 0)
            {
                using (Pen pen = new Pen(penColor))
                {
                    int diameter = radius * 2;
                    if (fillShapes)
                    {
                        using (SolidBrush brush = new SolidBrush(penColor))
                        {
                            graphics.FillEllipse(brush, x - radius, y - radius, diameter, diameter);
                        }
                    }
                    else
                    {
                        graphics.DrawEllipse(pen, x - radius, y - radius, diameter, diameter);
                    }
                }
            }
        }
    }
}
