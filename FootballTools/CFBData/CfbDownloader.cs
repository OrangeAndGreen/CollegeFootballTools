﻿using FootballTools.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;

namespace FootballTools.CFBData
{
    public class CfbDownloader
    {
        private static readonly string CacheDirectory = "Cache";

        public static List<Game> Retrieve(int year, bool forceDownload = false)
        {
            //Download all games for the specified year
            List <Game> games = new List<Game>();

            for (int week = 1; week <= 15; week++)
            {
                try
                {
                    List<Game> weekGames = null;
                    if (!forceDownload)
                    {
                        weekGames = RetrieveWeekFromCache(year, week);
                    }

                    if (weekGames == null)
                    {
                        Console.WriteLine($"Downloading week {week}");
                        string url = $"https://api.collegefootballdata.com/games?year={year}&week={week}&seasonType=regular";
                        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                        request.AutomaticDecompression = DecompressionMethods.GZip;


                        //Get the data from the HTTP request
                        using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                        using (Stream stream = response.GetResponseStream())
                        {
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Game>));
                            weekGames = (List<Game>) serializer.ReadObject(stream);
                            CacheWeek(year, week, weekGames);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Retrieved {year}_{week} from cache");
                    }

                    if (weekGames?.Count > 0)
                    {
                        games.AddRange(weekGames);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
            }

            return games;
        }

        private static string GetCacheFilepath(int year, int week)
        {
            return Path.Combine(CacheDirectory, $"{year}_{week}.json");
        }

        private static List<Game> RetrieveWeekFromCache(int year, int week)
        {
            try
            {
                if (!Directory.Exists(CacheDirectory))
                {
                    return null;
                }

                string filepath = GetCacheFilepath(year, week);
                if (!File.Exists(filepath))
                {
                    return null;
                }

                using (Stream stream = new FileStream(filepath, FileMode.Open))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Game>));
                    return (List<Game>)serializer.ReadObject(stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception while trying to retrieve cache file {year}_{week}: {e.Message}");
                return null;
            }
        }

        private static void CacheWeek(int year, int week, List<Game> games)
        {
            try
            {
                if (!Directory.Exists(CacheDirectory))
                {
                    Directory.CreateDirectory(CacheDirectory);
                }

                string filepath = GetCacheFilepath(year, week);
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }

                using (Stream stream = new FileStream(filepath, FileMode.CreateNew))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Game>));
                    serializer.WriteObject(stream, games);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception while trying to write cache file {year}_{week}: {e.Message}");
            }
            
        }
    }
}