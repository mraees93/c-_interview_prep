//06_

public class Demo
{
    //compile-time enforcement
    private const int Seconds = 60;
    private const int Minutes = 60;

    object str = "hello"; //runtime enforcement, object is compile-time safe
    int brokenCast = (int)str; 
    // runtime checks heap metadata and crashes. System.InvalidCastException thrown at runtime

    public void Calculate()
    {
        // 3600 is calculated at compile-time - dotnet build
        int total = Seconds * Minutes;
        //runtime sees int total = 3600
    }
}