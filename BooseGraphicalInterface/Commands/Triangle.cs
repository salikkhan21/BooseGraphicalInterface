using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Represents a command to draw a triangle.
/// </summary>
public class TriangleCommand : ICommand
{
    /// <summary>
    /// Checks the syntax of the TRIANGLE command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <returns>True if the syntax is correct, otherwise false.</returns>
    /// <remarks>
    /// The TRIANGLE command should have 2 arguments: base length and height.
    /// The base length and height should be positive integers.
    /// </remarks>
    /// <example>
    /// TRIANGLE 50 100
    /// </example>
    public bool SyntaxCheck(string[] commandParts, bool showError = true)
    {
        // The TRIANGLE command should have 3 parts: TRIANGLE, base length, and height
        if (commandParts.Length != 3)
        {
            string errorMessage = "Syntax error: TRIANGLE command should have 2 arguments.";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if (!int.TryParse(commandParts[1], out int baseLength) || baseLength <= 0)
        {
            string errorMessage = "Syntax error: Invalid base length for TRIANGLE command.";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if (!int.TryParse(commandParts[2], out int height) || height <= 0)
        {
            string errorMessage = "Syntax error: Invalid height for TRIANGLE command.";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Executes the TRIANGLE command to draw a triangle.
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
            if (int.TryParse(commandParts[1], out int baseLength) && baseLength > 0 &&
                int.TryParse(commandParts[2], out int height) && height > 0)
            {
                using (Pen pen = new Pen(penColor))
                {
                    Point[] points = new Point[3];
                    points[0] = new Point(x, y);
                    points[1] = new Point(x + baseLength, y);
                    points[2] = new Point(x + (baseLength / 2), y - height);

                    if (fillShapes)
                    {
                        using (SolidBrush brush = new SolidBrush(penColor))
                        {
                            graphics.FillPolygon(brush, points);
                        }
                    }
                    else
                    {
                        graphics.DrawPolygon(pen, points);
                    }
                }
            }
        }
    }
}
