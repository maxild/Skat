// TODO: should be ext method
public class FailureHelper 
{
    public static void ExceptionOnError(int exitCode, string errorMsg) 
    {
        if (exitCode != 0)
        {
            throw new System.Exception(errorMsg);   
        }
    }
}
