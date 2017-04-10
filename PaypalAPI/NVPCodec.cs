namespace PaypalAPI
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Text;
    using System.Web;
        public sealed class NVPCodec : NameValueCollection
        {
            private const string AMPERSAND = "&";
            private static readonly char[] AMPERSAND_CHAR_ARRAY = "&".ToCharArray();
            private const string EQUALS = "=";
            private static readonly char[] EQUALS_CHAR_ARRAY = "=".ToCharArray();

            public void Add(string name, string value, int index)
            {
                this.Add(GetArrayName(index, name), value);
            }

            public void Decode(string nvpstring)
            {
                this.Clear();
                foreach (string str in nvpstring.Split(AMPERSAND_CHAR_ARRAY))
                {
                    string[] strArray = str.Split(EQUALS_CHAR_ARRAY);
                    if (strArray.Length >= 2)
                    {
                        string name = HttpUtility.UrlDecode(strArray[0]);
                        string str3 = HttpUtility.UrlDecode(strArray[1]);
                        this.Add(name, str3);
                    }
                }
            }

            public string Encode()
            {
                StringBuilder builder = new StringBuilder();
                bool flag = true;
                foreach (string str in this.AllKeys)
                {
                    string str2 = HttpUtility.UrlEncode(str);
                    string str3 = HttpUtility.UrlEncode(base[str]);
                    if (!flag)
                    {
                        builder.Append("&");
                    }
                    builder.Append(str2).Append("=").Append(str3);
                    flag = false;
                }
                return builder.ToString();
            }

            private static string GetArrayName(int index, string name)
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("index", "index can not be negative : " + index);
                }
                return (name + index);
            }

            public void Remove(string arrayName, int index)
            {
                this.Remove(GetArrayName(index, arrayName));
            }

            public string this[string name, int index]
            {
                get
                {
                    return this[GetArrayName(index, name)];
                }
                set
                {
                    this[GetArrayName(index, name)] = value;
                }
            }
        }
    }