/* Web Utilities *
 * Project: Augmented Furniture *
 * Author: Erik Podetti *
 * email: erik@realmore.es *
 * ============================== *
 * Collection of static methods that *
 * can be useful to realize common tasks
 * related to web communication */

using UnityEngine;
using System.Collections;
using System.Text;
using System;

namespace SocialPoint

{
	public static class WebUtils 
	{
		// Generate an MD5 hashed string from the string passed as argument
		public static string Md5Sum(string strToEncrypt)
		{
			System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
			byte[] bytes = ue.GetBytes(strToEncrypt);
			
			// encrypt bytes
			System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] hashBytes = md5.ComputeHash(bytes);
			
			// Convert the encrypted bytes back to a string (base 16)
			string hashString = "";
			
			for (int i = 0; i < hashBytes.Length; i++)
			{
				hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
			}
			
			return hashString.PadLeft(32, '0');
		}

		public static string UnescapeString(string escapedString)
		{
			return System.Text.RegularExpressions.Regex.Unescape(escapedString);
		}


		public static string DecodeUnicodeString(string inputString)
		{
			//UnicodeEncoding Unicode = new UnicodeEncoding();

			inputString = System.Text.RegularExpressions.Regex.Unescape(inputString);

			return inputString;

			/*byte[] FilterBytes = Unicode.GetBytes(inputString);
			string FilterASCII = System.Text.ASCIIEncoding.ASCII.GetString(FilterBytes);
			byte[] AsciiBytes = Unicode.GetBytes(FilterASCII);
			string sEncodedText = Convert.ToBase64String(AsciiBytes);
			
			byte[] FileTextBytes = Convert.FromBase64String(sEncodedText);
			string ASCIItext = Unicode.GetString(FileTextBytes);
			byte[] ASCIIBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(ASCIItext);
			string sDecodedText = Unicode.GetString(ASCIIBytes);


			return sDecodedText;*/
		}

	}
}
