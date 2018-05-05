public class Singleton<T> where T: new()
{
	private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                //如果是引用类型创建一个T实例，如果是值类型返回值的默认值 
                _instance = (default(T) == null) ? System.Activator.CreateInstance<T>() : default(T);
            }

            return _instance;
        }
    }
}

