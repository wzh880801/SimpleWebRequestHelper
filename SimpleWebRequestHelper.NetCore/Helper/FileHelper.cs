using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace SimpleWebRequestHelper.Helper
{
    public static class FileHelper
    {
        public static string GetFileMd5(FileInfo _file)
        {
            if (!_file.Exists)
                throw new FileNotFoundException("未找到文件", nameof(_file.FullName));

            using (FileStream file = new FileStream(_file.FullName, System.IO.FileMode.Open))
            {
                MD5 md5 = MD5.Create();
                byte[] retVal = md5.ComputeHash(file);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static string GetFileType(FileInfo _file)
        {
            switch (_file.Extension.ToLower())
            {
                case ".png":
                    return "image/png";
                case ".jpg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".bmp":
                    return "image/bmp";
                case ".csv":
                    return "application/vnd.ms-excel";
                case ".txt":
                    return "text/plain";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }

            return "text/plain";
        }

        public static string GetMediaType(FileInfo _file)
        {
            switch (_file.Extension.ToLower())
            {
                case ".png":
                    return "pic";
                case ".jpg":
                    return "pic";
                case ".gif":
                    return "pic";
                case ".bmp":
                    return "pic";
                case ".csv":
                    return "doc";
                case ".txt":
                    return "doc";
                case ".xlsx":
                    return "doc";
            }

            return "doc";
        }

        public static byte[] ReadBytes(FileInfo _file, long offset, long length)
        {
            if (!_file.Exists)
                throw new FileNotFoundException("未找到文件", nameof(_file.FullName));

            var _bytes = new List<byte>();

            using (FileStream file = new FileStream(_file.FullName, System.IO.FileMode.Open))
            {
                file.Position = offset;

                if (length <= 1024)
                {
                    var buffer = new byte[length];
                    file.Read(buffer, 0, (int)length);
                    _bytes.AddRange(buffer);
                }
                else
                {
                    long _left = length;

                    while (true)
                    {
                        var _buffer = new byte[_left >= 1024 ? 1024 : _left];
                        file.Read(_buffer, 0, _buffer.Length);
                        _bytes.AddRange(_buffer);
                        _left -= _buffer.Length;
                        if (_left <= 0)
                            break;
                        file.Position += _buffer.Length;
                    }
                }
            }

            return _bytes.ToArray();
        }

        //public static string GetFileContent(FileInfo _file)
        //{

        //}
    }
}