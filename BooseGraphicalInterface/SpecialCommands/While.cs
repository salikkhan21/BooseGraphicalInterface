using System;
using System.Windows.Forms;

/// <summary>
/// Represents a WHILE and ENDWHILE command.
/// </summary>
/// <remarks>
/// The WHILE command is used to conditionally execute a block of commands.
/// The ENDWHILE command marks the end of the block of commands.
/// The WHILE command should have 3 arguments: first value, comparison operator, second value.
/// When the WHILE condition is false, the isExecutingSpecialCommand flag is set to true and the commands between WHILE and ENDWHILE are not executed.
/// When the WHILE condition is true, the commands between WHILE and ENDWHILE are executed till the WHILE condition becomes false.
/// </remarks>
/// <example>
/// X = 0
/// WHILE x < 50
///    DRAW x 100
///    X = X + 10
/// ENDWHILE
/// </example>
public class WhileCommand : ISpecialCommand
{
    // public int currentLoopLineIndex = 0;
    // public bool isConditionTrue = false;
    public Stack<int> currentLoopLineIndexStack = new Stack<int>();
    public Stack<bool> isConditionTrueStack = new Stack<bool>();
    public string[] validOperators = {
        ">",
        "<",
        "==",
        "!=",
        ">=",
        "<=",
    };

    /// <summary>
    /// Checks the syntax of the WHILE command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <returns>true if the syntax is valid; otherwise, false.</returns>
    /// <remarks>
    /// The WHILE command should have 3 arguments: first value/variable, comparison operator, second value/variable.
    /// The WHILE command also supports ENDWHILE command.
    /// </remarks>
    /// <example>
    /// WHILE x < 50
    ///   DRAW x 100
    ///   X = X + 10
    /// ENDWHILE
    /// </example>
    public bool SyntaxCheck(
        string[] commandParts,
        ref Dictionary<string, int> variables,
        ref Dictionary<string, string[]> methods,
        bool showError = true
    )
    {
    	// The ENDWHILE command should have 0 arguments
        if (commandParts.Length == 1 && commandParts[0] == "ENDWHILE")
        {
            return true;
        }

        // The WHILE command should have 3 arguments: first value, comparison operator, second value
        if (commandParts.Length != 4)
        {
            string errorMessage = "Syntax error: WHILE command should have 3 arguments. First value/variable, comparison operator, second value/variable (e.g. WHILE x < 10)";
            if (showError)
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // The WHILE command should have a valid comparison operator
        string comparisonOperator = commandParts[2];
        if (!Array.Exists(validOperators, o => o == comparisonOperator))
        {
            string errorMessage = "Syntax error: WHILE command should have a valid comparison operator. Valid operators are: >, <, ==, !=, >=, <=";
            if (showError)
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // The WHILE command should have a valid first value/variable
        if (!int.TryParse(commandParts[1], out int firstValue))
        {
            // The first value is not a valid integer, check if it is a valid variable
            if (VariableCommand.HasInvalidWord(commandParts[1]))
            {
                string errorMessage = "Syntax error: WHILE command should have a valid first value/variable.";
                if (showError)
                    MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // The WHILE command should have a valid second value/variable
        if (!int.TryParse(commandParts[3], out int secondValue))
        {
            // The second value is not a valid integer, check if it is a valid variable
            if (VariableCommand.HasInvalidWord(commandParts[3]))
            {
                string errorMessage = "Syntax error: WHILE command should have a valid second value/variable.";
                if (showError)
                    MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Executes the WHILE command.
	/// </summary>
	/// <param name="commandParts">An array of command parts.</param>
	/// <param name="variables">A dictionary of variables.</param>
	/// <param name="methods">A dictionary of methods.</param>
    /// <param name="isExecutingSpecialCommandStack">A flag that indicates if a special command is being executed.</param>
	/// <param name="specialCommandsStack">A stack of special commands.</param>
    /// <param name="currentLineIndex">The index of the current line.</param>
    public void Execute(
        string[] commandParts,
        ref Dictionary<string, int> variables,
        ref Dictionary<string, string[]> methods,
        ref Stack<bool> isExecutingSpecialCommandStack,
        ref Stack<string> specialCommandsStack,
        ref int currentLineIndex
    )
    {
        // The ENDWHILE command will set the index to the start of the WHILE command
        if (commandParts.Length == 1 && commandParts[0] == "ENDWHILE")
        {
            // check if stack is empty
            if (isExecutingSpecialCommandStack.Count == 0 || specialCommandsStack.Count == 0)
            {
                return;
            }
            // print full stacks
			// MessageBox.Show("isExecutingSpecialCommandStack: " + string.Join(", ", isExecutingSpecialCommandStack.ToArray()) + "\n" + "specialCommandsStack: " + string.Join(", ", specialCommandsStack.ToArray()) + "\n" + "currentLoopLineIndexStack: " + string.Join(", ", currentLoopLineIndexStack.ToArray()) + "\n" + "isConditionTrueStack: " + string.Join(", ", isConditionTrueStack.ToArray()) + "\n" + "currentLineIndex: " + currentLineIndex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (specialCommandsStack.Peek() == "WHILE")
            {
                if (isConditionTrueStack.Peek())
                {
                    // Will check the condition again
                    currentLineIndex = currentLoopLineIndexStack.Peek() - 1;

                    // delete the top of the stack because it will be added again
                    isExecutingSpecialCommandStack.Pop();
                    specialCommandsStack.Pop();
                    isConditionTrueStack.Pop();
                    currentLoopLineIndexStack.Pop();
                }
                else
                {
                    isExecutingSpecialCommandStack.Pop();
                    specialCommandsStack.Pop();
                    currentLoopLineIndexStack.Pop();
                    isConditionTrueStack.Pop();
                }
            }
            return;
        }

        // If the WHILE command is not executed, skip the code between WHILE and ENDWHILE
        if (isExecutingSpecialCommandStack.Peek() && specialCommandsStack.Peek() == "WHILE")
            return;

        // Get the first value
        int firstValue;
        if (int.TryParse(commandParts[1], out int firstValueInt))
        {
            firstValue = firstValueInt;
        }
        else
        {
            firstValue = variables[commandParts[1]];
        }

        // Get the second value
        int secondValue;
        if (int.TryParse(commandParts[3], out int secondValueInt))
        {
            secondValue = secondValueInt;
        }
        else
        {
            secondValue = variables[commandParts[3]];
        }

        // Get the comparison operator
        string comparisonOperator = commandParts[2];

        // Check if the WHILE condition is true
        switch (comparisonOperator)
        {
            case ">":
                isConditionTrueStack.Push(firstValue > secondValue);
                break;
            case "<":
                isConditionTrueStack.Push(firstValue < secondValue);
                break;
            case "==":
                isConditionTrueStack.Push(firstValue == secondValue);
                break;
            case "!=":
                isConditionTrueStack.Push(firstValue != secondValue);
                break;
            case ">=":
                isConditionTrueStack.Push(firstValue >= secondValue);
                break;
            case "<=":
                isConditionTrueStack.Push(firstValue <= secondValue);
                break;
            default:
                isConditionTrueStack.Push(false);
                break;
        }

        // If the WHILE condition is false, set the isExecutingSpecialCommand flag to true
        if (!isConditionTrueStack.Peek())
        {
            isExecutingSpecialCommandStack.Push(true);
            specialCommandsStack.Push("WHILE");
            currentLoopLineIndexStack.Push(0);
        }
        else
        {
            // If the WHILE condition is true, set the isExecutingSpecialCommand flag to false
            // Keep the code between WHILE and ENDWHILE running
            isExecutingSpecialCommandStack.Push(false);
            specialCommandsStack.Push("WHILE");
            currentLoopLineIndexStack.Push(currentLineIndex);
        }
    }
}
