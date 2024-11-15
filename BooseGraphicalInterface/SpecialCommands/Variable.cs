using System;
using System.Windows.Forms;

/// <summary>
/// Represents a command to set a variable.
/// </summary>
public class VariableCommand: ISpecialCommand
{
    /// <summary>
    /// Checks the syntax of the Variable command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <returns>True if the syntax is correct, otherwise false.</returns>
    /// <remarks>
    /// The Variable command should have 3 arguments: variable name, = sign and value.
    /// The variable name should be a string.
    /// The variable must have a = sign between the name and value.
    /// The value should be an integer.
    /// </remarks>
    /// <example>
    /// Count = 50
    /// </example>
    public bool SyntaxCheck(
        string[] commandParts,
        ref Dictionary<string, int> variables,
        ref Dictionary<string, string[]> methods,
        bool showError = true
    )
    {
        // The Variable command should have at least 3 parts: variable name, = sign and value
        if (commandParts.Length < 3)
        {
            string errorMessage = "Syntax error: Variable command should have at least 3 arguments. Variable name, = sign and value (e.g. Count = 50)";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // The variable name should be a string
        if (double.TryParse(commandParts[0], out double variableName))
        {
            string errorMessage = "Syntax error: Variable command variable name argument should be a string (e.g. VAR Count = 50)";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // The variable name should have an invalid word
        if (HasInvalidWord(commandParts[0]))
        {
            string errorMessage = "Syntax error: Variable command variable name argument should not contain a invalid word or symbol (e.g. VAR Count = 50)";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // The variable must have a = sign between the name and value
        if (commandParts[1] != "=")
        {
            string errorMessage = "Syntax error: Variable command should have = sign between the variable name and value (e.g. VAR Count = 50)";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // The value should be an integer
        if (!int.TryParse(commandParts[2], out int value))
        {
            string errorMessage = "Syntax error: Variable command value argument should be an integer (e.g. VAR Count = 50)";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // If there is a 4th part, it should be an operator
        if (commandParts.Length > 3 && !"+-*/".Contains(commandParts[3]))
        {
            string errorMessage = "Syntax error: Variable command operator argument should be +, -, * or / (e.g. Count = 1 + 1)";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // If there is a 5th part, it should be an integer
        if (commandParts.Length > 4 && !int.TryParse(commandParts[4], out int value2))
        {
            string errorMessage = "Syntax error: Variable command value argument should be an integer (e.g. Count = 1 + 1)";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // Command parts can not exceed 5 i.e variable = 1 + 1
        if (commandParts.Length > 5)
        {
            string errorMessage = "Syntax error: Variable command should have at most 5 arguments. Variable name, = sign, value, operator and value (e.g. Count = 1 + 1)";
            if (showError)
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Executes the Variable command to set a variable.
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
        if (SyntaxCheck(commandParts, ref variables, ref methods, false))
        {
            // If the command has 3 parts, assign or reassign the variable
            if (commandParts.Length == 3 && int.TryParse(commandParts[2], out int value))
            {
                variables[commandParts[0]] = value;
            }

            // If the command has more than 3 parts, reassign to the result of the calculation
            if (commandParts.Length > 3)
            {
                // only three parts calculation is allowed for now
                // val1 operator val2
                // e.g. 10 + 1 // it can only be integer because variables are already parsed
                
                // get the operator
                string op = commandParts[3];

                // get the first value (part 2 convert to int)
                int val1 = int.Parse(commandParts[2]);

                // get the second value (part 4 convert to int)
                int val2 = int.Parse(commandParts[4]);

                int result = 0;

                switch (op)
                {
                    case "+":
                        result = val1 + val2;
                        break;
                    case "-":
                        result = val1 - val2;
                        break;
                    case "*":
                        result = val1 * val2;
                        break;
                    case "/":
                        result = val1 / val2;
                        break;
                    default:
                        break;
                }

                // If the variable is in the method, add a temporary variable
                if (variables.ContainsKey(commandParts[0] + "_METHOD"))
                {
                    // Assign the variable
                    variables[commandParts[0] + "_METHOD"] = result;
                    return;
                }

                // Assign the variable
                variables[commandParts[0]] = result;
            }
        }
    }

    /// <summary>
    /// Parses the variables in a command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <param name="variables">A dictionary of variables.</param>
    /// <returns>An array of command parts with variables replaced by their values.</returns>
    /// <example>
    /// Count = 50
    /// CIRCLE Count
    /// Count = Count + 1
    /// </example>
    public static bool ParseVariables(ref string[] commandParts, Dictionary<string, int> variables, bool showError = true)
    {
        // If the command is to be ignored, skip it
        if (IgnoreCommand(commandParts[0]))
        {
            return true;
        }

        // Loop through the command parts (starting at index 1)
        for (int i = 1; i < commandParts.Length; i++)
        {
            // If the command part is a string
            if (int.TryParse(commandParts[i], out _) || HasInvalidWord(commandParts[i]))
            {
                // Skip the command part
                continue;
            }

            // If the special command is a method call (e.g. MethodName())
            // parse the temporary variables if available
            if (variables.ContainsKey(commandParts[i] + "_METHOD")) {
                // Replace the variable with its value
                commandParts[i] = variables[commandParts[i] + "_METHOD"].ToString();
                continue;
            }

            // If the command part is a variable
            if (variables.ContainsKey(commandParts[i]))
            {
                // Replace the variable with its value
                commandParts[i] = variables[commandParts[i]].ToString();
                continue;
            } 
            
            // If the command part is an undefined variable
            if (!variables.ContainsKey(commandParts[i]))
            {
                string errorMessage = "Syntax error: Variable " + commandParts[i] + " is not defined";
                if (showError)
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        return true;
    }

    // ignore these commands
    public static bool IgnoreCommand(string commandName)
    {
        string[] ignoreCommands = {
            "FILL",
            "COLOR",
            "WHILE",
            "METHOD",
        };

        return Array.Exists(ignoreCommands, c => c == commandName);
    }

    // check for invalid words
    public static bool HasInvalidWord(string word)
    {
        // Symbols and operators
        string[] invalidWords = {
            "=",
            "<",
            ">",
            "<=",
            ">=",
            "==",
            "!=",
            "!",
            "+",
            "-",
            "*",
            "/",
            "(",
            ")",
            "{",
            "}",
            ";",
            ":",
            ",",
            ".",
            " ",
            "\"",
        };
        
        foreach (string invalidWord in invalidWords)
        {
            if (word.Contains(invalidWord))
            {
                return true;
            }
        }

        return false;
    }

    // check if the given parts can be a variable assignment
    public static bool IsVariableAssignment(string[] commandParts)
    {
        // The Variable command should have at least 3 parts: variable name, = sign and value
        if (commandParts.Length < 3)
        {
            return false;
        }

        // The variable name should be a string
        if (double.TryParse(commandParts[0], out double variableName))
        {
            return false;
        }

        // The variable name should have an invalid word
        if (HasInvalidWord(commandParts[0]))
        {
            return false;
        }

        // The variable must have a = sign between the name and value
        if (commandParts[1] != "=")
        {
            return false;
        }

        // Command parts can not exceed 5 i.e variable = 1 + 1
        if (commandParts.Length > 5)
        {
            return false;
        }

        return true;
    }
}
