namespace MVVMLightExtension_Tests
{
	public static class StringExtensions
	{
		public static string ReverseString(this string s)
		{
			char[] newString = new char[s.Length];
			for (int i = 0; i < s.Length; i++)
				newString[s.Length - 1 - i] = s[i];
			return new string(newString);
		}
	}
}
