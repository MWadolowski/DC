namespace Interpreter
{
    public interface IProcessHandler
    {
        string StepName { get; }
        void Handle(ProcessMessage message, ulong tag);
    }
}
