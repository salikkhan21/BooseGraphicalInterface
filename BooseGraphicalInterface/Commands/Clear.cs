/// <summary>
/// Represents a command to clear the drawing area.
/// </summary>
public class ClearCommand : ICommand
{
    /// <summary>
    /// Checks the syntax of the CLEAR command.
    /// </summary>
    /// <param name="commandParts">The parts of the command.</param>
    /// <returns>True if the syntax is correct; otherwise, false.</returns>
    /// <remarks>The CLEAR command should have no arguments.</remarks>
    /// <example>CLEAR</example>
    public bool SyntaxCheck(string[] commandParts, bool showError = true)
    {
        // The CLEAR command should have no arguments
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
    /// Executes the CLEAR command to clear the drawing area.
    /// </summary>
    /// <param name="commandParts">The parts of the command.</param>
    /// <param name="x">The current x-coordinate.</param>
    /// <param name="y">The current y-coordinate.</param>
    /// <param name="penColor">The color of the drawing pen.</param>
    /// <param name="fillShapes">A boolean indicating whether shapes should be filled.</param>
    /// <param name="graphics">The Graphics object used for drawing.</param>
    public void Execute(string[] commandParts, ref int x, ref int y, ref Color penColor, ref bool fillShapes, Graphics graphics)
    {
        if (SyntaxCheck(commandParts))
        {
            graphics.Clear(Color.LightGray);
        }
    }
}
