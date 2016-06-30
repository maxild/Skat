namespace Maxfire.Skat
{
    public interface ITextValuePair<out TValue>
    {
        string Text { get; }
        TValue Value { get; }
    }
}
