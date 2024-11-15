using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Represents a command to change the drawing pen color.
/// </summary>
public class ColorCommand : ICommand
{
    /// <summary>
    /// Checks the syntax of the COLOR command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <returns>True if the syntax is correct, otherwise false.</returns>
    /// <remarks>
    /// The COLOR command should have 1 argument: a valid color name.
    /// Valid colors are: BLACK, BLUE, RED, GREEN
    /// </remarks>
    /// <example>
    /// COLOR RED
    /// </example>
    public bool SyntaxCheck(string[] commandParts, bool showError = true)
    {
        // The COLOR command should have 2 parts: COLOR and a valid color name
        if (commandParts.Length != 2)
        {
            Console.WriteLine("Syntax error: COLOR command should have 2 arguments.");
            return false;
        }

        string color = commandParts[1].ToUpper();
        string[] validColors = { "BLACK", "BLUE", "RED", "GREEN" };

        if (!Array.Exists(validColors, c => c == color))
        {
            string errorMessage = "Syntax error: COLOR command argument should be a valid color name. Valid colors are: BLACK, BLUE, RED, GREEN";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Executes the COLOR command to change the drawing pen color.
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
            string color = commandParts[1].ToUpper();
            penColor = Color.FromName(color);
        }
    }
}
