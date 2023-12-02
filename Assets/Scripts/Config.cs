public class Config
{
    public static string getAnimationPath(string animationName)
    {
        return $"./Animation-{animationName}";
    }
    public static string getAnimationData(string animationName)
    {
        return $"{getAnimationPath(animationName)}/animData.data";
    }
    public static string getAnimationFrame(string animationName, int frame)
    {
        return $"{getAnimationPath(animationName)}/Frame {frame}.json";
    }
}