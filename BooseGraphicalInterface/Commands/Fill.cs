using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Represents a command to toggle shape filling on or off.
/// </summary>
public class FillCommand : ICommand
{
    /// <summary>
    /// Checks the syntax of the FILL command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <returns>True if the syntax is correct, otherwise false.</returns>
    /// <remarks>
    /// The FILL command should have 1 argument: ON or OFF.
    /// </remarks>
    /// <example>
    /// FILL ON
    /// </example>
    public bool SyntaxCheck(string[] commandParts, bool showError = true)
    {
        // The FILL command should have 2 parts: FILL and either ON or OFF
        if (commandParts.Length != 2)
        {
            string errorMessage = "Syntax error: FILL command should have 1 argument.";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        string fillValue = commandParts[1].ToUpper();

        if (fillValue != "ON" && fillValue != "OFF")
        {
            string errorMessage = "Syntax error: FILL command argument should be ON or OFF.";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Executes the FILL command to toggle shape filling on or off.
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
            string fillValue = commandParts[1].ToUpper();
            fillShapes = (fillValue == "ON");
        }
    }
}
