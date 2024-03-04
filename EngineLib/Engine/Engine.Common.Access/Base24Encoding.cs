//�ļ���Base24Encoding.cs
//���ߣ�Elvis.Wu
//���ڣ�2021-04-01
//�汾��1.0
namespace System.Text
{
    /// <summary>
    /// ��ʾ base 24 ����
    /// </summary>
    public class Base24Encoding
    {
        /// <summary>
        /// ��ʾ base 24 �����Ĭ��ʵ��ʹ�õ��ַ�ӳ���BCDFGHJKMPQRTVWXY2346789
        /// </summary>
        public const string DefaultMap = "BCDFGHJKMPQRTVWXY2346789";

        private static Base24Encoding defaultInstance;

        private string map;

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public Base24Encoding()
        {
            this.map = DefaultMap;
        }

        /// <summary>
        /// ��ȡ base 24 �����Ĭ��ʵ��
        /// </summary>
        public static Base24Encoding Default
        {
            get
            {
                if (defaultInstance == null)
                    defaultInstance = new Base24Encoding();
                return defaultInstance;
            }
        }

        /// <summary>
        /// ��ȡ�����ý��� base 24 ������ַ�ӳ���
        /// </summary>
        /// <exception cref="ArgumentException">map�����Ϲ淶������24�������ظ��ַ�</exception>
        public string Map
        {
            get { return this.map; }
            set
            {
                if (value == null || value.Length != 24)
                {
                    throw new ArgumentException("map�����ǳ���24���ַ���");
                }
                for (byte i = 1; i < 24; i++)
                {
                    for (byte j = 0; j < i; j++)
                    {
                        if (value[i] == value[j])
                        {
                            throw new ArgumentException("map�в��ܺ����ظ��ַ�");
                        }
                    }
                }
                this.map = value;
            }
        }

        /// <summary>
        /// �� 8 λ�޷������������ֵת��Ϊ���ĵ�Ч System.String ��ʾ��ʽ��ʹ�� base 24 ���ֱ��룩��
        /// </summary>
        /// <param name="bytes">һ�� 8 λ�޷����������顣</param>
        /// <returns>bytes ���ݵ� System.String ��ʾ��ʽ���Ի���Ϊ 24 �����ֱ�ʾ��</returns>
        /// <exception cref="ArgumentNullException">bytes Ϊ null��</exception>
        public string GetString(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            string text = string.Empty;
            for (int i = bytes.Length - 1; i >= 0; i -= 4)
            {
                uint data = 0;
                for (int j = i < 3 ? i : 3; j >= 0; j--)
                {
                    data = (data << 8) + bytes[i - j];
                }

                string t = string.Empty;
                while (data > 0)
                {
                    int d = (int)(data % 24);
                    t = map[d] + t;
                    data = data / 24;
                }
                text = t.PadLeft(7, map[0]) + text;
            }
            return text;
        }

        /// <summary>
        /// ��ָ���� System.String���������������ݱ���Ϊ base 24 ���֣�ת���ɵ�Ч�� 8 λ�޷����������顣
        /// </summary>
        /// <param name="text">System.String��</param>
        /// <returns>��Ч�� text �� 8 λ�޷����������顣</returns>
        /// <exception cref="ArgumentNullException">text Ϊ null��</exception>
        /// <exception cref="FormatException">text ������ base 24 �ַ� �� text ��������ת���� base 24 �ַ�����</exception>
        public byte[] GetBytes(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            int len = text.Length / 7 + (text.Length % 7 == 0 ? 0 : 1);
            byte[] bytes = new byte[len * 4];
            int pos = bytes.Length - 1;
            for (int i = text.Length - 1; i >= 0; i -= 7, pos -= 4)
            {
                uint data = 0;
                for (int j = i < 6 ? i : 6; j >= 0; j--)
                {
                    int d = map.IndexOf(text[i - j]);
                    if (d == -1)
                    {
                        throw new FormatException("text ������ base 24 �ַ�");
                    }
                    try
                    {
                        data = checked(data * 24 + (uint)d);
                    }
                    catch (OverflowException)
                    {
                        throw new FormatException("text ��������ת���� base 24 �ַ�����");
                    }
                }

                byte[] t = BitConverter.GetBytes(data);
                for (int j = 0; j < 4; j++)
                {
                    bytes[pos - j] = t[j];
                }
            }
            return bytes;
        }

        /// <summary>
        /// �ַ�������
        /// </summary>
        /// <param name="strSrc"></param>
        /// <returns></returns>
        public string ToBase24String(string strSrc,bool OutputNetual = true)
        {
            byte[] data = UTF8Encoding.Default.GetBytes(strSrc);
            string text = Base24Encoding.Default.GetString(data);
            text = text.TrimStart(Base24Encoding.DefaultMap[0]);
            text = text.PadLeft(25, Base24Encoding.DefaultMap[0]);
            for (int i = text.Length - 5; i > 0; i -= 5)
                text = text.Insert(i, "-");
            return text;

        }

        /// <summary>
        /// �ַ�������
        /// </summary>
        public string FromBase24String(string strSrc)
        {
            try
            {
                string str = strSrc.Replace("-", "").Replace(" ","") ;
                byte[] data = Base24Encoding.Default.GetBytes(str);
                string text = UTF8Encoding.Default.GetString(data);
                return text.TrimStart('\0');
            }
            catch (Exception )
            {
                return string.Empty;
            }
        }
    }
}
