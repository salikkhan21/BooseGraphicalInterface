using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Parses and executes commands for drawing shapes and graphics operations.
/// </summary>
public class CommandParser
{
    private Graphics graphics;
    private int x;
    private int y;
    private Color penColor = Color.Black;
    private bool fillShapes = false;
    private Dictionary<string, int> variables;
    private Dictionary<string, int> tempVariables = new Dictionary<string, int>();
    private Dictionary<string, string[]> methods;
    private Dictionary<string, string[]> tempMethods = new Dictionary<string, string[]>();
    private Dictionary<string, ICommand> commandDictionary;
    private Dictionary<string, ISpecialCommand> specialCommandsDictionary;
    private Stack<bool> isExecutingSpecialCommandStack = new Stack<bool>();
    // stack of special commands, first in last out
    private Stack<string> specialCommandsStack = new Stack<string>();

    /// <summary>
    /// Initializes a new instance of the CommandParser class.
    /// </summary>
    /// <param name="graphics">The Graphics object for drawing.</param>
    /// <param name="initialX">The initial X-coordinate.</param>
    /// <param name="initialY">The initial Y-coordinate.</param>
    public CommandParser(Graphics graphics, int initialX = 0, int initialY = 0)
    {
        this.graphics = graphics;
        this.x = initialX;
        this.y = initialY;
        this.variables = new Dictionary<string, int>();
        this.methods = new Dictionary<string, string[]>();

        // add an initial false value to the stack
        isExecutingSpecialCommandStack.Push(false);

        // Initialize the command dictionary with command names and their respective ICommand implementations
        commandDictionary = new Dictionary<string, ICommand>
        {
            { "MOVE", new MoveCommand() },
            { "DRAW", new DrawCommand() },
            { "CLEAR", new ClearCommand() },
            { "RESET", new ResetCommand() },
            { "RECTANGLE", new RectangleCommand() },
            { "CIRCLE", new CircleCommand() },
            { "TRIANGLE", new TriangleCommand() },
            { "COLOR", new ColorCommand() },
            { "FILL", new FillCommand() },
            { "WRITE", new WriteCommand() }
            // Can add entries for other commands
        };

        // Add the special commands
        specialCommandsDictionary = new Dictionary<string, ISpecialCommand>
        {
            { "VAR", new VariableCommand() },
            { "IF", new IfCommand() },
            { "WHILE", new WhileCommand() },
            { "METHOD", new MethodCommand() }
        };

        // Add the end command for each special command, same class
        specialCommandsDictionary.Add("ENDWHILE", specialCommandsDictionary["WHILE"]);
        specialCommandsDictionary.Add("ENDIF", specialCommandsDictionary["IF"]);
        specialCommandsDictionary.Add("ENDMETHOD", specialCommandsDictionary["METHOD"]);
    }

    /// <summary>
    /// Executes a single command based on the provided command text.
    /// </summary>
    /// <param name="commandText">The text of the command to execute.</param>
    /// <param name="i">The index of the command line in the program.</param>
    public void ExecuteCommand(string commandText, ref int i)
    {
        string[] parts = commandText.Split(' ');

        if (parts.Length == 0)
        {
            return; // Handle empty command
        }

        // First word is the command name
        string commandName = parts[0].ToUpper();

        // Check if the command is a variable assignment, reassignment or method call
        if (variables.ContainsKey(commandName) || VariableCommand.IsVariableAssignment(parts))
        {
            commandName = "VAR";
        }

        if (MethodCommand.IsMethodCall(parts))
        {
            commandName = "METHOD";
        }

        if (!isExecutingSpecialCommandStack.Peek())
        {
            // Parse the variables
            if (!VariableCommand.ParseVariables(ref parts, variables)) {
                return;
            }
        }

        // Check if a special command is being executed
        if (isExecutingSpecialCommandStack.Peek() || specialCommandsDictionary.ContainsKey(commandName))
        {
            commandName = isExecutingSpecialCommandStack.Peek() ? specialCommandsStack.Peek() : commandName;
            ISpecialCommand command = specialCommandsDictionary[commandName];


            // Syntax check on special commands is only required when not executing a special command
            // It will be handled by the special command itself
            if (!isExecutingSpecialCommandStack.Peek())
            {
                if (!command.SyntaxCheck(parts, ref variables, ref methods))
                    {
                        // Handle syntax error
                        MessageBox.Show("Syntax error: " + commandName + " command is not formatted correctly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
            }

            command.Execute(
                parts,
                ref variables,
                ref methods,
                ref isExecutingSpecialCommandStack,
                ref specialCommandsStack,
                ref i
            );

            return;
        }

        if (commandDictionary.ContainsKey(commandName))
        {
            ICommand command = commandDictionary[commandName];

            if (command.SyntaxCheck(parts))
            {
                command.Execute(parts, ref x, ref y, ref penColor, ref fillShapes, graphics);
            }
            else
            {
                // Handle syntax error
                MessageBox.Show("Syntax error: " + commandName + " command is not formatted correctly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            // Handle unsupported command
            MessageBox.Show("Error: Unsupported command - " + commandName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 
    public void ResetProgram()
    {
        graphics.Clear(Color.LightGray);
        x = 0;
        y = 0;
        penColor = Color.Black;
        fillShapes = false;
        isExecutingSpecialCommandStack.Clear();
        isExecutingSpecialCommandStack.Push(false);
        specialCommandsStack.Clear();
        variables.Clear();
        methods.Clear();
    }

    /// <summary>
    /// Executes a program consisting of multiple commands.
    /// </summary>
    /// <param name="program">The program to execute.</param>
    public void ExecuteProgram(string program)
    {
        string[] lines = program.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        int index = 0;
        while (index < lines.Length)
        {
            ExecuteCommand(lines[index], ref index);
            index++;
        }
    }

    /// <summary>
    /// Checks the syntax of a program consisting of multiple commands.
    /// </summary>
    /// <param name="program">The program to check.</param>
    public bool SyntaxCheckProgram(string program)
    {
        string[] lines = program.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        tempVariables.Clear();
        tempMethods.Clear();

        for (int i = 0; i < lines.Length; i++)
        {
            if (!SyntaxCheckLine(lines[i], i + 1))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks the syntax of a single command.
    /// </summary>
    /// <param name="line">The command to check.</param>
    /// <param name="lineNumber">The line number of the command.</param>
    public bool SyntaxCheckLine(string line, int lineNumber = 0)
    {
        // Syntax rules
        string[] validCommands = commandDictionary.Keys.ToArray();
        validCommands = validCommands.Concat(specialCommandsDictionary.Keys.ToArray()).ToArray();

        // Split the line into words
        string[] words = line.Split(' ');

        if (words.Length == 0)
        {
            // Empty line
            return true;
        }

        // Parse the variables
        if (!VariableCommand.ParseVariables(ref words, tempVariables)) {
            SyntaxErrorException(lineNumber, line, "Invalid variable.");
            return false;
        }

        if (VariableCommand.IsVariableAssignment(words)) {
            // Variable assignment
            tempVariables[words[0]] = 1;
            return true;
        }

        // Check if the command is a method Assignment
        if (MethodCommand.IsMethodAssignment(words))
        {
            // Method assignment
            tempMethods[words[1].Substring(0, words[1].IndexOf("("))] = new string[3];
            tempMethods[words[1].Substring(0, words[1].IndexOf("("))][0] = words[1].Substring(words[1].IndexOf("(") + 1, words[1].IndexOf(")") - words[1].IndexOf("(") - 1);
            
            // add temp variables from parameters
            string[] parameters = tempMethods[words[1].Substring(0, words[1].IndexOf("("))][0].Split(',');
            foreach (string parameter in parameters)
            {
                string parameterName = parameter.Trim();
                tempVariables.Add(parameterName + "_METHOD", 1);
            }
            
            return true;
        }

        // Check if the command is a method call
        if (MethodCommand.IsMethodCall(words))
        {
            // Method call
            if (!tempMethods.ContainsKey(words[0].Substring(0, words[0].IndexOf("("))))
            {
                SyntaxErrorException(lineNumber, line, "Invalid method call. Method " + words[0] + " is not defined.");
                return false;
            }

            // if (!tempMethods[words[0].Substring(0, words[0].IndexOf("("))][0].Length.Equals(words[0].Substring(words[0].IndexOf("(") + 1, words[0].IndexOf(")") - words[0].IndexOf("(") - 1)))
            // {
            //     SyntaxErrorException(lineNumber, line, "Invalid method parameter. Method " + words[0] + " expects " + tempMethods[words[0].Substring(0, words[0].IndexOf("("))][0] + " parameter(s).");
            //     return false;
            // }

            return true;
        }

        // Check if the first word is a valid command and it's in uppercase
        string firstWord = words[0].Trim();
        if (firstWord != firstWord.ToUpper())
        {
            // Invalid command
            SyntaxErrorException(lineNumber, line, "Invalid command: (command must be in uppercase, valid commands are: " + string.Join(", ", validCommands) + ", RUN)");
            return false;
        }

        if (specialCommandsDictionary.ContainsKey(firstWord))
        {
            ISpecialCommand command = specialCommandsDictionary[firstWord];

            if (command.SyntaxCheck(words, ref tempVariables, ref tempMethods, false))
            {
                return true;
            }
            else
            {
                SyntaxErrorException(lineNumber, line, "Syntax error in " + firstWord + " command.");
                return false;
            }
        }
        else if (commandDictionary.ContainsKey(firstWord))
        {
            ICommand command = commandDictionary[firstWord];

            if (command.SyntaxCheck(words))
            {
                return true;
            }
            else
            {
                SyntaxErrorException(lineNumber, line, "Syntax error in " + firstWord + " command.");
                return false;
            }
        }
        else
        {
            // Handle unsupported command
            SyntaxErrorException(lineNumber, line, "Unsupported command: " + firstWord);
            return false;
        }
    }

    /// <summary>
    /// Displays a syntax error message.
    /// </summary>
    /// <param name="line">The line number of the command.</param>
    /// <param name="command">The command text.</param>
    /// <param name="message">The error message.</param>
    private void SyntaxErrorException(int line, string command, string message = "")
    {
        MessageBox.Show("Syntax error at line " + line + ": " + command + "\n" + message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // utility functions to provide current settings of the resulting drawing
    public Point GetCurrentPosition()
    {
        return new Point(x, y);
    }

    public bool IsFillOn()
    {
        return fillShapes;
    }

    // return the current pen color as a string (e.g. "Black")
    public string GetCurrentColor()
    {
        return penColor.Name;
    }
}
