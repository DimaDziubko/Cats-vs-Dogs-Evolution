// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("JNbWabXATnQchAhm0UMdy/Zkn43SBreaK21FpHM3oSMqQsgTCoGC7apFT9CQ83+fUz3gkr7R/fJEloomG6kqCRsmLSIBrWOt3CYqKiouKygrv4A+EIEs0j9Y/o5C05idh9yUYCQ4YIfqhhWYFZALzhaLJPXD2ic+tH7Jp7vU/2RvV6xBQlX+KiG2cFQBkgzpA/Tg2PusxB5htbQHooQBp9juysWSCbYiX49ypUlQhj6GW+CySt3xufY8CU/3wwaqgIzr6OU2JNxf6XbYuX6FGYsvf0Acj7dWmkTKvB0WZHhAPctwVt1fbFBU0Y0wIiLYnFX1uDR4aFMRYUUJNMBPeNAUj+ypKiQrG6kqISmpKioriW8Q5+czbU+uUPWPdX0v2ikoKisq");
        private static int[] order = new int[] { 8,12,13,8,5,5,10,8,11,9,13,11,13,13,14 };
        private static int key = 43;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
