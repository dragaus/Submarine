using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using UnityEngine;

public class Storage
{
    public static void SaveGameInfo()
    {
        var userData = Resources.Load<UserData>(GameInfo.userDataPath);
        var levelData = Resources.Load<LevelData>(GameInfo.levelDataPath);

        var gameDataToSave = new GameData()
        {
            currentLives = userData.currentGame.currentLives,
            currentScore = userData.currentGame.currentScore,
            currentTotalTime = userData.currentGame.currentTotalTime,
            isRepeatLevel = userData.currentGame.isRepeatLevel,
            totalTime = userData.generalData.totalTime,
            starsInLevels = levelData.starsInLevels,
            bestTime = levelData.bestTime,
            wins = levelData.wins,
            trys = levelData.trys,
            deads = levelData.deads,
        };

        Save(gameDataToSave, GameInfo.savingData);
    }

    static void Save(object obj, string direction)
    {
        var json = JsonUtility.ToJson(obj);
        var dirToSave = $"{Application.persistentDataPath}/{direction}.txt";

        var encrypted = AesEncryption.Encrypt(json, "galletita");
        var jsonToSave = JsonUtility.ToJson(encrypted);

        using (StreamWriter stream = new StreamWriter(dirToSave))
        {
            stream.Write(jsonToSave);
            stream.Close();
        }
    }

    public static string LoadJsonData(string direction)
    {
        var dirToRead = $"{Application.persistentDataPath}/{direction}.txt";

        using (StreamReader stream = new StreamReader(dirToRead))
        {
            var stringRecovered = stream.ReadToEnd();
            stream.Close();
            var jsonToRecover = JsonUtility.FromJson<AesEncryption.AESEncryptedText>(stringRecovered);

            return AesEncryption.Decrypt(jsonToRecover, "galletita");
        }

    }

    public static bool CanLoadData()
    {
        var savingPath = $"{Application.persistentDataPath}/{GameInfo.savingData}.txt";
        var userPath = $"{Application.persistentDataPath}/{GameInfo.userPath}.txt";
        return File.Exists(savingPath);
    }
}

class AesEncryption
{
    [Serializable]
    public struct AESEncryptedText
    {
        public string IV;
        public string EncryptedText;
    }

    public static AESEncryptedText Encrypt(string plainText, string password)
    {
        using (var aes = Aes.Create())
        {
            aes.GenerateIV();
            aes.Key = ConvertToKeyBytes(aes, password);

            var textBytes = Encoding.UTF8.GetBytes(plainText);
            var aesEncryptor = aes.CreateEncryptor();

            var encryptedBytes = aesEncryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

            return new AESEncryptedText
            {
                IV = Convert.ToBase64String(aes.IV),
                EncryptedText = Convert.ToBase64String(encryptedBytes)
            };
        }
    }

    public static string Decrypt(AESEncryptedText encryptedText, string password)
    {
        return Decrypt(encryptedText.EncryptedText, encryptedText.IV, password);
    }

    public static string Decrypt(string encryptedText, string iv, string password)
    {
        using (Aes aes = Aes.Create())
        {
            var ivBytes = Convert.FromBase64String(iv);
            var encryptedTextBytes = Convert.FromBase64String(encryptedText);

            var decryptor = aes.CreateDecryptor(ConvertToKeyBytes(aes, password), ivBytes);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedTextBytes, 0, encryptedTextBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }

    // Ensure the AES key byte-array is the right size - AES will reject it otherwise
    private static byte[] ConvertToKeyBytes(SymmetricAlgorithm algorithm, string password)
    {
        algorithm.GenerateKey();

        var keyBytes = Encoding.UTF8.GetBytes(password);
        var validKeySize = algorithm.Key.Length;

        if (keyBytes.Length != validKeySize)
        {
            var newKeyBytes = new byte[validKeySize];
            Array.Copy(keyBytes, newKeyBytes, Math.Min(keyBytes.Length, newKeyBytes.Length));
            keyBytes = newKeyBytes;
        }

        return keyBytes;
    }
}