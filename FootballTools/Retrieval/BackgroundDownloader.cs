using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FootballTools.Entities;

namespace FootballTools.Retrieval
{
    /// <summary>
    /// Works in the background to download and cache data
    /// </summary>
    public class BackgroundDownloader
    {
        private static readonly int EarliestYear = 1950;
        private static readonly int ThrottleDelayMs = 1000;

        private Thread mThread;
        private readonly ConcurrentQueue<DownloaderJob> mQueue;
        private bool mQuitter = false;

        private static BackgroundDownloader mSingleton = null;

        public static void Start()
        {
            if (mSingleton == null)
            {
                mSingleton = new BackgroundDownloader();
            }
        }

        public static void Shutdown()
        {
            if (mSingleton != null)
            {
                mSingleton.mQuitter = true;

                mSingleton.mThread.Join();
                mSingleton.mThread = null;

                mSingleton = null;
            }
        }

        private BackgroundDownloader()
        {
            mQueue = new ConcurrentQueue<DownloaderJob>();

            mThread = new Thread(WorkerLoop);
            mThread.Start();
        }

        private void WorkerLoop()
        {
            foreach (DownloaderJob job in CreateJobs())
            {
                mQueue.Enqueue(job);
            }

            mQuitter = false;
            while (!mQuitter)
            {
                if (mQueue.TryDequeue(out DownloaderJob job))
                {
                    bool doSleep = false;
                    switch (job.Type)
                    {
                        case DownloadType.SeasonGames:
                            if (!CacheHelper.IsItemInCache($"Games_{job.Year}"))
                            {
                                doSleep = true;
                                CfbDownloader.RetrieveSeasonGameList(job.Year);
                            }
                            break;
                        case DownloadType.WeekGames:
                            if (!CacheHelper.IsItemInCache($"Games_{job.Year}_{job.Week}"))
                            {
                                doSleep = true;
                                CfbDownloader.RetrieveWeeklyGameList(job.Year, job.Week);
                            }
                            break;
                        case DownloadType.SeasonPlays:
                            if (!CacheHelper.IsItemInCache($"Plays_{job.Year}"))
                            {
                                doSleep = true;
                                CfbDownloader.RetrieveSeasonPlayList(job.Year);
                            }
                            break;
                    }

                    if (doSleep)
                    {
                        Thread.Sleep(ThrottleDelayMs);
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }

        }

        /// <summary>
        /// Creates the initial jobs for each season
        /// </summary>
        /// <returns></returns>
        private static List<DownloaderJob> CreateJobs()
        {
            List<DownloaderJob> jobs = new List<DownloaderJob>();

            for (int year = DateTime.Now.Year; year >= EarliestYear; year--)
            {
                jobs.Add(new DownloaderJob {Type = DownloadType.SeasonGames, Year = year});
                jobs.Add(new DownloaderJob { Type = DownloadType.SeasonPlays, Year = year });
            }

            return jobs;
        }
    }
}
