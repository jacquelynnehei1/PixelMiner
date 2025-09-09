using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFML.System;

namespace pixel_miner.Core
{
    public static class Time
    {
        public static float DeltaTime { get; private set; }
        public static float TotalTime { get; private set; }
        public static float CurrentFPS { get; private set; }

        private static Clock deltaClock = new Clock();
        private static Clock totalClock = new Clock();

        // FPS tracking
        private static int frameCount = 0;
        private static float fpsAccumulator = 0f;
        private static float fpsUpdateInterval = 1f;

        // Memory tracking
        private static float memoryCheckInterval = 5f;
        private static float memoryAccumulator = 0f;
        public static long CurrentMemoryMB { get; private set; }

        public static void Update()
        {
            DeltaTime = deltaClock.Restart().AsSeconds();
            TotalTime = totalClock.ElapsedTime.AsSeconds();

            UpdateFPS();

            UpdateMemoryUsage();
        }

        private static void UpdateFPS()
        {
            frameCount++;
            fpsAccumulator += DeltaTime;

            if (fpsAccumulator >= fpsUpdateInterval)
            {
                CurrentFPS = frameCount / fpsAccumulator;
                frameCount = 0;
                fpsAccumulator = 0;
            }
        }

        private static void UpdateMemoryUsage()
        {
            memoryAccumulator += DeltaTime;

            if (memoryAccumulator >= memoryCheckInterval)
            {
                CurrentMemoryMB = GC.GetTotalMemory(false) / (1024 * 1024);
                memoryAccumulator = 0f;
            }
        }

        public static void LogPerformance()
        {
            Console.WriteLine($"FPS: {CurrentFPS:F1} | Memory: {CurrentMemoryMB} MB | Total Time: {TotalTime:F1}s");
        }

        public static bool IsPerformanceGood()
        {
            return CurrentFPS > 55f && CurrentMemoryMB < 100;
        }
    }
}