using Dashboard.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dashboard
{
    public static class Utils
    {
        public static string[] EnumerateAllFiles(string folder) => Directory.EnumerateFiles(folder).ToArray();

        private static bool TryParseName(string fileName, out string id, out string name)
        {
            id = null;
            name = null;
            var match = Regex.Match(fileName, @"^\s*(\S*)\W+(.*?)\s*$");
            if (match.Success)
            {
                id = match.Groups[1].Value;
                name = match.Groups[2].Value;
                return true;
            }

            return false;
        }

        public static FaceRegitration[] ParseFileNames(string[] files)
        {
            var result = new List<FaceRegitration>();
            var jpgFiles = files.Where(x => string.Compare(Path.GetExtension(x), ".jpg", StringComparison.OrdinalIgnoreCase) == 0);
            foreach (var file in jpgFiles)
            {
                var nameOnly = Path.GetFileNameWithoutExtension(file);
                if (TryParseName(nameOnly, out var id, out var name))
                {
                    var reg = new FaceRegitration { Id = id, Name = name, FullPathToImage = file };
                    result.Add(reg);
                }
            }

            return result.ToArray();

        }

        public static (string[] allFiles, FaceRegitration[] validFiles) LoadFiles(string[] allFiles)
        {
            var valid = ParseFileNames(allFiles);
            return (allFiles, valid);


        }

        private const int exifOrientationID = 0x112; //274

        public static Image ExifRotate(this Image img)
        {
            if (!img.PropertyIdList.Contains(exifOrientationID))
                return img;

            var prop = img.GetPropertyItem(exifOrientationID);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
                img.RotateFlip(rot);

            return img;
        }
    }
}
