using System;
using System.Windows.Forms;

/// <summary>
/// Represents an IF and ENDIF command.
/// </summary>
/// <remarks>
/// The IF command is used to conditionally execute a block of commands.
/// The ENDIF command marks the end of the block of commands.
/// The IF command should have 3 arguments: first value, comparison operator, second value.
/// When the IF condition is false, the isExecutingSpecialCommand flag is set to true and the commands between IF and ENDIF are not executed.
/// When the IF condition is true, the commands between IF and ENDIF are executed. isExecutingSpecialCommand flag is set to false.
/// </remarks>
/// <example>
/// IF x > 10
///     MOVE 25 50
///     DRAW 100 100
/// ENDIF
/// </example>
public class IfCommand: ISpecialCommand
{
	private string[] validOperators = {
		">",
		"<",
		"==",
		"!=",
		">=",
		"<=",
	};

    /// <summary>
    /// Checks the syntax of the IF command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <returns>true if the syntax is valid; otherwise, false.</returns>
    /// <remarks>
    /// The IF command should have 3 arguments: first value, comparison operator, second value.
    /// The IF command also supports ENDIF command.
    /// </remarks>
    /// <example>
    /// IF x > 10
    ///   MOVE 25 50
    ///   DRAW 100 100
    /// ENDIF
    /// </example>
    public bool SyntaxCheck(
        string[] commandParts,
        ref Dictionary<string, int> variables,
        ref Dictionary<string, string[]> methods,
        bool showError = true
    )
	{
		// The ENDIF command should have 0 arguments
		if (commandParts.Length == 1 && commandParts[0] == "ENDIF")
		{
			return true;
		}

		// The IF command should have 3 arguments: first value, comparison operator, second value
		if (commandParts.Length != 4)
		{
			string errorMessage = "Syntax error: IF command should have 3 arguments. First value/variable, comparison operator, second value/variable (e.g. IF x > 10)";
			if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
		}

		// Check if the comparison operator is valid
		if (Array.IndexOf(validOperators, commandParts[2]) == -1)
		{
			string errorMessage = "Syntax error: Invalid comparison operator. Valid operators are: >, <, ==, !=, >=, <=";
			if (showError)
			MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		// Check if the first value is a valid integer
		if (!int.TryParse(commandParts[1], out int firstValue))
		{
			string errorMessage = "Syntax error: Invalid first value. The first value should be a valid integer or a valid variable.";
			if (showError)
			MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		// Check if the second value is a valid integer
		if (!int.TryParse(commandParts[3], out int secondValue))
		{
			string errorMessage = "Syntax error: Invalid second value. The second value should be a valid integer or a valid variable.";
			if (showError)
			MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		return true;
	}

	/// <summary>
	/// Executes the IF command.
	/// </summary>
	/// <param name="commandParts">An array of command parts.</param>
	/// <param name="variables">A dictionary of variables.</param>
	/// <param name="methods">A dictionary of methods.</param>
	/// <param name="isExecutingSpecialCommandStack">A Stack of flags that indicates if a special command is being executed.</param>
	/// <param name="specialCommandsStack">A stack of special commands.</param>
	/// <param name="currentLineIndex">The index of the current special command.</param>
	public void Execute(
		string[] commandParts,
		ref Dictionary<string, int> variables,
    ref Dictionary<string, string[]> methods,
		ref Stack<bool> isExecutingSpecialCommandStack,
		ref Stack<string> specialCommandsStack,
		ref int currentLineIndex
	)
	{
		
		// The ENDIF command should end the execution of the IF command
		if (commandParts.Length == 1 && commandParts[0] == "ENDIF")
		{
			// check if stack is empty
			if (isExecutingSpecialCommandStack.Count == 0 || specialCommandsStack.Count == 0)
			{
					return;
			}

			if (specialCommandsStack.Peek() == "IF") {
				isExecutingSpecialCommandStack.Pop();
				specialCommandsStack.Pop();
			}
			return;
		}


		// If the IF command is not executed, skip the code between IF and ENDIF
		if (isExecutingSpecialCommandStack.Peek() && specialCommandsStack.Peek() == "IF") {
			// but if it is a nested IF, push the IF command to the stack again
			// this makes sure that nested if does not break the execution of the parent IF
			// it will be deleted when the nested IF is closed with ENDIF
			if (commandParts[0] == "IF") {
				isExecutingSpecialCommandStack.Push(true);
				specialCommandsStack.Push("IF");
			}

			return;
		}

		if (!SyntaxCheck(commandParts, ref variables, ref methods, false))
			return;

		// Get the first value
		int firstValue = int.Parse(commandParts[1]);

		// Get the comparison operator
		string comparisonOperator = commandParts[2];

		// Get the second value
		int secondValue = int.Parse(commandParts[3]);

		// Check if the condition is true
		bool condition = false;

		switch (comparisonOperator)
		{
			case ">":
				condition = firstValue > secondValue;
				break;
			case "<":
				condition = firstValue < secondValue;
				break;
			case "==":
				condition = firstValue == secondValue;
				break;
			case "!=":
				condition = firstValue != secondValue;
				break;
			case ">=":
				condition = firstValue >= secondValue;
				break;
			case "<=":
				condition = firstValue <= secondValue;
				break;
		}

		// If the condition is false, set the isExecutingSpecialCommand flag to true
		if (!condition)
		{
			// This will skip the code between IF and ENDIF
			isExecutingSpecialCommandStack.Push(true);
			specialCommandsStack.Push("IF");
		}
		else
		{
			// If the condition is true, set the isExecutingSpecialCommand flag to false
			// Keep the code between IF and ENDIF running
			isExecutingSpecialCommandStack.Push(false);
			specialCommandsStack.Push("IF");
		}
	}
}