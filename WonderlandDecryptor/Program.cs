using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderlandDecryptor
{
    public class Program
    {
        private static void PauseKeyPress()
        {
#if DEBUG
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#endif
        }

        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("WonderlandRenamer <Folder>");
                PauseKeyPress();
                Environment.Exit(1);
            }

            string path = args[0].Replace("\\", "/");
            string outputPath = Path.Combine(path, "Output").Replace("\\", "/");

            string[] dirs = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).ToArray();
            Console.WriteLine("Found {0} Files!", dirs.Length);

            foreach (string file in dirs)
            {
                try
                {
                    RenameFile(file.Replace("\\", "/"), path, outputPath);
                } catch (UnknownExtensionException ex)
                {
                    Console.WriteLine("Error: Unknown File Extension: " + ex.Message + " for " + file);
                }
            }

            Console.WriteLine("Done!");

            PauseKeyPress();

            Environment.Exit(0);
        }

        private static void RenameFile(string file, string path, string outputPath)
        {
            string localPath = file.Replace(path, "");
            if (localPath.StartsWith("/"))
                localPath = localPath.Substring(1);

            if (localPath.StartsWith("Data/Editor/"))
                return;

            //Console.WriteLine("Path: " + localPath);

            int lastIndex = localPath.LastIndexOf("/");
            string fileDir;
            if (lastIndex == -1)
                fileDir = "";
            else
                fileDir = localPath.Substring(0, lastIndex);

            if (fileDir == "Data")
                return;

            string fileName = Path.GetFileNameWithoutExtension(localPath);
            string extension = Path.GetExtension(localPath);

            string decryptedName = CryptString(fileName);
            string decryptedExtension = DetectExtension(file);

            if (decryptedExtension == extension || decryptedExtension == ".ogg")
                decryptedName = fileName;

            /*Console.WriteLine("Path: " + fileDir);
            Console.WriteLine("Encrypted Name: " + fileName);
            Console.WriteLine("Decrypted Name: " + decryptedName);
            Console.WriteLine("Encrypted Extension: " + extension);
            Console.WriteLine("Decrypted Extension: " + decryptedExtension);
            Console.WriteLine("------------------------");*/

            if (fileDir == "")
                Console.WriteLine("Processing: " + "<rootdir>" + " | " + fileName + extension + " | " + decryptedName + decryptedExtension);
            else
                Console.WriteLine("Processing: " + fileDir + " | " + fileName + extension + " | " + decryptedName + decryptedExtension);

            string outPath = Path.Combine(outputPath, fileDir);
            Directory.CreateDirectory(outPath);
            string outFile = Path.Combine(outPath, decryptedName + decryptedExtension).Replace("\\", "/");

            File.Copy(file, outFile, true);
        }

        private static string CryptString(string input, bool encrypt = false)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in input)
            {
                if (!char.IsLetter(c))
                {
                    sb.Append(c);
                    continue;
                }

                if (encrypt)
                {
                    char newChar = (char)((byte)c + (byte)1);
                    if (newChar == 0x7B)
                        newChar = 'a';
                    if (newChar == 0x5B)
                        newChar = 'A';
                    sb.Append(newChar);
                } else
                {
                    char newChar = (char)((byte)c - (byte)1);
                    if (newChar == 0x60)
                        newChar = 'z';
                    if (newChar == 0x40)
                        newChar = 'Z';
                    sb.Append(newChar);
                }
            }

            return sb.ToString();
        }

        private static string DetectExtension(string file)
        {
            string extension = Path.GetExtension(file);
            if (extension == ".wdf")
            {
                using (Stream s = File.OpenRead(file))
                using (BinaryReader br = new BinaryReader(s))
                {
                    byte[] bytes = br.ReadBytes(4);
                    if (bytes.Matches(new byte[] { 0xFF, 0xD8, 0xFF }, true))
                    {
                        return ".jpg";
                    } else if (bytes.Matches(new byte[] { 0x89, 0x50, 0x4E, 0x47 }))
                    {
                        return ".png";
                    } else if (bytes.Matches(new byte[] { 0x42, 0x4D }, true))
                    {
                        return ".bmp";
                    } else if (bytes.Matches(new byte[] { 0x52, 0x49, 0x46, 0x46 }))
                    {
                        return ".wav";
                    } else if (bytes.Matches(new byte[] { 0x4F, 0x67, 0x67, 0x53 }))
                    {
                        return ".ogg";
                    } else
                    {
                        throw new UnknownExtensionException(extension);
                    }
                }
            } else if (extension == ".wd1")
            {
                return ".b3d";
            }
            else if (extension == ".wd2")
            {
                return ".md2";
            }
            else if (extension == ".wd3")
            {
                return ".3ds";
            }

            return extension;
        }
    }
}
