// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("ApMfQOZ56xcKO4lRIW+UaFovkAQ0d4ELoNPX7qpbLpGb9G4oznB2jXT7STvKUhTuKI+4ISpJ7wXH+1B9RJBgxZu6EB0cwoMMwsSn31+BpszWBlF3i0LUYNPJNkRbkDEqwCyA4ydn/v2h1DQPzD6UCsVmyG3/v1uMOeE0IfAjcswYWM3gKLzEtyZKtoDEVNd0REj9BXwIYap7xtU82AUWEkXvD785RGXA9azYNvds9WDOrl/UmfIY/xV/C7O+/3zWfproUAJkVaN/zU5tf0JJRmXJB8m4Qk5OTkpPTNOfTYjlebsuAMal/WBJ3NmQmUSqj86KS2fOAesf5cCmbyHRK9oJmhfNTkBPf81ORU3NTk5PiygRmnIaiBHy9PkpncNN8E1MTk9O");
        private static int[] order = new int[] { 4,3,5,12,9,11,10,8,10,11,11,13,13,13,14 };
        private static int key = 79;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
