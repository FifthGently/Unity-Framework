public class Singleton<T> where T: new()
{
	private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                //������������ʹ���һ��Tʵ���������ֵ���ͷ���ֵ��Ĭ��ֵ 
                _instance = (default(T) == null) ? System.Activator.CreateInstance<T>() : default(T);
            }

            return _instance;
        }
    }
}

