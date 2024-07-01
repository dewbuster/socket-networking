using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Settings
    {
        public static Dictionary<string, string> settings = new Dictionary<string, string>();

        public void Set()
        {
            string filePath = "settings.ini";

            if (!File.Exists(filePath))
            {
                CreateSettingsFile(filePath);
            }

            foreach (string line in File.ReadLines(filePath))
            {
                string[] parts = line.Split(new char[] { '=' }, 2);

                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    settings[key] = value;
                }
            }
        }

        static void CreateSettingsFile(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("ipAddress=192.168.0.2");
                    writer.WriteLine("port=7777");
                    writer.WriteLine("number=777");
                }

                Console.WriteLine("settings.ini 파일이 생성되었습니다.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일 생성 오류: {ex.Message}");
            }
        }


        

    }
}
