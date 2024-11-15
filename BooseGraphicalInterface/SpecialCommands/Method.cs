using System;
using System.Windows.Forms;

/// <summary>
/// Represents a METHOD and ENDMETHOD command. As well as a method call command.
/// </summary>
/// <remarks>
/// The METHOD command is used to define a method.
/// The ENDMETHOD command marks the end of the method.
/// The METHOD command should have 1 argument: methodName(<parameter list>).
/// The method name should be unique.
/// The method is defined by the commands between METHOD and ENDMETHOD.
/// The method can have parameters. The parameters are separated by comma.
/// The method can be called by using the method name and passing the parameters.
/// </remarks>
/// <example>
/// METHOD DrawLine(x1, y1, x2, y2)
///    MOVE x1 y1
///    DRAW x2 y2
/// ENDMETHOD
/// DrawLine(10, 10, 100, 100)
/// </example>
public class MethodCommand : ISpecialCommand
{
    public Stack<string[]> runningMethodsStack = new Stack<string[]>();
    public Stack<string> storingMethodsStack = new Stack<string>();

    /// <summary>
    /// Checks the syntax of the METHOD command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <returns>true if the syntax is valid; otherwise, false.</returns>
    /// <remarks>
    /// The METHOD command should have 1 argument: methodName(<parameter list>).
    /// The method name should be unique.
    /// The method is defined by the commands between METHOD and ENDMETHOD.
    /// The method can have parameters. The parameters are separated by comma.
    /// </remarks>
    /// <example>
    /// METHOD DrawLine(x1, y1, x2, y2)
    ///    MOVE x1 y1
    ///    DRAW x2 y2
    /// ENDMETHOD
    /// </example>
    public bool SyntaxCheck(
        string[] commandParts,
        ref Dictionary<string, int> variables,
        ref Dictionary<string, string[]> methods,
        bool showError = true
    )
    {
        // The ENDMETHOD command should have 0 arguments
        if (commandParts.Length == 1 && commandParts[0] == "ENDMETHOD")
        {
            return true;
        }

		// The call to the method should have a valid method name
		if (commandParts.Length == 1 &&
            (!commandParts[0].Contains('(') || !commandParts[0].Contains(')')
            || commandParts[0].IndexOf(")") != commandParts[0].Length - 1
            ))
		{
			string errorMessage = "Syntax error: Invalid method name. The method name should be in the format: methodName(<parameter list>).";
			if (showError)
			MessageBox.Show(errorMessage, "Syntax error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		if (commandParts.Length == 1)
		{
			// The call to the method should have a valid method name
			string methodCallSingleName = commandParts[0][..commandParts[0].IndexOf("(")].Trim();
			if (methodCallSingleName == "" || int.TryParse(methodCallSingleName, out _) || VariableCommand.HasInvalidWord(methodCallSingleName)
                || !methods.ContainsKey(methodCallSingleName)
            )
			{
				string errorMessage = "Syntax error: Invalid undefined method. The method name should be in the format: methodName(<parameter list>).";
				if (showError)
				MessageBox.Show(errorMessage, "Syntax error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			// The call to the method should have a valid parameters which can also be numbers
            // substring from the first ( to the the length of the string - 1 (since it is checked that ) is the last character)
            int openCallBracketIndex = commandParts[0].IndexOf("(");
            int closeCallBracketIndex = commandParts[0].IndexOf(")");
            string parameterCallList = commandParts[0].Substring(openCallBracketIndex + 1, closeCallBracketIndex - openCallBracketIndex - 1);
            if (parameterCallList.Length > 0 && parameterCallList != "") {
                string[] parametersCall = parameterCallList.Split(',');
                foreach (string parameter in parametersCall)
                {
                    string parameterName = parameter.Trim();
                    if (parameterName == "" || VariableCommand.HasInvalidWord(parameterName))
                    {
                        string errorMessage = "Syntax error: Invalid parameter list. The parameter list should be in the format: methodName(<parameter list>).";
                        if (showError)
                        MessageBox.Show(errorMessage, "Syntax error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                // The call to the method should have a valid parameters which can also be numbers
                foreach (string parameter in parametersCall)
                {
                    string parameterName = parameter.Trim();
                    if (!int.TryParse(parameterName, out _) && !variables.ContainsKey(parameterName))
                    {
                        string errorMessage = "Syntax error: Invalid parameter list. The variable '" + parameterName + "' is not defined.";
                        if (showError)
                        MessageBox.Show(errorMessage, "Syntax error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                // check if the method defined has parameters and if the call to the method has the same number of parameters
                string definedParameterList = methods[methodCallSingleName][0];
                string[] definedParameters = definedParameterList.Split(',');
                if (definedParameters.Length != parametersCall.Length) {
                    string errorMessage = "Syntax error: Invalid parameter list. The method '" + methodCallSingleName + "' should have " + definedParameters.Length + " parameters.";
                    if (showError)
                    MessageBox.Show(errorMessage, "Syntax error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            } else {
                // check if the method defined has parameters
                string definedParameterList = methods[methodCallSingleName][0];
                if (definedParameterList.Length > 0 && definedParameterList != "") {
                    string errorMessage = "Syntax error: Invalid parameter list. The method '" + methodCallSingleName + "' should expects " + definedParameterList.Split(',').Length + " parameters.";
                    if (showError)
                    MessageBox.Show(errorMessage, "Syntax error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
		}

		// if all the checks for the call to the method passed, return true
		if (commandParts.Length == 1)
		{
			return true;
		}

        // The METHOD command should have 1 argument: methodName(<parameter list>)
        if (commandParts.Length != 2)
        {
            string errorMessage = "Syntax error: The METHOD command should have 1 argument: methodName(<parameter list>).";
            if (showError)
            MessageBox.Show(errorMessage, "Syntax error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // The METHOD command should have a unique method name
        string methodName = commandParts[1];
        if (methods.ContainsKey(methodName))
        {
            string errorMessage = "Syntax error: The method name should be unique. The method name '" + methodName + "' is already used.";
            if (showError)
            MessageBox.Show(errorMessage, "Syntax error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // The METHOD command should have a valid method name
        string methodSingleName = methodName[..methodName.IndexOf("(")].Trim();
        if (methodSingleName == "" || int.TryParse(methodSingleName, out _) || VariableCommand.HasInvalidWord(methodSingleName))
        {
            string errorMessage = "Syntax error: Invalid method name. The method name should be in the format: methodName(<parameter list>).";
            if (showError)
            MessageBox.Show(errorMessage, "Syntax error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // The METHOD command should have a valid method name
        if (commandParts.Length == 2 &&
            (!commandParts[1].Contains('(') || !commandParts[1].Contains(')')
            || commandParts[1].IndexOf(")") != commandParts[1].Length - 1
            ))
        {
            string errorMessage = "Syntax error: Invalid method name. The method name should be in the format: methodName(<parameter list>).";
            if (showError)
            MessageBox.Show(errorMessage, "Syntax error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // The METHOD command should have a valid parameter list
        int openBracketIndex = commandParts[1].IndexOf("(");
        int closeBracketIndex = commandParts[1].IndexOf(")");
        string parameterList = commandParts[1].Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
        if (parameterList.Length > 0 && parameterList != "") {
            string[] parameters = parameterList.Split(',');

            foreach (string parameter in parameters)
            {
                string parameterName = parameter.Trim();
                if (parameterName == "" || int.TryParse(parameterName, out _) || VariableCommand.HasInvalidWord(parameterName))
                {
                    string errorMessage = "Syntax error: Invalid parameter list. The parameter list should be in the format: methodName(<parameter list>).";
                    if (showError)
                    MessageBox.Show(errorMessage, "Syntax error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

		return true;
    }

    /// <summary>
    /// Executes the METHOD command.
    /// </summary>
    /// <param name="commandParts">An array of command parts.</param>
    /// <param name="variables">A dictionary of variables.</param>
    /// <param name="methods">A dictionary of methods.</param>
	/// <param name="isExecutingSpecialCommandStack">A Stack of flags that indicates if a special command is being executed.</param>
    /// <param name="specialCommandsStack">A stack of special commands.</param>
    /// <param name="currentLineIndex">The current line index.</param>
    public void Execute(
        string[] commandParts,
        ref Dictionary<string, int> variables,
        ref Dictionary<string, string[]> methods,
        ref Stack<bool> isExecutingSpecialCommandStack,
        ref Stack<string> specialCommandsStack,
        ref int currentLineIndex
    )
    {
        // methodName(<parameter list>), [startLine, endLine] is added to the methods dictionary
        // The ENDMETHOD command should end the stop the isExecutingSpecialCommand flag and pop the specialCommandsStack
        // It also stores the endLine of the method
        if (commandParts.Length == 1 && commandParts[0] == "ENDMETHOD")
        {
            // check if stack is empty
            if (isExecutingSpecialCommandStack.Count == 0 || specialCommandsStack.Count == 0)
            {
                return;
            }

            if (isExecutingSpecialCommandStack.Peek() && specialCommandsStack.Peek() == "METHOD")
            {
                isExecutingSpecialCommandStack.Pop();
                specialCommandsStack.Pop();
                string endMethodName = storingMethodsStack.Pop();
                methods[endMethodName][2] = currentLineIndex.ToString();
            }

            // if the method was running, pop the runningMethodsStack
            if (runningMethodsStack.Count > 0) {
                // pop the specialCommandsStack
                specialCommandsStack.Pop();
                // pop the isExecutingSpecialCommandStack
                isExecutingSpecialCommandStack.Pop();

                // remove the temporary variables
                string[] parameters = methods[runningMethodsStack.Peek()[0]][0].Split(',');
                foreach (string parameter in parameters)
                {
                    string parameterName = parameter.Trim();
                    variables.Remove(parameterName + "_METHOD");
                }
                
                // set the currentLineIndex to the calling line
				currentLineIndex = int.Parse(runningMethodsStack.Peek()[1]);
                runningMethodsStack.Pop();
            }
            return;
        }

        // If executing a special command, skip the code between METHOD and ENDMETHOD
        if (isExecutingSpecialCommandStack.Peek() && specialCommandsStack.Peek() == "METHOD")
            return;

        // check if the syntax is valid
        if (!SyntaxCheck(commandParts, ref variables, ref methods))
            return;

		// If its a method call, execute the method
		if (commandParts.Length == 1)
		{
            // get the method name
            string methodCallName = commandParts[0].Substring(0, commandParts[0].IndexOf("("));

			// store the currentLineIndex and the method name
			runningMethodsStack.Push(new string[] { methodCallName, currentLineIndex.ToString() });

			// jump to method start line
			int methodStartLine = int.Parse(methods[methodCallName][1]);
			currentLineIndex = methodStartLine;

            // put the method in the specialCommandsStack
            specialCommandsStack.Push("METHOD");

            // put the isExecutingSpecialCommand flag, false because it is a method call
            isExecutingSpecialCommandStack.Push(false);

            // put the parameters in the variables dictionary as a temporary variable
            string parameterCallList = commandParts[0].Substring(commandParts[0].IndexOf("(") + 1, commandParts[0].IndexOf(")") - commandParts[0].IndexOf("(") - 1);
            if (parameterCallList.Length > 0 && parameterCallList != "") {
                string[] parametersCall = parameterCallList.Split(',');
                string[] parameters = methods[methodCallName][0].Split(',');
                for (int i = 0; i < parametersCall.Length; i++)
                {
                    string parameterName = parameters[i].Trim();
                    string parameterValue = parametersCall[i].Trim();
                    if (int.TryParse(parameterValue, out _)) {
                        variables.Add(parameterName + "_METHOD", int.Parse(parameterValue));
                    } else {
                        variables.Add(parameterName + "_METHOD", variables[parameterValue]);
                    }
                }
            }

			return;
		}
        
        // methodName(<parameter list>), [startLine, endLine] is added to the methods dictionary
        string methodName = commandParts[1].Substring(0, commandParts[1].IndexOf("("));

        // The method is already validated in the SyntaxCheck method
        methods.Add(methodName, new string[3]);

        // The parameter list is stored
        string parameterList = commandParts[1].Substring(commandParts[1].IndexOf("(") + 1, commandParts[1].IndexOf(")") - commandParts[1].IndexOf("(") - 1);
        if (parameterList.Length > 0 && parameterList != "") {
            methods[methodName][0] = parameterList;
        }

        // The startLine of the method is stored
        methods[methodName][1] = currentLineIndex.ToString();

        // The isExecutingSpecialCommand flag is set to true and the method is pushed to the storingMethodsStack
        isExecutingSpecialCommandStack.Push(true);
        specialCommandsStack.Push("METHOD");
        storingMethodsStack.Push(methodName);
    }

    public static bool IsMethodAssignment(string[] commandParts)
    {
        // if it is a valid method assignment, return true (e.g. METHOD methodName(x, y))
        return commandParts.Length == 2 && commandParts[0] == "METHOD" && commandParts[1].IndexOf("(") != -1 && commandParts[1].IndexOf(")") != -1 && commandParts[1].IndexOf("(") < commandParts[1].IndexOf(")");
    }

    public static bool IsMethodCall(string[] commandParts)
    {
        // if it is a valid method call, return true (e.g. methodName(x, y))
        return commandParts.Length == 1 && commandParts[0].IndexOf("(") != -1 && commandParts[0].IndexOf(")") != -1 && commandParts[0].IndexOf("(") < commandParts[0].IndexOf(")");
    }
}