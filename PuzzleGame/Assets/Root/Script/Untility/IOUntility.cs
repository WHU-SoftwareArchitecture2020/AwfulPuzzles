using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// IO工具 用于访问文件
/// </summary>
public static class IOUntility {
    /// <summary>
    /// 创建文件夹    
    /// </summary>
    /// <param name="path">文件夹路径</param>
    public static void CreateFolder(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 创建文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="content">文件内容</param>
    public static void CreateFile(string filePath, string content) {
        //文件流
        StreamWriter writer;
        //判断文件目录是否存在
        //不存在则先创建目录
        Debug.Log(filePath);
        string folder = filePath.Substring(0, filePath.LastIndexOf("/"));
        CreateFolder(folder);
        //如果文件不存在则创建，存在则追加内容
        FileInfo file = new FileInfo(filePath);
        if (!file.Exists) {
            writer = file.CreateText();
        }
        else {
            file.Delete();
            writer = file.CreateText();
        }

        //写入内容
        writer.Write(content);
        writer.Close();
        writer.Dispose();
    }

    /// <summary>
    /// 判断文件是否存在
    /// </summary>
    /// <param name="path">文件路径</param>
    public static bool isFileExists(string path) {
        FileInfo file = new FileInfo(path);
        return file.Exists;
    }

	/// <summary>
	/// 删除文件
	/// </summary>
	/// <param name="fileName"></param>
    public static void DeleteFile(string fileName) {
        if (!File.Exists(fileName)) return;
        File.Delete(fileName);
    }
}