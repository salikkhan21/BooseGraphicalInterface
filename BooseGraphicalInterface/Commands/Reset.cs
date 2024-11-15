using System;

/// <summary>
/// Represents a RESET command that resets the drawing state to default values.
/// </summary>
public class ResetCommand : ICommand
{
    /// <summary>
    /// Checks the syntax of the RESET command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <returns><c>true</c> if the syntax is valid; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// The RESET command should have no arguments.
    /// </remarks>
    /// <example>
    /// RESET
    /// </example>
    public bool SyntaxCheck(string[] commandParts, bool showError = true)
    {
        // The RESET command should have no arguments
        if (commandParts.Length != 1)
        {
            string errorMessage = "Syntax error: CLEAR command should have no arguments.";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Executes the RESET command, resetting the drawing state to default values.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <param name="x">Reference to the current X coordinate.</param>
    /// <param name="y">Reference to the current Y coordinate.</param>
    /// <param name="graphics">The <see cref="Graphics"/> object used for drawing.</param>
    public void Execute(string[] commandParts, ref int x, ref int y, ref Color penColor, ref bool fillShapes, Graphics graphics)
    {
        if (SyntaxCheck(commandParts))
        {
            // Reset the drawing state to default values, including penColor and fillShapes
            x = 0;
            y = 0;
            penColor = Color.Black;
            fillShapes = false;
        }
    }
}
