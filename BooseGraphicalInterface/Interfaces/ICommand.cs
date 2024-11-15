
public interface ICommand
{
    bool SyntaxCheck(string[] commandParts, bool showError = true);
    void Execute(string[] commandParts, ref int x, ref int y, ref Color penColor, ref bool fillShapes, Graphics graphics);
}
