
public interface ISpecialCommand
{
    bool SyntaxCheck(
        string[] commandParts,
        ref Dictionary<string, int> variables,
        ref Dictionary<string, string[]> methods,
        bool showError = true
    );
    void Execute(
        string[] commandParts,
        ref Dictionary<string, int> variables,
        ref Dictionary<string, string[]> methods,
        ref Stack<bool> isExecutingSpecialCommandStack,
        ref Stack<string> specialCommandsStack,
        ref int currentLineIndex
    );
}
