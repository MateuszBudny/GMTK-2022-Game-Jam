using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
// a class borrowed from: https://github.com/UnityTechnologies/open-project-1

public static class FileManager
{
	public static bool WriteToFile(string fileName, string fileContents)
	{
		string fullPath = ConvertPath(fileName);

		if (fileName.Contains(Application.persistentDataPath))
		{
			fullPath = fileName;
		}
		else
		{
			fullPath = Path.Combine(Application.persistentDataPath, fileName);

		}
		
		try
		{
			File.WriteAllText(fullPath, fileContents);
			return true;
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to write to {fullPath} with exception {e}");
			return false;
		}
	}

	public static bool LoadFromFile(string fileName, out string result)
	{
		string fullPath = ConvertPath(fileName);

		if (!File.Exists(fullPath))
		{
			File.WriteAllText(fullPath, ""); 
		}
		try
		{
			result = File.ReadAllText(fullPath);
			return true;
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to read from {fullPath} with exception {e}");
			result = "";
			return false;
		}
	}

	public static bool MoveFile(string fileName, string newFileName)
	{
		string fullPath = ConvertPath(fileName);
		string newFullPath = ConvertPath(newFileName);

		try
		{
			if (!File.Exists(fullPath))
			{
				return false;
			}

			if (File.Exists(newFullPath))
			{
				File.Delete(newFullPath);
			}

			File.Move(fullPath, newFullPath);
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to move file from {fullPath} to {newFullPath} with exception {e}");
			return false;
		}

		return true;
	}

	public static bool CopyFile(string fileName, string newFileName)
	{
		var fullPath = Path.Combine(Application.persistentDataPath, fileName);
		var newFullPath = Path.Combine(Application.persistentDataPath, newFileName);

		try
		{

			if (!File.Exists(fullPath))
			{
				return false;
			}

			if (File.Exists(newFullPath))
			{
				File.Delete(newFullPath);
			}

			File.Copy(fullPath, newFullPath);
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to move file from {fullPath} to {newFullPath} with exception {e}");
			return false;
		}

		return true;
	}

	public static bool FileExists(string fileName){
		var fullPath = Path.Combine(Application.persistentDataPath, fileName);

		if(File.Exists(fullPath)){
			return true;
		}

		return false;
	}

	public static string[] ListFiles()
    {
		return Directory.GetFiles(Application.persistentDataPath);
    }

	public static bool DeleteFile(string fileName)
	{
		string fullPath = ConvertPath(fileName);
		try
		{
			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
			}
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to delete file {fullPath} with exception {e}");
			return false;
		}

		return true;
	}


	public static string ConvertPath(string path)
    {
		if (path.Contains(Application.persistentDataPath))
		{
			return path;
		}
		else
		{
			return Path.Combine(Application.persistentDataPath, path);

		}
	}

}
